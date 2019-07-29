using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Models;
using Manager.WebApp.Services;
using Autofac;

namespace Manager.WebApp.Controllers
{
    public class WarehouseController : BaseAuthedController
    {
        private readonly IStoreWarehouse _mainStore;
        private readonly ILog logger = LogProvider.For<WarehouseController>();

        public WarehouseController(IStoreWarehouse mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManageWarehouseModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            var filter = new IdentityWarehouse
            {
                ProductCode = !string.IsNullOrEmpty(model.Code) ? model.Code.Trim() : null,
                Keyword = !string.IsNullOrEmpty(model.Name) ? model.Name.Trim() : null,
                IsStockTakeQTY = model.IsConfirmStockTakeQTY == null ? -1 : (int)model.IsConfirmStockTakeQTY,
                IsStockOut = model.IsStockOut == null ? -1 : (int)model.IsStockOut
            };

            try
            {
                model.Units = CommonHelpers.GetListUnit();

                model.SearchResults = _mainStore.GetByPage(filter, currentPage, SystemSettings.DefaultPageSize);
                if (model.SearchResults != null && model.SearchResults.Count > 0)
                {
                    model.TotalCount = model.SearchResults[0].TotalCount;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        public ActionResult GoodsReceipt()
        {
            WarehouseActionModel model = new WarehouseActionModel();
            var id = Utils.ConvertToInt32(Request["WarehouseId"]);
            var productId = Utils.ConvertToInt32(Request["ProductId"]);
          
            try
            {
                model.WarehouseId = id;
                model.ProductId = productId;

                var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();

                if (productId > 0)
                    model.ProductInfo = productStore.GetDetail(productId);              
                
                if(model.ProductInfo != null)
                {
                    model.ItemCode = model.ProductInfo.Code;
                    model.ItemName = model.ProductInfo.Name;
                }

                model.Units = CommonHelpers.GetListUnit();
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show Goods Receipt form: " + ex.ToString());
            }

            return PartialView("_GoodsReceipt", model);
        }

        public ActionResult GoodsIssue()
        {
            WarehouseActionModel model = new WarehouseActionModel();
            var id = Utils.ConvertToInt32(Request["WarehouseId"]);
            var productId = Utils.ConvertToInt32(Request["ProductId"]);

            try
            {
                model.WarehouseId = id;
                model.ProductId = productId;

                var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();

                if (productId > 0)
                    model.ProductInfo = productStore.GetDetail(productId);

                if (model.ProductInfo != null)
                {
                    model.ItemCode = model.ProductInfo.Code;
                    model.ItemName = model.ProductInfo.Name;
                }

                model.Units = CommonHelpers.GetListUnit();
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show Goods Issue form: " + ex.ToString());
            }

            return PartialView("_GoodsIssue", model);
        }

        public ActionResult ReflectStockTake()
        {
            WarehouseActionModel model = new WarehouseActionModel();
            var id = Utils.ConvertToInt32(Request["WarehouseId"]);
            var productId = Utils.ConvertToInt32(Request["ProductId"]);         

            try
            {
                //model.WarehouseId = id;
                model.ProductId = productId;

                //var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();

                //if (productId > 0)
                //    model.ProductInfo = productStore.GetDetail(productId);

                //if (model.ProductInfo != null)
                //{
                //    model.ItemCode = model.ProductInfo.Code;
                //    model.ItemName = model.ProductInfo.Name;
                //}

                //model.Units = CommonHelpers.GetListUnit();
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show Reflect Stock-take form: " + ex.ToString());
            }

            return PartialView("_ReflectStockTake", model);
        }

        [HttpPost]
        [ActionName("GoodsReceipt")]
        [ValidateAntiForgeryToken]
        public ActionResult GoodsReceipt(WarehouseActionModel model)
        {
            var strError = string.Empty;
            try
            {                
                var productInfo = new IdentityProduct();
                var warehouseInfo = new IdentityWarehouse();

                productInfo.Id = model.ProductId;
                productInfo.Code = model.ItemCode;

                var qty = Utils.ConvertToDouble(model.WarehouseNum);
                if (qty <= 0)
                    return Json(new { success = false, message = ManagerResource.ERROR_QTY_MUST_LARGE_THAN_0, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });

                if (model.ProductId <= 0)
                {
                    if (!string.IsNullOrEmpty(model.ItemCode))
                    {
                        var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();
                        var productInfoCheck = productStore.GetByCode(model.ItemCode);
                        
                        if(productInfoCheck == null || (productInfoCheck != null && productInfoCheck.Id <= 0))
                            return Json(new { success = false, message = string.Format(ManagerResource.ERROR_PRODUCT_ITEM_CODE_NOTFOUND_FORMAT, model.ItemCode), title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });
                        else
                            productInfo = productInfoCheck;
                    }
                }
                
                productInfo.WarehouseNum = qty;
                warehouseInfo.ProductList.Add(productInfo);

                var activityInfo = new IdentityWarehouseActivity();
                activityInfo.ActivityType = (int)EnumWarehouseActivityType.GoodsReceipt;
                activityInfo.DeviceId = RegisterNewDevice();
                activityInfo.StaffId = GetCurrentStaffId();

                //Database execute
                _mainStore.GoodsReceipt(warehouseInfo, activityInfo);
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to exec GoodsReceipt because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_GOODS_RECEIPT_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
        }

        [HttpPost]
        [ActionName("GoodsIssue")]
        [ValidateAntiForgeryToken]
        public ActionResult GoodsIssue(WarehouseActionModel model)
        {
            var strError = string.Empty;
            try
            {
                var productInfo = new IdentityProduct();
                var warehouseInfo = new IdentityWarehouse();

                productInfo.Id = model.ProductId;
                productInfo.Code = model.ItemCode;

                var qty = Utils.ConvertToDouble(model.WarehouseNum);
                if (qty <= 0)
                    return Json(new { success = false, message = ManagerResource.ERROR_QTY_MUST_LARGE_THAN_0, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });

                if (model.ProductId <= 0)
                {
                    if (!string.IsNullOrEmpty(model.ItemCode))
                    {
                        var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();
                        var productInfoCheck = productStore.GetByCode(model.ItemCode);

                        if (productInfoCheck == null || (productInfoCheck != null && productInfoCheck.Id <= 0))
                            return Json(new { success = false, message = string.Format(ManagerResource.ERROR_PRODUCT_ITEM_CODE_NOTFOUND_FORMAT, model.ItemCode), title = ManagerResource.LB_ERROR_OCCURED, clientcallback = " ShowMyModalAgain();" });
                        else
                            productInfo = productInfoCheck;
                    }
                }                               

                productInfo.WarehouseNum = qty;

                warehouseInfo.ProductList.Add(productInfo);

                var activityInfo = new IdentityWarehouseActivity();
                activityInfo.ActivityType = (int)EnumWarehouseActivityType.GoodsIssue;
                activityInfo.DeviceId = RegisterNewDevice();
                activityInfo.StaffId = GetCurrentStaffId();               

                //Database execute
                var numCode = _mainStore.GoodsIssue(warehouseInfo, activityInfo);
                if(numCode == -1)
                {
                    var numberHighlight = string.Format("<b>{0}</b>", Utils.DoubleToStringFormat(Utils.ConvertToDouble(model.WarehouseNum)));
                    return Json(new { success = false, message = string.Format(ManagerResource.ERROR_GOODS_ISSUE_NOT_ENOUGH_FORMAT, numberHighlight), title = ManagerResource.LB_ERROR_OCCURED, clientcallback = " ShowMyModalAgain();" });
                }
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to exec GoodsIssue because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_GOODS_ISSUE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
        }

        [HttpPost]
        [ActionName("ReflectStockTake")]
        [ValidateAntiForgeryToken]
        public ActionResult ReflectStockTake(WarehouseActionModel model)
        {
            var strError = string.Empty;
            try
            {
                var productInfo = new IdentityProduct();
                var warehouseInfo = new IdentityWarehouse();

                productInfo.Id = model.ProductId;
                productInfo.Code = model.ItemCode;

                var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();
                var productInfoCheck = productStore.GetById(model.ProductId);
                if (productInfoCheck == null || (productInfoCheck != null && productInfoCheck.Id <= 0))
                {
                    return Json(new { success = false, message = ManagerResource.ERROR_PRODUCT_ITEM_NOTFOUND, title = ManagerResource.LB_NOTIFICATION });
                }
                else
                {
                    productInfo = productInfoCheck;
                }
                //if (model.ProductId <= 0)
                //{
                //    if (!string.IsNullOrEmpty(model.ItemCode))
                //    {
                //        var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();
                //        var productInfoCheck = productStore.GetByCode(model.ItemCode);

                //        if (productInfoCheck == null || (productInfoCheck != null && productInfoCheck.Id <= 0))
                //            return Json(new { success = false, message = string.Format(ManagerResource.ERROR_PRODUCT_ITEM_CODE_NOTFOUND_FORMAT, model.ItemCode), title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });
                //        else
                //            productInfo = productInfoCheck;
                //    }
                //}

                //productInfo.WarehouseNum = model.WarehouseNum;
                warehouseInfo.ProductList.Add(productInfo);

                var activityInfo = new IdentityWarehouseActivity();
                activityInfo.ActivityType = (int)EnumWarehouseActivityType.ReflectStockTake;
                activityInfo.DeviceId = RegisterNewDevice();
                activityInfo.StaffId = GetCurrentStaffId();

                //Database execute
                _mainStore.ReflectStockTake(warehouseInfo, activityInfo);
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to exec ReflectStockTake because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_GOODS_REFLECT_STOCK_TAKE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
        }

        //Show popup confirm delete        
        //[AccessRoleChecker]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return PartialView("_PopupDelete", id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Confirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _mainStore.Delete(id);

                //Clear cache
                //CachingHelpers.ClearWarehouseCache(id);
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Warehouse because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        public ActionResult GetListProductStockOut(ManageWarehouseModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            currentPage = model.CurrentPage;
            if (currentPage == 0)
                currentPage = 1;

            if (model.PageSize > 0)
                pageSize = model.PageSize;

            if (pageSize <= 0 || pageSize > 100)
                pageSize = SystemSettings.DefaultPageSize;

            var filter = new IdentityWarehouse
            {
                //ProductCode = !string.IsNullOrEmpty(model.Code) ? model.Code.Trim() : null,
                //Keyword = !string.IsNullOrEmpty(model.Name) ? model.Name.Trim() : null,
            };

            try
            {
                model.Units = CommonHelpers.GetListUnit();
                var warehouseStore = GlobalContainer.IocContainer.Resolve<IStoreWarehouse>();
                model.SearchResults = warehouseStore.GetProductStockOutByPage(filter, currentPage, SystemSettings.DefaultPageSize);
                if (model.SearchResults != null && model.SearchResults.Count > 0)
                {
                    model.TotalCount = model.SearchResults[0].TotalCount;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get GetProductStockOutByPage because: " + ex.ToString());
            }

            return PartialView("Partials/_ProductStockOutList", model);
        }

        [AccessRoleChecker]
        public ActionResult History(WarehouseHistoryModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            if (model.PageSize > 0)
            {
                pageSize = model.PageSize;
            }

            if (pageSize == 0 || pageSize > 100)
                pageSize = SystemSettings.DefaultPageSize;

            try
            {
                var filter = new IdentityWarehouseActivity
                {
                    ProductId = model.ProductId,
                    Keyword = model.Name,
                    StaffId = model.StaffId,
                    ActivityType = model.ActivityType,      
                    DeviceId = model.DeviceId
                };

                filter.FromDate = Utils.ConvertStringToDateTimeByFormat(model.FromDate);
                if(filter.FromDate != null)
                    filter.FromDate = Utils.GetBeginOfDate(filter.FromDate.Value);

                filter.ToDate = Utils.ConvertStringToDateTimeByFormat(model.ToDate);
                if (filter.ToDate != null)
                    filter.ToDate = Utils.GetEndOfDate(filter.ToDate.Value);

                if (filter.FromDate > filter.ToDate)
                {
                    this.AddNotification(ManagerResource.ERROR_DATE_RANGE_IN_PAST, NotificationType.ERROR);

                    return View(model);
                }

                //if (filter.FromDate == null && filter.ToDate == null)
                //{
                //    filter.FromDate = Utils.GetBeginOfDate(DateTime.Now);
                //    filter.ToDate = Utils.GetEndOfDate(DateTime.Now);
                //}

                model.Units = CommonHelpers.GetListUnit();
                model.Devices = CommonHelpers.GetListDevice();
                model.Users = CommonHelpers.GetListUser();

                model.SearchResults = _mainStore.GetHistoryByPage(filter, currentPage, pageSize);
                if (model.SearchResults.HasData())
                {
                    model.TotalCount = model.SearchResults[0].TotalCount;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        public ActionResult GetWarehouseActivityList(WarehouseHistoryModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            currentPage = model.CurrentPage;
            if (currentPage == 0)
                currentPage = 1;

            if (model.PageSize > 0)
            {
                pageSize = model.PageSize;
            }

            if (pageSize <= 0 || pageSize > 100)
                pageSize = SystemSettings.DefaultPageSize;

            try
            {
                var filter = new IdentityWarehouseActivity
                {
                    ProductId = model.ProductId,
                    ActivityType = model.ActivityType,
                    DeviceId = model.DeviceId
                };

                filter.FromDate = Utils.ConvertStringToDateTimeByFormat(model.FromDate);
                if (filter.FromDate != null)
                    filter.FromDate = Utils.GetBeginOfDate(filter.FromDate.Value);

                filter.ToDate = Utils.ConvertStringToDateTimeByFormat(model.ToDate);
                if (filter.ToDate != null)
                    filter.ToDate = Utils.GetEndOfDate(filter.ToDate.Value);

                if (filter.FromDate > filter.ToDate)
                {
                    this.AddNotification(ManagerResource.ERROR_DATE_RANGE_IN_PAST, NotificationType.ERROR);

                    return View(model);
                }

                //if (filter.FromDate == null && filter.ToDate == null)
                //{
                //    filter.FromDate = Utils.GetBeginOfDate(DateTime.Now);
                //    filter.ToDate = Utils.GetEndOfDate(DateTime.Now);
                //}

                model.Units = CommonHelpers.GetListUnit();
                model.Devices = CommonHelpers.GetListDevice();

                model.SearchResults = _mainStore.GetHistoryByPage(filter, currentPage, pageSize);
                if (model.SearchResults.HasData())
                {
                    model.TotalCount = model.SearchResults[0].TotalCount;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get GetWarehouseActivityList because: " + ex.ToString());

                return View(model);
            }

            return PartialView("Partials/_ActivityHistoryList", model);
        }

        #region Helpers

        private int RegisterNewDevice()
        {
            var deviceId = 0;
            var isNew = false;
            try
            {
                var deviceInfo = new IdentityDevice();
                deviceInfo.Name = System.Environment.MachineName;
                deviceId = CommonHelpers.RegisterNewDevice(deviceInfo, out isNew);
            }
            catch (Exception ex)
            {
                logger.Error("Failed to RegisterNewDevice because: " + ex.ToString());
            }

            return deviceId;
        }

        #endregion

    }
}