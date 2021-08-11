using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class WO_Componentes {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdPIC { get; set; }
		public int IdComponenteMenorRemovido { get; set; }
		public int SerieComponenteMenorRemovido { get; set; }
		public int IdItemRemovido { get; set; }
		public ItemMenor MenorRemovido { get; set; }
		public decimal? TSN_Removido { get; set; }
		public int? CSN_Removido { get; set; }
		public int? Dias_Removido { get; set; }
		public decimal? TSN_Componente_Instalacion_Removido { get; set; }
		public int? CSN_Componente_Instalacion_Removido { get; set; }
		public DateTime? Fecha_Componente_Instalacion_Removido { get; set; }
		public decimal? TSN_Airframe_Instalacion_Removido { get; set; }
		public int? CSN_Airframe_Instalacion_Removido { get; set; }
		public DateTime? Fecha_Airframe_Instalacion_Removido { get; set; }
		public List<Tiempos> Tiempos_Removido = new List<Tiempos>();

		public int IdComponenteMenorInstalado { get; set; }
		public int SerieComponenteMenorInstalado { get; set; }
		public int IdItemInstalado { get; set; }
		public ItemMenor MenorInstalado { get; set; }
		public decimal? TSN_Instalado { get; set; }
		public int? CSN_Instalado { get; set; }
		public int? Dias_Instalado { get; set; }
		public decimal? TSN_Componente_Instalacion_Instalado { get; set; }
		public int? CSN_Componente_Instalacion_Instalado { get; set; }
		public DateTime? Fecha_Componente_Instalacion_Instalado { get; set; }
		public decimal? TSN_Airframe_Instalacion_Instalado { get; set; }
		public int? CSN_Airframe_Instalacion_Instalado { get; set; }
		public DateTime? Fecha_Airframe_Instalacion_Instalado { get; set; }
		public List<Tiempos> Tiempos_Instalado = new List<Tiempos>();
	}
}