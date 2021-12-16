// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Mapping
{
    public interface IRexMediator<T> : IDisposable
    {
        void InitializeTarget(RexMapper mapper, T target);
    }

    public abstract class RexMediator<T> : RexMapperObject, IRexMediator<T>
    {
        public T Target { get; private set; }
        public bool IsDisposed { get; private set; }

        public void InitializeTarget(RexMapper mapper, T target)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Target = target;

            OnInitialize();
        }
        protected virtual void OnInitialize()
        {
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            IsDisposed = true;

            OnDispose();
            Target = default(T);
        }
        protected virtual void OnDispose()
        {
        }

        protected override void Destroy()
        {
            Dispose();
            base.Destroy();
        }
    }
}
