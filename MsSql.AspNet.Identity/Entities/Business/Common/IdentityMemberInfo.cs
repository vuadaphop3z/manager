using Newtonsoft.Json;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityMemberInfo : IdentityCommon
    { 
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        
        [JsonIgnore]
        public string TokenKey { get; set; }

        public string Avatar { get; set; }

        //For get data of an user
        public int OwnerId { get; set; }
    }
}
