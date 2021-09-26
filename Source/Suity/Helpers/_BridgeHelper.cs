#if BRIDGE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bridge;

namespace Suity.Helpers
{
    public static class _BridgeHelper
    {
        static readonly Dictionary<Type, Func<object, object>> _primitiveTypes = InitPrimitiveType();

        private static Dictionary<Type, Func<object, object>> InitPrimitiveType()
        {
            Dictionary<Type, Func<object, object>> types = new Dictionary<Type, Func<object, object>>();
            types.Add(typeof(Boolean), o => Convert.ToBoolean(o));
            types.Add(typeof(Byte), o => Convert.ToByte(o));
            types.Add(typeof(SByte), o => Convert.ToSByte(o));
            types.Add(typeof(Int16), o => Convert.ToInt16(o));
            types.Add(typeof(UInt16), o => Convert.ToUInt16(o));
            types.Add(typeof(Int32), o => Convert.ToInt32(o));
            types.Add(typeof(UInt32), o => Convert.ToUInt32(o));
            types.Add(typeof(Int64), o => Convert.ToInt64(o));
            types.Add(typeof(UInt64), o => Convert.ToUInt64(o));
            //types.Add(typeof(IntPtr), o => new IntPtr());
            //types.Add(typeof(UIntPtr), o => new UIntPtr());
            types.Add(typeof(Char), o => Convert.ToChar(o));
            types.Add(typeof(Double), o => Convert.ToDouble(o));
            types.Add(typeof(Single), o => Convert.ToSingle(o));

            return types;
        }

        public static bool IsPrimitive(this Type type)
        {
            return _primitiveTypes.ContainsKey(type);
        }

        public static object ChangeType(object value, Type targetType)
        {
            Func<object, object> converter;
            if (_primitiveTypes.TryGetValue(targetType, out converter))
            {
                return converter(value);
            }
            else
            {
                return null;
            }
        }

        public static int GetArrayRank(this Type type)
        {
            return type.IsArray ? 1 : 0;
        }

        public static Type[] GetExportedTypes(this Assembly assembly)
        {
            return assembly.GetTypes().Where(o => o.IsPublic).ToArray();
        }
    }

    [External]
    [Namespace(false)]
    public static class JSON
    {
        [External]
        extern public static object parse(string json);
        [External]
        extern public static string stringify(object obj);
    }
}
#endif