using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ATSM.Cuentas;

namespace ATSM.Areas.Cuentas.Controllers.api
{
    public class SaldoController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public Answer Get() {
			answer.Data = Saldo.GetSaldos();
			return answer;
		}
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Saldo(id);
			return answer;
		}

		// GET api/<controller>/<cadena>/ByCodigo
		[Route("api/Saldo/{cadena}/ByCodigo")]
		public Answer Get(string cadena) {
			answer.Data = new Saldo(cadena);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(Saldo iClase) {
			answer = Funciones.VRoles("cSaldo");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Saldo iClase) {
			answer = Funciones.VRoles("dSaldo");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
