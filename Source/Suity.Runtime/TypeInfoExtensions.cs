using Suity.Json;
using Suity.NodeQuery;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suity
{
    public static class TypeInfoExtensions
    {
        //public static SyncObjectProxy WrapSyncObjectProxy(object obj)
        //{
        //    if (obj == null)
        //    {
        //        return null;
        //    }

        //    var info = ObjectType.GetClassTypeInfo(obj.GetType());
        //    if (info != null)
        //    {
        //        return WrapSyncObjectProxy(info);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //public static SyncObjectProxy WrapSyncObjectProxy(this ClassTypeInfo info)
        //{
        //    return new ObjectTypeSyncProxyWrapper(info.Exchanger);
        //}

        //class ObjectTypeSyncProxyWrapper : SyncObjectProxy
        //{
        //    public readonly ObjectType.ExchangeDelegate Exchanger;

        //    public ObjectTypeSyncProxyWrapper(ObjectType.ExchangeDelegate exchanger)
        //    {
        //        Exchanger = exchanger ?? throw new ArgumentNullException(nameof(exchanger));
        //    }

        //    public override void Sync(IPropertySync sync, ISyncContext context)
        //    {
        //        Exchanger(Target, new ExchangeSync(sync));
        //    }

        //    public override SyncObjectProxy Clone()
        //    {
        //        return new ObjectTypeSyncProxyWrapper(Exchanger);
        //    }
        //}

        public static void WriteObjectInfo(this INodeWriter writer, object obj, string name = null)
        {
            if (obj == null)
            {
                writer.WriteInfo(name, name, "null");
            }
            else if (obj is string)
            {
                writer.WriteInfo(name, name, $"\"{obj}\"");
            }
            if (ObjectType.GetClassTypeInfo(obj.GetType()) != null)
            {
                JsonDataWriter jsonWriter = new JsonDataWriter();
                ObjectType.WriteObject(jsonWriter, obj);
                writer.WriteJsonInfo(jsonWriter.Value, name);
            }
            else
            {
                writer.WriteInfo(name, name, obj.ToString());
            }
        }
    }
}
