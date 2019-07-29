//using System;
//using System.Linq;
//using System.Reflection;
//using System.ComponentModel.DataAnnotations;
//using System.Configuration;
//using System.Globalization;

//namespace Manager.WebApp
//{
//    public static class AppSettings
//    {
//        public static string SampleSettingKey
//        {
//            get
//            {
//                return Setting<string>("SampleSettingKey");
//            }
//        }      

//        private static T Setting<T>(string name)
//        {
//            string value = ConfigurationManager.AppSettings[name];

//            if (value == null)
//            {
//                throw new Exception(String.Format("Could not find setting '{0}',", name));
//            }

//            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
//        }
//    }

    
//}