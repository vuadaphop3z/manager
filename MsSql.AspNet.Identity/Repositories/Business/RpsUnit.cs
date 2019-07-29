using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsUnit
    {
        private readonly string _connectionString;

        public RpsUnit(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsUnit()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityUnit> GetByPage(IdentityUnit filter, int currentPage, int pageSize)
        {           
            //Common syntax           
            var sqlCmd = @"Unit_GetByPage";
            List<IdentityUnit> listData = null;

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
                        listData = ParsingListUnitFromReader(reader);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Unit_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityUnit> ParsingListUnitFromReader(IDataReader reader)
        {
            List<IdentityUnit> listData = listData = new List<IdentityUnit>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractUnitData(reader);

                //Extends information
                if(reader.HasColumn("TotalCount"))
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityUnit ExtractUnitData(IDataReader reader)
        {
            var record = new IdentityUnit();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();

            //record.CreatedBy = reader["CreatedBy"].ToString();
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            //record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            //record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public int Insert(IdentityUnit identity)
        {
            //Common syntax           
            var sqlCmd = @"Unit_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code },               
                //{"@CreatedBy", identity.CreatedBy},
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
                var strError = "Failed to execute Unit_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityUnit identity)
        {
            //Common syntax
            var sqlCmd = @"Unit_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},                
                {"@Name", identity.Name},
                {"@Code", identity.Code },                
                //{"@LastUpdatedBy", identity.LastUpdatedBy},
                {"@Status", identity.Status}
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
                var strError = "Failed to execute Unit_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityUnit GetById(int Id)
        {
            var info = new IdentityUnit();         
            var sqlCmd = @"Unit_GetById";

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
                            info = ExtractUnitData(reader);
                        }
                    }
                }                            
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Unit_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Unit_Delete";

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
                var strError = "Failed to execute Unit_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityUnit> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Unit_GetList";

            List<IdentityUnit> listData = new List<IdentityUnit>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractUnitData(reader);

                            listData.Add(record);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Unit_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion
    }
}
