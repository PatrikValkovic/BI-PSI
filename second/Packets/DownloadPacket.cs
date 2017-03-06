using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace second.Packets
{
    class DownloadPacket : BasePacket
    {
        public DownloadPacket(byte[] data,UInt32 connectionNumber, byte flags, UInt64 serialNumber)
            :base(connectionNumber,flags,data)
        {
            this.SerialNumber = serialNumber;
        }

        public readonly UInt64 SerialNumber;
    }
}
