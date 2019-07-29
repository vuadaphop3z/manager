using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreUnit
    {
        List<IdentityUnit> GetByPage(IdentityUnit filter, int currentPage, int pageSize);
        int Insert(IdentityUnit identity);
        bool Update(IdentityUnit identity);
        IdentityUnit GetById(int id);
        bool Delete(int id);
        List<IdentityUnit> GetList();
    }

    public class StoreUnit : IStoreUnit
    {
        private readonly string _connectionString;
        private RpsUnit myRepository;

        public StoreUnit(): this("DefaultConnection")
        {

        }

        public StoreUnit(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsUnit(_connectionString);
        }

        #region  Common

        public List<IdentityUnit> GetByPage(IdentityUnit filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityUnit identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityUnit identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityUnit GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }
       
        public List<IdentityUnit> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
