using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Microsoft.AspNet.Identity;
using Manager.WebApp.Models;
using Manager.SharedLib.Caching.Providers;
using MsSql.AspNet.Identity;
using Manager.WebApp.Caching;

namespace Manager.WebApp.Helpers
{
    public class MenuHelper
    {
        const string CST_ALL_MENU_ITEMS_PREFIX = "MNU_ALL_MENU_ITEMS";
        const string CST_ALL_PERMISSIONS_PREFIX = "PRIVILEGES_";
        const string CST_ALL_MENU_ITEMS_PATTERN = "MNU_ALL_MENU_ITEMS.{0}";
        public static void ClearAllMenuCache()
        {
            var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

            //Clear all menu
            cacheProvider.ClearAll(CST_ALL_MENU_ITEMS_PREFIX);

            //Clear all permissions
            cacheProvider.ClearAll(CST_ALL_PERMISSIONS_PREFIX);
        }

        public static void ClearUserMenuCache(string currentUser)
        {
            string menuCacheKey = string.Format(CST_ALL_MENU_ITEMS_PATTERN, currentUser);
            var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
            cacheProvider.Clear(menuCacheKey);
            //cacheProvider.ClearAll(CST_ALL_MENU_ITEMS_PREFIX);
        }

        public static IEnumerable<IdentityMenu> GetAdminNavigationMenuItems_Oldversions()
        {
            const string CST_ALL_MENU_ITEMS_KEY = "MNU_ALL_MENU_ITEMS";

            if (HttpContext.Current != null)
            {
                //Check in request context storage
                var contextObject = HttpContext.Current.Items[CST_ALL_MENU_ITEMS_KEY];
                if (contextObject != null && contextObject is IEnumerable<IdentityMenu>)
                {
                    return (IEnumerable<IdentityMenu>)contextObject;
                }
            }
            string currentUser = HttpContext.Current.User.Identity.GetUserId();
            var _listMenu = GetMenuItemsFromDatabase(currentUser);

            //Store in request context storage
            var allMenuITems = _listMenu.OrderBy(m => m.SortOrder);
            HttpContext.Current.Items.Add(CST_ALL_MENU_ITEMS_KEY, allMenuITems);

            return allMenuITems;
        }

        public static IEnumerable<IdentityMenu> GetAdminNavigationMenuItems()
        {

            string currentUser = HttpContext.Current.User.Identity.GetUserId();
            var cacheKeyByUser = string.Format(CST_ALL_MENU_ITEMS_PATTERN, currentUser);
            var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

            IEnumerable<IdentityMenu> result;
            if (!cacheProvider.Get(cacheKeyByUser, out result))
            {
                var menuItems = GetMenuItemsFromDatabase(currentUser);
                if (menuItems != null && menuItems.Count > 0)
                {
                    result = menuItems.OrderBy(m => m.SortOrder);
                    cacheProvider.Set(cacheKeyByUser, result);
                }
            }
            return result;
        }


