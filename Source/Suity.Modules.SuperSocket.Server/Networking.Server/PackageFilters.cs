using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suity.Engine;
using Suity.Helpers.Conversion;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using LZ4;
using Suity.Crypto;

namespace Suity.Networking.Server
{
    class H5PackageFilter : FixedHeaderReceiveFilter<SsRequestInfo>
    {
        readonly PacketFormatter _packetFormatter;

        public H5PackageFilter(PacketFormats packetFormat, bool compressed, AesKey aesKey)
            : base(5)
        {
            _packetFormatter = PacketFormatter.CreatePacketFormatter(packetFormat, compressed, aesKey) ?? throw new ArgumentException(nameof(packetFormat));
        }


        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            int size = EndianBitConverter.Little.ToInt32(header, offset + 1);
            return size;
        }
        protected override SsRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            int channelCode = header.Array[header.Offset];
            int methodNumber = channelCode / SsAppSession.ChannelCount;
            int channel = channelCode % SsAppSession.ChannelCount;

            string typeName = null;
            object obj = null;
            if (_packetFormatter.Decode(bodyBuffer, offset, length, ref typeName, ref obj))
            {
                return new SsRequestInfo(typeName, obj, (NetworkDeliveryMethods)methodNumber, channel);
            }
            else
            {
                return null;
            }
        }
    }

}
