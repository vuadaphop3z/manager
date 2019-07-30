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
    public class ProjectController : BaseAuthedController
    {
        // GET: Project
        private readonly IStoreProject _mainStore;

        private readonly ILog logger = LogProvider.For<CompanyController>();

        public ProjectController(IStoreProject mainStore)
        {
            _mainStore = mainStore;
        }

        public ActionResult Index(ManageProjectModel model)
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

            var filter = new IdentityProject
            {
                Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                Status = model.Status == null ? -1 : (int)model.Status,
            };

            try
            {

                model.Companys = CommonHelpers.GetListCompany();
                model.ProjectCategorys = CommonHelpers.GetListProjectCatefory();
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

        public ActionResult Create ()
        {
            var createModel = new ProjectCreateModel();

            try
            {
                createModel.ProjectCategorys = CommonHelpers.GetListProjectCatefory();
                createModel.Companys = CommonHelpers.GetListCompany();

            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed display Create Project page: " + ex.ToString());
            }

            return View(createModel);
        }

        [HttpPost]


        public ActionResult Create (ProjectCreateModel model)
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

                var editModel = RenderEditProject(info);


                return View(editModel);

            }
            catch (Exception ex)
            {

                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Edit Project request: " + ex.ToString());
            }

            return View(new ProjectEditModel());
        }
        
        [HttpPost]
        public ActionResult Edit(ProjectEditModel model)
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
                var info = ExtractEditFormData(model);
                var isSuccess = _mainStore.Update(info);

                CachingHelpers.ClearProjectCache(info.Id);

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


        public IdentityProject ExtractCreateFormData (ProjectCreateModel formData)
        {
            var myIdentity = new IdentityProject();
            myIdentity.Code = formData.Code;
            myIdentity.ProjectCategoryId = formData.ProjectCategoryId;
            myIdentity.CompanyId = formData.CompanyId;
            myIdentity.Name = formData.Name;
            myIdentity.ShortDescription = formData.ShortDescription;
            myIdentity.Detail = formData.Detail;
            //myIdentity.BeginDate = Convert.ToDateTime(formData.BeginDate);
            //myIdentity.FinishDate = Convert.ToDateTime(formData.FinishDate);            
            myIdentity.BeginDate = formData.BeginDate;
            myIdentity.FinishDate = formData.FinishDate;
            myIdentity.Status = formData.Status;

            return myIdentity;
        }

        public ProjectEditModel RenderEditProject(IdentityProject identity)
        {
            var editModel = new ProjectEditModel();

            editModel.Code = identity.Code;
            editModel.ProjectCategoryId = identity.ProjectCategoryId;
            editModel.CompanyId = identity.CompanyId;            
            editModel.ProjectCategorys = CommonHelpers.GetListProjectCatefory();
            editModel.Companys = CommonHelpers.GetListCompany();
            editModel.Name = identity.Name;
            editModel.ShortDescription = identity.ShortDescription;
            editModel.Detail = identity.Detail;
            editModel.BeginDate = identity.BeginDate;
            editModel.FinishDate = identity.FinishDate;
            editModel.Status = identity.Status;

            return editModel;

        }

        public IdentityProject ExtractEditFormData(ProjectEditModel formData)
        {
            var myIdentity = new IdentityProject();
            myIdentity.Id = formData.Id;
            myIdentity.Code = formData.Code;
            myIdentity.ProjectCategoryId = formData.ProjectCategoryId;
            myIdentity.CompanyId = formData.CompanyId;
            myIdentity.Name = formData.Name;
            myIdentity.ShortDescription = formData.ShortDescription;
            myIdentity.Detail = formData.Detail;
            //myIdentity.BeginDate = Convert.ToDateTime(formData.BeginDate);
            //myIdentity.FinishDate = Convert.ToDateTime(formData.FinishDate);            
            myIdentity.BeginDate = formData.BeginDate;
            myIdentity.FinishDate = formData.FinishDate;
            myIdentity.Status = formData.Status;

            return myIdentity;
        }

    }

}