using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;

namespace Manager.WebApp.Controllers
{
    public class ProviderController : BaseAuthedController
    {
        private readonly IStoreProvider _mainStore;
        private readonly ILog logger = LogProvider.For<ProviderController>();

        public ProviderController(IStoreProvider mainStore)
        {
            _mainStore = mainStore;
        }

        // GET: Providers
        [AccessRoleChecker]
        public ActionResult Index(ManageProviderModel model)
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

            var filter = new IdentityProvider
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

                logger.Error("Failed to display Index page because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        //[AccessRoleChecker]
        public ActionResult Create()
        {
            var createModel = new ProviderCreateModel();

            return View(createModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProviderCreateModel model)
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
                    //Clear cache
                    CachingHelpers.ClearProviderCache();

                    this.AddNotification(NotifSettings.Success_Created, NotificationType.SUCCESS);
                }
                else
                {
                    this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Create request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Edit/" + newId);
        }

        //[AccessRoleChecker]
        public ActionResult Edit(int id)
        {
            if (id == 0)
                RedirectToErrorPage();

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

                logger.Error("Failed for Edit Provider request: " + ex.ToString());
            }

            return View(new ProviderEditModel());
        }

        [HttpPost]
        public ActionResult Edit(ProviderEditModel model)
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
                //Begin db transaction
                var shopInfo = ExtractEditFormData(model);

                var isSuccess = _mainStore.Update(shopInfo);
                if (isSuccess)
                {
                    //Clear cache
                    CachingHelpers.ClearProviderCache();

                    this.AddNotification(NotifSettings.Success_Updated, NotificationType.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Provider POST request: " + ex.ToString());
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

            return PartialView("_DeleteInfo", id);
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
                CachingHelpers.ClearProviderCache();
            }
            catch (Exception ex)
            {
                strError = NotifSettings.Error_SystemBusy;

                logger.Error("Failed to get Delete Provider because: " + ex.ToString());

                return Json(new { success = true, message = strError });
            }

            return Json(new { success = true, message = NotifSettings.Success_Deleted });
        }

        #region Helpers

        private void DeleteImageByUrl(string url)
        {
            try
            {
                string fullPath = Request.MapPath(url);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch
            {
                // Deliberately empty.
            }
        }

        private IdentityProvider ExtractCreateFormData(ProviderCreateModel formData)
        {
            var myIdetity = new IdentityProvider();
            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.Phone = formData.Phone;
            myIdetity.Email = formData.Email;
            myIdetity.Address = formData.Address;
            myIdetity.Status = formData.Status;
            myIdetity.Email = formData.Email;
            return myIdetity;
        }

        private IdentityProvider ExtractEditFormData(ProviderEditModel formData)
        {
            var myIdetity = new IdentityProvider();
            myIdetity.Id = formData.Id;

            myIdetity.Name = formData.Name;
            myIdetity.Code = formData.Code;
            myIdetity.Phone = formData.Phone;
            myIdetity.Email = formData.Email;
            myIdetity.Address = formData.Address;
            myIdetity.Status = formData.Status;
            myIdetity.Email = formData.Email;

            return myIdetity;
        }

        private ProviderEditModel RenderEditModel(IdentityProvider identity)
        {
            var editModel = new ProviderEditModel();

            editModel.Id = identity.Id;

            editModel.Name = identity.Name;
            editModel.Code = identity.Code;
            editModel.Phone = identity.Phone;
            editModel.Email = identity.Email;
            editModel.Address = identity.Address;
            editModel.Status = identity.Status;
            editModel.Email = identity.Email;         

            return editModel;
        }

        #endregion

    }
}