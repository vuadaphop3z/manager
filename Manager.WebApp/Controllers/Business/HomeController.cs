using Autofac;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Models.Business;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using System;
using System.Web.Mvc;

namespace Manager.WebApp.Controllers
{
    public class HomeController : BaseAuthedController
    {
        private readonly string _reportInYear = "REPORT_IN_YEAR";
        private readonly DateTimeOffset _cacheReportByYearExpiredTime;
        private readonly ILog logger = LogProvider.For<HomeController>();

        public HomeController()
        {
            //_cacheReportByYearExpiredTime = DateTimeOffset.Now.AddMinutes(SystemSettings.CacheExpireDataInDashBoard); 
        }

        [AccessRoleChecker]
        public ActionResult Index()
        {
            ManageWarehouseModel model = new ManageWarehouseModel();
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

                return View(model);
            }

            return View(model);
        }

        public ActionResult BookingStatistics()
        {
            StatisticsBookingModel finalData = null;

            try
            {
                finalData = GetStatisticsAndSaveToCache();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when get Report for Dashboard because: {0}", ex.ToString()));
            }           

            return Json(finalData, JsonRequestBehavior.AllowGet);
        }

        private StatisticsBookingModel GetStatisticsAndSaveToCache()
        {
            StatisticsBookingModel finalData = null;

            //Check from cache
            //var cacheClient = GlobalContainer.IocContainer.Resolve<ICacheClient>();
            //finalData = cacheClient.Get<StatisticsBookingModel>(_reportInYear);
            //if (finalData == null)
            //{
            //    finalData = new StatisticsBookingModel();

            //    //Agency statistics 
            //    var listHang = SystemSettings.AirlinesReportInDashBoard.Split(',').ToList();
            //    var listHangLabels = SystemSettings.AirlinesLabelInDashBoard.Split(',').ToList();
            //    var airlinesCounter = listHang.Count;

            //    finalData.AgencyData.ListHang = listHang;
            //    finalData.AgencyData.ListHangLabels = listHangLabels;
            //    finalData.AgencyData.TotalData = new int[airlinesCounter];

            //    var listBooking = new List<IdentityReportByYear>();

            //    int year = DateTime.Now.Year;
            //    DateTime firstDay = new DateTime(year, 1, 1);
            //    DateTime lastDay = new DateTime(year, 12, 31);
            //    DateTime dtNow = DateTime.Now;

            //    var _storeReport = GlobalContainer.IocContainer.Resolve<IStoreReport>();
            //    var filter = new ReportFilter();
            //    filter.ProviderId = 0;
            //    filter.FromDate = firstDay;
            //    filter.ToDate = lastDay;

            //    listBooking = _storeReport.GetStatisticsForDashBoard(filter);

            //    if (listBooking != null && listBooking.Count > 0)
            //    {                    
            //        foreach (var item in listBooking)
            //        {
            //            //Get data statistics for year
            //            for (int i = 0; i < finalData.InYearData.successData.Length; i++)
            //            {
            //                if (item.NgayBook.Value.Month == (i + 1))
            //                {
            //                    if (item.TinhTrang == (int)EnumTinhTrangVe.DaXuatVe)
            //                    {
            //                        finalData.InYearData.successData[i]++;
            //                    }
            //                    else if (item.TinhTrang == (int)EnumTinhTrangVe.DaHuy)
            //                    {
            //                        finalData.InYearData.failedData[i]++;
            //                    }
            //                    else
            //                    {
            //                        finalData.InYearData.processingData[i]++;
            //                    }
            //                }
            //            }

            //            //Get data statistics for Agency                       
            //            if (listHang != null && airlinesCounter > 0)
            //            {
            //                var isAgencyData = (item.NgayBook.Value.Month == dtNow.Month && item.NgayBook.Value.Day == dtNow.Day);
            //                var matched = false;
            //                if (isAgencyData)
            //                {
            //                    for (int i = 0; i < airlinesCounter; i++)
            //                    {
            //                        var currentHang = listHang[i];
            //                        if (item.HangId == currentHang)
            //                        {
            //                            matched = true;
            //                            finalData.AgencyData.TotalData[i]++;
            //                            break;
            //                        }                                    
            //                    }

            //                    if(!matched)
            //                        finalData.AgencyData.TotalData[airlinesCounter - 1]++;
            //                }
                           
            //            }
            //        }                    
            //    }

            //    finalData.UpdatedTime = dtNow.ToString("dd/MM/yyyy HH:mm");
            //    finalData.NextUpdate = dtNow.AddMinutes(SystemSettings.CacheExpireDataInDashBoard).ToString("dd/MM/yyyy HH:mm");

            //    //Save to cache: Expire time default is: 30 minutes
            //    cacheClient.Add(_reportInYear, finalData, _cacheReportByYearExpiredTime);
            //}           

            return finalData;
        }
    }
}