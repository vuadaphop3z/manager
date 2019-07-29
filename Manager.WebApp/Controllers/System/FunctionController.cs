using System;
using System.Web.Mvc;
using Manager.WebApp.Helpers;
using MsSql.AspNet.Identity;
using Manager.WebApp.Models;
using Manager.SharedLib.Logging;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Net;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Controllers
{
    public class FunctionController : BaseAuthedController
    {
        private readonly IAccessRolesStore _identityStore;
        private readonly IActivityStore _activityStore;
        private readonly ILog logger = LogProvider.For<FunctionController>();

        public FunctionController(IAccessRolesStore identityStore, IActivityStore activityStore)
        {
            _identityStore = identityStore;
            _activityStore = activityStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(FunctionViewModel model)
        {
            ModelState.Clear();
            var strError = string.Empty;
            try
            {
                model.AllAccesses = _identityStore.GetAllAccess();
                model.AllOperations = _identityStore.GetAllOperations();
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not GetAllAccess because: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult GetOperationsByControllerName(string ControllerName)
        {
            var myOps = Constant.GetAllControllerOperations().Where(m => m.ControllerName == ControllerName).SingleOrDefault();
            if(myOps != null && myOps.AllOperations != null)
            {
                myOps.AllOperations.OrderBy(m => m.ActionName);
            }             

            return Json(new { data = myOps }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessRoleChecker]
        public ActionResult Create(FunctionViewModel model)
        {
            var result = false;
            var strError = string.Empty;
            var identity = new IdentityOperation { AccessId = model.AccessId, ActionName = model.ActionName, OperationName = model.OperationName };
            try
            {
                var isDuplicated = _identityStore.CheckOperationDuplicate(identity);
                if (isDuplicated)
                {
                    this.AddNotification(string.Format("Could not create function due to the function [{0} of {1}] is existed", model.ActionName, model.AccessName), NotificationType.ERROR);
                    return RedirectToAction("Index");
                }

                result = _identityStore.CreateOperation(identity);
                if (result)
                {
                    this.AddNotification(string.Format("The function [{0} of {1}] is created succesfully", model.ActionName, model.AccessName), NotificationType.SUCCESS);

                    //Write log
                    var activityText = "Create new function [{0} of {1}]";
                    activityText = string.Format(activityText, model.ActionName, model.AccessName);
                    WriteActivityLog(activityText, ActivityLogType.CreateFunction, model.Id.ToString(), TargetObjectType.Function);

                    return RedirectToAction("Index");
                }
                else
                {
                    this.AddNotification("Could not create function due to database exception occurred", NotificationType.ERROR);
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not CreateFunction because: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }


            return RedirectToAction("Index");
        }

        [HttpPost]
        [AccessRoleChecker]
        public ActionResult Update(FunctionViewModel model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                var strError = string.Empty;
                var identity = new IdentityOperation { Id = model.Id, AccessId = model.AccessId, ActionName = model.ActionName, OperationName = model.OperationName };
                try
                {
                    var isDuplicated = _identityStore.CheckOperationDuplicate(identity);
                    if (isDuplicated)
                    {
                        this.AddNotification(string.Format("Could not update due to the function [{0} of {1}] is existed", model.ActionName, model.AccessName), NotificationType.ERROR);
                        return RedirectToAction("Index");
                    }

                    result = _identityStore.UpdateOperation(identity);
                    if (result)
                    {
                        this.AddNotification(string.Format("The function [{0} of {1}] is updated succesfully", model.ActionName, model.AccessName), NotificationType.SUCCESS);

                        //Write log
                        var activityText = "Updated the function [{0} of {1}]";
                        activityText = string.Format(activityText, model.ActionName, model.AccessName);
                        WriteActivityLog(activityText, ActivityLogType.UpdateFunction, model.Id.ToString(), TargetObjectType.Function);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        this.AddNotification("Could not update function due to database exception occurred", NotificationType.ERROR);
                    }
                }
                catch (Exception ex)
                {
                    strError = string.Format("Could not UpdateFunction because: {0}", ex.ToString());
                    logger.Error(strError);
                    this.AddNotification(strError, NotificationType.ERROR);
                }
            }

            return RedirectToAction("Index");
        }

        //Show popup confirm delete
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FunctionViewModel record = new FunctionViewModel();
            record.Id = id;

            return PartialView("_DeleteFunctionInfo", record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public ActionResult AcceptDeleteFunction(string id)
        {
            var strError = string.Empty;
            var result = false;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            result = _identityStore.DeleteOperation(Convert.ToInt32(id));
            if (result)
            {
                MenuHelper.ClearAllMenuCache();

                //Write log
                var activityText = "Delete the function [Id: {0}]";
                activityText = string.Format(activityText, id);
                WriteActivityLog(activityText, ActivityLogType.DeleteFunction, id, TargetObjectType.Function);

                return Json(new { success = true });
            }
            else
            {
                throw new Exception("Failed to delete this function");
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