using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using ATSM.Gastos;

using Newtonsoft.Json;

namespace ATSM.Areas.Gastos.Controllers.api {
	public class ComprobacionController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Comprobacion(id);
			return answer;
		}

		// GET api/<controller>/byVuelo
		[Route("api/Comprobacion/byVuelo")]
		public Answer GetByVuelo(int idvuelo, int idaccount) {
			answer.Data = new Comprobacion(idvuelo, idaccount);
			return answer;
		}

		// GET api/<controller>/Comprobaciones
		[Route("api/Comprobacion/Comprobaciones")]
		public Answer GetComprobacionesCuenta(int idaccount) {
			answer.Data = Comprobacion.GetComprobaciones(idaccount);
			return answer;
		}

		// GET api/<controller>/Pendientes
		[Route("api/Comprobacion/Pendientes")]
		public Answer GetVuelosPendientesComprobarPiloto(int idaccount) {
			answer.Data = Comprobacion.GetVuelosPendientesComprobarPiloto(idaccount);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post() {
			answer = Funciones.VRoles("cGastos");
			if (answer.Status) {
				HttpRequest Request = HttpContext.Current.Request;
				string strData = Request["comprobacion"];
				Comprobacion iClase = JsonConvert.DeserializeObject<Comprobacion>(strData);
				respuesta = iClase.Save();
				if (respuesta.Valid) {
					string ruta = Request.MapPath($"~/Files/Gastos/Comp/Pilotos/{iClase.Id}/");
					var archivos = Request.Files;
					foreach (string file in archivos) {
						var postedFile = Request.Files[file];
						var pars = file.Split('_');
						if (pars.Length != 3) continue;
						int indGas = -1;
						Int32.TryParse(pars[2], out indGas);
						if (!string.IsNullOrEmpty(postedFile.FileName) && indGas > -1) {
							if (!Directory.Exists(ruta))
								Directory.CreateDirectory(ruta);
							string arc = postedFile.FileName.Trim();
							string ext = Path.GetExtension(arc);
							string noma = $"{pars[1]}_{iClase.Gastos[indGas].Id}{ext}";
							if (noma != "") {
								postedFile.SaveAs(ruta + noma);
							}
						}
					}
				}
				return respuesta;
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Comprobacion iClase) {
			answer = Funciones.VRoles("dComprobacion");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}