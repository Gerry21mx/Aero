using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ATSM.Gastos;

namespace ATSM.Areas.Gastos.Controllers.api
{
    public class GastoController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Gasto(id);
			return answer;
		}

		// GET api/<controller>/Comprobacion
		[Route("api/Gasto/Comprobacion")]
		public Answer GetGastosComprobacion(int idcomprobacion) {
			answer.Data = Gasto.GetGastos(idcomprobacion);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(Gasto iClase) {
			answer = Funciones.VRoles("cGastos");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Gasto iClase) {
			answer = Funciones.VRoles("dGastos");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
