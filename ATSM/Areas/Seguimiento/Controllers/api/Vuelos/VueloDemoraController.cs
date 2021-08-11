using System.Web;
using System.Web.Http;

using ATSM.Seguimiento;

namespace ATSM.Areas.Seguimiento.Controllers.api {
	public class VueloDemoraController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();
		
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new VueloDemora(id);
			return answer;
		}

		// GET api/<controller>/5
		[Route("api/VueloDemora/{idtramo}/ByTramo")]
		public Answer GetByTramo(int idtramo) {
			answer.Data = VueloDemora.GetDemorasTramo(idtramo);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(VueloDemora iClase) {
			answer = Funciones.VRoles("cVuelo");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(VueloDemora iClase) {
			answer = Funciones.VRoles("dVuelo");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}