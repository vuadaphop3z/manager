using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.ComponentModel.DataAnnotations;

namespace Manager.WebApp.Settings
{
    public class CacheSettings: SettingsBase
    {
        [Required]
        [Display(Name = "Caching Provider", Description = "Choose the proper cache provider supported by web hosting")]        
        public string CacheProvider { get; set; }

        [Required]
        [Display(Name = "System Default Cache Duration In Minutes", Description = "The system default cache duration in minutes")]        
        public int SystemDefaultCacheDuration { get; set; }

        [Required]
        [Display(Name = "Website Cache Duration In Minutes", Description = "The overrided cache duration for Web which procedure the data for Website")]        
        public int WebsiteCacheDuration { get; set; }
    }
}
