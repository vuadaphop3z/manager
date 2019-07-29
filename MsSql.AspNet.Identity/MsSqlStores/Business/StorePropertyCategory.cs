using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStorePropertyCategory
    {
        List<IdentityPropertyCategory> GetByPage(IdentityPropertyCategory filter, int currentPage, int pageSize);
        int Insert(IdentityPropertyCategory identity);
        bool Update(IdentityPropertyCategory identity);
        IdentityPropertyCategory GetById(int id);
        IdentityPropertyCategory GetDetail(int id);
        bool Delete(int id);
        List<IdentityPropertyCategory> GetList();
    }

    public class StorePropertyCategory : IStorePropertyCategory
    {
        private readonly string _connectionString;
        private RpsPropertyCategory myRepository;

        public StorePropertyCategory(): this("DefaultConnection")
        {

        }

        public StorePropertyCategory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsPropertyCategory(_connectionString);
        }

        #region  Common

        public List<IdentityPropertyCategory> GetByPage(IdentityPropertyCategory filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPropertyCategory identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityPropertyCategory identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityPropertyCategory GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public IdentityPropertyCategory GetDetail(int Id)
        {
            return myRepository.GetDetail(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }
       
        public List<IdentityPropertyCategory> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
