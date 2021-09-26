// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Engine
{
    public abstract class AssetRef
    {
        public string Key { get; set; }
        public abstract Type DefinitionType { get; }
        public string AssetTypeName => ObjectType.ResolveAssetDefinition(DefinitionType);


        public AssetRef()
        {
        }
        public AssetRef(string key)
        {
            Key = key;
        }

        public object Instance => ObjectType.GetAssetImplement(Key, DefinitionType, false);

        public object GetInstance(bool autoCreate = true)
        {
            return ObjectType.GetAssetImplement(Key, DefinitionType, autoCreate);
        }
        public object CreateInstance()
        {
            return ObjectType.CreateAssetImplement(Key, DefinitionType);
        }

        public override string ToString()
        {
            return Instance?.ToString() ?? Key;
        }

        public static AssetRef Create(Type definitionType, string key = null)
        {
            Type type = typeof(AssetRef<>).MakeGenericType(new Type[] { definitionType });
            AssetRef dataRef = (AssetRef)Activator.CreateInstance(type);
            dataRef.Key = key;
            return dataRef;
        }
    }

    public sealed class AssetRef<T> : AssetRef where T : class
    {
        public override Type DefinitionType => typeof(T);

        public AssetRef()
        {
        }
        public AssetRef(string key)
            : base(key)
        {
        }

        public new T Instance => ObjectType.GetAssetImplement<T>(Key, false);
        public new T GetInstance(bool autoCreate = true)
        {
            return ObjectType.GetAssetImplement<T>(Key, autoCreate);
        }
        public new T CreateInstance()
        {
            return ObjectType.CreateAssetImplement<T>(Key);
        }
    }
}
