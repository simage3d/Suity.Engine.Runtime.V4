// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Suity.Collections;
using Suity.Controlling;
using Suity.Helpers;
using Suity.Json;
using Suity.NodeQuery;
using Suity.Views;

namespace Suity
{
    [MultiThreadSecurity(MultiThreadSecurityMethods.ReadonlySecure)]
    /// <summary>
    /// 对象类型全局管理器
    /// </summary>
    public static class ObjectType
    {
        #region Delegate
        /// <summary>
        /// 格式器数据读取委托
        /// </summary>
        /// <param name="reader">读取器</param>
        /// <returns></returns>
        public delegate object ReadDelegate(IDataReader reader);
        /// <summary>
        /// 格式器数据写入委托
        /// </summary>
        /// <param name="writer">编写器</param>
        /// <param name="obj"></param>
        public delegate void WriteDelegate(IDataWriter writer, object obj);
        /// <summary>
        /// 格式器数据克隆委托
        /// </summary>
        /// <param name="source"></param>
        /// <param name="autoNew"></param>
        /// <returns></returns>
        public delegate object CloneDelegate(object source, object target, bool autoNew);
        /// <summary>
        /// 格式器数据比较委托
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public delegate bool EqualsDelegate(object obj1, object obj2);
        /// <summary>
        /// 格式器读取属性委托
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public delegate object GetPropertyDelegate(object obj, string name);
        /// <summary>
        /// 格式器写入属性委托
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public delegate void SetPropertyDelegate(object obj, string name, object value);
        /// <summary>
        /// 格式器同步属性委托
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sync"></param>
        /// <param name="context"></param>
        public delegate void ExchangeDelegate(object obj, IExchange ex);
        /// <summary>
        /// 函数委托
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public delegate object FunctionDelegate(FunctionContext context);


        #endregion

        readonly static Dictionary<string, TypeFamily> _families = new Dictionary<string, TypeFamily>();
        
        readonly static Dictionary<Type, TypeInfo> _typeInfos = new Dictionary<Type, TypeInfo>();
        readonly static Dictionary<string, TypeInfo> _typeInfosById = new Dictionary<string, TypeInfo>();
        readonly static Dictionary<string, TypeInfo> _typeInfosByTypeName = new Dictionary<string, TypeInfo>();
        readonly static Dictionary<string, FunctionInfo> _functions = new Dictionary<string, FunctionInfo>();

        static readonly Dictionary<string, Dictionary<Type, AssetImplementInfo>> _assetImplements = new Dictionary<string, Dictionary<Type, AssetImplementInfo>>();
        static readonly Dictionary<Type, string> _serviceTypeNames = new Dictionary<Type, string>();

        public static void EmptyDelegate(FunctionContext ctx) { }

        static ObjectType()
        {
            TypeFamily pFamily = new TypeFamily("*System");
            pFamily.RegisterPrimaryInfos();
            pFamily.RegisterClassType(typeof(Controller), AssetDefinitionCodes.MaskAssetTypeName(AssetDefinitionCodes.Controller), null);
            RegisterFamily(pFamily);

            SuityFormatter.Initialize();
        }

        #region Register
        public static void RegisterFamily(TypeFamily family)
        {
            if (family == null)
            {
                throw new ArgumentNullException(nameof(family));
            }

            _families[family.Key] = family;

            foreach (var info in family.TypeInfos.SkipNull())
            {
                RegisterType(info);

                if (info is ClassTypeInfo)
                {
                    RegisterAssetImplement(info.Key, typeof(ClassTypeInfo), info);
                }
                else if (info is EnumTypeInfo)
                {
                    RegisterAssetImplement(info.Key, typeof(EnumTypeInfo), info);
                }
            }
            foreach (var info in family.FunctionInfos.SkipNull())
            {
                _functions.Add(info.Name, info);
            }

            RegisterAssetImplement(family.Key, typeof(TypeFamily), family);
        }


        public static void RegisterClassType<T>()
        {
            RegisterClassType(typeof(T)); 
        }
        public static void RegisterClassType(Type type)
        {
            ClassTypeInfo info = new ClassTypeInfo(type, type.FullName, type.FullName);
            RegisterType(info);

            EnsureAssetImplements(info.Key)[typeof(ClassTypeInfo)] = new AssetImplementInfo(info.Key, typeof(ClassTypeInfo), info);
        }
        public static void RegisterClassType(Type type, string typeName, string aliasName = null)
        {
            ClassTypeInfo info = new ClassTypeInfo(type, typeName, aliasName);
            RegisterType(info);

            EnsureAssetImplements(info.Key)[typeof(ClassTypeInfo)] = new AssetImplementInfo(info.Key, typeof(ClassTypeInfo), info);
        }

