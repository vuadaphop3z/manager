using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageSlideModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentitySlide> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }
    }

    public class SlideCommonUpdateModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Code { get; set; }

        [Display(Name = "Phân loại")]
        public string SlideType { get; set; }

        [Display(Name = "Css Class")]
        public string CssClass { get; set; }

        [Display(Name = "Thời gian trễ (mili giây)")]
        public int DelayTime { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int Status { get; set; }
    }

    public class SlideCreateModel : SlideCommonUpdateModel
    {

    }

    public class SlideEditModel : SlideCommonUpdateModel
    {
        public int Id { get; set; }
    }

    #region Slide Item

    public class ManageSlideItemModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentitySlideItem> SearchResults { get; set; }

        public int SlideId { get; set; }
    }

    public class SlideItemUpdateModel
    {
        public int Id { get; set; }

        public int SlideId { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Title { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        public string CurrentImageUrl { get; set; }

        [Display(Name = "Ảnh")]
        public List<HttpPostedFileBase> ImageUpload { get; set; }

        [Display(Name = "Link điều hướng")]
        public string Link { get; set; }

        [Display(Name = "Sự kiện click vào link")]
        public int LinkAction { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int SortOrder { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int Status { get; set; }
    }

    #endregion
}