using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Ingenieria.Controllers
{
    public class OperacionesController : Controller
    {
        // GET: Ingenieria/Operaciones
        public ActionResult Index()
        {
            return View();
        }

        // GET: BitacoraVuelo
        public ActionResult BitacoraVuelo() {
            return View("BitacoraVuelo/Index");
        }

        // GET: BitacoraReporte
        public ActionResult BitacoraReporte() {
            return View("BitacoraReporte/Index");
        }
    }
}