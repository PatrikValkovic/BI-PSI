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
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)65280);
            object res = o.Invoke("receive", new CommunicationPacket(0, 65535, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)65535, down.SerialNumber);
        }

        [TestMethod]
        public void At65280Arrive254()
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)65280);
            object res = o.Invoke("receive", new CommunicationPacket(0, 254, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)65790, down.SerialNumber);
        }

        [TestMethod]
        public void At65535Arrive65535()
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)65535);
            object res = o.Invoke("receive", new CommunicationPacket(0, 65535, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)65535, down.SerialNumber);
        }

        [TestMethod]
        public void At65535Arrive509()
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)65535);
            object res = o.Invoke("receive", new CommunicationPacket(0, 509, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)66045, down.SerialNumber);
        }

        [TestMethod]
        public void At66045Arrive655535()
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)66045);
            object res = o.Invoke("receive", new CommunicationPacket(0, 65535, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)65535, down.SerialNumber);
        }

        [TestMethod]
        public void At66045Arrive509()
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)66045);
            object res = o.Invoke("receive", new CommunicationPacket(0, 509, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)66045, down.SerialNumber);
        }
    }
}
