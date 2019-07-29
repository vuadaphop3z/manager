using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Stores
{
    public class StorePost : IStorePost
    {
        private readonly string _connectionString;
        private RpsPost myRepository;

        public StorePost()
            : this("PfoodDBConnection")
        {

        }

        public StorePost(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsPost(_connectionString);
        }

        #region --- Post ---
        public List<IdentityPost> GetByPage(IdentityPost filter)
        {
            return myRepository.GetByPage(filter);
        }

        public IdentityPost GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public int Insert(IdentityPost identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentityPost identity)
        {
            return myRepository.Update(identity);
        }

        public int Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public bool AssignCategory(int postId, int catId)
        {
            return myRepository.AssignCategory(postId, catId);
        }

        #endregion
    }

    public interface IStorePost
    {
        List<IdentityPost> GetByPage(IdentityPost identity);

        IdentityPost GetById(int id);

        int Insert(IdentityPost identity);

        int Update(IdentityPost identity);

        int Delete(int id);

        bool AssignCategory(int postId, int catId);
    }
}
