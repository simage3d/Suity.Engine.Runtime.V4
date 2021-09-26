// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity
{
    /// <summary>
    /// 类型信息
    /// </summary>
    public abstract class TypeInfo : Suity.ResourceObject
    {
        private readonly string _name;

        internal TypeInfo(Type type, string key, string aliasName = null)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            Key = key;

            if (!string.IsNullOrEmpty(aliasName))
            {
                _name = AliasName = aliasName;
            }
            else
            {
                _name = type.FullName;
            }
        }

        public Type Type { get; }

        public TypeFamily Family { get; internal set; }
        public string AliasName { get; }
        public string Description => Descriptor?.Description ?? string.Empty;
        public TypeInfoDescriptor Descriptor => DataStorage.GetObject<TypeInfoDescriptor>(Key);

        protected override string GetName() => _name;

        public virtual bool IsEnum => false;
        public virtual bool IsClass => false;
        public virtual bool IsPrimitive => false;
        public virtual string FullName => Type?.FullName ?? _name;
        public virtual PacketFormats PacketFormat => PacketFormats.Default;

        public override string ToString()
        {
            return Type.Name;
        }
    }
}