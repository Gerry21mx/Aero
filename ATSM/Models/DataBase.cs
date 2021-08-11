using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;

using Newtonsoft.Json;

namespace ATSM {
	public class DataBase {
		public static SqlConnection Conexion(string baseName = null) {
			dynamic CadenaConexion = OConexion(baseName ?? "DATA");
			baseName = baseName ?? CadenaConexion.Database;
			SqlConnection Conexion = new SqlConnection($@"Server = {CadenaConexion.Server}; Database = {baseName}; Uid = {CadenaConexion.Uid}; Pwd = {CadenaConexion.Pwd}");
			return Conexion;
		}
		/// <summary>
		/// Ejecuta un comando y devuelve una entidad que representa el resultado de la consulta. (ExecuteReader)
		/// </summary>
		/// <param name="Comando">SqlCommand Consulta</param>
		/// <param name="CerrarConexion">No Cerrar Conexion</param>
		public static RespuestaQuery Query(SqlCommand Comando, bool CerrarConexion = true) {
			RespuestaQuery res = new RespuestaQuery();
			res.Consulta = Comando.CommandText;
			for (var i = 0; i < Comando.Parameters.Count; i++) {
				res.Parametros.Add(new { Nombre = Comando.Parameters[i].ParameterName, Valor = Comando.Parameters[i].Value });
			}
			if (Comando.Connection.State == System.Data.ConnectionState.Closed) {
				Comando.Connection.Open();
			}
			SqlDataReader Reader = null;
			try {
				Reader = Comando.ExecuteReader();
				if (Reader.HasRows) {
					int cntr = 0;
					while (Reader.Read()) {
						dynamic objeto = new ExpandoObject();
						for (var i = 0; i < Reader.FieldCount; i++) {
							var valor = Reader.IsDBNull(i) ? null : Reader.GetValue(i);
							if (Reader.GetFieldType(i).Name == "String" && valor != null) {
								valor = valor.ToString().Trim();
							}
							((IDictionary<String, Object>)objeto).Add(Reader.GetName(i), valor);
						}
						if (cntr == 0) {
							res.Row = objeto;
						}
						res.Rows.Add(objeto);
						cntr++;
					}
					res.Afectados = cntr;
					res.Valid = true;
				}
				if (Reader.RecordsAffected > 0) {
					res.Afectados = Reader.RecordsAffected;
					res.Valid = true;
				}
			}
			catch (Exception ex) {
				res.Error = @"Error en la Consulta" + Environment.NewLine;
				res.Error += ex.Message + Environment.NewLine;
				res.Error += ex.HelpLink + Environment.NewLine;
				res.Error += ex.HResult + Environment.NewLine;
			}
			finally {
				if (Reader != null) {
					Reader.Close();
				}
				if (CerrarConexion) {
					if (Comando.Connection.State == System.Data.ConnectionState.Open) {
						Comando.Connection.Close();
					}
				}
			}
			return res;
		}
		public static RespuestaQuery Query(string SqlQueryString, bool CerrarConexion = true) {
			RespuestaQuery res = DataBase.Query(new SqlCommand(SqlQueryString, Conexion()), CerrarConexion);
			return res;
		}
		/// <summary>
		/// Devuelve un valor unico de una Consulta, especificamente el primer valor del primer registro. (ExecuteScalar)
		/// </summary>
		/// <param name="Comando">SqlCommand Consulta</param>
		/// <param name="CerrarConexion">No Cerrar Conexion</param>
		/// <returns>Objeto Valor</returns>
		public static object QueryValue(SqlCommand Comando, bool CerrarConexion = true) {
			if (Comando.Connection.State == System.Data.ConnectionState.Closed) {
				Comando.Connection.Open();
			}
			var res = Comando.ExecuteScalar();
			if (CerrarConexion) {
				if (Comando.Connection.State == System.Data.ConnectionState.Open) {
					Comando.Connection.Close();
				}
			}
			return res;
		}
		public static object QueryValue(string SqlQueryString, bool CerrarConexion = true) {
			object res = DataBase.QueryValue(new SqlCommand(SqlQueryString, Conexion()), CerrarConexion);
			return res;
		}
		/// <summary>
		/// Ejecuta una sentencia Insert y devuelve el Id Autoincremental agregado
		/// </summary>
		/// <param name="Comando">SqlCommand Comando Query</param>
		/// <param name="CerrarConexion">No Cerrar Conexion</param>
		/// <returns></returns>
		public static RespuestaQuery Insert(SqlCommand Comando, bool CerrarConexion = true) {
			RespuestaQuery res = new RespuestaQuery();
			res.Consulta = Comando.CommandText;
			int tipo = 0;
			if (Comando.CommandText.ToUpper().IndexOf("INSERT") == -1) {
				if (Comando.CommandText.ToUpper().IndexOf("UPDATE") == -1) {
					res.Error = "El comando no contiene ninguna Instruccion INSERT o UPDATE";
					return res;
				}
				else {
					tipo = 2;
				}
			}
			else {
				tipo = 1;
			}
			if (Comando.CommandText.ToUpper().IndexOf("SELECT SCOPE_IDENTITY();") == -1 && tipo == 1) {
				Comando.CommandText += ";SELECT SCOPE_IDENTITY();";
			}
			for (var i = 0; i < Comando.Parameters.Count; i++) {
				res.Parametros.Add(new { Nombre = Comando.Parameters[i].ParameterName, Valor = Comando.Parameters[i].Value });
			}
			if (Comando.Connection.State == System.Data.ConnectionState.Closed) {
				Comando.Connection.Open();
			}
			try {
				int id = 0;
				var r = Comando.ExecuteScalar();
				if (r != null) {
					if (r.GetType().Name == "Decimal") {
						id = Convert.ToInt32(r);
					}
					else {
						int.TryParse(r.ToString(), out id);
					}
				}
				res.IdRegistro = id;
				res.Valid = true;
				res.Afectados = 1;
			}
			catch (Exception ex) {
				res.Error = @"Error en el QUERY<br>";
				res.Error += ex.Message + "<br>";
				res.Error += ex.HelpLink + "<br>";
				res.Error += ex.HResult + "<br>";
			}
			finally {
				if (CerrarConexion) {
					if (Comando.Connection.State == System.Data.ConnectionState.Open) {
						Comando.Connection.Close();
					}
				}
			}
			return res;
		}
		/// <summary>
		/// Ejecuta una instrucción de Transact-SQL (ExecuteNonQuery) en la conexión y devuelve el número de filas afectadas
		/// </summary>
		/// <param name="Comando">Comando SqlCommand</param>
		public static RespuestaQuery Execute(SqlCommand Comando, bool CerrarConexion = true) {
			RespuestaQuery res = new RespuestaQuery();
			if (Comando.Connection.State == System.Data.ConnectionState.Closed) {
				Comando.Connection.Open();
			}
			try {
				res.Afectados = Comando.ExecuteNonQuery();
				res.Valid = true;
			}
			catch (Exception ex) {
				res.Error = "Error en el Comando: " + Environment.NewLine;
				res.Error += ex.Message + Environment.NewLine;
				res.Error += ex.HelpLink + Environment.NewLine;
				res.Error += ex.HResult + Environment.NewLine;
			}
			finally {
				if (CerrarConexion) {
					if (Comando.Connection.State == System.Data.ConnectionState.Open) {
						Comando.Connection.Close();
					}
				}
			}
			return res;
		}
		public static RespuestaQuery Execute(string SqlQueryString, bool CerrarConexion = true) {
			RespuestaQuery res = DataBase.Execute(new SqlCommand(SqlQueryString, Conexion()), CerrarConexion);
			return res;
		}
		/// <summary>
		///	Devuelve un objeto Conexion basado en la informacion obtenida en la configuracion de conexiones del archivo web.config.
		/// </summary>
		/// <param name="conectionStringName">Nombre de la Conexion</param>
		/// <returns>Objeto Dinamico con los parametros de la conexion</returns>
		public static dynamic OConexion(string conectionStringName) {
			if (conectionStringName is null) {
				return null;
			}
			dynamic cc = new ExpandoObject();
			var cs = ConfigurationManager.ConnectionStrings[conectionStringName ?? ""];
			if (cs != null) {
				string sc = cs.ToString();
				var ad = sc.Split(';');
				foreach (string x in ad) {
					var cv = x.Split('=');
					((IDictionary<String, Object>)cc).Add(cv[0], cv[1]);
				}
				return cc;
			}
			else {
				return null;
			}
		}
	}
	public class RespuestaQuery {
		public bool Valid { get; set; }
		public string Error { get; set; }
		public int Afectados { get; set; }
		public dynamic Row { get; set; }
		public string Consulta { get; set; }
		public List<dynamic> Parametros = new List<dynamic>();
		public List<dynamic> Rows = new List<dynamic>();
		public int IdRegistro { get; set; }
		[JsonConstructor]
		public RespuestaQuery(bool valid = false, string error = "", int filas = 0, string consulta = "") {
			Valid = valid;
			Error = error;
			Afectados = filas;
			Consulta = consulta;
		}
		public string GetJSON() {
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}