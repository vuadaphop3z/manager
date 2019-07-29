using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityProject :CommonIdentity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int ProjectCategoryId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Detail { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? FinishDate {get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        //public int LastUpdatedBy { get; set; }

        //public DateTime? LastUpdated { get; set; }
        public int Status { get; set; }

    }
}
