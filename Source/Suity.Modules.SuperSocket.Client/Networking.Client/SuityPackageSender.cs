using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Networking.Client
{
    class SuityPackageSender
    {
        readonly PacketFormatter _packetFormatter;

        public SuityPackageSender(PacketFormats packetFormat, bool compressed)
        {
            _packetFormatter = PacketFormatter.CreatePacketFormatter(packetFormat, compressed);

            if (_packetFormatter == null)
            {
                throw new ArgumentNullException(nameof(packetFormat));
            }
        }

        public void WritePackage(BinaryDataWriter writer, object obj, int channel)
        {
            int offsetBegin = writer.Offset;
            writer.Offset += 5;
            _packetFormatter.Encode(writer, obj);
            int offsetEnd = writer.Offset;

            int size = offsetEnd - offsetBegin - 5;
            writer.Offset = offsetBegin;
            writer.WriteByte((byte)channel);
            writer.WriteInt32(size);

            writer.Offset = offsetEnd;
        }
    }
}
