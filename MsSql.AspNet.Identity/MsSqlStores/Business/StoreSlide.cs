using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreSlide
    {
        List<IdentitySlide> GetByPage(IdentitySlide filter, int currentPage, int pageSize);
        List<IdentitySlide> GetAll(IdentitySlide filter, int currentPage, int pageSize);
        List<IdentitySlideItem> GetAllSlideItemBySlide(int slideId);
        int Insert(IdentitySlide identity);
        bool Update(IdentitySlide identity);
        IdentitySlide GetById(int id);
        bool Delete(int id);
        List<IdentitySlide> GetList();

        #region Slide Item

        int InsertSlideItem(IdentitySlideItem identity);
        bool UpdateSlideItem(IdentitySlideItem identity);
        IdentitySlideItem GetSlideItemById(int id);
        bool DeleteSlideItem(int id);

        #endregion
    }

    public class StoreSlide : IStoreSlide
    {
        private readonly string _connectionString;
        private RpsSlide myRepository;

        public StoreSlide(): this("PfoodDBConnection")
        {

        }

        public StoreSlide(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsSlide(_connectionString);
        }

        #region  Common

        public List<IdentitySlide> GetByPage(IdentitySlide filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentitySlide> GetAll(IdentitySlide filter, int currentPage, int pageSize)
        {
            return myRepository.GetAll(filter, currentPage, pageSize);
        }

        public List<IdentitySlideItem> GetAllSlideItemBySlide(int slideId)
        {
            return myRepository.GetAllSlideItemBySlide(slideId);
        }

        public int Insert(IdentitySlide identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentitySlide identity)
        {
            return myRepository.Update(identity);
        }

        public IdentitySlide GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }
       
        public List<IdentitySlide> GetList()
        {
            return myRepository.GetList();
        }

        #endregion

        #region Slide Item

        public int InsertSlideItem(IdentitySlideItem identity)
        {
            return myRepository.InsertSlideItem(identity);
        }

        public bool UpdateSlideItem(IdentitySlideItem identity)
        {
            return myRepository.UpdateSlideItem(identity);
        }

        public IdentitySlideItem GetSlideItemById(int id)
        {
            return myRepository.GetSlideItemById(id);
        }
        
        public bool DeleteSlideItem(int id)
        {
            return myRepository.DeleteSlideItem(id);
        }

        #endregion
    }
}
