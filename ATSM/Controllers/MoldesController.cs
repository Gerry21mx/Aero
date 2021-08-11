using System.Web.Mvc;

using ATSM.Ingenieria;
using ATSM.Seguimiento;

namespace ATSM.Controllers {
	public class MoldesController : Controller
    {
        // GET: Moldes
        public ActionResult Index()
        {
            return View();
        }

        // GET: Moldes
        //[Route("{conreoller}/{action}/{id}")]
        public ActionResult IAeropuerto(int? id) {
            Aeropuerto iClase = new Aeropuerto(id);
            return View(iClase);
        }

        // GET: Moldes
        public ActionResult ITramo(int idtramo) {
            VueloTramo tramo = new VueloTramo(idtramo);
            ViewBag.Vuelo = tramo.GetVuelo();
            ViewBag.Tramo = tramo;
            return View();
        }
    }
}