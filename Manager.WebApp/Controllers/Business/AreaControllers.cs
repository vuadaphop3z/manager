using System.Web.Mvc;
using Manager.SharedLib.Logging;
using Manager.WebApp.Helpers;
using MsSql.AspNet.Identity.MsSqlStores;

namespace Manager.WebApp.Controllers
{
    public class AreaController : BaseAuthedController
    {
        private readonly IStoreArea _mainStore;
        private readonly ILog logger = LogProvider.For<AreaController>();

        public AreaController(IStoreArea mainStore)
        {
            _mainStore = mainStore;
        }

        public ActionResult GetCountryByArea(int areaId)
        {
            var myList = CommonHelpers.GetCountryByArea(areaId);

            var myHtmlResult = PartialViewAsString("Partials/_Country", myList);

            return Json(new { data = myHtmlResult }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProvinceByCountry(int countryId)
        {
            var myList = CommonHelpers.GetProvinceByCountry(countryId);

            var myHtmlResult = PartialViewAsString("Partials/_Province", myList);

            return Json(new { data = myHtmlResult }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDistrictByProvince(int provinceId)
        {
            var myList = CommonHelpers.GetDistrictByProvince(provinceId);

            var myHtmlResult = PartialViewAsString("Partials/_District", myList);

            return Json(new { data = myHtmlResult }, JsonRequestBehavior.AllowGet);
        }
    }
}