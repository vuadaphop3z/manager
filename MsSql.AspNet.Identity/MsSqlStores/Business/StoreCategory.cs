using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StoreCategory : IStoreCategory
    {
        private readonly string _connectionString;
        private RpsCategory myRepository;

        public StoreCategory(): this("PfoodDBConnection")
        {

        }

        public StoreCategory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCategory(_connectionString);
        }

        #region  Common

        public List<IdentityCategory> GetAll(IdentityCategory filter, int currentPage, int pageSize)
        {
            return myRepository.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCategory identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityCategory identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCategory GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }
       
        public List<IdentityCategory> GetList()
        {
            return myRepository.GetList();
        }

        public IdentityCategory GetDetail(int id)
        {
            return myRepository.GetDetail(id);
        }

        public IdentityCategoryLang GetLangDetail(int id)
        {
            return myRepository.GetLangDetail(id);
        }

        public int AddNewLang(IdentityCategoryLang identity)
        {
            return myRepository.AddNewLang(identity);
        }

        public int UpdateLang(IdentityCategoryLang identity)
        {
            return myRepository.UpdateLang(identity);
        }

        public bool DeleteLang(int id)
        {
            return myRepository.DeleteLang(id);
        }

        #endregion
    }

    public interface IStoreCategory
    {
        List<IdentityCategory> GetAll(IdentityCategory filter, int currentPage, int pageSize);
        int Insert(IdentityCategory identity);
        bool Update(IdentityCategory identity);
        IdentityCategory GetById(int id);
        bool Delete(int id);
        List<IdentityCategory> GetList();
        IdentityCategory GetDetail(int id);

        int AddNewLang(IdentityCategoryLang identity);
        int UpdateLang(IdentityCategoryLang identity);
        IdentityCategoryLang GetLangDetail(int id);
        bool DeleteLang(int id);
    }
}
