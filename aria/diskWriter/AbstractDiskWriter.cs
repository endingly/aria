using aria.exception;
using System.IO;
using aria.util;
using System.Text;

namespace aria.diskWriter
{
    class AbstractDiskWriter : DiskWriter
    {
        protected FileStream fd;

        ~AbstractDiskWriter()
        {
            if (fd != null)
            {
                fd.Close();
            }
        }

        public void OpenExistingFile(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new DlAbortEx(Message.EX_FILE_PATH_NOT_EXISTS);
            }

            if (!(fd = File.Open(filename,FileMode.Open)).CanWrite)
            {
                throw new DlAbortEx("File not support write!");
            }
        }

        public void CloseFile()
        {
            if (fd != null)
            {
                fd.Close();
                fd = null;
            }
        }

        protected void CreateFile(string filename)
        {
            // TODO proper filename handling needed
            try
            {
                File.Create(filename);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        protected void WriteDataInternal(char[] str, int len)
        {
            fd.Write(Encoding.UTF8.GetBytes(str), 0, len);
        }

    }
}
