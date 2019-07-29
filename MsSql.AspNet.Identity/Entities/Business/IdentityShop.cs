using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityShop : CommonIdentity
    {
        public int Id { get; set; }
        public int IsInternal { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Cover { get; set; }
        public int ProviderId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ContactInfo { get; set; }
        public int AreaId { get; set; }
        public int CountryId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int Openned { get; set; }
        public string PostCode { get; set; }
        public string Description { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastBooking { get; set; }
        public int Status { get; set; }
        public int ShopType { get; set; }
        public string Email { get; set; }
        public int CategoryId { get; set; }

        //Images
        public List<MetaShopImage> Images { get; set; }

        //Meta data
        public bool IsOwner { get; set; }
        public MetaDataShop MetaData { get; set; }

        public IdentityShop()
        {
            Images = new List<MetaShopImage>();
            MetaData = new MetaDataShop();
        }
    }

    public class IdentityShopMetaData
    {
        public int ShopId { get; set; }
        public string Amenities { get; set; }
        public string NearPlaces { get; set; }
        public decimal Reviews { get; set; }
        public string Policies { get; set; }
        public string Payments { get; set; }
    }

    public class IdentityShopMember
    {
        public int MemberId { get; set; }
        public bool IsOwner { get; set; }
    }
}
