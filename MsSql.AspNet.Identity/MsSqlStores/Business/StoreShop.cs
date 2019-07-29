using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreShop
    {
        List<IdentityShop> GetByPage(IdentityShop filter, int currentPage, int pageSize);
        int Insert(IdentityShop identity);
        bool Update(IdentityShop identity);
        IdentityShop GetById(int id);
        bool Delete(int id);
        List<IdentityShop> GetList(string keyword);
        List<IdentityShop> GetAssigned(int userId);
        bool UpdateMap(int shopId, string latitude, string longitude);
        int AssignToUser(int shopId, int userId, bool isOwner);
        bool DeleteAssignedUser(int shopId, int userId);
        List<IdentityShopMember> GetAssignedMembers(int shopId);
        int AssignCategory(int shopId, int categoryId);

        #region Meta data

        #region Images

        List<MetaShopImage> GetListImage(int Id);
        bool AddNewImage(MetaShopImage identity);
        bool RemoveImage(string Id);

        #endregion

        #region Amenities

        bool UpdateAmenities(int shopId, string selectedValues);

        #endregion

        #region NearPlaces

        string GetNearPlaces(int shopId);
        bool UpdateNearPlaces(int shopId, string nearPlaces);

        #endregion

        #region Policies

        string GetPolicies(int shopId);
        bool UpdatePolicies(int shopId, string policies);

        #endregion

        #region Payments

        bool UpdatePayments(int shopId, string selectedValues);

        #endregion

        #endregion
    }

    public class StoreShop : IStoreShop
    {
        private readonly string _connectionString;
        private RpsShop myRepository;

        public StoreShop() : this("PfoodDBConnection")
        {

        }

        public StoreShop(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsShop(_connectionString);
        }

        public List<IdentityShop> GetByPage(IdentityShop filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityShop identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityShop identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityShop GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }

        public List<IdentityShop> GetList(string keyword)
        {
            return myRepository.GetList(keyword);
        }

        public bool UpdateMap(int shopId, string latitude, string longitude)
        {
            return myRepository.UpdateMap(shopId, latitude, longitude);
        }

        public int AssignToUser(int shopId, int userId, bool isOwner)
        {
            return myRepository.AssignToUser(shopId, userId, isOwner);
        }

        public List<IdentityShop> GetAssigned(int userId)
        {
            return myRepository.GetAssigned(userId);
        }

        public bool DeleteAssignedUser(int shopId, int userId)
        {
            return myRepository.DeleteAssignedUser(shopId, userId);
        }

        public List<IdentityShopMember> GetAssignedMembers(int shopId)
        {
            return myRepository.GetAssignedMembers(shopId);
        }

        public int AssignCategory(int shopId, int categoryId)
        {
            return myRepository.AssignCategory(shopId, categoryId);
        }

        #region Meta data

        #region Images

        public bool AddNewImage(MetaShopImage identity)
        {
            return myRepository.AddNewImage(identity);
        }

        public bool RemoveImage(string shopId)
        {
            return myRepository.RemoveImage(shopId);
        }

        public List<MetaShopImage> GetListImage(int shopId)
        {
            return myRepository.GetListImage(shopId);
        }

        #endregion

        #region Amenities

        public bool UpdateAmenities(int shopId, string selectedValues)
        {
            return myRepository.UpdateAmenities(shopId, selectedValues);
        }

        #endregion

        #region NearPlaces

        public string GetNearPlaces(int shopId)
        {
            return myRepository.GetNearPlaces(shopId);
        }

        public bool UpdateNearPlaces(int shopId, string nearPlaces)
        {
            return myRepository.UpdateNearPlaces(shopId, nearPlaces);
        }

        #endregion

        #region Policies

        public string GetPolicies(int shopId)
        {
            return myRepository.GetPolicies(shopId);
        }

        public bool UpdatePolicies(int shopId, string policies)
        {
            return myRepository.UpdatePolicies(shopId, policies);
        }

        #endregion

        #region Payments

        public bool UpdatePayments(int shopId, string selectedValues)
        {
            return myRepository.UpdatePayments(shopId, selectedValues);
        }

        #endregion

        #endregion
    }
}
