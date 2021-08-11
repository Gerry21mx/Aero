using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Ingenieria.Controllers
{
    public class TareasController : Controller
    {
        // GET: Ingenieria/Tareas
        public ActionResult Index()
        {
            return View();
        }

        // GET: Tareas
        public ActionResult Tareas() {
            return View("Tareas/Index");
        }
    }
}