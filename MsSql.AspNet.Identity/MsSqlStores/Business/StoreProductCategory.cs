using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreProductCategory
    {
        List<IdentityProductCategory> GetByPage(IdentityProductCategory filter, int currentPage, int pageSize);
        int Insert(IdentityProductCategory identity);
        bool Update(IdentityProductCategory identity);
        IdentityProductCategory GetById(int id);
        bool Delete(int id);
        List<IdentityProductCategory> GetList();
    }

    public class StoreProductCategory : IStoreProductCategory
    {
        private readonly string _connectionString;
        private RpsProductCategory myRepository;

        public StoreProductCategory(): this("DefaultConnection")
        {

        }

        public StoreProductCategory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsProductCategory(_connectionString);
        }

        #region  Common

        public List<IdentityProductCategory> GetByPage(IdentityProductCategory filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityProductCategory identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityProductCategory identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityProductCategory GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }
       
        public List<IdentityProductCategory> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
