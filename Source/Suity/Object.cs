// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace Suity
{
    public abstract class Object
    {
        public Object()
        {
            Environment._device.ObjectCreate(this);
        }

        public string Name { get => GetName(); set => SetName(value); }

        internal protected virtual void Destroy()
        {
            (this as IDisposable)?.Dispose();
        }
        protected virtual string GetName() => null;
        protected virtual void SetName(string name) { }

        public override string ToString()
        {
            return GetName() ?? base.ToString();
        }

        #region Static

        public static void DestroyObject(Object obj)
        {
            if (obj == null)
            {
                return;
            }

            obj.Destroy();
        }

        #endregion
    }

    public abstract class ObjectWithId : Object
    {
        public abstract long Id { get; }
    }

    public abstract class SystemObject : Object
    {
        public abstract bool IsStarted { get; }

        public abstract void Start();
        public abstract void Stop();

        protected internal override void Destroy()
        {
            if (IsStarted)
            {
                Stop();
            }
            base.Destroy();
        }
    }
}
