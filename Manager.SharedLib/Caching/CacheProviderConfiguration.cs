using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Configuration.Provider;

namespace Manager.SharedLib.Caching
{
    
    public class CacheProviderConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get
            {
                return (ProviderSettingsCollection)base["providers"];
            }
        }

        [ConfigurationProperty("default", DefaultValue = "WebCacheProvider")]
        public string DefaultProviderName
        {
            get
            {
                return base["default"] as string;
            }
        }
    }
}
