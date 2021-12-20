using System;
using System.Collections.Generic;
using System.Text;

namespace Suity
{
    public abstract class RuntimeObject
    {
        public RuntimeObject()
        {
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

        public static void DestroyObject(RuntimeObject obj)
        {
            if (obj == null)
            {
                return;
            }

            obj.Destroy();
        }

        #endregion
    }

    public abstract class RuntimeResourceObject : RuntimeObject
    {
        public string Key { get; protected set; }

        public void MarkAccess()
        {
            string key = Key;
            if (!string.IsNullOrEmpty(key))
            {
                Logs.AddResourceLog(key, null);
            }
        }
        public void MarkAccess(string message)
        {
            string key = Key;
            if (!string.IsNullOrEmpty(key))
            {
                Logs.AddResourceLog(key, message);
            }
        }
    }

    public interface IRuntimeInitialize
    {
    }
}
