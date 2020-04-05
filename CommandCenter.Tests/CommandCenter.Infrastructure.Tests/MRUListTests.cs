using CommandCenter.Infrastructure.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandCenter.Infrastructure.Tests {
    [TestClass]
    public class MRUListTests {
        [TestMethod]
        public void itShouldAddItemsByFIFO() {
            var mruList = new MRUList<string>();
            mruList.AddItem("a");
            mruList.AddItem("b");
            mruList.AddItem("c");
            mruList.AddItem("d");

            Assert.AreEqual(4, mruList.Items.Count);
            Assert.AreEqual("d", mruList.Items[0]);
            Assert.AreEqual("c", mruList.Items[1]);
            Assert.AreEqual("b", mruList.Items[2]);
            Assert.AreEqual("a", mruList.Items[3]);
        }

        [TestMethod]
        public void itShouldLimitByMaxItemsWhenGiven() {
            var maxItems = 3;
            var mruList = new MRUList<string>(maxItems);
            mruList.AddItem("a");
            mruList.AddItem("b");
            mruList.AddItem("c");
            mruList.AddItem("d");

            Assert.AreEqual(maxItems, mruList.Items.Count);
            Assert.AreEqual("d", mruList.Items[0]);
            Assert.AreEqual("c", mruList.Items[1]);
            Assert.AreEqual("b", mruList.Items[2]);
        }

        [TestMethod]
        public void itShouldPutExistingItemToFirstSlotWhenAddedAgain() {
            var mruList = new MRUList<string>();
            mruList.AddItem("a");
            mruList.AddItem("b");
            mruList.AddItem("c");
            mruList.AddItem("a");

            Assert.AreEqual(3, mruList.Items.Count);
            Assert.AreEqual("a", mruList.Items[0]);
            Assert.AreEqual("c", mruList.Items[1]);
            Assert.AreEqual("b", mruList.Items[2]);

            mruList.AddItem("b");
            Assert.AreEqual(3, mruList.Items.Count);
            Assert.AreEqual("b", mruList.Items[0]);
            Assert.AreEqual("a", mruList.Items[1]);
            Assert.AreEqual("c", mruList.Items[2]);

            mruList.AddItem("c");
            Assert.AreEqual(3, mruList.Items.Count);
            Assert.AreEqual("c", mruList.Items[0]);
            Assert.AreEqual("b", mruList.Items[1]);
            Assert.AreEqual("a", mruList.Items[2]);
        }
    }
}