        private static List<IdentityMenu> GetMenuItemsFromDatabase(string currentUser)
        {

            List<IdentityMenu> _listMenu = null;

            var _rolesIdentityStore = GlobalContainer.IocContainer.Resolve<IAccessRolesStore>();

            try
            {
                if (currentUser != null)
                {
                    _listMenu = _rolesIdentityStore.GetRootMenuByUserId(currentUser);
                    if (_listMenu != null && _listMenu.Count > 0)
                    {
                        foreach (var parentItem in _listMenu)
                        {
                            MenuItemFindAllChildren(parentItem, currentUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return _listMenu;
        }


        public static void MenuItemFindAllChildren(IdentityMenu parentItem, string UserId)
        {
            var _rolesIdentityStore = GlobalContainer.IocContainer.Resolve<IAccessRolesStore>();
            try
            {
                List<IdentityMenu> childMenus = _rolesIdentityStore.GetChildMenuByUserId(UserId, parentItem.Id).ToList();
               
                parentItem.SubMenu = childMenus;
                if (childMenus != null && childMenus.Count > 0)
                {
                    foreach (IdentityMenu childInfo in childMenus)
                    {
                        MenuItemFindAllChildren(childInfo, UserId);
                    }
                }
            }
            catch
            {
            }
        }     
        
        public static List<IdentityMenu> GetCurrentListMenuByLang(List<IdentityMenu> allMenus)
        {
            try
            {
                var currentLangCode = UserCookieManager.GetCurrentLanguageOrDefault();
                if (allMenus.HasData())
                {
                    foreach (var item in allMenus)
                    {
                        var langItem = item.LangList.Where(x => x.MenuId == item.Id && x.LangCode == currentLangCode).FirstOrDefault();
                        if (langItem != null)
                            item.CurrentTitleLang = langItem.Title;
                        else
                            item.CurrentTitleLang = item.Title;

                        if (item.SubMenu != null && item.SubMenu.Count() > 0)
                            item.SubMenu = GetCurrentListMenuByLang(item.SubMenu);
                    }
                }
            }
            catch
            {
            }

            return allMenus;
        }

        //public static MenuItem GetCurrentMenuItem()
        //{

        //    const string CST_CURRENT_MENU_ITEM_KEY = "MNU_CURRENT_MENU_ITEM";

        //    if (HttpContext.Current == null)
        //        return null;

        //    //Check in request context storage
        //    var contextObject = HttpContext.Current.Items[CST_CURRENT_MENU_ITEM_KEY];
        //    if (contextObject!=null  && contextObject is MenuItem)
        //    {
        //        return (MenuItem)contextObject;
        //    }

        //    string controllerName = null;
        //    string actionName = null;

        //    var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
        //    if (routeValues != null)
        //    {
        //        if (routeValues.ContainsKey("action"))
        //        {
        //            actionName = routeValues["action"].ToString();
        //        }
        //        if (routeValues.ContainsKey("controller"))
        //        {
        //            controllerName = routeValues["controller"].ToString();
        //        }
        //    }

        //    var menuItems = GetAdminNavigationMenuItems();

        //    var currentMenuItem = CheckCurrentMenuItemInCollections(menuItems, actionName, controllerName);

        //    if (currentMenuItem == null)
        //        return null;

        //    //Store in request context storage
        //    HttpContext.Current.Items.Add(CST_CURRENT_MENU_ITEM_KEY, currentMenuItem);
        //    return currentMenuItem;
        //}

        private static MenuItem CheckCurrentMenuItemInCollections(IEnumerable<MenuItem> menuItems, string actionName, string controllerName)
        {
            MenuItem result = null;
            foreach (var mi in menuItems)
            {
                if (CheckCurrentAction(mi, actionName, controllerName))
                {
                    //Check current item
                    result = mi;
                    break;
                }

                if (mi.SubMenu != null && mi.SubMenu.Any())
                {
                    //Find in subMenu
                    foreach (var smi in mi.SubMenu)
                    {
                        var tempItem = CheckCurrentMenuItemInCollections(mi.SubMenu, actionName, controllerName);
                        if (tempItem != null)
                        {
                            result = tempItem;
                            break;
                        }
                    }

                    //Found in subMenu
                    if (result != null)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        private static bool CheckCurrentAction(MenuItem mi, string actionName, string controllerName)
        {
            if (
                !string.IsNullOrEmpty(mi.Action)
                && !string.IsNullOrEmpty(mi.Controller)
                && !string.IsNullOrEmpty(actionName)
                && !string.IsNullOrEmpty(controllerName)
                && mi.Action.Equals(actionName, StringComparison.InvariantCultureIgnoreCase)
                && mi.Controller.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase)
                )
            {
                return true;
            }

            return false;
        }

        public static bool CheckCurrentAction(MenuItem mi)
        {
            if (HttpContext.Current == null)
                return false;

            string controllerName = null;
            string actionName = null;

            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            if (routeValues != null)
            {
                if (routeValues.ContainsKey("action"))
                {
                    actionName = routeValues["action"].ToString();
                }
                if (routeValues.ContainsKey("controller"))
                {
                    controllerName = routeValues["controller"].ToString();
                }
            }

            return CheckCurrentAction(mi, actionName, controllerName);
        }

        public static bool CheckCurrentGroup(MenuItem gmi)
        {
            if (CheckCurrentAction(gmi))
            {
                return true;
            }

            if (gmi.SubMenu != null && gmi.SubMenu.Any())
            {
                foreach (var smi in gmi.SubMenu)
                {
                    if (CheckCurrentGroup(smi))
                        return true;
                }
            }

            return false;
        }

        #region Customize
        public static IdentityMenu GetCurrentMenuItemCustom()
        {

            const string CST_CURRENT_MENU_ITEM_KEY = "MNU_CURRENT_MENU_ITEM";

            if (HttpContext.Current == null)
                return null;

            //Check in request context storage
            var contextObject = HttpContext.Current.Items[CST_CURRENT_MENU_ITEM_KEY];
            if (contextObject != null && contextObject is IdentityMenu)
            {
                return (IdentityMenu)contextObject;
            }

            string controllerName = null;
            string actionName = null;

            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            if (routeValues != null)
            {
                if (routeValues.ContainsKey("action"))
                {
                    actionName = routeValues["action"].ToString();
                }
                if (routeValues.ContainsKey("controller"))
                {
                    controllerName = routeValues["controller"].ToString();
                }
            }

            var menuItems = GetAdminNavigationMenuItems();

            var currentMenuItem = CheckCurrentMenuItemInCollectionsCustom(menuItems, actionName, controllerName);

            if (currentMenuItem == null)
                return null;

            //Store in request context storage
            HttpContext.Current.Items.Add(CST_CURRENT_MENU_ITEM_KEY, currentMenuItem);
            return currentMenuItem;
        }

        private static IdentityMenu CheckCurrentMenuItemInCollectionsCustom(IEnumerable<IdentityMenu> menuItems, string actionName, string controllerName)
        {
            IdentityMenu result = null;
            if (menuItems != null && menuItems.Count() > 0)
            {
                foreach (var mi in menuItems)
                {
                    if (CheckCurrentActionCustom(mi, actionName, controllerName))
                    {
                        //Check current item
                        result = mi;
                        break;
                    }

                    if (mi.SubMenu != null && mi.SubMenu.Any())
                    {
                        //Find in subMenu
                        foreach (var smi in mi.SubMenu)
                        {
                            var tempItem = CheckCurrentMenuItemInCollectionsCustom(mi.SubMenu, actionName, controllerName);
                            if (tempItem != null)
                            {
                                result = tempItem;
                                break;
                            }
                        }

                        //Found in subMenu
                        if (result != null)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }


        private static bool CheckCurrentActionCustom(IdentityMenu mi, string actionName, string controllerName)
        {
            if (
                !string.IsNullOrEmpty(mi.Action)
                && !string.IsNullOrEmpty(mi.Controller)
                && !string.IsNullOrEmpty(actionName)
                && !string.IsNullOrEmpty(controllerName)
                && mi.Action.Equals(actionName, StringComparison.InvariantCultureIgnoreCase)
                && mi.Controller.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase)
                )
            {
                return true;
            }

            return false;
        }

        public static bool CheckCurrentActionCustom(IdentityMenu mi)
        {
            if (HttpContext.Current == null)
                return false;

            string controllerName = null;
            string actionName = null;

            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            if (routeValues != null)
            {
                if (routeValues.ContainsKey("action"))
                {
                    actionName = routeValues["action"].ToString();
                }
                if (routeValues.ContainsKey("controller"))
                {
                    controllerName = routeValues["controller"].ToString();
                }
            }

            return CheckCurrentActionCustom(mi, actionName, controllerName);
        }

        public static bool CheckCurrentGroupCustom(IdentityMenu gmi)
        {
            if (CheckCurrentActionCustom(gmi))
            {
                return true;
            }

            if (gmi.SubMenu != null && gmi.SubMenu.Any())
            {
                foreach (var smi in gmi.SubMenu)
                {
                    if (CheckCurrentGroupCustom(smi))
                        return true;
                }
            }

            return false;
        }

        public static IdentityMenu GetMenuItemByIDCustom(int menuId)
        {
            var menuItems = GetAdminNavigationMenuItems();
            var stack = new Stack<IdentityMenu>();
            foreach (var mi in menuItems)
            {
                stack.Push(mi);
            }

            while (stack.Any())
            {
                //Visit node
                var next = stack.Pop();
                if (next.Id == menuId)
                {
                    return next;
                }

                //Add child nodes
                if (next.SubMenu != null)
                {
                    foreach (var smi in next.SubMenu)
                    {
                        stack.Push(smi);
                    }
                }
            }

            return null;
        }

        #endregion

        //public static MenuItem GetMenuItemByID(int menuId)
        //{
        //    var menuItems = GetAdminNavigationMenuItems();
        //    var stack = new Stack<MenuItem>();
        //    foreach(var mi in menuItems)
        //    {
        //        stack.Push(mi);
        //    }

        //    while(stack.Any())
        //    {
        //        //Visit node
        //        var next = stack.Pop();
        //        if (next.Id == menuId)
        //        {
        //            return next;
        //        }

        //        //Add child nodes
        //        if (next.SubMenu != null)
        //        {
        //            foreach (var smi in next.SubMenu)
        //            {
        //                stack.Push(smi);
        //            }
        //        }
        //    }

        //    return null;
        //}       
    }
}