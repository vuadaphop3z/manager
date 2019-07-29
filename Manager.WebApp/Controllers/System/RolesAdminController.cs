using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using MsSql.AspNet.Identity;

using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;
using System.Net;
using Manager.SharedLib.Logging;

namespace Manager.WebApp.Controllers
{
    public class RolesAdminController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<RolesAdminController>();
        private readonly IActivityStore _activityStore;

        public RolesAdminController(IActivityStore activityStore)
        {
            _activityStore = activityStore;
        }

        public RolesAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager, IActivityStore activityStore)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            _activityStore = activityStore;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /RolesAdmin/
        [AccessRoleChecker]
        public ActionResult Index()
        {
            var model = new IndexRoleViewModel(RoleManager.Roles);
            List<IdentityRole> roles = new List<IdentityRole>();
            try
            {
                roles = new RoleRepository(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetAll().OrderBy(m => m.Name).ToList();
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                return View(model);
            }

            model.RoleList = roles;
            return View(model);
        }

        //
        // POST: /RolesAdmin/Create
        [HttpPost]
        [AccessRoleChecker]
        public async Task<ActionResult> Create(IndexRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(model.CurrentRole.Name);
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                }
                else
                {
                    MenuHelper.ClearAllMenuCache();

                    this.AddNotification("The role ["+role.Name+"] is added succesffully", NotificationType.SUCCESS);

                    //Write log
                    var activityText = "Create new role device [Name: {0}]";
                    activityText = string.Format(activityText, model.CurrentRole.Name);
                    WriteActivityLog(activityText, ActivityLogType.CreateRole, model.CurrentRole.Id, TargetObjectType.RolesAdmin);

                    return RedirectToAction("Index");
                }
            }

            this.AddNotificationModelStateErrors(ModelState);
            return RedirectToAction("Index");                       
        }

        //
        // POST: /RolesAdmin/Update
        [HttpPost]
        [AccessRoleChecker]
        public async Task<ActionResult> Update(IndexRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(model.CurrentRole.Name,model.CurrentRole.Id);
                var roleresult = await RoleManager.UpdateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                }
                else
                {
                    MenuHelper.ClearAllMenuCache();
                    this.AddNotification("The role [" + role.Name + "] is updated succesffully", NotificationType.SUCCESS);

                    //Write log
                    var activityText = "Updated the role [Name: {0}]";
                    activityText = string.Format(activityText, model.CurrentRole.Name);
                    WriteActivityLog(activityText, ActivityLogType.UpdateRole, model.CurrentRole.Id, TargetObjectType.RolesAdmin);

                    return RedirectToAction("Index");
                }
            }
            this.AddNotificationModelStateErrors(ModelState);
            return RedirectToAction("Index");
        }

        //Show popup confirm delete
        public ActionResult DeleteRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RoleViewModel record = new RoleViewModel();
            record.Id = id;

            return PartialView("_DeleteroleInfo", record);
        }

        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public async Task<ActionResult>  AcceptDeleteRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = new IdentityRole(string.Empty,id);
            var roleresult = await RoleManager.DeleteAsync(role);
            if (roleresult.Succeeded)
            {
                MenuHelper.ClearAllMenuCache();

                //Write log
                var activityText = "Delete the role [Id: {0}]";
                activityText = string.Format(activityText, id);
                WriteActivityLog(activityText, ActivityLogType.DeleteRole, id, TargetObjectType.RolesAdmin);

                return Json(new { success = true, title = "Notification", message = "Deleted successfully" });
            }
            else
            {
                throw new Exception("Failed to delete this role");
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