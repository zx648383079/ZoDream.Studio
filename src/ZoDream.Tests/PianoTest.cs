using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoDream.Shared.Models;

namespace ZoDream.Tests
{
    [TestClass]
    public class PianoTest
    {
        [TestMethod]
        public void TestKey()
        {
            var key = PianoKey.Create("C5");
            Assert.AreEqual(key.ToKey127(), 60);
        }
    }
}