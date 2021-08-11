using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Operaciones.Controllers
{
    public class MovimientosController : Controller
    {
        // GET: Operaciones/Movimientos
        public ActionResult Index()
        {
            return View();
        }

        // GET: Operaciones/Cotizacion
        public ActionResult Cotizacion()
        {
            return View("Cotizacion/Index");
        }

        // GET: Operaciones/Confirmacion
        public ActionResult Confirmacion() {
            return View("Confirmacion/Index");
        }

        // GET: Operaciones/Activacion
        public ActionResult Activacion() {
            return View("Activacion/Index");
        }
    }
}