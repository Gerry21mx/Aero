using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Gastos.Controllers
{
    public class HomeController : Controller
    {
        // GET: Gastos/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}