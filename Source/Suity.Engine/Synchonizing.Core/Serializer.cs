// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.NodeQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Synchonizing.Preset;
using Suity.Reflecting;
using Suity.Engine;
using Suity.Views;

namespace Suity.Synchonizing.Core
{
    public static class Serializer
    {
        public static void Serialize(object obj, INodeWriter writer, SyncIntent intent = SyncIntent.Serialize)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (obj == null)
            {
                return;
            }

            Serialize(obj, intent, writer, SyncContext.Empty);
        }
        public static void Serialize(object obj, INodeWriter writer, ISyncTypeResolver resolver, IServiceProvider provider = null, SyncIntent intent = SyncIntent.Serialize)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (obj == null)
            {
                return;
            }

            if (resolver != null)
            {
                string typeName = resolver.ResolveTypeName(obj.GetType(), obj);
                if (!string.IsNullOrEmpty(typeName))
                {
                    writer.SetAttribute("type", typeName);
                }
                //为何要做多余动作？resolver找不到类型应该忽略
                //else
                //{
                //    writer.SetAttribute("type", DefaultSyncTypeResolver.Instance.ResolveTypeName(obj.GetType()));
                //}
            }

            Serialize(obj, intent, writer, new SyncContext(null, resolver, provider));
        }
        public static void Serialize(object obj, INodeWriter writer, string typeName, SyncIntent intent = SyncIntent.Serialize)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (obj == null)
            {
                return;
            }

            writer.SetAttribute("type", typeName);
            Serialize(obj, intent, writer, SyncContext.Empty);
        }



        private static void Serialize(object obj, SyncIntent intent, INodeWriter writer, SyncContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (obj == null)
            {
                writer.SetAttribute("null", "true");
                return;
            }

            if (obj.GetType().IsPrimitive || obj.GetType().IsEnum || obj is string)
            {
                writer.SetValue(obj.ToString());
            }
            else if (obj is ISerializeAsString serializeAsString)
            {
                writer.SetValue(serializeAsString.Key);
            }
            else if (obj is ISyncObject syncObject)
            {
                SerializeSyncObject(syncObject, intent, writer, context);
            }
            else if (obj is ISyncList syncList)
            {
                SerializeSyncList(syncList, intent, writer, context);
            }
            else if (SyncTypeExtensions.GetObjectProxyType(obj) != null)
            {
                SerializeSyncObject(SyncTypeExtensions.CreateObjectProxy(obj), intent, writer, context);
            }
            else if (SyncTypeExtensions.GetListProxyType(obj) != null)
            {
                SerializeSyncList(SyncTypeExtensions.CreateListProxy(obj), intent, writer, context);
            }
            else if (obj is INodeReader nodeReader)
            {
                nodeReader.WriteTo(writer);
            }
            else if (obj is Guid || obj is DateTime || obj is TimeSpan || obj is ButtonValue)
            {
                writer.SetValue(obj.ToString() ?? string.Empty);
            }
            else if (context.Resolver != null)
            {
                object proxy = context.Resolver.CreateProxy(obj);

                if (proxy is ISyncObject syncObject2)
                {
                    SerializeSyncObject(syncObject2, intent, writer, context);
                }
                else if (proxy is ISyncList syncList2)
                {
                    SerializeSyncList(syncList2, intent, writer, context);
                }
                else
                {
                    string str = context.Resolver.ResolveObjectValue(obj);
                    if (!string.IsNullOrEmpty(str))
                    {
                        writer.SetValue(str);
                    }
                }
            }
            else
            {
                // Do nothing.
            }
        }
        private static void SerializeSyncObject(ISyncObject obj, SyncIntent intent, INodeWriter writer, SyncContext context)
        {
            GetAllPropertySync sync = new GetAllPropertySync(intent, true);
            obj.Sync(sync, context);

           SyncContext childContext = context.CreateNew(obj);
            foreach (var pair in sync.Values)
            {
                if ((pair.Value.Flag & SyncFlag.ByRef) == SyncFlag.ByRef)
                {
                    continue;
                }
                if ((pair.Value.Flag & SyncFlag.NoSerialize) == SyncFlag.NoSerialize)
                {
                    continue;
                }

                if ((pair.Value.Flag & SyncFlag.AttributeMode) != SyncFlag.AttributeMode)
                {
                    writer.AddElement(pair.Key, w =>
                    {
                        CheckForTypeName(pair.Value, w, childContext.Resolver, obj);
                        Serialize(pair.Value.Value, intent, w, childContext);
                    });
                }
                else
                {
                    if (!(pair.Value.Value is string))
                    {
                        throw new InvalidOperationException("SyncFlag.AttributeMode supports String type only.");
                    }
                    writer.SetAttribute(pair.Key.TrimStart('@'), pair.Value.Value);
                }
            }
        }
        private static void SerializeSyncList(ISyncList list, SyncIntent intent, INodeWriter writer, SyncContext context)
        {
            GetAllIndexSync sync = new GetAllIndexSync(intent);
            list.Sync(sync, context);

            SyncContext childContext = context.CreateNew(list);
            foreach (var pair in sync.Attributes)
            {
                writer.SetAttribute(pair.Key, pair.Value);
            }
            for (int i = 0; i < sync.Values.Count; i++)
            {
                SyncValueInfo info = sync.Values[i];

                if ((info.Flag & SyncFlag.ByRef) == SyncFlag.ByRef)
                {
                    continue;
                }

                //没有提供类型时，使用列表的默认类型
                if (info.BaseType == typeof(object))
                {
                    info.BaseType = list.GetElementType() ?? typeof(object);
                }

                writer.AddElement("Item", w => 
                {
                    CheckForTypeName(info, w, childContext.Resolver, list);
                    Serialize(info.Value, intent, w, childContext);
                });
            }
        }
        private static void CheckForTypeName(SyncValueInfo typedValue, INodeWriter writer, ISyncTypeResolver callerResolver, object localResolverObj)
        {
            if (typedValue.Value != null && typedValue.BaseType != typedValue.Value.GetType() && (typedValue.Flag & SyncFlag.ByRef) == SyncFlag.None)
            {
                ISyncTypeResolver localResolver = localResolverObj as ISyncTypeResolver;
                ISyncTypeResolver globalResolver = SyncTypeExtensions._globalResolver;

                if (localResolver != null)
                {
                    string typeName = localResolver.ResolveTypeName(typedValue.Value.GetType(), typedValue.Value);
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        writer.SetAttribute("type", typeName);
                        return;
                    }
                    else if (typeName == string.Empty) //默认类型
                    {
                        return;
                    }
                }

                if (callerResolver != null)
                {
                    string typeName = callerResolver.ResolveTypeName(typedValue.Value.GetType(), typedValue.Value);
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        writer.SetAttribute("type", typeName);
                        return;
                    }
                }

                if (globalResolver != null)
                {
                    string typeName = globalResolver.ResolveTypeName(typedValue.Value.GetType(), typedValue.Value);
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        writer.SetAttribute("type", typeName);
                        return;
                    }
                }

                string originTypeName = DefaultSyncTypeResolver.Instance.ResolveTypeName(typedValue.Value.GetType(), typedValue.Value);
                writer.SetAttribute("type", originTypeName);


                //throw new InvalidOperationException("Can not resolve type : " + typedValue.Value.GetType());
            }
        }



        public static object Deserialize(INodeReader reader, ISyncTypeResolver resolver, IServiceProvider provider = null)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            string typeName = reader.GetAttribute("type");
            if (string.IsNullOrEmpty(typeName))
            {
                throw new NullReferenceException("typeName");
            }

            Type type = resolver.ResolveType(typeName, reader.NodeValue);
            if (type == null)
            {
                throw new NullReferenceException("ResolveType");
            }
            object obj = Activator.CreateInstance(type);
            if (obj == null)
            {
                throw new NullReferenceException("CreateInstance");
            }

            Deserialize(obj, reader, new SyncContext(null, resolver, provider));
            return obj;
        }
        public static T Deserialize<T>(INodeReader reader, ISyncTypeResolver resolver = null, IServiceProvider provider = null)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            SyncContext context = new SyncContext(null, resolver, provider);
            object obj = CreateObject(typeof(T), reader, context, null);

            if (obj is T)
            {
                Deserialize(obj, reader, context);
                return (T)obj;
            }
            else
            {
                return default(T);
            }
        }
        public static void Deserialize(object obj, INodeReader reader)
        {
            Deserialize(obj, reader, SyncContext.Empty);
        }
        public static void Deserialize(object obj, INodeReader reader, ISyncTypeResolver resolver, IServiceProvider provider = null)
        {
            Deserialize(obj, reader, new SyncContext(null, resolver, provider));
        }

        private static void Deserialize(object obj, INodeReader reader, SyncContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (obj == null)
            {
            }
            else if (obj is ISerializeAsString)
            {
                // Do nothing.
            }
            else if (obj is ISyncObject syncObject)
            {
                DeserializeSyncObject(syncObject, reader, context);
            }
            else if (obj is ISyncList syncList)
            {
                DeserializeSyncList(syncList, reader, context);
            }
            else if (SyncTypeExtensions.GetObjectProxyType(obj) != null)
            {
                DeserializeSyncObject(SyncTypeExtensions.CreateObjectProxy(obj), reader, context);
            }
            else if (SyncTypeExtensions.GetListProxyType(obj) != null)
            {
                DeserializeSyncList(SyncTypeExtensions.CreateListProxy(obj), reader, context);
            }
            else if (obj is RawNode rawNode)
            {
                DeserializeRawNode(rawNode, reader, context);
            }
            else if (context.Resolver != null)
            {
                object proxy = context.Resolver.CreateProxy(obj);

                if (proxy is ISyncObject syncObject2)
                {
                    DeserializeSyncObject(syncObject2, reader, context);
                }
                else if (proxy is ISyncList syncList2)
                {
                    DeserializeSyncList(syncList2, reader, context);
                }
            }
            else
            {
                // Do nothing.
            }
        }
        private static void DeserializeSyncObject(ISyncObject obj, INodeReader reader, SyncContext context)
        {
            SyncContext elementContext = context.CreateNew(obj);

            DeserializePropertySync sync = new DeserializePropertySync(reader)
            {
                Creater = (elementType, elementReader) =>
                {
                    return CreateObject(elementType, elementReader, context, obj as ISyncTypeResolver);
                },
                Deserializer = (elementObj, elementReader) =>
                {
                    Deserialize(elementObj, elementReader, elementContext);
                },
                Value = reader.NodeValue,
            };

            obj.Sync(sync, context);
        }
        private static void DeserializeSyncList(ISyncList list, INodeReader reader, SyncContext context)
        {
            SyncContext elementContext = context.CreateNew(list);

            DeserializeIndexSync sync = new DeserializeIndexSync(reader)
            {
                Creater = (elementType, elementReader) =>
                {
                    return CreateObjectForList(elementType, elementReader, context, list);
                },
                Deserializer = (elementObj, elementReader) =>
                {
                    Deserialize(elementObj, elementReader, elementContext);
                },
                Value = reader.NodeValue,
            };

            list.Sync(sync, context);
        }
        private static void DeserializeRawNode(RawNode node, INodeReader reader, SyncContext context)
        {
            node.Read(reader);
        }


        private static object CreateObject(Type type, INodeReader reader, SyncContext context, ISyncTypeResolver localResolver)
        {
            bool isNull = reader.GetAttribute("null") == "true";
            if (isNull) return null;

            string typeName = reader.GetAttribute("type");
            string content = reader.NodeValue;

            try
            {
                return SyncTypeExtensions.CreateObject(type, typeName, content, context.Resolver, localResolver);
            }
            catch (TypeResolveException e)
            {
                Logs.LogError(e.Message);
                throw;
            }
        }
        private static object CreateObjectForList(Type type, INodeReader reader, SyncContext context, ISyncList list)
        {
            //空判定
            bool isNull = reader.GetAttribute("null") == "true";
            if (isNull)
            {
                return null;
            }

            //覆盖类型
            string typeName = reader.GetAttribute("type");
            string content = reader.NodeValue;

            try
            {
                //尝试覆盖类型和请求类型构建
                object obj = SyncTypeExtensions.CreateObject(type, typeName, content, context.Resolver, list as ISyncTypeResolver);
                if (obj != null)
                {
                    return obj;
                }

                //列表默认构造器构建
                obj = list.CreateNewItem(content);
                if (obj != null)
                {
                    if (obj is ISerializeAsString serializeAsString)
                    {
                        serializeAsString.Key = content;
                    }
                    return obj;
                }

                //列表默认类型构建
                var listElementType = list.GetElementType();
                if (listElementType != null && listElementType != typeof(object))
                {
                    return SyncTypeExtensions.CreateObject(listElementType, typeName, content, context.Resolver, list as ISyncTypeResolver);
                }

                return null;
            }
            catch (TypeResolveException e)
            {
                Logs.LogError(e.Message);
                throw;
            }
        }
    }
}
