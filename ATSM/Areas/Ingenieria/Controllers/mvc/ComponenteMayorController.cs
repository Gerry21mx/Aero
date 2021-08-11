using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Ingenieria.Controllers
{
    public class ComponenteMayorController : Controller
    {
        // GET: Ingenieria/Mayores
        public ActionResult Index() {
            return View();
        }

        // GET: Ingenieria/Mayores
        public ActionResult Mayores(int idc) {
            ComponenteMayor mayor = new ComponenteMayor(idc);
            return View("Mayores/Index", mayor);
        }
    }
}