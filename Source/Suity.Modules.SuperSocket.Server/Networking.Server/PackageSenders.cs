using LZ4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Networking.Server
{
    abstract class PackageSender
    {
        public abstract void WritePackage(BinaryDataWriter writer, object obj, int channel);
    }

    class H5PackageSender : PackageSender
    {
        readonly PacketFormatter _packetFormatter;

        public H5PackageSender(PacketFormats packetFormat, bool compressed)
        {
            _packetFormatter = PacketFormatter.CreatePacketFormatter(packetFormat, compressed) ?? throw new ArgumentException(nameof(packetFormat));
        }

        public override void WritePackage(BinaryDataWriter writer, object obj, int channel)
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
