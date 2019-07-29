using Manager.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Manager.WebApp.Models
{
    public class UserDetailsViewModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Role")]
        public string RoleId { get; set; }

        public string SearchExec { get; set; }

        public string Page { get; set; }

        public ApplicationUser User { get; set; }
        public LockoutViewModel Lockout { get; set; }

        public int IsLocked { get; set; }

        public List<Claim> Claims { get; set; }

        public UserDetailsViewModel()
        {
            Claims = new List<Claim>();
        }
    }
}