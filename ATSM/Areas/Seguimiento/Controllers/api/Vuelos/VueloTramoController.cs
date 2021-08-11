using System;
using System.Web.Http;

using ATSM.hubs;
using ATSM.Seguimiento;

using Microsoft.AspNet.SignalR;

using Newtonsoft.Json;

namespace ATSM.Areas.Seguimiento.Controllers.api {
	public class VueloTramoController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new VueloTramo(id);
			return answer;
		}

		// GET api/<controller>/<idVuelo>/<pierna>/ByPierna
		[Route("api/VueloTramo/{idVuelo}/{pierna}/ByPierna")]
		public Answer Get(int idVuelo, int pierna) {
			answer.Data = new VueloTramo(idVuelo,pierna);
			return answer;
		}

		// GET api/<controller>/<idRuta>/Tramos
		[Route("api/VueloTramo/{idVuelo}/Tramos")]
		public Answer GetTramos(int idVuelo) {
			answer.Data = VueloTramo.GetTramosVuelo(idVuelo);
			return answer;
		}

		// POST api/<controller>
		//public Respuesta Post(VueloTramo iClase) {
		public Respuesta Post(dynamic datos) {
			datos.Aeronave = null;
			datos.Capitan = null;
			datos.Copiloto = null;
			datos.Destino = null;
			datos.Origen = null;
			VueloTramo iClase = JsonConvert.DeserializeObject<VueloTramo>(JsonConvert.SerializeObject(datos));
			answer = Funciones.VRoles("cVuelo");
			if (answer.Status) {
				respuesta = iClase.Save();
				if (respuesta.Valid) {
					if (iClase.Aeronave == null) {
						iClase.GetAeronave();
					}
					if (iClase.Origen == null) {
						iClase.GetOrigen();
					}
					if (iClase.Destino == null) {
						iClase.GetDestino();
					}
					if (iClase.Capitan == null && iClase.IdCapitan != null) {
						iClase.GetTripulacion();
					}
					respuesta.Elemento = iClase;
					var context = GlobalHost.ConnectionManager.GetHubContext<VueloHub>();
					context.Clients.All.AddTramo(iClase);
				}
				return respuesta;
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(VueloTramo iClase) {
			answer = Funciones.VRoles("dVuelo");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// POST api/<controller>
		[Route("api/VueloTramo/TimeLine")]
		public Answer PostTimeLine(dynamic datos) {
			DateTime hasta = DateTime.Now;
			DateTime desde = hasta.AddDays(-7);
			int? idtramo = null;
			try {
				if (datos.desde != null) {
					DateTime fec = (DateTime)datos.desde;
					desde = new DateTime(fec.Year, fec.Month, fec.Day, 0, 0, 0);
					if (datos.hasta == null) {
						fec = desde.AddDays(7);
						hasta = new DateTime(fec.Year, fec.Month, fec.Day, 23, 59, 59);
					}
				}
				if (datos.hasta != null) {
					DateTime fec = (DateTime)datos.hasta;
					hasta = new DateTime(fec.Year, fec.Month, fec.Day, 23, 59, 59);
					if (datos.desde == null) {
						fec = hasta.AddDays(-7);
						desde = new DateTime(fec.Year, fec.Month, fec.Day, 0, 0, 0);
					}
				}
				idtramo = datos.idtramo != null ? (int)datos.idtramo : null;
			}
			catch (Exception ex) {
				answer.Message = $"Fallo la Conversion alguno de los Parametros Enviados.<br>{ex.Message}";
			}
			answer.Data = VueloTramo.GetTimeLine(desde, hasta, idtramo);
			return answer;
		}
	}
}
