// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Helpers;
using Suity.Reflecting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;

namespace Suity.Engine.Debugging
{
    public static class Debugger
    {
        public const int DebuggerPort = 8666;
        public const string DebuggerServiceName = "SuityEditorDebug";
        public static bool NetworkLog = false;

        static internal HashSet<string> probePaths = new HashSet<string>();

        static internal IDebugHostService _debugHost;
        static internal IDebugInstanceService _debugInstance;

        public static string GalaxyFile { get; private set; }
        public static string NodeName { get; private set; }


        public static void Start(string galaxyFile, string nodeName)
        {
            Environment.InitializeDevice(DebugDevice.Instance);

            PrintTitle();

            if (NodeApplication.Current != null)
            {
                throw new InvalidOperationException("NodeApplication is started.");
            }

            if (string.IsNullOrEmpty(galaxyFile))
            {
                throw new ArgumentException("GalaxyFile can not be empty.", nameof(galaxyFile));
            }
            if (string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentException("ServiceName can not be empty.", nameof(nodeName));
            }

            if (!galaxyFile.EndsWith(".sasset", StringComparison.OrdinalIgnoreCase))
            {
                galaxyFile = galaxyFile + ".sasset";
            }

            GalaxyFile = galaxyFile;
            NodeName = nodeName;

            bool started = false;

            do
            {
                if (_debugHost != null)
                {
                    break;
                }
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                InitializeAssemblies();

                try
                {
                    _debugHost = GetDebugService();
                    if (_debugHost == null)
                    {
                        break;
                    }

                    var nodeStartInfo = _debugHost.GetStartInfo(galaxyFile, nodeName);
                    if (nodeStartInfo == null)
                    {
                        Console.WriteLine("Galaxy file or node name not found.");
                        break;
                    }

                    Console.WriteLine($"Starting application using : {nodeStartInfo.GalaxyName} - {nodeStartInfo.ApplicationName}");
                    Console.WriteLine();

                    DebugNode debugNode = new DebugNode(nodeStartInfo);
                    _debugInstance = _debugHost.CreateDebugInstance(nodeStartInfo, debugNode);
                    if (_debugInstance == null)
                    {
                        Console.WriteLine("Create debugger instance failed.");
                        break;
                    }

                    debugNode.Start(_debugHost, _debugInstance);
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.ToString());
                    break;
                }

                started = true;
            } while (false);

            if (started)
            {
                Console.WriteLine();
                (NodeApplication.Current as DebugApplication)?.ConsoleCommand.PrintHelp();
                ReadLineProcess();
            }
            else
            {
                Console.WriteLine("NodeApplication is not started. Press any key to exit.");
                Console.ReadKey();
            }
        }
        public static void Start(params Type[] compoentTypes)
        {
            Environment.InitializeDevice(DebugDevice.Instance);

            PrintTitle();

            if (compoentTypes == null)
            {
                throw new ArgumentNullException(nameof(compoentTypes));
            }
            if (NodeApplication.Current != null)
            {
                throw new InvalidOperationException("NodeApplication is started.");
            }

            bool started = false;

            do
            {
                if (_debugHost != null)
                {
                    break;
                }
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                InitializeAssemblies();

                try
                {
                    _debugHost = GetDebugService();
                    if (_debugHost == null)
                    {
                        break;
                    }

                    Console.WriteLine("Starting application with custom configuration ...");
                    Console.WriteLine();

                    var nodeStartInfo = MakeCustomNodeStartInfo(compoentTypes);

                    DebugNode debugNode = new DebugNode(nodeStartInfo);
                    _debugInstance = _debugHost.CreateDebugInstance(nodeStartInfo, debugNode);
                    if (_debugInstance == null)
                    {
                        Console.WriteLine("Create debugger instance failed.");
                        break;
                    }

                    debugNode.Start(_debugHost, _debugInstance);
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.ToString());
                    break;
                }

                started = true;
            } while (false);

