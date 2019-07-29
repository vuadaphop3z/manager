using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity.Repositories;

namespace MsSql.AspNet.Identity
{
    public class RoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : IdentityRole
    {
        private readonly string _connectionString;
        private readonly RoleRepository _roleRepository;
        public RoleStore()
            : this("DefaultConnection")
        {

        }

        public RoleStore(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _roleRepository = new RoleRepository(_connectionString);
        }


        /// <summary>
        /// Get all Roles defined.
        /// This code is a loose implementation.
        /// An occurrence of a performance problem is when you get a large amount of data.
        /// </summary>
        public IQueryable<TRole> Roles
        {
            get {
                return (IQueryable<TRole>)_roleRepository.GetAll();
            }
        }

        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _roleRepository.Insert(role);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            _roleRepository.Delete(role.Id);

            return Task.FromResult<Object>(null);
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
            var result = _roleRepository.GetRoleById(roleId) as TRole;

            return Task.FromResult(result);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            var result = _roleRepository.GetRoleByName(roleName) as TRole;
            return Task.FromResult(result);
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            _roleRepository.Update(role);

            return Task.FromResult<Object>(null);
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }
    }
}
