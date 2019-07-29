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




    }
}
