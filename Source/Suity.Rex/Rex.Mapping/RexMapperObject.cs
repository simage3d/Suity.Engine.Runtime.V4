// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using Suity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Rex.Mapping
{
    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexMapperObject : Suity.Object
    {
        protected DisposeCollector Listeners { get; set; }

        internal RexMapper _mapper;

        public RexMapperObject()
        {
            _mapper = RexMapper.Global;
        }
        public RexMapperObject(RexMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public RexMapper Mapper => _mapper;


        protected override void Destroy()
        {
            base.Destroy();
            Listeners?.Dispose();
            Listeners = null;
        }

        protected void Provide<T>(T value) where T : class
        {
            Listeners += _mapper.Provide<T>(value);
        }
        protected void ProvideType<T>(bool singleton = true) where T : class
        {
            Listeners += _mapper.ProvideType<T>(singleton);
        }
        protected void ProvideType<T, TImplement>(bool singleton = true) where T : class where TImplement : T
        {
            Listeners += _mapper.ProvideType<T, TImplement>(singleton);
        }
        protected void ProvideHandler<T>(IRexHandler<T> handler)
        {
            Listeners += _mapper.ProvideHandler<T>(handler);
        }
        protected void ProvideHandler<T>(RexHandleDelegate<T> handle)
        {
            Listeners += _mapper.ProvideHandler<T>(handle);
        }
        protected void ProvideHandlerType<T, THandler>(bool singleton = true) where THandler : IRexHandler<T>
        {
            Listeners += _mapper.ProvideHandlerType<T, THandler>(singleton);
        }
        protected void ProvideProducer<T>(IRexProducer<T> producer)
        {
            Listeners += _mapper.ProvideProducer<T>(producer);
        }
        protected void ProvideProducer<T>(RexProduceDelegate<T> produce, RexRecycleDelegate<T> recycle = null)
        {
            Listeners += _mapper.ProvideProducer<T>(produce, recycle);
        }
        protected void ProvideProducerType<T, TProducer>(bool singleton = true) where TProducer : IRexProducer<T>
        {
            Listeners += _mapper.ProvideProducerType<T, TProducer>(singleton);
        }
        protected void ProvideAssembler<T>(IRexAssembler<T> assembler)
        {
            Listeners += _mapper.ProvideAssembler<T>(assembler);
        }
        protected void ProvideAssembler<T>(RexAssembleDelegate<T> assemble)
        {
            Listeners += _mapper.ProvideAssembler<T>(assemble);
        }
        protected void ProvideAssemblerType<T, TAssembler>(bool singleton = true) where TAssembler : IRexAssembler<T>
        {
            Listeners += _mapper.ProvideAssemblerType<T, TAssembler>(singleton);
        }
        protected void ProvideReducer<T>(IRexReducer<T> reducer)
        {
            Listeners += _mapper.ProvideReducer<T>(reducer);
        }
        protected void ProvideReducer<T>(RexReduceDelegate<T> reduce)
        {
            Listeners += _mapper.ProvideReducer<T>(reduce);
        }
        protected void ProvideReducerType<T, TReducer>(bool singleton = true) where TReducer : IRexReducer<T>
        {
            Listeners += _mapper.ProvideReducerType<T, TReducer>(singleton);
        }

        protected T Get<T>() where T : class
        {
            return _mapper.Get<T>();
        }
        protected IEnumerable<T> GetMany<T>() where T : class
        {
            return _mapper.GetMany<T>();
        }
        protected void Do<T>(Action<T> action) where T : class
        {
            T obj = _mapper.Get<T>();
            if (obj != null)
            {
                action(obj);
            }
        }
        protected void DoMany<T>(Action<T> action) where T : class
        {
            IEnumerable<T> objs = _mapper.GetMany<T>();
            objs.Foreach(o => action(o));
        }

        protected void Handle<T>(T value)
        {
            _mapper.Handle(value);
        }
        protected void HandleMany<T>(T value)
        {
            _mapper.HandleMany(value);
        }
        protected T Produce<T>() where T : class
        {
            return _mapper.Produce<T>(null);
        }
        protected T Produce<T>(string name) where T : class
        {
            return _mapper.Produce<T>(name);
        }
        protected bool Recycle<T>(T product) where T : class
        {
            return _mapper.Recycle<T>(null, product);
        }
        protected bool Recycle<T>(string name, T product) where T : class
        {
            return _mapper.Recycle<T>(name, product);
        }
        protected T Assemble<T>() where T : class
        {
            return _mapper.Assemble<T>(this, null);
        }
        protected T Assemble<T>(object target) where T : class
        {
            return _mapper.Assemble<T>(target, null);
        }
        protected T Assemble<T>(string name) where T : class
        {
            return _mapper.Assemble<T>(this, name);
        }
        protected T Assemble<T>(object target, string name) where T : class
        {
            return _mapper.Assemble<T>(target, name);
        }
        protected IEnumerable<T> AssembleMany<T>() where T : class
        {
            return _mapper.AssembleMany<T>(this, null);
        }
        protected IEnumerable<T> AssembleMany<T>(object target) where T : class
        {
            return _mapper.AssembleMany<T>(target, null);
        }
        protected IEnumerable<T> AssembleMany<T>(string name) where T : class
        {
            return _mapper.AssembleMany<T>(this, name);
        }
        protected IEnumerable<T> AssembleMany<T>(object target, string name) where T : class
        {
            return _mapper.AssembleMany<T>(target, name);
        }
        protected T Reduce<T>(T state, string name, object payload)
        {
            return _mapper.Reduce<T>(state, name, payload);
        }
        protected IRexMediator<T> GetMediator<T>(T target)
        {
            return _mapper.GetMediator<T>(target);
        }
        protected IEnumerable<IRexMediator<T>> GetMediators<T>(T target)
        {
            return _mapper.GetMediators<T>(target);
        }
    }
}
