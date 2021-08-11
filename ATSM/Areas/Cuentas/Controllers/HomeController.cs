using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Cuentas.Controllers
{
    public class HomeController : Controller
    {
        // GET: Cuentas/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}