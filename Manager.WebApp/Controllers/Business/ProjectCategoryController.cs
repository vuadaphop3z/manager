using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Manager.WebApp.Controllers
{
    public class ProjectCategoryController : BaseAuthedController
    {

        private readonly IStoreProjectCategory _mainStore;
        private readonly ILog logger = LogProvider.For<CompanyController>();

        public ProjectCategoryController(IStoreProjectCategory mainStore)
        {
            _mainStore = mainStore;
        }
       
        // GET: ProjectCategory
        public ActionResult Index(ManageProjectCategoryModel model)
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

            if (Request["Page"] !=null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            var filter = new IdentityProjectCategory
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status,
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

        public ActionResult Create()
        {
            var createModel = new ProjectCategoryCreateModel();

            return View(createModel);
 
        }

        [HttpPost]
        public ActionResult Create(ProjectCategoryCreateModel model)
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

                logger.Error("Failed for Create Product request: " + ex.ToString());

                return View(model);
            }

            return RedirectToAction("Create");

        }

        public ActionResult Edit(int Id)
        {

            if (Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var info = _mainStore.GetById(Id);
                if (info == null)
                    return RedirectToErrorPage();
                var editModel = RenderEditProjectCategory(info);
                return View(editModel);
            }
            catch (Exception ex)
            {

                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Product request: " + ex.ToString());
            }

            return View(new ProjectCategoryEditMode());

        }

        [HttpPost]
        public ActionResult Edit(ProjectCategoryEditMode model)
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
                var info = ExtractEditFromData(model);

                var isSuccess = _mainStore.Update(info);

                //Clear cache
                CachingHelpers.ClearProductCache(info.Id);

                if (isSuccess)
                {
                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }

            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Product request: " + ex.ToString());

                return View(model);

            }

            return RedirectToAction("Edit/" + model.Id);

        }

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

        public ActionResult Delete_comfirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _mainStore.Delete(id);

                CachingHelpers.ClearProductCache(id);
            }
            catch (Exception ex)
            {

                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Company because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        public IdentityProjectCategory ExtractCreateFormData(ProjectCategoryCreateModel formData)
        {
            var myIdentity = new IdentityProjectCategory();

            myIdentity.Code = formData.Code;
            myIdentity.Name = formData.Name;
            myIdentity.Status = formData.Status;

            return myIdentity;

        }

        public ProjectCategoryEditMode RenderEditProjectCategory(IdentityProjectCategory identity)
        {
            var editModel = new ProjectCategoryEditMode();

            editModel.Code = identity.Code;
            editModel.Name = identity.Name;
            editModel.Status = identity.Status;

            return editModel;
        }
        public IdentityProjectCategory ExtractEditFromData(ProjectCategoryEditMode formData)
        {
            var myIdentity = new IdentityProjectCategory();

            myIdentity.Id = formData.Id;
            myIdentity.Code = formData.Code;
            myIdentity.Name = formData.Name;
            myIdentity.Status = formData.Status;

            return myIdentity;

        }
    }

}