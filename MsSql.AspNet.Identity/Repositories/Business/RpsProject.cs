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
    public class RpsProject
    {
        private readonly string _connectinString;

        public RpsProject(string connectionString)
        {
            _connectinString = connectionString;
        }

        public RpsProject()
        {
            _connectinString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public List<IdentityProject> GetByPage(IdentityProject filter, int currentPage, int pageSize)
        {
            var sqlCmd = @"Project_GetByPage";

            List<IdentityProject> listData = null;

            int offset = (currentPage - 1) * pageSize;

            var paramaters = new Dictionary<string, object>
            {
                {@"Keyword", filter.Keyword },
                {@"Status", filter.Status },
                {@"offset", offset },
                {@"pageSize", pageSize },
            };

            try
            {
                using (var conn = new SqlConnection(_connectinString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure,sqlCmd, paramaters))
                    {
                        listData = ParsingListFromReader(reader);
                    }
                }

            }
            catch (Exception ex)
            {

                var strError = "Failed to execute Project_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityProject GetById(int Id)
        {
            var sqlCmd = @"Project_GetById";

            IdentityProject info = null;

            var parameters = new Dictionary<string, object>
            {
                {@"Id", Id }
            };

            try
            {
                using(var conn = new SqlConnection(_connectinString))
                {
                    using(var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractProjectData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                var strError = "Failed to execute Project_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;

        }

        public List<IdentityProject> GetList()
        {
            var sqlCmd = @"Project_GetList";

            List<IdentityProject> listData = new List<IdentityProject>();


            try
            {
                using (var conn = new SqlConnection(_connectinString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractProjectData(reader);
                            listData.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                var strError = "Failed to execute Project_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;

        }

        public int Insert(IdentityProject identity)
        {
            var sqlCmd = @"Project_Insert";

            var newId = 0;

            var strNow = DateTime.Now.ToString("dd/mm/yyyy h:mm");

            var parameters = new Dictionary<string, object>
            {
                {@"Code",identity.Code  },
                {@"ProjectCategoryId",identity.ProjectCategoryId  },
                {@"CompanyId",identity.CompanyId },
                {@"Name",identity.Name  },
                {@"ShortDescription",identity.ShortDescription  },
                {@"Detail",identity.Detail  },
                {@"BeginDate",identity.BeginDate  },
                {@"FinishDate",identity.FinishDate },
                {@"CreatedBy",identity.CreatedBy  },
                {@"CreatedDate", strNow  },
                {@"Status",identity.Status  },
            };

            try
            {
                using (var conn = new SqlConnection(_connectinString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    newId = Convert.ToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {

                var strError = "Failed to execute Project_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;

        }

        public bool Update(IdentityProject identity)
        {
            var sqlCmd = @"Project_Update";

            var strNow = DateTime.Now.ToString("dd/mm/yyyy h:mm");

            var parameters = new Dictionary<string, object>
            {
                {@"Id", identity.Id },
                {@"Code",identity.Code  },
                {@"ProjectCategoryId",identity.ProjectCategoryId  },
                {@"CompanyId",identity.CompanyId },
                {@"Name",identity.Name  },
                {@"ShortDescription",identity.ShortDescription  },
                {@"Detail",identity.Detail  },
                {@"BeginDate",identity.BeginDate  },
                {@"FinishDate",identity.FinishDate },
                {@"CreatedBy",identity.CreatedBy  },
                {@"CreatedDate", strNow  },
                {@"Status",identity.Status  },
            };

            try
            {
                using (var conn = new SqlConnection(_connectinString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }

            }
            catch (Exception ex)
            {

                var strError = "Failed to execute Project_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;

        }

        public bool Delete(int Id)
        {
            var sqlCmd = @"Project_Delete";

            var parameters = new Dictionary<string, object>
            {
                {@"Id", Id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectinString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }

            }
            catch (Exception ex)
            {

                var strError = "Failed to execute Project_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;

        }


        public List<IdentityProject> ParsingListFromReader(IDataReader reader)
        {
            List<IdentityProject> listData = new List<IdentityProject>();
            while (reader.Read())
            {
                var record = ExtractProjectData(reader);

                //Extends information
                if (reader.HasColumn("TotalCount"))
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        public IdentityProject ExtractProjectData(IDataReader reader)
        {
            var record = new IdentityProject();

            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Code = reader["Code"].ToString();
            record.ProjectCategoryId = Utils.ConvertToInt32(reader["ProjectCategoryId"]);
            record.CompanyId = Utils.ConvertToInt32(reader["CompanyId"]);
            record.Name = reader["Name"].ToString();
            record.ShortDescription = reader["ShortDescription"].ToString();
            record.Detail = reader["Detail"].ToString();
            record.BeginDate = reader["BeginDate"] == DBNull.Value ? null : (DateTime?)reader["BeginDate"];
            record.FinishDate = reader["FinishDate"] == DBNull.Value ? null : (DateTime?)reader["FinishDate"];
            record.CreatedBy = Utils.ConvertToInt32(reader["CreatedBy"]);
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            //record.LastUpdatedBy = Utils.ConvertToInt32(reader["LastUpdatedBy"]);
            //record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            record.Status = Utils.ConvertToInt32(reader["Status"]);


            return record;
        }
    }
}
