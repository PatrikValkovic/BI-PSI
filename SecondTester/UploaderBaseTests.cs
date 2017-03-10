using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using second;
using System.Net.Sockets;
using System.IO;
using second.Packets;

namespace SecondTester
{
    [TestClass]
    public class UploaderBaseTests
    {
        [TestMethod]
        public void At98175Arrive33599()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(33599, 98175, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)99135, real);
        }

    }
}
