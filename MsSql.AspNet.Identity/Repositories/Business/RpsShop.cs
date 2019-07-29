using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsShop
    {
        private readonly string _connectionString;

        public RpsShop(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsShop()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }

        public List<IdentityShop> GetByPage(IdentityShop filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"M_Shop_GetByPage";
            List<IdentityShop> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword },
                {"@Status", filter.Status },
                {"@CategoryId", filter.CategoryId },
                {"@Offset", offset},
                {"@PageSize", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListShopFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Shop_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityShop> ParsingListShopFromReader(IDataReader reader)
        {
            List<IdentityShop> listData = listData = new List<IdentityShop>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractShopData(reader);

                //Extends information
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityShop ExtractShopData(IDataReader reader)
        {
            var record = new IdentityShop();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.IsInternal = Convert.ToInt32(reader["IsInternal"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();
            record.Cover = reader["Cover"].ToString();
            record.ProviderId = Utils.ConvertToInt32(reader["ProviderId"]);
            record.Phone = reader["Phone"].ToString();
            record.Address = reader["Address"].ToString();
            record.ContactInfo = reader["ContactInfo"].ToString();
            record.AreaId = Utils.ConvertToInt32(reader["AreaId"]);
            record.CountryId = Utils.ConvertToInt32(reader["CountryId"]);
            record.ProvinceId = Utils.ConvertToInt32(reader["ProvinceId"]);
            record.DistrictId = Utils.ConvertToInt32(reader["DistrictId"]);
            record.Openned = Utils.ConvertToInt32(reader["Openned"]);
            record.PostCode = reader["PostCode"].ToString();
            record.Description = reader["Description"].ToString();
            record.Longitude = reader["Longitude"].ToString();
            record.Latitude = reader["Latitude"].ToString();

            record.CreatedBy = reader["CreatedBy"].ToString();
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();

            record.LastBooking = reader["LastBooking"] == DBNull.Value ? null : (DateTime?)reader["LastBooking"];
            record.Status = Utils.ConvertToInt32(reader["Status"]);
            record.Email = reader["Email"].ToString();
            record.CategoryId = Utils.ConvertToInt32(reader["CategoryId"]);

            return record;
        }

        private List<MetaShopImage> ExtractShopImageData(IDataReader reader)
        {
            var myList = new List<MetaShopImage>();
            while (reader.Read())
            {
                var record = new MetaShopImage();

                //Seperate properties
                record.Id = reader["Id"].ToString();
                record.ShopId = Utils.ConvertToInt32(reader["ShopId"]);
                record.Name = reader["Name"].ToString();
                record.Url = reader["Url"].ToString();
                record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];

                myList.Add(record);
            }

            return myList;
        }

        private MetaDataShop ExtractShopMetaData(IDataReader reader)
        {
            var record = new MetaDataShop();
            if (reader.Read())
            {
                //Seperate properties
                record.ShopId = Utils.ConvertToInt32(reader["ShopId"]);
                record.Amenities = reader["Amenities"].ToString();
                record.NearPlaces = reader["NearPlaces"].ToString();
                record.Policies = reader["Policies"].ToString();
                record.Payments = reader["Payments"].ToString();
            }

            return record;
        }

        public int Insert(IdentityShop identity)
        {
            //Common syntax           
            var sqlCmd = @"M_Shop_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@IsInternal", identity.IsInternal },
                {"@Name", identity.Name},
                {"@Code", identity.Code },
                {"@ProviderId", identity.ProviderId },
                {"@Phone", identity.Phone},
                {"@Address", identity.Address},
                {"@ContactInfo", identity.ContactInfo},
                {"@AreaId", identity.AreaId},
                {"@CountryId", identity.CountryId},
                {"@ProvinceId", identity.ProvinceId},
                {"@DistrictId", identity.DistrictId},
                {"@Openned", identity.Openned},
                {"@PostCode", identity.PostCode},
                {"@Description", identity.Description},
                {"@CreatedBy", identity.CreatedBy},
                {"@Status", identity.Status},
                {"@ShopType", identity.ShopType},
                {"@Email", identity.Email}
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
                var strError = "Failed to execute M_Shop_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityShop identity)
        {
            //Common syntax
            var sqlCmd = @"M_Shop_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@IsInternal", identity.IsInternal },
                {"@Name", identity.Name},
                {"@Code", identity.Code },
                {"@ProviderId", identity.ProviderId },
                {"@Phone", identity.Phone},
                {"@Address", identity.Address},
                {"@ContactInfo", identity.ContactInfo},
                {"@AreaId", identity.AreaId},
                {"@CountryId", identity.CountryId},
                {"@ProvinceId", identity.ProvinceId},
                {"@DistrictId", identity.DistrictId},
                {"@Openned", identity.Openned},
                {"@PostCode", identity.PostCode},
                {"@Description", identity.Description},
                {"@LastUpdatedBy", identity.LastUpdatedBy},
                {"@Status", identity.Status},
                {"@ShopType", identity.ShopType},
                {"@Email", identity.Email}
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
                var strError = "Failed to execute M_Shop_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityShop GetById(int Id)
        {
            var info = new IdentityShop();
            var sqlCmd = @"Shop_GetById";

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
                        //Base information
                        if (reader.Read())
                        {
                            info = ExtractShopData(reader);
                        }

                        //Images
                        if (reader.NextResult())
                        {
                            info.Images = ExtractShopImageData(reader);
                        }

                        //Meta data
                        if (reader.NextResult())
                        {
                            info.MetaData = ExtractShopMetaData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Shop_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Shop_Delete";

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
                var strError = "Failed to execute Shop_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityShop> GetList(string keyword)
        {
            //Common syntax            
            var sqlCmd = @"M_Shop_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", keyword},
            };

            List<IdentityShop> listData = new List<IdentityShop>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = new IdentityShop();
                            record.Id = Utils.ConvertToInt32(reader["Id"]);
                            record.Name = reader["Name"].ToString();

                            listData.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Shop_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityShop> GetAssigned(int userId)
        {
            //Common syntax            
            var sqlCmd = @"M_Shop_GetAssigned";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@UserId", userId},
            };

            List<IdentityShop> listData = new List<IdentityShop>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = new IdentityShop();
                            record.Id = Utils.ConvertToInt32(reader["Id"]);
                            record.Name = reader["Name"].ToString();
                            record.IsOwner = Utils.ConvertToBoolean(reader["IsOwner"]);

                            listData.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Shop_GetAssigned. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityShopMember> GetAssignedMembers(int shopId)
        {
            //Common syntax            
            var sqlCmd = @"M_Shop_GetAssignedMembers";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", shopId},
            };

            List<IdentityShopMember> listData = new List<IdentityShopMember>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = new IdentityShopMember();
                            record.MemberId = Utils.ConvertToInt32(reader["UserId"]);
                            record.IsOwner = Utils.ConvertToBoolean(reader["IsOwner"]);

                            listData.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Shop_GetAssignedMembers. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public bool UpdateMap(int shopId, string latitude, string longitude)
        {
            //Common syntax
            var sqlCmd = @"Shop_UpdateMap";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", shopId},
                {"@Latitude", latitude},
                {"@Longitude", longitude}
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
                var strError = "Failed to execute Shop_UpdateMap. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public int AssignToUser(int shopId, int userId, bool isOwner)
        {
            //Common syntax
            var sqlCmd = @"M_Shop_AssignToUser";
            var result = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", shopId},
                {"@UserId", userId},
                {"@IsOwner", isOwner}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                        result = Utils.ConvertToInt32(reader[0]);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Shop_AssignToUser. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return result;
        }

        public bool DeleteAssignedUser(int shopId, int userId)
        {
            //Common syntax
            var sqlCmd = @"M_Shop_DeleteAssignedUser";
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", shopId},
                {"@UserId", userId}
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
                var strError = "Failed to execute M_Shop_DeleteAssignedUser. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public int AssignCategory(int shopId, int categoryId)
        {
            //Common syntax
            var sqlCmd = @"M_Shop_AssignCategory";
            var result = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", shopId},
                {"@CategoryId", categoryId}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                            result = Utils.ConvertToInt32(reader[0]);
                    }                   
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Shop_AssignCategory. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return result;
        }

        #region Meta data

        #region Images

        public bool AddNewImage(MetaShopImage identity)
        {
            //Common syntax
            var sqlCmd = @"Shop_AddNewImage";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@ShopId", identity.ShopId},
                {"@Name", identity.Name},
                {"@Url", identity.Url}
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
                var strError = "Failed to execute Shop_AddNewImage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool RemoveImage(string Id)
        {
            //Common syntax
            var sqlCmd = @"Shop_RemoveImage";

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
                var strError = "Failed to execute Shop_RemoveImage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<MetaShopImage> GetListImage(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Shop_GetListImage";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", Id},
            };

            List<MetaShopImage> listData = new List<MetaShopImage>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var item = new MetaShopImage();

                            item.Id = reader["Id"].ToString();
                            item.ShopId = Utils.ConvertToInt32(reader["ShopId"]);
                            item.Name = reader["Name"].ToString();
                            item.Url = reader["Url"].ToString();
                            item.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];

                            listData.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Shop_GetListImage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion

        #region Amenities

        public bool UpdateAmenities(int shopId, string selectedValues)
        {
            //Common syntax
            var sqlCmd = @"Shop_UpdateAmenities";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", shopId},
                {"@Amenities", selectedValues}
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
                var strError = "Failed to execute Shop_UpdateAmenities. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        #endregion

        #region NearPlaces

        public string GetNearPlaces(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Shop_GetNearPlaces";
            var myNearPlacesJson = string.Empty;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", Id},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            myNearPlacesJson = reader["NearPlaces"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Shop_GetNearPlaces. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myNearPlacesJson;
        }

        public bool UpdateNearPlaces(int shopId, string nearPlaces)
        {
            //Common syntax
            var sqlCmd = @"Shop_UpdateNearPlaces";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", shopId},
                {"@NearPlaces", nearPlaces}
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
                var strError = "Failed to execute Shop_UpdateNearPlaces. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        #endregion

        #region Policies

        public string GetPolicies(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Shop_GetPolicies";
            var myPoliciesJson = string.Empty;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", Id},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            myPoliciesJson = reader["Policies"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Shop_GetPolicies. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myPoliciesJson;
        }

        public bool UpdatePolicies(int shopId, string policies)
        {
            //Common syntax
            var sqlCmd = @"Shop_UpdatePolicies";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", shopId},
                {"@Policies", policies}
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
                var strError = "Failed to execute Shop_UpdatePolicies. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        #endregion

        #region Payments

        public bool UpdatePayments(int shopId, string selectedValues)
        {
            //Common syntax
            var sqlCmd = @"Shop_UpdatePayments";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ShopId", shopId},
                {"@Payments", selectedValues}
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
                var strError = "Failed to execute Shop_UpdatePayments. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        #endregion

        #endregion
    }
}
