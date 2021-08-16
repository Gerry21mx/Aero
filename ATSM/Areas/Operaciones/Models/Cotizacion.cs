using System.Data.SqlClient;

namespace ATSM.Operaciones {
	public class Cotizacion {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdCliente { get; set; }
		public int IdContactoCliente { get; set; }
		public int IdOrigen { get; set; }
		public int IdDestino { get; set; }
		public int Bultos { get; set; }
		public int Largo { get; set; }
		public int Alto { get; set; }
		public int Ancho { get; set; }
		public string UMEDimensiones { get; set; }
		public int Peso { get; set; }
		public string UMEPeso { get; set; }
		public int TipoServicio { get; set; }
		public bool Hazmat { get; set; }
		public bool Despaletizable { get; set; }
		public bool Valid { get; set; }
		public Cliente Cliente { get; set; }
		public ClienteContacto Contacto { get; set; }
	}
}