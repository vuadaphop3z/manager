using System;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityPricePlan : CommonIdentity
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Title { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Approved { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }        
        public int Status { get; set; }
        public string Note { get; set; }
    }
}
