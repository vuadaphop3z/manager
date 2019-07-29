using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsGroupProperty
    {
        private readonly string _connectionString;

        public RpsGroupProperty(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsGroupProperty()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }

        public List<IdentityGroupProperty> GetByPage(IdentityGroupProperty filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"M_GroupProperty_GetByPage";
            List<IdentityGroupProperty> listData = null;

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
                        listData = ParsingListGroupPropertyFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_GroupProperty_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }
        public List<IdentityGroupProperty> GetAll()
        {
            //Common syntax           
            var sqlCmd = @"M_GroupProperty_GetAll";
            List<IdentityGroupProperty> listData = new List<IdentityGroupProperty>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractGroupPropertyData(reader);
                            listData.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_GroupProperty_All. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityGroupProperty GetById(int Id)
        {
            var info = new IdentityGroupProperty();
            var sqlCmd = @"GroupProperty_GetById";

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
                            info = ExtractGroupPropertyData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute GroupProperty_GetGroupPropertyById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public IdentityGroupProperty GetDetail(int Id)
        {
            var info = new IdentityGroupProperty();
            var sqlCmd = @"M_GroupProperty_GetDetail";

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
                            info = ExtractGroupPropertyData(reader);
                        }

                        //Get data for all languages
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var langItem = new IdentityGroupPropertyLang();
                                langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                                langItem.LangCode = reader["LangCode"].ToString();
                                langItem.GroupName = reader["GroupName"].ToString();
                                langItem.Description = reader["Description"].ToString();
                                langItem.GroupId = Utils.ConvertToInt32(reader["GroupId"]);

                                info.LangList.Add(langItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_GroupProperty_GetDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public int Insert(IdentityGroupProperty identity)
        {
            var newId = 0;
            var sqlCmd = @"M_GroupProperty_Insert";

            var parameters = new Dictionary<string, object>
            {
                { "@Name", identity.Name},
                {"@Icon", identity.Icon},
                {"@Status", identity.Status},
                {"@Description", identity.Description},
                {"@CreatedBy", identity.CreatedBy}
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
                var strError = "Failed to M_GroupProperty_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityGroupProperty identity)
        {
            //Common syntax
            var sqlCmd = @"M_GroupProperty_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Icon", identity.Icon },
                {"@Status", identity.Status},
                {"@Description", identity.Description},
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
                var strError = "Failed to execute M_GroupProperty_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_GroupProperty_Delete";

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
                var strError = "Failed to execute M_Category_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityGroupPropertyLang> GetLangDetail(int Id)
        {
            List<IdentityGroupPropertyLang> listItem = new List<IdentityGroupPropertyLang>();
            var sqlCmd = @"M_GroupProperty_GetLangDetail";

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
                          var  info = new IdentityGroupPropertyLang();

                            info.Id = Utils.ConvertToInt32(reader["Id"]);
                            info.LangCode = reader["LangCode"].ToString();
                            info.GroupName = reader["GroupName"].ToString();
                            info.Description = reader["Description"].ToString();
                            info.GroupId = Utils.ConvertToInt32(reader["GroupId"]);
                            listItem.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_GroupProperty_GetLangDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return listItem;
        }

        public int AddNewLang(IdentityGroupPropertyLang identity)
        {
            //Common syntax           
            var sqlCmd = @"M_GroupProperty_AddNewLang";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@GroupName", identity.GroupName},
                {"@LangCode", identity.LangCode },
                {"@GroupId", identity.GroupId},
                {"@Description", identity.Description}
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
                var strError = "Failed to execute M_GroupProperty_AddNewLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int UpdateLang(IdentityGroupPropertyLang identity)
        {
            //Common syntax           
            var sqlCmd = @"M_GroupProperty_UpdateLang";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@GroupName", identity.GroupName},
                {"@LangCode", identity.LangCode },
                 {"@Description", identity.Description}

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
                var strError = "Failed to execute M_GroupProperty_UpdateLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool DeleteLang(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_GroupProperty_DeleteLang";

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
                var strError = "Failed to execute M_GroupProperty_DeleteLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        private List<IdentityGroupProperty> ParsingListGroupPropertyFromReader(IDataReader reader)
        {
            List<IdentityGroupProperty> listData = listData = new List<IdentityGroupProperty>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractGroupPropertyData(reader);

                //Extends information
                if (listData != null)
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityGroupProperty ExtractGroupPropertyData(IDataReader reader)
        {
            var record = new IdentityGroupProperty();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Icon = reader["Icon"].ToString();
            record.Description = reader["Description"].ToString();

            record.CreatedBy = reader["CreatedBy"].ToString();
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }
    }
}
