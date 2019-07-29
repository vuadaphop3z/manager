using System;

using System.Collections.Generic;
using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using System.ComponentModel.DataAnnotations;
using Manager.WebApp.Resources;

namespace Manager.WebApp.Models
{
    public class NavigationViewModels
    {
        public List<IdentityNavigation> AllNavigations { get; set; }
        public int Id { get; set; }
        public int ParentId { get; set; }

        [Display(Name = "Link")]
        public string AbsoluteUri { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Title { get; set; }

        [Display(Name = "Tên action")]
        public string Action { get; set; }

        [Display(Name = "Tên controller")]
        public string Controller { get; set; }

        [Display(Name = "Hiển thị")]
        public bool Visible { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool Active { get; set; }

        public string CssClass { get; set; }

        public string IconCss { get; set; }

        [Display(Name = "Thứ tự")]
        public int SortOrder { get; set; }
        public NavigationViewModels()
        {
            AllNavigations = new List<IdentityNavigation>();
        }
    }
}