using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Manager.WebApp.Helpers;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class CommonPolicyModel
    {
        public string Id { get; set; }
        public int ShopId { get; set; }
        public int PolicyId { get; set; }
        public string PolicyCode { get; set; }
    }

    public class PolicyModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityShop> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        [Display(Name = "Điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Từ ngày")]
        public string FromDate { get; set; }

        [Display(Name = "Đến ngày")]
        public string ToDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }
    }

    public class PolicyCheckInOutModel : CommonPolicyModel
    {
        [Display(Name = "Check in")]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string CheckIn { get; set; }

        [Display(Name = "Check out")]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string CheckOut { get; set; }

        [Display(Name = "Ghi chú")]
        [AllowHtml]
        public string Note { get; set; }
    }

    public class PolicyCommonChildModel : CommonPolicyModel
    {
        [Display(Name = "Độ tuổi không được ở riêng phòng")]
        public string AcceptGuest { get; set; }

        [Display(Name = "Tính giá theo người lớn với độ tuổi trên")]
        public string ChildrenOver { get; set; }

        [Display(Name = "Thông tin khác")]
        [AllowHtml]
        public string PolicyDes { get; set; }

        public string ChildrenExtraBeds { get; set; }
    }

    public class PolicyChildAndExtraBedModel : CommonPolicyModel
    {
        [Display(Name = "Áp dụng cho lứa tuổi từ")]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string AgeFrom { get; set; }

        [Display(Name = "Áp dụng đến độ tuổi")]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string AgeTo { get; set; }

        [Display(Name = "Thêm giường em bé")]
        public bool ExtraInfantBed { get; set; }

        [Display(Name = "Phụ phí giường em bé")]
        public string ExtraInfantBedSurcharge { get; set; }

        [Display(Name = "Chung giường")]
        public bool BedShare { get; set; }

        [Display(Name = "Phụ phí chung giường")]
        public string BedShareSurcharge { get; set; }

        [Display(Name = "Kèm bữa sáng")]
        public bool BreakFast { get; set; }

        [Display(Name = "Phụ phí bữa sáng")]
        public string BreakFastSurcharge { get; set; }

        [Display(Name = "Loại tiền tệ")]
        public string CurrencyCode { get; set; }

        [Display(Name = "Ghi chú")]
        [AllowHtml]
        public string Note { get; set; }

        public List<IdentityCurrency> Currencies { get; set; }
    }

    public class PolicyDinningModel : CommonPolicyModel
    {
        [Display(Name = "Loại bữa ăn")]
        public int DinningType { get; set; }

        [Display(Name = "Ăn tự chọn")]
        public bool IsBuffet { get; set; }

        [Display(Name = "Phụ phí")]
        public string Surcharge { get; set; }

        [Display(Name = "Loại tiền tệ")]
        public string CurrencyCode { get; set; }

        [Display(Name = "Ghi chú")]
        [AllowHtml]
        public string Note { get; set; }

        public List<IdentityCurrency> Currencies { get; set; }
    }

    public class PolicyPetsModel : CommonPolicyModel
    {
        [Display(Name = "Được phép mang theo vật nuôi")]
        public bool Accepted { get; set; }

        [Display(Name = "Số lượng vật nuôi mang theo")]
        public string Pets { get; set; }

        [Display(Name = "Ghi chú")]
        [AllowHtml]
        public string Note { get; set; }
    }

    public class PolicyCreditCardsModel : CommonPolicyModel
    {
        public string CreditCards { get; set; }

        [Display(Name = "Ghi chú")]
        [AllowHtml]
        public string Note { get; set; }

        public List<IdentityCredit> Credits { get; set; }
    }
}