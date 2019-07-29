using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Manager.WebApp
{
    public class AdminSettings
    {
        public static int PageSize
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:DefaultPageSize"]);
            }
        }
    }
}