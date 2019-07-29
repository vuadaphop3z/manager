using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores.Business;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsArea
    {
        private readonly string _connectionString;

        public RpsArea(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsArea()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region Area

        private List<IdentityArea> ParsingListAreaFromReader(IDataReader reader)
        {
            List<IdentityArea> listData = listData = new List<IdentityArea>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractAreaData(reader);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityArea ExtractAreaData(IDataReader reader)
        {
            var record = new IdentityArea();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();

            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public bool Area_Insert(IdentityArea identity)
        {
            //Common syntax           
            var sqlCmd = @"Area_Insert";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code},
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
                var strError = "Failed to execute Area_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool Area_Update(IdentityArea identity)
        {
            //Common syntax
            var sqlCmd = @"Area_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Code", identity.Code},
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
                var strError = "Failed to execute Area_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityArea Area_GetById(int Id)
        {
            var info = new IdentityArea();
            var sqlCmd = @"Area_GetById";

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
                            info = ExtractAreaData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Area_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Area_Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Area_Delete";

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
                var strError = "Failed to execute Area_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityArea> Area_GetList()
        {
            //Common syntax            
            var sqlCmd = @"Area_GetList";

            List<IdentityArea> listData = new List<IdentityArea>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        listData = ParsingListAreaFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Area_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion        

        #region Country

        private List<IdentityCountry> ParsingListCountryFromReader(IDataReader reader)
        {
            List<IdentityCountry> listData = listData = new List<IdentityCountry>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractCountryData(reader);

                //Extends information
                //record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityCountry ExtractCountryData(IDataReader reader)
        {
            var record = new IdentityCountry();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();

            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public bool Country_Insert(IdentityCountry identity)
        {
            //Common syntax           
            var sqlCmd = @"Country_Insert";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code},
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
                var strError = "Failed to execute Country_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool Country_Update(IdentityCountry identity)
        {
            //Common syntax
            var sqlCmd = @"Country_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Code", identity.Code},
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
                var strError = "Failed to execute Country_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityCountry Country_GetById(int Id)
        {
            var info = new IdentityCountry();
            var sqlCmd = @"Country_GetById";

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
                            info = ExtractCountryData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Country_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Country_Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Country_Delete";

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
                var strError = "Failed to execute Country_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityCountry> Country_GetList()
        {
            //Common syntax            
            var sqlCmd = @"Country_GetList";

            List<IdentityCountry> listData = new List<IdentityCountry>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        listData = ParsingListCountryFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Country_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityCountry> Country_GetByArea(int areaId)
        {
            //Common syntax            
            var sqlCmd = @"Country_GetByArea";
            var parameters = new Dictionary<string, object>
            {
                {"@AreaId", areaId}
            };

            List<IdentityCountry> listData = new List<IdentityCountry>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListCountryFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Country_GetByArea. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion        

        #region Province

        private List<IdentityProvince> ParsingListProvinceFromReader(IDataReader reader)
        {
            List<IdentityProvince> listData = listData = new List<IdentityProvince>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractProvinceData(reader);

                //Extends information
                //record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityProvince ExtractProvinceData(IDataReader reader)
        {
            var record = new IdentityProvince();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();

            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public bool Province_Insert(IdentityProvince identity)
        {
            //Common syntax           
            var sqlCmd = @"Province_Insert";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code},
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
                var strError = "Failed to execute Province_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool Province_Update(IdentityProvince identity)
        {
            //Common syntax
            var sqlCmd = @"Province_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Code", identity.Code},
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
                var strError = "Failed to execute Province_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityProvince Province_GetById(int Id)
        {
            var info = new IdentityProvince();
            var sqlCmd = @"Province_GetById";

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
                            info = ExtractProvinceData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Province_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Province_Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Province_Delete";

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
                var strError = "Failed to execute Province_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityProvince> Province_GetList()
        {
            //Common syntax            
            var sqlCmd = @"Province_GetList";

            List<IdentityProvince> listData = new List<IdentityProvince>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        listData = ParsingListProvinceFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Province_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityProvince> Province_GetByCountry(int countryId)
        {
            //Common syntax            
            var sqlCmd = @"Province_GetByCountry";
            var parameters = new Dictionary<string, object>
            {
                {"@CountryId", countryId}
            };

            List<IdentityProvince> listData = new List<IdentityProvince>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListProvinceFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Province_GetByCountry. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion        

        #region District

        private List<IdentityDistrict> ParsingListDistrictFromReader(IDataReader reader)
        {
            List<IdentityDistrict> listData = listData = new List<IdentityDistrict>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractDistrictData(reader);

                //Extends information
                //record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityDistrict ExtractDistrictData(IDataReader reader)
        {
            var record = new IdentityDistrict();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();

            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public bool District_Insert(IdentityDistrict identity)
        {
            //Common syntax           
            var sqlCmd = @"District_Insert";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code},
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
                var strError = "Failed to execute District_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool District_Update(IdentityDistrict identity)
        {
            //Common syntax
            var sqlCmd = @"District_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Code", identity.Code},
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
                var strError = "Failed to execute District_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityDistrict District_GetById(int Id)
        {
            var info = new IdentityDistrict();
            var sqlCmd = @"District_GetById";

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
                            info = ExtractDistrictData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute District_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool District_Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"District_Delete";

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
                var strError = "Failed to execute District_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityDistrict> District_GetList()
        {
            //Common syntax            
            var sqlCmd = @"District_GetList";

            List<IdentityDistrict> listData = new List<IdentityDistrict>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        listData = ParsingListDistrictFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute District_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityDistrict> District_GetByProvince(int provinceId)
        {
            //Common syntax            
            var sqlCmd = @"District_GetByProvince";

            var parameters = new Dictionary<string, object>
            {
                {"@ProvinceId", provinceId}
            };

            List<IdentityDistrict> listData = new List<IdentityDistrict>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListDistrictFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute District_GetByProvince. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion        
    }
}
