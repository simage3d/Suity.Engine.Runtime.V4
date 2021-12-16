using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.VirtualDom
{
    public class RexPropertyWrapperValue<T> : IRexValue<T>
    {
        IRexProperty<T> _property;

        public RexPropertyWrapperValue(IRexProperty<T> property)
        {
            _property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public T Value => _property.Value;

        public void AddListener(Action<T> action)
        {
            _property.Tree.AddDataListener<T>(_property.Path, action);
        }

        public void RemoveListener(Action<T> action)
        {
            _property.Tree.RemoveListener<T>(_property.Path, action);
        }
    }
}
