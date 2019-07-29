using Manager.SharedLib.Extensions;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsWarehouse
    {
        private readonly string _connectionString;

        public RpsWarehouse(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsWarehouse()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityWarehouse> GetByPage(IdentityWarehouse filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Warehouse_GetByPage";
            List<IdentityWarehouse> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ProductCode", filter.ProductCode },
                {"@Keyword", filter.Keyword },
                {"@IsStockOut", filter.IsStockOut },
                {"@IsStockTakeQTY", filter.IsStockTakeQTY },
                {"@Offset", offset},
                {"@PageSize", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListWarehouseFromReader(reader);

                        if (listData.Count > 0)
                        {
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    var productInfo = RpsProduct.ExtractProductData(reader);
                                    foreach (var item in listData)
                                    {
                                        if (productInfo.Id == item.ProductId)
                                        {
                                            item.ProductInfo = productInfo;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Warehouse_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityWarehouse> GetProductStockOutByPage(IdentityWarehouse filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Warehouse_GetProductStockOutByPage";
            List<IdentityWarehouse> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {               
                {"@Offset", offset},
                {"@PageSize", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListWarehouseFromReader(reader);

                        if (listData.Count > 0)
                        {
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    var productInfo = RpsProduct.ExtractProductData(reader);
                                    foreach (var item in listData)
                                    {
                                        if (productInfo.Id == item.ProductId)
                                        {
                                            item.ProductInfo = productInfo;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Warehouse_GetProductStockOutByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityWarehouse> ParsingListWarehouseFromReader(IDataReader reader)
        {
            List<IdentityWarehouse> listData = new List<IdentityWarehouse>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractWarehouseData(reader);

                //Extends information
                if (reader.HasColumn("TotalCount"))
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityWarehouse ExtractWarehouseData(IDataReader reader)
        {
            var record = new IdentityWarehouse();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.ProductId = Utils.ConvertToInt32(reader["ProductId"]);
            record.WarehouseNum = Utils.ConvertToDouble(reader["WarehouseNum"]);
            record.StockTakeQTY = Utils.ConvertToDouble(reader["StockTakeQTY"]);

            //record.CreatedBy = reader["CreatedBy"].ToString();
            //record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            //record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            //record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
            //record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public int GoodsReceipt(IdentityWarehouse identity, IdentityWarehouseActivity activity)
        {
            //Common syntax           
            var sqlGoodsReceiptCmd = @"Warehouse_GoodsReceipt_Item";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@Name", identity.Name},
                //{"@Code", identity.Code },               
                //{"@CreatedBy", identity.CreatedBy},
                //{"@Status", identity.Status}
            };

            StringBuilder activityInsertCmd = new StringBuilder();
            var cmdGoodsReceiptCreate = string.Empty;
            StringBuilder goodsReceiptDetailCmd = new StringBuilder();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    if (identity.ProductList != null && identity.ProductList.Count > 0)
                    {
                        foreach (var item in identity.ProductList)
                        {
                            var itemParms = new Dictionary<string, object>
                            {
                                {"@ProductId", item.Id},
                                {"@WarehouseNum", item.WarehouseNum}
                            };

                            var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlGoodsReceiptCmd, itemParms);
                            newId = Utils.ConvertToInt32(returnObj);
                            if (newId >= 0)
                            {
                               cmdGoodsReceiptCreate = string.Format("INSERT INTO tbl_goods_receipt(LotNo,CreatedBy) VALUES('{0}',{1}); SELECT SCOPE_IDENTITY();", string.Empty, activity.StaffId);

                               var activityCmd = string.Format(@"INSERT INTO tbl_warehouse_activity (ActivityType,ProductId,NumberOfProducts,StaffId,DeviceId,ObjectId) 
                               VALUES ({0},{1},{2},{3},{4},{5}); ", activity.ActivityType, item.Id, item.WarehouseNum, activity.StaffId, activity.DeviceId, "##NEWID##");

                                var productDetailCmd = string.Format(@"INSERT INTO tbl_goods_receipt_detail (GoodsReceiptId,ProductId,NumberOfProducts) 
                               VALUES ({0},{1},{2});", "##NEWID##", item.Id, item.WarehouseNum);

                                activityInsertCmd.Append(activityCmd);
                                goodsReceiptDetailCmd.Append(productDetailCmd);
                            }
                        }

                        var goodsReceiptId = 0;
                        if (!string.IsNullOrEmpty(cmdGoodsReceiptCreate))
                        {
                            var returnObjGoodsReceipt = MsSqlHelper.ExecuteScalar(conn, CommandType.Text, cmdGoodsReceiptCreate, null);
                            goodsReceiptId = Utils.ConvertToInt32(returnObjGoodsReceipt);
                            if(goodsReceiptId > 0)
                            {
                                goodsReceiptDetailCmd.Replace("##NEWID##", goodsReceiptId.ToString());                               

                                var goodsReceiptDetailCmdCmdExec = goodsReceiptDetailCmd.ToString();
                                if (!string.IsNullOrEmpty(goodsReceiptDetailCmdCmdExec))
                                {
                                    //Insert details
                                    MsSqlHelper.ExecuteScalar(conn, CommandType.Text, goodsReceiptDetailCmdCmdExec, null);
                                }
                            }
                        }

                        activityInsertCmd.Replace("##NEWID##", goodsReceiptId.ToString());

                        var activityCmdExec = activityInsertCmd.ToString();
                        if (!string.IsNullOrEmpty(activityCmdExec))
                        {
                            //Insert logs
                            MsSqlHelper.ExecuteScalar(conn, CommandType.Text, activityCmdExec, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Warehouse_GoodsReceipt_Item. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int GoodsIssue(IdentityWarehouse identity, IdentityWarehouseActivity activity)
        {
            //Common syntax           
            var sqlGoodsIssueCmd = @"Warehouse_GoodsIssue_Item";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@Name", identity.Name},
                //{"@Code", identity.Code },               
                //{"@CreatedBy", identity.CreatedBy},
                //{"@Status", identity.Status}
            };

            StringBuilder activityInsertCmd = new StringBuilder();
            var cmdGoodsIssueCreate = string.Empty;
            StringBuilder goodsIssueDetailCmd = new StringBuilder();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    if (identity.ProductList != null && identity.ProductList.Count > 0)
                    {
                        foreach (var item in identity.ProductList)
                        {
                            var itemParms = new Dictionary<string, object>
                            {
                                {"@ProductId", item.Id},
                                {"@WarehouseNum", item.WarehouseNum}
                            };

                            var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlGoodsIssueCmd, itemParms);
                            newId = Utils.ConvertToInt32(returnObj);
                            if (newId >= 0)
                            {
                                cmdGoodsIssueCreate = string.Format("INSERT INTO tbl_goods_issue(LotNo,CreatedBy) VALUES('{0}',{1}); SELECT SCOPE_IDENTITY();", string.Empty, activity.StaffId);

                                var activityCmd = string.Format(@"INSERT INTO tbl_warehouse_activity (ActivityType,ProductId,NumberOfProducts,StaffId,DeviceId,ObjectId) 
                               VALUES ({0},{1},{2},{3},{4},{5}); ", activity.ActivityType, item.Id, item.WarehouseNum, activity.StaffId, activity.DeviceId, "##NEWID##");

                                var productDetailCmd = string.Format(@"INSERT INTO tbl_goods_issue_detail (GoodsIssueId,ProductId,NumberOfProducts) 
                               VALUES ({0},{1},{2});", "##NEWID##", item.Id, item.WarehouseNum);

                                activityInsertCmd.Append(activityCmd);
                                goodsIssueDetailCmd.Append(productDetailCmd);
                            }
                        }

                        var goodsIssueId = 0;
                        if (!string.IsNullOrEmpty(cmdGoodsIssueCreate))
                        {
                            var returnObjGoodsIssue = MsSqlHelper.ExecuteScalar(conn, CommandType.Text, cmdGoodsIssueCreate, null);
                            goodsIssueId = Utils.ConvertToInt32(returnObjGoodsIssue);
                            if (goodsIssueId > 0)
                            {
                                goodsIssueDetailCmd.Replace("##NEWID##", goodsIssueId.ToString());                              
                                var goodsIssueDetailCmdCmdExec = goodsIssueDetailCmd.ToString();
                                if (!string.IsNullOrEmpty(goodsIssueDetailCmdCmdExec))
                                {
                                    //Insert details
                                    MsSqlHelper.ExecuteScalar(conn, CommandType.Text, goodsIssueDetailCmdCmdExec, null);
                                }
                            }
                        }

                        activityInsertCmd.Replace("##NEWID##", goodsIssueId.ToString());

                        var activityCmdExec = activityInsertCmd.ToString();
                        if (!string.IsNullOrEmpty(activityCmdExec))
                        {
                            //Insert logs
                            MsSqlHelper.ExecuteScalar(conn, CommandType.Text, activityCmdExec, null);
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Warehouse_GoodsIssue_Item. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public double ReflectStockTake(IdentityWarehouse identity, IdentityWarehouseActivity activity)
        {
            //Common syntax           
            var sqlReflectCmd = @"Warehouse_ReflectStockTake_Item";
            double stockQTY = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@Name", identity.Name},
                //{"@Code", identity.Code },               
                //{"@CreatedBy", identity.CreatedBy},
                //{"@Status", identity.Status}
            };

            StringBuilder activityInsertCmd = new StringBuilder();
            var cmdReflectStockCreate = string.Empty;
            StringBuilder reflectDetailCmd = new StringBuilder();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    if (identity.ProductList != null && identity.ProductList.Count > 0)
                    {
                        foreach (var item in identity.ProductList)
                        {
                            var itemParms = new Dictionary<string, object>
                            {
                                {"@ProductId", item.Id}
                            };

                            var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlReflectCmd, itemParms);
                            stockQTY = Utils.ConvertToDouble(returnObj);
                            if (stockQTY >= 0)
                            {
                                cmdReflectStockCreate = string.Format("INSERT INTO tbl_reflectstocktake(CreatedBy) VALUES({0}); SELECT SCOPE_IDENTITY();", activity.StaffId);

                                var activityCmd = string.Format(@"INSERT INTO tbl_warehouse_activity (ActivityType,ProductId,NumberOfProducts,StaffId,DeviceId,ObjectId) 
                               VALUES ({0},{1},{2},{3},{4},{5}); ", activity.ActivityType, item.Id, stockQTY, activity.StaffId, activity.DeviceId, "##NEWID##");

                                var productDetailCmd = string.Format(@"INSERT INTO tbl_reflectstocktake_detail (ReflectStockTakeId,ProductId,NumberOfProducts) 
                               VALUES ({0},{1},{2});", "##NEWID##", item.Id, stockQTY);

                                activityInsertCmd.Append(activityCmd);
                                reflectDetailCmd.Append(productDetailCmd);
                            }
                        }

                        var reflectStockId = 0;
                        if (!string.IsNullOrEmpty(cmdReflectStockCreate))
                        {
                            var returnObjReflectStock = MsSqlHelper.ExecuteScalar(conn, CommandType.Text, cmdReflectStockCreate, null);
                            reflectStockId = Utils.ConvertToInt32(returnObjReflectStock);
                            if (reflectStockId > 0)
                            {
                                reflectDetailCmd.Replace("##NEWID##", reflectStockId.ToString());                                

                                var reflectStockDetailCmdCmdExec = reflectDetailCmd.ToString();
                                if (!string.IsNullOrEmpty(reflectStockDetailCmdCmdExec))
                                {
                                    //Insert details
                                    MsSqlHelper.ExecuteScalar(conn, CommandType.Text, reflectStockDetailCmdCmdExec, null);
                                }
                            }                            
                        }

                        activityInsertCmd.Replace("##NEWID##", reflectStockId.ToString());

                        var activityCmdExec = activityInsertCmd.ToString();
                        if (!string.IsNullOrEmpty(activityCmdExec))
                        {
                            //Insert logs
                            MsSqlHelper.ExecuteScalar(conn, CommandType.Text, activityCmdExec, null);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Warehouse_GoodsIssue_Item. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return stockQTY;
        }

        public bool Update(IdentityWarehouse identity)
        {
            //Common syntax
            var sqlCmd = @"Warehouse_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},                
                //{"@Name", identity.Name},
                //{"@Code", identity.Code },                
                //{"@LastUpdatedBy", identity.LastUpdatedBy},
                //{"@Status", identity.Status}
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
                var strError = "Failed to execute Warehouse_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityWarehouse GetById(int Id)
        {
            var info = new IdentityWarehouse();
            var sqlCmd = @"Warehouse_GetById";

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
                            info = ExtractWarehouseData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Warehouse_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Warehouse_Delete";

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
                var strError = "Failed to execute Warehouse_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityWarehouse> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Warehouse_GetList";

            List<IdentityWarehouse> listData = new List<IdentityWarehouse>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractWarehouseData(reader);

                            listData.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Warehouse_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #region Warehouse activity

        public List<IdentityWarehouseActivity> GetHistoryByPage(IdentityWarehouseActivity filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Warehouse_Activity_GetByPage";
            List<IdentityWarehouseActivity> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ProductId", filter.ProductId },
                {"@StaffId", filter.StaffId },
                {"@Keyword", filter.Keyword },
                {"@ActivityType", filter.ActivityType },
                {"@DeviceId", filter.DeviceId },
                {"@FromDate", filter.FromDate },
                {"@ToDate", filter.ToDate },
                {"@Offset", offset},
                {"@PageSize", pageSize}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityWarehouseActivity>();
                        while (reader.Read())
                        {
                            var item = ExtractWarehouseActivityData(reader);
                            listData.Add(item);
                        }

                        if (listData.Count > 0)
                        {
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    var productInfo = RpsProduct.ExtractProductData(reader);
                                    foreach (var item in listData)
                                    {
                                        if (productInfo.Id == item.ProductId)
                                        {
                                            item.ProductInfo = productInfo;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Warehouse_Activity_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private IdentityWarehouseActivity ExtractWarehouseActivityData(IDataReader reader)
        {
            var record = new IdentityWarehouseActivity();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.ActivityType = Utils.ConvertToInt32(reader["ActivityType"]);
            record.ObjectId = Utils.ConvertToInt32(reader["ObjectId"]);
            record.ProductId = Utils.ConvertToInt32(reader["ProductId"]);
            record.NumberOfProducts = Utils.ConvertToDouble(reader["NumberOfProducts"]);
            record.StaffId = Utils.ConvertToInt32(reader["StaffId"]);
            record.DeviceId = Utils.ConvertToInt32(reader["DeviceId"]);
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];

            if(reader.HasColumn("TotalCount"))
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

            return record;
        }

        #endregion

        #endregion
    }
}
