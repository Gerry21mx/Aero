using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class WorkOrder {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdItemMayor { get; set; }
		public ItemMayor ItemMayor { get; set; }
		public string Destinatario { get; set; }
		public string Estacion { get; set; }
		public DateTime Fecha_Programacion { get; set; }
		public decimal TAT_Programacion { get; set; }		//	Reservado para la empresa
		public int TAC_Programacion { get; set; }            //	Reservado para la empresa
		public int IdBitacora { get; set; }            //	Reservado para la empresa
		public decimal TAT_Instalacion { get; set; }
		public int TAC_Instalacion { get; set; }
		public DateTime Fecha_Cierre { get; set; }
		public int UsuCrea { get; set; }
		public int UsuCierra { get; set; }
		public int UsuCancela { get; set; }
		public DateTime Fecha_Cancelacion { get; set; }
		public string Observaciones_Cancelacion { get; set; }
	}
}