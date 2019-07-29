using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreLanguage
    {
        List<IdentityLanguage> GetAll(IdentityLanguage filter, int currentPage, int pageSize);
        int Insert(IdentityLanguage identity);
        bool Update(IdentityLanguage identity);
        IdentityLanguage GetById(int id);
        bool Delete(int id);
        List<IdentityLanguage> GetList();
    }

    public class StoreLanguage : IStoreLanguage
    {
        private readonly string _connectionString;
        private RpsLanguage myRepository;

        public StoreLanguage(): this("PfoodDBConnection")
        {

        }

        public StoreLanguage(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsLanguage(_connectionString);
        }

        #region  Common

        public List<IdentityLanguage> GetAll(IdentityLanguage filter, int currentPage, int pageSize)
        {
            return myRepository.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityLanguage identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityLanguage identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityLanguage GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }
       
        public List<IdentityLanguage> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
