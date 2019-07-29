using System.Web;
using Microsoft.AspNet.Identity;

using MsSql.AspNet.Identity;

using Autofac;
using System.Collections.Generic;
using Manager.SharedLib.Caching.Providers;
using System;
using Manager.SharedLib.Logging;

namespace Manager.WebApp.Helpers
{
    public class PermissionHelper
    {
        private static readonly ILog logger = LogProvider.For<PermissionHelper>();
        private static readonly string ALL_PERMISSIONS_KEY = "PRIVILEGES_";
        //public static bool CheckPermission(HttpContextBase httpContext,  string controller, string action)
        //{
        //    string currentUser = HttpContext.Current.User.Identity.GetUserId();
        //    var _rolesIdentityStore = GlobalContainer.IocContainer.Resolve<IAccessRolesStore>();
        //    bool hasPermission = _rolesIdentityStore.CheckPermission(currentUser, controller, action);

        //    return hasPermission;
        //}

        //public static bool CheckPermission(HttpContextBase httpContext)
        //{
        //    var rd = httpContext.Request.RequestContext.RouteData;
        //    string currentAction = rd.GetRequiredString("action");
        //    string currentController = rd.GetRequiredString("controller");
        //    string currentArea = rd.Values["area"] as string;
        //    string currentUser = HttpContext.Current.User.Identity.GetUserId();

        //    return CheckPermission(httpContext, currentController, currentAction);

        //}

        public static bool CheckPermission(string actionName = "", string controllerName = "")
        {
            var hasPermission = false;
            var currentUser = string.Empty;
            try
            {
                var rd = HttpContext.Current.Request.RequestContext.RouteData;
                if (string.IsNullOrEmpty(actionName))
                    actionName = rd.GetRequiredString("action");

                if (string.IsNullOrEmpty(controllerName))
                    controllerName = rd.GetRequiredString("controller");

                string currentArea = rd.Values["area"] as string;
                currentUser = HttpContext.Current.User.Identity.GetUserId();

                var allPermissions = GetAllPermission();
                if (allPermissions.HasData())
                {
                    hasPermission = allPermissions.Exists(m => string.Equals(m.Action, actionName, StringComparison.CurrentCultureIgnoreCase)
                   && (string.Equals(m.Controller, controllerName, StringComparison.CurrentCultureIgnoreCase)));
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not CheckPermission of user [{0}] because: {1}", currentUser, ex.ToString());
                logger.Error(strError);
            }

            return hasPermission;
        }

        public static List<IdentityPermission> GetAllPermission()
        {
            var currentUser = string.Empty;
            List<IdentityPermission> perList = null;
            try
            {
                var rd = HttpContext.Current.Request.RequestContext.RouteData;
                string currentAction = rd.GetRequiredString("action");
                string currentController = rd.GetRequiredString("controller");
                string currentArea = rd.Values["area"] as string;
                currentUser = HttpContext.Current.User.Identity.GetUserId();

                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                var myKey = ALL_PERMISSIONS_KEY + currentUser;

                //Check from cache first
                cacheProvider.Get(myKey, out perList);

                //Has data from cache
                if (perList == null)
                {
                    var _rolesIdentityStore = GlobalContainer.IocContainer.Resolve<IAccessRolesStore>();

                    perList = _rolesIdentityStore.GetPermissionsByUser(currentUser);
                    if (perList != null && perList.Count > 0)
                    {
                        //Write to cache
                        cacheProvider.Set(myKey, perList);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not GetAllPermission of user [{0}] because: {1}", currentUser, ex.ToString());
                logger.Error(strError);
            }

            return perList;
        }
    }
}