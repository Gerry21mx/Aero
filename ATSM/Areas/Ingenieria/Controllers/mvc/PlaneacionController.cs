using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Ingenieria.Controllers
{
    public class PlaneacionController : Controller
    {
        // GET: Ingenieria/Planeacion
        public ActionResult Index()
        {
            return View();
        }

        // GET: Programacion
        public ActionResult Programacion() {
            return View("Programacion/Index");
        }

        // GET: Pronostico
        public ActionResult Pronostico() {
            return View("Pronostico/Index");
        }

        // GET: ots
        public ActionResult ots() {
            return View("ots/Index");
        }
    }
}