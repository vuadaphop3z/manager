using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsCategory
    {
        private readonly string _connectionString;

        public RpsCategory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsCategory()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityCategory> GetAll(IdentityCategory filter, int currentPage, int pageSize)
        {           
            //Common syntax           
            var sqlCmd = @"M_Category_GetByPage";
            List<IdentityCategory> listData = null;

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
                        listData = ParsingListCategoryFromReader(reader);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Category_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityCategory> ParsingListCategoryFromReader(IDataReader reader)
        {
            List<IdentityCategory> listData = listData = new List<IdentityCategory>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractCategoryData(reader);

                //Extends information
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityCategory ExtractCategoryData(IDataReader reader)
        {
            var record = new IdentityCategory();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();
            record.Icon = reader["Icon"].ToString();
            record.UrlFriendly = reader["UrlFriendly"].ToString();
            record.CreatedBy = reader["CreatedBy"].ToString();
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public int Insert(IdentityCategory identity)
        {
            //Common syntax           
            var sqlCmd = @"M_Category_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code },
                {"@Icon", identity.Icon },
                {"@UrlFriendly", identity.UrlFriendly },
                {"@CreatedBy", identity.CreatedBy},
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
                var strError = "Failed to execute M_Category_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityCategory identity)
        {
            //Common syntax
            var sqlCmd = @"M_Category_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},                
                {"@Name", identity.Name},
                {"@Code", identity.Code },
                {"@Icon", identity.Icon },
                {"@UrlFriendly", identity.UrlFriendly },
                {"@LastUpdatedBy", identity.LastUpdatedBy},
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
                var strError = "Failed to execute M_Category_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityCategory GetById(int Id)
        {
            var info = new IdentityCategory();         
            var sqlCmd = @"Category_GetById";

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
                        if(reader.Read())
                        {
                            info = ExtractCategoryData(reader);
                        }
                    }
                }                            
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Category_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_Category_Delete";

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
                var strError = "Failed to execute M_Category_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityCategory> GetList()
        {
            //Common syntax            
            var sqlCmd = @"M_Category_GetList";

            List<IdentityCategory> listData = new List<IdentityCategory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCategoryData(reader);

                            listData.Add(record);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Category_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityCategory GetDetail(int Id)
        {
            var info = new IdentityCategory();
            var sqlCmd = @"M_Category_GetDetail";

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
                        //Get base info
                        if(reader.Read())
                        {
                            info = ExtractCategoryData(reader);
                        }

                        //Get data for all languages
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var langItem = new IdentityCategoryLang();
                                langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                                langItem.LangCode = reader["LangCode"].ToString();
                                langItem.Name = reader["Name"].ToString();
                                langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                                langItem.CategoryId = Utils.ConvertToInt32(reader["CategoryId"]);

                                info.LangList.Add(langItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Category_GetDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public IdentityCategoryLang GetLangDetail(int Id)
        {
            IdentityCategoryLang info = null;
            var sqlCmd = @"M_Category_GetLangDetail";

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
                        if (reader.Read())
                        {
                            info = new IdentityCategoryLang();

                            info.Id = Utils.ConvertToInt32(reader["Id"]);
                            info.LangCode = reader["LangCode"].ToString();
                            info.Name = reader["Name"].ToString();
                            info.UrlFriendly = reader["UrlFriendly"].ToString();
                            info.CategoryId = Utils.ConvertToInt32(reader["CategoryId"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Category_GetLangDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public int AddNewLang(IdentityCategoryLang identity)
        {
            //Common syntax           
            var sqlCmd = @"M_Category_AddNewLang";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@LangCode", identity.LangCode },
                {"@UrlFriendly", identity.UrlFriendly },
                {"@CategoryId", identity.CategoryId}
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
                var strError = "Failed to execute M_Category_AddNewLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int UpdateLang(IdentityCategoryLang identity)
        {
            //Common syntax           
            var sqlCmd = @"M_Category_UpdateLang";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@LangCode", identity.LangCode },
                {"@UrlFriendly", identity.UrlFriendly },
                {"@CategoryId", identity.CategoryId}
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
                var strError = "Failed to execute M_Category_UpdateLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool DeleteLang(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_Category_DeleteLang";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id},
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
                var strError = "Failed to execute M_Category_DeleteLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        #endregion
    }
}
