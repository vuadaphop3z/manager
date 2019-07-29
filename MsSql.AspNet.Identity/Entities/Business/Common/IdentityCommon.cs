using Newtonsoft.Json;
using System;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityCommon
    {
        public string Keyword { get; set; }

        //[JsonIgnore]
        public int TotalCount { get; set; }

        [JsonIgnore]
        public DateTime? FromDate { get; set; }

        [JsonIgnore]
        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string SortField { get; set; }

        //ASC or DESC
        public string SortType { get; set; }
    }

    public class IdentityCommonCategory : IdentityCommon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }
        public int Status { get; set; }
    }
}
