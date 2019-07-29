using Manager.WebApp.Resources;
using MsSql.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Manager.WebApp.Models
{
    public class UserViewModel
    {
        public UserViewModel()
        {

        }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_ROLE))]
        public string RoleId { get; set; }

        public string SearchExec { get; set; }
        public List<IdentityRole> AllRoles { get; set; }

        public IEnumerable<SelectListItem> UserRoleList
        {
            get
            {                
                var selectList = from role in AllRoles
                                 select new SelectListItem
                                 {
                                     Text = role.Name,
                                     Value = role.Id,
                                     Selected = role.Id == this.RoleId
                                 };
                return selectList;
            }
        }

        public int Total { get; set; }

        public int CurrentPage { get; set; }

        public int PageNo { get; set; }

        public int PageSize { get; set; }        

        /**************Search Result==============*/
        public List<IdentityUser> SearchResult { get; set; }

        public IdentityUser UserInfoViewModel { get; set; }

        public int IsLocked { get; set; }
    }
}