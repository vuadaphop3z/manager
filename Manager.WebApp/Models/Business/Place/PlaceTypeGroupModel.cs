using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManagePlaceTypeGroupModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityPlaceTypeGroup> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }
    }

    public class PlaceTypeGroupCreateModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Code { get; set; }

        [Display(Name = "Ảnh")]
        public string Icon { get; set; }

        [Display(Name = "Thứ tự")]
        public int SortOrder { get; set; }

        [Display(Name = "Bộ lọc")]
        public int FilterOnMap { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int Status { get; set; }
    }

    public class PlaceTypeGroupEditModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Code { get; set; }

        [Display(Name = "Ảnh")]
        public string Icon { get; set; }

        [Display(Name = "Thứ tự")]
        public int SortOrder { get; set; }

        [Display(Name = "Bộ lọc")]
        public int FilterOnMap { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int Status { get; set; }

        public List<IdentityPlaceTypeGroupLang> LangList { get; set; }
        public PlaceTypeGroupEditModel()
        {
            LangList = new List<IdentityPlaceTypeGroupLang>();
        }
    }

    public class PlaceTypeGroupLangModel
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string LangCode { get; set; }

        public bool IsUpdate { get; set; }

        public List<IdentityLanguage> Languages { get; set; }

        public PlaceTypeGroupLangModel()
        {
            Languages = new List<IdentityLanguage>();
        }
    }
}