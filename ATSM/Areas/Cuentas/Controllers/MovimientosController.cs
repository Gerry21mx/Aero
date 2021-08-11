using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Cuentas.Controllers
{
    public class MovimientosController : Controller {
        // GET: Cuentas/Movimientos
        public ActionResult Index() {
            return View();
        }
        // GET: Cuentas/Deposito
        public ActionResult Deposito() {
            return View("Deposito/Index");
        }
        // GET: Cuentas/Reposicion
        public ActionResult Reposicion() {
            return View("Reposicion/Index");
        }
        // GET: Cuentas/Devolucion
        public ActionResult Devolucion() {
            return View("Devolucion/Index");
        }
    }
}