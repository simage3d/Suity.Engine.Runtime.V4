// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suity.Collections;

namespace Suity.Rex.Mapping
{
    [MultiThreadSecurity(MultiThreadSecurityMethods.Insecure)]
    public class RexMapper : Suity.Object
    {
        public static readonly RexMapper Global = new RexMapper(true, true);


        internal readonly bool _isGlobal;
        readonly HashSet<Type> _doNotProvide = new HashSet<Type>();

        readonly Dictionary<Type, RexMappingCollection> _typeToObject = new Dictionary<Type, RexMappingCollection>();
        readonly Dictionary<Type, RexMappingCollection> _typeToHandler = new Dictionary<Type, RexMappingCollection>();
        readonly Dictionary<Type, RexMappingCollection> _typeToProducer = new Dictionary<Type, RexMappingCollection>();
        readonly Dictionary<Type, RexMappingCollection> _typeToAssembler = new Dictionary<Type, RexMappingCollection>();
        readonly Dictionary<Type, RexMappingCollection> _typeToReducer = new Dictionary<Type, RexMappingCollection>();
        readonly Dictionary<Type, RexMappingCollection> _typeToMediator = new Dictionary<Type, RexMappingCollection>();

        readonly List<IServiceProvider> _externalResolvers = new List<IServiceProvider>();

        private bool _useEnvService = true;
        public bool UseEnvironmentService
        {
            get { return _useEnvService; }
            // set { _useGlobalService = value; }
        }


        public event EventHandler<RexMapperEventArgs> ObjectResolved;
        public event EventHandler<RexMapperHandlerEventArgs> HandlerResolved;
        public event EventHandler<RexMapperProducerEventArgs> ProducerResolved;
        public event EventHandler<RexMapperAssemblerEventArgs> AssemblerResolved;
        public event EventHandler<RexMapperReducerEventArgs> ReducerResolved;
        public event EventHandler<RexMapperEventArgs> MediatorResolved;

        public event EventHandler<RexMapperUnsolvedEventArgs> ObjectUnsolved;
        public event EventHandler<RexMapperUnsolvedEventArgs> HandlerUnsolved;
        public event EventHandler<RexMapperUnsolvedEventArgs> ProducerUnsolved;
        public event EventHandler<RexMapperUnsolvedEventArgs> AssemblerUnsolved;
        public event EventHandler<RexMapperUnsolvedEventArgs> ReducerUnsolved;
        public event EventHandler<RexMapperUnsolvedEventArgs> MediatorUnsolved;


        public RexMapper()
        {
        }
        public RexMapper(bool useGlobalService)
        {
            _useEnvService = useGlobalService;
        }
        private RexMapper(bool useGlobalService, bool isGlobal)
        {
            _isGlobal = isGlobal;
        }
        protected override string GetName()
        {
            if (_isGlobal)
            {
                return "Global RexMapper";
            }
            else
            {
                return base.GetName();
            }
        }

        public void AddResolver(IServiceProvider resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            _externalResolvers.Add(resolver);
        }
        public void RemoveResolver(IServiceProvider resolver)
        {
            if (resolver != null)
            {
                _externalResolvers.Remove(resolver);
            }
        }

        #region Provide
        public IDisposable Provide(Type type, object value)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!type.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException("Value is not a " + type.Name, nameof(value));
            }
            if (_doNotProvide.Contains(value.GetType()))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToObject.GetValueOrCreate(type, () => new RexMappingCollection(type, InfoFilter));
            if (collection.Contains(value))
            {
                return new DisposableAction(() => collection.Remove(value));
            }

