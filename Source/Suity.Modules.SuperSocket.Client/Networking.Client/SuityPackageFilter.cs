using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Suity.Engine;

using Suity.Helpers.Conversion;
using SuperSocket.ProtoBase;

namespace Suity.Networking.Client
{
    public class SuityPackageFilter : FixedHeaderReceiveFilter<ObjectPackageInfo>
    {
        PacketFormatter _packetFormatter;

        public PacketFormatter Formatter
        {
            get { return _packetFormatter; }
            set { _packetFormatter = value; }
        }

        public SuityPackageFilter()
            : base(5)
        {
        }
        public SuityPackageFilter(PacketFormatter formatter)
            : this()
        {
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            bufferStream.Skip(1);
            return bufferStream.ReadInt32(true);
        }

        public override ObjectPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            int channel = bufferStream.ReadByte();
            int len = bufferStream.ReadInt32(true);
            byte[] bodyBuffer = new byte[len];
            bufferStream.Read(bodyBuffer, 0, len);

            string typeName = null;
            object obj = null;

            if (_packetFormatter.Decode(bodyBuffer, 0, len, ref typeName, ref obj))
            {
                return new ObjectPackageInfo(typeName, obj, NetworkDeliveryMethods.Default, channel);
            }
            else
            {
                return null;
            }
        }
    }

}
