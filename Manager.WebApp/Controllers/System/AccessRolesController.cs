using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

using Manager.SharedLib.Caching.Providers;
using MsSql.AspNet.Identity;
using Manager.WebApp.Resources;

namespace Manager.WebApp.Controllers
{
    public class AccessRolesController : BaseAuthedController
    {
        private readonly IAccessRolesStore _identityStore;
        private readonly ICacheProvider _cacheProvider;

        public AccessRolesController(IAccessRolesStore identityStore, ICacheProvider cacheProvider)
        {
            _identityStore = identityStore;
            _cacheProvider = cacheProvider;
        }

        public AccessRolesController(ApplicationRoleManager roleManager, IAccessRolesStore identityStore, ICacheProvider cacheProvider)
        {
            RoleManager = roleManager;
            _identityStore = identityStore;
            _cacheProvider = cacheProvider;
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

        // GET: AccessRoles
        //[AccessRoleChecker(ControllerName = "AccessRoles", ActionName = "Index")]
        [AccessRoleChecker]
        public ActionResult Index()
        {
            AccessRolesViewModel model = new AccessRolesViewModel();
            string roleId = null;
            if (Request["RoleId"] != null)
            {
                roleId = Request["RoleId"];
            }

            try
            {
                model.AllRoles = new RoleRepository(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetAll().OrderBy(m => m.Name).ToList();
                model.AllOperations = _identityStore.GetAllOperations();

                if (!string.IsNullOrEmpty(roleId))
                {
                    //List access by role
                    //model.AccessList = _identityStore.GetAccessByRoleId(roleId);
                    //model.AccessList = _identityStore.GetAccessByRoleId(roleId);
                    //if (model.AccessList != null && model.AccessList.Count > 0)
                    //{
                    //    foreach (var acc in model.AccessList)
                    //    {
                    //        acc.OperationsList = _identityStore.GetOperationsByAccessId(acc.AccessId).ToList();
                    //    }
                    //}

                    model.RoleId = roleId;
                    model.PermissionsList = _identityStore.GetPermissionByRoleId(roleId);                   

                    //All access
                    model.AllAccess = _identityStore.GetAllAccess();
                    if (model.AllAccess != null && model.AllAccess.Count > 0)
                    {
                        foreach (var acc in model.AllAccess)
                        {
                            acc.OperationsList = _identityStore.GetOperationsByAccessId(acc.Id).ToList();
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                this.AddNotification("Failed to get data: " + ex.ToString(), NotificationType.ERROR);
                return View(model);
            }
            
            return View(model);
        }

        //
        // POST: /UpdateAccessRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public async Task<ActionResult> UpdateAccessRoles(params string[] selectedOperations)
        {
            AccessRolesViewModel model = new AccessRolesViewModel();
            string RoleId = Request["RoleId"];
            if (ModelState.IsValid)
            {
                var roleInfo = await RoleManager.FindByIdAsync(RoleId);
                if (roleInfo == null)
                {
                    this.AddNotification("This role not found" , NotificationType.ERROR);
                    return View();
                }

                selectedOperations = selectedOperations ?? new string[] { };

                try
                {
                    model.RoleId = RoleId;
                    //Remove all access roles
                    _identityStore.DeleteAllAccessOfRole(RoleId);

                    //Update new access roles
                    bool isUpdated = _identityStore.UpdateAccessOfRole(selectedOperations, RoleId);

                    if (isUpdated)                  
                    {

                        MenuHelper.ClearAllMenuCache();

                        this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                    }
                    else
                        this.AddNotification("Failed to update aceess roles: Please recheck the query and you connection", NotificationType.ERROR);
                }
                catch(Exception ex)
                {
                    this.AddNotification("Failed to update aceess roles: " + ex.InnerException.Message.ToString(), NotificationType.ERROR);                     
                }
                    
            }

            model.RoleId = RoleId;
            this.AddNotificationModelStateErrors(ModelState);
            return RedirectToAction("Index",model);
        }
        
        [AccessRoleChecker]
        public ActionResult ManageAccess(AccessViewModel model)
        {
            model.AllAccess = _identityStore.GetAllAccess();
            return View(model);
        }
    }
}