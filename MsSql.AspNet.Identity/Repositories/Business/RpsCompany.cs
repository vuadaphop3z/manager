using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Repositories.Business
{
    public class RpsCompany
    {

        private readonly string _connectionString;

        public RpsCompany(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsCompany()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

     
        public List<IdentityCompany> GetByPage(IdentityCompany filter, int currentPage, int pageSize)
        {
            var sqlCmd = @"Company_GetByPage";
            List<IdentityCompany> listdata = null;

            int offset = (currentPage - 1) * pageSize;

            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword }, 
                {"@Adress", filter.Address }, 
                {"@Offset", offset},
                {"@PageSize", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listdata = ParsingListFromReader(reader);
                    }
                }

            }
            catch (Exception ex)
            {

                var strError = "Failed to execute Company_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }


            return listdata;
        }

        public IdentityCompany GetById(int Id)
        {
            var sqlCmd = @"Company_GetById";

           IdentityCompany  info = null;

            var paramaters = new Dictionary<string, object>
            {
                {"Id", Id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader =  MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, paramaters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractCompanyData(reader);
                        }
            
                    }

                }
            }

            catch (Exception ex)
            {
                var strError = "Failed to execute Company_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;

        }

        public List<IdentityCompany> GetList()
        {
            var sqlCmd = @"Company_GetList";
            List<IdentityCompany> listData = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        listData = ParsingListFromReader(reader);
                    }
                }

            }
            catch (Exception ex)
            {

                var strError = "Failed to execute Company_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;

        }

        public int Insert(IdentityCompany identity)
        {

            var sqlCmd = @"Company_Insert";

            var newId = 0;
            
            var paramaters = new Dictionary<string, object>
            {
                {"Code", identity.Code },
                {"Name", identity.Name },
                {"CountryId", identity.CountryId },
                {"ProvinceId", identity.ProvinceId },
                {"DistrictId", identity.DistrictId },
                {"Address", identity.Address },
                {"Email", identity.Email },
                {"Phone", identity.Phone },
                {"IsEPE", identity.IsEPE },
                {"Status", identity.Status },
            };

            try
            {
                using (var connn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(connn, CommandType.StoredProcedure, sqlCmd, paramaters);

                    newId = Convert.ToInt32(returnObj);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Company_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;

        }

        public bool Update(IdentityCompany identity)
        {
            var sqlCmd = @"Company_Update";

            var paramaters = new Dictionary<string, object>
            {
                {"Id", identity.Id },
                {"Code", identity.Code},
                {"Name", identity.Name},
                {"CountryId", identity.CountryId },
                {"ProvinceId", identity.ProvinceId },
                {"DistrictId", identity.DistrictId },
                {"Address", identity.Address},
                {"Email", identity.Email},
                {"Phone", identity.PageIndex},
                {"IsEPE", identity.IsEPE},
                {"Status", identity.Status},

            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, paramaters);
                }
            }
            catch (Exception ex)
            {

                var strError = "Failed to execute Company_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool Delete(int Id)

        {
            var sqlCmd = @"Company_Delete";

            var paramaters = new Dictionary<string, object>
            {
                {"Id", Id }

            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, paramaters);
                }
            }
            catch (Exception ex)
            {

                var strError = "Failed to execute Company_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;

        }

        private List<IdentityCompany> ParsingListFromReader(IDataReader reader)
        {
            List<IdentityCompany> listData = new List<IdentityCompany>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractCompanyData(reader);

                //Extends information
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }



        public static IdentityCompany ExtractCompanyData(IDataReader reader)
        {
            var record = new IdentityCompany();

            //Seperate properties;
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Code = reader["Code"].ToString();
            record.Name = reader["Name"].ToString();
            record.CountryId = Utils.ConvertToInt32(reader["CountryId"]);
            record.ProvinceId = Utils.ConvertToInt32(reader["ProvinceId"]);
            record.DistrictId = Utils.ConvertToInt32(reader["DistrictId"]);
            record.Address = reader["Address"].ToString();
            record.Email = reader["Email"].ToString();
            record.Phone = reader["Phone"].ToString();
            //record.Lat = reader["Lat"].ToString();
            //record.Long = reader["Long"].ToString();
            record.IsEPE = Utils.ConvertToBoolean(reader["IsEPE"]);
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

    }
}
