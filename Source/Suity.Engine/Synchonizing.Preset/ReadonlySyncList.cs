using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Synchonizing.Preset
{
    public class ReadonlySyncList<T> : ISyncList
    {
        readonly List<T> _list = new List<T>();
        public List<T> List => _list;

        int ISyncList.Count => _list.Count;

        void ISyncList.Sync(IIndexSync sync, ISyncContext context)
        {
            if (sync.IsGetter())
            {
                sync.SyncGenericIList(_list, typeof(T));
            }
        }
    }
}
