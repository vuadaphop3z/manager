using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreProduct
    {
        List<IdentityProduct> GetByPage(IdentityProduct filter, int currentPage, int pageSize);

        List<IdentityProduct> GetActiveForChoosen(IdentityProduct filter, int currentPage, int pageSize);

        int Insert(IdentityProduct identity);
        bool Update(IdentityProduct identity);
        IdentityProduct GetById(int id);

        IdentityProduct GetDetail(int id);
        IdentityProduct GetByCode(string code);
        bool Delete(int id);
    }

    public class StoreProduct : IStoreProduct
    {
        private readonly string _connectionString;
        private RpsProduct myRepository;

        public StoreProduct() : this("DefaultConnection")
        {
        }

        public StoreProduct(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsProduct(_connectionString);
        }

        #region  Common

        public List<IdentityProduct> GetByPage(IdentityProduct filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityProduct identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityProduct identity)
        {
            return myRepository.Update(identity);
        }

        public List<IdentityProduct> GetActiveForChoosen(IdentityProduct filter, int currentPage, int pageSize)
        {
            return myRepository.GetActiveForChoosen(filter, currentPage, pageSize);
        }

        public IdentityProduct GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityProduct GetDetail(int id)
        {
            return myRepository.GetDetail(id);
        }

        public IdentityProduct GetByCode(string code)
        {
            return myRepository.GetByCode(code);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        #endregion
    }

}
