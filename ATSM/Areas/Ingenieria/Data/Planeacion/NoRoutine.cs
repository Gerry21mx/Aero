using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class NoRoutine {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int WOT_Reference { get; set; }
	}
}