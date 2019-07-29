using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Manager.SharedLib.Logging;

namespace Manager.WebApp
{
    /// <summary>
    /// http://www.codeproject.com/Articles/422572/Exception-Handling-in-ASP-NET-MVC
    /// 
    /// The HandleError filter has some limitations by the way.
    ///     + Not support to log the exceptions
    ///     + Doesn't catch HTTP exceptions other than 500
    ///     + Doesn't catch exceptions that are raised outside controllers
    ///     + Return error view even for exceptions occurred in AJAX calls
    /// 
    /// Extending HandleError
    /// Most of the cases we have to extend the built-in HandleError filter or have to create a custom exception filter to do some useful job like logging. 
    /// Here is an example that shows how to extend the built-in filter to log the exceptions using log4net and return a JSON object for AJAX calls.
    /// </summary>
    public class LogExceptionFilter: HandleErrorAttribute
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
            {
                return;
            }


            // if the request is AJAX return JSON else view.
            // if (filterContext.HttpContext.Request.i.Headers["X-Requested-With"] == "XMLHttpRequest")
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message
                    }
                };
            }
            else
            {
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };
            }

            // log the error using log4net.
            logger.ErrorException("Filter Error: {0}", filterContext.Exception, filterContext.Exception.Message);

            //base.OnException(filterContext);

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;

            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;  
        }
    }
}