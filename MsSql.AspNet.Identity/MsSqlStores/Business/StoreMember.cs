using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StoreMember : IStoreMember
    {
        private readonly string _connectionString;
        private RpsMember myRepository;

        public StoreMember()
            : this("SingleSignOnDB")
        {

        }

        public StoreMember(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsMember(_connectionString);
        }

        public List<IdentityMember> GetByPage(IdentityMember filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityMember identity, out int code)
        {
            return myRepository.Insert(identity, out code);
        }

        public bool Update(IdentityMember identity, out int code)
        {
            return myRepository.Update(identity, out code);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public IdentityMember GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public List<IdentityMember> GetList(string keyword)
        {
            return myRepository.GetList(keyword);
        }
    }

    public interface IStoreMember
    {
        List<IdentityMember> GetByPage(IdentityMember filter, int currentPage, int pageSize);

        int Insert(IdentityMember identity, out int code);
        bool Update(IdentityMember identity, out int code);
        bool Delete(int id);

        IdentityMember GetById(int id);
        List<IdentityMember> GetList(string keyword);
    }
}
