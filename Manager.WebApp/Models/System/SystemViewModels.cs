using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using System.Configuration;

using Manager.SharedLib.Caching;
using Manager.WebApp.Settings;
using System.Web;

namespace Manager.WebApp.Models
{
    public class SettingsViewModel
    {
        /// <summary>
        /// Forcus on current settings type
        /// </summary>
        public string CurrentSettingsType { get; set; }

        /// <summary>
        /// Contains all settings type instances
        /// </summary>
        public SiteSettings SystemSestings { get; set; }

        public IEnumerable<SelectListItem> TimeZoneList()
        {
            var timeZoneList = TimeZoneInfo
            .GetSystemTimeZones()
            .Select(t => new SelectListItem
            {
                Text = t.DisplayName,
                Value = t.Id,
                Selected = !string.IsNullOrEmpty(this.SystemSestings.General.TimeZoneId) && t.Id == this.SystemSestings.General.TimeZoneId
            });

            return timeZoneList;
        }

        public IEnumerable<SelectListItem> CacheProviderList()
        {
            List<SelectListItem> cacheProviderList = new List<SelectListItem>();

            foreach (ProviderSettings setting in CacheProviderManager.ProviderSettings)
            {
                var item = new SelectListItem()
                {
                    Text = setting.Name,
                    Value = setting.Name,
                    Selected = !string.IsNullOrEmpty(this.SystemSestings.Cache.CacheProvider) && setting.Name == this.SystemSestings.Cache.CacheProvider,
                };

                cacheProviderList.Add(item);
            }

            return cacheProviderList;
        }
    }

    public class FrontEndSettingsViewModel
    {
        /// <summary>
        /// Forcus on current settings type
        /// </summary>
        public string CurrentFrontEndSettingsType { get; set; }

        /// <summary>
        /// Contains all settings type instances
        /// </summary>
        public SiteFrontEndSettings SystemSestings { get; set; }

        public string CurrentLogo { get; set; }
        public List<HttpPostedFileBase> SiteLogoUpload { get; set; }
    }
}