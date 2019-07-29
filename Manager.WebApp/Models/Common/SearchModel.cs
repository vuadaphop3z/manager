using System.Collections.Generic;

namespace Manager.WebApp.Models
{
    public class ApiSearchModel
    {
        public string Keyword { get; set; }
    }

    public class ApiUserModel
    {
        public int UserId { get; set; }
    }

    public class ApiListUserInfoModel
    {
        /// <summary>
        /// ListUserId
        /// </summary>
        public List<int> ListUserId { get; set; }
    }
}