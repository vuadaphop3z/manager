using MsSql.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manager.WebApp.Models
{
    public class ActivityLogViewModel
    {
        public List<ActivityLog> SearchResults { get; set; }

        public string Email { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string ActivityText { get; set; }

        public string ActivityType { get; set; }

        public string SearchExec { get; set; }

        public int Total { get; set; }

        public int CurrentPage { get; set; }

        public int PageNo { get; set; }

        public int PageSize { get; set; }

        public int StartCount()
        {
            var index = CurrentPage * PageSize - PageSize + 1;
            return index;
        }

        public int EndCount()
        {
            var index = StartCount() + SearchResults.Count - 1;
            return index;
        }

        public IEnumerable<SelectListItem> ActivityTypeList
        {
            get
            {
                IEnumerable<ActivityLogType> actionTypes = Enum.GetValues(typeof(ActivityLogType))
                                                       .Cast<ActivityLogType>();
                var selectList = from action in actionTypes
                                 select new SelectListItem
                                 {
                                     Text = action.ToString(),
                                     Value = action.ToString(),
                                     Selected = action.ToString() == this.ActivityType
                                 };
                return selectList;
            }
        }   
    }

    public enum ActivityLogType
    {
        CreateRole,
        UpdateRole,
        DeleteRole,
        DeleteAccess,
        CreateAccess,
        UpdateAccess,
        DeleteFunction,
        CreateFunction,
        UpdateFunction
    }

    public enum TargetObjectType
    {
        RolesAdmin,
        Account,
        Access,
        Function        
    }
}