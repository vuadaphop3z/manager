using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories.Business;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.MsSqlStores.Business
{

    public interface IStoreCompany
    {
        List<IdentityCompany> GetByPage(IdentityCompany filter, int currentPage, int pageSize);
        int Insert(IdentityCompany identity);
        bool Update(IdentityCompany identity);
        IdentityCompany GetById(int id);
        bool Delete(int id);
        List<IdentityCompany> GetList();

    }


    public class StoreCompany : IStoreCompany
    {

        private readonly string _connectionString;

        private RpsCompany myRepository;

        public StoreCompany() : this("DefaultConnection")
        {

        }

        public StoreCompany(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            myRepository = new RpsCompany(_connectionString);
        }


        public List<IdentityCompany> GetByPage(IdentityCompany filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }


        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }

        public IdentityCompany GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public int Insert(IdentityCompany identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityCompany identity)
        {
            return myRepository.Update(identity);
        }

        public List<IdentityCompany> GetList()
        {
            return myRepository.GetList();
        }
    }


}
