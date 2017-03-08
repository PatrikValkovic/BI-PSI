using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using second;
using System.Net.Sockets;
using System.IO;
using second.Packets;

namespace SecondTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SerialBeforeEdgeOfOverflow()
        {
            Downloader d = new Downloader(null,new StringWriter());
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)UInt16.MaxValue-510);
            object res = o.Invoke("receive", new CommunicationPacket(0,UInt16.MaxValue-255,0,0,new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual(down.SerialNumber,(UInt64) UInt16.MaxValue-255);
        }

        [TestMethod]
        public void SerialAfterEdgeOfOverflow()
        {
            Downloader d = new Downloader(null, new StringWriter());
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)UInt16.MaxValue - 510);
            object res = o.Invoke("receive", new CommunicationPacket(0, 255, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual(down.SerialNumber, (UInt64)UInt16.MaxValue + 255);
        }
    }
}
