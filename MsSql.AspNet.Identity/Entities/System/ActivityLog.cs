using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity
{
    public class ActivityLog
    {
        public int ActivityLogId { get; set; }

        public string UserId { get; set; }

        public string ActivityText { get; set; }

        public string ActivityType { get; set; }

        public string TargetType { get; set; }

        public string TargetId { get; set; }

        public string IPAddress { get; set; }

        public DateTime ActivityDate { get; set; }

        //For display relative time Ex: an hour ago, 5 hours ago, one day ago,...
        public string FriendlyRelativeTime { get; set; }

        //Actor
        public string UserName { get; set; }
    }

    public class ActivityLogQueryParms
    {
        public string Email { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string ActivityText { get; set; }

        public string ActivityType { get; set; }

        public string SearchExec { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }
    }
}
