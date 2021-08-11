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
    public class TipoGastoController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new TipoGasto(id);
			return answer;
		}

		// GET api/<controller>/5
		[Route("api/TipoGasto/cadena")]
		public Answer GetCadena(string cadena) {
			answer.Data = new TipoGasto(cadena);
			return answer;
		}

		// GET api/<controller>/
		public Answer Get() {
			answer.Data = TipoGasto.GetTipoGastos();
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(TipoGasto iClase) {
			answer = Funciones.VRoles("cTipoGasto");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(TipoGasto iClase) {
			answer = Funciones.VRoles("dTipoGasto");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
