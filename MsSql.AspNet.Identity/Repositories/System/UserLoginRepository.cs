﻿using System.Collections.Generic;
using System.Data;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class UserLoginRepository
    {
        private readonly string _connectionString;
        public UserLoginRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Insert(IdentityUser user, UserLoginInfo login)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", user.Id},
                    {"@LoginProvider", login.LoginProvider},
                    {"@ProviderKey", login.ProviderKey}
                };

                MsSqlHelper.ExecuteNonQuery(conn, @"INSERT INTO AspNetUserLogins(UserId,LoginProvider,ProviderKey) VALUES(@Id,@LoginProvider,@ProviderKey)", parameters);
            }
        }

        public void Delete(IdentityUser user, UserLoginInfo login)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", user.Id},
                    {"@LoginProvider", login.LoginProvider},
                    {"@ProviderKey", login.ProviderKey}
                };

                MsSqlHelper.ExecuteNonQuery(conn, @"DELETE FROM AspNetUserLogins WHERE UserId = @Id AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey", parameters);
            }
        }

        public string GetByUserLoginInfo(UserLoginInfo login)
        {
            string userId;
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@LoginProvider", login.LoginProvider},
                    {"@ProviderKey", login.ProviderKey}
                };
                var userIdObject = MsSqlHelper.ExecuteScalar(conn, CommandType.Text,
                    @"SELECT UserId FROM AspNetUserLogins WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey", parameters);
                userId = userIdObject == null
                    ? null
                    : userIdObject.ToString();
            }
            return userId;
        }

        public List<UserLoginInfo> PopulateLogins(string userId)
        {
            var listLogins = new List<UserLoginInfo>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id",userId}
                };

                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text,
                   @"SELECT LoginProvider,ProviderKey FROM AspNetUserLogins Where UserId = @Id", parameters);
                while (reader.Read())
                {
                    listLogins.Add(new UserLoginInfo(reader[0].ToString(), reader[1].ToString()));
                }
            }
            return listLogins;
        }

   
    }
}
