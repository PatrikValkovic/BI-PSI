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
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)130815);
            object res = o.Invoke("receive", new CommunicationPacket(0, 65534, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)131070, down.SerialNumber);
        }

        [TestMethod]
        public void At130815Arrive253()
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)130815);
            object res = o.Invoke("receive", new CommunicationPacket(0, 253, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)131325, down.SerialNumber);
        }

        [TestMethod]
        public void At131070Arrive65534()
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)131070);
            object res = o.Invoke("receive", new CommunicationPacket(0, 65535, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)131070, down.SerialNumber);
        }

        [TestMethod]
        public void At131070Arrive508()
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)131070);
            object res = o.Invoke("receive", new CommunicationPacket(0, 508, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)131580, down.SerialNumber);
        }

        [TestMethod]
        public void At131325Arrive655534()
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)131325);
            object res = o.Invoke("receive", new CommunicationPacket(0, 65534, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)131070, down.SerialNumber);
        }

        [TestMethod]
        public void At131325Arrive508()
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)131325);
            object res = o.Invoke("receive", new CommunicationPacket(0, 508, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)131580, down.SerialNumber);
        }
    }
}
