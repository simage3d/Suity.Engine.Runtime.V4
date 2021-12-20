using LZ4;
using Suity.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Networking.Server
{
    abstract class PackageSender
    {
        public abstract void WritePackage(BinaryDataWriter writer, object obj, NetworkDeliveryMethods method, int channel);
    }

    class H5PackageSender : PackageSender
    {
        readonly PacketFormatter _packetFormatter;

        public H5PackageSender(PacketFormats packetFormat, bool compressed, AesKey aesKey)
        {
            _packetFormatter = PacketFormatter.CreatePacketFormatter(packetFormat, compressed, aesKey) ?? throw new ArgumentException(nameof(packetFormat));
        }

        public override void WritePackage(BinaryDataWriter writer, object obj, NetworkDeliveryMethods method, int channel)
        {
            int offsetBegin = writer.Offset;
            writer.Offset += 5;

            _packetFormatter.Encode(writer, obj);
            int offsetEnd = writer.Offset;

            int size = offsetEnd - offsetBegin - 5;

            int channelCode = ((int)method) * SsAppSession.ChannelCount + channel;

            writer.Offset = offsetBegin;
            writer.WriteByte((byte)channelCode);
            writer.WriteInt32(size);

            writer.Offset = offsetEnd;
        }
    }

}
