using Suity.Collections;
using Suity.Synchonizing.Preset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suity.Synchonizing.Core
{
    public class SyncPropertyResolver : IObjectResolver, IRuntimeInitialize
    {
        public SyncPropertyResolver()
        {
            ObjectType.ObjectResolver = this;
        }


        public IEnumerable<string> GetPropertyNames(object obj)
        {
            var syncObj = SyncTypeExtensions.GetSyncObject(obj);
            if (syncObj != null)
            {
                GetAllPropertySync sync = new GetAllPropertySync(false);
                syncObj.Sync(sync, EmptySyncContext.Empty);
                return sync.Names;
            }
            else
            {
                return EmptyArray<string>.Empty;
            }
        }
        public object GetProperty(object obj, string propertyName)
        {
            var syncObj = SyncTypeExtensions.GetSyncObject(obj);
            return syncObj?.GetProperty(propertyName, EmptySyncContext.Empty);
        }
        public void SetProperty(object obj, string propertyName, object value)
        {
            var syncObj = SyncTypeExtensions.GetSyncObject(obj);
            syncObj?.SetProperty(propertyName, value, EmptySyncContext.Empty);
        }
        public bool ObjectEquals(object objA, object objB)
        {
            return Equality.ObjectEquals(objA, objB);
        }
        public object Clone(object source)
        {
            return Cloner.Clone(source);
        }
    }
}
