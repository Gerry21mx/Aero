using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Seguimiento.Controllers
{
    public class AdminController : Controller
    {
        // GET: Seguimiento/Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Seguimiento/Perfil
        public ActionResult Perfil() {
            return View("Perfil/Index");
        }

        // GET: Seguimiento/Usuario
        public ActionResult Usuario() {
            return View("Usuario/Index");
        }
    }
}