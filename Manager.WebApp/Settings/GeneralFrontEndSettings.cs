using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Manager.WebApp.Settings
{
    public class GeneralFrontEndSettings : FrontEndSettingsBase
    {      
        [Required]
        [Display(Name = "Tên trang", Description = "Tên hiển thị của trang khách hàng")]     
        [AllowHtml]
        public string SiteName { get; set; }

        [Display(Name = "Logo", Description = "Logo của trang khách hàng")]
        public string SiteLogo { get; set; }

        [Display(Name = "Nhúng script", Description = "Các script nhúng vào website (Google Analytics, Facebook,...)")]
        [AllowHtml]
        public string EmbeddedScripts { get; set; }
    }
}