            if (value is IUseRexMapper useRexMapper)
            {
                useRexMapper.Mapper = this;
            }
            RexMappingInfo info = new RexMappingInfo(value);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(value));
        }
        public IDisposable Provide<T>(T value) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (_doNotProvide.Contains(value.GetType()))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToObject.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(value))
            {
                return new DisposableAction(() => collection.Remove(value));
            }

            if (value is IUseRexMapper useRexMapper)
            {
                useRexMapper.Mapper = this;
            }
            RexMappingInfo info = new RexMappingInfo(value);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(value));
        }
        public IDisposable ProvideType<T, TImplement>(bool singleton = true) where T : class where TImplement : T
        {
            if (_doNotProvide.Contains(typeof(TImplement)))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToObject.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(typeof(TImplement)))
            {
                return new DisposableAction(() => collection.Remove(typeof(TImplement)));
            }

            RexMappingInfo info = new RexMappingInfo(typeof(TImplement), singleton);
            collection.Add(info);

            return new DisposableAction(() => collection.Remove(typeof(TImplement)));
        }
        public IDisposable ProvideType<T>(bool singleton = true) where T : class
        {
            if (_doNotProvide.Contains(typeof(T)))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToObject.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(typeof(T)))
            {
                return new DisposableAction(() => collection.Remove(typeof(T)));
            }

            RexMappingInfo info = new RexMappingInfo(typeof(T), singleton);
            collection.Add(info);

            return new DisposableAction(() => collection.Remove(typeof(T)));
        }

        public IDisposable ProvideHandler<T>(IRexHandler<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (_doNotProvide.Contains(handler.GetType()))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToHandler.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(handler))
            {
                return new DisposableAction(() => collection.Remove(handler));
            }

            if (handler is IUseRexMapper useRexMapper)
            {
                useRexMapper.Mapper = this;
            }
            RexMappingInfo info = new RexMappingInfo(handler);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(handler));
        }
        public IDisposable ProvideHandler<T>(RexHandleDelegate<T> handler)
        {
            return ProvideHandler<T>(new RexHandler<T>(handler));
        }
        public IDisposable ProvideHandlerType<T, THandler>(bool singleton = true) where THandler : IRexHandler<T>
        {
            if (_doNotProvide.Contains(typeof(THandler)))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToHandler.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(typeof(THandler)))
            {
                return new DisposableAction(() => collection.Remove(typeof(THandler)));
            }

            RexMappingInfo info = new RexMappingInfo(typeof(THandler), singleton);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(typeof(THandler)));
        }

        public IDisposable ProvideProducer<T>(IRexProducer<T> producer)
        {
            if (producer == null)
            {
                throw new ArgumentNullException(nameof(producer));
            }
            if (_doNotProvide.Contains(producer.GetType()))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToProducer.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(producer))
            {
                return new DisposableAction(() => collection.Remove(producer));
            }

            if (producer is IUseRexMapper useRexMapper)
            {
                useRexMapper.Mapper = this;
            }
            RexMappingInfo info = new RexMappingInfo(producer);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(producer));
        }
        public IDisposable ProvideProducer<T>(RexProduceDelegate<T> produce, RexRecycleDelegate<T> recycle = null)
        {
            return ProvideProducer<T>(new RexProducer<T>(produce, recycle));
        }
        public IDisposable ProvideProducerType<T, TProducer>(bool singleton = true) where TProducer : IRexProducer<T>
        {
            if (_doNotProvide.Contains(typeof(TProducer)))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToProducer.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(typeof(TProducer)))
            {
                return new DisposableAction(() => collection.Remove(typeof(TProducer)));
            }

            RexMappingInfo info = new RexMappingInfo(typeof(TProducer), singleton);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(typeof(TProducer)));
        }

        public IDisposable ProvideAssembler<T>(IRexAssembler<T> assembler)
        {
            if (assembler == null)
            {
                throw new ArgumentNullException(nameof(assembler));
            }
            if (_doNotProvide.Contains(assembler.GetType()))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToAssembler.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(assembler))
            {
                return new DisposableAction(() => collection.Remove(assembler));
            }

            if (assembler is IUseRexMapper useRexMapper)
            {
                useRexMapper.Mapper = this;
            }
            RexMappingInfo info = new RexMappingInfo(assembler);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(assembler));
        }
        public IDisposable ProvideAssembler<T>(RexAssembleDelegate<T> assemble)
        {
            return ProvideAssembler(new RexAssembler<T>(assemble));
        }
        public IDisposable ProvideAssemblerType<T, TAssembler>(bool singleton = true) where TAssembler : IRexAssembler<T>
        {
            if (_doNotProvide.Contains(typeof(TAssembler)))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToAssembler.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(typeof(TAssembler)))
            {
                return new DisposableAction(() => collection.Remove(typeof(TAssembler)));
            }

            RexMappingInfo info = new RexMappingInfo(typeof(TAssembler), singleton);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(typeof(TAssembler)));
        }


        public IDisposable ProvideReducer<T>(IRexReducer<T> reducer)
        {
            if (reducer == null)
            {
                throw new ArgumentNullException(nameof(reducer));
            }
            if (_doNotProvide.Contains(reducer.GetType()))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToReducer.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(reducer))
            {
                return new DisposableAction(() => collection.Remove(reducer));
            }

            if (reducer is IUseRexMapper useRexMapper)
            {
                useRexMapper.Mapper = this;
            }
            RexMappingInfo info = new RexMappingInfo(reducer);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(reducer));
        }
        public IDisposable ProvideReducer<T>(RexReduceDelegate<T> reduce)
        {
            return ProvideReducer(new RexReducer<T>(reduce));
        }
        public IDisposable ProvideReducerType<T, TReducer>(bool singleton = true) where TReducer : IRexReducer<T>
        {
            if (_doNotProvide.Contains(typeof(TReducer)))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToReducer.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(typeof(TReducer)))
            {
                return new DisposableAction(() => collection.Remove(typeof(TReducer)));
            }

            RexMappingInfo info = new RexMappingInfo(typeof(TReducer), singleton);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(typeof(TReducer)));
        }


        public IDisposable ProvideMediator<T, TMediator>() where TMediator : IRexMediator<T>
        {
            if (_doNotProvide.Contains(typeof(TMediator)))
            {
                return EmptyDisposable.Empty;
            }
            RexMappingCollection collection = _typeToMediator.GetValueOrCreate(typeof(T), () => new RexMappingCollection(typeof(T), InfoFilter));
            if (collection.Contains(typeof(TMediator)))
            {
                return new DisposableAction(() => collection.Remove(typeof(TMediator)));
            }

            RexMappingInfo info = new RexMappingInfo(typeof(TMediator), false);
            collection.Add(info);
            return new DisposableAction(() => collection.Remove(typeof(TMediator)));
        }


        public void DoNotProvide<T>()
        {
            _doNotProvide.Add(typeof(T));
        }
        public void DoNotProvide(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            _doNotProvide.Add(type);
        }
        #endregion

        #region Resolve
        public T Get<T>() where T : class
        {
            var result = Resolve<T, T>(_typeToObject, true, true, out RexMappingInfo info);
            if (result != null)
            {
                ObjectResolved?.Invoke(this, new RexMapperEventArgs(typeof(T), result.GetType()));
                return result;
            }
            else
            {
                ObjectUnsolved?.Invoke(this, new RexMapperUnsolvedEventArgs(typeof(T)));
                return null;
            }
        }
        public IEnumerable<T> GetMany<T>() where T : class
        {
            var results = ResolveMany<T, T>(_typeToObject);
            if (results != null)
            {
                ObjectResolved?.Invoke(this, new RexMapperEventArgs(typeof(T), results.GetType()));
                return results;
            }
            else
            {
                ObjectUnsolved?.Invoke(this, new RexMapperUnsolvedEventArgs(typeof(T)));
                return null;
            }
        }
        public object Get(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type getterType = typeof(RexMappingGenericObjectGetter<>).MakeGenericType(new Type[] { type });
            IRexMappingGenericObjectGetter getter = (IRexMappingGenericObjectGetter)Activator.CreateInstance(getterType);
            return getter.GetObject(this);
        }
        public IEnumerable<object> GetMany(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type getterType = typeof(RexMappingGenericObjectGetter<>).MakeGenericType(new Type[] { type });
            IRexMappingGenericObjectGetter getter = (IRexMappingGenericObjectGetter)Activator.CreateInstance(getterType);
            return getter.GetObjects(this);
        }
        public void Handle<T>(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            IRexHandler<T> result = Resolve<T, IRexHandler<T>>(_typeToHandler, true, true, out RexMappingInfo info);
            if (result != null)
            {
                HandlerResolved?.Invoke(this, new RexMapperHandlerEventArgs(typeof(T), result.GetType(), value));
                result.Handle(value);
            }
            else
            {
                HandlerUnsolved?.Invoke(this, new RexMapperUnsolvedEventArgs(typeof(T)));
            }
        }
        public void HandleMany<T>(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            IEnumerable<IRexHandler<T>> results = ResolveMany<T, IRexHandler<T>>(_typeToHandler);
            if (results != null)
            {
                HandlerResolved?.Invoke(this, new RexMapperHandlerEventArgs(typeof(T), results.GetType(), value));
                foreach (var result in results)
                {
                    result.Handle(value);
                }
            }
            else
            {
                HandlerUnsolved?.Invoke(this, new RexMapperUnsolvedEventArgs(typeof(T)));
            }
        }
        public T Produce<T>(string name) where T : class
        {
            IRexProducer<T> result = Resolve<T, IRexProducer<T>>(_typeToProducer, true, true, out RexMappingInfo info);
            if (result != null)
            {
                var product = result.Produce(name);
                if (product != null)
                {
                    info?.AddName(name, true);
                    ProducerResolved?.Invoke(this, new RexMapperProducerEventArgs(typeof(T), result.GetType(), name, product));
                    return product;
                }
            }

            info?.AddName(name, false);
            ProducerUnsolved?.Invoke(this, new RexMapperUnsolvedEventArgs(typeof(T)));
            return default(T);
        }
        public bool Recycle<T>(string name, T product)
        {
            IRexProducer<T> result = Resolve<T, IRexProducer<T>>(_typeToProducer, true, true, out RexMappingInfo info);
            if (result != null)
            {
                bool success = result.Recycle(name, product);
                if (success)
                {
                    info?.AddName(name, true);
                    ProducerResolved?.Invoke(this, new RexMapperProducerEventArgs(typeof(T), result.GetType(), name, product));
                    return success;
                }
            }

            info?.AddName(name, false);
            ProducerUnsolved?.Invoke(this, new RexMapperUnsolvedEventArgs(typeof(T)));
            return false;
        }
        public T Assemble<T>(object target, string name)
        {
            IRexAssembler<T> result = Resolve<T, IRexAssembler<T>>(_typeToAssembler, true, true, out RexMappingInfo info);
            if (result != null)
            {
                var aResult = result.Assemble(target, name);
                if (aResult != null)
                {
                    AssemblerResolved?.Invoke(this, new RexMapperAssemblerEventArgs(typeof(T), result.GetType(), target, aResult));
                    return aResult;
                }
            }

            AssemblerUnsolved?.Invoke(this, new RexMapperUnsolvedEventArgs(typeof(T)));
            return default(T);
        }
        public IEnumerable<T> AssembleMany<T>(object target, string name)
        {
            IEnumerable<IRexAssembler<T>> results = ResolveMany<T, IRexAssembler<T>>(_typeToAssembler);
            if (results != null)
            {
                List<T> assembles = new List<T>();
                foreach (var result in results)
                {
                    var aResult = result.Assemble(target, name);
                    if (aResult != null)
                    {
                        AssemblerResolved?.Invoke(this, new RexMapperAssemblerEventArgs(typeof(T), results.GetType(), target, aResult));
                    }
                    assembles.Add(aResult);
                }
                return assembles;
            }

            AssemblerUnsolved?.Invoke(this, new RexMapperUnsolvedEventArgs(typeof(T)));
            return EmptyArray<T>.Empty;
        }
        public T Reduce<T>(T state, string name, object payload)
        {
            IRexReducer<T> result = Resolve<T, IRexReducer<T>>(_typeToReducer, true, true, out RexMappingInfo info);
            if (result != null)
            {
                var newState = result.Reduce(state, name, payload);
                if (newState != null)
                {
                    ReducerResolved?.Invoke(this, new RexMapperReducerEventArgs(typeof(T), result.GetType(), state, newState));
                    return newState;
                }
            }

            ReducerUnsolved?.Invoke(this, new RexMapperUnsolvedEventArgs(typeof(T)));
            return default(T);
        }
        public IRexMediator<T> GetMediator<T>(T target)
        {
            IRexMediator<T> result = Resolve<T, IRexMediator<T>>(_typeToMediator, false, false, out RexMappingInfo info);
            if (result != null)
            {
                MediatorResolved?.Invoke(this, new RexMapperEventArgs(typeof(T), result.GetType()));
                result.InitializeTarget(this, target);
                return result;
            }
            else
            {
                MediatorUnsolved?.Invoke(this, new RexMapperUnsolvedEventArgs(typeof(T)));
                return null;
            }
        }
        public IEnumerable<IRexMediator<T>> GetMediators<T>(T target)
        {
            return ResolveMany<T, IRexMediator<T>>(_typeToMediator).Select(o =>
            {
                MediatorResolved?.Invoke(this, new RexMapperEventArgs(typeof(T), o.GetType()));
                o.InitializeTarget(this, target);
                return o;
            });
        }

        private TImplement Resolve<T, TImplement>(Dictionary<Type, RexMappingCollection> dic, bool tryExternal, bool tryEnvService, out RexMappingInfo info) where TImplement : class
        {
            TImplement result = null;

            RexMappingCollection collection = dic.GetValueOrDefault(typeof(T));
            info = collection?.First();
            if (info != null)
            {
                result = info.Resolve<TImplement>();
                if (result != null)
                {
                    return result;
                }
            }

            if (tryExternal)
            {
                foreach (var resolver in _externalResolvers)
                {
                    result = resolver.GetService(typeof(TImplement)) as TImplement;
                    if (result != null && !_doNotProvide.Contains(result.GetType()))
                    {
                        if (collection == null)
                        {
                            collection = new RexMappingCollection(typeof(T));
                            // dic.Add(typeof(T), collection); 不要加入
                        }
                        info = collection.IncreaseExternalResolved();
                        return result;
                    }
                }
            }

            if (tryEnvService && _useEnvService)
            {
                result = Suity.Environment.GetService(typeof(TImplement)) as TImplement;
                if (result != null && !_doNotProvide.Contains(result.GetType()))
                {
                    if (collection == null)
                    {
                        collection = new RexMappingCollection(typeof(T));
                        //dic.Add(typeof(T), collection); 不要加入
                    }
                    info = collection.IncreaseExternalResolved();
                    return result;
                }
            }

            return null;
        }
        private IEnumerable<TImplement> ResolveMany<T, TImplement>(Dictionary<Type, RexMappingCollection> dic) where TImplement : class
        {
            RexMappingCollection collection = dic.GetValueOrDefault(typeof(T));
            if (collection != null)
            {
                return collection.Infos.Select(o => o.Resolve<TImplement>()).OfType<TImplement>();
            }
            else
            {
                return EmptyArray<TImplement>.Empty;
            }
        }
        private bool InfoFilter(RexMappingInfo info)
        {
            return !_doNotProvide.Contains(info.ImplementType);
        } 
        #endregion

        public IEnumerable<Type> DisabledTypes => _doNotProvide.Select(o => o);
        public IEnumerable<RexMappingCollection> ObjectTypes => _typeToObject.Values.Select(o => o);
        public IEnumerable<RexMappingCollection> HandlerTypes => _typeToHandler.Values.Select(o => o);
        public IEnumerable<RexMappingCollection> ProducerTypes => _typeToProducer.Values.Select(o => o);
        public IEnumerable<RexMappingCollection> AssemblerTypes => _typeToAssembler.Values.Select(o => o);
        public IEnumerable<RexMappingCollection> MediatorTypes => _typeToMediator.Values.Select(o => o);

    }
}
