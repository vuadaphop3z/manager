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
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Manager.WebApp.Controllers
{
    public class FrontEndSystemController : BaseAuthedController
    {        
        IFrontEndSettingsService _frontSettingService;
        private readonly IActivityStore _activityStore;
        private readonly ILog logger = LogProvider.For<SystemController>();

        public FrontEndSystemController(IFrontEndSettingsService settingService, IActivityStore activityStore)
        {
            _frontSettingService = settingService;
            _activityStore = activityStore;
        }   

        //[AccessRoleChecker]
        public ActionResult FrontEndSettings()
        {
            ModelState.Clear();

            var settings = new SiteFrontEndSettings();
            settings.Load(_frontSettingService, false);

            var model = new FrontEndSettingsViewModel()
            {
                CurrentFrontEndSettingsType = settings.General.GetType().Name,
                SystemSestings = settings,
            }
            ;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FrontEndSettings(FrontEndSettingsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var logoImg = string.Empty;
                    if (model.SiteLogoUpload != null)
                    {
                        if (model.SiteLogoUpload[0] != null)
                        {
                            var apiReturn = CdnServices.UploadImagesAsync(model.SiteLogoUpload, "0", "Home").Result;
                            if (apiReturn != null)
                            {
                                if (apiReturn.Data != null)
                                {
                                    var resultData = JsonConvert.DeserializeObject<List<string>>(apiReturn.Data.ToString());
                                    var returnData = new List<string>();
                                    if (resultData.HasData())
                                    {
                                        logoImg = resultData[0];
                                    }
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(logoImg))
                        logoImg = model.CurrentLogo;

                    model.SystemSestings.General.SiteLogo = logoImg;
                    model.SystemSestings.Save(_frontSettingService);

                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }
                else
                {
                    this.AddNotificationModelStateErrors(ModelState);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to save FrontEndSettings because: " + ex.ToString());
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