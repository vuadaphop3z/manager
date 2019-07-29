using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StorePriceType : IStorePriceType
    {
        private readonly string _connectionString;
        private RpsPriceType myRepository;

        public StorePriceType()
            : this("PfoodDBConnection")
        {

        }

        public StorePriceType(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsPriceType(_connectionString);
        }

        public List<IdentityPriceType> GetAll(IdentityPriceType filter, int currentPage, int pageSize)
        {
            return myRepository.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPriceType identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityPriceType identity)
        {
            return myRepository.Update(identity);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public IdentityPriceType GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityPriceType GetDetail(int id)
        {
            return myRepository.GetDetail(id);
        }

        public IdentityPriceTypeLang GetLangDetail(int id)
        {
            return myRepository.GetLangDetail(id);
        }

        public int AddNewLang(IdentityPriceTypeLang identity)
        {
            return myRepository.AddNewLang(identity);
        }

        public int UpdateLang(IdentityPriceTypeLang identity)
        {
            return myRepository.UpdateLang(identity);
        }

        public bool DeleteLang(int id)
        {
            return myRepository.DeleteLang(id);
        }
    }

    public interface IStorePriceType
    {
        List<IdentityPriceType> GetAll(IdentityPriceType filter, int currentPage, int pageSize);

        int Insert(IdentityPriceType identity);
        bool Update(IdentityPriceType identity);
        bool Delete(int id);

        IdentityPriceType GetById(int id);
        IdentityPriceType GetDetail(int id);

        IdentityPriceTypeLang GetLangDetail(int id);
        int AddNewLang(IdentityPriceTypeLang identity);
        int UpdateLang(IdentityPriceTypeLang identity);
        bool DeleteLang(int id);
    }
}
