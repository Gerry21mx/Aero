using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Operaciones.Controllers
{
    public class PruebaController : Controller
    {
        // GET: Operaciones/Prueba
        public ActionResult Index()
        {
            return View("prueba.vue");
        }
    }
}