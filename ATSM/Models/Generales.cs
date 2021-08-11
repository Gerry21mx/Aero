using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Web.Security;
using WebMatrix.WebData;

namespace ATSM {
	public class Answer {
		public int Type { get; set; }
		public string Message { get; set; }
		public object Data { get; set; }
		public bool Status { get; set; }
		public Answer() {
			this.Type = 0;
			this.Message = "";
			this.Data = null;
			this.Status = false;
		}
	}
	public class Respuesta {
		public bool Valid { get; set; }
		public string Mensaje { get; set; }
		private string _Err { get; set; }
		public string Error { get; set; }
		public string Tipo { get; set; }
		public dynamic Elemento { get; set; }
		public Respuesta(bool val = false) {
			Inicializar();
			Valid = val;
		}
		public Respuesta(string msj) {
			Inicializar();
			Error = msj;
		}
		public Respuesta(bool val, string msj) {
			Inicializar();
			Valid = val;
			Mensaje = msj;
		}
		public Respuesta(bool val, string msj, string err) {
			Inicializar();
			Valid = val;
			Mensaje = msj;
			Error = err;
		}
		public Respuesta(bool val, string msj, string err, string tip) {
			Inicializar();
			Valid = val;
			Mensaje = msj;
			Error = err;
			Tipo = tip;
		}
		/// <summary>
		/// Obtiene la entidad en formato JSON
		/// </summary>
		/// <returns>Cadena de Texto con formato JSON</returns>
		public string GetJSON() {
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
		private void Inicializar() {
			Valid = false;
			Mensaje = "";
			Error = "";
			Tipo = "";
		}
	}
	public class Funciones {
		public static string GetHttp(string url) {
			string response = "";
			WebRequest oRequest = WebRequest.Create(url);
			WebResponse oResponse = oRequest.GetResponse();
			using (var oSR = new StreamReader(oResponse.GetResponseStream())) {
				response = oSR.ReadToEnd().Trim();
			}
			return response;
		}
		public static string PostHttp(string url, dynamic datos) {
			string response = "";
			WebRequest oRequest = WebRequest.Create(url);
			oRequest.Method = "post";
			oRequest.ContentType = "application/json;charset-UTF-8;";
			using (var oSW = new StreamWriter(oRequest.GetRequestStream())) {
				string json = JsonConvert.SerializeObject(datos);

				oSW.Write(json);
				oSW.Flush();
				oSW.Close();
			}

			WebResponse oResponse = oRequest.GetResponse();
			using (var oSR = new StreamReader(oResponse.GetResponseStream())) {
				response = oSR.ReadToEnd().Trim();
			}
			return response;
		}
		public static DateTime Str2Fec(string cadena) {
			DateTime res = default(DateTime);
			if (!string.IsNullOrEmpty(cadena)) {
				Int32 tipo = cadena.IndexOf("/") > -1 ? 1 : (cadena.IndexOf("-") > -1 ? 2 : 0);
				if (tipo > 0) {
					char[] separadores = new char[] { '/', '-', 'T', ':', '.' };
					var x = cadena.Split(separadores);
					if (x.Length == 3) {
						int y = Int32.Parse(x[0].Length == 4 ? x[0] : x[2]);
						int m = Int32.Parse(x[1]);
						int d = Int32.Parse(x[0].Length != 4 ? x[0] : x[2]);
						res = new DateTime(y, m, d);
					}
					else if (x.Length == 6) {
						int y = Int32.Parse(x[0].Length == 4 ? x[0] : x[2]);
						int m = Int32.Parse(x[1]);
						int d = Int32.Parse(x[0].Length != 4 ? x[0] : x[2]);
						int h = Int32.Parse(x[3]);
						int mi = Int32.Parse(x[4]);
						int s = Int32.Parse(x[5]);
						res = new DateTime(y, m, d, h, mi, s);
					}
					else if (x.Length == 7) {
						res = Convert.ToDateTime(cadena);
					}
				}
			}
			return res;
		}
		public static TimeSpan Str2TS(string cadena) {
			TimeSpan res = new TimeSpan(0);
			if (!string.IsNullOrEmpty(cadena)) {
				var x = cadena.Split(':');
				int h = 0, m = 0, s = 0;
				bool exh = false, exm = false, exs = false;
				switch (x.Length) {
					case 1:
						exm = Int32.TryParse(x[0], out m);
						break;
					case 2:
						exh = Int32.TryParse(x[0], out h);
						exm = Int32.TryParse(x[1], out m);
						break;
					case 3:
						exh = Int32.TryParse(x[0], out h);
						exm = Int32.TryParse(x[1], out m);
						exs = Int32.TryParse(x[2], out s);
						break;
					default:
						break;
				}
				res = new TimeSpan(h, m, s);
			}
			return res;
		}
		public static Answer VAuth() {
			Answer ans = new Answer();
			if (!WebSecurity.IsAuthenticated) {
				ans.Message = "Debe Iniciar Sesion";
			}
			else {
				ans.Status = true;
			}
			return ans;
		}
		public static Answer VRoles(string RoleName) {
			Answer ans = VAuth();
			if (ans.Status) {
				if (WebSecurity.CurrentUserId == 1) {
					ans.Status = true;
				}
				else {
					if (!Usuario.CurrentInRole(RoleName)) {
						ans.Status = false;
						ans.Message = "Acceso Restringido";
					}
					else {
						ans.Status = true;
					}
				}
			}
			return ans;
		}
		/// <summary>
		/// Habilita o Deshabilita el Menu en base a la validacion del Rol por usuario
		/// </summary>
		/// <param name="role">Rol a Validar</param>
		/// <returns>Clase para menu</returns>
		public static string menuRole(string role) {
			if (string.IsNullOrEmpty(role)) {
				return "nav-link disabled";
			}
			if (WebSecurity.CurrentUserId == 1) {
				return "nav-link dropdown-toggle text-white";
			}
			return Usuario.CurrentInRole(role) ? "nav-link dropdown-toggle text-white" : "nav-link disabled";
		}
		/// <summary>
		/// Habilita o Deshabilita el Sub-Menu en base a la validacion del Rol por usuario
		/// </summary>
		/// <param name="role">Rol a Validar</param>
		/// <returns>Clase para sub-menu</returns>
		public static string subMenuRole(string role) {
			if (string.IsNullOrEmpty(role)) {
				return "dropdown-item disabled";
			}
			if (WebSecurity.CurrentUserId == 1) {
				return "dropdown-item dropdown-toggle text-white";
			}
			return Usuario.CurrentInRole(role) ? "dropdown-item dropdown-toggle" : "dropdown-item disabled";
		}
		/// <summary>
		/// Devuelve la Clase del Menu si se cumple con el Rol
		/// </summary>
		/// <param name="role">Nombre del Rol</param>
		/// <returns>Clase habilitada o deshabilitada de la opcion del menu</returns>
		public static string itemRole(string role) {
			if (string.IsNullOrEmpty(role)) {
				return "nav-link disabled";
			}
			if (WebSecurity.CurrentUserId == 1) {
				return "dropdown-item";
			}
			return Usuario.CurrentInRole(role) ? "dropdown-item" : "nav-link disabled";
		}
		/// <summary>
		/// Habilita la ruta del menu o la ruta por defecto del menu segun el Rol especificado
		/// </summary>
		/// <param name="role">Nombre del Rol a Validar</param>
		/// <param name="link">Link valido para el Rol</param>
		/// <param name="linkNo">Link No valido para el Rol</param>
		/// <returns></returns>
		public static string linkRole(string role, string link, string linkNo = "") {
			if (string.IsNullOrEmpty(role)) {
				return "";
			}
			if (WebSecurity.CurrentUserId == 1) {
				return link;
			}
			return Usuario.CurrentInRole(role) ? link : linkNo;
		}
	}
	public class Momento {
		public string Avion { get; set; }
		public int Bitacora { get; set; }
		public int Pierna { get; set; }
		public TimeSpan Time { get; set; }
		public decimal Horas { get; set; }
		public decimal Minutos { get; set; }
		public int Ciclos { get; set; }
		public int Dias { get; set; }
		public int Semanas { get; set; }
		public int Meses { get; set; }
		public int Anios { get; set; }
		public Nullable<System.DateTime> Fecha { get; set; }
		public bool Valid { get; set; }
		public Momento() {
			Inicializar();
		}
		public Momento(int minutos) {
			Inicializar();
			if (minutos != 0) {
				Valid = true;
				Horas = Convert.ToDecimal(minutos) / 60;
				Minutos = minutos;
				Time = new TimeSpan(0, minutos, 0);
			}
		}
		public Momento(decimal horas) {
			Inicializar();
			if (horas != 0) {
				Valid = true;
				Horas = horas;
				Minutos = horas * 60;
				Time = new TimeSpan(0, Convert.ToInt32(Minutos), 0);
			}
		}
		public Momento(decimal horas, int ciclos) {
			Inicializar();
			if (horas != 0 || ciclos > 0) {
				Valid = true;
				Horas = horas;
				Ciclos = ciclos;
				Minutos = horas * 60;
				Time = new TimeSpan(0, Convert.ToInt32(Minutos), 0);
			}
		}
		public Momento(int minutos, int ciclos) {
			Inicializar();
			if (minutos != 0 || ciclos > 0) {
				Valid = true;
				Horas = Convert.ToDecimal(minutos) / 60;
				Ciclos = ciclos;
				Minutos = minutos;
				Time = new TimeSpan(0, minutos, 0);
			}
		}
		/// <summary>
		/// Constructor de Momento
		/// </summary>
		/// <param name="horas">Horas en Formato Decimal</param>
		/// <param name="ciclos">Ciclos Int32</param>
		/// <param name="diasMesesAños">Int32 que especifica la cantidad de Dias, Meses o Años por Default Indica Dias</param>
		/// <param name="tipo">Tipo del Parametro anterior especificado 1 Dias, 2 Semanas, 3 Meses, 4 Años</param>
		public Momento(decimal horas, int ciclos, int diasMesesAños, int tipo = 1) {
			Inicializar();
			if (horas != 0 || ciclos != 0 || diasMesesAños != 0) {
				Valid = true;
				Horas = horas;
				Ciclos = ciclos;
				Dias = tipo == 1 ? diasMesesAños : tipo == 2 ? diasMesesAños * 7 : 0;
				Semanas = tipo == 2 ? diasMesesAños : 0;
				Meses = tipo == 3 ? diasMesesAños : 0;
				Anios = tipo == 4 ? diasMesesAños : 0;
				Minutos = horas * 60;
				Time = new TimeSpan(0, Convert.ToInt32(Minutos), 0);
			}
		}
		public Momento(int minutos, int ciclos, int dias) {
			Inicializar();
			if (minutos != 0 || ciclos > 0 || dias > 0) {
				Valid = true;
				Horas = Convert.ToDecimal(minutos) / 60;
				Ciclos = ciclos;
				Dias = dias;
				Minutos = minutos;
				Time = new TimeSpan(0, minutos, 0);
			}
		}
		public Momento(string avion, int bitacora, int pierna, decimal horas, int ciclos) {
			Inicializar();
			if (horas != 0 || ciclos > 0) {
				Valid = true;
				Avion = avion;
				Bitacora = bitacora;
				Pierna = pierna;
				Horas = horas;
				Ciclos = ciclos;
				Minutos = horas * 60;
				Time = new TimeSpan(0, Convert.ToInt32(Minutos), 0);
			}
		}
		public void Add(Momento momento) {
			if (momento == null)
				momento = new Momento();
			Horas += momento.Horas;
			Minutos += momento.Minutos;
			Ciclos += momento.Ciclos;
			Dias += momento.Dias;
			Semanas += momento.Semanas;
			Meses += momento.Meses;
			Anios += momento.Anios;
			Time = Time.Add(momento.Time);
		}
		public void Subtract(Momento momento) {
			if (momento == null)
				momento = new Momento();
			Horas -= momento.Horas;
			Minutos -= momento.Minutos;
			Ciclos -= momento.Ciclos;
			Dias -= momento.Dias;
			Semanas -= momento.Semanas;
			Meses -= momento.Meses;
			Anios -= momento.Anios;
			Time = Time.Subtract(momento.Time);
		}
		public string GetJSON() {
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
		private void Inicializar() {
			Avion = "";
			Bitacora = 0;
			Pierna = 0;
			Horas = 0;
			Minutos = 0;
			Ciclos = 0;
			Dias = 0;
			Semanas = 0;
			Meses = 0;
			Anios = 0;
			Time = new TimeSpan();
			Valid = false;
		}
	}
}