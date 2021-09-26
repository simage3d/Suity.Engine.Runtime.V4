using Suity.Engine;
using Suity.Helpers;
using Suity.Synchonizing;
using Suity.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Networking
{
    public abstract class DataCacheComponent<TKey, TValue> : NodeComponent, IDataCache<TKey, TValue>, IViewObject
        where TValue : class
    {
        static readonly TimeSpan DefaultHealthCheckDuration = TimeSpan.FromSeconds(300);

        IDataStorage<TKey, TValue> _storage;
        readonly ConcurrentDictionary<TKey, TValue> _caches = new ConcurrentDictionary<TKey, TValue>();
        readonly ConcurrentDictionary<TKey, TValue> _unSavedCaches = new ConcurrentDictionary<TKey, TValue>();

        DateTime _lastHealthCheck;
        readonly object _healthCheckLock = new object();

        float _durationS = 300;
        TimeSpan _duration = DefaultHealthCheckDuration;
        protected float HealthCheckDurationS
        {
            get => _durationS;
            set
            {
                if (_durationS != value)
                {
                    _durationS = value;
                }
                _duration = TimeSpan.FromSeconds(value);
            }
        }

        protected override void OnStart()
        {
            _storage = GetComponent<IDataStorage<TKey, TValue>>();
            if (_storage == null)
            {
                throw new NullReferenceException("Component implmentation not found : " + typeof(IDataStorage<TKey, TValue>).GetTypeId());
            }
        }

        #region IDataCache<T>
        public int Count { get { return _caches.Count; } }

        public TValue Get(TKey id)
        {

            if (_unSavedCaches.TryRemove(id, out TValue cache))
            {
                return _caches.GetOrAdd(id, o => cache);
            }
            else if (_caches.TryGetValue(id, out cache))
            {
                return cache;
            }
            else
            {
                return null;
            }
        }

        public TValue GetOrLoad(TKey id)
        {
            TValue cache = Get(id);
            if (cache != null)
            {
                return cache;
            }

            cache = _storage.GetData(id);
            if (cache != null && _caches.TryAdd(id, cache))
            {
                OnCacheAdded(cache);
                return cache;
            }
            else
            {
                return Get(id);
            }
        }

        public TValue GetOrCreateNew(TKey id, Func<TValue> creation)
        {
            return _caches.GetOrAdd(id, pid =>
            {
                if (_unSavedCaches.TryRemove(id, out TValue cache))
                {
                    return cache;
                }

                cache = _storage.GetData(id);
                if (cache != null)
                {
                    OnCacheAdded(cache);
                    return cache;
                }
                else
                {
                    cache = creation();
                    if (cache == null)
                    {
                        throw new NullReferenceException();
                    }
                    _storage.SetData(cache);
                    OnCacheAdded(cache);
                    return cache;
                }
            });
        }

        public TValue Remove(TKey id)
        {
            if (_caches.TryRemove(id, out TValue cache))
            {
                if (!_storage.SetData(cache))
                {
                    _unSavedCaches[id] = cache;
                }

                OnCacheRemoved(cache);
                return cache;
            }
            else
            {
                return null;
            }
        }

        public void Foreach(Action<TValue> action)
        {
            foreach (var item in _caches.Values)
            {
                action(item);
            }
        }

        public bool Save(TKey id)
        {
            var cache = Get(id);
            if (cache != null)
            {
                _storage.SetData(cache);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SaveAll()
        {
            Logs.LogInfo("Perform saving all " + typeof(TValue).Name + " caches....");

            Dictionary<TKey, TValue> caches = new Dictionary<TKey, TValue>();

            foreach (var item in _caches)
            {
                caches[item.Key] = item.Value;
            }
            foreach (var item in _unSavedCaches)
            {
                if (!caches.ContainsKey(item.Key))
                {
                    caches[item.Key] = item.Value;
                }
            }

            bool allSaved = true;

            foreach (var cache in caches.Values)
            {
                if (!_storage.SetData(cache))
                {
                    allSaved = false;
                }
            }

            return allSaved;
        }

        public void HealthCheck()
        {
            lock (_healthCheckLock)
            {
                DateTime now = DateTime.Now;
                TimeSpan span = now - _lastHealthCheck;
                if (span > _duration)
                {
                    Logs.LogInfo("Perform " + typeof(TValue).Name + " cache health check...");
                    _lastHealthCheck = now;
                    CheckUnSavedCaches();
                }
            }
        }
        private void CheckUnSavedCaches()
        {
            List<TKey> saved = new List<TKey>();
            int num = 0;

            foreach (var cache in _unSavedCaches)
            {
                if (!_caches.ContainsKey(cache.Key))
                {
                    if (_storage.SetData(cache.Value))
                    {
                        saved.Add(cache.Key);
                        num++;
                    }
                }
                else
                {
                    saved.Add(cache.Key);
                }
            }

            foreach (var id in saved)
            {
                _unSavedCaches.TryRemove(id, out TValue cache);
            }

            if (num > 0)
            {
                Logs.LogInfo("Saved " + num + " " + typeof(TValue).Name);
            }
        }
        #endregion

        public int UnSavedCount => _unSavedCaches.Count;


        protected virtual void OnCacheAdded(TValue cache)
        {
        }
        protected virtual void OnCacheRemoved(TValue cache)
        {
        }

        public virtual void Sync(IPropertySync sync, ISyncContext context)
        {
            HealthCheckDurationS = sync.Sync(nameof(HealthCheckDurationS), HealthCheckDurationS, SyncFlag.None, 300);
        }

        public virtual void SetupView(IViewObjectSetup setup)
        {
            setup.InspectorField(HealthCheckDurationS, new ViewProperty(nameof(HealthCheckDurationS), "检测时长(秒)"));
        }
    }
}
