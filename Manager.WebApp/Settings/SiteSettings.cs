using Manager.WebApp.Services;

namespace Manager.WebApp.Settings
{
    public interface ISettings
    {
        GeneralSettings General { get; }

        CacheSettings Cache { get; }

        void Load(ISettingsService settingsService, bool useCache=false);

        void Save(ISettingsService settingsService);

        void Save(string settingTypeName, ISettingsService settingsService);
    }

    public class SiteSettings : ISettings
    {
        private GeneralSettings _generalSettings;
        public GeneralSettings General { get { return _generalSettings; } }

        private CacheSettings _cacheSettings;
        public CacheSettings Cache { get { return _cacheSettings; } }

        public SiteSettings()
        {
            //Init all settings -- loading after
            _generalSettings = new GeneralSettings();
            _cacheSettings = new CacheSettings();
        }

        public void Load(ISettingsService settingsService, bool useCache = false)
        {
            _generalSettings = CreateSettings<GeneralSettings>(settingsService, useCache);
            _cacheSettings = CreateSettings<CacheSettings>(settingsService, useCache);
        }

        public void Save(ISettingsService settingsService)
        {
            if (_generalSettings != null)
                _generalSettings.Save(settingsService);

            if (_cacheSettings != null)
                _cacheSettings.Save(settingsService);
        }

        public void Save(string settingTypeName, ISettingsService settingsService)
        {
            if (_generalSettings != null && _generalSettings.GetType().Name == settingTypeName)
            {
                _generalSettings.Save(settingsService);
                return;
            }

            if (_cacheSettings != null && _cacheSettings.GetType().Name == settingTypeName)
            {
                _cacheSettings.Save(settingsService);
                return;
            }
        }

        private T CreateSettings<T>(ISettingsService settingsService, bool useCache) where T : SettingsBase, new()
        {
            var settings = new T();
            settings.Load(settingsService, useCache);
            return settings;
        }
    }

    public interface IFrontEndSettings
    {
        GeneralFrontEndSettings General { get; }

        void Load(IFrontEndSettingsService settingsService, bool useCache = false);

        void Save(IFrontEndSettingsService settingsService);

        void Save(string settingTypeName, IFrontEndSettingsService settingsService);
    }

    public class SiteFrontEndSettings : IFrontEndSettings
    {
        private GeneralFrontEndSettings _generalFrontEndSettings;
        public GeneralFrontEndSettings General { get { return _generalFrontEndSettings; } }

        public SiteFrontEndSettings()
        {
            //Init all settings -- loading after
            _generalFrontEndSettings = new GeneralFrontEndSettings();
        }

        public void Load(IFrontEndSettingsService settingsService, bool useCache = false)
        {
            _generalFrontEndSettings = CreateFrontEndSettings<GeneralFrontEndSettings>(settingsService, useCache);
        }

        public void Save(IFrontEndSettingsService settingsService)
        {
            if (_generalFrontEndSettings != null)
                _generalFrontEndSettings.Save(settingsService);
        }

        public void Save(string settingTypeName, IFrontEndSettingsService settingsService)
        {
            if (_generalFrontEndSettings != null && _generalFrontEndSettings.GetType().Name == settingTypeName)
            {
                _generalFrontEndSettings.Save(settingsService);
                return;
            }           
        }

        private T CreateFrontEndSettings<T>(IFrontEndSettingsService settingsService, bool useCache) where T : FrontEndSettingsBase, new()
        {
            var settings = new T();
            settings.Load(settingsService, useCache);
            return settings;
        }
    }
}
