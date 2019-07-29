using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Collections.Generic;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StorePlaceTypeGroup : IStorePlaceTypeGroup
    {
        private readonly string _connectionString;
        private RpsPlaceTypeGroup myRepository;

        public StorePlaceTypeGroup()
            : this("PfoodDBConnection")
        {

        }

        public StorePlaceTypeGroup(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsPlaceTypeGroup(_connectionString);
        }

        public List<IdentityPlaceTypeGroup> GetAll(IdentityPlaceTypeGroup filter, int currentPage, int pageSize)
        {
            return myRepository.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPlaceTypeGroup identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityPlaceTypeGroup identity)
        {
            return myRepository.Update(identity);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public IdentityPlaceTypeGroup GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityPlaceTypeGroup GetDetail(int id)
        {
            return myRepository.GetDetail(id);
        }

        public IdentityPlaceTypeGroupLang GetLangDetail(int id)
        {
            return myRepository.GetLangDetail(id);
        }

        public int AddNewLang(IdentityPlaceTypeGroupLang identity)
        {
            return myRepository.AddNewLang(identity);
        }

        public int UpdateLang(IdentityPlaceTypeGroupLang identity)
        {
            return myRepository.UpdateLang(identity);
        }

        public bool DeleteLang(int id)
        {
            return myRepository.DeleteLang(id);
        }
    }

    public interface IStorePlaceTypeGroup
    {
        List<IdentityPlaceTypeGroup> GetAll(IdentityPlaceTypeGroup filter, int currentPage, int pageSize);

        int Insert(IdentityPlaceTypeGroup identity);
        bool Update(IdentityPlaceTypeGroup identity);
        bool Delete(int id);

        IdentityPlaceTypeGroup GetById(int id);
        IdentityPlaceTypeGroup GetDetail(int id);

        IdentityPlaceTypeGroupLang GetLangDetail(int id);
        int AddNewLang(IdentityPlaceTypeGroupLang identity);
        int UpdateLang(IdentityPlaceTypeGroupLang identity);
        bool DeleteLang(int id);
    }
}
