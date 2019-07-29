using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StoreProperty : IStoreProperty
    {
        private readonly string _connectionString;
        private RpsProperty myRepository;

        public StoreProperty()
            : this("DefaultConnection")
        {

        }

        public StoreProperty(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsProperty(_connectionString);
        }

        public List<IdentityProperty> GetByPage(IdentityProperty filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityProperty identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityProperty identity)
        {
            return myRepository.Update(identity);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public IdentityProperty GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityProperty GetDetail(int id)
        {
            return myRepository.GetDetail(id);
        }

        public List<IdentityPropertyLang> GetLangDetail(int id)
        {
            return myRepository.GetLangDetail(id);
        }

        public int InsertLang(IdentityPropertyLang identity)
        {
            return myRepository.InsertLang(identity);
        }

        public int UpdateLang(IdentityPropertyLang identity)
        {
            return myRepository.UpdateLang(identity);
        }

        public bool DeleteLang(int id)
        {
            return myRepository.DeleteLang(id);
        }

        public List<IdentityProperty> GetAll()
        {
            return myRepository.GetAll();
        }
    }

    public interface IStoreProperty
    {
        List<IdentityProperty> GetByPage(IdentityProperty filter, int currentPage, int pageSize);

        int Insert(IdentityProperty identity);
        bool Update(IdentityProperty identity);
        bool Delete(int id);

        IdentityProperty GetById(int id);
        IdentityProperty GetDetail(int id);

        List<IdentityPropertyLang> GetLangDetail(int id);
        int InsertLang(IdentityPropertyLang identity);
        int UpdateLang(IdentityPropertyLang identity);
        bool DeleteLang(int id);
        List<IdentityProperty> GetAll();
    }
}
