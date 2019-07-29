using System;
using System.Configuration;

namespace Manager.WebApp.Settings
{
    public class SystemSettings
    {
        public static string FileManagerServer
        {
            get
            {
                return ConfigurationManager.AppSettings["System:FileManagerServer"];
            }
        }

        public static string CultureKey
        {
            get
            {
                return ConfigurationManager.AppSettings["System:CultureKey"];
            }
        }
       

        public static string FrontendUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["System:FrontendUrl"];
            }
        }

        public static int DefaultPageSize
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:DefaultPageSize"]);
            }
        }

        public static string VietNamDateTimeFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["System:VietNamDateTimeFormat"].ToString();
            }
        }

        public static int ExtenalSeviceTimeOutInSeconds
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:ExtenalSeviceTimeOutInSeconds"]);
            }
        }

        public static string MyCloudServer
        {
            get
            {
                return ConfigurationManager.AppSettings["System:MyCloudServer"].ToString();
            }
        }

        public static string EmailFromAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailFromAddress"].ToString();
            }
        }

        public static string MailAuthUser
        {
            get
            {
                return ConfigurationManager.AppSettings["MailAuthUser"].ToString();
            }
        }

        public static string EmailConfirmSubject
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailConfirmSubject"].ToString();
            }
        }

        public static int CacheExpireDataInDashBoard
        {
            get {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:CacheExpireDataInDashBoard"]);
            }
        }

        public static string AirlinesReportInDashBoard
        {
            get
            {
                return ConfigurationManager.AppSettings["System:AirlinesReportInDashBoard"];
            }
        }

        public static string AirlinesLabelInDashBoard
        {
            get
            {
                return ConfigurationManager.AppSettings["System:AirlinesLabelInDashBoard"];
            }
        }

        public static string CommonCacheKeyPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["System:CommonCacheKeyPrefix"];
            }
        }

        public static string CoreContainerServer
        {
            get
            {
                return ConfigurationManager.AppSettings["CoreCdn:ContainerServer"];
            }
        }

        public static string CoreCdnReadContentLink
        {
            get
            {
                return ConfigurationManager.AppSettings["CoreCdn:CdnReadContentLink"];
            }
        }

        public static string SocialContainerServer
        {
            get
            {
                return ConfigurationManager.AppSettings["SocialCdn:ContainerServer"];
            }
        }

        public static string SocialCdnReadContentLink
        {
            get
            {
                return ConfigurationManager.AppSettings["SocialCdn:CdnReadContentLink"];
            }
        }

    }

    public class ImageSettings
    {        
        public static int AvatarWidth
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["Img:AvatarWidth"]);
            }
        }

        public static int AvatarHeight
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["Img:AvatarHeight"]);
            }
        }

        public static string AvatarFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["Img:AvatarFolder"];
            }
        }

        public static int MaxFileSizeUploadLength
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["Img:MaxFileSizeUploadLength"]);
            }
        }
    }

    public class MapSettings
    {
        public static string DefaultLatitude
        {
            get
            {
                return ConfigurationManager.AppSettings["Map:DefaultLatitude"];
            }
        }

        public static string DefaultLongitude
        {
            get
            {
                return ConfigurationManager.AppSettings["Map:DefaultLongitude"];
            }
        }


        public static int DefaultZoomSize
        {
            get
            {
                return Utils.ConvertToInt32(ConfigurationManager.AppSettings["Map:DefaultZoomSize"]);
            }
        }
    }
}