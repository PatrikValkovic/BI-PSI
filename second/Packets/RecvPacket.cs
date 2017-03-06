using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace second.Packets
{
    class RecvPacket : BasePacket
    {
        public RecvPacket(UInt32 connectionNumber, UInt16 serialNumber, UInt16 confirmationNumber, byte flags, byte[] data)
            : base(connectionNumber,flags,data)
        {
            this.SerialNumber = serialNumber;
            this.ConfirmationNumber = confirmationNumber;
        }

        public readonly UInt16 SerialNumber;
        public readonly UInt16 ConfirmationNumber;
    }
}
