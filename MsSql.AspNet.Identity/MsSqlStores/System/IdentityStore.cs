using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity.Repositories;


namespace MsSql.AspNet.Identity
{
    public class IdentityStore: IIdentityStore        
    {
        private readonly string _connectionString;
        private readonly IdentityRepository _identityRepository;
        
        public IdentityStore()
            : this("DefaultConnection")
        {

        }

        public IdentityStore(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _identityRepository = new IdentityRepository(_connectionString);
        }

        public virtual void Dispose()
        {
            // connection is automatically disposed
        }


        public List<IdentityUser> FilterUserList(string email, string roleId, bool isLocked,int page, int pageSize)
        {            
            var users = _identityRepository.FilterUserList(email, roleId, isLocked, page, pageSize);           
            return users.ToList();
        }

        public int CountAll(string email,string roleId, bool isLocked)
        {
            return _identityRepository.CountAll(email,roleId, isLocked);
        }

        public IdentityUser GetUserByID(string Id)
        {
            var userInfo = _identityRepository.GetById(Id);
            return userInfo;
        }

        public List<IdentityUser> GetListUser()
        {
            var users = _identityRepository.GetListUser();
            return users.ToList();
        }

        //public void UpdateProfile(IdentityUser user)
        //{
        //    _identityRepository.UpdateProfile(user);
        //}
    }

    public interface IIdentityStore
    {
        List<IdentityUser> FilterUserList(string email, string roleId, bool isLocked, int page, int pageSize);

        int CountAll(string email, string roleId, bool isLocked);

        IdentityUser GetUserByID(string Id);
        List<IdentityUser> GetListUser();
        //void UpdateProfile(IdentityUser user);
    }
}
