// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using Suity.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Suity.Networking
{
    /// <summary>
    /// 网络会话
    /// </summary>
    public abstract class NetworkSession : Suity.Object
    {
        Dictionary<Type, object> _properties;
        NetworkUser _user;

        public NetworkSession()
        {
            LastActiveTime = StartTime = DateTime.UtcNow;
        }

        protected override string GetName()
        {
            return SessionId;
        }

        /// <summary>
        /// 网络服务器对象
        /// </summary>
        public abstract NetworkServer Server { get; }

        /// <summary>
        /// 连接的远程地址
        /// </summary>
        public abstract IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 连接的本地地址
        /// </summary>
        public abstract IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public abstract bool Connected { get; }

        /// <summary>
        /// 会话Id
        /// </summary>
        public abstract string SessionId { get; }

        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastActiveTime { get; internal set; }

        /// <summary>
        /// 按频道获取最后活动时间
        /// </summary>
        /// <param name="channel">频道</param>
        /// <returns></returns>
        public virtual DateTime GetLastActiveTime(int channel)
        {
            return LastActiveTime;
        }

        /// <summary>
        /// 是否长连接会话
        /// </summary>
        public abstract KeepAliveModes KeepAlive { get; }

        /// <summary>
        /// 用户身份对象
        /// </summary>
        public NetworkUser User
        {
            get
            {
                if (_user == null)
                {
                    _user = ResolveNetworkUser();
                    if (_user != null)
                    {
                        NodeApplication.Current?.ReportUserLogin(this);
                    }
                }

                return _user;
            }
            set
            {
                if (_user == value)
                {
                    return;
                }

                _user = value;

                if (_user != null)
                {
                    OnSetNetworkUser(_user);
                    NodeApplication.Current?.ReportUserLogin(this);
                }
            }
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract T GetService<T>() where T : class;
        
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetProperty<T>() where T : class
        {
            lock (this)
            {
                return _properties?.GetValueOrDefault(typeof(T)) as T;
            }
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        public virtual void SetProperty<T>(T property) where T : class
        {
            lock (this)
            {
                if (property != null)
                {
                    if (_properties == null)
                    {
                        _properties = new Dictionary<Type, object>();
                    }
                    _properties[typeof(T)] = property;
                }
                else
                {
                    _properties?.Remove(typeof(T));
                }
            }
        }

        /// <summary>
        /// 发送数据对象
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <param name="method">方法</param>
        /// <param name="channel">频道</param>
        public abstract void Send(object obj, NetworkDeliveryMethods method, int channel);

        /// <summary>
        /// 关闭会话
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 报告用户登出
        /// </summary>
        /// <param name="now"></param>
        public void ReportUserLogout()
        {
            NodeApplication.Current?.ReportUserLogout(this);
        }
        /// <summary>
        /// 报告用户购买
        /// </summary>
        /// <param name="productId">产品Id</param>
        /// <param name="token">产品唯一码</param>
        /// <param name="receipt">产品收据</param>
        public void ReportPurchaseItem(string productId, string token, string receipt)
        {
            NodeApplication.Current?.ReportPurchaseItem(this, productId, token, receipt);
        }

        /// <summary>
        /// 解算当前网络用户
        /// </summary>
        /// <returns></returns>
        protected virtual NetworkUser ResolveNetworkUser()
        {
            return null;
        }

        /// <summary>
        /// 网络用户已设置
        /// </summary>
        /// <param name="user"></param>
        protected virtual void OnSetNetworkUser(NetworkUser user)
        {
        }


        public override string ToString()
        {
            return GetName();
        }
    }
}
