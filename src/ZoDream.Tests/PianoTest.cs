using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

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

        [TestMethod]
        public void TestDraw()
        {
            var helper = new PianoDraw()
            {
                BoxWidth = 200,
                BoxHeight = 200,
                BlackKeyHeight = 50,
                WhiteKeyHeight = 100,
                WhiteKeyWidth = 60,
                BlackKeyWidth = 30,
                Min = PianoKey.Create127(0),
                Max = PianoKey.Create127(131),
                Offset = -61
            };
            var key = helper.Get(0);
            Assert.AreEqual(key.ToKey127(), (byte)1);
        }
    }
}