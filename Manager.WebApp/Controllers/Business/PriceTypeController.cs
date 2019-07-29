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

namespace Manager.WebApp.Controllers
{
    public class PriceTypeController : BaseAuthedController
    {
        private readonly IStorePriceType _mainStore;
        private readonly ILog logger = LogProvider.For<PriceTypeController>();

        public PriceTypeController(IStorePriceType mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManagePriceTypeModel model)
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

            var filter = new IdentityPriceType
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
            var createModel = new PriceTypeCreateModel();
            return View(createModel);
        }

        [HttpPost]
        public ActionResult Create(PriceTypeCreateModel model)
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

                logger.Error("Failed for Create PriceType request: " + ex.ToString());

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

                logger.Error("Failed for Edit PriceType request: " + ex.ToString());
            }

            return View(new PriceTypeEditModel());
        }

        [HttpPost]
        public ActionResult Edit(PriceTypeEditModel model)
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
                //    var apiResult = CdnServices.UploadPriceTypeCoverAsync(model).Result;
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

                logger.Error("Failed for Edit PriceType request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + model.Id);
        }

        public ActionResult UpdateLang()
        {
            PriceTypeLangModel model = new PriceTypeLangModel();
            var id = Utils.ConvertToInt32(Request["Id"]);
            var roomcategoryId = Utils.ConvertToInt32(Request["PriceTypeId"]);

            if (roomcategoryId == 0)
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
                model.PriceTypeId = roomcategoryId;

                //Begin db transaction
                var info = _mainStore.GetLangDetail(id);

                if (info != null)
                {
                    model.PriceTypeId = roomcategoryId;
                    model.Id = info.Id;
                    model.LangCode = info.LangCode;
                    model.Name = info.Name;
                    model.Description = info.Description;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show UpdateLang frm request: " + ex.ToString());
            }

            return PartialView("../PriceType/_UpdateLang", model);
        }

        [HttpPost]
        public ActionResult UpdateLang(PriceTypeLangModel model)
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
                var data = new IdentityPriceTypeLang();
                data.PriceTypeId = model.PriceTypeId;
                data.Id = model.Id;
                data.LangCode = model.LangCode;
                data.Name = model.Name;
                data.Description = model.Description;

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

                logger.Error("Failed to get Delete PriceType because: " + ex.ToString());

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

                logger.Error("Failed to get Delete PriceTypeLang because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload()" });
        }

        #region Helpers

        private IdentityPriceType ExtractCreateFormData(PriceTypeCreateModel formData)
        {
            var myIdetity = new IdentityPriceType();
            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            //myIdetity.UrlFriendly = UrlFriendly.ConvertToUrlFriendly(formData.Name);
            myIdetity.Icon = formData.Icon;
            myIdetity.Status = Utils.ConvertToInt32(formData.Status);
            myIdetity.CreatedBy = User.Identity.GetUserId();
            myIdetity.Description = formData.Description;

            return myIdetity;
        }

        private IdentityPriceType ExtractEditFormData(PriceTypeEditModel formData)
        {
            var myIdetity = new IdentityPriceType();
            myIdetity.Id = formData.Id;
            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.Icon = formData.Icon;
            myIdetity.Status = formData.Status;
            myIdetity.Description = formData.Description;

            return myIdetity;
        }

        private PriceTypeEditModel RenderEditModel(IdentityPriceType identity)
        {
            var editModel = new PriceTypeEditModel();

            editModel.Id = identity.Id;
            editModel.Name = identity.Name;
            editModel.Code = identity.Code;
            editModel.Icon = identity.Icon;
            editModel.Status = identity.Status;
            editModel.LangList = identity.LangList;
            editModel.Description = identity.Description;

            return editModel;
        }

        #endregion

    }
}