            if (started)
            {
                Console.WriteLine();
                (NodeApplication.Current as DebugApplication)?.ConsoleCommand.PrintHelp();
                ReadLineProcess();
            }
            else
            {
                Console.WriteLine("NodeApplication is not started. Press any key to exit.");
                Console.ReadKey();
            }
        }

        private static void InitializeAssemblies()
        {
            //强行加载全部程序集
            var dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            foreach (var fileInfo in dirInfo.GetFiles("*.dll"))
            {
                try
                {
                    Assembly.LoadFile(fileInfo.FullName);
                }
                catch (BadImageFormatException err)
                {
                    Logs.LogWarning($"Skipping bad image assembly : {fileInfo.Name}");
                }
            }
            foreach (var fileInfo in dirInfo.GetFiles("*.exe"))
            {
                try
                {
                    Assembly.LoadFile(fileInfo.FullName);
                }
                catch (BadImageFormatException err)
                {
                    Logs.LogWarning($"Skipping bad image assembly : {fileInfo.Name}");
                }
            }

            //Console.WriteLine("Loaded Assembly :");
            //foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    Console.WriteLine(asm.FullName);
            //}
            //Console.WriteLine();
        }
        private static IDebugHostService GetDebugService()
        {
            //启动Remoting
            string url = $"tcp://localhost:{DebuggerPort}/{DebuggerServiceName}";
            int channelPort = NetworkHelper.AllocateTcpPort(9500, 30000);

            TcpChannel channel = new TcpChannel(channelPort);
            ChannelServices.RegisterChannel(channel, true);

            var debug = (IDebugHostService)Activator.GetObject(typeof(IDebugHostService), url);

            if (debug == null)
            {
                Console.WriteLine("Connecting to SuityEditor Failed : Couldn't crate Remoting Object 'IEditorDebugService'.");
            }

            return debug;
        }
        private static NodeStartInfo MakeCustomNodeStartInfo(IEnumerable<Type> components, IDictionary<string, string> configs = null)
        {
            string asmName = Assembly.GetCallingAssembly().GetShortAssemblyName();

            NodeStartInfo nodeStartInfo = new NodeStartInfo
            {
                NodeId = asmName,
                ApplicationName = asmName,
                ApplicationId = "Debug",
                PackageDirectory = AppDomain.CurrentDomain.BaseDirectory,
                AppDirectory = AppDomain.CurrentDomain.BaseDirectory,
                ModuleDirectory = AppDomain.CurrentDomain.BaseDirectory.PathAppend("Modules")
            };

            foreach (var compType in components)
            {
                if (!typeof(NodeComponent).IsAssignableFrom(compType))
                {
                    throw new ArgumentException($"{compType.Name} is not derived from NodeComponent.");
                }
                NodeComponentStartInfo componentStartInfo = new NodeComponentStartInfo
                {
                    FullTypeName = compType.GetTypeId(),
                    ComponentData = new NodeQuery.RawNode(),
                };
                nodeStartInfo.Components.Add(componentStartInfo);
            }

            if (configs != null)
            {
                foreach (var pair in configs)
                {
                    EnvironmentConfigInfo info = new EnvironmentConfigInfo
                    {
                        Name = pair.Key,
                        Value = pair.Value,
                        Override = true,
                    };
                    nodeStartInfo.Configs.Add(info);
                }
            }

            return nodeStartInfo;
        }

        private static void ReadLineProcess()
        {
            while (true)
            {
                string str = Console.ReadLine();
                if (string.IsNullOrEmpty(str))
                {
                    continue;
                }
                NodeApplication.Current?.ConsoleCommand.DoCommand(str);
                if (str.Length == 4 && string.Equals(str, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            if (!string.IsNullOrEmpty(GalaxyFile))
            {
                try
                {
                    _debugInstance?.NodeLeaving(GalaxyFile);
                }
                catch (Exception)
                {
                }
            }
        }
        private static void PrintTitle()
        {
            Console.WriteLine("[Suity Editor 4 Local Debugger]");
            Console.WriteLine();
        }

        internal static void AddFileNameForProbePath(string fileName)
        {
            var path = Path.GetDirectoryName(fileName);
            probePaths.Add(path);
        }
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //Console.WriteLine("Resolving " + args.Name + " ...");

            //Mod by simage
            var current = AppDomain.CurrentDomain.GetAssemblies().Where(o => o.FullName == args.Name).FirstOrDefault();
            if (current != null)
            {
                return current;
            }
            //EndMod by simage

            var name = new AssemblyName(args.Name);
            foreach (var path in probePaths)
            {
                var dllPath = Path.Combine(path, string.Format("{0}.dll", name.Name));
                //Console.WriteLine("Checking " + dllPath + "...");

                if (File.Exists(dllPath))
                {
                    return Assembly.LoadFile(dllPath);
                }

                var exePath = Path.ChangeExtension(dllPath, "exe");
                //Console.WriteLine("Checking " + exePath + "...");

                if (File.Exists(exePath))
                {
                    return Assembly.LoadFile(exePath);
                }
            }

            Console.WriteLine("Can not resolve " + args.Name + ".");
            // Not found.
            return null;
        }
    }

}
