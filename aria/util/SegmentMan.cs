using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using System.Threading;

namespace aria.util
{
    /// <summary>
    /// 这个类维护整个下载进程，并一直存在，直到下载结束
    /// </summary>
    class SegmentMan
    {
        #region 成员
        /// <summary>
        /// 需要下载的大小(字节)
        /// 如果编码内部没有提供该值，那么就将它设置为0
        /// </summary>
        public long totalSize;
        /// <summary>
        /// 表示此次下载是否可以断点续传
        /// </summary>
        public bool isSplittable;
        /// <summary>
        /// 表示此次下载是否开始,
        /// 默认值为 flase
        /// </summary>
        public bool downloadStarted;
        /// <summary>
        /// 保持块
        /// </summary>
        public List<Segment> segments;
        /// <summary>
        /// 下载文件的名字
        /// </summary>
        public string filename;
        /// <summary>
        /// 文件的存储路径
        /// </summary>
        public string dir;
        /// <summary>
        /// 用户自定义的文件名
        /// </summary>
        public string ufilename;
        public Logger logger;
        const string SEGMENT_FILE_EXTENSION = ".aria2"; 
        #endregion

        #region 属性
        public string FilePath
        {
            get
            {
                return (dir == "" ? "." : dir) + "/" + (ufilename == "" ? (filename == "" ? "index.html" : filename) : ufilename);
            }
        }

        public string SegmentFilePath
        {
            get
            {
                return FilePath + SEGMENT_FILE_EXTENSION;
            }
        }
        #endregion

        #region 方法

        public SegmentMan()
        {
            totalSize = 0;
            isSplittable = true;
            downloadStarted = false;
            dir = ".";
        }

        /// <summary>
        /// 指定Segment的cuid
        /// </summary>
        /// <param name="cuid"></param>
        public void UnregisterId(int cuid)
        {
            foreach(Segment itr in segments)
            {
                if (itr.cuid == cuid)
                    cuid = 0;
            }
        }

        /// <summary>
        /// There is a segment available for DownloadCommand specified by cuid,
        /// fills segment and returns true.
        /// There is no segment available, then returns false and segment is
        /// undefined in this case.
        /// </summary>
        /// <param name="seg">segment to attach for cuid.</param>
        /// <param name="cuid">cuid of DownloadCommand.</param>
        /// <returns>true: there is a segment available, false: there is no segment available.</returns>
        public bool getSegment(ref Segment seg, int cuid)
        {
            if(segments.Count==0)
            {
                logger.Debug("assign new segment { sp=0 , ep={0} to cuid {1} }", totalSize == 0 ? "0" : Util.Llitos(totalSize - 1), Util.Llitos(cuid));
                seg.cuid = cuid;
                seg.sp = seg.ds = 0;
                seg.ep = totalSize == 0 ? 0 : totalSize - 1;
                seg.finish = false;
                segments.Add(seg);
                return true;
            }
            foreach(Segment itr in segments)
            {
                if (itr.cuid==cuid&&!itr.finish)
                {
                    seg = itr;
                    return true;
                }
            }
            if (!isSplittable)
                return false;
            foreach (Segment itr in segments)
            {
                if (itr.finish)
                    continue;
                if (itr.cuid == 0)
                {
                    itr.cuid = cuid;
                    seg = itr;
                    return true;
                }      
            }
            foreach(Segment itr in segments)
            {
                if (itr.finish)
                    continue;
                if (itr.ep-(itr.sp+itr.ds)>524288)
                {
                    long nep = (itr.ep - (itr.sp + itr.ds)) / 2 + (itr.sp + itr.ds);
                    seg.cuid = cuid;
                    seg.sp = nep + 1;
                    seg.ep = itr.ep;
                    seg.finish = false;
                    itr.ep = nep;
                    logger.Debug("return new segment { sp = {0} , ep = {1} , ds = {2} } to cuid = {3}", 
                        Util.Llitos(seg.sp), 
                        Util.Llitos(seg.ep), 
                        Util.Llitos(seg.ds), 
                        Util.Llitos(cuid));
                    logger.Debug("update segment { sp = {0} , ep = {1} , ds = {2} } of cuid = {3}",
                        Util.Llitos(seg.sp),
                        Util.Llitos(seg.ep),
                        Util.Llitos(seg.ds),
                        Util.Llitos(cuid));
                    segments.Add(seg);
                    return true;
                }
            }
            return false;
        }

