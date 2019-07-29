﻿using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsPolicy
    {
        private readonly string _connectionString;

        public RpsPolicy(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsPolicy()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityPolicy> GetAll(IdentityPolicy filter, int currentPage, int pageSize)
        {           
            //Common syntax           
            var sqlCmd = @"Policy_GetAll";
            List<IdentityPolicy> listData = null;

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
                using(var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListPolicyFromReader(reader);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Policy_GetAll. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityPolicy> ParsingListPolicyFromReader(IDataReader reader)
        {
            List<IdentityPolicy> listData = listData = new List<IdentityPolicy>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractPolicyData(reader);

                //Extends information
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityPolicy ExtractPolicyData(IDataReader reader)
        {
            var record = new IdentityPolicy();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();

            record.CreatedBy = reader["CreatedBy"].ToString();
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public int Insert(IdentityPolicy identity)
        {
            //Common syntax           
            var sqlCmd = @"Policy_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code },               
                {"@CreatedBy", identity.CreatedBy},
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
                var strError = "Failed to execute Policy_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityPolicy identity)
        {
            //Common syntax
            var sqlCmd = @"Policy_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},                
                {"@Name", identity.Name},
                {"@Code", identity.Code },                
                {"@LastUpdatedBy", identity.LastUpdatedBy},
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
                var strError = "Failed to execute Policy_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityPolicy GetById(int Id)
        {
            var info = new IdentityPolicy();         
            var sqlCmd = @"Policy_GetById";

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
                            info = ExtractPolicyData(reader);
                        }
                    }
                }                            
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Policy_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Policy_Delete";

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
                var strError = "Failed to execute Policy_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityPolicy> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Policy_GetList";

            List<IdentityPolicy> listData = new List<IdentityPolicy>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractPolicyData(reader);

                            listData.Add(record);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Policy_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion
    }
}
