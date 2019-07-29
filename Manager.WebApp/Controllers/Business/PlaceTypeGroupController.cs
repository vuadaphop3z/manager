using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Microsoft.AspNet.Identity;

namespace Manager.WebApp.Controllers
{
    public class PlaceTypeGroupController : BaseAuthedController
    {
        private readonly IStorePlaceTypeGroup _mainStore;
        private readonly ILog logger = LogProvider.For<PlaceTypeGroupController>();

        public PlaceTypeGroupController(IStorePlaceTypeGroup mainStore)
        {
            _mainStore = mainStore;
        }

        //[AccessRoleChecker]
        public ActionResult Index(ManagePlaceTypeGroupModel model)
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

            var filter = new IdentityPlaceTypeGroup
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status
            };

            try
            {
                model.SearchResults = _mainStore.GetAll(filter, currentPage, SystemSettings.DefaultPageSize);
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
            var createModel = new PlaceTypeGroupCreateModel();
            return View(createModel);
        }

        [HttpPost]
        public ActionResult Create(PlaceTypeGroupCreateModel model)
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

                logger.Error("Failed for Create PlaceTypeGroup request: " + ex.ToString());

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

                logger.Error("Failed for Edit PlaceTypeGroup request: " + ex.ToString());
            }

            return View(new PlaceTypeGroupEditModel());
        }

        [HttpPost]
        public ActionResult Edit(PlaceTypeGroupEditModel model)
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
                //    var apiResult = CdnServices.UploadPlaceTypeGroupCoverAsync(model).Result;
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

                logger.Error("Failed for Edit PlaceTypeGroup request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + model.Id);
        }

        public ActionResult UpdateLang()
        {
            PlaceTypeGroupLangModel model = new PlaceTypeGroupLangModel();
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
                model.Languages = CommonHelpers.GetListLanguages();
                model.GroupId = groupId;

                //Begin db transaction
                var info = _mainStore.GetLangDetail(id);

                if (info != null)
                {
                    model.GroupId = groupId;
                    model.Id = info.Id;
                    model.LangCode = info.LangCode;
                    model.Name = info.GroupName;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show UpdateLang frm request: " + ex.ToString());
            }

            return PartialView("../PlaceTypeGroup/_UpdateLang", model);
        }

        [HttpPost]
        public ActionResult UpdateLang(PlaceTypeGroupLangModel model)
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
                var data = new IdentityPlaceTypeGroupLang();
                data.GroupId = model.GroupId;
                data.Id = model.Id;
                data.LangCode = model.LangCode;
                data.GroupName = model.Name;

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

                logger.Error("Failed to get Delete PlaceTypeGroup because: " + ex.ToString());

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

                logger.Error("Failed to get Delete PlaceTypeGroupLang because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload()" });
        }

        #region Helpers

        private IdentityPlaceTypeGroup ExtractCreateFormData(PlaceTypeGroupCreateModel formData)
        {
            var myIdetity = new IdentityPlaceTypeGroup();
            myIdetity.GroupName = formData.Name;
            myIdetity.GroupCode = formData.Code;
            //myIdetity.UrlFriendly = UrlFriendly.ConvertToUrlFriendly(formData.Name);
            myIdetity.Icon = formData.Icon;
            myIdetity.FilterOnMap = Utils.ConvertToBoolean(formData.FilterOnMap);
            myIdetity.SortOrder = formData.SortOrder;
            myIdetity.Status = Utils.ConvertToInt32(formData.Status);
            myIdetity.CreatedBy = User.Identity.GetUserId();

            return myIdetity;
        }

        private IdentityPlaceTypeGroup ExtractEditFormData(PlaceTypeGroupEditModel formData)
        {
            var myIdetity = new IdentityPlaceTypeGroup();
            myIdetity.Id = formData.Id;
            myIdetity.GroupName = formData.Name;
            myIdetity.GroupCode = formData.Code;
            myIdetity.Icon = formData.Icon;
            myIdetity.FilterOnMap = (formData.FilterOnMap == 1) ? true : false;
            myIdetity.SortOrder = formData.SortOrder;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private PlaceTypeGroupEditModel RenderEditModel(IdentityPlaceTypeGroup identity)
        {
            var editModel = new PlaceTypeGroupEditModel();

            editModel.Id = identity.Id;
            editModel.Name = identity.GroupName;
            editModel.Code = identity.GroupCode;
            editModel.FilterOnMap = (identity.FilterOnMap) ? 1 : 0;
            editModel.Icon = identity.Icon;
            editModel.SortOrder = identity.SortOrder;
            editModel.Status = identity.Status;
            editModel.LangList = identity.LangList;

            return editModel;
        }

        #endregion

    }
}