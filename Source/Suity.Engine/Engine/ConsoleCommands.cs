// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using Suity.Collections;
using Suity.Helpers;

namespace Suity.Engine
{
#region ConsoleCommandBase
    public abstract class ConsoleCommandBase : Suity.Object
    {
        readonly string _name;

        public string ArgPattern { get; }
        public string Description { get; }

        internal ConsoleCommandBase()
        {
        }
        internal ConsoleCommandBase(string name, string argPattern = null, string description = null)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            if (name.Contains(' '))
            {
                throw new ArgumentException("Name can not contains write spaces.");
            }
            ArgPattern = argPattern;
            Description = description;
        }

        protected override string GetName()
        {
            return _name;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Description))
            {
                if (!string.IsNullOrEmpty(ArgPattern))
                {
                    return string.Format($"{Name} {ArgPattern} : {Description}");
                }
                else
                {
                    return string.Format($"{Name} : {Description}");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ArgPattern))
                {
                    return string.Format($"{Name} {ArgPattern}");
                }
                else
                {
                    return Name;
                }
            }
        }
        public string GetPatternString()
        {
            if (!string.IsNullOrEmpty(ArgPattern))
            {
                return string.Format($"{Name} {ArgPattern}");
            }
            else
            {
                return Name;
            }
        }
    }
    #endregion

