using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using second;
using System.Net.Sockets;
using System.IO;
using second.Packets;

namespace SecondTester
{
    [TestClass]
    public class DownloaderBeforeExpected
    {
        [TestMethod]
        public void At66045Arrive254() // ...P....MIN-------MAX.....
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)66045);
            object res = o.Invoke("receive", new CommunicationPacket(0, 254, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)65790, down.SerialNumber);
        }

        [TestMethod]                    //          V
        public void At66045Arrive509()  //  .......MIN------MAX.....
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)66045);
            object res = o.Invoke("receive", new CommunicationPacket(0, 509, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)66045, down.SerialNumber);
        }

        [TestMethod]             
        public void At66045Arrive764()  //  .......MIN--P---MAX.....
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)66045);
            object res = o.Invoke("receive", new CommunicationPacket(0, 764, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)66300, down.SerialNumber);
        }

        [TestMethod]                     //                   V
        public void At66045Arrive2549()  //  .......MIN------MAX.....
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)66045);
            object res = o.Invoke("receive", new CommunicationPacket(0, 2549, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)68085, down.SerialNumber);
        }

        [TestMethod]          
        public void At66045Arrive2804()  //  .......MIN------MAX...P..
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)66045);
            object res = o.Invoke("receive", new CommunicationPacket(0, 2805, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)2805, down.SerialNumber); //it thinks, that it overlaps
        }





        ///// NEXT EDGE




        [TestMethod]
        public void At131580Arrive253() // ...P....MIN-------MAX.....
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)131580);
            object res = o.Invoke("receive", new CommunicationPacket(0, 253, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)131325, down.SerialNumber);
        }

        [TestMethod]                    //          V
        public void At131580Arrive508()  //  .......MIN------MAX.....
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)131580);
            object res = o.Invoke("receive", new CommunicationPacket(0, 508, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)131580, down.SerialNumber);
        }

        [TestMethod]
        public void At131580Arrive1018()  //  .......MIN--P---MAX.....
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)131580);
            object res = o.Invoke("receive", new CommunicationPacket(0, 1018, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)132090, down.SerialNumber);
        }

        [TestMethod]                      //                   V
        public void At131580Arrive2548()  //  .......MIN------MAX.....
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)131580);
            object res = o.Invoke("receive", new CommunicationPacket(0, 2548, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)133620, down.SerialNumber);
        }

        [TestMethod]
        public void At131580Arrive3059()  //  .......MIN------MAX...P..
        {
            Downloader d = new Downloader(null, new BinaryWriter(new MemoryStream()));
            PrivateObject o = new PrivateObject(d);
            o.SetFieldOrProperty("required", (UInt64)131580);
            object res = o.Invoke("receive", new CommunicationPacket(0, 3059, 0, 0, new byte[] { }));
            DownloadPacket down = (DownloadPacket)res;
            Assert.AreEqual((UInt64)68595, down.SerialNumber); //it thinks, that it overlaps
        }
    }
}
