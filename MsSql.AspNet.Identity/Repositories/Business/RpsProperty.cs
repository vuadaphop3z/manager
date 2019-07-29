using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsProperty
    {
        private readonly string _connectionString;

        public RpsProperty(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsProperty()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public List<IdentityProperty> GetByPage(IdentityProperty filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Property_GetByPage";
            List<IdentityProperty> listData = null;

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
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListPropertyFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Property_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }
        public List<IdentityProperty> GetAll()
        {
            //Common syntax           
            var sqlCmd = @"Property_GetAll";
            List<IdentityProperty> listData = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        listData = ParsingListPropertyFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Property_All. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityProperty GetById(int Id)
        {
            var info = new IdentityProperty();
            var sqlCmd = @"Property_GetById";

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
                            info = ExtractPropertyData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Property_GetPropertyById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public IdentityProperty GetDetail(int Id)
        {
            var info = new IdentityProperty();
            var sqlCmd = @"Property_GetDetail";

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
                        if (reader.Read())
                        {
                            info = ExtractPropertyData(reader);
                        }

                        //Get data for all languages
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var langItem = new IdentityPropertyLang();
                                langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                                langItem.LangCode = reader["LangCode"].ToString();
                                langItem.Name = reader["Name"].ToString();
                                langItem.PropertyId = Utils.ConvertToInt32(reader["PropertyId"]);

                                info.LangList.Add(langItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Property_GetDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public int Insert(IdentityProperty identity)
        {
            var newId = 0;
            var sqlCmd = @"Property_Insert";

            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code},
                {"@PropertyCategoryId", identity.PropertyCategoryId},
                {"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        newId = Utils.ConvertToInt32(reader[0]);
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Property_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityProperty identity)
        {
            //Common syntax
            var sqlCmd = @"Property_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Code", identity.Code},
                {"@PropertyCategoryId", identity.PropertyCategoryId},
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
                var strError = "Failed to execute Property_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Property_Delete";

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
                var strError = "Failed to execute Category_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityPropertyLang> GetLangDetail(int Id)
        {
            List<IdentityPropertyLang> listItem = new List<IdentityPropertyLang>();
            var sqlCmd = @"Property_Lang_GetDetail";

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
                          var  info = new IdentityPropertyLang();

                            info.Id = Utils.ConvertToInt32(reader["Id"]);
                            info.LangCode = reader["LangCode"].ToString();
                            info.Name = reader["Name"].ToString();
                            info.PropertyId = Utils.ConvertToInt32(reader["PropertyId"]);
                            listItem.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Property_GetLangDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return listItem;
        }

        public int InsertLang(IdentityPropertyLang identity)
        {
            //Common syntax           
            var sqlCmd = @"Property_Lang_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@LangCode", identity.LangCode },
                {"@PropertyId", identity.PropertyId}
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
                var strError = "Failed to execute Property_AddNewLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int UpdateLang(IdentityPropertyLang identity)
        {
            //Common syntax           
            var sqlCmd = @"Property_Lang_Update";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@LangCode", identity.LangCode }
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
                var strError = "Failed to execute Property_Lang_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool DeleteLang(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Property_Lang_Delete";

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
                var strError = "Failed to execute Property_Lang_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        private List<IdentityProperty> ParsingListPropertyFromReader(IDataReader reader)
        {
            List<IdentityProperty> listData = listData = new List<IdentityProperty>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractPropertyData(reader);

                //Extends information
                if (listData != null)
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        public IdentityProperty ExtractPropertyData(IDataReader reader)
        {
            var record = new IdentityProperty();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();
            record.Status = Utils.ConvertToInt32(reader["Status"]);
            record.PropertyCategoryId = Utils.ConvertToInt32(reader["PropertyCategoryId"]);

            return record;
        }
    }
}
