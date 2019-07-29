using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Settings
{
    public class AuthorizationCoreSettings
    {
        public static string HeaderKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeCore:HeaderKey"];
            }
        }

        public static string HeaderValue
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeCore:HeaderValue"];
            }
        }

        public static string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeCore:Username"];
            }
        }

        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeCore:Password"];
            }
        }
    }

    public class AuthorizationSocialSettings
    {
        public static string HeaderKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSocial:HeaderKey"];
            }
        }

        public static string HeaderValue
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSocial:HeaderValue"];
            }
        }

        public static string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSocial:Username"];
            }
        }

        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthorizeSocial:Password"];
            }
        }
    }
}