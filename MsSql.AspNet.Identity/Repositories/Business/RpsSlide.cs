using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsSlide
    {
        private readonly string _connectionString;

        public RpsSlide(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsSlide()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentitySlide> GetByPage(IdentitySlide filter, int currentPage, int pageSize)
        {           
            //Common syntax           
            var sqlCmd = @"M_Slide_GetByPage";
            List<IdentitySlide> listData = null;

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
                using(var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListSlideFromReader(reader);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Slide_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentitySlide> GetAll(IdentitySlide filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"M_Slide_GetAll";
            List<IdentitySlide> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", filter.Name },
                {"@Code", filter.Code },
                {"@Status", filter.Status },
                {"@TuNgay", filter.FromDate },
                {"@DenNgay", filter.ToDate },
                {"@Offset", offset},
                {"@PageSize", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListSlideFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Slide_GetAll. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentitySlide> ParsingListSlideFromReader(IDataReader reader)
        {
            List<IdentitySlide> listData = listData = new List<IdentitySlide>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractSlideData(reader);

                //Extends information
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentitySlide ExtractSlideData(IDataReader reader)
        {
            var record = new IdentitySlide();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);            
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();
            record.SlideType = reader["SlideType"].ToString();
            record.CssClass = reader["CssClass"].ToString();
            record.DelayTime = Utils.ConvertToInt32(reader["DelayTime"]);
            record.Configs = reader["Configs"].ToString();
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public List<IdentitySlideItem> GetAllSlideItemBySlide(int slideId)
        {
            //Common syntax           
            var sqlCmd = @"M_Slide_GetAllItem";
            List<IdentitySlideItem> listData = new List<IdentitySlideItem>();

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", slideId }               
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var item = ExtractSlideItemData(reader);

                            listData.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Slide_GetAllItem. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private IdentitySlideItem ExtractSlideItemData(IDataReader reader)
        {
            var record = new IdentitySlideItem();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.SlideId = Utils.ConvertToInt32(reader["SlideId"]);
            record.Title = reader["Title"].ToString();
            record.Description = reader["Description"].ToString();
            record.ImageUrl = reader["ImageUrl"].ToString();
            record.Link = reader["Link"].ToString();
            record.LinkAction = Utils.ConvertToInt32(reader["LinkAction"]);
            record.SortOrder = Utils.ConvertToInt32(reader["SortOrder"]);
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public int Insert(IdentitySlide identity)
        {
            //Common syntax           
            var sqlCmd = @"M_Slide_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code },               
                {"@SlideType", identity.SlideType },               
                {"@CssClass", identity.CssClass },               
                {"@DelayTime", identity.DelayTime },               
                {"@Configs", identity.Configs },               
                {"@Status", identity.Status}
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
                var strError = "Failed to execute M_Slide_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentitySlide identity)
        {
            //Common syntax
            var sqlCmd = @"M_Slide_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Code", identity.Code },
                {"@SlideType", identity.SlideType },
                {"@CssClass", identity.CssClass },
                {"@DelayTime", identity.DelayTime },
                {"@Configs", identity.Configs },
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
                var strError = "Failed to execute M_Slide_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentitySlide GetById(int Id)
        {
            var info = new IdentitySlide();         
            var sqlCmd = @"M_Slide_GetById";

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
                            info = ExtractSlideData(reader);
                        }
                    }
                }                            
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Slide_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_Slide_Delete";

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
                var strError = "Failed to execute M_Slide_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentitySlide> GetList()
        {
            //Common syntax            
            var sqlCmd = @"M_Slide_GetList";

            List<IdentitySlide> listData = new List<IdentitySlide>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractSlideData(reader);

                            listData.Add(record);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Slide_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #region Slide Item

        public int InsertSlideItem(IdentitySlideItem identity)
        {
            //Common syntax           
            var sqlCmd = @"M_SlideItem_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@SlideId", identity.SlideId},
                {"@Title", identity.Title },
                {"@Description", identity.Description },
                {"@ImageUrl", identity.ImageUrl },
                {"@Link", identity.Link },
                {"@LinkAction", identity.LinkAction },
                {"@SortOrder", identity.SortOrder },
                {"@Status", identity.Status}
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
                var strError = "Failed to execute M_SlideItem_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool UpdateSlideItem(IdentitySlideItem identity)
        {
            //Common syntax
            var sqlCmd = @"M_SlideItem_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@SlideId", identity.SlideId},
                {"@Title", identity.Title },
                {"@Description", identity.Description },
                {"@ImageUrl", identity.ImageUrl },
                {"@Link", identity.Link },
                {"@LinkAction", identity.LinkAction },
                {"@SortOrder", identity.SortOrder },
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
                var strError = "Failed to execute M_SlideItem_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentitySlideItem GetSlideItemById(int Id)
        {
            var info = new IdentitySlideItem();
            var sqlCmd = @"M_SlideItem_GetById";

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
                            info = ExtractSlideItemData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_SlideItem_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool DeleteSlideItem(int Id)
        {
            //Common syntax            
            var sqlCmd = @"M_SlideItem_Delete";

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
                var strError = "Failed to execute M_SlideItem_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        #endregion

        #endregion
    }
}
