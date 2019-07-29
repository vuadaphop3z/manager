using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Manager.WebApp.Models;
using Manager.WebApp.Helpers;

using MsSql.AspNet.Identity;
using Manager.SharedLib.Logging;
using Microsoft.AspNet.Identity;
using System.Net;

namespace Manager.WebApp.Controllers
{
    public class AccessController : BaseAuthedController
    {
        private readonly IAccessRolesStore _identityStore;
        private readonly IActivityStore _activityStore;

        private readonly ILog logger = LogProvider.For<AccessController>();

        public AccessController(IAccessRolesStore identityStore, IActivityStore activityStore)
        {
            _identityStore = identityStore;
            _activityStore = activityStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(AccessViewModel model)
        {
            var strError = string.Empty;
            try
            {
                model.AllAccess = _identityStore.GetAllAccess();
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not GetAllAccess because: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }

            model.AllControllers = Constant.GetAllControllers(Server.MapPath("~/Controllers"));
            return View(model);
        }

        [HttpPost]
        [AccessRoleChecker]
        public ActionResult Create(AccessViewModel model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                var strError = string.Empty;
                var accessIdentity = new IdentityAccess { Id = model.AccessId, AccessName = model.AccessName, Description = model.AccessDesc };
                try
                {
                    var isDuplicated = _identityStore.CheckAccessDuplicate(accessIdentity);
                    if (isDuplicated)
                    {
                        this.AddNotification(string.Format("Could not create access due to the access [{0}] is existed", model.AccessName), NotificationType.ERROR);
                        return RedirectToAction("Index");
                    }

                    result = _identityStore.CreateAccess(accessIdentity);
                    if (result)
                    {
                        this.AddNotification("The access [" + model.AccessName + "] is created succesfully", NotificationType.SUCCESS);

                        //Write log
                        var activityText = "Create new access [Name: {0}]";
                        activityText = string.Format(activityText, model.AccessName);
                        WriteActivityLog(activityText, ActivityLogType.CreateAccess, model.AccessId, TargetObjectType.Access);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        this.AddNotification("Could not create access due to database exception occurred", NotificationType.ERROR);
                    }
                }
                catch (Exception ex)
                {
                    strError = string.Format("Could not CreateAccess because: {0}", ex.ToString());
                    logger.Error(strError);
                    this.AddNotification(strError, NotificationType.ERROR);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AccessRoleChecker]
        public ActionResult Update(AccessViewModel model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                var strError = string.Empty;
                var accessIdentity = new IdentityAccess { Id = model.AccessId, AccessName = model.AccessName, Description = model.AccessDesc };
                try
                {
                    var isDuplicated = _identityStore.CheckAccessDuplicate(accessIdentity);
                    if (isDuplicated)
                    {
                        this.AddNotification(string.Format("Could not update due to the access [{0}] is existed", model.AccessName), NotificationType.ERROR);
                        return RedirectToAction("Index");
                    }

                    result = _identityStore.UpdateAccess(accessIdentity);
                    if (result)
                    {
                        this.AddNotification("The access [" + model.AccessName + "] is updated succesfully", NotificationType.SUCCESS);

                        //Write log
                        var activityText = "Updated the access [Name: {0}]";
                        activityText = string.Format(activityText, model.AccessName);
                        WriteActivityLog(activityText, ActivityLogType.UpdateAccess, model.AccessId, TargetObjectType.Access);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        this.AddNotification("Could not update acces due to database exception occurred", NotificationType.ERROR);
                    }                   
                }
                catch (Exception ex)
                {
                    strError = string.Format("Could not UpdateAccess because: {0}", ex.ToString());
                    logger.Error(strError);
                    this.AddNotification(strError, NotificationType.ERROR);
                }
            }

            return RedirectToAction("Index");
        }

        //Show popup confirm delete
        public ActionResult DeleteAccess(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AccessViewModel record = new AccessViewModel();
            record.AccessId = id;

            return PartialView("_DeleteAccessInfo", record);
        }

        [HttpPost, ActionName("DeleteAccess")]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public ActionResult AcceptDeleteAccess(string id)
        {
            var strError = string.Empty;
            var result = false;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            result = _identityStore.DeleteAccess(id);
            if (result)
            {
                MenuHelper.ClearAllMenuCache();

                //Write log
                var activityText = "Delete the Access [Id: {0}]";
                activityText = string.Format(activityText, id);
                WriteActivityLog(activityText, ActivityLogType.DeleteAccess, id, TargetObjectType.Access);

                return Json(new { success = true });
            }
            else
            {
                throw new Exception("Failed to delete this access");
            }
        }

        #region Helpers

        private void WriteActivityLog(string activityText, ActivityLogType activityType, string targetId, TargetObjectType targetType)
        {
            var logData = new ActivityLog
            {
                UserId = User.Identity.GetUserId(),
                ActivityText = activityText,
                ActivityType = activityType.ToString(),
                TargetId = targetId,
                TargetType = targetType.ToString(),
                IPAddress = Request.UserHostAddress
            };

            _activityStore.WriteActivityLog(logData);
        }

        #endregion
    }
}