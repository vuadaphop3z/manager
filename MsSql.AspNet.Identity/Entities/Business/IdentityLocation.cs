using Newtonsoft.Json;
using System;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityLocation : IdentityCommon
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Icon { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        [JsonIgnore]
        public string CreatedBy { get; set; }

        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }

        [JsonIgnore]
        public DateTime? LastUpdated { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }

        [JsonIgnore]
        public int Status { get; set; }

    }

}
