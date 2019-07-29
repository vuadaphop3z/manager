using System;
using System.Collections.Generic;
using StackExchange.Redis.Extensions.Core;
using Autofac;
using Manager.SharedLib.Caching.Providers;

namespace Manager.WebApp.Caching.Providers
{
    /// <summary>
    /// Use this cache in Web environment.
    /// </summary>
    public class RedisCacheProvider : CacheProviderBase
    {
        private readonly ICacheClient _redisCacheClient;

        #region Configuration Parameters
        private int _defaultCacheDuration = 30;

        public override void SetParameters(System.Collections.Specialized.NameValueCollection config)
        {
            try
            {
                _defaultCacheDuration = int.Parse(config["CacheDurationInMinutes"]);
                CacheDuration = _defaultCacheDuration;
            }
            catch { }
        }
        #endregion


        public RedisCacheProvider(ICacheClient redisCacheClient)
        {
            _redisCacheClient = redisCacheClient;
        }


        public RedisCacheProvider()
        {
            _redisCacheClient = Startup.AutofacContainer.Resolve<ICacheClient>();
        }

        protected override object InitCache()
        {
            return _redisCacheClient;
        }

        public override bool Get<T>(string key, out T value)
        {
            try
            {
                var obj = _redisCacheClient.Get<T>(key);

                if (obj == null)
                {
                    value = default(T);
                    return false;
                }

                value = (T)obj;
            }
            catch
            {
                value = default(T);
                return false;
            }

            return true;
        }

        public override void Set<T>(string key, T value)
        {
            Set<T>(key, value, CacheDuration);
        }

        public override void Set<T>(string key, T value, int duration)
        {
            _redisCacheClient.Add<T>(key, value, TimeSpan.FromMinutes(duration));
        }

        public override void Clear(string key)
        {
            _redisCacheClient.Remove(key);
        }


        public override void ClearAll(string keyPrefix = null)
        {
            var allItems = _redisCacheClient.SearchKeys(keyPrefix + "*");
            foreach (var item in allItems)
            {
                _redisCacheClient.Remove(item as string);
            }
        }

        public override IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return null;
        }
    }
}
