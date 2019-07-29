using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsPlaceTypeGroup
    {
        private readonly string _connectionString;

        public RpsPlaceTypeGroup(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsPlaceTypeGroup()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }

        public List<IdentityPlaceTypeGroup> GetAll(IdentityPlaceTypeGroup filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"M_PlaceTypeGroup_GetByPage";
            List<IdentityPlaceTypeGroup> listData = null;

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
                        listData = ParsingListPlaceTypeGroupFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_PlaceTypeGroup_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityPlaceTypeGroup GetById(int Id)
        {
            var info = new IdentityPlaceTypeGroup();
            var sqlCmd = @"PlaceTypeGroup_GetById";

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
                            info = ExtractPlaceTypeGroupData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute PlaceTypeGroup_GetPlaceTypeGroupById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public IdentityPlaceTypeGroup GetDetail(int Id)
        {
            var info = new IdentityPlaceTypeGroup();
            var sqlCmd = @"M_PlaceTypeGroup_GetDetail";

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
                            info = ExtractPlaceTypeGroupData(reader);
                        }

                        //Get data for all languages
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var langItem = new IdentityPlaceTypeGroupLang();
                                langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                                langItem.LangCode = reader["LangCode"].ToString();
                                langItem.GroupName = reader["GroupName"].ToString();
                                langItem.GroupId = Utils.ConvertToInt32(reader["GroupId"]);

                                info.LangList.Add(langItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_PlaceTypeGroup_GetDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public int Insert(IdentityPlaceTypeGroup identity)
        {
            var newId = 0;
            var sqlCmd = @"M_PlaceTypeGroup_Insert";

            var parameters = new Dictionary<string, object>
            {
                {"@GroupCode", identity.GroupCode},
                {"@GroupName", identity.GroupName},
                {"@Icon", identity.Icon},
                {"@FilterOnMap", identity.FilterOnMap},
                {"@Status", identity.Status},
                {"@SortOrder", identity.SortOrder},
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
                var strError = "Failed to M_PlaceTypeGroup_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityPlaceTypeGroup identity)
        {
            //Common syntax
            var sqlCmd = @"M_PlaceTypeGroup_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@GroupName", identity.GroupName},
                {"@GroupCode", identity.GroupCode },
                {"@FilterOnMap", identity.FilterOnMap },
                {"@SortOrder", identity.SortOrder },
                {"@Icon", identity.Icon },
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
                var strError = "Failed to execute M_PlaceTypeGroup_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_PlaceTypeGroup_Delete";

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

        public IdentityPlaceTypeGroupLang GetLangDetail(int Id)
        {
            IdentityPlaceTypeGroupLang info = null;
            var sqlCmd = @"M_PlaceTypeGroup_GetLangDetail";

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
                            info = new IdentityPlaceTypeGroupLang();

                            info.Id = Utils.ConvertToInt32(reader["Id"]);
                            info.LangCode = reader["LangCode"].ToString();
                            info.GroupName = reader["GroupName"].ToString();
                            info.GroupId = Utils.ConvertToInt32(reader["GroupId"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_PlaceTypeGroup_GetLangDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public int AddNewLang(IdentityPlaceTypeGroupLang identity)
        {
            //Common syntax           
            var sqlCmd = @"M_PlaceTypeGroup_AddNewLang";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@GroupName", identity.GroupName},
                {"@LangCode", identity.LangCode },
                {"@GroupId", identity.GroupId}
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
                var strError = "Failed to execute M_PlaceTypeGroup_AddNewLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int UpdateLang(IdentityPlaceTypeGroupLang identity)
        {
            //Common syntax           
            var sqlCmd = @"M_PlaceTypeGroup_UpdateLang";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@GroupName", identity.GroupName},
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
                var strError = "Failed to execute M_PlaceTypeGroup_UpdateLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool DeleteLang(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_PlaceTypeGroup_DeleteLang";

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
                var strError = "Failed to execute M_PlaceTypeGroup_DeleteLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        private List<IdentityPlaceTypeGroup> ParsingListPlaceTypeGroupFromReader(IDataReader reader)
        {
            List<IdentityPlaceTypeGroup> listData = listData = new List<IdentityPlaceTypeGroup>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractPlaceTypeGroupData(reader);

                //Extends information
                if (listData != null)
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityPlaceTypeGroup ExtractPlaceTypeGroupData(IDataReader reader)
        {
            var record = new IdentityPlaceTypeGroup();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.GroupName = reader["GroupName"].ToString();
            record.GroupCode = reader["GroupCode"].ToString();
            record.Icon = reader["Icon"].ToString();
            record.CultureName = record.GroupName;
            record.FilterOnMap = Utils.ConvertToBoolean(reader["FilterOnMap"]);
            record.SortOrder = Utils.ConvertToInt32(reader["SortOrder"]);
            record.Status = Utils.ConvertToInt32(reader["Status"]);
            record.CreatedDate = (reader["CreatedDate"] == DBNull.Value) ? null : (DateTime?)reader["CreatedDate"];
            record.CreatedBy = reader["CreatedBy"].ToString();

            return record;
        }
    }
}
