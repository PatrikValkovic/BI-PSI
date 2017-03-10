using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace second.Packets
{
    class UploadRecvPacket : BasePacket
    {
        public UploadRecvPacket(uint connectionNumber, byte flags, byte[] data, UInt64 confirmation) 
            : base(connectionNumber, flags, data)
        {
            this.ConfirmationNumber = confirmation;
        }

        public readonly UInt64 ConfirmationNumber;
    }
}
