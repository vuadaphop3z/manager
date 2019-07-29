using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityArea : CommonIdentity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
    }

    public class IdentityCountry : CommonIdentity
    {
        public IdentityCountry()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int AreaId { get; set; }
        public int MasterId { get; set; }
        public string MasterName { get; set; }
        public int Status { get; set; }
    }

    public class IdentityProvince : CommonIdentity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int CountryId { get; set; }
        public int MasterId { get; set; }
        public int MasterCountryId { get; set; }
        public string MasterName { get; set; }
        public int Status { get; set; }
    }

    public class IdentityDistrict : CommonIdentity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int ProvinceId { get; set; }
        public int MasterId { get; set; }
        public int MasterProvinceId { get; set; }
        public string MasterName { get; set; }
        public int Status { get; set; }
    }
}
