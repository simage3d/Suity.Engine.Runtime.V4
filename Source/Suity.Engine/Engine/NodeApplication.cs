// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;
using Suity.Helpers;
using Suity.Json;
using Suity.Networking;
using Suity.NodeQuery;

namespace Suity.Engine
{
    public class NodeApplication : Suity.SystemObject
    {
        readonly ConsoleCommandService _consoleCommand;
        readonly DateTime _creationTime;

        public NodeObject Node { get; private set; }
        public ConsoleCommandService ConsoleCommand => _consoleCommand;

        public virtual bool IsInOperation => false;
        public virtual string ServiceId => "NodeApplication";
        public virtual int MultipleLaunchIndex => 0;
        public virtual string GalaxyName => null;
        public virtual string GalaxyId => null;
        public virtual string GalaxyVersion => null;
        public virtual string StellarId => null;
        public virtual string ApplicationName => GetName();
        public virtual string ApplicationId => null;
        public virtual string DataId => null;
        public virtual string DataVersion => null;
        public virtual string InternalIPAddress => "127.0.0.1";
        public virtual string PublicIPAddress => "127.0.0.1";

#if BRIDGE
        public virtual string AppDirectory { get { return AppDomain.CurrentDomain.ToLocaleString(); } }
#else
        public virtual string AppDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }
#endif

        #region Events
        public event EventHandler DataReloaded;
        #endregion

        public NodeApplication()
        {
            _creationTime = DateTime.UtcNow;

            _consoleCommand = new ConsoleCommandService();
            _consoleCommand.AddCommand(new HelpCommand());
            _consoleCommand.AddCommand(new InfoCommand());
            _consoleCommand.AddCommand(new ReloadCommand());
        }

