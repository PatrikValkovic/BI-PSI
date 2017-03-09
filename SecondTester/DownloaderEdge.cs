using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using second;
using System.Net.Sockets;
using System.IO;
using second.Packets;

namespace SecondTester
{
    [TestClass]
    public class DownloaderEdge
    {
        [TestMethod]
        public void At65280Arrive65535()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(65535, 65280, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)65535, real);
        }

        [TestMethod]
        public void At65280Arrive254()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(254, 65280, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)65790, real);
        }

        [TestMethod]
        public void At65535Arrive65535()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(65535, 65535, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)65535, real);
        }

        [TestMethod]
        public void At65535Arrive509()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(509, 65535, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)66045, real);
        }

        [TestMethod]
        public void At66045Arrive655535()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(65535, 66045, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)65535, real);
        }

        [TestMethod]
        public void At66045Arrive509()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(509, 66045, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)66045, real);
        }
    }
}
