using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ATSM.Areas.Ingenieria.Data.Planeacion {
	public class WO_Material {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdTask { get; set; }
		public int IdArticulo { get; set; }
		public decimal Cantidad { get; set; }
	}
}