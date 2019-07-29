using Manager.WebApp.Caching;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Manager.WebApp.Controllers
{
    [Authorize]
    public class BaseAuthedController : BaseController
    {
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            var lang = UserCookieManager.GetCurrentLanguageOrDefault();
            var cultureInfo = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);

            if (!User.Identity.IsAuthenticated && !Request.IsAjaxRequest())
                Response.Redirect("~/Account/Login");

            return base.BeginExecuteCore(callback, state);
        }

        public ActionResult ChangeLanguage(string lang)
        {
            var currentLang = UserCookieManager.GetCurrentLanguageOrDefault();
            if (Request != null)
            {
                if (Request.UrlReferrer != null)
                {
                    var currentUrl = Request.UrlReferrer.ToString();
                    if (!string.IsNullOrEmpty(Request.UrlReferrer.ToString()))
                    {
                        if (currentLang != lang)
                            new LanguageMessageHandler().SetLanguage(lang);

                        //HttpResponse.RemoveOutputCacheItem(Request.UrlReferrer.AbsolutePath);
                        //HttpResponse.RemoveOutputCacheItem("/home/index");
                        //HttpResponse.RemoveOutputCacheItem("/");

                        return Redirect(currentUrl);
                    }
                }
            }

            new LanguageMessageHandler().SetLanguage(lang);

            return RedirectToAction("Index", "Home");
        }

        protected string PartialViewAsString(string partialviewName, object model)
        {
            if (string.IsNullOrEmpty(partialviewName))
            {
                partialviewName = ControllerContext.RouteData.GetRequiredString("action");
            }

            var viewData = ViewData;
            ViewData = new ViewDataDictionary(viewData) { Model = model };

            using (var writer = new StringWriter())
            {
                var viewResult = ViewEngineCollection.FindPartialView(ControllerContext, partialviewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, writer);
                viewResult.View.Render(viewContext, writer);
                ViewData = viewData;

                return writer.ToString();
            }
        }

        protected ActionResult RedirectToErrorPage(string pageName = "NotFound")
        {
            return View(string.Format("../Error/{0}", pageName));
        }
    }  
}