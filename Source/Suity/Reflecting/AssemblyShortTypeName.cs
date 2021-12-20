// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Suity.Collections;

namespace Suity.Reflecting
{
    public static class AssemblyShortTypeName
    {
        static readonly Dictionary<Assembly, UniqueMultiDictionary<string, Type>> _typeNames = new Dictionary<Assembly, UniqueMultiDictionary<string, Type>>();

        public static Type ResolveExportedClassType(Assembly assembly, string name)
        {
            UniqueMultiDictionary<string, Type> collection = EnsureCollection(assembly);
            return collection[name].FirstOrDefault();
        }

        static UniqueMultiDictionary<string, Type> EnsureCollection(Assembly asm)
        {
            if (!_typeNames.TryGetValue(asm, out UniqueMultiDictionary<string, Type> collection))
            {
                collection = new UniqueMultiDictionary<string, Type>();
                _typeNames.Add(asm, collection);
                foreach (var type in asm.GetExportedTypes())
                {
                    if (type.IsClass && !type.IsAbstract)
                    {
                        collection.Add(type.Name, type);
                    }
                }

            }
            return collection;
        }
    }
}
