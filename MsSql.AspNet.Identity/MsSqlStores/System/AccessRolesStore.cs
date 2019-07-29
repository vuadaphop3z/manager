using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;
using Manager.SharedLib.Extensions;

namespace MsSql.AspNet.Identity
{
    public class AccessRolesStore : IAccessRolesStore
    {
        private readonly string _connectionString;
        private readonly AccessRolesRepository _identityRepository;

        public AccessRolesStore()
            : this("DefaultConnection")
        {

        }

        public AccessRolesStore(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _identityRepository = new AccessRolesRepository(_connectionString);
        }

        public virtual void Dispose()
        {
            // connection is automatically disposed
        }


        public List<IdentityAccess> GetAllAccess()
        {
            var list = _identityRepository.GetAllAccess();
            return list.ToList();
        }

        public List<IdentityOperation> GetAllOperations()
        {
            var list = _identityRepository.GetAllOperations();
            return list.ToList();
        }

        public List<IdentityAccessRoles> GetAccessRolesByAccessId(string AccessId)
        {
            var list = _identityRepository.GetAccessRolesByAccessId(AccessId);
            return list.ToList();
        }

        public List<IdentityAccessRoles> GetPermissionByRoleId(string RoleId)
        {
            var list = _identityRepository.GetPermissionByRoleId(RoleId);
            return list.ToList();
        }

        public List<IdentityAccessRoles> GetAccessByRoleId(string RoleId)
        {
            var list = _identityRepository.GetAccessByRoleId(RoleId);
            return list.ToList();
        }

        public List<IdentityOperation> GetListOperationNotUse()
        {
            var list = _identityRepository.GetListOperationNotUse();
            return list.ToList();
        }
        public List<IdentityOperation> GetOperationsByAccessId(string AccessId)
        {
            var list = _identityRepository.GetOperationsByAccessId(AccessId);
            return list.ToList();
        }
        public void DeleteAllAccessOfRole(string RoleId)
        {
            _identityRepository.DeleteAllAccessOfRole(RoleId);
        }

        public bool UpdateAccessOfRole(string[] operations, string RoleId)
        {
            return _identityRepository.UpdateAccessOfRole(operations, RoleId);
        }

        public bool CheckPermission(string UserId, string AccessName, string ActionName)
        {
            return _identityRepository.CheckPermission(UserId, AccessName, ActionName);
        }

        public List<IdentityPermission> GetPermissionsByUser(string UserId)
        {
            return _identityRepository.GetPermissionsByUser(UserId);
        }

        public List<IdentityMenu> GetRootMenuByUserId(string UserId)
        {
            var list = _identityRepository.GetRootMenuByUserId(UserId);
            return list.ToList();
        }

        public List<IdentityMenu> GetChildMenuByUserId(string UserId, int ParentId)
        {
            var list = _identityRepository.GetChildMenuByUserId(UserId, ParentId);
            return list.ToList();
        }
        //public int CountAll(string email,string roleId)
        //{
        //    return _identityRepository.CountAll(email,roleId);
        //}

        //public IdentityUser GetUserByID(string Id)
        //{
        //    var userInfo = _identityRepository.GetById(Id);
        //    return userInfo;
        //}

        public List<IdentityMenu> GetAllMenus()
        {
            var list = _identityRepository.GetAllMenus();
            return list.ToList();
        }

        #region Menu
        public string InsertMenu(IdentityMenu identity)
        {
            return _identityRepository.InsertMenu(identity);
        }

        public string UpdateMenu(IdentityMenu identity)
        {
            return _identityRepository.UpdateMenu(identity);
        }

        public IdentityMenu GetMenuById(int id)
        {
            return _identityRepository.GetMenuById(id);
        }

        public void DeleteMenu(int id)
        {
            _identityRepository.DeleteMenu(id);
        }

        public IdentityMenu GetMenuDetail(int id)
        {
            return _identityRepository.GetMenuDetail(id);
        }

        public bool UpdateSorting(List<SortingElement> elements)
        {
            return _identityRepository.UpdateSorting(elements);
        }

        public int AddNewLang(IdentityMenuLang identity)
        {
            return _identityRepository.AddNewLang(identity);
        }

        public int UpdateLang(IdentityMenuLang identity)
        {
            return _identityRepository.UpdateLang(identity);
        }

        public bool DeleteLang(int Id)
        {
            return _identityRepository.DeleteLang(Id);
        }

        public IdentityMenuLang GetLangDetail(int id)
        {
            return _identityRepository.GetLangDetail(id);
        }

        #endregion

        #region Access

        public bool DeleteAccess(string AccessId)
        {
            return _identityRepository.DeleteAccess(AccessId);
        }

        public bool CheckAccessDuplicate(IdentityAccess identity)
        {
            return _identityRepository.CheckAccessDuplicate(identity);
        }

        public bool CreateAccess(IdentityAccess identity)
        {
            return _identityRepository.CreateAccess(identity);
        }

        public bool UpdateAccess(IdentityAccess identity)
        {
            return _identityRepository.UpdateAccess(identity);
        }

        #endregion

        #region  Operations
        public bool DeleteOperation(int Id)
        {
            return _identityRepository.DeleteOperation(Id);
        }

        public bool CheckOperationDuplicate(IdentityOperation identity)
        {
            return _identityRepository.CheckOperationDuplicate(identity);
        }

        public bool CreateOperation(IdentityOperation identity)
        {
            return _identityRepository.CreateOperation(identity);
        }

        public bool UpdateOperation(IdentityOperation identity)
        {
            return _identityRepository.UpdateOperation(identity);
        }
        #endregion
    }

    public interface IAccessRolesStore
    {
        List<IdentityAccess> GetAllAccess();

        List<IdentityOperation> GetAllOperations();

        List<IdentityAccessRoles> GetAccessRolesByAccessId(string AccessId);

        List<IdentityAccessRoles> GetPermissionByRoleId(string RoleId);

        List<IdentityAccessRoles> GetAccessByRoleId(string RoleId);

        List<IdentityOperation> GetOperationsByAccessId(string AccessId);
        List<IdentityOperation> GetListOperationNotUse();
        
        void DeleteAllAccessOfRole(string RoleId);

        bool UpdateAccessOfRole(string[] operations, string RoleId);

        bool CheckPermission(string userId, string AccessName, string ActionName);

        List<IdentityPermission> GetPermissionsByUser(string UserId);

        List<IdentityMenu> GetRootMenuByUserId(string UserId);

        List<IdentityMenu> GetChildMenuByUserId(string UserId, int ParentId);

        List<IdentityMenu> GetAllMenus();
        #region Menu
        string InsertMenu(IdentityMenu identity);
        string UpdateMenu(IdentityMenu identity);
        IdentityMenu GetMenuById(int Id);
        void DeleteMenu(int Id);
        bool UpdateSorting(List<SortingElement> elements);

        int AddNewLang(IdentityMenuLang identity);
        int UpdateLang(IdentityMenuLang identity);
        bool DeleteLang(int Id);
        IdentityMenu GetMenuDetail(int id);
        IdentityMenuLang GetLangDetail(int id);

        #endregion

        #region Access
        bool DeleteAccess(string AccessId);

        bool UpdateAccess(IdentityAccess identity);

        bool CheckAccessDuplicate(IdentityAccess identity);

        bool CreateAccess(IdentityAccess identity);
        #endregion

        #region Operation
        bool DeleteOperation(int Id);

        bool UpdateOperation(IdentityOperation identity);

        bool CheckOperationDuplicate(IdentityOperation identity);

        bool CreateOperation(IdentityOperation identity);
        #endregion
    }
}
