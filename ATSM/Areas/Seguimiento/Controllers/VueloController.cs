using System.Web.Mvc;

using ATSM.Tripulaciones;

namespace ATSM.Areas.Seguimiento.Controllers {
	public class VueloController : Controller
    {
        // GET: Seguimiento/Vuelo
        public ActionResult Index()
        {
            return View();
        }

        // GET: Seguimiento/Vuelo
        public ActionResult Vuelo() {
            return View("Vuelo/Index");
        }

        // GET: TimeLine
        public ActionResult TimeLine() {
            return View("TimeLine/Index");
        }

        // GET: TimeLine
        public ActionResult General() {
            ViewBag.crews = Crew.GetCrew();
            return View("General/Index");
        }
    }
}