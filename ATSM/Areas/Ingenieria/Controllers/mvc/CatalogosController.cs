using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Ingenieria.Controllers
{
    public class CatalogosController : Controller
    {
        // GET: Ingenieria/Catalogos
        public ActionResult Index()
        {
            return View();
        }

        // GET: Biweekly
        public ActionResult Biweekly() {
            return View("Biweekly/Index");
        }

        // GET: Familias
        public ActionResult Familias() {
            return View("Familias/Index");
        }

        // GET: Posiciones
        public ActionResult Posiciones() {
            return View("Posiciones/Index");
        }

        // GET: Alernos
        public ActionResult Alternos() {
            return View("Alternos/Index");
        }

        // GET: Aircraft
        public ActionResult Aircraft() {
            return View("Aircraft/Index");
        }

        // GET: Modelos
        public ActionResult Modelos() {
            return View("Modelos/Index");
        }

        // GET: Capacidades
        public ActionResult Capacidades() {
            return View("Capacidad/Index");
        }

        // GET: Limites
        public ActionResult Limites() {
            return View("Limites/Index");
        }

        // GET: Componentes Mayores
        public ActionResult ComponenteMayor() {
            return View("ComponenteMayor/Index");
        }
    }
}