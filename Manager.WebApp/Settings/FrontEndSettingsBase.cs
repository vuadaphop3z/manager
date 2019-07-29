using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manager.WebApp.Services;
using MsSql.AspNet.Identity;

namespace Manager.WebApp.Settings
{
    public abstract class FrontEndSettingsBase
    {
        // 1 name and properties cached in readonly fields
        private readonly string _name;
        private readonly PropertyInfo[] _properties;

        public FrontEndSettingsBase()
        {
            var type = this.GetType();
            _name = type.Name;
            // 2
            _properties = type.GetProperties();
        }

        public virtual void Load(IFrontEndSettingsService settingsService, bool useCache)
        {
            // ARGUMENT CHECKING SKIPPED FOR BREVITY
            // 3 get settings for this type name
            var settings = settingsService.LoadFrontEndSettings(this._name, useCache).Result;
            if (settings != null && settings.Count>0)
            {
                foreach (var propertyInfo in _properties)
                {
                    // get the setting from the settings list
                    var setting = settings.SingleOrDefault(s => s.Name == propertyInfo.Name);
                    if (setting != null)
                    {
                        // 4 assign the setting values to the properties in the type inheriting this class
                        propertyInfo.SetValue(this, Convert.ChangeType(setting.Value, propertyInfo.PropertyType));
                    }
                }
            }
        }


        public virtual void Save(IFrontEndSettingsService settingsService)
        {
            // 5 load existing settings for this type            
            var settings = new List<Setting>();
            foreach (var propertyInfo in _properties)
            {
                object propertyValue = propertyInfo.GetValue(this, null);
                string value = (propertyValue == null) ? null : propertyValue.ToString();
                
                // 7 create new setting
                var newSetting = new Setting()
                {
                    Name = propertyInfo.Name,
                    Type = _name,
                    Value = value,
                };

                settings.Add(newSetting);
            }
            
            if (settings!=null && settings.Count>0)
            {               
                settingsService.SaveFrontEndSettings(settings);
            }
        }
    }
}
