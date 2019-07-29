using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsMember
    {
        private readonly string _connectionString;

        public RpsMember(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsMember()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SingleSignOnDB"].ConnectionString;
        }

        public List<IdentityMember> GetByPage(IdentityMember filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"M_User_GetByPage";
            List<IdentityMember> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword },
                {"@Offset", offset},
                {"@PageSize", pageSize},
                {"@Status", filter.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityMember>();
                        while (reader.Read())
                        {
                            var info = ExtractMemberData(reader);

                            listData.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_User_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityMember GetById(int Id)
        {
            var info = new IdentityMember();
            var sqlCmd = @"M_User_GetById";

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
                            info = ExtractMemberData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_User_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public List<IdentityMember> GetList(string keyword)
        {
            //Common syntax           
            var sqlCmd = @"M_User_GetList";
            List<IdentityMember> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", keyword }               
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityMember>();
                        while (reader.Read())
                        {
                            var info = ExtractMemberData(reader);

                            listData.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_User_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public int Insert(IdentityMember identity, out int code)
        {
            var newId = 0;
            var sqlCmd = @"M_User_Insert";
            code = 1;

            var parameters = new Dictionary<string, object>
            {
                {"@UserName", identity.UserName},
                {"@Email", identity.Email },
                {"@PhoneNumber", identity.PhoneNumber },
                {"@DisplayName", identity.DisplayName},               
                {"@PasswordHash", identity.PasswordHash},
                {"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        code = Utils.ConvertToInt32(reader[0]);
                    }

                    if (reader.NextResult())
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
                var strError = "Failed to M_User_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityMember identity, out int code)
        {
            //Common syntax
            var sqlCmd = @"M_User_Update";
            code = 1;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@UserName", identity.UserName},
                {"@Email", identity.Email },
                {"@PhoneNumber", identity.PhoneNumber },
                {"@DisplayName", identity.DisplayName},
                {"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    if (reader.Read())
                    {
                        code = Utils.ConvertToInt32(reader[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_User_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_User_Delete";

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
                var strError = "Failed to execute M_User_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        private IdentityMember ExtractMemberData(IDataReader reader)
        {
            var entity = new IdentityMember();
            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.UserName = reader["UserName"].ToString();
            entity.Email = reader["Email"].ToString();
            entity.PhoneNumber = reader["PhoneNumber"].ToString();
            entity.FullName = reader["FullName"].ToString();
            entity.DisplayName = reader["DisplayName"].ToString();
            entity.Avatar = reader["Avatar"].ToString();

            entity.OTPType = reader["OTPType"].ToString();
            entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];
            entity.Sex = Utils.ConvertToInt32(reader["Sex"]);
            entity.Address = reader["Address"].ToString();
            entity.IDCard = reader["IDCard"].ToString();
            entity.Note = reader["Note"].ToString();
            entity.Status = Utils.ConvertToInt32(reader["Status"]);

            if (reader.HasColumn("TotalCount"))
                entity.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

            return entity;
        }
    }
}
