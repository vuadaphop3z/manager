using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;
using MsSql.AspNet.Identity.MsSqlStores.Business;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreArea
    {
        #region Area

        bool Area_Insert(IdentityArea identity);
        bool Area_Update(IdentityArea identity);
        IdentityArea Area_GetById(int id);
        bool Area_Delete(int id);
        List<IdentityArea> Area_GetList();

        #endregion

        #region Country

        bool Country_Insert(IdentityCountry identity);
        bool Country_Update(IdentityCountry identity);
        IdentityCountry Country_GetById(int id);
        bool Country_Delete(int id);
        List<IdentityCountry> Country_GetList();

        List<IdentityCountry> Country_GetByArea(int areaId);

        #endregion

        #region Province

        bool Province_Insert(IdentityProvince identity);
        bool Province_Update(IdentityProvince identity);
        IdentityProvince Province_GetById(int id);
        bool Province_Delete(int id);
        List<IdentityProvince> Province_GetList();

        List<IdentityProvince> Province_GetByCountry(int countryId);

        #endregion

        #region District

        bool District_Insert(IdentityDistrict identity);
        bool District_Update(IdentityDistrict identity);
        IdentityDistrict District_GetById(int id);
        bool District_Delete(int id);
        List<IdentityDistrict> District_GetList();

        List<IdentityDistrict> District_GetByProvince(int provinceId);

        #endregion
    }

    public class StoreArea : IStoreArea
    {
        private readonly string _connectionString;
        private RpsArea myRepository;

        public StoreArea() : this("DefaultConnection")
        {

        }

        public StoreArea(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsArea(_connectionString);
        }

        #region Area

        public bool Area_Insert(IdentityArea identity)
        {
            return myRepository.Area_Insert(identity);
        }

        public bool Area_Update(IdentityArea identity)
        {
            return myRepository.Area_Update(identity);
        }

        public IdentityArea Area_GetById(int Id)
        {
            return myRepository.Area_GetById(Id);
        }

        public bool Area_Delete(int Id)
        {
            return myRepository.Area_Delete(Id);
        }

        public List<IdentityArea> Area_GetList()
        {
            return myRepository.Area_GetList();
        }

        #endregion

        #region Country

        public bool Country_Insert(IdentityCountry identity)
        {
            return myRepository.Country_Insert(identity);
        }

        public bool Country_Update(IdentityCountry identity)
        {
            return myRepository.Country_Update(identity);
        }

        public IdentityCountry Country_GetById(int Id)
        {
            return myRepository.Country_GetById(Id);
        }

        public bool Country_Delete(int Id)
        {
            return myRepository.Country_Delete(Id);
        }

        public List<IdentityCountry> Country_GetList()
        {
            return myRepository.Country_GetList();
        }

        public List<IdentityCountry> Country_GetByArea(int areaId)
        {
            return myRepository.Country_GetByArea(areaId);
        }

        #endregion

        #region Province

        public bool Province_Insert(IdentityProvince identity)
        {
            return myRepository.Province_Insert(identity);
        }

        public bool Province_Update(IdentityProvince identity)
        {
            return myRepository.Province_Update(identity);
        }

        public IdentityProvince Province_GetById(int Id)
        {
            return myRepository.Province_GetById(Id);
        }

        public bool Province_Delete(int Id)
        {
            return myRepository.Province_Delete(Id);
        }

        public List<IdentityProvince> Province_GetList()
        {
            return myRepository.Province_GetList();
        }

        public List<IdentityProvince> Province_GetByCountry(int countryId)
        {
            return myRepository.Province_GetByCountry(countryId);
        }

        #endregion

        #region District

        public bool District_Insert(IdentityDistrict identity)
        {
            return myRepository.District_Insert(identity);
        }

        public bool District_Update(IdentityDistrict identity)
        {
            return myRepository.District_Update(identity);
        }

        public IdentityDistrict District_GetById(int Id)
        {
            return myRepository.District_GetById(Id);
        }

        public bool District_Delete(int Id)
        {
            return myRepository.District_Delete(Id);
        }

        public List<IdentityDistrict> District_GetList()
        {
            return myRepository.District_GetList();
        }

        public List<IdentityDistrict> District_GetByProvince(int provinceId)
        {
            return myRepository.District_GetByProvince(provinceId);
        }

        #endregion
    }
}
