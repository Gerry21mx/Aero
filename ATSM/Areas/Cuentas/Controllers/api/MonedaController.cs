using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ATSM.Cuentas;

namespace ATSM.Areas.Cuentas.Controllers.api
{
    public class MonedaController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public Answer Get() {
			answer.Data = Moneda.GetMonedas();
			return answer;
		}
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Moneda(id);
			return answer;
		}

		// GET api/<controller>/<cadena>/ByCodigo
		[Route("api/Moneda/{cadena}/ByCodigo")]
		public Answer Get(string cadena) {
			answer.Data = new Moneda(cadena);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(Moneda iClase) {
			answer = Funciones.VRoles("cMoneda");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Moneda iClase) {
			answer = Funciones.VRoles("dMoneda");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
