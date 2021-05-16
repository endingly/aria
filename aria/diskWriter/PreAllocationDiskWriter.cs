using System;
using System.Collections.Generic;
using System.Text;

namespace aria.diskWriter
{
    class PreAllocationDiskWriter : AbstractDiskWriter
    {
        private int _size;

        public PreAllocationDiskWriter()
        {
            _size = 0;
        }

        public PreAllocationDiskWriter(int size)
        {
            _size = size;
        }

        public void InitAndOpenFile(string filename)
        {
            CreateFile(filename);
            char[] buf = new char[4096];
            var input = Encoding.UTF8.GetBytes(buf);
            try
            {
                openExistingFile(filename);
                fd.Write(input, 0, input.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }
        }

        public void WriteData(char[] str, int len, long offset)
        {
            var index = fd.Seek(offset, System.IO.SeekOrigin.Begin);
            // TODO: 检查 index 的位置并写入
            WriteDataInternal(str, len);
        }


    }
}
