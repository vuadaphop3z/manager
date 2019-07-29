using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageWarehouseModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityWarehouse> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_ITEM_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_ITEM_CODE))]
        //[Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STOCK_OUT))]
        public int? IsStockOut { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CONFIRM_STOCK_QTY))]
        public int? IsConfirmStockTakeQTY { get; set; }

        public List<IdentityUnit> Units { get; set; }

        public ManageWarehouseModel()
        {
            Units = new List<IdentityUnit>();
        }

        public int WarehouseId { get; set; }
        public int ProductId { get; set; }

    }

    public class WarehouseActionModel
    {
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public IdentityProduct ProductInfo { get; set; }

        public string ItemName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_SELECT_ITEM))]
        public string ItemCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        //[Range(0.1, float.MaxValue, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NUMBER_MIN_0))]
        public string WarehouseNum { get; set; }

        public List<IdentityUnit> Units { get; set; }
    }

    public class WarehouseHistoryModel : CommonPagingModel
    {
        //public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";
        public static string DATETIME_FORMAT = "dd/MM/yyyy";

        public List<IdentityWarehouseActivity> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_KEYWORD_ITEMNAME_ITEMCODE))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_ITEM_CODE))]
        //[Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Code { get; set; }
        public int ProductId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STAFF))]
        public int StaffId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_EVENT_ACTIVITY))]
        public int ActivityType { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_DEVICE))]
        public int DeviceId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        public List<IdentityUnit> Units { get; set; }
        public List<IdentityDevice> Devices { get; set; }
        public List<IdentityUser> Users { get; set; }

        public WarehouseHistoryModel()
        {
            Units = new List<IdentityUnit>();
            Devices = new List<IdentityDevice>();
            Users = new List<IdentityUser>();
        }
    }
}