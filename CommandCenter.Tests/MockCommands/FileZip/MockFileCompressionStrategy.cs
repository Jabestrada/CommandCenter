using CommandCenter.Commands.FileZip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Tests.MockCommands.FileZip {
    public class MockFileCompressionStrategy : IFileCompressionStrategy {
        private int _returnVal = 0;

        public MockFileCompressionStrategy(int returnVal = 0) {
            _returnVal = returnVal;
        }
        public void setReturnVal(int returnVal) {
            _returnVal = returnVal;
        }

        public int DoCompression(string targetZipfilename, params string[] sourcesToZip) {
            return _returnVal;
        }
    }
}
