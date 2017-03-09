using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using second;
using System.Net.Sockets;
using System.IO;
using second.Packets;

namespace SecondTester
{
    [TestClass]
    public class DownloaderSecondEdge
    {
        [TestMethod]
        public void At130815Arrive65534()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(65534, 130815, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)131070, real);
        }

        [TestMethod]
        public void At130815Arrive253()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(253, 130815, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)131325, real);
        }

        [TestMethod]
        public void At131070Arrive65534()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(65534, 131070, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)131070, real);
        }

        [TestMethod]
        public void At131070Arrive508()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(508, 131070, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)131580, real);
        }

        [TestMethod]
        public void At131325Arrive655534()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(65534, 131325, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)131070, real);
        }

        [TestMethod]
        public void At131325Arrive508()
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(508, 131325, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)131580, real);
        }
    }
}
