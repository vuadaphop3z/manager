using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manager.WebApp.Controllers
{
    public class AgencyController : BaseAuthedController
    {
        // GET: Agency
        public ActionResult Index()
        {
            return View();
        }
    }
}