        /// <summary>
        /// 注册格式器使用的对象类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <param name="aliasName"></param>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="cloner"></param>
        /// <param name="equalsMatcher"></param>
        /// <param name="propGetter"></param>
        /// <param name="propSetter"></param>
        public static ClassTypeInfo RegisterClassType(
            Type type,
            string typeName,
            string aliasName,
            PacketFormats packetFormat,

            ReadDelegate reader = null,
            WriteDelegate writer = null,
            CloneDelegate cloner = null,
            EqualsDelegate equalsMatcher = null,
            ExchangeDelegate syncer = null,
            GetPropertyDelegate propGetter = null,
            SetPropertyDelegate propSetter = null
            )
        {
            //if (type == null)
            //{
            //    throw new ArgumentNullException(nameof(type));
            //}
            //if (string.IsNullOrEmpty(typeName))
            //{
            //    throw new ArgumentNullException(nameof(typeName));
            //}
            //if (reader == null)
            //{
            //    throw new ArgumentNullException(nameof(reader));
            //}
            //if (writer == null)
            //{
            //    throw new ArgumentNullException(nameof(writer));
            //}
            //if (cloner == null)
            //{
            //    throw new ArgumentNullException(nameof(cloner));
            //}
            //if (syncer == null)
            //{
            //    throw new ArgumentNullException(nameof(syncer));
            //}

            if (string.IsNullOrEmpty(aliasName))
            {
                aliasName = type.FullName;
            }

            ClassTypeInfo info = new ClassTypeInfo(type, typeName, aliasName, packetFormat, reader, writer, cloner, equalsMatcher, syncer, propGetter, propSetter);
            RegisterType(info);

            EnsureAssetImplements(info.Key)[typeof(ClassTypeInfo)] = new AssetImplementInfo(info.Key, typeof(ClassTypeInfo), info);

            return info;
        }

        /// <summary>
        /// 注册格式器使用的枚举类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <param name="aliasName"></param>
        public static EnumTypeInfo RegisterEnumType(Type type, string typeName, string aliasName = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            EnumTypeInfo info = new EnumTypeInfo(type, typeName, aliasName);
            RegisterType(info);

            EnsureAssetImplements(info.Key)[typeof(EnumTypeInfo)] = new AssetImplementInfo(info.Key, typeof(EnumTypeInfo), info);

            return info;
        }

        private static void RegisterType(TypeInfo info)
        {
            _typeInfos[info.Type] = info;
            _typeInfosById[info.Key] = info;
            _typeInfosByTypeName[info.Type.FullName] = info;
            if (!string.IsNullOrEmpty(info.AliasName))
            {
                _typeInfosById[info.AliasName] = info;
            }
        }

        public static void RegisterFunction(string name, FunctionDelegate handler)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            FunctionInfo info = new FunctionInfo(name, handler);
            _functions[name] = info;
        }

