using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Pilotos.Controllers
{
    public class VuelosController : Controller
    {
        // GET: Pilotos/Vuelos
        public ActionResult Index()
        {
            return View();
        }
        // GET: Pilotos/Vuelos
        public ActionResult Vuelo() {
            return View("Vuelo/Index");
        }
        // GET: Pilotos/Vuelos
        public ActionResult Bitacora() {
            return View("Bitacora/Index");
        }
        // GET: Pilotos/Vuelos
        public ActionResult Itinerario() {
            return View("Itinerario/Index");
        }
    }
}