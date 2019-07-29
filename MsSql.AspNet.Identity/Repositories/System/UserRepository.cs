using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MsSql.AspNet.Identity.Repositories
{
    public class UserRepository<TUser> where TUser : IdentityUser
    {
        private readonly string _connectionString;
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insert(TUser user)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", user.Id},
                    {"@Email", (object) user.Email ?? DBNull.Value},
                    {"@EmailConfirmed", user.EmailConfirmed},
                    {"@PasswordHash", (object) user.PasswordHash ?? DBNull.Value},
                    {"@SecurityStamp", (object) user.SecurityStamp ?? DBNull.Value},
                    {"@PhoneNumber", (object) user.PhoneNumber ?? DBNull.Value},
                    {"@PhoneNumberConfirmed", user.PhoneNumberConfirmed},
                    {"@TwoFactorEnabled", user.TwoFactorEnabled},
                    {"@LockoutEndDateUtc", (object) user.LockoutEndDateUtc ?? DBNull.Value},
                    {"@LockoutEnabled", user.LockoutEnabled},
                    {"@AccessFailedCount", user.AccessFailedCount},
                    {"@UserName", user.UserName}
                };

                MsSqlHelper.ExecuteNonQuery(conn, @"INSERT INTO AspNetUsers(Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,
                PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,CreatedDateUtc)
                VALUES(@Id,@Email,@EmailConfirmed,@PasswordHash,@SecurityStamp,@PhoneNumber,@PhoneNumberConfirmed,
                @TwoFactorEnabled,@LockoutEndDateUtc,@LockoutEnabled,@AccessFailedCount,@UserName, GETDATE())", parameters);
               
            }
        }

        public void Delete(TUser user)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", user.Id}
                };

                MsSqlHelper.ExecuteNonQuery(conn, @"DELETE FROM AspNetUsers WHERE Id=@Id", parameters);
            }
        }

        public IQueryable<TUser> GetAll()
        {
            List<TUser> users = new List<TUser>();

            using (var conn = new SqlConnection(_connectionString))
            {
                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id,Email,EmailConfirmed,
                PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
                LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,CreatedDateUtc FROM AspNetUsers", null);
                
                while (reader.Read())
                {
                    var user = (TUser)Activator.CreateInstance(typeof(TUser));
                    user.Id = reader["Id"].ToString();
                    user.Email = reader["Email"].ToString();
                    user.EmailConfirmed = Convert.ToBoolean(reader["EmailConfirmed"]);
                    user.PasswordHash = reader["PasswordHash"].ToString();
                    user.SecurityStamp = reader["SecurityStamp"].ToString();
                    user.PhoneNumber = reader["PhoneNumber"].ToString();
                    user.PhoneNumberConfirmed = Convert.ToBoolean(reader["PhoneNumberConfirmed"]);
                    user.TwoFactorEnabled = Convert.ToBoolean(reader["TwoFactorEnabled"]);
                    user.LockoutEndDateUtc = reader["LockoutEndDateUtc"] == DBNull.Value ? null : (DateTime?)reader["LockoutEndDateUtc"];
                    user.LockoutEnabled = Convert.ToBoolean(reader["LockoutEnabled"]);
                    user.AccessFailedCount = Convert.ToInt32(reader["AccessFailedCount"]);
                    user.UserName = reader["UserName"].ToString();

                    user.CreatedDateUtc = (DateTime)reader["CreatedDateUtc"];

                    users.Add(user);
                }
            }
            return users.AsQueryable<TUser>();
        }
        
        public TUser GetById(string userId)
        {
            var user = (TUser)Activator.CreateInstance(typeof(TUser));
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", userId}
                };

                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id,Email,EmailConfirmed,
                PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
                LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName, CreatedDateUtc,ProviderId FROM AspNetUsers WHERE Id=@Id", parameters);
                while (reader.Read())
                {
                    user.Id = reader["Id"].ToString();
                    user.Email = reader["Email"].ToString();
                    user.EmailConfirmed = Convert.ToBoolean(reader["EmailConfirmed"]);
                    user.PasswordHash = reader["PasswordHash"].ToString();
                    user.SecurityStamp = reader["SecurityStamp"].ToString();
                    user.PhoneNumber = reader["PhoneNumber"].ToString();
                    user.PhoneNumberConfirmed = Convert.ToBoolean(reader["PhoneNumberConfirmed"]);
                    user.TwoFactorEnabled = Convert.ToBoolean(reader["TwoFactorEnabled"]);
                    user.LockoutEndDateUtc = reader["LockoutEndDateUtc"] == DBNull.Value ? null : (DateTime?)reader["LockoutEndDateUtc"];
                    user.LockoutEnabled = Convert.ToBoolean(reader["LockoutEnabled"]);
                    user.AccessFailedCount = Convert.ToInt32(reader["AccessFailedCount"]);
                    user.UserName = reader["UserName"].ToString();

                    user.CreatedDateUtc = (DateTime)reader["CreatedDateUtc"];
                    user.ProviderId = Convert.ToInt32(reader["ProviderId"]);

                }

            }
            return user;
        }

        public TUser GetByName(string userName)
        {
            var user = (TUser)Activator.CreateInstance(typeof(TUser));
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@UserName", userName}
                };

                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id,Email,EmailConfirmed,
                PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
                LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName, CreatedDateUtc FROM AspNetUsers WHERE UserName=@UserName", parameters);
                while (reader.Read())
                {
                    user.Id = reader["Id"].ToString();
                    user.Email = reader["Email"].ToString();
                    user.EmailConfirmed = Convert.ToBoolean(reader["EmailConfirmed"]);
                    user.PasswordHash = reader["PasswordHash"].ToString();
                    user.SecurityStamp = reader["SecurityStamp"].ToString();
                    user.PhoneNumber = reader["PhoneNumber"].ToString();
                    user.PhoneNumberConfirmed = Convert.ToBoolean(reader["PhoneNumberConfirmed"]);
                    user.TwoFactorEnabled = Convert.ToBoolean(reader["TwoFactorEnabled"]);
                    user.LockoutEndDateUtc = reader["LockoutEndDateUtc"] == DBNull.Value ? null : (DateTime?)reader["LockoutEndDateUtc"];
                    user.LockoutEnabled = Convert.ToBoolean(reader["LockoutEnabled"]);
                    user.AccessFailedCount = Convert.ToInt32(reader["AccessFailedCount"]);
                    user.UserName = reader["UserName"].ToString();

                    user.CreatedDateUtc = (DateTime)reader["CreatedDateUtc"];
                }

            }
            return user;
        }

        public TUser GetByEmail(string email)
        {
            var user = (TUser)Activator.CreateInstance(typeof(TUser));
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Email", email}
                };

                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id,Email,EmailConfirmed,
                PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
                LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName, CreatedDateUtc FROM AspNetUsers WHERE Email=@Email", parameters);
                while (reader.Read())
                {
                    user.Id = reader["Id"].ToString();
                    user.Email = reader["Email"].ToString();
                    user.EmailConfirmed = Convert.ToBoolean(reader["EmailConfirmed"]);
                    user.PasswordHash = reader["PasswordHash"].ToString();
                    user.SecurityStamp = reader["SecurityStamp"].ToString();
                    user.PhoneNumber = reader["PhoneNumber"].ToString();
                    user.PhoneNumberConfirmed = Convert.ToBoolean(reader["PhoneNumberConfirmed"]);
                    user.TwoFactorEnabled = Convert.ToBoolean(reader["TwoFactorEnabled"]);
                    user.LockoutEndDateUtc = reader["LockoutEndDateUtc"] == DBNull.Value ? null : (DateTime?)reader["LockoutEndDateUtc"];
                    user.LockoutEnabled = Convert.ToBoolean(reader["LockoutEnabled"]);
                    user.AccessFailedCount = Convert.ToInt32(reader["AccessFailedCount"]);
                    user.UserName = reader["UserName"].ToString();

                    user.CreatedDateUtc = (DateTime)reader["CreatedDateUtc"];
                }

            }
            return user;
        }

       

        public void Update(TUser user)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@NewId", user.Id},
                    {"@Email", (object) user.Email ?? DBNull.Value},
                    {"@EmailConfirmed", user.EmailConfirmed},
                    {"@PasswordHash", (object) user.PasswordHash ?? DBNull.Value},
                    {"@SecurityStamp", (object) user.SecurityStamp ?? DBNull.Value},
                    {"@PhoneNumber", (object) user.PhoneNumber ?? DBNull.Value},
                    {"@PhoneNumberConfirmed", user.PhoneNumberConfirmed},
                    {"@TwoFactorEnabled", user.TwoFactorEnabled},
                    {"@LockoutEndDateUtc", (object) user.LockoutEndDateUtc ?? DBNull.Value},
                    {"@LockoutEnabled", user.LockoutEnabled},
                    {"@AccessFailedCount", user.AccessFailedCount},
                    {"@UserName", user.UserName},
                    {"@Id", user.Id},
                    {"@ProviderId", user.ProviderId},
                };

                MsSqlHelper.ExecuteNonQuery(conn, @"UPDATE AspNetUsers 
                SET Id = @NewId,Email=@Email,EmailConfirmed=@EmailConfirmed,PasswordHash=@PasswordHash,SecurityStamp=@SecurityStamp,PhoneNumber=@PhoneNumber,PhoneNumberConfirmed=@PhoneNumberConfirmed,
                TwoFactorEnabled=@TwoFactorEnabled,LockoutEndDateUtc=@LockoutEndDateUtc,LockoutEnabled=@LockoutEnabled,AccessFailedCount=@AccessFailedCount,UserName=@UserName,ProviderId=@ProviderId
                WHERE Id=@Id", parameters);
            }
        }
    }
}
