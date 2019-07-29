using Manager.SharedLib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MsSql.AspNet.Identity.Repositories
{
    public class IdentityRepository
    {
        private readonly string _connectionString;
        public IdentityRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<IdentityUser> FilterUserList(string email, string roleId, bool isLocked, int page, int pageSize)
        {
            List<IdentityUser> users = new List<IdentityUser>();

            using (var conn = new SqlConnection(_connectionString))
            {
                int offset = (page - 1) * pageSize;
                //if (email == null) email = string.Empty;
                //if (roleId == null) roleId = string.Empty;
                var parameters = new Dictionary<string, object>
                {
                    {"@Email", email},
                    {"@RoleId", roleId},
                    {"@lockedEnable", (isLocked)? 1:0 },
                    {"@offset", offset},
                    {"@pageSize", pageSize},
                };
                var query = @"select DISTINCT a.*                      
                        FROM AspNetUsers as a
                        LEFT JOIN AspNetUserRoles b on a.Id = b.UserId 
                        Where 1=1	
                        AND(a.Email LIKE CONCAT('%', @Email , '%') OR a.UserName LIKE CONCAT('%', @Email , '%')
                            OR a.PhoneNumber LIKE CONCAT('%', @Email , '%')
                        )
                        AND (@RoleId IS NULL OR b.RoleId = @RoleId)
                        {0}
                        ORDER BY a.Email ASC
                        OFFSET @offset ROWS		
                        FETCH NEXT @pageSize ROWS ONLY
                        ";
                string appendQuery = "";
                if (isLocked)
                {
                    appendQuery = "AND (a.LockoutEnabled = @lockedEnable AND a.LockoutEndDateUtc IS NOT NULL AND a.LockoutEndDateUtc > GETDATE() )";

                }
                query = string.Format(query, appendQuery);

                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text, query, parameters);
                while (reader.Read())
                {
                    var user = (IdentityUser)Activator.CreateInstance(typeof(IdentityUser));
                    user = ParsingUserDataFromReader(reader);
                    users.Add(user);
                }
            }
            return users.AsQueryable<IdentityUser>();
        }

        public int CountAll(string email, string roleId, bool isLocked)
        {
            var total = 0;
            if (email == null) email = string.Empty;
            var parameters = new Dictionary<string, object>
                {
                    {"@Email", email},
                    {"@RoleId", roleId},
                    {"@lockedEnable", (isLocked)? 1:0 }
                };
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"SELECT count(*) as CountNum FROM (SELECT DISTINCT a.Id
                    FROM AspNetUsers as a
                    LEFT JOIN AspNetUserRoles b on a.Id = b.UserId 
                    Where a.Email LIKE CONCAT('%', @Email , '%')
                    {0}
                    AND (@RoleId IS NULL OR b.RoleId = @RoleId) ) as tbl_count                  
                    ";
                string appendQuery = "";
                if (isLocked)
                {
                    appendQuery = "AND (a.LockoutEnabled = @lockedEnable AND a.LockoutEndDateUtc IS NOT NULL AND a.LockoutEndDateUtc > GETDATE())";

                }
                query = string.Format(query, appendQuery);
                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text, query, parameters);
                if (reader.Read())
                {
                    total = Convert.ToInt32(reader["CountNum"]);
                }
            }
            return total;
        }

        public IdentityUser GetById(string userId)
        {
            var user = (IdentityUser)Activator.CreateInstance(typeof(IdentityUser));
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", userId}
                };

                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT * FROM AspNetUsers WHERE Id=@Id", parameters);
                while (reader.Read())
                {
                    user = ParsingUserDataFromReader(reader);
                }

            }
            return user;
        }

        public IdentityUser ParsingUserDataFromReader(IDataReader reader)
        {
            var user = (IdentityUser)Activator.CreateInstance(typeof(IdentityUser));
            user.Id = reader["Id"].ToString();
            user.Email = reader["Email"].ToString();
            user.EmailConfirmed = Convert.ToBoolean(reader["EmailConfirmed"]);
            user.PhoneNumber = reader["PhoneNumber"].ToString();
            user.PhoneNumberConfirmed = Convert.ToBoolean(reader["PhoneNumberConfirmed"]);
            user.TwoFactorEnabled = Convert.ToBoolean(reader["TwoFactorEnabled"]);
            user.LockoutEndDateUtc = reader["LockoutEndDateUtc"] == DBNull.Value ? null : (DateTime?)reader["LockoutEndDateUtc"];
            user.LockoutEnabled = Convert.ToBoolean(reader["LockoutEnabled"]);
            user.AccessFailedCount = Convert.ToInt32(reader["AccessFailedCount"]);
            user.UserName = reader["UserName"].ToString();

            user.CreatedDateUtc = (DateTime)reader["CreatedDateUtc"];
            user.StaffId= Convert.ToInt32(reader["StaffId"]);



            return user;
        }
        public List<IdentityUser> ParsingListUserDataFromReader(IDataReader reader)
        {
            List<IdentityUser> listData = listData = new List<IdentityUser>();
            while (reader.Read())
            {

                var record = new IdentityUser();
                record.Id = reader["Id"].ToString();
                record.UserName = reader["UserName"].ToString();
                record.Email = reader["Email"].ToString();
                record.StaffId = Utils.ConvertToInt32(reader["StaffId"]);

                listData.Add(record);
            }
            return listData;
        }
        public List<IdentityUser> GetListUser()
        {
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"Users_GetListUser";
            List<IdentityUser> listData = null;
            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                {
                    listData = ParsingListUserDataFromReader(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Users_GetListUser. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }
        public void UpdateProfile(IdentityUser user)
        {
            var conn = new SqlConnection(_connectionString);
            var parameters = new Dictionary<string, object>
            {
                {"@UserId", user.Id},
                {"@Avatar", user.Avatar }
            };

            var sqlCmd = @"UPDATE aspnetusers SET Avatar = @Avatar WHERE 1=1 AND Id = @UserId";
            try
            {
                MsSqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlCmd, parameters);
            }
            catch (Exception ex)
            {
                var strError = "Failed to UPDATE user profile. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }
    }
}
