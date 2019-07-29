using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsProvider
    {
        private readonly string _connectionString;

        public RpsProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsProvider()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }

        public List<IdentityProvider> GetByPage(IdentityProvider filter, int currentPage, int pageSize)
        {           
            //Common syntax
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"Provider_GetByPage";
            List<IdentityProvider> listData = null;

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
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    listData = ParsingListDataFromReader(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Provider_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityProvider> ParsingListDataFromReader(IDataReader reader)
        {
            List<IdentityProvider> listData = listData = new List<IdentityProvider>();
            while (reader.Read())
            {
                try
                {
                    var record = ExtractDataItem(reader);

                    listData.Add(record);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return listData;
        }

        private IdentityProvider ExtractDataItem(IDataReader reader)
        {
            var record = new IdentityProvider();
            //Common properties
            if (reader.HasColumn("TotalCount"))
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();
            record.Phone = reader["Phone"].ToString();           
            record.Email = reader["Email"].ToString();
            record.Address = reader["Address"].ToString();
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public int Insert(IdentityProvider identity)
        {
            //Common syntax
            var sqlCmd = @"Provider_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name },
                {"@Code", identity.Code },
                {"@Phone", identity.Phone },
                {"@Email", identity.Email },
                {"@Address", identity.Address },
                {"@Status", identity.Status },
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {

                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            newId = Utils.ConvertToInt32(reader[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Provider_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityProvider identity)
        {
            //Common syntax
            var sqlCmd = @"Provider_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name },
                {"@Code", identity.Code },
                {"@Phone", identity.Phone },
                {"@Email", identity.Email },
                {"@Address", identity.Address },               
                {"@Status", identity.Status }
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
                var strError = "Failed to execute Provider_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityProvider GetById(int Id)
        {
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"Provider_GetById";
            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id},
            };
            try
               
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    if (reader.Read())
                    {
                        var info = ExtractDataItem(reader);

                        return info;
                    }
                }                   
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Provider_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return null;
        }
        
        public bool Delete(int Id)
        {
            //Common syntax
            var sqlCmd = @"Provider_Delete";

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
                var strError = "Failed to execute Provider_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityProvider> GetList()
        {
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"Provider_GetList";
            List<IdentityProvider> listData = new List<IdentityProvider>();
            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                {
                    listData = ParsingListData(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Provider_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityProvider> GetListByUserId(string UserId)
        {
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"Provider_GetListByUserId";
            List<IdentityProvider> listData = new List<IdentityProvider>();
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@UserId", UserId}
            };
            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    listData = ParsingListData(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Provider_GetListByUserId. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityProvider> ParsingListData(IDataReader reader)
        {
            List<IdentityProvider> listData = listData = new List<IdentityProvider>();
            while (reader.Read())
            {
                try
                {
                    var record = new IdentityProvider();
                    record.Id = Utils.ConvertToInt32(reader["Id"]);
                    record.Name = reader["Name"].ToString();

                   
                    record.Status = Utils.ConvertToInt32(reader["Status"]);
                    listData.Add(record);
                }
                catch (Exception ex)
                {
                    var strError = "Failed to ParsingListData. Error: " + ex.Message;
                    throw new CustomSQLException(strError);
                }
            }

            return listData;
        }
    }
}
