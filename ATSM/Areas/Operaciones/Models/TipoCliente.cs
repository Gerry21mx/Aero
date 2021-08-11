using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ATSM.Operaciones {
	public class TipoCliente {
		public int Id { get; set; }
		public string Tipo { get; set; }
		public TipoCliente(int? id = null) {
			Id = id ?? 0;
			switch (id) {
				case 1:
				Tipo = "Nuevos";
				break;
				case 2:
				Tipo = "AAA";
				break;
				case 3:
				Tipo = "AA";
				break;
				case 4:
				Tipo = "A";
				break;
				case 5:
				Tipo = "Agencia Aduanal";
				break;
				case 6:
				Tipo = "Similares";
				break;
				case 7:
				Tipo = "Baja";
				break;
				case 8:
				Tipo = "Otros";
				break;
				default:
				Tipo = "Indefinido";
				break;
			}
		}
	}
}