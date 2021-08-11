using System.Web.Http;

using ATSM.Seguimiento;

namespace ATSM.Areas.Seguimiento.Controllers.api {
	public class AeropuertoController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public Answer Get() {
			answer.Data = Aeropuerto.GetAeropuertos();
			return answer;
		}
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Aeropuerto(id);
			return answer;
		}

		// GET api/<controller>/<cadena>/ByAeropuerto
		[Route("api/Aeropuerto/{cadena}/ByAeropuerto")]
		public Answer Get(string cadena) {
			answer.Data = new Aeropuerto(cadena);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(Aeropuerto iClase) {
			answer = Funciones.VRoles("cAeropuerto");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Aeropuerto iClase) {
			answer = Funciones.VRoles("dAeropuerto");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
