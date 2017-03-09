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
            UInt64 real = CommunicationFacade.ComputeRealNumber(254,66045,UInt16.MaxValue,(uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)65790, real);
        }

        [TestMethod]                    //          V
        public void At66045Arrive509()  //  .......MIN------MAX.....
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(509, 66045, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)66045, real);
        }

        [TestMethod]             
        public void At66045Arrive764()  //  .......MIN--P---MAX.....
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(764, 66045, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)66300, real);
        }

        [TestMethod]                     //                   V
        public void At66045Arrive2549()  //  .......MIN------MAX.....
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(2549, 66045, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)68085, real);
        }

        [TestMethod]          
        public void At66045Arrive2804()  //  .......MIN------MAX...P..
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(2805, 66045, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)2805, real);  //it thinks, that it overlaps
        }





        ///// NEXT EDGE




        [TestMethod]
        public void At131580Arrive253() // ...P....MIN-------MAX.....
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(253, 131580, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)131325, real);
        }

        [TestMethod]                    //          V
        public void At131580Arrive508()  //  .......MIN------MAX.....
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(508, 131580, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)131580, real);
        }

        [TestMethod]
        public void At131580Arrive1018()  //  .......MIN--P---MAX.....
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(1018, 131580, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)132090, real);

        }

        [TestMethod]                      //                   V
        public void At131580Arrive2548()  //  .......MIN------MAX.....
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(2548, 131580, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)133620, real);
        }

        [TestMethod]
        public void At131580Arrive3059()  //  .......MIN------MAX...P..
        {
            UInt64 real = CommunicationFacade.ComputeRealNumber(3059, 131580, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
            Assert.AreEqual((UInt64)68595, real);  //it thinks, that it overlaps
        }
    }
}
