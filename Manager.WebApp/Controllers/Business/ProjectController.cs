using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using System;
using System.Collections.Generic;
using System.Linq;
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
                //Status = model.Status == null ? -1 : (int)model.Status,
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
    }
}