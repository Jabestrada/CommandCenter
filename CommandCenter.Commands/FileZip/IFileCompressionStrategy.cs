using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Commands.FileZip {
    public interface IFileCompressionStrategy {
        int DoCompression(string targetZipfilename, params string[] sourcesToZip);
    }
}
