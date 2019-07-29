using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageProductModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityProduct> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PROVIDER))]
        public int? ProviderId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PRODUCT_CATEGORY))]
        public int? ProductCategoryId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        public List<IdentityProvider> Providers { get; set; }

        public List<IdentityProductCategory> ProductCategories { get; set; }

        public List<IdentityUnit> Units { get; set; }

        public ManageProductModel()
        {
            Providers = new List<IdentityProvider>();
            ProductCategories = new List<IdentityProductCategory>();
            Units = new List<IdentityUnit>();
        }
    }

    public class ProductCommonInfoModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PROVIDER))]
        public int ProviderId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PRODUCT_CATEGORY))]
        public int ProductCategoryId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_MIN_INVENTORY))]
        public string MinInventory { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_UNIT))]
        public int UnitId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int Status { get; set; }

        public List<IdentityProvider> Providers { get; set; }

        public List<IdentityProductCategory> ProductCategories { get; set; }

        public List<IdentityUnit> Units { get; set; }

        public ProductCommonInfoModel()
        {
            Providers = new List<IdentityProvider>();
            ProductCategories = new List<IdentityProductCategory>();
            Units = new List<IdentityUnit>();
        }
    }

    public class ProductCreateModel : ProductCommonInfoModel
    {

    }

    public class ProductEditModel : ProductCommonInfoModel
    {
        public int Id { get; set; }
    }

    public class ProductChoosenModel : ManageProductModel
    {
        public string CallbackFunction { get; set; }
    }

    public class ProductItemInDropdownListModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string WarehouseNum { get; set; }
        public string UnitName { get; set; }
    }
}