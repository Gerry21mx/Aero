using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class WO_Task {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdWorkOrder { get; set; }	
		public int IdItem { get; set; }
		//public int IdMiscelanea { get; set; }
		public DateTime Fecha_Programacion { get; set; }    //	Reservado para la empresa
		public DateTime Fecha_Apertura { get; set; }
		public int UsuCierra { get; set; }
		public DateTime Fecha_Cierre { get; set; }
		public int UsuValida { get; set; }
		public int Fecha_Valida { get; set; }
		public decimal TAT_Programacion { get; set; }			//	Reservado para la empresa
		public int TAC_Programacion { get; set; }				//	Reservado para la empresa
		public decimal RemHrs_Programacion { get; set; }		//	Reservado para la empresa
		public int RemCyc_Programacion { get; set; }            //	Reservado para la empresa
		public decimal LimHrs { get; set; }                     //	Reservado para la empresa
		public int LimCyc { get; set; }                         //	Reservado para la empresa
		public int LimDays { get; set; }                        //	Reservado para la empresa
		public int IdTecnico { get; set; }
		public decimal Horas { get; set; }
		public int IdInspector { get; set; }
		public string Observaciones { get; set; }
		public string ObservacionesInternas { get; set; }
		public DateTime Fecha_Interna { get; set; }    //	Reservado para la empresa
		
	}
}