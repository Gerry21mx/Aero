using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Ingenieria {
    public class ItemMayorController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>/Id
		public Answer Get(int id) {
			answer.Data = new ItemMayor(id);
			return answer;
		}

		// GET api/<controller>/ByCadena
		[Route("api/ItemMayor/ByCadena")]
		public Answer Get(int idMayor, int idModelo, string cadena) {
			answer.Data = new ItemMayor(idMayor, idModelo, cadena);
			return answer;
		}

		// GET api/<controller>/ByComponente
		/// <summary>
		/// Lista de Items Mayores de Un Componente Mayor
		/// </summary>
		/// <param name="idMayor">Id de Componente Mayor</param>
		/// <returns>Lista de Items Mayores</returns>
		[Route("api/ItemMayor/ByComponente")]
		public Answer GetByComponente(int idMayor) {
			var items = ItemMayor.GetItemMayors(idMayor);
			foreach(var item in items) {
				item.SetAircraft();
				item.SetModelo();
				item.SetPosicion();
				item.SetLimites();
				item.SetTiempos();
			}
			answer.Data = items;
			return answer;
		}

		// GET api/<controller>/ItemsByModelo
		[Route("api/ItemMayor/ItemsByModelo")]
		public Answer GetItemsByModelo(int idComponente, int idModelo) {
			var items = ItemMayor.GetItemMayoresByModelo(idComponente, idModelo);
			foreach (var item in items) {
				item.SetAircraft();
				item.SetModelo();
				item.SetPosicion();
				item.SetLimites();
				item.SetTiempos();
			}
			answer.Data = items;
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(ItemMayor iClase) {
			answer = Funciones.VRoles("cItemMayor");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(ItemMayor iClase) {
			answer = Funciones.VRoles("dItemMayor");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}