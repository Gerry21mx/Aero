using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Ingenieria.Controllers
{
    public class ComponenteMenorController : Controller
    {
        // GET: Ingenieria/ComponenteMenor
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

        // GET: Vinculacion
        public ActionResult Vinculacion() {
            return View("Vinculacion/Index");
        }

        // GET: PIC
        public ActionResult PIC() {
            return View("PIC/Index");
        }
    }
}