using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ATSM.Cuentas;

namespace ATSM.Areas.Cuentas.Controllers.api
{
    public class AccountTypeTypeController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public dynamic Get() {
			return AccountType.GetAccountTypes();
		}

		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new AccountType(id);
			return answer;
		}

		// GET api/<controller>/<cadena>/CodNom
		[Route("api/AccountType/{cadena}/CodNom")]
		public Answer Get(string cadena) {
			answer.Data = new AccountType(cadena);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(AccountType iClase) {
			answer = Funciones.VRoles("cAccountType");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(AccountType iClase) {
			answer = Funciones.VRoles("dAccountType");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
