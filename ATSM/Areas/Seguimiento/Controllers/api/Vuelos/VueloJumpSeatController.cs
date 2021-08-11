using System;
using System.Web;
using System.Web.Http;

using ATSM.hubs;
using ATSM.Seguimiento;

using Microsoft.AspNet.SignalR;

using Newtonsoft.Json;

namespace ATSM.Areas.Seguimiento.Controllers.api {
	public class VueloJumpSeatController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new VueloJumpSeat(id);
			return answer;
		}

		// GET api/<controller>
		[Route("api/VueloJumpSeat")]
		public Answer Get(int idtramo, string nombre) {
			answer.Data = new VueloJumpSeat(idtramo, nombre);
			return answer;
		}

		// GET api/<controller>
		[Route("api/VueloJumpSeat/ByTramo")]
		public Answer GetByTramo(int idtramo) {
			answer.Data = VueloJumpSeat.GetVueloJumpSeats(idtramo);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(VueloJumpSeat iClase) {
			answer = Funciones.VRoles("cVueloJumpSeat");
			if (answer.Status) {
				respuesta = iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(VueloJumpSeat iClase) {
			answer = Funciones.VRoles("dVueloJumpSeat");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}