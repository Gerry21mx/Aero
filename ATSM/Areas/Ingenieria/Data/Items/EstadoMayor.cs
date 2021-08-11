using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class EstadoMayor {
		public int Id { get; set; }
		public string Nombre { get; set; }
		public EstadoMayor(int? id = null) {
			Id = id ?? 0;
			switch (id) {
				case 1:
				Nombre = "Instalado";
				break;
				case 2:
				Nombre = "Inoperativo";
				break;
				case 3:
				Nombre = "Stock";
				break;
				default:
				Id = 0;
				Nombre = "";
				break;
			}
		}
	}
}