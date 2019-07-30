using System;
using System.Collections.Generic;
using Autofac;
using Manager.SharedLib.Caching.Providers;
using Manager.SharedLib.Logging;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using MsSql.AspNet.Identity.MsSqlStores.Business;
using MsSql.AspNet.Identity.Repositories;
using StackExchange.Redis.Extensions.Core;

namespace Manager.WebApp.Helpers
{
    public class CommonHelpers
    {
        private static readonly ILog logger = LogProvider.For<CommonHelpers>();

        public static List<IdentityUser> GetListUser()
        {
            var myKey = "USERS";
            List<IdentityUser> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityUser>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IIdentityStore>();
                    myList = myStore.GetListUser();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListUser: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityProvider> GetListProvider()
        {
            var myKey = "PROVIDERS";
            List<IdentityProvider> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityProvider>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreProvider>();
                    myList = myStore.GetList();

                    if(myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListProvider: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityDevice> GetListDevice()
        {
            var myKey = "DEVICES";
            List<IdentityDevice> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityDevice>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreDevice>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList, 2);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListDevice: " + ex.ToString());
            }

            return myList;
        }

        public static int RegisterNewDevice(IdentityDevice deviceInfo, out bool isNew)
        {
            var newId = 0;
            isNew = false;
            try
            {
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreDevice>();

                if(deviceInfo != null)
                {
                    newId = myStore.RegisterNewDevice(deviceInfo, out isNew);
                    if(isNew)
                    {
                        //Clear cache
                        CachingHelpers.ClearDeviceCache();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not RegisterNewDevice: " + ex.ToString());
            }

            return newId;
        }

        public static List<IdentityProductCategory> GetListProductCategory()
        {
            var myKey = "PRODUCTCATEGORIES";
            List<IdentityProductCategory> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityProductCategory>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreProductCategory>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListProductCategory: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityArea> GetListArea()
        {
            var myKey = "AREAS";
            List<IdentityArea> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityArea>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreArea>();
                    myList = myStore.Area_GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListArea: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityCountry> GetListCountry()
        {
            var myKey = "COUNTRYS";
            List<IdentityCountry> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityCountry>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreArea>();
                    myList = myStore.Country_GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListArea: " + ex.ToString());
            }

            return myList;
        }


        // 20190724 Oai ADD
        public static List<IdentityProvince> GetListProvince()
        {
            var myKey = "PROVINCES";
            List<IdentityProvince> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityProvince>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreArea>();
                    myList = myStore.Province_GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListArea: " + ex.ToString());
            }

            return myList;
        }

        // 20190724 Oai ADD
        public static List<IdentityDistrict> GetListDistrict()
        {
            var myKey = "PROVINCES";
            List<IdentityDistrict> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityDistrict>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreArea>();
                    myList = myStore.District_GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListArea: " + ex.ToString());
            }

            return myList;
        }


        public static List<IdentityCountry> GetCountryByArea(int areaId)
        {
            var myKey = string.Format("{0}_{1}", "COUNTRIES", areaId);
            List<IdentityCountry> myList = null;

            if (areaId <= 0)
                return new List<IdentityCountry>();
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityCountry>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreArea>();
                    myList = myStore.Country_GetByArea(areaId);

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCountry: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityProvince> GetProvinceByCountry(int countryId)
        {
            var myKey = string.Format("{0}_{1}", "PROVINCES", countryId);
            List<IdentityProvince> myList = null;

            if (countryId <= 0)
                return new List<IdentityProvince>();
            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityProvince>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreArea>();
                    myList = myStore.Province_GetByCountry(countryId);

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListProvince: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityDistrict> GetDistrictByProvince(int provinceId)
        {
            var myKey = string.Format("{0}_{1}", "DISTRICTS", provinceId);
            List<IdentityDistrict> myList = null;

            if (provinceId <= 0)
                return new List<IdentityDistrict>();

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityDistrict>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreArea>();
                    myList = myStore.District_GetByProvince(provinceId);

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListDistrict: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityUnit> GetListUnit()
        {
            var myKey = "UNITS";
            List<IdentityUnit> myList = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.Get<List<IdentityUnit>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreUnit>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheProvider.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListUnit: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityPolicy> GetListPolicy()
        {
            var myKey = string.Format("{0}{1}", SystemSettings.CommonCacheKeyPrefix, "POLICIES");
            List<IdentityPolicy> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityPolicy>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePolicy>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListPolicy: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityCurrency> GetListCurrency()
        {
            var myKey = "CURRENCIES";
            List<IdentityCurrency> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityCurrency>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCurrency>();
                    myList = myStore.GetList();

                    if(myList.HasData())
                        //Storage to cache
                        cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCurrency: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityCredit> GetListCredit()
        {
            var myKey = "CREDITS";
            List<IdentityCredit> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityCredit>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCredit>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCredit: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityPayment> GetListPayment()
        {
            var myKey = string.Format("{0}{1}", SystemSettings.CommonCacheKeyPrefix, "PAYMENTS");
            List<IdentityPayment> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityPayment>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStorePayment>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListPayment: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityCategory> GetListCategory()
        {
            var myKey = string.Format("{0}{1}", SystemSettings.CommonCacheKeyPrefix, "CATEGORIES");
            List<IdentityCategory> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityCategory>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCategory>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCategory: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityLanguage> GetListLanguages()
        {
            var myKey = string.Format("{0}{1}", SystemSettings.CommonCacheKeyPrefix, "LANGUAGES");
            List<IdentityLanguage> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityLanguage>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreLanguage>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListLanguages: " + ex.ToString());
            }

            return myList;
        }



        public static List<IdentityLanguage> GetListLanguageNotExist(List<string> listLangExists)
        {
            var myKey = string.Format("{0}{1}", SystemSettings.CommonCacheKeyPrefix, "LANGUAGES");
            List<IdentityLanguage> myList = null;

            try
            {
                //Check from cache first
                var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
                myList = cacheClient.Get<List<IdentityLanguage>>(myKey);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreLanguage>();
                    myList = myStore.GetList();

                    //Storage to cache
                    cacheClient.Add(myKey, myList);
                }

                if (myList != null && myList.Count > 0)
                {
                    for (int i = 0; i < myList.Count; i++)
                    {
                        if (listLangExists != null && listLangExists.Count > 0)
                        {
                            var item = myList[i];
                            var itemCheck = listLangExists.Contains(item.LangCode);
                            if (itemCheck)
                            {
                                myList.RemoveAt(i); i--;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListLanguages: " + ex.ToString());
            }

            return myList;
        }

        // 20190726 Oai ADD

        public static List<IdentityCompany> GetListCompany()
        {
            var myKey = "COMPANYS";
            List<IdentityCompany> myList = null;

            try
            {
                //Check from cache first
                var cacheCompany = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheCompany.Get<List<IdentityCompany>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreCompany>();
                    myList = myStore.GetList();

                    if (myList.HasData())
                        //Storage to cache
                        cacheCompany.Set(myKey, myList);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetListCompany: " + ex.ToString());
            }

            return myList;
        }

        public static List<IdentityProjectCategory> GetListProjectCatefory()
        {
            var myKey = "PROJECTCATEGORYS";
            List<IdentityProjectCategory> myList = null;

            try
            {
                //Check from cache first
                var cacheProjectCatefory = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProjectCatefory.Get<List<IdentityProjectCategory>>(myKey, out myList);

                if (myList == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IStoreProjectCategory>();
                    myList = myStore.GetList();

                    if(myList.HasData())
                        //Storage to cache
                        cacheProjectCatefory.Set(myKey, myList);

                }

            }
           catch (Exception ex)
            {
                logger.Error("Could not GetListProjectCatefory: " + ex.ToString());
            }

            return myList;
        }

    }
}