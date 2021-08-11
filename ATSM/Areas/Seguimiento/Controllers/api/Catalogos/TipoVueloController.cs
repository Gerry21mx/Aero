using System.Web.Http;

using ATSM.Seguimiento;

namespace ATSM.Areas.Seguimiento.Controllers.api {
	public class TipoVueloController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public Answer Get() {
			answer.Data = TipoVuelo.GetTipoVuelos();
			return answer;
		}
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new TipoVuelo(id);
			return answer;
		}

		// GET api/<controller>/<cadena>/ByTipo
		[Route("api/TipoVuelo/{cadena}/ByTipo")]
		public Answer Get(string cadena) {
			answer.Data = new TipoVuelo(cadena);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(TipoVuelo iClase) {
			answer = Funciones.VRoles("cTipoVuelo");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(int id) {
		//public Respuesta Delete(TipoVuelo iClase) {
			answer = Funciones.VRoles("dTipoVuelo");
			if (answer.Status) {
				TipoVuelo iClase = new TipoVuelo(id);
				if (iClase.Valid) {
					respuesta = iClase.Delete();
				} else {
					respuesta.Mensaje = "No se encontro la entidad a eliminar";
				}
				return respuesta;
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
