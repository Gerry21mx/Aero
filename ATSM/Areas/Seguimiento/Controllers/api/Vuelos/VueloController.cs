using System;
using System.Web;
using System.Web.Http;

using ATSM.hubs;
using ATSM.Seguimiento;

using Microsoft.AspNet.SignalR;

using Newtonsoft.Json;

namespace ATSM.Areas.Seguimiento.Controllers.api {
	public class VueloController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Vuelo(id);
			return answer;
		}

		// GET api/<controller>/<cadena>/ByTrip
		[Route("api/Vuelo/{cadena}/ByTrip")]
		public Answer Get(string cadena) {
			answer.Data = new Vuelo(cadena);
			return answer;
		}

		//	PUT api/<controler>
		[Route("api/Vuelo/ReOpen")]
		public Answer PutReOpen(dynamic data) {
			answer = Funciones.VRoles("raVuelo");
			if (answer.Status) {
				int idv = 0;
				Int32.TryParse(data.idVuelo.ToString(), out idv);
				Vuelo vuelo = new Vuelo(idv);
				if (vuelo.Valid) {
					vuelo.Cerrado = false;
					var res = vuelo.Close();
					answer.Status = res.Valid;
					answer.Message = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Mensaje;
					answer.Data = res.Valid;
					if (res.Valid) {
						var context = GlobalHost.ConnectionManager.GetHubContext<VueloHub>();
						context.Clients.All.VueloAC(vuelo.IdVuelo, vuelo.Cerrado);
					}
				} else {
					answer.Status = false;
					answer.Message = "El Vuelo no Existe";
					answer.Data = false;
				}
			}
			return answer;
		}

		//	PUT api/<controler>
		[Route("api/Vuelo/Close")]
		public Answer PutClose(dynamic data) {
			answer = Funciones.VRoles("raVuelo");
			if (answer.Status) {
				int idv = 0;
				Int32.TryParse(data.idVuelo.ToString(), out idv);
				Vuelo vuelo = new Vuelo(idv);
				if (vuelo.Valid) {
					vuelo.Cerrado = true;
					foreach (var t in vuelo.Tramos) {
						if (t.IdOrigen <= 0 || t.IdDestino <= 0 || t.IdAeronave <= 0 || t.Pierna <= 0 || t.IdVuelo <= 0 || t.Salida == null || t.Llegada==null || t.Despegue==null || t.Aterrizaje== null || t.IdCapitan == null || t.IdCapitan <= 0 || t.IdCopiloto == null || t.IdCopiloto <= 0) {
							answer.Status = false;
							answer.Message = "Aun hay Tramos de Vuelo Incompletos, no puedes cerrar el vuelo";
							break;
						}
					}
					if (answer.Status) {
						var res = vuelo.Close();
						answer.Status = res.Valid;
						answer.Message = !string.IsNullOrEmpty(res.Error) ? res.Error : res.Mensaje;
						answer.Data = res.Valid;
						if (res.Valid) {
							var context = GlobalHost.ConnectionManager.GetHubContext<VueloHub>();
							context.Clients.All.VueloAC(vuelo.IdVuelo, vuelo.Cerrado);
						}
					}
				}
				else {
					answer.Status = false;
					answer.Message = "El Vuelo no Existe";
					answer.Data = false;
				}
			}
			return answer;
		}

		// POST api/<controller>
		[Route("api/Vuelo/Vuelos")]
		public Answer PostVuelos(dynamic datos) {
			DateTime? desde = null;
			DateTime? hasta = null;
			bool cerrado = false;
			try {
				desde = datos.desde == null ? null : (DateTime)datos.desde;
				hasta = datos.hasta == null ? null : (DateTime)datos.hasta;
				cerrado = (bool)datos.cerrado;
			}
			catch (Exception ex) {
				answer.Message = $"Fallo la Conversion de los Parametros Enviados.<br>{ex.Message}";
			}
			answer.Data = Vuelo.GetVuelos(desde, hasta,cerrado);
			return answer;
		}

		// POST api/<controller>
		[Route("api/Vuelo/Reporte")]
		public Answer PostReporte(dynamic datos) {
			string trip = null;
			int idaeronave = 0;
			DateTime? desde = null;
			DateTime? hasta = null;
			int idtipovuelo = 0;
			int idorigen = 0;
			int iddestino = 0;
			int idcapitan = 0;
			int idcopiloto = 0;
			int iddemora = 0;
			int idruta = 0;
			int idcapacidad = 0;
			int? estado = null;
			bool tipo_reporte = false;
			try { trip = (string)datos.trip;
				desde = datos.desde == null ? null : (DateTime)datos.desde;
				if (datos.hasta != null) {
					DateTime fec = (DateTime)datos.hasta;
					hasta = new DateTime(fec.Year, fec.Month, fec.Day, 23, 59, 59);
				}
				if (datos.idaeronave != null)
					Int32.TryParse(datos.idaeronave.ToString(), out idaeronave);
				if (datos.idtipovuelo != null)
					Int32.TryParse(datos.idtipovuelo.ToString(), out idtipovuelo);
				if (datos.idorigen != null)
					Int32.TryParse(datos.idorigen.ToString(), out idorigen);
				if (datos.iddestino != null)
					Int32.TryParse(datos.iddestino.ToString(), out iddestino);
				if (datos.idcapitan != null)
					Int32.TryParse(datos.idcapitan.ToString(), out idcapitan);
				if (datos.idcopiloto != null)
					Int32.TryParse(datos.idcopiloto.ToString(), out idcopiloto);
				if (datos.iddemora != null)
					Int32.TryParse(datos.iddemora.ToString(), out iddemora);
				if (datos.idruta != null)
					Int32.TryParse(datos.idruta.ToString(), out idruta);
				if (datos.idcapacidad != null)
					Int32.TryParse(datos.idcapacidad.ToString(), out idcapacidad);
				if(datos.estado != null) {
					int edo = -1;
					Int32.TryParse(datos.estado.ToString(), out edo);
					estado = edo != -1 ? edo : null;
				}
				if (datos.tipo_reporte != null)
					tipo_reporte = (bool)datos.tipo_reporte;
			}
			catch (Exception ex) {
				answer.Message = $"Fallo la Conversion alguno de los Parametros Enviados.<br>{ex.Message}";
			}
			answer.Data = Vuelo.GetReporte(trip, idaeronave, desde, hasta, idtipovuelo, idorigen, iddestino, idcapitan, idcopiloto, iddemora, idruta, idcapacidad, estado, tipo_reporte);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(dynamic data) {
			answer = Funciones.VRoles("cVuelo");
			if (answer.Status) {
				data.Aeronave = null;
				foreach(var tra in data.Tramos) {
					tra.Aeronave = null;
				}
				Vuelo iClase = JsonConvert.DeserializeObject<Vuelo>(JsonConvert.SerializeObject(data));
				respuesta = iClase.Save();
				if (respuesta.Valid) {
					iClase.GetAeronave();
					var context = GlobalHost.ConnectionManager.GetHubContext<VueloHub>();
					context.Clients.All.AddVuelo(iClase);
				}
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(dynamic objeto) {
			answer = Funciones.VRoles("dVuelo");
			if (answer.Status) {
				int idvuelo = (int)objeto.IdVuelo;
				Vuelo iClase = new Vuelo(idvuelo);
				respuesta = iClase.Delete();
				if (respuesta.Valid) {
					var context = GlobalHost.ConnectionManager.GetHubContext<VueloHub>();
					context.Clients.All.DelVuelo(idvuelo);
				}
				return respuesta;
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}