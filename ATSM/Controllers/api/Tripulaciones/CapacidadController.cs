using System.Web.Http;

using ATSM.Ingenieria;
using ATSM.Tripulaciones;

namespace ATSM.Controllers.api.Tripulaciones {
	public class CapacidadController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public Answer Get() {
			answer.Data = Capacidad.GetCapacidades();
			return answer;
		}
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Capacidad(id);
			return answer;
		}

		// GET api/<controller>/<cadena>/ByCapacidad
		[Route("api/Capacidad/{cadena}/ByCapacidad")]
		public Answer Get(string cadena) {
			answer.Data = new Capacidad(cadena);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(Capacidad iClase) {
			answer = Funciones.VRoles("cCapacidad");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Capacidad iClase) {
			answer = Funciones.VRoles("dCapacidad");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
