using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace second.Packets
{
    class BasePacket
    {
        public BasePacket(byte[] data, UInt32 connectionNumber, byte flags)
        {
            this.Data = data;
            this.ConnectionNumber = connectionNumber;
            this.Flags = flags;
        }
        public readonly byte[] Data;
        public readonly UInt32 ConnectionNumber;
        public readonly byte Flags;
    }
}
