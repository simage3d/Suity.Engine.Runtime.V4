// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Crypto;
using Suity.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    public static class NetworkConfigs
    {
        public static readonly ModuleConfigParameter<string> IP = new ModuleConfigParameter<string>("IP");
        public static readonly ModuleConfigParameter<string> Uri = new ModuleConfigParameter<string>("Uri");
        public static readonly ModuleConfigParameter<string> ApiPath = new ModuleConfigParameter<string>("ApiPath");
        public static readonly ModuleConfigParameter<string> DomainName = new ModuleConfigParameter<string>("DomainName");
        public static readonly ModuleConfigParameter<string> DomainPrivateKey = new ModuleConfigParameter<string>("DomainPrivateKey");
        public static readonly ModuleConfigParameter<string> ContentRequestPath = new ModuleConfigParameter<string>("ContentRequestPath");
        public static readonly ModuleConfigParameter<string> ContentRealPath = new ModuleConfigParameter<string>("ContentRealPath");
        public static readonly ModuleConfigParameter<string> PublicKey = new ModuleConfigParameter<string>("PublicKey");
        public static readonly ModuleConfigParameter<string> PrivateKey = new ModuleConfigParameter<string>("PrivateKey");
        public static readonly ModuleConfigParameter<string> Secret = new ModuleConfigParameter<string>("Secret");
        public static readonly ModuleConfigParameter<string> Version = new ModuleConfigParameter<string>("Version");
        public static readonly ModuleConfigParameter<int> Port = new ModuleConfigParameter<int>("Port");
        public static readonly ModuleConfigParameter<int> MaxRequestLength = new ModuleConfigParameter<int>("MaxRequestLength");
        public static readonly ModuleConfigParameter<int> KeepAliveTime = new ModuleConfigParameter<int>("KeepAliveTime");
        public static readonly ModuleConfigParameter<int> MaxConnectionNumber = new ModuleConfigParameter<int>("MaxConnectionNumber");
        public static readonly ModuleConfigParameter<bool> Compressed = new ModuleConfigParameter<bool>("Compressed");
        public static readonly ModuleConfigParameter<bool> Encryped = new ModuleConfigParameter<bool>("Encryped");
        public static readonly ModuleConfigParameter<bool> AutoDiscovery = new ModuleConfigParameter<bool>("AutoDiscovery");
        public static readonly ModuleConfigParameter<bool> ServiceProxy = new ModuleConfigParameter<bool>("ServiceProxy");

        public static readonly ModuleConfigParameter<RsaKey> RsaKey = new ModuleConfigParameter<RsaKey>("RsaKey");
        public static readonly ModuleConfigParameter<DesKey> DesKey = new ModuleConfigParameter<DesKey>("DesKey");
        public static readonly ModuleConfigParameter<AesKey> AesKey = new ModuleConfigParameter<AesKey>("AesKey");


        public static readonly ModuleConfigParameter<PacketFormats> PacketFormat = new ModuleConfigParameter<PacketFormats>("PacketFormat");

        public static readonly ModuleConfigParameter<IEnumerable<TypeFamily>> ObjectFamilies = new ModuleConfigParameter<IEnumerable<TypeFamily>>("ObjectFamilies");
        public static readonly ModuleConfigParameter<IEnumerable<NetworkCommandFamily>> CommandFamilies = new ModuleConfigParameter<IEnumerable<NetworkCommandFamily>>("CommandFamilies");
        public static readonly ModuleConfigParameter<IEnumerable<NetworkCommand>> CommandHandlers = new ModuleConfigParameter<IEnumerable<NetworkCommand>>("CommandHandlers");


        public static readonly ModuleConfigParameter<NetworkUpdaterFamily> UpdaterFamily = new ModuleConfigParameter<NetworkUpdaterFamily>("UpdaterFamily");
        public static readonly ModuleConfigParameter<NetworkUpdater> UpdaterHandler = new ModuleConfigParameter<NetworkUpdater>("UpdaterHandler");
        public static readonly ModuleConfigParameter<IEnumerable<NetworkUpdaterFamily>> UpdaterFamilies = new ModuleConfigParameter<IEnumerable<NetworkUpdaterFamily>>("UpdaterFamilies");
        public static readonly ModuleConfigParameter<IEnumerable<NetworkUpdater>> UpdaterHandlers = new ModuleConfigParameter<IEnumerable<NetworkUpdater>>("UpdaterHandlers");


        public static readonly ModuleConfigParameter<IEnumerable<string>> SupportedVersions = new ModuleConfigParameter<IEnumerable<string>>("SupportedVersions");


        public const string Event_SessionConnected = "Server.SessionConnected";
        public const string Event_SessionClosed = "Server.SessionClosed";
        public const string Event_UserLoggedIn = "Server.UserLoggedIn";

        public const string Event_PlayerAdded = "Player.Added";
        public const string Event_PlayerRemoved = "Player.Removed";

        public const string Event_ClientConnected = "Client.ClientConnected";
        public const string Event_ClientReconnected = "Client.ClientReconnected";
        public const string Event_ClientClosed = "Client.ClientClosed";
    }
}
