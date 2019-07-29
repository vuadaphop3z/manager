using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity;

namespace MsSql.AspNet.Identity
{
    public class MsSqlStore : IMsSqlStore
    {
        private readonly string _connectionString;
        private MsSqlRepository _myRepository;

        public MsSqlStore()
            : this("DefaultConnection")
        {

        }

        public MsSqlStore(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _myRepository = new MsSqlRepository(_connectionString);                        
        }


        #region Settings

        public virtual Task<List<Setting>> LoadSettings(string settingType)
        {
            if (string.IsNullOrEmpty(settingType))
            {
                throw new ArgumentNullException("settingType");
            }

            var settings = _myRepository.LoadSettings(settingType);
            return Task.FromResult<List<Setting>>(settings);
        }


        public virtual Task<bool> SaveSettings(List<Setting> settings)
        {
            if (settings == null || settings != null && settings.Count <= 0)
            {
                throw new ArgumentNullException("settings");
            }

            var rowsEffected = _myRepository.SaveSettings(settings);

            return Task.FromResult<bool>(rowsEffected == settings.Count);
        }

        #endregion       
    }

    public interface IMsSqlStore
    {      
        #region Settings

        Task<List<Setting>> LoadSettings(string settingType);

        Task<bool> SaveSettings(List<Setting> settings);

        #endregion       
    }
}
