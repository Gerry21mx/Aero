using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Operaciones.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Operaciones/Default
        public ActionResult Index()
        {
            return View();
        }
    }
}