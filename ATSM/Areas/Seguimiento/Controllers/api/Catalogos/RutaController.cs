using System.Web.Http;

using ATSM.Seguimiento;

namespace ATSM.Areas.Seguimiento.Controllers.api {
	public class RutaController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public Answer Get() {
			var rutas= Ruta.GetRutas();
			foreach(var ruta in rutas) {
				ruta.GetTipo();
			}
			answer.Data = rutas;
			return answer;
		}
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Ruta(id);
			return answer;
		}

		// GET api/<controller>/<cadena>/ByCodigo
		[Route("api/Ruta/{cadena}/ByCodigo")]
		public Answer Get(string cadena) {
			answer.Data = new Ruta(cadena);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(Ruta iClase) {
			answer = Funciones.VRoles("cRuta");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Ruta iClase) {
			answer = Funciones.VRoles("dRuta");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
