// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public interface IExchangableObject
    {
        void ExchangeProperty(IExchange exchange);
    }

    public interface IExchange
    {
        object Exchange(string name, object value);
    }

    public static class ExchangeExtensions
    {
        public static IEnumerable<string> GetPropertyNames(this IExchangableObject obj)
        {
            EnumPropertyExchange ex = new EnumPropertyExchange();
            obj.ExchangeProperty(ex);
            return ex.Names;
        }
        public static object GetProperty(this IExchangableObject obj, string propertyName)
        {
            GetPropertyExchange ex = new GetPropertyExchange(propertyName);
            obj.ExchangeProperty(ex);
            return ex.Value;
        }
        public static void SetProperty(this IExchangableObject obj, string propertyName, object value)
        {
            SetPropertyExchange ex = new SetPropertyExchange(propertyName, value);
            obj.ExchangeProperty(ex);
        }

    }


#if BRIDGE
    class GetPropertyExchange : IExchange
#else
    class GetPropertyExchange : MarshalByRefObject, IExchange
#endif
    {
        public string Name { get; }
        public object Value { get; private set; }

        public GetPropertyExchange(string name)
        {
            Name = name;
        }

        public object Exchange(string name, object value)
        {
            if (name == Name)
            {
                Value = value;
            }
            return value;
        }
    }

#if BRIDGE
    class SetPropertyExchange : IExchange
#else
    class SetPropertyExchange : MarshalByRefObject, IExchange
#endif
    {
        public string Name { get; }
        public object Value { get; }

        public SetPropertyExchange(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public object Exchange(string name, object value)
        {
            if (name == Name)
            {
                return Value;
            }
            else
            {
                return value;
            }
        }
    }


#if BRIDGE
    class EnumPropertyExchange : IExchange
#else
    class EnumPropertyExchange : MarshalByRefObject, IExchange
#endif
    {
        public List<string> Names { get; }

        public EnumPropertyExchange()
        {
            Names = new List<string>(0);
        }

        public object Exchange(string name, object value)
        {
            Names.Add(name);
            return value;
        }
    }
}


