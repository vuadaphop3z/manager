using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores.Business;

namespace Manager.WebApp.Models
{
    public class ManageShopModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityShop> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        [Display(Name = "Phân loại")]
        public int? CategoryId { get; set; }
    }

    public class ShopCommonUpdateModel
    {
        [Display(Name = "Quốc tế")]
        public bool IsInternal { get; set; }

        [Display(Name = "Tên cửa hàng")]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        [Display(Name = "Nhà cung cấp")]
        public int ProviderId { get; set; }

        public List<IdentityProvider> Providers { get; set; }

        [Display(Name = "Điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Address { get; set; }

        [Display(Name = "Thông tin liên hệ")]
        public string ContactInfo { get; set; }

        public int AreaId { get; set; }

        public List<IdentityArea> Areas { get; set; }

        public int CountryId { get; set; }

        public int ProvinceId { get; set; }

        public int DistrictId { get; set; }

        [Display(Name = "Năm bắt đầu")]
        public int Openned { get; set; }

        [Display(Name = "Mã bưu điện")]
        public string PostCode { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Vĩ độ")]
        public string Latitude { get; set; }

        [Display(Name = "Kinh độ")]
        public string Longitude { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        public List<IdentityPolicy> Policies { get; set; }
    }

    public class ShopCreateModel : ShopCommonUpdateModel
    {

    }

    public class ShopEditModel : ShopCommonUpdateModel
    {
        public int Id { get; set; }

        public List<IdentityCountry> Countries { get; set; }

        public List<IdentityProvince> Provinces { get; set; }

        public List<IdentityDistrict> Districts { get; set; }

        public List<MetaShopImage> Images { get; set; }

        public MetaDataShop MetaData { get; set; }
    }

    public class ShopDetailsModel
    {
        public IdentityShop shopInfo { get; set; }

        public string CurrentUser { get; set; }
        public string CurrentUserName { get; set; }
    }

    public class PlaceModel
    {
        public string Id { get; set; }

        public int ShopId { get; set; }

        [Display(Name = "Tên địa điểm")]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_ICON))]
        public string Icon { get; set; }

        [Display(Name = "Khoảng cách")]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Range { get; set; }

        [Display(Name = "Đơn vị")]
        public string UnitCode { get; set; }

        public List<IdentityUnit> Units { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }

    public class ManageAddShopOwnerModel
    {
        public List<IdentityMember> ListMember { get; set; }
    }

    public class ShopItemInListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ShopAssignToMemberModel
    {
        public int MemberId { get; set; }
        public int ShopId { get; set; }

        [Required(ErrorMessage = "Nhập tên quản trị viên, số điện thoại hoặc email để tìm kiếm")]
        public string DisplayName { get; set; }

        public bool IsOwner { get; set; }

        public List<IdentityMember> AssignedMembers { get; set; }
    }

    public class ShopAssignCategoryModel
    {
        public int Id { get; set; }
        public int ShopCategory { get; set; }
    }
}