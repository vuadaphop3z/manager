using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.SharedLib.Caching.Providers;
using MsSql.AspNet.Identity;

namespace Manager.WebApp.Services
{
    public class SettingsService : ServiceBase, ISettingsService
    {
        const string KEY_PREFIX = "SETTINGS.";
        const string KEY_SETTINGs_BY_TYPES = KEY_PREFIX + ".TYPES.{0}";

        //private readonly IOracleEpgStore _epgStore;
        private readonly IMsSqlStore _myStore;
        private readonly ICacheProvider _cache;

        public SettingsService(IMsSqlStore myStore, ICacheProvider cacheProvider)
        {
            _myStore = myStore;
            _cache = cacheProvider;
        }


        public virtual Task<List<Setting>> LoadSettings(string settingType, bool useCache = true)
        {
            List<Setting> result;
            if (useCache)
            {
                string cacheKey = string.Format(KEY_SETTINGs_BY_TYPES, settingType);
                if (!_cache.Get(cacheKey, out result))
                {
                    result = _myStore.LoadSettings(settingType).Result;
                    _cache.Set(cacheKey, result);
                }
            }
            else
            {
                result = _myStore.LoadSettings(settingType).Result;
            }

            return Task.FromResult<List<Setting>>(result);
        }


        public virtual Task<bool> SaveSettings(List<Setting> settings)
        {
            this.ClearAllCacheData();
            var savedResult = _myStore.SaveSettings(settings).Result;
            return Task.FromResult<bool>(savedResult);
        }

        public virtual void ClearAllCacheData()
        {
            _cache.ClearAll(KEY_PREFIX);
        }
    }

    public interface ISettingsService
    {
        Task<List<Setting>> LoadSettings(string settingType, bool useCache = true);

        Task<bool> SaveSettings(List<Setting> settings);

        void ClearAllCacheData();
    }
}
