using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Seguimiento.Controllers {
    public class EstadisticoController : Controller {
        // GET: Estadistico
        public ActionResult Index() {
            return View("");
        }
        // GET: Estadistico
        public ActionResult Vuelos() {
            ViewBag.idaeronave = Request["a"];
            ViewBag.desde = Request["d"];
            ViewBag.hasta = Request["h"];
            ViewBag.idcapacidad = Request["c"];
            return View();
        }
        // GET: Estadistico
        public ActionResult Horas() {
            ViewBag.idaeronave = Request["a"];
            ViewBag.desde = Request["d"];
            ViewBag.hasta = Request["h"];
            ViewBag.idcapacidad = Request["c"];
            return View();
        }
    }
}