        // TODO : 此处可能重写
        public void UpdateSegment(Segment segment)
        {
            foreach(Segment s in segments)
            {
                int index;
                if (s.cuid == segment.cuid && s.sp == segment.sp && s.ep == segment.ep)
                {
                    index = segments.IndexOf(s);
                    break;
                }
            }            
        }

        public bool SegmentFileExists()
        {
            if (!isSplittable)
                return false;
            string segFilename = this.SegmentFilePath;
            if(File.Exists(segFilename))
            {
                logger.Info(Message.MSG_SEGMENT_FILE_EXISTS, segFilename);
                return true;
            }
            else
            {
                logger.Info(Message.MSG_SEGMENT_FILE_DOES_NOT_EXIST, segFilename);
                return false;
            }
        }

        private FileStream openSegFile(string segFilename,string mode)
        {
            FileStream segFile;
            switch (mode)
            {
                case "r+":
                    try
                    {
                        File.OpenRead(segFilename);
                    }
                    catch(FileNotFoundException)
                    {
                        throw new FileNotFoundException();
                    }
                    segFile = File.OpenRead(segFilename);
                    return segFile;
                case "w":
                    try
                    {
                        File.OpenWrite(segFilename);
                    }
                    catch(DirectoryNotFoundException)
                    {
                        throw new DirectoryNotFoundException();
                    }
                    segFile = File.OpenWrite(segFilename);
                    return segFile;
                default:
                    return null;
            }
        }

        // TODO : 关联 Read()
        public void Load()
        {
            if (!isSplittable)
                return;
            string segFilename = this.SegmentFilePath;
            logger.Info(Message.MSG_LOADING_SEGMENT_FILE, segFilename);
            FileStream segFile = openSegFile(segFilename, "r+");
        }

        /// <summary>
        /// 以二进制流的形式存储对象
        /// </summary>
        public void Save()
        {
            if (!isSplittable)
                return;
            logger.Info(Message.MSG_SAVING_SEGMENT_FILE, SegmentFilePath);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream segFile = new FileStream(SegmentFilePath, FileMode.Create);
            try
            {
                binaryFormatter.Serialize(segFile, segments);
            }
            catch (SerializationException)
            {
                throw new SerializationException();
            }
            segFile.Close();
            logger.Info(Message.MSG_SAVED_SEGMENT_FILE);
        }

        /// <summary>
        /// 读取整个对象的二进制流并在程序中反序列化
        /// </summary>
        /// <param name="fileStream">指定文件的路径</param>
        public void Read(ref FileStream fileStream)
        {
            if (fileStream == null)
                logger.Error("file stream is null!");
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                segments = (List<Segment>)binaryFormatter.Deserialize(fileStream);
            }
            catch(SerializationException)
            {
                throw new SerializationException();
            }
        }

        public void Remove()
        {
            if (!isSplittable)
                return;
            if (SegmentFileExists())
            {
                File.Delete(this.SegmentFilePath);
            }
        }

        public bool Finished()
        {
            if (!downloadStarted || segments.Count == 0)
                return false;
            foreach (Segment s in segments)
            {
                if (!s.finish)
                    return false;
            }
            return true;
        }

        public void RemoveIfFinished()
        {
            if (Finished())
                Remove();
        }

        public long GetDownLoadedSize()
        {
            long size = 0;
            foreach (Segment s in segments)
                size += s.ds;
            return size;
        }

        #endregion

    }
}
