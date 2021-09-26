// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Suity
{
    [AssetDefinitionType(AssetDefinitionCodes.TypeLibrary)]
    public class TypeFamily : Suity.Object
    {
        private readonly List<FunctionInfo> _functionInfos = new List<FunctionInfo>();
        private readonly List<TypeInfo> _typeInfos = new List<TypeInfo>();
        string _name;

        public TypeFamily(string key, string name = null)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _name = name;
        }

        public IEnumerable<FunctionInfo> FunctionInfos => _functionInfos.Select(o => o);
        public string Key { get; }

        public IEnumerable<TypeInfo> TypeInfos => _typeInfos.Select(o => o);

        /// <summary>
        /// 注册格式器使用的对象类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="aliasName"></param>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="cloner"></param>
        /// <param name="comparer"></param>
        /// <param name="propGetter"></param>
        /// <param name="propSetter"></param>
        public ClassTypeInfo RegisterClassType(
            Type type, string id, string aliasName,
            ObjectType.ReadDelegate reader = null,
            ObjectType.WriteDelegate writer = null,
            ObjectType.CloneDelegate cloner = null,
            ObjectType.EqualsDelegate comparer = null,
            ObjectType.ExchangeDelegate exchanger = null,
            ObjectType.GetPropertyDelegate propGetter = null,
            ObjectType.SetPropertyDelegate propSetter = null
            )
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
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
            //if (exchanger == null)
            //{
            //    throw new ArgumentNullException(nameof(exchanger));
            //}

            ClassTypeInfo info = new ClassTypeInfo(type, id, aliasName, PacketFormats.Default, reader, writer, cloner, comparer, exchanger, propGetter, propSetter);
            RegisterTypeInfo(info);

            return info;
        }

        public ClassTypeInfo RegisterClassType(
            Type type, string id, string aliasName, PacketFormats packetFormat,
            ObjectType.ReadDelegate reader = null,
            ObjectType.WriteDelegate writer = null,
            ObjectType.CloneDelegate cloner = null,
            ObjectType.EqualsDelegate comparer = null,
            ObjectType.ExchangeDelegate exchanger = null,
            ObjectType.GetPropertyDelegate propGetter = null,
            ObjectType.SetPropertyDelegate propSetter = null
            )
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
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
            //if (exchanger == null)
            //{
            //    throw new ArgumentNullException(nameof(exchanger));
            //}

            ClassTypeInfo info = new ClassTypeInfo(type, id, aliasName, packetFormat, reader, writer, cloner, comparer, exchanger, propGetter, propSetter);
            RegisterTypeInfo(info);

            return info;
        }


        /// <summary>
        /// 注册格式器使用的枚举类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <param name="aliasName"></param>
        public EnumTypeInfo RegisterEnumType(Type type, string typeName, string aliasName = null)
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
            RegisterTypeInfo(info);

            return info;
        }
        public EnumTypeInfo RegisterEnumType(Type type, string typeName, string aliasName = null, PacketFormats packetFormat = PacketFormats.Default)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            EnumTypeInfo info = new EnumTypeInfo(type, typeName, aliasName, packetFormat);
            RegisterTypeInfo(info);

            return info;
        }

        public FunctionInfo RegisterFunction(string name, ObjectType.FunctionDelegate handler)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            FunctionInfo info = new FunctionInfo(name, handler);
            _functionInfos.Add(info);

            return info;
        }

        internal void RegisterClassType(Type type, string id, string aliasName)
        {
            ClassTypeInfo info = new ClassTypeInfo(type, id, aliasName);
            RegisterTypeInfo(info);
        }

        internal void RegisterPrimaryInfos()
        {
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.Boolean));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.SByte));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.Byte));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.Int16));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.UInt16));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.Int32));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.UInt32));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.Int64));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.UInt64));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.Single));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.Double));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.DateTime));
            RegisterTypeInfo(PrimaryTypeInfo.Create(TypeCode.String));
        }

        internal void RegisterTypeInfo(TypeInfo typeInfo)
        {
            _typeInfos.Add(typeInfo);
            typeInfo.Family = this;
        }

        protected override string GetName()
        {
            return _name ?? Key;
        }
    }
}