using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Cuentas.Controllers
{
    public class CatalogoController : Controller {
        // GET: Cuentas/Catalogo
        public ActionResult Index() {
            return View();
        }
        // GET: Cuentas/Moneda
        public ActionResult Divisa() {
            return View();
        }
        // GET: Cuentas/Saldos
        public ActionResult Saldo() {
            return View();
        }
    }
}