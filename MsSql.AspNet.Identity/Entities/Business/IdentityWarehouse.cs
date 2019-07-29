using System;
using System.Collections.Generic;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityWarehouse : CommonIdentity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public double WarehouseNum { get; set; }
        public double StockTakeQTY { get; set; }

        //Extends
        public IdentityProduct ProductInfo { get; set; }
        public string ProductCode { get; set; }
        public List<IdentityProduct> ProductList { get; set; }

        public int IsStockTakeQTY { get; set; }
        public int IsStockOut { get; set; }

        public IdentityWarehouse()
        {
            ProductList = new List<IdentityProduct>();
        }
    }

    public class IdentityWarehouseActivity : CommonIdentity
    {
        public int Id { get; set; }
        public int ActivityType { get; set; }
        public int ObjectId { get; set; }
        public int ProductId { get; set; }
        public double NumberOfProducts { get; set; }
        public int StaffId { get; set; }
        public int DeviceId { get; set; }
        public DateTime? CreatedDate { get; set; }

        public IdentityProduct ProductInfo { get; set; }
    }
}
