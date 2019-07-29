using System;
using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using Manager.SharedLib.Extensions;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreNavigation
    {
        List<IdentityNavigation> GetByPage(IdentityNavigation filter, int currentPage, int pageSize);
        List<IdentityNavigation> GetList();
        int Insert(IdentityNavigation identity);
        bool Update(IdentityNavigation identity);
        IdentityNavigation GetById(int id);
        bool Delete(int id);
        bool UpdateSorting(List<SortingElement> elements);
    }

    public class StoreNavigation : IStoreNavigation
    {
        private readonly string _connectionString;
        private RpsNavigation myRepository;

        public StoreNavigation() : this("PfoodDBConnection")
        {
        }

        public StoreNavigation(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsNavigation(_connectionString);
        }

        #region  Common

        public List<IdentityNavigation> GetByPage(IdentityNavigation filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityNavigation> GetList()
        {
            return myRepository.GetList();
        }


        public int Insert(IdentityNavigation identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityNavigation identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityNavigation GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public bool UpdateSorting(List<SortingElement> elements)
        {
            return myRepository.UpdateSorting(elements);
        }

        #endregion
    }
}
