using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityProduct : CommonIdentity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProviderId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Detail { get; set; }
        public string OtherInfo { get; set; }
        public string Cost { get; set; }
        public string SaleOffCost { get; set; }
        public double MinInventory { get; set; }
        public int UnitId { get; set; }
        public int CurrencyId { get; set; }        
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int Status { get; set; }

        //Extends
        public double WarehouseNum { get; set; }

        public double CalculateMissingRemains()
        {
            return WarehouseNum - MinInventory;
        }
    }
}
