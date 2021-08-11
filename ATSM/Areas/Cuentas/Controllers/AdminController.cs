using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Cuentas.Controllers {
    public class AdminController : Controller
    {
        // GET: Cuentas/Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Cuentas/Perfil
        public ActionResult Perfil() {
            return View("Perfil/Index");
        }

        // GET: Cuentas/Usuario
        public ActionResult Usuario() {
            return View("Usuario/Index");
        }
    }
}