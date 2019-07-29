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
    public class GroupPropertyController : BaseAuthedController
    {
        private readonly IStoreGroupProperty _mainStore;
        private readonly ILog logger = LogProvider.For<GroupPropertyController>();

        public GroupPropertyController(IStoreGroupProperty mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManageGroupPropertyModel model)
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

            var filter = new IdentityGroupProperty
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
            var createModel = new GroupPropertyCreateModel();
            return View(createModel);
        }

        [HttpPost]
        public ActionResult Create(GroupPropertyCreateModel model)
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

                logger.Error("Failed for Create GroupProperty request: " + ex.ToString());

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

                //Render to view model
                var editModel = RenderEditModel(info);

                return View(editModel);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit GroupProperty request: " + ex.ToString());
            }

            return View(new GroupPropertyEditModel());
        }

        [HttpPost]
        public ActionResult Edit(GroupPropertyEditModel model)
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
                //    var apiResult = CdnServices.UploadGroupPropertyCoverAsync(model).Result;
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

                logger.Error("Failed for Edit GroupProperty request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + model.Id);
        }

        public ActionResult UpdateLang()
        {
            GroupPropertyLangModel model = new GroupPropertyLangModel();
            var id = Utils.ConvertToInt32(Request["Id"]);
            var groupId = Utils.ConvertToInt32(Request["GroupId"]);

            if (groupId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id > 0)
            {
                model.IsUpdate = true;
            }

            try
            {
                model.GroupId = groupId;

                //Begin db transaction
                var listLangExists = _mainStore.GetLangDetail(groupId);
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
                        model.GroupId = groupId;
                        model.Id = info.Id;
                        model.LangCode = info.LangCode;
                        model.Name = info.GroupName;
                        model.Description = info.Description;
                    }
                }
              
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show UpdateLang frm request: " + ex.ToString());
            }

            return PartialView("../GroupProperty/_UpdateLang", model);
        }

        [HttpPost]
        public ActionResult UpdateLang(GroupPropertyLangModel model)
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
                var data = new IdentityGroupPropertyLang();
                data.GroupId = model.GroupId;
                data.Id = model.Id;
                data.Description = model.Description;
                data.GroupName = model.Name;
                data.LangCode = model.LangCode;

                if (model.Id > 0)
                {
                    //Update
                    _mainStore.UpdateLang(data);
                }
                else
                {
                    //Add new
                    code = _mainStore.AddNewLang(data);

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

                logger.Error("Failed to get Delete GroupProperty because: " + ex.ToString());

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

                logger.Error("Failed to get Delete GroupPropertyLang because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload()" });
        }

        #region Helpers

        private IdentityGroupProperty ExtractCreateFormData(GroupPropertyCreateModel formData)
        {
            var myIdetity = new IdentityGroupProperty();
            myIdetity.Name = formData.Name;
            myIdetity.Description = formData.Description;
            //myIdetity.UrlFriendly = UrlFriendly.ConvertToUrlFriendly(formData.Name);
            myIdetity.Icon = formData.Icon;
            myIdetity.Status = Utils.ConvertToInt32(formData.Status);
            myIdetity.CreatedBy = User.Identity.GetUserId();

            return myIdetity;
        }

        private IdentityGroupProperty ExtractEditFormData(GroupPropertyEditModel formData)
        {
            var myIdetity = new IdentityGroupProperty();
            myIdetity.Id = formData.Id;
            myIdetity.Name = formData.Name;
            myIdetity.Description = formData.Description;
            myIdetity.Icon = formData.Icon;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private GroupPropertyEditModel RenderEditModel(IdentityGroupProperty identity)
        {
            var editModel = new GroupPropertyEditModel();

            editModel.Id = identity.Id;
            editModel.Name = identity.Name;
            editModel.Description = identity.Description;
            editModel.Icon = identity.Icon;
            editModel.Status = identity.Status;
            editModel.LangList = identity.LangList;

            return editModel;
        }

        #endregion

    }
}