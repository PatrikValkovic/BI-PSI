using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace second.Packets
{
    class UploadSendPacket : BasePacket
    {
        public UploadSendPacket(uint connectionNumber, byte flags, byte[] data, UInt64 serial)
            : base(connectionNumber, flags, data)
        {
            this.SerialNumber = serial;
        }

        public readonly UInt64 SerialNumber;

        public ushort Sended = 0;
        public DateTime LastSend;

        public CommunicationPacket CreatePacketToSend()
        {
            return new CommunicationPacket(this.ConnectionNumber, Convert.ToUInt16(this.SerialNumber & UInt16.MaxValue), 0, this.Flags, this.Data);
        }
    }
}
