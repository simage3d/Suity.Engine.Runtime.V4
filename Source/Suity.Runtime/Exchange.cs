using System;
using System.Collections.Generic;
using System.Text;

namespace Suity
{
    public interface IExchangableObject
    {
        void ExchangeProperty(IExchange exchange);
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
}
