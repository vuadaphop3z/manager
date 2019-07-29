using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Web.Configuration;

using Manager.SharedLib.Caching.Providers;

namespace Manager.SharedLib.Caching
{
    public class CacheProviderManager
    {
        static CacheProviderManager()
        {
            Initialize();
        }

        private static CacheProviderBase _default;
        /// <summary>
        /// Returns the default configured data provider
        /// </summary>
        public static CacheProviderBase Default
        {
            get { return _default; }
        }

        private static CacheProviderCollection _providerCollection;
        /// <summary>
        /// .Returns the provider collection
        /// </summary>
        public static CacheProviderCollection Providers
        {
            get { return _providerCollection; }
        }

        private static ProviderSettingsCollection _providerSettings;
        public static ProviderSettingsCollection ProviderSettings
        {
            get { return _providerSettings; }
        }

        /// <summary>
        /// Reads the configuration related to the set of configured 
        /// providers and sets the default and collection of providers and settings.
        /// </summary>
        public static void Initialize()
        {
            CacheProviderConfiguration configSection = (CacheProviderConfiguration)ConfigurationManager.GetSection("CacheProviders");
            if (configSection == null)
#pragma warning disable CS0618 // Type or member is obsolete
                throw new ConfigurationException("Data provider section is not set.");
#pragma warning restore CS0618 // Type or member is obsolete

            _providerCollection = new CacheProviderCollection();
            ProvidersHelper.InstantiateProviders(configSection.Providers, _providerCollection, typeof(CacheProviderBase));

            _providerSettings = configSection.Providers;

            if (_providerCollection[configSection.DefaultProviderName] == null)
#pragma warning disable CS0618 // Type or member is obsolete
                throw new ConfigurationException("Default data provider is not set.");
#pragma warning restore CS0618 // Type or member is obsolete

            _default = _providerCollection[configSection.DefaultProviderName];

            var defaultSettings = _providerSettings[configSection.DefaultProviderName];

            _default.SetParameters(defaultSettings.Parameters);
        }
    }
}
