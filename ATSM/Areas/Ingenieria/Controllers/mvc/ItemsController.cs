using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Ingenieria.Controllers
{
    public class ItemsController : Controller
    {
        // GET: Ingenieria/Items
        public ActionResult Index()
        {
            return View();
        }

        // GET: Componente
        public ActionResult Componente() {
            return View("Componente/Index");
        }

        // GET: ADSB
        public ActionResult ADSB() {
            return View("ADSB/Index");
        }

        // GET: Servicio
        public ActionResult Servicio() {
            return View("Servicio/Index");
        }
    }
}