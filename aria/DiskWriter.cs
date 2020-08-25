using System;
using System.Collections.Generic;
using System.Text;

namespace aria
{
    /// <summary>
    /// 二进制字节流的写接口
    /// </summary>
    class DiskWriter
    {
        /// <summary>
        /// Creates a file output stream to write to the file with the specified name.
        /// If the file exists, then it is truncated to 0 length.
        /// </summary>
        /// <param name="filename">the file name to be opened.</param>
        public virtual void initAndOpenFile(string filename) { return; }

        /// <summary>
        /// Closes this output stream.
        /// </summary>
        public virtual void closeFile() { return; }

        /// <summary>
        /// Opens a file output stream to write to the file with the specified name.
        /// If the file doesnot exists, an exception may be throwed.
        /// </summary>
        /// <param name="filename">the file name to be opened.</param>
        public virtual void openExistingFile(string filename) { return; }

        /// <summary>
        /// Writes len bytes from data to this binary stream at offset position.
        /// In case where offset position is not concerned(just write data
        /// sequencially, for example), those subclasses can ignore the offset value.
        /// </summary>
        /// <param name="data">the data</param>
        /// <param name="len">the number of bytes to write</param>
        /// <param name="position">the offset of this binary stream</param>
        public virtual void writeData(char data, int len, long position = 0) { return; }
    }
}
