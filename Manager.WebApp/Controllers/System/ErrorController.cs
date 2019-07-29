using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manager.WebApp.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NotFound()
        {
            return View();
        }


        public ActionResult Unauthorised()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}