using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsLanguage
    {
        private readonly string _connectionString;

        public RpsLanguage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsLanguage()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityLanguage> GetAll(IdentityLanguage filter, int currentPage, int pageSize)
        {           
            //Common syntax           
            var sqlCmd = @"Language_GetAll";
            List<IdentityLanguage> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", filter.Name },
                {"@LangCode", filter.LangCode },
                {"@Status", filter.Status },
                {"@TuNgay", filter.FromDate },
                {"@DenNgay", filter.ToDate },
                {"@Offset", offset},
                {"@PageSize", pageSize},
            };

            try
            {
                using(var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListLanguageFromReader(reader);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Language_GetAll. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityLanguage> ParsingListLanguageFromReader(IDataReader reader)
        {
            List<IdentityLanguage> listData = listData = new List<IdentityLanguage>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractLanguageData(reader);

                //Extends information
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityLanguage ExtractLanguageData(IDataReader reader)
        {
            var record = new IdentityLanguage();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.LangCode = reader["LangCode"].ToString();
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public int Insert(IdentityLanguage identity)
        {
            //Common syntax           
            var sqlCmd = @"Language_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@Name", identity.Name},
                //{"@Code", identity.Code },               
                //{"@CreatedBy", identity.CreatedBy},
                //{"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    newId = Convert.ToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Language_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityLanguage identity)
        {
            //Common syntax
            var sqlCmd = @"Language_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@Id", identity.Id},                
                //{"@Name", identity.Name},
                //{"@Code", identity.Code },                
                //{"@LastUpdatedBy", identity.LastUpdatedBy},
                //{"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Language_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityLanguage GetById(int Id)
        {
            var info = new IdentityLanguage();         
            var sqlCmd = @"Language_GetById";

            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            info = ExtractLanguageData(reader);
                        }
                    }
                }                            
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Language_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Language_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id},
            };

            try
            {
                using(var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Language_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityLanguage> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Language_GetList";

            List<IdentityLanguage> listData = new List<IdentityLanguage>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractLanguageData(reader);

                            listData.Add(record);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Language_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion
    }
}
