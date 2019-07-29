using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Collections.Generic;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StorePlace : IStorePlace
    {
        private readonly string _connectionString;
        private RpsPlace myRepository;

        public StorePlace()
            : this("PfoodDBConnection")
        {

        }

        public StorePlace(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsPlace(_connectionString);
        }

        public List<IdentityPlace> GetAll(IdentityPlace filter, int currentPage, int pageSize)
        {
            return myRepository.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPlace identity, string language)
        {
            return myRepository.Insert(identity, language);
        }

        public bool Update(IdentityPlace identity)
        {
            return myRepository.Update(identity);
        }

        public List<IdentityPlace> GetByProvince(int provinceId)
        {
            return myRepository.GetByProvince(provinceId);
        }

        public IdentityPlace GetPlaceById(int Id, bool getShort = false)
        {
            return myRepository.GetPlaceById(Id, getShort);
        }

        public List<IdentityPlace> GetFromList(string placesList)
        {
            return myRepository.GetFromList(placesList);
        }
    }
    public interface IStorePlace
    {
        List<IdentityPlace> GetAll(IdentityPlace filter, int currentPage, int pageSize);

        int Insert(IdentityPlace identity, string language);
        bool Update(IdentityPlace identity);

        List<IdentityPlace> GetByProvince(int provinceId);
        IdentityPlace GetPlaceById(int Id, bool getShort = false);
        List<IdentityPlace> GetFromList(string placesList);
    }
}
