using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebMatrix.WebData;

namespace ATSM.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
			if (WebSecurity.IsAuthenticated) {
                return RedirectToAction("Index", "Home");
            }
			else {
                return View();
			}
        }

        // GET: Restablecer/Restablecer
        public ActionResult Restablecer() {
            return View("Restablecer/Index");
        }
    }
}