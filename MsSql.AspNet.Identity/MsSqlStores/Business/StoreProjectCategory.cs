using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.MsSqlStores
{

    public interface IStoreProjectCategory
    {
        List<IdentityProjectCategory> GetByPage(IdentityProjectCategory filter, int currentPage, int pageSize);
        IdentityProjectCategory GetById(int Id);
        int Insert(IdentityProjectCategory identity);
        bool Update(IdentityProjectCategory identity);
        bool Delete(int Id);
        List<IdentityProjectCategory> GetList();


    }
    public class StoreProjectCategory : IStoreProjectCategory
    {
        private readonly string _connectionString;
        private RpsProjectCategory myRepository;

        public StoreProjectCategory() : this("DefaultConnection")
        {
            
        }
        public StoreProjectCategory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            myRepository = new RpsProjectCategory(_connectionString);

        }

        public List<IdentityProjectCategory> GetByPage(IdentityProjectCategory filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public IdentityProjectCategory GetById(int Id)
        {
            return myRepository.GetById(Id);
        }


       public int Insert(IdentityProjectCategory identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityProjectCategory identity)
        {
            return myRepository.Update(identity);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }

        public List<IdentityProjectCategory> GetList()
        {
            return myRepository.GetList();
        }
    }
}
