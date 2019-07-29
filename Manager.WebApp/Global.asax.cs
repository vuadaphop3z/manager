using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Manager.SharedLib.Logging;
using Manager.WebApp.Controllers;

using Newtonsoft.Json;

namespace Manager.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //This value provider will handle requests that are encoded as application/json. 
            //There’s no need to specify a model binder on classes that accept JSON input.
            //ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
            System.Web.Helpers.AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
        }


        protected void Application_Error(object sender, EventArgs e)
        {            
            var httpContext = ((MvcApplication)sender).Context;

            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = " ";
            var currentAction = " ";

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();

            var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "Index";

            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "NotFound";
                        break;

                    // others if any
                    default:
                        action = "Index";
                        break;
                }
            }

            logger.ErrorException("Application Error: {0}", ex, ex.Message);

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            //Process error for Ajax Request
            //http://stackoverflow.com/questions/7551424/how-to-know-if-the-request-is-ajax-in-asp-net-in-application-error
            bool isAjaxCall = string.Equals("XMLHttpRequest", Context.Request.Headers["x-requested-with"], StringComparison.OrdinalIgnoreCase);
            Context.ClearError();
            if (isAjaxCall)
            {
                var jsonObject = new { error = true, message = ex.Message };
                var jsonStringResponse = JsonConvert.SerializeObject(jsonObject);

                Context.Response.ContentType = "application/json";                
                Context.Response.Write(jsonStringResponse );

                return;
            }

            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;

            //Response.TrySkipIisCustomErrors = true; // If you are using IIS7, have this line
            //IController errorsController = new ErrorController();
            //HttpContextWrapper wrapper = new HttpContextWrapper(Context);
            //var rc = new System.Web.Routing.RequestContext(wrapper, routeData);
            //errorsController.Execute(rc);

            //IController controller = new ErrorController();
            //controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            //Response.End();

            controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
            Response.End();
        }

    }
}
