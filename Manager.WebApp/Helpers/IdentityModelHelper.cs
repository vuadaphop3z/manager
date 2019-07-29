using Manager.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Manager.WebApp.Helpers
{
    public class IdentityModelHelper
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        public IdentityModelHelper(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            Contract.Assert(null != userManager);
            Contract.Assert(null != roleManager);
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserDetailsViewModel> GetUserDetailsViewModel(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var detailsModel = new UserDetailsViewModel() { User = user, Lockout = new LockoutViewModel() };
            var isLocked = await _userManager.IsLockedOutAsync(user.Id);
            detailsModel.Lockout.Status = isLocked ? LockoutStatus.Locked : LockoutStatus.Unlocked;
            if (detailsModel.Lockout.Status == LockoutStatus.Locked)
            {
                detailsModel.Lockout.LockoutEndDate = (await _userManager.GetLockoutEndDateAsync(user.Id)).DateTime;
            }
            return detailsModel;
        }
    }
}