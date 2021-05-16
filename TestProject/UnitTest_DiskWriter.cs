using System;
using Xunit;
using aria;
using aria.diskWriter;
using System.Text;

namespace TestProject
{
    public class UnitTest_DiskWriter
    {
        [Fact]
        public void Test_WriteData()
        {
            char[] vs = new char[4096];
            var index = Encoding.UTF8.GetBytes(vs);
            
        }
    }
}
