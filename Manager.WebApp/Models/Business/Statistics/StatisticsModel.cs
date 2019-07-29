using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models.Business
{
    public class StatisticsUsersOnlineModel
    {
        public List<Connector> ListUser { get; set; }

        public StatisticsUsersOnlineModel()
        {
            ListUser = new List<Connector>();
        }
    }

    public class StatisticsBookingModel
    {
        public StatisticsBookingInYearModel InYearData { get; set; }
        public StatisticBookingAgencyModel AgencyData { get; set; }


        public string UpdatedTime { get; set; }
        public string NextUpdate { get; set; }

        public StatisticsBookingModel()
        {
            InYearData = new StatisticsBookingInYearModel();
            AgencyData = new StatisticBookingAgencyModel();
        }
    }

    public class StatisticsBookingInYearModel
    {
        public int[] processingData { get; set; }
        public int[] successData { get; set; }
        public int[] failedData { get; set; }

        public StatisticsBookingInYearModel()
        {
            processingData = new int[12];
            successData = new int[12];
            failedData = new int[12];
        }
    }

    public class StatisticBookingAgencyModel
    {
        public List<string> ListHang { get; set; }
        public List<string> ListHangLabels { get; set; }

        public int[] TotalData { get; set; }
    }
}