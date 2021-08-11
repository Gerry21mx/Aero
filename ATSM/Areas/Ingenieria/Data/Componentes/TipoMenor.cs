using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class TipoMenor {
		public int Id { get; set; }
		public string Nombre { get; set; }
		public TipoMenor(int? id = null) {
			Id = id ?? 0;
			switch (id) {
				case 1:
				Nombre = "Componente";
				break;
				case 2:
				Nombre = "Directiva/Service Bulletin";
				break;
				case 3:
				Nombre = "Servicio";
				break;
				default:
				Id = 0;
				Nombre = "";
				break;
			}
		}
	}
}