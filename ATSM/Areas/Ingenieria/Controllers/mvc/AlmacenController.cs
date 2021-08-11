using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Ingenieria.Controllers
{
    public class AlmacenController : Controller
    {
        // GET: Ingenieria/Almacen
        public ActionResult Index()
        {
            return View();
        }

        // GET: Ingenieria/Articulos
        public ActionResult Articulos()
        {
            return View("Articulos/Index");
        }

        // GET: Ingenieria/Articulos/Registro
        public ActionResult Registro() {
            return View("Articulos/Registro/Index");
        }

        // GET: Ingenieria/Articulos/Categoria
        public ActionResult Categoria() {
            return View("Articulos/Categoria/Index");
        }

        // GET: Ingenieria/Articulos/Ume
        public ActionResult Ume() {
            return View("Articulos/Ume/Index");
        }

        // GET: Ingenieria/Alternos
        public ActionResult Alternos()
        {
            return View("Alternos/Index");
        }

        // GET: Ingenieria/Incoming
        public ActionResult Incoming()
        {
            return View("Incoming/Index");
        }

        // GET: Ingenieria/Items
        public ActionResult Items()
        {
            return View("Items/Index");
        }
    }
}