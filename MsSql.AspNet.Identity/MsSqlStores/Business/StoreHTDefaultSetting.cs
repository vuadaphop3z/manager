using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreHTDefaultSetting
    {
        List<IdentityHTDefaultSetting> GetByPage(IdentityHTDefaultSetting filter, int currentPage, int pageSize);
        int Insert(IdentityHTDefaultSetting identity);
        int Update(IdentityHTDefaultSetting identity);
        IdentityHTDefaultSetting GetById(int id);
        bool Delete(int id);
        List<IdentityHTDefaultSetting> GetList();
    }

    public class StoreHTDefaultSetting : IStoreHTDefaultSetting
    {
        private readonly string _connectionString;
        private RpsHTDefaultSetting myRepository;

        public StoreHTDefaultSetting(): this("DefaultConnection")
        {

        }

        public StoreHTDefaultSetting(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsHTDefaultSetting(_connectionString);
        }

        #region  Common

        public List<IdentityHTDefaultSetting> GetByPage(IdentityHTDefaultSetting filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityHTDefaultSetting identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentityHTDefaultSetting identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityHTDefaultSetting GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }
       
        public List<IdentityHTDefaultSetting> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
