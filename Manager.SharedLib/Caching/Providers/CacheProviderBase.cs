/*
 http://blog.codeishard.net/2012/11/07/simple-generic-c-caching-class-i-use-this-all-the-time/
 
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;


namespace Manager.SharedLib.Caching.Providers
{    
    public abstract class CacheProviderBase : ProviderBase, ICacheProvider
    {       
        public virtual void SetParameters(NameValueCollection config)
        {

        }

        public int CacheDuration
        {
            get;
            set;
        }

        private int defaultCacheDurationInMinutes = 30;

        protected object _cache;

        public CacheProviderBase()
        {
            CacheDuration = defaultCacheDurationInMinutes;
            _cache = InitCache();
        }
        public CacheProviderBase(int durationInMinutes)
        {
            CacheDuration = durationInMinutes;
            _cache = InitCache();
        }

        protected abstract object InitCache();

        public abstract bool Get<T>(string key, out T value);

        public abstract void Set<T>(string key, T value);

        public abstract void Set<T>(string key, T value, int duration);

        public abstract void Clear(string key);

        public abstract void ClearAll(string keyPrefix);

        public abstract IEnumerable<KeyValuePair<string, object>> GetAll();
    }
}
