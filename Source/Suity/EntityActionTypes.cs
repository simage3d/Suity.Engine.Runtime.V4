using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public enum EntityActionTypes
    {
        CreateEntity,
        DestroyEntity,
        AddOrReplaceValue,
        RemoveValue,

        AddedToLogic,
        RemovedFromLogic,

        Message,
    }
}
