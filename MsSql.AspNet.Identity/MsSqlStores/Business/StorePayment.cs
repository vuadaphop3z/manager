using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStorePayment
    {
        List<IdentityPayment> GetAll(IdentityPayment filter, int currentPage, int pageSize);
        int Insert(IdentityPayment identity);
        bool Update(IdentityPayment identity);
        IdentityPayment GetById(int id);
        bool Delete(int id);
        List<IdentityPayment> GetList();
    }

    public class StorePayment : IStorePayment
    {
        private readonly string _connectionString;
        private RpsPayment myRepository;

        public StorePayment(): this("DefaultConnection")
        {

        }

        public StorePayment(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsPayment(_connectionString);
        }

        #region  Common

        public List<IdentityPayment> GetAll(IdentityPayment filter, int currentPage, int pageSize)
        {
            return myRepository.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPayment identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityPayment identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityPayment GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }
       
        public List<IdentityPayment> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
