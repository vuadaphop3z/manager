using System;
using Manager.WebApp.Helpers;
using MsSql.AspNet.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class FunctionViewModel
    {
        public int Id { get; set; }

        public string OperationName { get; set; }

        [Required]
        [Display(Name = "Controller")]
        public string AccessId { get; set; }
        public string AccessName { get; set; }

        [Required]
        [Display(Name = "Action")]
        public string ActionName { get; set; }

        public List<ControllerOperations> AllControllerOperations { get; set; }
        public List<IdentityAccess> AllAccesses { get; set; }

        public List<IdentityOperation> AllOperations { get; set; }

        public FunctionViewModel()
        {
            AllControllerOperations = Constant.GetAllControllerOperations();
            AllOperations = new List<IdentityOperation>();
            AllAccesses = new List<IdentityAccess>();
        }
    }
}