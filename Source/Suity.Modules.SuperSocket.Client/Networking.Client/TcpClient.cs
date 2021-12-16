using Suity.Engine;
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Networking.Client
{
    class TcpClient : NetworkClient
    {
        readonly EasyClient _client;

        readonly SuityPackageFilter _filter;

        public override bool IsConnected => _client.IsConnected;

        public TcpClient()
            : base(32, 16)
        {
            _filter = new SuityPackageFilter();

            _client = new EasyClient();
            _client.Initialize(_filter, this.NotifyPacket);
            _client.Connected += _client_Connected;
            _client.Error += _client_Error;
            _client.Closed += _client_Closed;
        }

        protected override void OnPacketFormatterUpdated()
        {
            _filter.Formatter = Formatter;
        }

        protected override IPEndPoint GetClientRemoteEndPoint()
        {
            return _client.LocalEndPoint as IPEndPoint;
        }

        protected override void ClientStart()
        {
        }

        protected override void ClientConnect(IPEndPoint endPoint, bool reconnecting)
        {
            _client.ConnectAsync(endPoint);
        }

        protected override void ClientSendMessage(object obj, NetworkDeliveryMethods method, int channel)
        {
            _buffer.Offset = 5;
            Formatter.Encode(_buffer, obj);
            int offsetEnd = _buffer.Offset;

            int size = _buffer.Offset - 5;
            _buffer.Offset = 0;
            _buffer.WriteByte((byte)channel);
            _buffer.WriteInt32(size);

            _buffer.Offset = offsetEnd;

            byte[] b = _buffer.ToBytes();
            _client.Send(b);
        }

        protected override void ClientDisconnect(string reason)
        {
            _client.Close();
        }

        protected override void ClientShutdown(string reason)
        {
            _client.Close();
        }


        void _client_Connected(object sender, EventArgs e)
        {
            NotifyClientConnected();
        }
        void _client_Error(object sender, ErrorEventArgs e)
        {
            NotifyClientError(e.Exception);
        }
        void _client_Closed(object sender, EventArgs e)
        {
            NotifyClientDisconnected();
        }

    }
}
