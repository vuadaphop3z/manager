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

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsProjectCategory
    {
        private readonly string _connectionString;

        public RpsProjectCategory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsProjectCategory()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public List<IdentityProjectCategory> GetByPage(IdentityProjectCategory filter, int currentPage, int pageSize)
        {

            var sqlCmd = @"ProjectCategory_GetByPage";

            int offset = (currentPage - 1) * pageSize;

            List<IdentityProjectCategory> listData = null;

            var paramaters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@Status", filter.Status},
                {"@Offset", offset},
                {"@PageSize", pageSize},       
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using(var reader =  MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, paramaters))
                    {
                        listData = ParsingListFromReader(reader);
                    }
                }

            }
            catch (Exception ex)
            {

                var strError = "Failed to execute ProjectCategory_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityProjectCategory GetById(int Id)
        {
            var sqlCmd = @"ProjectCategory_GetById";

            IdentityProjectCategory info = null;

            var paramaters = new Dictionary<string, object>
            {
                {"Id", Id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using(var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, paramaters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractProjectCategory(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                var strError = "Failed to execute ProjectCategory_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;

        }
        public List<IdentityProjectCategory> GetList()
        {

            var sqlCmd = @"ProjectCategory_GetList";

            List<IdentityProjectCategory> listData = null;

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

                var strError = "Failed to execute ProjectCategory_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public int Insert(IdentityProjectCategory identity)
        {
            var sqlCmd = @"ProjectCategory_Insert";

            var newId = 0;

            var paramaters = new Dictionary<string, object>
            {
                {"Code", identity.Code },
                {"Name", identity.Name },
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

                var strError = "Failed to execute ProjectCategory_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return newId;
  
        }

        public bool Update(IdentityProjectCategory identity)
        {
            var sqlCmd = @"ProjectCategory_Update";

            var paramaters = new Dictionary<string, object>
            {
                {"Id", identity.Id },
                {"Code", identity.Code },
                {"Name", identity.Name },
                {"Status", identity.Status },
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

                var strError = "Failed to execute ProjectCategory_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;

        }

        public bool Delete(int Id)
        {
            var sqlCmd = @"ProjectCategory_Delete";
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
                throw new CustomSQLException(strError); throw;
            }

            return true;

        }


        public static List<IdentityProjectCategory> ParsingListFromReader(IDataReader reader)
        {
            List<IdentityProjectCategory> listData = new List<IdentityProjectCategory>();

            while (reader.Read())
            {
                var record = ExtractProjectCategory(reader);

                //Extends information
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }
            return listData;
        }
        public static IdentityProjectCategory ExtractProjectCategory(IDataReader reader)
        {
            var record = new IdentityProjectCategory();
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Code = reader["Code"].ToString();
            record.Name = reader["Name"].ToString();
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

    }
}
