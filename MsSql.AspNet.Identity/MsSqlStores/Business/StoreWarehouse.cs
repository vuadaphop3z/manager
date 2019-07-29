using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreWarehouse
    {
        List<IdentityWarehouse> GetByPage(IdentityWarehouse filter, int currentPage, int pageSize);
        List<IdentityWarehouse> GetProductStockOutByPage(IdentityWarehouse filter, int currentPage, int pageSize);
        int GoodsReceipt(IdentityWarehouse identity, IdentityWarehouseActivity activity);
        int GoodsIssue(IdentityWarehouse identity, IdentityWarehouseActivity activity);
        double ReflectStockTake(IdentityWarehouse identity, IdentityWarehouseActivity activity);
        bool Update(IdentityWarehouse identity);
        IdentityWarehouse GetById(int id);
        bool Delete(int id);
        List<IdentityWarehouse> GetList();
        List<IdentityWarehouseActivity> GetHistoryByPage(IdentityWarehouseActivity filter, int currentPage, int pageSize);
    }

    public class StoreWarehouse : IStoreWarehouse
    {
        private readonly string _connectionString;
        private RpsWarehouse myRepository;

        public StoreWarehouse() : this("DefaultConnection")
        {

        }

        public StoreWarehouse(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsWarehouse(_connectionString);
        }

        #region  Common

        public List<IdentityWarehouse> GetByPage(IdentityWarehouse filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityWarehouse> GetProductStockOutByPage(IdentityWarehouse filter, int currentPage, int pageSize)
        {
            return myRepository.GetProductStockOutByPage(filter, currentPage, pageSize);
        }

        public int GoodsReceipt(IdentityWarehouse identity, IdentityWarehouseActivity activity)
        {
            return myRepository.GoodsReceipt(identity, activity);
        }

        public int GoodsIssue(IdentityWarehouse identity, IdentityWarehouseActivity activity)
        {
            return myRepository.GoodsIssue(identity, activity);
        }

        public double ReflectStockTake(IdentityWarehouse identity, IdentityWarehouseActivity activity)
        {
            return myRepository.ReflectStockTake(identity, activity);
        }

        public bool Update(IdentityWarehouse identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityWarehouse GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }

        public List<IdentityWarehouse> GetList()
        {
            return myRepository.GetList();
        }

        public List<IdentityWarehouseActivity> GetHistoryByPage(IdentityWarehouseActivity filter, int currentPage, int pageSize)
        {
            return myRepository.GetHistoryByPage(filter, currentPage, pageSize);
        }

        #endregion
    }
}
