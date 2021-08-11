using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Pilotos.Controllers
{
    public class FichaController : Controller
    {
        // GET: Pilotos/Ficha
        public ActionResult Index()
        {
            return View();
        }
        // GET: Pilotos/Informacion
        public ActionResult Informacion() {
            return View("Informacion/Index");
        }
        // GET: Pilotos/Documentos
        public ActionResult Documentos() {
            return View("Documentos/Index");
        }
        // GET: Pilotos/Cursos
        public ActionResult Cursos() {
            return View("Cursos/Index");
        }
    }
}