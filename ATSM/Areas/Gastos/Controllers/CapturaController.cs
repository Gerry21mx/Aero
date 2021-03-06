using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Gastos.Controllers
{
    public class CapturaController : Controller
    {
        // GET: Gastos/Captura
        public ActionResult Index()
        {
            return View();
        }
        // GET: Cuentas/Piloto
        public ActionResult Piloto() {
            return View("Piloto/Index");
        }
        // GET: Cuentas/Caja
        public ActionResult Caja() {
            return View("Caja/Index");
        }
        // GET: Cuentas/Viaticos
        public ActionResult Viaticos() {
            return View("Viaticos/Index");
        }
        // GET: Cuentas/Representante
        public ActionResult Representante() {
            return View("Representante/Index");
        }
    }
}