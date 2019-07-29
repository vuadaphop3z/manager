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

namespace Manager.WebApp.Controllers
{
    public class HTDefaultSettingController : BaseAuthedController
    {
        private readonly IStoreHTDefaultSetting _mainStore;
        private readonly ILog logger = LogProvider.For<HTDefaultSettingController>();

        public HTDefaultSettingController(IStoreHTDefaultSetting mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManageHTDefaultSettingModel model)
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

            var filter = new IdentityHTDefaultSetting
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
            var createModel = new HTDefaultSettingCreateModel();
            return View(createModel);
        }

        [HttpPost]
        [AccessRoleChecker]
        public ActionResult Create(HTDefaultSettingCreateModel model)
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

                ////Clear cache
                //CachingHelpers.ClearHTDefaultSettingCache();
                if (newId > 0)
                {
                    this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                }
                else
                {
                    this.AddNotification(ManagerResource.ERROR_ENUMVALUE_EXISTED, NotificationType.ERROR);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Create HTDefaultSetting request: " + ex.ToString());

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
                var info = _mainStore.GetById(id);

                if (info == null)
                    return RedirectToErrorPage();

                //Render to view model
                var editModel = RenderEditModel(info);

                return View(editModel);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit HTDefaultSetting request: " + ex.ToString());
            }

            return View(new HTDefaultSettingEditModel());
        }

        [HttpPost]
        public ActionResult Edit(HTDefaultSettingEditModel model)
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

                var returnNum = _mainStore.Update(info);

                ////Clear cache
                //CachingHelpers.ClearHTDefaultSettingCache();

                if (returnNum < 0)
                {
                    this.AddNotification(ManagerResource.ERROR_ENUMVALUE_EXISTED, NotificationType.ERROR);
                }
                else
                {
                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit HTDefaultSetting request: " + ex.ToString());

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

                ////Clear cache
                //CachingHelpers.ClearHTDefaultSettingCache();
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete HTDefaultSetting because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        #region Helpers

        private IdentityHTDefaultSetting ExtractCreateFormData(HTDefaultSettingCreateModel formData)
        {
            var myIdetity = new IdentityHTDefaultSetting();
            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.EnumValue = formData.EnumValue;
            myIdetity.MaxLength = formData.MaxLength;
            myIdetity.StartPosition = formData.StartPosition;
            myIdetity.NumberOfCharacters = formData.NumberOfCharacters;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private IdentityHTDefaultSetting ExtractEditFormData(HTDefaultSettingEditModel formData)
        {
            var myIdetity = new IdentityHTDefaultSetting();
            myIdetity.Id = formData.Id;

            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.EnumValue = formData.EnumValue;
            myIdetity.MaxLength = formData.MaxLength;
            myIdetity.StartPosition = formData.StartPosition;
            myIdetity.NumberOfCharacters = formData.NumberOfCharacters;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private HTDefaultSettingEditModel RenderEditModel(IdentityHTDefaultSetting identity)
        {
            var editModel = new HTDefaultSettingEditModel();

            editModel.Id = identity.Id;
            editModel.Name = identity.Name;
            editModel.Code = identity.Code;
            editModel.EnumValue = identity.EnumValue;
            editModel.MaxLength = identity.MaxLength;
            editModel.StartPosition = identity.StartPosition;
            editModel.NumberOfCharacters = identity.NumberOfCharacters;
            editModel.Status = identity.Status;

            return editModel;
        }

        #endregion

    }
}