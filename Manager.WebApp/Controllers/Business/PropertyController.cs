using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Resources;
using Microsoft.AspNet.Identity;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using System.Collections.Generic;

namespace Manager.WebApp.Controllers
{
    public class PropertyController : BaseAuthedController
    {
        private readonly IStoreProperty _mainStore;
        private readonly IStoreGroupProperty _groupStore;
        private readonly ILog logger = LogProvider.For<PropertyController>();

        public PropertyController(IStoreProperty mainStore, IStoreGroupProperty groupStore)
        {
            _mainStore = mainStore;
            _groupStore = groupStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(PropertyModel model)
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

            var filter = new IdentityProperty
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

        //[AccessRoleChecker]
        public ActionResult Create()
        {
            var createModel = new PropertyCreateModel();
            createModel.GroupList = _groupStore.GetAll();
            return View(createModel);
        }

        [HttpPost]
        public ActionResult Create(PropertyCreateModel model)
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
                //Begin db transaction
                var info = ExtractCreateFormData(model);

                newId = _mainStore.Insert(info);

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

                logger.Error("Failed for Create Property request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + newId);
        }

        //[AccessRoleChecker]
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

                if (info == null)
                    return RedirectToErrorPage();

                //Render to view model
                var editModel = RenderEditModel(info);
                editModel.GroupList = _groupStore.GetAll();

                return View(editModel);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Property request: " + ex.ToString());
            }

            return View(new PropertyEditModel());
        }

        [HttpPost]
        public ActionResult Edit(PropertyEditModel model)
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
                //Upload file
                //if (model.Files != null && model.Files[0] != null)
                //{
                //    var apiResult = CdnServices.UploadPropertyCoverAsync(model).Result;
                //    if (apiResult != null)
                //    {
                //        if (apiResult.Code == EnumCommonCode.Success)
                //        {
                //            var imagesList = JsonConvert.DeserializeObject<List<string>>(apiResult.Data.ToString());
                //            if (imagesList != null && imagesList.Count > 0)
                //            {
                //                model.Cover = imagesList[0];
                //            }
                //        }
                //    }
                //}

                //Begin db transaction
                var info = ExtractEditFormData(model);

                var isSuccess = _mainStore.Update(info);

                if (isSuccess)
                {
                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Property request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + model.Id);
        }

        public ActionResult UpdateLang()
        {
            PropertyLangModel model = new PropertyLangModel();
            var id = Utils.ConvertToInt32(Request["Id"]);
            var propertyId = Utils.ConvertToInt32(Request["PropertyId"]);

            if (propertyId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id > 0)
            {
                model.IsUpdate = true;
            }

            try
            {
                model.PropertyId = propertyId;

                //Begin db transaction
                var listLangExists = _mainStore.GetLangDetail(propertyId);
                if (id == 0)
                {
                    var listItem = new List<string>();
                    if (listLangExists != null && listLangExists.Count > 0)
                    {
                        listItem.AddRange(listLangExists.Select(s => s.LangCode));
                    }
                    model.Languages = CommonHelpers.GetListLanguageNotExist(listItem);
                }
                else
                {
                    model.Languages = CommonHelpers.GetListLanguages();

                    var info = listLangExists.FirstOrDefault(s => s.Id == id);
                    if (info != null)
                    {
                        model.PropertyId = propertyId;
                        model.Id = info.Id;
                        model.LangCode = info.LangCode;
                        model.Name = info.Name;
                    }
                }
              
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show UpdateLang frm request: " + ex.ToString());
            }

            return PartialView("../Property/_UpdateLang", model);
        }

        [HttpPost]
        public ActionResult UpdateLang(PropertyLangModel model)
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
                var code = 0;

                //Begin db transaction
                var data = new IdentityPropertyLang();
                data.PropertyId  = model.PropertyId;
                data.Id = model.Id;
                data.Name = model.Name;
                data.LangCode = model.LangCode;

                if (model.Id > 0)
                {
                    //Update
                    _mainStore.UpdateLang(data);
                }
                else
                {
                    //Add new
                    code = _mainStore.InsertLang(data);

                    if (code == EnumCommonCode.Error)
                    {
                        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.LB_DUPLICATE_DATA, clientcallback = " location.reload()" });
                    }
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for UpdateLang request: " + ex.ToString());

                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = NotifSettings.Error_SystemBusy });
            }

            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = " location.reload()" });
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
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Property because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        public ActionResult DeleteLang(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return PartialView("_PopupDelete", id);
        }

        [HttpPost, ActionName("DeleteLang")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLang_Confirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _mainStore.DeleteLang(id);

            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete PropertyLang because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload()" });
        }

        #region Helpers

        private IdentityProperty ExtractCreateFormData(PropertyCreateModel formData)
        {
            var myIdetity = new IdentityProperty();
            myIdetity.Name = formData.Name;
            //myIdetity.UrlFriendly = UrlFriendly.ConvertToUrlFriendly(formData.Name);
            myIdetity.Icon = formData.Icon;
            myIdetity.Status = Utils.ConvertToInt32(formData.Status);
            myIdetity.CreatedBy = User.Identity.GetUserId();
            myIdetity.PropertyCategoryId = formData.PropertyCategoryId;

            return myIdetity;
        }

        private IdentityProperty ExtractEditFormData(PropertyEditModel formData)
        {
            var myIdetity = new IdentityProperty();
            myIdetity.Id = formData.Id;
            myIdetity.Name = formData.Name;
            myIdetity.Icon = formData.Icon;
            myIdetity.Status = formData.Status;
            myIdetity.PropertyCategoryId = formData.PropertyCategoryId;

            return myIdetity;
        }

        private PropertyEditModel RenderEditModel(IdentityProperty identity)
        {
            var editModel = new PropertyEditModel();

            editModel.Id = identity.Id;
            editModel.Name = identity.Name;
            editModel.Icon = identity.Icon;
            editModel.Status = identity.Status;
            editModel.LangList = identity.LangList;
            editModel.PropertyCategoryId = identity.PropertyCategoryId;

            return editModel;
        }

        #endregion

    }
}