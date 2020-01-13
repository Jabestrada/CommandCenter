using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Tests.Commands {
    [TestClass]
    public class Zip7CompressCommandTests {
        [TestMethod]
        public void itShouldDeleteTargetZipOnUndoIfCommandSucceeded() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void itShouldNotDeleteTargetZipOnUndoIfCommandFailed() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void itShouldFailIfTargetZipAlreadyExists() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void itShouldInterpretZeroFromFileCompressionStrategyAsSuccess() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void itShouldInterpretNonZeroFromFileCompressionStrategyAsFailure() {
            throw new NotImplementedException();
        }
    }
}
