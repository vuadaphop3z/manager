using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Manager.WebApp.Controllers;

namespace Manager.WebApp.Helpers
{
    public static class ViewExtensions
    {
        public static string RenderToString(this PartialViewResult partialView)
        {
            var httpContext = HttpContext.Current;

            if (httpContext == null)
            {
                throw new NotSupportedException("An HTTP context is required to render the partial view to a string");
            }

            //var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            //var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(httpContext.Request.RequestContext, "Templates");
            //var controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);

            var context = new HttpContextWrapper(HttpContext.Current);
            var routeData = new RouteData();
            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new TemplatesController());

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    view.Render(new ViewContext(controllerContext, view, partialView.ViewData, partialView.TempData, tw), tw);
                }
            }

            return sb.ToString();
        }

        //public static string GetRazorViewAsString(object model, string view)
        //{
        //    var guid = Guid.NewGuid();
        //    var filePath = AppDomain.CurrentDomain.BaseDirectory + guid + ".cshtml";
        //    File.WriteAllText(filePath, string.Format("@inherits System.Web.Mvc.WebViewPage<{0}>\r\n {1}", model.GetType().FullName, view));
        //    var st = new StringWriter();
        //    var context = new HttpContextWrapper(HttpContext.Current);
        //    var routeData = new RouteData();
        //    var controllerContext = new ControllerContext(new RequestContext(context, routeData), new TempController());
        //    var razor = new RazorView(controllerContext, "~/" + guid + ".cshtml", null, false, null);
        //    razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
        //    File.Delete(filePath);
        //    return st.ToString();
        //}
    }
}