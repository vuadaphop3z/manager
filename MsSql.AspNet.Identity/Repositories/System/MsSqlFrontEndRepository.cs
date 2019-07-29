using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Manager.SharedLib.Extensions;
using System.Text;

namespace MsSql.AspNet.Identity.Repositories
{
    public class MsSqlFrontEndRepository
    {
        private readonly string _connectionString;
        public MsSqlFrontEndRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MsSqlFrontEndRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }


        #region Settings

        public List<Setting> LoadSettings(string settingType)
        {
            List<Setting> listReturn = null;
            using (var conn = new SqlConnection(_connectionString))
            {

                var parameters = new Dictionary<string, object>
                {
                    {"@pType", settingType}
                };

                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure,
                    @"Settings_LoadSettings", parameters);

                while (reader.Read())
                {
                    if (listReturn == null)
                        listReturn = new List<Setting>();

                    var entity = FillDataSeting(reader);

                    listReturn.Add(entity);
                }
            }
            return listReturn;
        }



        private Setting FillDataSeting(IDataReader reader)
        {
            return new Setting()
            {
                Name = reader["SettingName"].ToString(),
                Type = reader["SettingType"].ToString(),
                Value = reader["SettingValue"].ToString(),
            };
        }

        public int SaveSettings(List<Setting> settings)
        {
            //Common syntax            
            var sqlCmd = new StringBuilder();

            try
            {
                if(settings != null && settings.Count > 0)
                {
                    sqlCmd.Append(string.Format("DELETE FROM cmn_settings WHERE 1=1 AND SettingType = '{0}'; ", settings[0].Type));
                    foreach (var st in settings)
                    {
                        sqlCmd.Append(string.Format("INSERT INTO cmn_settings(SettingName, SettingType, SettingValue) VALUES (N'{0}',N'{1}',N'{2}'); ", st.Name, st.Type, st.Value));
                    }
                }

                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlCmd.ToString(), null);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute SaveSettings. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return 1;
        }

        #endregion

        #region Helpers

        public long CountQuery(string strQuery)
         {
             using (var conn = new SqlConnection(_connectionString))
             {
                 string sqlCount = string.Format("SELECT COUNT(1) FROM ({0}) a", strQuery);

                 var result = MsSqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount, null);
                 if (result != null)
                 {
                     return Convert.ToInt64(result.ToString());
                 }
             }

             return 0;
         }

         public string BuildPagingQuery(string strQuery, int pageNo, int pageSize, out long totalCount)
         {
             //This checks to see if there is a page number. If not, it will set it to page 1 
             int pagenum = pageNo;
             if (pagenum < 1)
             {
                 pagenum = 1;
             }

             //Build the query statement
             string sqlQuery = strQuery;

             //Here we count the number of results             
             totalCount = CountQuery(sqlQuery);

             var maxQuery = "";

             if (totalCount > 0)
             {
                 //This is the number of results displayed per page 
                 var page_rows = pageSize;
                 if (page_rows <= 0)
                 {
                     page_rows = 20;
                 }

                 //This tells us the page number of our last page 
                 var last = (int)Math.Ceiling((double)totalCount / page_rows);

                 //this makes sure the page number isn't below one, or more than our maximum pages 
                 if (pagenum < 1)
                 {
                     pagenum = 1;
                 }
                 else
                 {
                     if (pagenum > last)
                     {
                         pagenum = last;
                     }
                 }

                 // Determine the number of records to skip
                 int skip = (pagenum - 1) * page_rows;

                 //This sets the range to display in our query 
                 maxQuery = " LIMIT " + skip.ToString() + ',' + page_rows.ToString();
             }

             return sqlQuery + maxQuery;
         }

        #endregion
    }
}
