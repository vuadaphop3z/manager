using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Manager.WebApp.Models
{
    public class AccessRolesViewModel
    {
        public AccessRolesViewModel()
        {

        }

        public string RoleId { get; set; }

        public List<IdentityAccessRoles> AccessList { get; set; }

        public List<IdentityAccess> AllAccess { get; set; }

        public List<IdentityRole> AllRoles { get; set; }

        public List<IdentityAccessRoles> PermissionsList { get; set; }

        public List<IdentityOperation> AllOperations { get; set; }
    }

    public class AccessViewModel
    {
        public List<IdentityAccess> AllAccess { get; set; }
        public List<IdentityOperation> Operations { get; set; }
        public List<string> AllControllers { get; set; }

        public string AccessId { get; set; }
        public string AccessName { get; set; }
        public string AccessDesc { get; set; }

        public AccessViewModel()
        {
            AllAccess = new List<IdentityAccess>();
        }
    }
}