        #region Start Stop
        public sealed override bool IsStarted => Node?.State == NodeObjectState.Started;
        public sealed override void Start()
        {
            if (IsStarted)
            {
                return;
            }

            try
            {
                Logs.LogInfo($"Starting {ToString()}...");


                //自动初始化 IInitialize 接口
                try
                {
                    foreach (Type init in typeof(IInitialize).GetDerivedTypes().Where(o => o.IsClass && (!o.IsAbstract)))
                    {
                        try
                        {
                            Activator.CreateInstance(init);
                            Logs.LogDebug($"Initialize {init.FullName} ({init.Assembly.FullName}).");
                        }
                        catch (Exception)
                        {
                            Logs.LogError($"IInitialize FAILED : {init.FullName} ({init.Assembly.FullName}).");
                        }
                    }
                }
                catch (Exception)
                {
                    Logs.LogError("IInitialize FAILED.");
                }

                //加载数据
                LoadData();

                //创建 NodeObject 和组件
                if (Node == null)
                {
                    Node = CreateNodeObject();
                }
                if (Node == null)
                {
                    Logs.LogWarning($"{ToString()} is NOT started.");
                    return;
                }

                if (Node.State != NodeObjectState.Started)
                {
                    Node.Start();
                    Logs.LogInfo($"{ToString()} is started.");
                }
                else
                {
                    Logs.LogError($"{ToString()} is in invalid state.");
                }

                OnStarted();
            }
            catch (Exception e)
            {
                Logs.LogError(e);
                throw;
            }
        }
        public sealed override void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            try
            {
                if (Node != null)
                {
                    Logs.LogInfo($"Stopping {ToString()}...");
                    Node.Stop();
                    Node = null;

                    Logs.LogInfo($"{ToString()} is stopped.");
                }
            }
            catch (Exception e)
            {
                Logs.LogError(e);
                throw;
            }
            finally
            {
                try
                {
                    OnStopped();
                }
                catch (Exception e)
                {
                    Logs.LogError(e);
                    throw;
                }
            }
        }

        protected virtual NodeObject CreateNodeObject()
        {
            return new NodeObject();
        }

        protected virtual void OnStarted()
        {
        }
        protected virtual void OnStopped()
        {
        }

        protected virtual void CollectInformation(INodeWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var node = Node;
            if (node == null)
            {
                return;
            }

            writer.WriteInfo("Components", "Components", compWriter => 
            {
                foreach (var component in node.Components)
                {
                    if (component is IInfoNode info)
                    {
                        try
                        {
                            writer.WriteInfo(component.GetType().Name, component.Description, w => info.WriteInfo(w));
                        }
                        catch (Exception err)
                        {
                            Logs.LogError(err);
                        }
                    }
                }
            });
        }

        #endregion

        #region Data
        public void LoadData()
        {
            LoadData(false);
        }
        public void LoadData(bool notify)
        {
            LoadData(true, notify);
        }
        public virtual void LoadData(bool clearData, bool notify)
        {
            IDataResource dataResource = Suity.Environment.GetService<IDataResource>();
            NodeStartInfo startInfo = Suity.Environment.GetService<NodeStartInfo>();

            if (dataResource == null)
            {
                Logs.LogError($"{nameof(IDataResource)} service not found.");
                return;
            }
            if (startInfo == null)
            {
                Logs.LogError($"{nameof(NodeStartInfo)} service not found.");
                return;
            }

            try
            {
                if (clearData)
                {
                    DataStorage.Clear(false);
                    DataStorage.PushDataLayer();
                }

                //Logs.LogInfo("Start loading data...");

                foreach (var dataId in startInfo.DataInputs)
                {
                    var data = dataResource.GetDataResource(dataId);
                    if (data != null)
                    {
                        DataCollection collection = DataStorage.LoadCollection(new JsonDataReader(data));
                        if (collection != null)
                        {
                            Logs.LogDebug($"Added data : {collection}");
                        }
                        else
                        {
                            Logs.LogError($"Add data failed : {dataId}");
                        }
                    }
                    else
                    {
                        Logs.LogError($"Data not found or export failed : {dataId}");
                    }
                }
                //Logs.LogInfo("Finished loading data.");
                if (notify)
                {
                    RaiseDataReloaded();
                }
            }
            catch (Exception err)
            {
                Logs.LogError(err);
            }
        }


        protected void RaiseDataReloaded()
        {
            DataReloaded?.Invoke(this, EventArgs.Empty);
            NodeObject node = Node;
            if (node != null)
            {
                foreach (NodeComponent component in node.GetAllComponents())
                {
                    component.OnDataReloaded();
                }
            }
        }
        #endregion

        #region Component and message
        public T GetComponent<T>() where T : class
        {
            return Node?.GetComponent<T>();
        }
        public T GetComponent<T>(string name) where T : class
        {
            return Node?.GetComponent<T>(name);
        }
        public void SendMessage(string eventKey)
        {
            Node?.SendMessage(eventKey);
        }
        public void SendMessage(string eventKey, object value)
        {
            Node?.SendMessage(eventKey, value);
        }
        #endregion

        #region Network Report

        internal protected virtual bool VerifyNetworkCommand(NetworkCommand command)
        {
            return true;
        }

        internal protected virtual void ReportUserLogin(NetworkSession session)
        {
        }

        internal protected virtual void ReportUserLogout(NetworkSession session)
        {
        }

        internal protected virtual void ReportPurchaseItem(NetworkSession session, string productId, string token, string receipt)
        {
        }

        #endregion

        public override string ToString()
        {
            return ServiceId ?? base.ToString();
        }

        #region Static

        private static readonly object _sync = new object();
        private static NodeApplication _current;

        public static NodeApplication Current
        {
            get
            {
                return _current;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                lock (_sync)
                {
                    if (_current != null)
                    {
                        throw new InvalidOperationException("Application is already initialized.");
                    }
                    _current = value;
                }

                if (!_current.IsStarted)
                {
                    _current.Start();
                }
            }
        }

        #endregion
    }
}
