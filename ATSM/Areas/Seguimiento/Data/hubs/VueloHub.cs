using ATSM.Seguimiento;

using Microsoft.AspNet.SignalR;

namespace ATSM.hubs {
	public class VueloHub : Hub {
		public void AddVuelo(int idVuelo) {
			Vuelo vuelo = new Vuelo(idVuelo);
			Clients.All.AddVuelo(vuelo);
		}
		public void AddTramo(int idTramo) {
			VueloTramo tramo = new VueloTramo(idTramo);
			Clients.All.AddTramo(tramo);
		}
		/// <summary>
		/// Cambio de Estado de Vuelo Cerrado/Abierto
		/// </summary>
		/// <param name="idVuelo">Id de Vuelo</param>
		public void VueloAC(int idVuelo, bool estado) {
			Clients.All.VueloAC(idVuelo, estado);
		}
	}
}