using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ATSM.Seguimiento;

namespace ATSM.Areas.Seguimiento.Controllers.api {
    public class RutaTramoController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new RutaTramo(id);
			return answer;
		}

		// GET api/<controller>/<idRuta>/<pierna>/ByPierna
		[Route("api/RutaTramo/{idRuta}/{pierna}/ByPierna")]
		public Answer Get(int idRuta, int pierna) {
			answer.Data = new RutaTramo(idRuta, pierna);
			return answer;
		}

		// GET api/<controller>/<idRuta>-<pierna>
		[Route("api/RutaTramo/{idRuta}-{pierna}")]
		public Answer GetByPierna(int idRuta, int pierna) {
			answer.Data = new RutaTramo(idRuta, pierna);
			return answer;
		}

		// GET api/<controller>/<idRuta>/Tramos
		[Route("api/RutaTramo/{idRuta}/Tramos")]
		public Answer GetTramos(int idRuta) {
			answer.Data = RutaTramo.GetTramosRuta(idRuta);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(RutaTramo iClase) {
			answer = Funciones.VRoles("cRuta");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(RutaTramo iClase) {
			answer = Funciones.VRoles("dRuta");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
