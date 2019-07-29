using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsHTDefaultSetting
    {
        private readonly string _connectionString;

        public RpsHTDefaultSetting(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsHTDefaultSetting()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityHTDefaultSetting> GetByPage(IdentityHTDefaultSetting filter, int currentPage, int pageSize)
        {           
            //Common syntax           
            var sqlCmd = @"HTDefault_Setting_GetByPage";
            List<IdentityHTDefaultSetting> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword },
                {"@Status", filter.Status },
                {"@Offset", offset},
                {"@PageSize", pageSize},
            };

            try
            {
                using(var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListHTDefaultSettingFromReader(reader);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute HTDefault_Setting_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityHTDefaultSetting> ParsingListHTDefaultSettingFromReader(IDataReader reader)
        {
            List<IdentityHTDefaultSetting> listData = listData = new List<IdentityHTDefaultSetting>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractHTDefaultSettingData(reader);

                //Extends information
                if(reader.HasColumn("TotalCount"))
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityHTDefaultSetting ExtractHTDefaultSettingData(IDataReader reader)
        {
            var record = new IdentityHTDefaultSetting();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();
            record.EnumValue = Utils.ConvertToInt32(reader["EnumValue"]);
            record.MaxLength = Utils.ConvertToInt32(reader["MaxLength"]);
            record.StartPosition = Utils.ConvertToInt32(reader["StartPosition"]);
            record.NumberOfCharacters = Utils.ConvertToInt32(reader["NumberOfCharacters"]);
            record.Status = Utils.ConvertToInt32(reader["Status"]);
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];

            return record;
        }

        public int Insert(IdentityHTDefaultSetting identity)
        {
            //Common syntax           
            var sqlCmd = @"HTDefault_Setting_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code },
                {"@EnumValue", identity.EnumValue },
                {"@MaxLength", identity.MaxLength },
                {"@StartPosition", identity.StartPosition },
                {"@NumberOfCharacters", identity.NumberOfCharacters },
                {"@Status", identity.Status}
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
                var strError = "Failed to execute HTDefault_Setting_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int Update(IdentityHTDefaultSetting identity)
        {
            //Common syntax
            var sqlCmd = @"HTDefault_Setting_Update";

            var returnNum = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Code", identity.Code },
                {"@EnumValue", identity.EnumValue },
                {"@MaxLength", identity.MaxLength },
                {"@StartPosition", identity.StartPosition },
                {"@NumberOfCharacters", identity.NumberOfCharacters },
                {"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    returnNum = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute HTDefault_Setting_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return returnNum;
        }

        public IdentityHTDefaultSetting GetById(int Id)
        {
            var info = new IdentityHTDefaultSetting();         
            var sqlCmd = @"HTDefault_Setting_GetById";

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
                            info = ExtractHTDefaultSettingData(reader);
                        }
                    }
                }                            
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute HTDefault_Setting_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"HTDefault_Setting_Delete";

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
                var strError = "Failed to execute HTDefault_Setting_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityHTDefaultSetting> GetList()
        {
            //Common syntax            
            var sqlCmd = @"HTDefault_Setting_GetList";

            List<IdentityHTDefaultSetting> listData = new List<IdentityHTDefaultSetting>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractHTDefaultSettingData(reader);

                            listData.Add(record);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute HTDefault_Setting_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion
    }
}
