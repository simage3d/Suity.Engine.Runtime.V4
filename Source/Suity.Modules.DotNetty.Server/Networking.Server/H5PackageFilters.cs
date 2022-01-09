using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Suity.Crypto;
using Suity.Modules;
using System;

namespace Suity.Networking.Server
{
    class H5PackageFilter : LengthFieldBasedFrameDecoder
    {
        [ThreadStatic]
        static BinaryDataWriter _writer;

        readonly PacketFormatter _packetFormatter;

        public H5PackageFilter(PacketFormats packetFormat, bool compressed, AesKey aesKey, int maxFrameLength)
            : base(ByteOrder.LittleEndian, maxFrameLength, 1, 4, 0, 0, true)
        {
            _packetFormatter = PacketFormatter.CreatePacketFormatter(packetFormat, compressed, aesKey) ?? throw new ArgumentException(nameof(packetFormat));
        }

        protected override object Decode(IChannelHandlerContext context, IByteBuffer input)
        {
            int beginIndex = input.ReaderIndex;

            int channelCode = input.GetByte(input.ReaderIndex);

            var b = base.Decode(context, input);
            if (b is IByteBuffer buffer)
            {
                int methodNumber = channelCode / NettySession.ChannelCount;
                int channel = channelCode % NettySession.ChannelCount;

                int size = input.ReaderIndex - beginIndex - 5;

                var writer = _writer ?? (_writer = new BinaryDataWriter());
                writer.Offset = size;
                writer.EnsureBufferOffset();

                input.GetBytes(beginIndex + 5, writer.Buffer, 0, size);

                string typeName = null;
                object obj = null;
                if (_packetFormatter.Decode(writer.Buffer, 0, size, ref typeName, ref obj))
                {
                    return new NettyRequestInfo(typeName, obj, (NetworkDeliveryMethods)methodNumber, channel);
                }
                else
                {
                    return b;
                }

            }
            else
            {
                return b;
            }
        }

    }

}
