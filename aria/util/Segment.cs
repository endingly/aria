using System;
using System.Collections.Generic;
using System.Text;

namespace aria.util
{
    /// <summary>
    /// Segment represents a download segment.
    /// sp, ep is a offset from a begining of a file.
    /// Therefore, if a file size is x, then 0 <= sp <= ep <= x-1.
    /// sp, ep is used in Http Range header.
    /// e.g. Range: bytes=sp-ep
    /// ds is downloaded bytes.
    /// If a download of this segement is complete, finish must be set to true.
    /// </summary>
    [Serializable]
    class Segment
    {
        public int cuid;
        public long sp;
        public long ep;
        public long ds;
        public bool finish;

        public static bool Equals(Segment X, Segment Y)
        {
            return (X.cuid == Y.cuid && X.sp == Y.sp && X.ep == Y.ep && X.ds == Y.ds && X.finish == Y.finish);
        }
    }
}
