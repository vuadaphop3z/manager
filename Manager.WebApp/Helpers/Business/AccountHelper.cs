using Manager.SharedLib.Logging;
using MsSql.AspNet.Identity;
using System;
using Manager.SharedLib.Caching.Providers;
using Autofac;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Manager.WebApp.Helpers
{
    public class AccountHelper
    {
        private static readonly ILog logger = LogProvider.For<AccountHelper>();

        public static IdentityUser GetUserById(string userId)
        {
            var myKey = string.Format("{0}_{1}", "USER", userId);
            IdentityUser info = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

                cacheProvider.Get<IdentityUser>(myKey, out info);

                if (info == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IIdentityStore>();
                    info = myStore.GetUserByID(userId);

                    //Storage to cache
                    if(info != null)
                        cacheProvider.Set(myKey, info);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetUserById: " + ex.ToString());
            }

            return info;
        }

        public static bool CurrentUserIsAdmin()
        {
            try
            {
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();
                var currentUser = GetUserById(currentUserId);
                if(currentUser != null)
                {
                    var currentUserName = currentUser.UserName.ToLower();
                    if (currentUser.UserName == "admin" || currentUser.UserName == "bangvl")
                        return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not check CurrentUserIsAdmin: " + ex.ToString());
            }

            return false;
        }
    }
}