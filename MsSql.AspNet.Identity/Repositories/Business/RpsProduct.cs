using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsProduct
    {
        private readonly string _connectionString;

        public RpsProduct(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsProduct()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }


        #region  Common

        public List<IdentityProduct> GetByPage(IdentityProduct filter, int currentPage, int pageSize)
        {
            //Common syntax
            var sqlCmd = @"Product_GetByPage";
            List<IdentityProduct> listData = null;

            //For paging
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Keyword", filter.Keyword },
                    {"@Status", filter.Status },
                    {"@ProviderId", filter.ProviderId },
                    {"@ProductCategoryId", filter.ProductCategoryId },
                    {"@Offset", offset },
                    {"@PageSize", pageSize }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityProduct> GetActiveForChoosen(IdentityProduct filter, int currentPage, int pageSize)
        {
            //Common syntax
            var sqlCmd = @"Product_GetActiveForChoosen";
            List<IdentityProduct> listData = null;

            //For paging
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword },
                {"@ProviderId", filter.ProviderId },
                {"@ProductCategoryId", filter.ProductCategoryId },
                {"@Offset", offset },
                {"@PageSize", pageSize }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetActiveForChoosen. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public int Insert(IdentityProduct identity)
        {
            //Common syntax
            var sqlCmd = @"Product_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Code", identity.Code },
                    {"@ProductCategoryId", identity.ProductCategoryId },
                    {"@ProviderId", identity.ProviderId },
                    {"@Name", identity.Name },
                    //{"@ShortDescription", identity.ShortDescription },
                    //{"@Detail", identity.Detail },
                    //{"@OtherInfo", identity.OtherInfo },
                    //{"@Cost", identity.Cost },
                    //{"@SaleOffCost", identity.SaleOffCost },
                    //{"@CurrencyId", identity.CurrencyId },
                    {"@MinInventory", identity.MinInventory },
                    {"@UnitId", identity.UnitId },
                    {"@CreatedBy", identity.CreatedBy },
                    //{"@CreatedDate", identity.CreatedDate },
                    //{"@LastUpdatedBy", identity.LastUpdatedBy },
                    //{"@LastUpdated", identity.LastUpdated },
                    {"@Status", identity.Status },
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
                var strError = "Failed to execute Product_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityProduct identity)
        {
            //Common syntax
            var sqlCmd = @"Product_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", identity.Id },
                    {"@Code", identity.Code },
                    {"@ProductCategoryId", identity.ProductCategoryId },
                    {"@ProviderId", identity.ProviderId },
                    {"@Name", identity.Name },
                    //{"@ShortDescription", identity.ShortDescription },
                    //{"@Detail", identity.Detail },
                    //{"@OtherInfo", identity.OtherInfo },
                    //{"@Cost", identity.Cost },
                    //{"@SaleOffCost", identity.SaleOffCost },
                    //{"@CurrencyId", identity.CurrencyId },
                    {"@MinInventory", identity.MinInventory },
                    {"@UnitId", identity.UnitId },
                    {"@LastUpdatedBy", identity.LastUpdatedBy },
                    {"@Status", identity.Status }
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
                var strError = "Failed to execute Product_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityProduct GetById(int id)
        {
            //Common syntax
            var sqlCmd = @"Product_GetById";
            IdentityProduct info = null;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", id }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractProductData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public IdentityProduct GetDetail(int id)
        {
            //Common syntax
            var sqlCmd = @"Product_GetDetail";
            IdentityProduct info = null;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", id }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractProductData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public IdentityProduct GetByCode(string code)
        {
            //Common syntax
            var sqlCmd = @"Product_GetByCode";
            IdentityProduct info = null;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Code", code }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractProductData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetByCode. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public bool Delete(int id)
        {
            //Common syntax
            var sqlCmd = @"Product_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", id }
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
                var strError = "Failed to execute Product_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        #endregion

        #region  Helpers

        private List<IdentityProduct> ParsingListFromReader(IDataReader reader)
        {
            List<IdentityProduct> listData = listData = new List<IdentityProduct>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractProductData(reader);

                //Extends information
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }
            return listData;
        }

        public static IdentityProduct ExtractProductData(IDataReader reader)
        {
            var record = new IdentityProduct();

            //Seperate properties;
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Code = reader["Code"].ToString();
            record.ProductCategoryId = Utils.ConvertToInt32(reader["ProductCategoryId"]);
            record.ProviderId = Utils.ConvertToInt32(reader["ProviderId"]);
            record.Name = reader["Name"].ToString();
            //record.ShortDescription = reader["ShortDescription"].ToString();
            //record.Detail = reader["Detail"].ToString();
            //record.OtherInfo = reader["OtherInfo"].ToString();
            //record.Cost = reader["Cost"].ToString();
            //record.SaleOffCost = reader["SaleOffCost"].ToString();
            //record.UnitId = Utils.ConvertToInt32(reader["UnitId"]);
            //record.CurrencyId = Utils.ConvertToInt32(reader["CurrencyId"]);  
            record.MinInventory = Utils.ConvertToDouble(reader["MinInventory"]);
            record.UnitId = Utils.ConvertToInt32(reader["UnitId"]);
            //record.CreatedBy = Utils.ConvertToInt32(reader["CreatedBy"]);
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            //record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
            //record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            if (reader.HasColumn("WarehouseNum"))
                record.WarehouseNum = Utils.ConvertToDouble(reader["WarehouseNum"]);

            return record;
        }

        #endregion
    }

}
