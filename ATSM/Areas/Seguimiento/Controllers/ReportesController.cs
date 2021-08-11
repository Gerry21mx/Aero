using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Seguimiento.Controllers {
    public class ReportesController : Controller {
        // GET: Seguimiento/Reportes
        public ActionResult Index() {
            return View();
        }
        // GET: Reportes
        public ActionResult Operaciones()
        {
            return View();
        }
        // GET: Reportes
        public ActionResult Mantenimiento() {
            return View();
        }
        // GET: Reportes
        public ActionResult Pilotos() {
            return View();
        }
        // GET: Reportes
        public ActionResult Emisiones() {
            return View();
        }
        // GET: Reportes
        public ActionResult tERep() {
            return View("tablas/emisiones");
        }
        // GET: Reportes
        public ActionResult Tripulaciones() {
            return View();
        }
        // GET: Reportes
        public ActionResult EdoTrip() {
            return View();
        }
        // GET: Reportes
        public ActionResult PosFlota() {
            return View();
        }
    }
}