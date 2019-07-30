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

    public interface IStoreProject
    {
        List<IdentityProject> GetByPage(IdentityProject filter, int currentPage, int pageSize);
        IdentityProject GetById(int Id);
        int Insert(IdentityProject identity);
        bool Update(IdentityProject identity);
        bool Delete(int Id);

    }
     public class StoreProject : IStoreProject
    {
        private readonly string _connectionString;

        private RpsProject myRepository;

        public StoreProject() : this("DefaultConnection")
        {

        }

        public StoreProject(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            myRepository = new RpsProject(_connectionString);
        }

        public List<IdentityProject> GetByPage(IdentityProject filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public IdentityProject GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public int Insert(IdentityProject filter)
        {
            return myRepository.Insert(filter);
        }

        public bool Update (IdentityProject identity)
        {
            return myRepository.Update(identity);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }

        public bool Update(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
