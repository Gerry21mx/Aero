using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Areas.Seguimiento.Controllers
{
    public class CatalogoController : Controller
    {
        // GET: Seguimiento/Catalogo
        public ActionResult Index()
        {
            return View();
        }
        // GET: Aeropuerto
        public ActionResult Aeropuerto() {
            return View("Aeropuerto/Index");
        }

        // GET: Demora
        public ActionResult Demora() {
            RespuestaQuery rCla = DataBase.Query(new SqlCommand("SELECT DISTINCT Clasificacion FROM Demora ORDER BY Clasificacion", DataBase.Conexion()));
            ViewBag.Clasificacion = rCla.Rows;
            return View("Demora/Index");
        }

        // GET: Ruta
        public ActionResult Ruta() {
            return View("Ruta/Index");
        }

        // GET: TipoVuelo
        public ActionResult TipoVuelo() {
            return View("TipoVuelo/Index");
        }
    }
}
