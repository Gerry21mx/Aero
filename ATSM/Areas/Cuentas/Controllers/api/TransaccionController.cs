using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ATSM.Cuentas;

namespace ATSM.Areas.Cuentas.Controllers.api
{
    public class TransaccionController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer = Funciones.VRoles("Transaccion");
			if (answer.Status) {
				answer.Data = new Transaccion(id);
			}
			return answer;
		}

		// POST api/<controller>
		[Route("api/Transaccion/Transaccions")]
		public Answer Post(dynamic data) {
			answer = Funciones.VRoles("Transaccion");
			if (answer.Status) {
				int idc = 0;
				int idm = 0;
				try {
					idc = (Int32)data.idcuenta;
					idm = (Int32)data.idmovimiento;
				}
				catch (Exception) {
					answer.Message = "Error al Convertir los Parametros";
				}
				answer.Data = Transaccion.GetTransacciones(idc, idm);
			}
			answer.Message = answer.Message;
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(Transaccion iClase) {
			answer = Funciones.VRoles("cTransaccion");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
