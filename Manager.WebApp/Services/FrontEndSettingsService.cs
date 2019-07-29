using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.SharedLib.Caching.Providers;
using MsSql.AspNet.Identity;

namespace Manager.WebApp.Services
{
    public class FrontEndSettingsService : ServiceBase, IFrontEndSettingsService
    {
        const string KEY_PREFIX = "FRONTEND_SETTINGS.";
        const string KEY_FRONTEND_SETTINGS_BY_TYPES = KEY_PREFIX + ".TYPES.{0}";

        //private readonly IOracleEpgStore _epgStore;
        private readonly IMsSqlFrontEndStore _myStore;
        private readonly ICacheProvider _cache;

        public FrontEndSettingsService(IMsSqlFrontEndStore myStore, ICacheProvider cacheProvider)
        {
            _myStore = myStore;
            _cache = cacheProvider;
        }


        public virtual Task<List<Setting>> LoadFrontEndSettings(string settingType, bool useCache = true)
        {
            List<Setting> result;
            if (useCache)
            {
                string cacheKey = string.Format(KEY_FRONTEND_SETTINGS_BY_TYPES, settingType);
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

        public virtual Task<bool> SaveFrontEndSettings(List<Setting> settings)
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

    public interface IFrontEndSettingsService
    {
        Task<List<Setting>> LoadFrontEndSettings(string settingType, bool useCache = true);

        Task<bool> SaveFrontEndSettings(List<Setting> settings);

        void ClearAllCacheData();
    }
}
