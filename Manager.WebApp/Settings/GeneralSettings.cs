
using Manager.WebApp.Resources;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Manager.WebApp.Settings
{
    public class GeneralSettings : SettingsBase
    {
        //[Required]
        [Display(Name = "Site Name", Description = "The name of website backend application")]     
        [AllowHtml]
        public string SiteName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [Range(0, 120, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NUMBER_MIN_0))]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STORAGE_HISTORY_PERIOD_TIME), Description = nameof(ManagerResource.LB_STORAGE_HISTORY_PERIOD_TIME_DES))]
        public int StoragePeriodTime { get; set; }

        //[Required]
        [Display(Name = "Admin Email", Description = "This email is used for sending notification or receive feedback information from user")]        
        public string AdminEmail { get; set; }

        //[Required]
        [Display(Name = "Time Zone", Description = "The valid time zone for display correct date time on the UI")]        
        public string TimeZoneId { get; set; }

        /*
        public TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId); }
            set { TimeZoneId = value.Id; }
        }
        */

        /*
        public  TimeZoneList ()
        {
            var timeZoneList = TimeZoneInfo
            .GetSystemTimeZones()
            .Select(t => new SelectListItem
            {
                Text = t.DisplayName,
                Value = t.Id,
                Selected = Model != null && t.Id == Model.TimeZoneDefault.Id
            });
        }
        */ 
    }
}
