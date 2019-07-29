using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class PropertyModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityProperty> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        public int PropertyCategoryId { get; set; }
    }

    public class PropertyCommonUpdateModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        [Display(Name = "Ảnh")]
        public string Icon { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int Status { get; set; }

        [Display(Name = "Loại thuộc tính")]
        public int PropertyCategoryId { get; set; }

        public List<IdentityGroupProperty> GroupList { get; set; }

        public bool IsUpdate { get; set; }
    }

    public class PropertyCreateModel : PropertyCommonUpdateModel
    {

    }

    public class PropertyEditModel : PropertyCommonUpdateModel
    {
        public int Id { get; set; }

        public List<IdentityPropertyLang> LangList { get; set; }

        public PropertyEditModel()
        {
            LangList = new List<IdentityPropertyLang>();
        }
    }

    public class PropertyLangModel
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        public string LangCode { get; set; }

        public bool IsUpdate { get; set; }

        public int PropertyCategoryId { get; set; }

        public List<IdentityLanguage> Languages { get; set; }

        public PropertyLangModel()
        {
            Languages = new List<IdentityLanguage>();
        }
    }
}