using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StoreGroupProperty : IStoreGroupProperty
    {
        private readonly string _connectionString;
        private RpsGroupProperty myRepository;

        public StoreGroupProperty()
            : this("PfoodDBConnection")
        {

        }

        public StoreGroupProperty(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsGroupProperty(_connectionString);
        }

        public List<IdentityGroupProperty> GetByPage(IdentityGroupProperty filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityGroupProperty identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityGroupProperty identity)
        {
            return myRepository.Update(identity);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public IdentityGroupProperty GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityGroupProperty GetDetail(int id)
        {
            return myRepository.GetDetail(id);
        }

        public List<IdentityGroupPropertyLang> GetLangDetail(int id)
        {
            return myRepository.GetLangDetail(id);
        }

        public int AddNewLang(IdentityGroupPropertyLang identity)
        {
            return myRepository.AddNewLang(identity);
        }

        public int UpdateLang(IdentityGroupPropertyLang identity)
        {
            return myRepository.UpdateLang(identity);
        }

        public bool DeleteLang(int id)
        {
            return myRepository.DeleteLang(id);
        }

        public List<IdentityGroupProperty> GetAll()
        {
            return myRepository.GetAll();
        }
    }

    public interface IStoreGroupProperty
    {
        List<IdentityGroupProperty> GetByPage(IdentityGroupProperty filter, int currentPage, int pageSize);

        int Insert(IdentityGroupProperty identity);
        bool Update(IdentityGroupProperty identity);
        bool Delete(int id);

        IdentityGroupProperty GetById(int id);
        IdentityGroupProperty GetDetail(int id);

        List<IdentityGroupPropertyLang> GetLangDetail(int id);
        int AddNewLang(IdentityGroupPropertyLang identity);
        int UpdateLang(IdentityGroupPropertyLang identity);
        bool DeleteLang(int id);
        List<IdentityGroupProperty> GetAll();
    }
}
