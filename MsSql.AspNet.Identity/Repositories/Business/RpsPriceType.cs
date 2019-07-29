using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsPriceType
    {
        private readonly string _connectionString;

        public RpsPriceType(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsPriceType()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }

        public List<IdentityPriceType> GetAll(IdentityPriceType filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"M_PriceType_GetByPage";
            List<IdentityPriceType> listData = null;

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
                        listData = ParsingListPriceTypeFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_PriceType_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityPriceType GetById(int Id)
        {
            var info = new IdentityPriceType();
            var sqlCmd = @"PriceType_GetById";

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
                            info = ExtractPriceTypeData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute PriceType_GetPriceTypeById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public IdentityPriceType GetDetail(int Id)
        {
            var info = new IdentityPriceType();
            var sqlCmd = @"M_PriceType_GetDetail";

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
                            info = ExtractPriceTypeData(reader);
                        }

                        //Get data for all languages
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var langItem = new IdentityPriceTypeLang();
                                langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                                langItem.LangCode = reader["LangCode"].ToString();
                                langItem.Name = reader["Name"].ToString();
                                langItem.PriceTypeId = Utils.ConvertToInt32(reader["PriceTypeId"]);

                                info.LangList.Add(langItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_PriceType_GetDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public int Insert(IdentityPriceType identity)
        {
            var newId = 0;
            var sqlCmd = @"M_PriceType_Insert";

            var parameters = new Dictionary<string, object>
            {
                {"@Code", identity.Code},
                {"@Name", identity.Name},
                {"@Icon", identity.Icon},
                {"@Status", identity.Status},
                {"@CreatedBy", identity.CreatedBy},
                {"@Description", identity.Description}
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
                var strError = "Failed to M_PriceType_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityPriceType identity)
        {
            //Common syntax
            var sqlCmd = @"M_PriceType_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Code", identity.Code },
                {"@Icon", identity.Icon },
                {"@Status", identity.Status},
                {"@Description", identity.Description}
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
                var strError = "Failed to execute M_PriceType_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_PriceType_Delete";

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

        public IdentityPriceTypeLang GetLangDetail(int Id)
        {
            IdentityPriceTypeLang info = null;
            var sqlCmd = @"M_PriceType_GetLangDetail";

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
                            info = new IdentityPriceTypeLang();

                            info.Id = Utils.ConvertToInt32(reader["Id"]);
                            info.LangCode = reader["LangCode"].ToString();
                            info.Name = reader["Name"].ToString();
                            info.PriceTypeId = Utils.ConvertToInt32(reader["PriceTypeId"]);
                            info.Description = reader["Description"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_PriceType_GetLangDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public int AddNewLang(IdentityPriceTypeLang identity)
        {
            //Common syntax           
            var sqlCmd = @"M_PriceType_AddNewLang";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@LangCode", identity.LangCode },
                {"@PriceTypeId", identity.PriceTypeId},
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
                var strError = "Failed to execute M_PriceType_AddNewLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int UpdateLang(IdentityPriceTypeLang identity)
        {
            //Common syntax           
            var sqlCmd = @"M_PriceType_UpdateLang";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@LangCode", identity.LangCode },
                {"@Description", identity.Description }
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
                var strError = "Failed to execute M_PriceType_UpdateLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool DeleteLang(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_PriceType_DeleteLang";

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
                var strError = "Failed to execute M_PriceType_DeleteLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        private List<IdentityPriceType> ParsingListPriceTypeFromReader(IDataReader reader)
        {
            List<IdentityPriceType> listData = listData = new List<IdentityPriceType>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractPriceTypeData(reader);

                //Extends information
                if (listData != null)
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityPriceType ExtractPriceTypeData(IDataReader reader)
        {
            var record = new IdentityPriceType();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();
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
