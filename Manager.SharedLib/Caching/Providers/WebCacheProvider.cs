using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;
using System.Web.Caching;

namespace Manager.SharedLib.Caching.Providers
{
    /// <summary>
    /// Use this cache in Web environment.
    /// </summary>
    public class WebCacheProvider : CacheProviderBase
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

        private Cache DataCache
        {
            get { return (Cache)_cache; }
        }

        protected override object InitCache()
        {
            return HttpRuntime.Cache;
        }

        public override bool Get<T>(string key, out T value)
        {
            try
            {
                if (DataCache[key] == null)
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
            DataCache.Insert(
                key,
                value,
                null,
                DateTime.Now.AddMinutes(duration),
                TimeSpan.Zero);
        }

        public override void Clear(string key)
        {
            DataCache.Remove(key);
        }


        public override void ClearAll(string keyPrefix = null)
        {
            foreach (DictionaryEntry item in DataCache)
            {
                if (keyPrefix==null || (keyPrefix!=null && (item.Key as string).StartsWith(keyPrefix)))
                { 
                    DataCache.Remove(item.Key as string);
                }
            }
        }

        public override IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            foreach (DictionaryEntry item in DataCache)
            {
                yield return new KeyValuePair<string, object>(item.Key as string, item.Value);
            }

        }
    }
}
