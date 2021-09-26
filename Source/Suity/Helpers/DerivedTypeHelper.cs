// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Suity.Helpers
{
    /// <summary>
    /// 派生类型缓存
    /// </summary>
    public static class DerivedTypeHelper
    {
        private static readonly Dictionary<Type, List<Type>> _derivedTypeDict = new Dictionary<Type, List<Type>>();


        /// <summary>
        /// 获取基于指定基类型的所有派生类型
        /// </summary>
        /// <param name="baseType">基类型</param>
        /// <returns>返回基于指定基类型的所有派生类型</returns>
        public static IEnumerable<Type> GetDerivedTypes(this Type baseType)
        {
            lock (_derivedTypeDict)
            {
                if (_derivedTypeDict.TryGetValue(baseType, out List<Type> availTypes))
                {
                    return availTypes.Select(o => o);
                }

                availTypes = new List<Type>();
                Assembly[] asmQuery = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly asm in asmQuery)
                {
                    if (asm.IsDynamic)
                    {
                        continue;
                    }

                    // Try to retrieve all Types from the current Assembly
                    Type[] types;
                    try { types = asm.GetExportedTypes(); }
                    catch (Exception err)
                    {
                        Logs.LogWarning($"Assembly is dynamic or not supported : {asm.FullName}");
                        Logs.LogWarning(err);
                        continue;
                    }

                    // Add the matching subset of these types to the result
                    availTypes.AddRange(
                        from t in types
                        where t != baseType && baseType.IsAssignableFrom(t)
                        orderby t.Name
                        select t);
                }
                _derivedTypeDict[baseType] = availTypes;

                return availTypes;
            }
        }
        public static IEnumerable<Type> GetDerivedTypes(this Type baseType, Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes().Where(type => type != baseType && baseType.IsAssignableFrom(type));
            }
            catch (Exception)
            {
                throw;
                //return EmptyArray<Type>.Empty;
            }
        }


        public static IEnumerable<Type> GetGenericDerivedType(this Type genericDefinition, Type genericArgument)
        {
            Type baseType = genericDefinition.MakeGenericType(new Type[] { genericArgument });
            return GetDerivedTypes(baseType);
        }
        public static IEnumerable<Type> GetGenericDerivedType(this Type genericDefinition, Type genericArgument, Assembly assembly)
        {
            Type baseType = genericDefinition.MakeGenericType(new Type[] { genericArgument });
            return GetDerivedTypes(baseType, assembly);
        }


        public static void ResetCache()
        {
            lock (_derivedTypeDict)
            {
                _derivedTypeDict.Clear();
            }
        }
    }
}
