using Manager.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Web;

namespace Manager.WebApp.Helpers
{
    public enum EnumStatus
    {
        [Description("Đã khóa")]
        Locked = 0,

        [Description("Kích hoạt")]
        Activated = 1,
    }

    public enum EnumWarehouseActivityType
    {
        [LocalizedDescription("LB_GOODS_RECEIPT", typeof(ManagerResource))]
        GoodsReceipt = 1,

        [LocalizedDescription("LB_GOODS_ISSUE", typeof(ManagerResource))]
        GoodsIssue = 2,

        [LocalizedDescription("LB_REFLECT_STOCK_TAKE", typeof(ManagerResource))]
        ReflectStockTake = 3
    }

    public enum EnumSlideType
    {
        [Description("Tĩnh")]
        Static = 1,

        [Description("Linh hoạt")]
        Dynamic = 2
    }

    public enum EnumLinkActionClick
    {
        [Description("Mở tab mới")]
        Blank = 1,

        [Description("Mở trực tiếp")]
        Self = 2
    }

    public enum EnumPostType
    {
        Benefit = 1,

        HostStory = 2,

        Special = 3
    }

    public enum EnumShopCategory
    {
        SmallShop = 1,
                
        Agency = 2
    }

    public static class EnumCommonCode
    {
        public static int Success = 1;
        public static int Error = -1;
        public static int Error_Info_NotFound = -2;
    }

    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _resourceKey;
        private readonly ResourceManager _resource;
        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
        {
            _resource = new ResourceManager(resourceType);
            _resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                string displayName = _resource.GetString(_resourceKey);

                return string.IsNullOrEmpty(displayName)
                    ? string.Format("[[{0}]]", _resourceKey)
                    : displayName;
            }
        }
    }
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}