        public static void RegisterAssetDefinition(string assetTypeName, Type serviceType)
        {
            if (string.IsNullOrEmpty(assetTypeName))
            {
                throw new ArgumentException("assetTypeName is empty", nameof(assetTypeName));
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            _serviceTypeNames[serviceType] = AssetDefinitionCodes.MaskAssetTypeName(assetTypeName);
        }

        #endregion

        #region Resolve
        public static IEnumerable<TypeFamily> Families => _families.Values.Select(o => o);
        public static TypeFamily GetFamily(string familyName) => _families.GetValueOrDefault(familyName);



        public static TypeInfo GetTypeInfo(Type type) => _typeInfos.GetValueOrDefault(type);
        public static TypeInfo GetTypeInfo(string typeName)
        {
            TypeInfo info = _typeInfosByTypeName.GetValueOrDefault(typeName);
            if (info == null)
            {
                info = _typeInfosById.GetValueOrDefault(typeName);
            }

            return info;
        }

        public static ClassTypeInfo GetClassTypeInfo(Type type) => _typeInfos.GetValueOrDefault(type) as ClassTypeInfo;
        public static EnumTypeInfo GetEnumTypeInfo(Type type) => _typeInfos.GetValueOrDefault(type) as EnumTypeInfo;

        public static ClassTypeInfo GetClassTypeInfo(string typeName)
        {
            ClassTypeInfo info = _typeInfosById.GetValueOrDefault(typeName) as ClassTypeInfo;
            if (info == null)
            {
                info = _typeInfosByTypeName.GetValueOrDefault(typeName) as ClassTypeInfo;
            }

            return info;
        }
        public static EnumTypeInfo GetEnumTypeInfo(string typeName)
        {
            EnumTypeInfo info = _typeInfosById.GetValueOrDefault(typeName) as EnumTypeInfo;
            if (info == null)
            {
                info = _typeInfosByTypeName.GetValueOrDefault(typeName) as EnumTypeInfo;
            }

            return info;
        }

        public static object CreateObject(string typeName)
        {
            Type type = GetTypeInfo(typeName)?.Type;
            if (type != null)
            {
                if (type.IsEnum)
                {
                    return Enum.GetValues(type).GetValue(0);
                }
                else
                {
                    //经过测试il2cpp可以使用。
                    return Activator.CreateInstance(type);
                }
            }
            return null;
        }

        public static string ResolveAssetDefinition(Type definitionType)
        {
            if (definitionType == null)
            {
                return null;
            }

            string typeName = _serviceTypeNames.GetValueOrDefault(definitionType);
            if (typeName != null)
            {
                return typeName;
            }

            AssetDefinitionTypeAttribute attr = definitionType.GetAttributeCached<AssetDefinitionTypeAttribute>();
            typeName = attr?.AssetTypeName ?? definitionType.GetTypeId();

            if (!string.IsNullOrEmpty(typeName))
            {
                typeName = AssetDefinitionCodes.MaskAssetTypeName(typeName);
                _serviceTypeNames[definitionType] = typeName;
                return typeName;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Asset Implements

        /// <summary>
        /// 注册资源实现类型
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="implementType">实现类型</param>
        public static void RegisterAssetImplement(string key, Type implementType, object instance = null)
        {
            if (implementType == null)
            {
                throw new ArgumentNullException(nameof(implementType));
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is emtpy.", nameof(key));
            }
            if (instance != null && !implementType.IsAssignableFrom(instance.GetType()))
            {
                throw new ArgumentException("Instance type mismatch.", nameof(instance));
            }

            AssetImplementInfo info = new AssetImplementInfo(key, implementType, instance);

            EnsureAssetImplements(key)[implementType] = info;
        }
        /// <summary>
        /// 注销资源实现类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="implementType"></param>
        public static void UnregisterAssetImplement(string key, Type implementType)
        {
            if (implementType == null)
            {
                throw new ArgumentNullException(nameof(implementType));
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is emtpy", nameof(key));
            }

            GetAssetImplements(key)?.Remove(implementType);
        }


        public static bool ContainsAssetImplement(string key, Type baseType)
        {
            var types = GetAssetImplements(key);
            if (types != null)
            {
                return types.Keys.Any(o => baseType.IsAssignableFrom(o));
            }

            return false;
        }

        /// <summary>
        /// 解算资源实现类型
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="baseType">资源基类型</param>
        /// <returns></returns>
        public static Type ResolveAssetImplementType(string key, Type baseType)
        {
            var types = GetAssetImplements(key);
            if (types != null)
            {
                return types.Keys.FirstOrDefault(o => baseType.IsAssignableFrom(o));
            }

            return null;
        }

        /// <summary>
        /// 获取资源实现的单个实例，每次返回的是同一个实例。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="serviceType"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public static object GetAssetImplement(string key, Type serviceType, bool autoCreate = true)
        {
            var info = ResolveAssetImplementInfo(key, serviceType);
            if (info != null)
            {
                return info.GetInstance(autoCreate);
            }

            return null;
        }
        /// <summary>
        /// 获取资源实现的单个实例，每次返回的是同一个实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public static T GetAssetImplement<T>(string key, bool autoCreate = true) where T : class
        {
            var info = ResolveAssetImplementInfo(key, typeof(T));
            if (info != null)
            {
                return (T)info.GetInstance(autoCreate);
            }

            return null;
        }

        /// <summary>
        /// 获取资源实现的单个实例，每次返回的是不同的实例。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object CreateAssetImplement(string key, Type serviceType)
        {
            var info = ResolveAssetImplementInfo(key, serviceType);
            if (info != null)
            {
                return info.CreateInstance();
            }

            return null;
        }
        /// <summary>
        /// 获取资源实现的单个实例，每次返回的是不同的实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T CreateAssetImplement<T>(string key) where T : class
        {
            var info = ResolveAssetImplementInfo(key, typeof(T));
            if (info != null)
            {
                return (T)info.CreateInstance();
            }

            return null;
        }


        private static AssetImplementInfo ResolveAssetImplementInfo(string key, Type baseType)
        {
            var types = GetAssetImplements(key);
            if (types != null)
            {
                return types.Values.FirstOrDefault(o => baseType.IsAssignableFrom(o.ServiceType));
            }

            return null;
        }
        private static Dictionary<Type, AssetImplementInfo> EnsureAssetImplements(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _assetImplements.GetValueOrCreate(key, () => new Dictionary<Type, AssetImplementInfo>());
        }
        private static Dictionary<Type, AssetImplementInfo> GetAssetImplements(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            return _assetImplements.GetValueOrDefault(typeName);
        }

        #endregion

        #region IObjectResolver

        public static IObjectResolver ObjectResolver { get; set; }

        #endregion

        #region Read
        public static bool TryReadObject(IDataReader reader, out string typeName, out object obj, out bool isArray)
        {
            typeName = null;
            obj = null;
            isArray = false;

            if (reader == null)
            {
                return false;
            }
            if (reader.ReadIsEmpty())
            {
                return true;
            }

            typeName = reader.ReadTypeName();
            if (string.IsNullOrEmpty(typeName))
            {
                return false;
            }

            //增加数组支持
            isArray = typeName.EndsWith("[]");
            if (isArray)
            {
                typeName = typeName.RemoveFromLast(2);
            }

            try
            {
                TypeInfo info = GetTypeInfo(typeName);
                if (info == null)
                {
                    return false;
                }

                if (info.IsClass)
                {
                    if (isArray)
                    {
                        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { info.Type }));
                        foreach (var childReader in reader.Nodes("Array"))
                        {
                            list.Add(Read(info.Type, childReader));
                        }
                        obj = list;
                        return true;
                    }
                    else
                    {
                        obj = (info as ClassTypeInfo)?.Reader(reader);
                        return true;
                    }
                }
                else if (info.IsEnum)
                {
                    if (isArray)
                    {
                        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { info.Type }));
                        foreach (var childReader in reader.Nodes("Array"))
                        {
                            list.Add(ParseEnumString(info.Type, childReader.ReadString()));
                        }
                        obj = list;
                        return true;
                    }
                    else
                    {
                        obj = ParseEnumString(info.Type, reader.ReadString());
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }

            return false;
        }
        public static object ReadObject(IDataReader reader, Type hintType = null)
        {
            if (reader == null || reader.ReadIsEmpty())
            {
                return null;
            }
            string typeName = reader.ReadTypeName();
            if (string.IsNullOrEmpty(typeName) && hintType == null)
            {
                return null;
            }

            //增加数组支持
            bool isArray = typeName?.EndsWith("[]") == true;
            if (isArray)
            {
                typeName = typeName.RemoveFromLast(2);
            }

            TypeInfo info = GetTypeInfo(typeName);
            if (info == null && hintType != null)
            {
                info = _typeInfos.GetValueOrDefault(hintType);
            }

            if (info == null)
            {
                throw new InvalidOperationException($"ReadObject failed, type name not found : {typeName}.");
            }

            try
            {

                if (info.IsClass)
                {
                    if (isArray)
                    {
                        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { info.Type }));
                        if (info.IsPrimitive)
                        {
                            foreach (var childReader in reader.Nodes("Array"))
                            {
                                list.Add(((ClassTypeInfo)info).Reader(childReader.Node("Value")));
                            }
                        }
                        else
                        {
                            foreach (var childReader in reader.Nodes("Array"))
                            {
                                list.Add(Read(info.Type, childReader));
                            }
                        }
                        return list;
                    }
                    else
                    {
                        if (info.IsPrimitive)
                        {
                            return ((ClassTypeInfo)info).Reader(reader.Node("Value"));
                        }
                        else
                        {
                            return ((ClassTypeInfo)info).Reader(reader);
                        }
                    }
                }
                else if (info.IsEnum)
                {
                    if (isArray)
                    {
                        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { info.Type }));
                        foreach (var childReader in reader.Nodes("Array"))
                        {
                            list.Add(ParseEnumString(info.Type, childReader.ReadString()));
                        }
                        return list;
                    }
                    else
                    {
                        return ParseEnumString(info.Type, reader.ReadString());
                    }
                }
            }
            catch (Exception e)
            {
                throw new DataException($"ReadObject failed : {typeName}", e);
            }

            throw new InvalidOperationException($"ReadObject failed.");
        }


        public static object Read(IDataReader reader, ReadDelegate read)
        {
            if (reader == null || reader.ReadIsEmpty())
            {
                return null;
            }
            return read(reader);
        }
        public static T Read<T>(IDataReader reader)
        {
            if (reader == null || reader.ReadIsEmpty())
            {
                return default(T);
            }

            if (_typeInfos.TryGetValue(typeof(T), out TypeInfo info) && info is ClassTypeInfo classTypeInfo)
            {
                return (T)classTypeInfo.Reader(reader);
            }

            throw new InvalidOperationException($"Class type formatter not found : {typeof(T)}.");
        }
        public static object Read(Type type, IDataReader reader)
        {
            if (reader == null || reader.ReadIsEmpty())
            {
                return null;
            }

            if (_typeInfos.TryGetValue(type, out TypeInfo info) && info is ClassTypeInfo classTypeInfo)
            {
                return classTypeInfo.Reader(reader);
            }

            throw new InvalidOperationException($"Class type formatter not found : {type}.");
        }
        public static object ReadEnum(IDataReader reader, Type enumType)
        {
            if (reader != null)
            {
                return ParseEnumString(enumType, reader.ReadString());
            }
            else
            {
                return ParseEnumString(enumType, null);
            }
        }
        public static object ParseEnum(Type enumType, object obj)
        {
            if (obj == null)
            {
                return ParseEnumString(enumType, null);
            }
            else if (obj.GetType() == enumType)
            {
                return obj;
            }
            else
            {
                return ParseEnumString(enumType, obj.ToString());
            }
        }
        public static object ParseEnumString(Type enumType, string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    return Enum.Parse(enumType, str);
                }
                catch (Exception)
                {
                }
            }

            try
            {
                var values = Enum.GetValues(enumType);
                return values.Length > 0 ? values.GetValue(0) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static bool ReadBoolean(IDataReader reader)
        {
            return reader?.ReadBoolean() ?? default(bool);
        }
        public static byte ReadByte(IDataReader reader)
        {
            return reader?.ReadByte() ?? default(byte);
        }
        public static sbyte ReadSByte(IDataReader reader)
        {
            return reader?.ReadSByte() ?? default(sbyte);
        }
        public static short ReadInt16(IDataReader reader)
        {
            return reader?.ReadInt16() ?? default(short);
        }
        public static ushort ReadUInt16(IDataReader reader)
        {
            return reader?.ReadUInt16() ?? default(ushort);
        }
        public static int ReadInt32(IDataReader reader)
        {
            return reader?.ReadInt32() ?? default(int);
        }
        public static uint ReadUInt32(IDataReader reader)
        {
            return reader?.ReadUInt32() ?? default(uint);
        }
        public static long ReadInt64(IDataReader reader)
        {
            return reader?.ReadInt64() ?? default(long);
        }
        public static ulong ReadUInt64(IDataReader reader)
        {
            return reader?.ReadUInt64() ?? default(ulong);
        }
        public static float ReadSingle(IDataReader reader)
        {
            return reader?.ReadSingle() ?? default(float);
        }
        public static double ReadDouble(IDataReader reader)
        {
            return reader?.ReadDouble() ?? default(double);
        }
        public static string ReadString(IDataReader reader)
        {
            return reader?.ReadString();
        }
        public static DateTime ReadDateTime(IDataReader reader)
        {
            return reader.ReadDateTime();
        }

        #endregion

        #region Write
        public static void Write(IDataWriter writer, WriteDelegate write, object obj)
        {
            if (obj == null)
            {
                writer.WriteEmpty(true);
                return;
            }
            writer.WriteEmpty(false);
            write(writer, obj);
        }
        public static void Write<T>(IDataWriter writer, T obj)
        {
            if (obj == null)
            {
                writer.WriteEmpty(true);
                return;
            }

            if (_typeInfos.TryGetValue(typeof(T), out TypeInfo info))
            {
                if (info is ClassTypeInfo clsInfo)
                {
                    writer.WriteEmpty(false);
                    clsInfo.Writer(writer, obj);
                    return;
                }

                if (info.IsEnum)
                {
                    writer.WriteString(obj.ToString());
                    return;
                }
            }

            throw new InvalidOperationException($"Type formatter not found : {typeof(T)}.");
        }
        public static void Write(Type type, IDataWriter writer, object obj)
        {
            if (obj == null)
            {
                writer.WriteEmpty(true);
                return;
            }

            if (_typeInfos.TryGetValue(type, out TypeInfo info))
            {
                if (info is ClassTypeInfo clsInfo)
                {
                    writer.WriteEmpty(false);
                    clsInfo.Writer(writer, obj);
                    return;
                }

                if (info.IsEnum)
                {
                    writer.WriteString(obj.ToString());
                    return;
                }
            }
            throw new InvalidOperationException($"Type formatter not found : {type}.");
        }
        public static void WriteObject(IDataWriter writer, object obj, bool forceFullName = false)
        {
            if (obj == null)
            {
                writer.WriteEmpty(true);
                return;
            }

            Type objType = obj.GetType();

            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type type = objType.GetGenericArguments()[0];
                IList list = (IList)obj;
                WriteObjects(writer, type, list.Count, list);
                return;
            }
            else if (objType.IsArray)
            {
                Type type = objType.GetElementType();
                Array ary = (Array)obj;
                WriteObjects(writer, type, ary.Length, ary);
                return;
            }

            if (_typeInfos.TryGetValue(objType, out TypeInfo info))
            {
                string name = forceFullName ? info.FullName : info.Name;

                if (info is ClassTypeInfo clsInfo)
                {
                    if (info.IsPrimitive)
                    {
                        writer.WriteEmpty(false);
                        writer.WriteTypeName(name);
                        clsInfo.Writer(writer.Node("Value"), obj);
                    }
                    else
                    {
                        writer.WriteEmpty(false);
                        writer.WriteTypeName(name);
                        clsInfo.Writer(writer, obj);
                    }
                }
                else if (objType.IsEnum)
                {
                    writer.WriteEmpty(false);
                    writer.WriteTypeName(name);
                    writer.WriteString(obj.ToString());
                }
            }
            else
            {
                throw new InvalidOperationException($"Type formatter not found : {obj.GetType().FullName}.");
            }
        }
        public static void WriteObjects<T>(IDataWriter writer, IList<T> list)
        {
            if (list == null)
            {
                writer.WriteEmpty(true);
                return;
            }
            WriteObjects(writer, typeof(T), list.Count, list);
        }
        public static void WriteObjects<T>(IDataWriter writer, T[] array)
        {
            if (array == null)
            {
                writer.WriteEmpty(true);
                return;
            }
            WriteObjects(writer, typeof(T), array.Length, array);
        }
        private static void WriteObjects(IDataWriter writer, Type type, int count, IEnumerable objs, bool forceFullName = false)
        {
            if (objs == null)
            {
                writer.WriteEmpty(true);
                return;
            }

            if (_typeInfos.TryGetValue(type, out TypeInfo info))
            {
                string name = forceFullName ? info.FullName : info.Name;

                if (info is ClassTypeInfo clsInfo)
                {
                    writer.WriteEmpty(false);
                    writer.WriteTypeName(name + "[]");
                    IDataArrayWriter aryWriter = writer.Nodes("Array", count);
                    foreach (var obj in objs)
                    {
                        clsInfo.Writer(aryWriter.Item(), obj);
                    }
                    aryWriter.Finish();
                }
                else if (info.Type.IsEnum)
                {
                    writer.WriteEmpty(false);
                    writer.WriteTypeName(name + "[]");
                    IDataArrayWriter aryWriter = writer.Nodes("Array", count);
                    foreach (var obj in objs)
                    {
                        aryWriter.Item().WriteString(obj.ToString());
                    }
                    aryWriter.Finish();
                }
            }
            else
            {
                throw new InvalidOperationException($"Type formatter not found : {type}.");
            }
        }
        public static void WriteEnum(IDataWriter writer, Enum value)
        {
            writer.WriteString(value.ToString());
        }


        public static void WritePrimative(IDataWriter writer, object value)
        {
            switch (value)
            {
                case bool boolValue:
                    WriteBoolean(writer, boolValue);
                    break;
                case byte byteValue:
                    WriteByte(writer, byteValue);
                    break;
                case sbyte sbyteValue:
                    WriteSByte(writer, sbyteValue);
                    break;
                case short shortValue:
                    WriteInt16(writer, shortValue);
                    break;
                case ushort ushortValue:
                    WriteUInt16(writer, ushortValue);
                    break;
                case int intValue:
                    WriteInt32(writer, intValue);
                    break;
                case uint uintValue:
                    WriteUInt32(writer, uintValue);
                    break;
                case long longValue:
                    WriteInt64(writer, longValue);
                    break;
                case ulong ulongValue:
                    WriteUInt64(writer, ulongValue);
                    break;
                case float floatValue:
                    WriteSingle(writer, floatValue);
                    break;
                case double doubleValue:
                    WriteDouble(writer, doubleValue);
                    break;
                case string stringValue:
                    WriteString(writer, stringValue);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        public static void WriteBoolean(IDataWriter writer, bool value)
        {
            writer.WriteBoolean(value);
        }
        public static void WriteByte(IDataWriter writer, byte value)
        {
            writer.WriteByte(value);
        }
        public static void WriteSByte(IDataWriter writer, sbyte value)
        {
            writer.WriteSByte(value);
        }
        public static void WriteInt16(IDataWriter writer, short value)
        {
            writer.WriteInt16(value);
        }
        public static void WriteUInt16(IDataWriter writer, ushort value)
        {
            writer.WriteUInt16(value);
        }
        public static void WriteInt32(IDataWriter writer, int value)
        {
            writer.WriteInt32(value);
        }
        public static void WriteUInt32(IDataWriter writer, uint value)
        {
            writer.WriteUInt32(value);
        }
        public static void WriteInt64(IDataWriter writer, long value)
        {
            writer.WriteInt64(value);
        }
        public static void WriteUInt64(IDataWriter writer, ulong value)
        {
            writer.WriteUInt64(value);
        }
        public static void WriteSingle(IDataWriter writer, float value)
        {
            writer.WriteSingle(value);
        }
        public static void WriteDouble(IDataWriter writer, double value)
        {
            writer.WriteDouble(value);
        }
        public static void WriteString(IDataWriter writer, string value)
        {
            writer.WriteString(value);
        }
        public static void WriteDateTime(IDataWriter writer, DateTime value)
        {
            writer.WriteDateTime(value);
        }

        #endregion

        #region Reader Writer Convert

        public static IDataReader ConvertReader(IDataReader reader, PacketFormats packetFormat)
        {
            // 忽略文本形式(用于通用序列化)
            if (reader is JsonDataReader)
            {
                return reader;
            }

            switch (packetFormat)
            {
                case PacketFormats.Binary:
                    if (reader is BinaryDataReader)
                    {
                        return reader;
                    }
                    else
                    {
                        return new BinaryDataReader(reader.Node("v").ReadBytes());
                    }
                case PacketFormats.Bson:
                    if (reader is BsonDataReader)
                    {
                        return reader;
                    }
                    else
                    {
                        return new BsonDataReader(reader.Node("v").ReadBytes());
                    }
                case PacketFormats.Json:
                    if (reader is JsonDataReader)
                    {
                        return reader;
                    }
                    else
                    {
                        return new JsonDataReader(reader.Node("v").ReadString());
                    }
                case PacketFormats.Default:
                default:
                    return reader;
            }
        }

        public static void ConvertWriter(IDataWriter writer, PacketFormats packetFormat, Action<IDataWriter> writeAction)
        {
            // 忽略文本形式(用于通用序列化)
            if (writer is JsonDataWriter)
            {
                writeAction(writer);
                return;
            }

            switch (packetFormat)
            {
                case PacketFormats.Binary:
                    if (writer is BinaryDataWriter)
                    {
                        writeAction(writer);
                    }
                    else
                    {
                        BinaryDataWriter binWriter = new BinaryDataWriter();
                        writeAction(binWriter);
                        writer.Node("v").WriteBytes(binWriter.Buffer, 0, binWriter.Offset);
                    }
                    break;
                case PacketFormats.Bson:
                    if (writer is BsonDataWriter)
                    {
                        writeAction(writer);
                    }
                    else
                    {
                        BsonDataWriter bsonWriter = new BsonDataWriter();
                        writeAction(bsonWriter);
                        byte[] b = bsonWriter.ToBytes();
                        writer.Node("v").WriteBytes(b, 0, b.Length);
                    }
                    break;
                case PacketFormats.Json:
                    if (writer is JsonDataWriter)
                    {
                        writeAction(writer);
                    }
                    else
                    {
                        JsonDataWriter jsonWriter = new JsonDataWriter();
                        writeAction(jsonWriter);
                        writer.Node("v").WriteString(jsonWriter.ToString(false));
                    }
                    break;
                case PacketFormats.Default:
                default:
                    writeAction(writer);
                    break;
            }
        }

        #endregion

        #region Function

        public static object CallFunction(string name, FunctionContext context)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FunctionInfo info = _functions.GetValueOrDefault(name);
            if (info != null)
            {
                return info.Handler(context);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Utility
        public static object CloneObject(object source, object target = null, bool autoNew = false)
        {
            if (source == null)
            {
                return null;
            }

            if (_typeInfos.TryGetValue(source.GetType(), out TypeInfo info) && info is ClassTypeInfo clsInfo)
            {
                return clsInfo.Cloner(source, target, autoNew);
            }
            else if (source is ICloneable cloneable)
            {
                return cloneable.Clone();
            }
            else if (ObjectResolver != null)
            {
                return ObjectResolver.Clone(source);
            }
            else
            {
                return null;
            }
        }
        public static T Clone<T>(T source, T target = null, bool autoNew = false) where T : class
        {
            if (source == null)
            {
                return null;
            }

            if (_typeInfos.TryGetValue(source.GetType(), out TypeInfo info) && info is ClassTypeInfo clsInfo)
            {
                return (T)clsInfo.Cloner(source, target, autoNew);
            }
            else if (source is ICloneable cloneable)
            {
                return cloneable.Clone() as T;
            }
            else if (ObjectResolver != null)
            {
                return ObjectResolver.Clone(source) as T;
            }
            else
            {
                return default(T);
            }
        }
        public static bool ObjectEquals(object objA, object objB)
        {
            if (objA == null && objB == null)
            {
                return true;
            }
            if (objA == null || objB == null)
            {
                return false;
            }
            if (objA.GetType() != objB.GetType())
            {
                return false;
            }

            if (_typeInfos.TryGetValue(objA.GetType(), out TypeInfo info) && info is ClassTypeInfo clsInfo)
            {
                return clsInfo.EqualChecker(objA, objB);
            }
            else if (objA is IComparable comparable)
            {
                return comparable.CompareTo(objB) == 0;
            }
            else if (ObjectResolver != null)
            {
                return ObjectResolver.ObjectEquals(objA, objB);
            }
            else
            {
                return false;
            }
        }
        public static void ExchangeObject(object obj, IExchange exchange)
        {
            if (_typeInfos.TryGetValue(obj.GetType(), out TypeInfo info) && info is ClassTypeInfo clsInfo)
            {
                clsInfo.Exchanger(obj, exchange);
            }
            else if (obj is IExchangableObject ex)
            {
                ex.ExchangeProperty(exchange);
            }
        }

        public static IEnumerable<string> GetPropertyNames(object obj)
        {
            if (obj == null)
            {
                return EmptyArray<string>.Empty;
            }

            if (_typeInfos.TryGetValue(obj.GetType(), out TypeInfo info) && info is ClassTypeInfo clsInfo)
            {
                var ex = new EnumPropertyExchange();
                clsInfo.Exchanger(obj, ex);
                return ex.Names;
            }
            else if (obj is IExchangableObject)
            {
                return (obj as IExchangableObject).GetPropertyNames();
            }
            else if (ObjectResolver != null)
            {
                return ObjectResolver.GetPropertyNames(obj);
            }
            else
            {
                return EmptyArray<string>.Empty;
            }
        }
        public static object GetProperty(object obj, string name)
        {
            if (obj == null)
            {
                return null;
            }

            if (_typeInfos.TryGetValue(obj.GetType(), out TypeInfo info) && info is ClassTypeInfo clsInfo)
            {
                if (clsInfo.PropertyGetter != null)
                {
                    return clsInfo.PropertyGetter(obj, name);
                }
                else
                {
                    var ex = new GetPropertyExchange(name);
                    clsInfo.Exchanger(obj, ex);
                    return ex.Value;
                }
            }
            else if (obj is IExchangableObject ex)
            {
                return ex.GetProperty(name);
            }
            else if (ObjectResolver != null)
            {
                return ObjectResolver.GetProperty(obj, name);
            }
            else
            {
                Logs.LogWarning("ObjectType.PropertyResolver is not set.");
                return null;
            }
        }
        public static void SetProperty(object obj, string name, object value)
        {
            if (obj == null)
            {
                return;
            }

            if (_typeInfos.TryGetValue(obj.GetType(), out TypeInfo info) && info is ClassTypeInfo clsInfo)
            {
                if (clsInfo.PropertySetter != null)
                {
                    clsInfo.PropertySetter(obj, name, value);
                }
                else
                {
                    var ex = new SetPropertyExchange(name, value);
                    clsInfo.Exchanger(obj, ex);
                }
            }
            else if (obj is IExchangableObject ex)
            {
                ex.SetProperty(name, value);
            }
            else if (ObjectResolver != null)
            {
                ObjectResolver.SetProperty(obj, name, value);
            }
            else
            {
                Logs.LogWarning("ObjectType.PropertyResolver is not set.");
            }
        } 
        #endregion
    }
}
