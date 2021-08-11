using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ATSM.Seguimiento;

namespace ATSM.Areas.Seguimiento.Controllers.api {
    public class DemoraController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public Answer Get() {
			answer.Data = Demora.GetDemoras();
			return answer;
		}
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Demora(id);
			return answer;
		}

		// GET api/<controller>/<cadena>/ByCodigo
		[Route("api/Demora/{cadena}/ByCodigo")]
		public Answer Get(string cadena) {
			answer.Data = new Demora(cadena);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(Demora iClase) {
			answer = Funciones.VRoles("cDemora");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Demora iClase) {
			answer = Funciones.VRoles("dDemora");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
