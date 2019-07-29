using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityCompany : CommonIdentity
    {
        public int Id { get; set;}
        public string Code { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public Boolean IsEPE { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Status { get; set; }

    }
}
