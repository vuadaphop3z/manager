using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsPlace
    {
        private readonly string _connectionString;

        public RpsPlace(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsPlace()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PfoodDBConnection"].ConnectionString;
        }

        public List<IdentityPlace> GetAll(IdentityPlace filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"M_Place_GetByPage";
            List<IdentityPlace> listData = null;

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
                        listData = ParsingListPlaceFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute M_Place_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityPlace GetById(int Id)
        {
            var info = new IdentityPlace();
            var sqlCmd = @"Place_GetPlaceById";

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
                            info = ExtractPlaceData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Place_GetPlaceById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public int Insert(IdentityPlace identity, string language)
        {
            var newId = 0;
            var sqlCmd = @"Place_Insert";

            var parameters = new Dictionary<string, object>
            {
                {"@GName", identity.GName},
                {"@GFullName", identity.GFullName},
                {"@GType", identity.GType},
                {"@GPlaceId", identity.GPlaceId},
                {"@GId", identity.GId},
                {"@GLat", identity.GLat},
                {"@GLong", identity.GLong},
                {"@GUrl", identity.GUrl},
                {"@Icon", identity.Icon},
                {"@RawData", identity.RawData},
                {"@Province", identity.Province},
                {"@District", identity.District},
                {"@IsProvince", identity.IsProvince},
                {"@Languages",language}
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
                var strError = "Failed to Place_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityPlace identity)
        {
            //Common syntax
            var sqlCmd = @"M_Place_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@GName", identity.GName},
                {"@Cover", identity.Cover },
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
                var strError = "Failed to execute M_Place_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        private List<IdentityPlace> ParsingListPlaceFromReader(IDataReader reader)
        {
            List<IdentityPlace> listData = listData = new List<IdentityPlace>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractPlaceData(reader);

                //Extends information
                if (listData != null)
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityPlace ExtractPlaceData(IDataReader reader, bool getShort = false)
        {
            var record = new IdentityPlace();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.GName = reader["GName"].ToString();
            record.GFullName = reader["GFullName"].ToString();
            record.GId = reader["GId"].ToString();
            record.GLat = reader["GLat"].ToString();
            record.GLong = reader["GLong"].ToString();
            record.GUrl = reader["GUrl"].ToString();
            record.Icon = reader["Icon"].ToString();
            record.ProvinceId = Utils.ConvertToInt32(reader["ProvinceId"]);
            record.DistrictId = Utils.ConvertToInt32(reader["DistrictId"]);

            if (!getShort)
            {
                record.GType = reader["GType"].ToString();
                record.GPlaceId = reader["GPlaceId"].ToString();
                record.Province = reader["Province"].ToString();
                record.District = reader["District"].ToString();
                record.RawData = reader["RawData"].ToString();
            }

            record.Status = Utils.ConvertToInt32(reader["Status"]);
            record.PostCount = Utils.ConvertToInt32(reader["PostCount"]);
            record.PostCountProvince = Utils.ConvertToInt32(reader["PostCountProvince"]);
            record.Cover = reader["Cover"].ToString();

            return record;
        }

        public List<IdentityPlace> GetByProvince(int provinceId)
        {
            //Common syntax           
            var sqlCmd = @"Place_GetByProvince";
            List<IdentityPlace> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ProvinceId", provinceId }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityPlace>();
                        while (reader.Read())
                        {
                            var item = ExtractPlaceData(reader);

                            listData.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Place_GetByProvince. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityPlace GetPlaceById(int Id, bool getShort = false)
        {
            //Common syntax           
            var sqlCmd = @"Place_GetPlaceById";
            IdentityPlace record = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        record = new IdentityPlace();
                        if (reader.Read())
                        {
                            record = ExtractPlaceData(reader, getShort);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Place_GetPlaceByUrl. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return record;
        }

        public List<IdentityPlace> GetFromList(string placesList)
        {
            //Common syntax           
            var sqlCmd = @"Place_GetFromList";
            List<IdentityPlace> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ListPlaces", placesList }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityPlace>();
                        while (reader.Read())
                        {
                            var item = ExtractPlaceData(reader);
                            listData.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Place_GetFromList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }
    }
}
