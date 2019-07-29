using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreCredit
    {
        List<IdentityCredit> GetAll(IdentityCredit filter, int currentPage, int pageSize);
        int Insert(IdentityCredit identity);
        bool Update(IdentityCredit identity);
        IdentityCredit GetById(int id);
        bool Delete(int id);
        List<IdentityCredit> GetList();
    }

    public class StoreCredit : IStoreCredit
    {
        private readonly string _connectionString;
        private RpsCredit myRepository;

        public StoreCredit(): this("DefaultConnection")
        {

        }

        public StoreCredit(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCredit(_connectionString);
        }

        #region  Common

        public List<IdentityCredit> GetAll(IdentityCredit filter, int currentPage, int pageSize)
        {
            return myRepository.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCredit identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityCredit identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCredit GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }
       
        public List<IdentityCredit> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