#region ConsoleCommand
    public abstract class ConsoleCommand : ConsoleCommandBase
    {
        readonly static Dictionary<Type, Func<string, object>> PrimativeParsers = new Dictionary<Type, Func<string, object>>();

        static ConsoleCommand()
        {
            PrimativeParsers.Add(typeof(Boolean), s => Convert.ToBoolean(s));
            PrimativeParsers.Add(typeof(Byte), s => Convert.ToByte(s));
            PrimativeParsers.Add(typeof(SByte), s => Convert.ToSByte(s));
            PrimativeParsers.Add(typeof(Int16), s => Convert.ToInt16(s));
            PrimativeParsers.Add(typeof(Int32), s => Convert.ToInt32(s));
            PrimativeParsers.Add(typeof(Int64), s => Convert.ToInt64(s));
            PrimativeParsers.Add(typeof(UInt16), s => Convert.ToUInt16(s));
            PrimativeParsers.Add(typeof(UInt32), s => Convert.ToUInt32(s));
            PrimativeParsers.Add(typeof(UInt64), s => Convert.ToUInt64(s));
            PrimativeParsers.Add(typeof(Single), s => Convert.ToSingle(s));
            PrimativeParsers.Add(typeof(Double), s => Convert.ToDouble(s));
            PrimativeParsers.Add(typeof(Decimal), s => Convert.ToDecimal(s));
        }

        public ConsoleCommand(string name, string argPattern = null, string description = null) : base(name, argPattern, description)
        {
        }

        public abstract void ExecuteComand(string[] args);

        protected T GetArg<T>(string[] args, int index, T defaultValue = default(T), bool safe = false)
        {
            if (args == null)
            {
                Logs.LogError("Argument array == null.");

                if (safe)
                {
                    return defaultValue;
                }
                else
                {
                    throw new ArgumentNullException(nameof(args));
                }
            }

            if (index < 0 && index >= args.Length)
            {
                Logs.LogError("Argument index out of range : " + index + ".");
                if (safe)
                {
                    return defaultValue;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object)args[index];
            }
#if BRIDGE
            if (typeof(T).IsPrimitive())
#else
            if (typeof(T).IsPrimitive)
#endif
            {
                Func<string, object> func = PrimativeParsers.GetValueOrDefault(typeof(T));
                if (func != null)
                {
                    try
                    {
                        return (T)func(args[index]);
                    }
                    catch (Exception)
                    {
                        Logs.LogError("Parse argument failed at index " + index + ".");
                        if (safe)
                        {
                            return defaultValue;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            if (typeof(T).IsEnum)
            {
                try
                {
                    return (T)Enum.Parse(typeof(T), args[index], true);
                }
                catch (Exception)
                {
                    Logs.LogError("Parse argument failed at index " + index + ".");
                    if (safe)
                    {
                        return defaultValue;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            Logs.LogError("Parse argument failed : " + args[index] + ".");
            if (safe)
            {
                return default(T);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
#endregion

#region ConsoleCommandMapped
    public abstract class ConsoleCommandMapped<TArg1> : ConsoleCommand
    {
        public ConsoleCommandMapped(string name, string argPattern = null, string description = null) : base(name, argPattern, description)
        {
        }

        public override void ExecuteComand(string[] args)
        {
            if (args.Length != 1)
            {
                Logs.LogError("Need 1 argument.");
                return;
            }
            TArg1 arg1;
            try
            {
                arg1 = GetArg<TArg1>(args, 0);
            }
            catch (Exception)
            {
                return;
            }

            ExecuteMapped(arg1);
        }
        protected abstract void ExecuteMapped(TArg1 arg1);
    }
    public abstract class ConsoleCommandMapped<TArg1, TArg2> : ConsoleCommand
    {
        public ConsoleCommandMapped(string name, string argPattern = null, string description = null) : base(name, argPattern, description)
        {
        }

        public override void ExecuteComand(string[] args)
        {
            if (args.Length != 2)
            {
                Logs.LogError("Need 2 arguments.");
                return;
            }
            TArg1 arg1;
            TArg2 arg2;
            try
            {
                arg1 = GetArg<TArg1>(args, 0);
                arg2 = GetArg<TArg2>(args, 1);
            }
            catch (Exception)
            {
                return;
            }

            ExecuteMapped(arg1, arg2);
        }
        protected abstract void ExecuteMapped(TArg1 arg1, TArg2 arg2);
    }
    public abstract class ConsoleCommandMapped<TArg1, TArg2, TArg3> : ConsoleCommand
    {
        public ConsoleCommandMapped(string name, string argPattern = null, string description = null) : base(name, argPattern, description)
        {
        }

        public override void ExecuteComand(string[] args)
        {
            if (args.Length != 3)
            {
                Logs.LogError("Need 3 arguments.");
                return;
            }
            TArg1 arg1;
            TArg2 arg2;
            TArg3 arg3;
            try
            {
                arg1 = GetArg<TArg1>(args, 0);
                arg2 = GetArg<TArg2>(args, 1);
                arg3 = GetArg<TArg3>(args, 2);
            }
            catch (Exception)
            {
                return;
            }

            ExecuteMapped(arg1, arg2, arg3);
        }
        protected abstract void ExecuteMapped(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }
    public abstract class ConsoleCommandMapped<TArg1, TArg2, TArg3, TArg4> : ConsoleCommand
    {
        public ConsoleCommandMapped(string name, string argPattern = null, string description = null) : base(name, argPattern, description)
        {
        }

        public override void ExecuteComand(string[] args)
        {
            if (args.Length != 4)
            {
                Logs.LogError("Need 4 arguments.");
                return;
            }
            TArg1 arg1;
            TArg2 arg2;
            TArg3 arg3;
            TArg4 arg4;
            try
            {
                arg1 = GetArg<TArg1>(args, 0);
                arg2 = GetArg<TArg2>(args, 1);
                arg3 = GetArg<TArg3>(args, 2);
                arg4 = GetArg<TArg4>(args, 3);
            }
            catch (Exception)
            {
                return;
            }

            ExecuteMapped(arg1, arg2, arg3, arg4);
        }
        protected abstract void ExecuteMapped(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
    }
#endregion

#region ConsoleCommandGroup
    public class ConsoleCommandGroup : ConsoleCommandBase
    {
        internal readonly Dictionary<string, ConsoleCommandBase> _commands = new Dictionary<string, ConsoleCommandBase>();

        internal ConsoleCommandGroup()
        {
        }
        public ConsoleCommandGroup(string name, string description = null) : base(name, description)
        {
        }

        protected void RegisterCommand(ConsoleCommandBase command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (command == this)
            {
                throw new ArgumentException("Command can not be self");
            }
            if (command is ConsoleCommandService)
            {
                throw new ArgumentException("Command can not be " + nameof(ConsoleCommandService));
            }
            if (_commands.ContainsKey(command.Name))
            {
                throw new ArgumentException("Name exists : " + command.Name);
            }

            _commands.Add(command.Name.ToLower(), command);
        }

        public ConsoleCommandBase GetCommand(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return _commands.GetValueOrDefault(name.ToLower());
        }

        public IEnumerable<ConsoleCommandBase> Commands => _commands.Values.Select(o => o);

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Description))
            {
                return string.Format($"{Name} (Group) : {Description}");
            }
            else
            {
                return $"{Name} (Group)";
            }
        }

        public IEnumerable<string> GetPatternStrings()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                yield return Name;

                foreach (var cmd in _commands.Values)
                {
                    if (cmd is ConsoleCommand consoleCommand)
                    {
                        yield return Name + " " + consoleCommand.GetPatternString();
                    }
                    else if (cmd is ConsoleCommandGroup consoleCommandGroup)
                    {
                        foreach (var str in consoleCommandGroup.GetPatternStrings())
                        {
                            yield return Name + " " + str;
                        }
                    }
                }
            }
            else
            {
                foreach (var cmd in _commands.Values)
                {
                    if (cmd is ConsoleCommand consoleCommand)
                    {
                        yield return consoleCommand.GetPatternString();
                    }
                    else if (cmd is ConsoleCommandGroup consoleCommandGroup)
                    {
                        foreach (var str in consoleCommandGroup.GetPatternStrings())
                        {
                            yield return str;
                        }
                    }
                }
            }
        }
    }
#endregion

#region ConsoleCommandRoot
    public sealed class ConsoleCommandService : ConsoleCommandGroup
    {
        public ConsoleCommandService()
        {
        }

        public void AddCommand(ConsoleCommandBase command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (command == this)
            {
                throw new ArgumentException();
            }
            if (command is ConsoleCommandService)
            {
                throw new ArgumentException();
            }
            RegisterCommand(command);
        }

        public void DoCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return;
            }
            if (command.All(v => v == ' '))
            {
                return;
            }
            string[] c = command.Split();
            DoCommand(c, 0);
        }
        public void DoCommand(string[] c, int index)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }
            if (c.Length == index)
            {
                // 无后续指令或参数，返回自己的帮助信息
                PrintHelp();
                return;
            }

            ConsoleCommandBase cmd = this;
            int argStart = 0;

            for (int i = index; i < c.Length; i++)
            {
                if (cmd is ConsoleCommandGroup consoleCommandGroup)
                {
                    string name = c[i];
                    cmd = consoleCommandGroup.GetCommand(name);
                    if (cmd == null)
                    {
                        Logs.LogError("Command not found : " + name);
                        return;
                    }
                }
                if (cmd is ConsoleCommand)
                {
                    argStart = i + 1;
                    break;
                }
            }

            if (cmd is ConsoleCommand consoleCommand)
            {
                if (c.Length > argStart)
                {
                    string[] args = new string[c.Length - argStart];
                    for (int i = 0; i < args.Length; i++)
                    {
                        args[i] = c[i + argStart];
                    }
                    consoleCommand.ExecuteComand(args);
                }
                else
                {
                    consoleCommand.ExecuteComand(EmptyArray<string>.Empty);
                }
            }
            else if (cmd is ConsoleCommandGroup consoleCommandGroup)
            {
                Logs.LogInfo($"<Command information : {cmd.Name}>");
                foreach (var item in consoleCommandGroup.Commands)
                {
                    Logs.LogInfo(item.ToString());
                }
            }
        }

        public void PrintHelp()
        {
            Logs.LogInfo($"<Command information>");
            foreach (var item in Commands)
            {
                Logs.LogInfo(item.ToString());
            }
        }
    }
#endregion

#region Commands
    class HelpCommand : ConsoleCommand
    {
        public HelpCommand()
            : base("help", null, "Show this message.")
        {
        }
        public override void ExecuteComand(string[] args)
        {
            NodeApplication.Current?.ConsoleCommand.PrintHelp();
        }
    }
    class InfoCommand : ConsoleCommand
    {
        public InfoCommand()
            : base("info", null, "Show application information.")
        {
        }

        public override void ExecuteComand(string[] args)
        {
            var app = NodeApplication.Current;
            if (app == null)
            {
                return;
            }

            Logs.LogInfo($"Service Id : {app.ServiceId}");
            Logs.LogInfo($"Is in operation : {app.IsInOperation}");
            Logs.LogInfo($"Galaxy name : {app.GalaxyName}");
            Logs.LogInfo($"Galaxy Id : {app.GalaxyId}");
            Logs.LogInfo($"Galaxy version : {app.GalaxyVersion}");
            Logs.LogInfo($"Application name : {app.ApplicationName}");
            Logs.LogInfo($"Data Id : {app.DataId}");
            Logs.LogInfo($"Data version : {app.DataVersion}");
            Logs.LogInfo($"Multiple launch index : {app.MultipleLaunchIndex}");
            Logs.LogInfo($"Public IP address : {app.PublicIPAddress}");
            Logs.LogInfo($"Internal IP address : {app.InternalIPAddress}");
        }
    }
    class ReloadCommand : ConsoleCommand
    {
        public ReloadCommand()
            : base("reload", null, "Reload all data from data source.")
        {
        }
        public override void ExecuteComand(string[] args)
        {
            NodeApplication.Current?.LoadData();
        }
    }
#endregion
}
