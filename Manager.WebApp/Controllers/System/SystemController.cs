using System;
using System.Web.Mvc;
using Manager.WebApp.Helpers;
using Manager.WebApp.Services;
using MsSql.AspNet.Identity;
using Manager.WebApp.Settings;
using Manager.WebApp.Models;
using System.Net;
using Manager.SharedLib.Logging;
using Manager.WebApp.Resources;

namespace Manager.WebApp.Controllers
{
    public class SystemController : BaseAuthedController
    {        
        ISettingsService _settingService;
        private readonly IActivityStore _activityStore;
        private readonly ILog logger = LogProvider.For<SystemController>();

        public SystemController(ISettingsService settingService, IActivityStore activityStore)
        {
            _settingService = settingService;
            _activityStore = activityStore;
        }   

        [AccessRoleChecker]
        public ActionResult Settings()
        {
            ModelState.Clear();

            var settings = new SiteSettings();
            settings.Load(_settingService, false);

            var model = new SettingsViewModel()
            {
                CurrentSettingsType = settings.General.GetType().Name,
                SystemSestings = settings,
            }
            ;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings(SettingsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.SystemSestings.Save(_settingService);

                    this.AddNotification(ManagerResource.LB_OPERATION_SUCCESS, NotificationType.SUCCESS);
                }
                else
                {
                    this.AddNotificationModelStateErrors(ModelState);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to save Settings because: " + ex.ToString());
            }
            return View(model);
        }

        #region User Activities

        [AccessRoleChecker]
        public ActionResult UserActivity(ActivityLogViewModel model)
        {
            int currentPage = 1;

            int pageSize = AdminSettings.PageSize;

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
                currentPage = Convert.ToInt32(Request["Page"]);
            }

            var str_min = DateTime.MinValue.Year + "-" + DateTime.MinValue.Month + "-" + DateTime.MinValue.Day;
            var str_now = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;

            var parms = new ActivityLogQueryParms
            {
                Email = model.Email != null ? model.Email : string.Empty,
                ActivityText = model.ActivityText != null ? model.ActivityText : string.Empty,
                ActivityType = model.ActivityType != null ? model.ActivityType : string.Empty,
                FromDate = model.FromDate != null ? model.FromDate : str_min,
                ToDate = model.ToDate != null ? model.ToDate : str_now,
                PageSize = pageSize,
                CurrentPage = currentPage
            };

            try
            {
                model.SearchResults = _activityStore.FilterActivityLog(parms);
                model.Total = _activityStore.CountAllFilterActivityLog(parms);
                model.CurrentPage = currentPage;
                model.PageNo = (model.Total + pageSize - 1) / pageSize;
                model.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                return View(model);
            }
            return View(model);
        }

        public ActionResult UserActivityDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ActivityLog record = _activityStore.GetActivityLogById(id);
            if (record == null)
            {
                return HttpNotFound();
            }

            return PartialView("_ShowActivityLogInfo", record);
        }

        #endregion        
    }
}