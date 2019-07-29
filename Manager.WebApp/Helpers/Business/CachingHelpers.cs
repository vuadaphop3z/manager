using System;
using Autofac;
using Manager.SharedLib.Caching.Providers;
using Manager.SharedLib.Logging;

namespace Manager.WebApp.Helpers
{
    public class CachingHelpers
    {
        private static readonly ILog logger = LogProvider.For<CachingHelpers>();

        public static void ClearUnitCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("UNITS");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearUnitCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearUserCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("USERS");
                else
                    cacheProvider.Clear(string.Format("USER_{0}", cacheKey));
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearUserCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearDeviceCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("DEVICES");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearDeviceCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearProviderCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("PROVIDERS");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearProviderCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearCurrencyCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("CURRENCIES");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearCurrencyCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearProductCategoryCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("PRODUCTCATEGORIES");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearProductCategoryCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearPropertyCategoryCache(string cacheKey = "")
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                if (string.IsNullOrEmpty(cacheKey))
                    cacheProvider.ClearAll("PROPERTYCATEGORIES");
                else
                    cacheProvider.Clear(cacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearPropertyCategoryCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static void ClearProductCache(int productId)
        {
            var strError = string.Empty;
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Clear(string.Format("PRODUCT_{0}", productId));
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed ClearProductCache: {0}", ex.ToString());
                logger.Error(strError);
            }
        }
    }
}