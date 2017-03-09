using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace second.Packets
{
    class UploadSendPacket : BasePacket
    {
        public UploadSendPacket(uint connectionNumber, byte flags, byte[] data)
            : base(connectionNumber, flags, data)
        {
        }

        public readonly UInt64 SerialNumber;

        public ushort Sended = 0;
        public DateTime LastSend;

        public CommunicationPacket CreatePacketToSend()
        {
            throw new NotImplementedException();
        }
    }
}
