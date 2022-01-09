using DotNetty.Common.Utilities;
using DotNetty.Handlers.Flow;
using DotNetty.Transport.Channels;
using Suity.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Networking.Server
{
    public class ServerHandler : FlowControlHandler
    {
        public static IChannelHandlerContext Current;

        readonly NettyBinding _server;

        public ServerHandler(NettyBinding server)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
        }

        //服务启动
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Logs.LogInfo(@"--- DotNetty Server is active ---");
            Current = context;
        }

        //服务关闭
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Logs.LogInfo($"--- DotNetty {context.Name} is inactive ---");
        }

        //收到消息
        public override void ChannelRead(IChannelHandlerContext context, object msg)
        {
            var info = msg as NettyRequestInfo;
            if (info is null)
            {
                return;
            }

            var sessionAttr = context.GetAttribute(AttributeKey<NettySession>.ValueOf("session"));
            var session = sessionAttr.Get();

            if (session is null)
            {
                return;
            }

            _server.ExecuteCommand(session, info);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
        //客户端长时间没有Write，会触发此事件
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            base.UserEventTriggered(context, evt);
        }
        //捕获异常
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Logs.LogInfo($"DotNetty Server Exception: ");
            Logs.LogInfo(exception);
            //context.CloseAsync();
        }


        //客户端连接
        public override void HandlerAdded(IChannelHandlerContext context)
        {
            var sessionAttr = context.GetAttribute(AttributeKey<NettySession>.ValueOf("session"));
            var session = sessionAttr.Get();

            if (session is null)
            {
                session = new NettySession(_server, context);
                sessionAttr.Set(session);
            }

            Logs.LogInfo($"Client {context} is Connected.");
            base.HandlerAdded(context);
        }

        //客户端断开
        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            Logs.LogInfo($"Client {context} is Disconnected.");
            base.HandlerRemoved(context);
        }
    }
}
