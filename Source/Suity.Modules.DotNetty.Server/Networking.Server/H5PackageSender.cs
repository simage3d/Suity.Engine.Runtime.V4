using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Suity.Crypto;
using Suity.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Networking.Server
{
    class H5PackageSender : MessageToMessageEncoder<NettyRequestInfo>
    {
        [ThreadStatic]
        static BinaryDataWriter _buffer;

        readonly PacketFormatter _packetFormatter;

        public H5PackageSender(PacketFormats packetFormat, bool compressed, AesKey aesKey)
        {
            _packetFormatter = PacketFormatter.CreatePacketFormatter(packetFormat, compressed, aesKey) ?? throw new ArgumentException(nameof(packetFormat));
        }

        protected override void Encode(IChannelHandlerContext context, NettyRequestInfo message, List<object> output)
        {
            var writer = _buffer ?? (_buffer = new BinaryDataWriter());

            writer.Offset = 5;

            _packetFormatter.Encode(writer, message.Body);
            int offsetEnd = writer.Offset;

            int size = offsetEnd - 5;

            int channelCode = ((int)message.Method) * NettySession.ChannelCount + message.Channel;

            writer.Offset = 0;
            writer.WriteByte((byte)channelCode);
            writer.WriteInt32(size);

            writer.Offset = offsetEnd;

            var b = context.Allocator.Buffer(offsetEnd).WriteBytes(writer.Buffer, 0, offsetEnd);
            output.Add(b);
        }
    }
}
