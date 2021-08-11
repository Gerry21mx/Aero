using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Operaciones.Controllers
{
    public class CatalogosController : Controller
    {
        // GET: Operaciones/Catalogos
        public ActionResult Index()
        {
            return View();
        }

        // GET: Operaciones/Aeropuertos
        public ActionResult Aeropuertos() {
            return View("Aeropuertos/Index");
        }

        // GET: Operaciones/Clientes
        public ActionResult Clientes() {
            return View("Clientes/Index");
        }
    }
}