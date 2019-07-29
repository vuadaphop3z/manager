using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity;

namespace Manager.WebApp.Controllers
{
    public class BaseController : Controller
    {
        private List<ActionError> _actionErrors;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Preparing before executing action
            _actionErrors = new List<ActionError>();
            ViewBag.ActionErrors = _actionErrors;
            ViewBag.AdminNavMenu = MenuHelper.GetAdminNavigationMenuItems();

            
            base.OnActionExecuting(filterContext);
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                _actionErrors.Add(new ActionError() { Reason = error });
            }
        }

        public void AddError(Exception ex)
        {
            _actionErrors.Add(new ActionError() { Reason = ex.Message, Details = ex.StackTrace });
        }


        protected string GetModelStateErrors(ModelStateDictionary stateDic)
        {
            var sb = new StringBuilder();            
            foreach (var errorKey in stateDic.Keys)
            {
                foreach (var errorMsg in stateDic[errorKey].Errors)
                {
                    sb.AppendLine(errorMsg.ErrorMessage + "<br />");
                }
            }

            return sb.ToString();
        }


        public Dictionary<string, List<string>> GetModelStateErrorList(ModelStateDictionary stateDic)
        {
            var result = new Dictionary<string, List<string>>();
            
            foreach (var errorKey in stateDic.Keys)
            {
                var errors = new List<string>();
                foreach (var errorMsg in stateDic[errorKey].Errors)
                {
                    errors.Add(errorMsg.ErrorMessage);
                }

                result.Add(errorKey, errors);
            }

            return result;
        }


        public void AddNotificationModelStateErrors(ModelStateDictionary stateDic)
        {
            foreach (var errorKey in stateDic.Keys)
            {
                foreach (var errorMsg in stateDic[errorKey].Errors)
                {
                    this.AddNotification(errorMsg.ErrorMessage, NotificationType.ERROR);
                }
            }
        }

        protected IdentityUser GetCurrentUser()
        {
            var userId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(userId))
                return AccountHelper.GetUserById(userId);

            return null;
        }

        protected string GetCurrentUserId()
        {
            var currentUser = GetCurrentUser();
            if (currentUser != null)
                return currentUser.Id;

            return string.Empty;
        }

        protected int GetCurrentStaffId()
        {
            var currentUser = GetCurrentUser();
            if (currentUser != null)
                return currentUser.StaffId;

            return 0;
        }
    }
}