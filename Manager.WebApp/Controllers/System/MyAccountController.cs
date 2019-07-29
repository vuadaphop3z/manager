using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Manager.WebApp.Models;
using MsSql.AspNet.Identity;
using Manager.WebApp.Helpers;
using System.Configuration;

using Manager.SharedLib.Logging;
using System.Security.Claims;
using System.Globalization;
using System.Threading;
using Manager.WebApp.Caching;
using Manager.WebApp.Resources;

namespace Manager.WebApp.Controllers
{        
    public class MyAccountController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<AccountController>();

        private readonly IIdentityStore _identityStore;
        private readonly IActivityStore _activityStore;

        public MyAccountController(IIdentityStore identityStore, IActivityStore activityStore)
        {
            _identityStore = identityStore;
            _activityStore = activityStore;
        }

        public MyAccountController(ApplicationUserManager userManager, IIdentityStore identityStore, IActivityStore activityStore)
        {
            UserManager = userManager;
            _identityStore = identityStore;
            _activityStore = activityStore;
        }


        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
      
        private SignInHelper _helper;

        private SignInHelper SignInHelper
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new SignInHelper(UserManager, AuthenticationManager);
                }
                return _helper;
            }
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        [AccessRoleChecker]
        public ActionResult Profile()
        {
            AccountDetailViewModel model = new AccountDetailViewModel();
            string currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Account", "Login");
            try
            {
                var userInfo = _identityStore.GetUserByID(currentUser);
                var _userRoles = UserManager.GetRoles(currentUser);

                if (_userRoles != null)
                {
                    model.RolesList = _userRoles.ToList();
                }
                model.Id = userInfo.Id;
                model.UserName = userInfo.UserName;
                model.PhoneNumber = userInfo.PhoneNumber;
                model.Email = userInfo.Email;
                model.CreatedDateUtc = userInfo.CreatedDateUtc;
                model.Avatar = userInfo.Avatar;
            }
            catch 
            {

            }            
             
            //Get newest activity
            try
            {
                int currentPage = 1;
                //Limit activity on once query.
                int pageSize = int.Parse(ConfigurationManager.AppSettings["Paging:PageSize"]);
                int total = 0;

                model.ActivityNews = _activityStore.GetActivityLogByUserId(currentUser, currentPage, pageSize);
                total = _activityStore.CountAllActivityLogByUserId(currentUser);

                model.ActivityPagingInfo = new PagingInfo { 
                    CurrentPage = currentPage,
                    //PageNo = (int)(total / pageSize),
                    PageNo = (total + pageSize - 1) / pageSize,
                    PageSize = pageSize,
                    Total = total
                };

                if (model.ActivityNews != null && model.ActivityNews.Count > 0)
                {
                    foreach (var record in model.ActivityNews)
                    {
                        //Calculate time
                        record.FriendlyRelativeTime = DateTimeHelper.GetFriendlyRelativeTime(record.ActivityDate);
                    }
                }
            }
            catch
            {
            }            

            return View(model);
        }

        [AllowAnonymous]
        public JsonResult GetActivityLogs(string page)
        {
            List<ActivityLog> list = new List<ActivityLog>();
            string currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
            try
            {
                list = _activityStore.GetActivityLogByUserId(currentUser, Convert.ToInt32(page), AdminSettings.PageSize);
                if (list != null && list.Count > 0)
                {
                    foreach (var record in list)
                    {
                        //Calculate time
                        record.FriendlyRelativeTime = DateTimeHelper.GetFriendlyRelativeTime(record.ActivityDate);
                    }
                }
            }
            catch
            {
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //Show change password form
        [AccessRoleChecker]
        public ActionResult ChangePassword()
        {
            string currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login","Account");

            AccountChangePasswordViewModel model = new AccountChangePasswordViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public ActionResult ChangePassword(AccountChangePasswordViewModel model)
        {
            string currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Account", "Login");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.OldPassword = Utility.Md5HashingData(model.OldPassword);
            model.NewPassword = Utility.Md5HashingData(model.NewPassword);

            //Change password
            var result = UserManager.ChangePassword(currentUser, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                this.AddNotification("Changed password successfull",NotificationType.SUCCESS);
                return RedirectToAction("Profile", "Account");
            }
            
            AddErrors(result);
            return View();
        }

        public ActionResult UpdateProfile()
        {
            var model = new UpdateUserProfileModel();            
            try
            {
                if (Request.IsAuthenticated)
                {
                    model.UserId = User.Identity.GetUserId();
                    model.UserName = User.Identity.GetUserName();

                    var claims = (ClaimsIdentity)User.Identity;
                    model.FullName = claims.FindFirstValue(ClaimKeys.FullName);
                    model.Avatar = claims.FindFirstValue(ClaimKeys.Avatar);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification("System is busy now. Please try again later", NotificationType.ERROR);
                var strError = string.Format("Could not open page UpdateProfile because: {0}", ex.ToString());
                logger.Error(strError);

                return View(model);
            }

            return View(model);
        }

        [ActionName("UpdateProfile")]
        [HttpPost]
        public ActionResult UpdateProfile_Post(UpdateUserProfileModel model)
        {
            try
            {
                var currentIdentity = HttpContext.User.Identity as ClaimsIdentity;

                var avatarClaim = currentIdentity.FindFirst(ClaimKeys.Avatar);
                if(avatarClaim != null)
                {
                    currentIdentity.RemoveClaim(avatarClaim);
                    UserManager.RemoveClaim(model.UserId, avatarClaim);
                }

                currentIdentity.AddClaim(new Claim(ClaimKeys.Avatar, model.Avatar));
                UserManager.AddClaim(model.UserId, new Claim(ClaimKeys.Avatar, model.Avatar));

                var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(currentIdentity), new AuthenticationProperties() { IsPersistent = true });                

                this.AddNotification("Update profile successfully", NotificationType.SUCCESS);
            }
            catch (Exception ex)
            {
                this.AddNotification("System is busy now. Please try again later", NotificationType.ERROR);
                var strError = string.Format("Could not UpdateProfile because: {0}", ex.ToString());
                logger.Error(strError);

                return View(model);
            }

            return View(model);
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