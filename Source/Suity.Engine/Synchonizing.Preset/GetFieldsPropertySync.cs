using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Synchonizing.Preset
{
    public class GetFieldsPropertySync : MarshalByRefObject, IPropertySync
    {
        readonly public HashSet<string> Fields = new HashSet<string>();

        public GetFieldsPropertySync(SyncIntent intent = SyncIntent.Serialize)
        {
            Intent = intent;
        }

        public SyncMode Mode => SyncMode.GetAll;

        public SyncIntent Intent { get; }

        public string Name => null;

        public IEnumerable<string> Names => Fields;

        public object Value => null;

        public T Sync<T>(string name, T obj, SyncFlag flag = SyncFlag.None, T defaultValue = default(T))
        {
            if ((flag & SyncFlag.AttributeMode) != SyncFlag.AttributeMode)
            {
                Fields.Add(name);
            }

            return obj;
        }
    }
}
