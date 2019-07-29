using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsPropertyCategory
    {
        private readonly string _connectionString;

        public RpsPropertyCategory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsPropertyCategory()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityPropertyCategory> GetByPage(IdentityPropertyCategory filter, int currentPage, int pageSize)
        {           
            //Common syntax           
            var sqlCmd = @"PropertyCategory_GetByPage";
            List<IdentityPropertyCategory> listData = null;

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
                        listData = ParsingListPropertyCategoryFromReader(reader);

                        if (listData != null && listData.Count > 0)
                        {
                            if (reader.NextResult())
                            {
                                var properties = new List<IdentityProperty>();
                                while (reader.Read())
                                {
                                    var property = new RpsProperty().ExtractPropertyData(reader);
                                    properties.Add(property);

                                    foreach (var item in listData)
                                    {
                                        if(item.Id == property.PropertyCategoryId)
                                        {
                                            item.Properties.Add(property);
                                        }
                                    }
                                }
                            }
                        }                        
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute PropertyCategory_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityPropertyCategory> ParsingListPropertyCategoryFromReader(IDataReader reader)
        {
            List<IdentityPropertyCategory> listData = listData = new List<IdentityPropertyCategory>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractPropertyCategoryData(reader);

                //Extends information
                if(reader.HasColumn("TotalCount"))
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityPropertyCategory ExtractPropertyCategoryData(IDataReader reader)
        {
            var record = new IdentityPropertyCategory();

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

        public int Insert(IdentityPropertyCategory identity)
        {
            //Common syntax           
            var sqlCmd = @"PropertyCategory_Insert";
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
                var strError = "Failed to execute PropertyCategory_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityPropertyCategory identity)
        {
            //Common syntax
            var sqlCmd = @"PropertyCategory_Update";

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
                var strError = "Failed to execute PropertyCategory_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityPropertyCategory GetById(int Id)
        {
            var info = new IdentityPropertyCategory();         
            var sqlCmd = @"PropertyCategory_GetById";

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
                            info = ExtractPropertyCategoryData(reader);
                        }
                    }
                }                            
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute PropertyCategory_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public IdentityPropertyCategory GetDetail(int Id)
        {
            var info = new IdentityPropertyCategory();
            var sqlCmd = @"PropertyCategory_GetDetail";

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
                        //Base info
                        if (reader.Read())
                        {
                            info = ExtractPropertyCategoryData(reader);
                        }

                        //Properties
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var item = new RpsProperty().ExtractPropertyData(reader);
                                info.Properties.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute PropertyCategory_GetDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"PropertyCategory_Delete";

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
                var strError = "Failed to execute PropertyCategory_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityPropertyCategory> GetList()
        {
            //Common syntax            
            var sqlCmd = @"PropertyCategory_GetList";

            List<IdentityPropertyCategory> listData = new List<IdentityPropertyCategory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractPropertyCategoryData(reader);

                            listData.Add(record);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute PropertyCategory_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion
    }
}
