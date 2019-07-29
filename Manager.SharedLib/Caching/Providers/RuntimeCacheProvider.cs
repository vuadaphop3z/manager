using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Manager.SharedLib.Caching.Providers
{
    /// <summary>
    /// Use this cache for both ASP.NET or WIN FORM Applications - Runtime caching
    /// </summary>
    public class RuntimeCacheProvider : CacheProviderBase
    {
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


        private ObjectCache DataCache {
            get { return (ObjectCache)_cache; }
        }

        protected override object InitCache()
        {
            return System.Runtime.Caching.MemoryCache.Default;
        }

        public override bool Get<T>(string key, out T value)
        {
            try
            {
                if ((object)DataCache[key] == null)
                {
                    value = default(T);
                    return false;
                }

                value = (T)DataCache[key];
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
            DataCache.Set(
                key,
                value,                
                DateTime.Now.AddMinutes(duration));
        }

        public override void Clear(string key)
        {
            if (DataCache.Contains(key))
            {
                DataCache.Remove(key);
            }
        }


        public override void ClearAll(string keyPrefix)
        {
            foreach (var element in DataCache)
            {
                if (keyPrefix == null || (keyPrefix != null && (element.Key as string).StartsWith(keyPrefix)))
                {
                    DataCache.Remove(element.Key);
                }
            }
        }

        public override IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            foreach (var item in DataCache)
            {                
                yield return new KeyValuePair<string, object>(item.Key as string, item.Value);
            }
        }
    }

}
