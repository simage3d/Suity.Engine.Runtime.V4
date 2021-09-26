using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Synchonizing
{
    public interface ISyncNode : ISyncObject
    {
        ISyncList GetList();
    }
}
