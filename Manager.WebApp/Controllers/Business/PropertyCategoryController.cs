using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Models;
using Manager.WebApp.Services;
using Autofac;

namespace Manager.WebApp.Controllers
{
    public class PropertyCategoryController : BaseAuthedController
    {
        private readonly IStorePropertyCategory _mainStore;
        private readonly ILog logger = LogProvider.For<PropertyCategoryController>();

        public PropertyCategoryController(IStorePropertyCategory mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManagePropertyCategoryModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            var filter = new IdentityPropertyCategory
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status
            };

            try
            {
                model.SearchResults = _mainStore.GetByPage(filter, currentPage, SystemSettings.DefaultPageSize);
                if (model.SearchResults != null && model.SearchResults.Count > 0)
                {
                    model.TotalCount = model.SearchResults[0].TotalCount;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        [AccessRoleChecker]
        public ActionResult Create()
        {
            var createModel = new PropertyCategoryCreateModel();
            return View(createModel);
        }

        [HttpPost]
        [AccessRoleChecker]
        public ActionResult Create(PropertyCategoryCreateModel model)
        {
            var newId = 0;
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            try
            {
                //Extract info
                var info = ExtractCreateFormData(model);

                newId = _mainStore.Insert(info);

                //Clear cache
                CachingHelpers.ClearPropertyCategoryCache();

                if (newId > 0)
                {
                    this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                }
                else
                {
                    this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Create PropertyCategory request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + newId);
        }

        [AccessRoleChecker]
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                //Begin db transaction
                var info = _mainStore.GetDetail(id);

                if(info == null)
                    return RedirectToErrorPage();

                //Render to view model
                var editModel = RenderEditModel(info);

                return View(editModel);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit PropertyCategory request: " + ex.ToString());
            }

            return View(new PropertyCategoryEditModel());
        }

        [HttpPost]
        public ActionResult Edit(PropertyCategoryEditModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            try
            {
                //Extract data
                var info = ExtractEditFormData(model);

                var isSuccess = _mainStore.Update(info);

                //Clear cache
                CachingHelpers.ClearPropertyCategoryCache();

                if (isSuccess)
                {
                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit PropertyCategory request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + model.Id);
        }

        //Show popup confirm delete        
        //[AccessRoleChecker]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return PartialView("_PopupDelete", id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Confirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _mainStore.Delete(id);

                //Clear cache
                CachingHelpers.ClearPropertyCategoryCache();
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete PropertyCategory because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        public ActionResult UpdateProperty()
        {
            PropertyEditModel model = new PropertyEditModel();
            var id = Utils.ConvertToInt32(Request["Id"]);
            var propertyCategoryId = Utils.ConvertToInt32(Request["PropertyCategoryId"]);

            if (propertyCategoryId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id > 0)
            {
                model.IsUpdate = true;
            }

            try
            {
                model.PropertyCategoryId = propertyCategoryId;                
                if(id > 0)
                {
                    var storeProperty = GlobalContainer.IocContainer.Resolve<IStoreProperty>();

                    //Begin db transaction
                    var info = storeProperty.GetById(id);

                    if (info != null)
                    {                        
                        model.Id = info.Id;
                        model.Code = info.Code;
                        model.Name = info.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show UpdateProperty form request: " + ex.ToString());
            }

            return PartialView("../PropertyCategory/_UpdateProperty", model);
        }

        [HttpPost]
        public ActionResult UpdateProperty(PropertyEditModel model)
        {
            var msg = ManagerResource.LB_OPERATION_SUCCESS;
            var isSuccess = false;

            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);

                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = messages });
            }

            try
            {
                //Begin db transaction
                var data = new IdentityProperty();
                data.PropertyCategoryId = model.PropertyCategoryId;
                data.Id = model.Id;
                data.Code = model.Code;
                data.Name = model.Name;
                data.Status = (int)EnumStatus.Activated;

                var storeProperty = GlobalContainer.IocContainer.Resolve<IStoreProperty>();

                if (model.Id > 0)
                {
                    //Update
                    storeProperty.Update(data);

                    //Clear cache
                    CachingHelpers.ClearPropertyCategoryCache();
                }
                else
                {
                    //Add new
                    var newId = storeProperty.Insert(data);

                    if (newId > 0)
                    {
                        isSuccess = true;

                        //Clear cache
                        CachingHelpers.ClearPropertyCategoryCache();

                        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.LB_OPERATION_SUCCESS, clientcallback = " location.reload()" });
                    }
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for UpdateProperty request: " + ex.ToString());

                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = NotifSettings.Error_SystemBusy });
            }

            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = " location.reload()" });
        }

        public ActionResult DeleteProperty(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return PartialView("_PopupDelete", id);
        }

        [HttpPost, ActionName("DeleteProperty")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProperty_Confirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var storeProperty = GlobalContainer.IocContainer.Resolve<IStoreProperty>();
                storeProperty.Delete(id);

            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to DeleteProperty because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload()" });
        }

        #region Helpers

        private IdentityPropertyCategory ExtractCreateFormData(PropertyCategoryCreateModel formData)
        {
            var myIdetity = new IdentityPropertyCategory();
            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private IdentityPropertyCategory ExtractEditFormData(PropertyCategoryEditModel formData)
        {
            var myIdetity = new IdentityPropertyCategory();
            myIdetity.Id = formData.Id;

            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private PropertyCategoryEditModel RenderEditModel(IdentityPropertyCategory identity)
        {
            var editModel = new PropertyCategoryEditModel();

            editModel.Id = identity.Id;
            editModel.Name = identity.Name;
            editModel.Code = identity.Code;
            editModel.Status = identity.Status;
            editModel.Properties = identity.Properties;

            return editModel;
        }

        #endregion

    }
}