using System;

using System.Collections.Generic;
using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using System.ComponentModel.DataAnnotations;
using Manager.WebApp.Resources;
using static Manager.WebApp.LanguagesProvider;

namespace Manager.WebApp.Models
{
    public class MenuViewModels
    {
        public List<IdentityMenu> AllMenus { get; set; }
        public List<IdentityAccess> AllAccess { get; set; }
        public List<IdentityOperation> AllOperation { get; set; }
        public IdentityAccess Access { get; set; }
        public int Id { get; set; }
        public int ParentId { get; set; }

        [Display(Name = "Chức năng")]
        public string AccessName { get; set; }
        public string AccessId { get; set; }

        [Display(Name = "Phương thức")]
        public string ActionName { get; set; }
        public string OperationName { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TITLE))]
        public string Title { get; set; }

        [Display(Name = "Hiển thị")]
        public bool Visible { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool Active { get; set; }
       
        public string CssClass { get; set; }

        public string IconCss { get; set; }

        [Display(Name = "Thứ tự")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Vui lòng nhập số")]
        public int SortOrder { get; set; }
        public MenuViewModels()
        {
            AllMenus = new List<IdentityMenu>();
            AllAccess = new List<IdentityAccess>();
            AllOperation = new List<IdentityOperation>();
        }
    }

    public class ManageMenuLangModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Title { get; set; }

        public bool IsUpdate { get; set; }
        
        public string LangCode { get; set; }

        public int MenuId { get; set; }

        public List<Languages> Languages { get; set; }

        public IdentityMenu MenuInfo { get; set; }

        public ManageMenuLangModel()
        {
            Languages = new List<Languages>();
        }
    }
}