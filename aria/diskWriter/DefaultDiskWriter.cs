using System;
using System.Collections.Generic;
using System.Text;

namespace aria.diskWriter
{
    class DefaultDiskWriter : AbstractDiskWriter
    {
        public void InitAndOpenFile(string filename)
        {
            CreateFile(filename);
        }

        public void WriteData(char[] str, int len, long offset)
        {
            var index = fd.Seek(offset, System.IO.SeekOrigin.Begin);
            // TODO: 检查 index 的位置并写入
            WriteDataInternal(str, len);
        }

    }
}
