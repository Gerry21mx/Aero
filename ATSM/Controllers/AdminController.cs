using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATSM.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Perfil
        public ActionResult Perfil() {
            SqlCommand comando = new SqlCommand("SELECT DISTINCT Area FROM webpages_Roles ORDER BY Area", DataBase.Conexion());
            ViewBag.Areas = DataBase.Query(comando).Rows;
            return View("Perfil/Index");
        }

        // GET: Admin/Usuario
        public ActionResult Usuario() {
            return View("Usuario/Index");
        }
    }
}