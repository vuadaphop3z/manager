using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity;
using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using System.Net;
using Manager.SharedLib.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;
using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.MsSqlStores;
using Autofac;
using MsSql.AspNet.Identity.Entities;
using Manager.WebApp.Resources;
using System.Linq;

namespace Manager.WebApp.Controllers
{
    public class NavigationController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<NavigationController>();
        private readonly IStoreNavigation _mainStore;

        public NavigationController()
        {
            _mainStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();

            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [AccessRoleChecker]
        public ActionResult Index(NavigationViewModels model)
        {
            try
            {
                model.AllNavigations = _mainStore.GetList();
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat(string.Format("Could not display Index page because: {0}", ex.ToString()));
            }

            return View(model);
        }

        public ActionResult Create(int parentid)
        {
            NavigationViewModels model = new NavigationViewModels();
            model.ParentId = parentid;
            model.Active = true;
            model.Visible = true;
            try
            {
                
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat("Failed to display Create page because: {0}", ex.ToString());
                PartialView("_Create", model);
            }
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]        
        public ActionResult Create(NavigationViewModels model)
        {           
            try
            {
                if (!ModelState.IsValid)
                {
                    if (!ModelState.IsValid)
                    {
                        string messages = string.Join("; ", ModelState.Values
                                               .SelectMany(x => x.Errors)
                                               .Select(x => x.ErrorMessage + x.Exception));
                        this.AddNotification(messages, NotificationType.ERROR);
                        return View(model);
                    }
                }

                var identity = new IdentityNavigation();
                identity.ParentId = model.ParentId;
                identity.Active = (model.Active) ? 1 : 0;
                //identity.Controller = model.Controller;
                //identity.Action = model.Action;
                identity.CssClass = model.CssClass;
                identity.IconCss = model.IconCss;
                identity.SortOrder = model.SortOrder;
                identity.Name = model.Name;
                identity.AbsoluteUri = model.AbsoluteUri;
                identity.Visible = (model.Visible) ? 1 : 0;
                identity.Title = model.Title;

                _mainStore.Insert(identity);

                this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Failed to create new because: {0}",ex.ToString());
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            NavigationViewModels model = new NavigationViewModels();
            try
            {
               var navigation = _mainStore.GetById(id);

                if(navigation != null)
                {
                    model.Id = id;
                    model.ParentId = navigation.ParentId;
                    model.Active = (navigation.Active == 1) ? true : false;
                    //model.Controller = navigation.Controller;
                    //model.Action = navigation.Action;
                    model.CssClass = navigation.CssClass;
                    model.IconCss = navigation.IconCss;
                    model.SortOrder = navigation.SortOrder;
                    model.Name = navigation.Name;
                    model.AbsoluteUri = navigation.AbsoluteUri;
                    model.Visible = (navigation.Visible == 1) ? true : false;
                    model.Title = navigation.Title;
                }             
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat("Failed to display Edit page because: {0}", ex.ToString());
                PartialView("_Update", model);
            }
            return PartialView("_Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(NavigationViewModels model)
        {           
            try
            {
                if (!ModelState.IsValid)
                {
                    if (!ModelState.IsValid)
                    {
                        string messages = string.Join("; ", ModelState.Values
                                               .SelectMany(x => x.Errors)
                                               .Select(x => x.ErrorMessage + x.Exception));
                        this.AddNotification(messages, NotificationType.ERROR);
                        return View(model);
                    }
                }

                var identity = new IdentityNavigation();
                identity.Id = model.Id;
                identity.ParentId = model.ParentId;
                identity.Active = (model.Active) ? 1 : 0;
                //identity.Controller = model.Controller;
                //identity.Action = model.Action;
                identity.CssClass = model.CssClass;
                identity.IconCss = model.IconCss;
                identity.SortOrder = model.SortOrder;
                identity.Name = model.Name;
                identity.AbsoluteUri = model.AbsoluteUri;
                identity.Visible = (model.Visible) ? 1 : 0;
                identity.Title = model.Title;

                _mainStore.Update(identity);

                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);               
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat("Failed to update because: {0}", ex.ToString());                
            }

            return RedirectToAction("Index");
        }

        private void ApplyNewSorting(List<SortingElement> sortList, int parentId = 0)
        {
            if (sortList.HasData())
            {
                var beginOrder = 0;
                foreach (var item in sortList)
                {
                    beginOrder++;
                    item.SortOrder = beginOrder;
                    item.ParentId = parentId;

                    if (item.children.HasData())
                    {
                        ApplyNewSorting(item.children, item.id);
                    }
                }
            }
        }

        public ActionResult UpdateSorting(string data)
        {            
            try
            {
                var sortList = new List<SortingElement>();

                if(!string.IsNullOrEmpty(data))
                    sortList = JsonConvert.DeserializeObject<List<SortingElement>>(data);

                if (sortList != null)
                    ApplyNewSorting(sortList);

                //Update in DB
                if (sortList != null)
                    _mainStore.UpdateSorting(sortList);

                //Clear Cache
                //NavigationHelper.ClearAllNavigationCache();
                
                return Json(new AjaxResponseModel { Success = true, Message = ManagerResource.LB_UPDATE_SUCCESS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat("Failed to UpdateSorting because: {0}" + ex.ToString());
                return Json(new AjaxResponseModel { Success = false, Message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT }, JsonRequestBehavior.AllowGet);
            }
        }

        //Show popup confirm delete        
        public ActionResult PopupDelete(string id)
        {
            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NavigationViewModels record = new NavigationViewModels();
            record.Id = Convert.ToInt32(id);

            return PartialView("_DeleteInfo", record);
        }

        [HttpPost, ActionName("PopupDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int Id = Convert.ToInt32(id);

                _mainStore.Delete(Id);

                return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                logger.ErrorFormat("Failed to delete because: {0}", ex.ToString());
                return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT }, JsonRequestBehavior.AllowGet);
            }            
        }       

        #region Helpers



        #endregion
    }
}