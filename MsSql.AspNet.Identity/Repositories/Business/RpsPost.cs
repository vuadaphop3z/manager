using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsPost
    {
        private readonly string _connectionString;

        public RpsPost(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsPost()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }

        #region --- Post ----

        public List<IdentityPost> GetByPage(IdentityPost filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@Offset", offset},
                {"@PageSize", filter.PageSize},
                {"@SortField", filter.SortField },
                {"@SortType", filter.SortType },
                {"@Status", filter.Status }
            };

            var sqlCmd = @"M_Post_GetByPage";

            List<IdentityPost> myList = new List<IdentityPost>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractPostItem(reader);

                            myList.Add(entity);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to M_Post_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public IdentityPost GetById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            var sqlCmd = @"M_Post_GetById";

            IdentityPost info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if(reader.Read())
                        {
                            //Get common info
                            info = ExtractPostItem(reader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        private IdentityPostData ExtractPostData(IDataReader reader)
        {
            var record = new IdentityPostData();
            record.Images = JsonConvert.DeserializeObject<List<IdentityImage>>(reader["Images"].ToString());
            record.Locations = JsonConvert.DeserializeObject<List<IdentityLocation>>(reader["Locations"].ToString());

            return record;
        }

        private List<IdentityPost> ParsingPostData(IDataReader reader)
        {
            List<IdentityPost> listData = new List<IdentityPost>();
            while (reader.Read())
            {
                //Get common info
                var entity = ExtractPostItem(reader);               

                listData.Add(entity);
            }

            return listData;
        }

        private IdentityPost ExtractPostItem(IDataReader reader)
        {
            var entity = new IdentityPost();          
            
            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.Title = reader["Title"].ToString();
            entity.Description = reader["Description"].ToString();
            entity.BodyContent = reader["BodyContent"].ToString();
            entity.IsHighlights = Utils.ConvertToBoolean(reader["IsHighlights"]);
            entity.Cover = reader["Cover"].ToString();
            entity.UrlFriendly = reader["UrlFriendly"].ToString();
            entity.PostType = Utils.ConvertToInt32(reader["PostType"]);
            entity.CreatedBy = reader["CreatedBy"].ToString();
            entity.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            entity.Status = Utils.ConvertToInt32(reader["Status"]);

            if (reader.HasColumn("TotalCount"))
                entity.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

            return entity;
        }

        public int Insert(IdentityPost identity)
        {
            var sqlCmd = @"M_Post_Insert";
            var newId = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@Title", identity.Title},
                {"@Description", identity.Description},
                {"@BodyContent", identity.BodyContent},
                {"@IsHighlights", identity.IsHighlights},
                {"@Cover", identity.Cover},
                {"@UrlFriendly", identity.UrlFriendly},
                {"@PostType", identity.PostType},
                {"@CreatedBy", identity.CreatedBy},
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
                var strError = "Failed to M_Post_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int Update(IdentityPost identity)
        {
            var sqlCmd = @"M_Post_Update";
            
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Title", identity.Title},
                {"@Description", identity.Description},
                {"@BodyContent", identity.BodyContent},
                {"@IsHighlights", identity.IsHighlights},
                {"@Cover", identity.Cover},
                {"@UrlFriendly", identity.UrlFriendly},
                {"@PostType", identity.PostType},
                {"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to M_Post_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int Delete(int id)
        {
            var sqlCmd = @"M_Post_Delete";

            var parameters = new Dictionary<string, object>
            {
                    {"@Id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to M_Post_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        #endregion

        public bool AssignCategory(int postId, int catId)
        {
            //Common syntax
            var sqlCmd = @"M_Post_AssignCategory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@PostId", postId},
                {"@CatId", catId}
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
                var strError = "Failed to execute M_Post_AssignCategory. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }
    }
}
