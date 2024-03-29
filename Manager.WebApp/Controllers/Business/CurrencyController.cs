﻿using System;
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
    public class CurrencyController : BaseAuthedController
    {
        private readonly IStoreCurrency _mainStore;
        private readonly ILog logger = LogProvider.For<CurrencyController>();

        public CurrencyController(IStoreCurrency mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManageCurrencyModel model)
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

            var filter = new IdentityCurrency
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
            var createModel = new CurrencyCreateModel();
            return View(createModel);
        }

        [HttpPost]
        [AccessRoleChecker]
        public ActionResult Create(CurrencyCreateModel model)
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
                CachingHelpers.ClearCurrencyCache();

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

                logger.Error("Failed for Create Currency request: " + ex.ToString());

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

                logger.Error("Failed for Edit Currency request: " + ex.ToString());
            }

            return View(new CurrencyEditModel());
        }

        [HttpPost]
        public ActionResult Edit(CurrencyEditModel model)
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
                CachingHelpers.ClearCurrencyCache();

                if (isSuccess)
                {
                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Currency request: " + ex.ToString());

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
                CachingHelpers.ClearCurrencyCache();
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Currency because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        #region Helpers

        private IdentityCurrency ExtractCreateFormData(CurrencyCreateModel formData)
        {
            var myIdetity = new IdentityCurrency();
            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private IdentityCurrency ExtractEditFormData(CurrencyEditModel formData)
        {
            var myIdetity = new IdentityCurrency();
            myIdetity.Id = formData.Id;

            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.Status = formData.Status;

            return myIdetity;
        }

        private CurrencyEditModel RenderEditModel(IdentityCurrency identity)
        {
            var editModel = new CurrencyEditModel();

            editModel.Id = identity.Id;
            editModel.Name = identity.Name;
            editModel.Code = identity.Code;
            editModel.Status = identity.Status;

            return editModel;
        }

        #endregion

    }
}