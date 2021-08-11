using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class Miscelanea {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdWorkOrder { get; set; }
		public int Descripcion { get; set; }
		public int Accion { get; set; }
		public int Referencia { get; set; }
		public int UsuCrea { get; set; }
		public int Fecha { get; set; }

	}
}