using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Transactions;
using WebMatrix.WebData;

namespace ATSM {
	public class Perfil {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int IdPerfil { get; private set; }
		public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public string Area { get; set; }
		public bool Activo { get; set; }
		public List<Rol> Roles = new List<Rol>();
		public bool Valid { get; set; }
		public Perfil(int idPerfil) {
			Inicializar();
			if (idPerfil > 0) {
				string query = $"SELECT * FROM Perfil WHERE IdPerfil=@idp";
				SqlCommand Cmnd = new SqlCommand(query, DataBase.Conexion());
				Cmnd.Parameters.Add(new SqlParameter("@idp", idPerfil));
				SetDatos(Cmnd);
			}
		}
		public Perfil(string nombre) {
			Inicializar();
			if (!string.IsNullOrEmpty(nombre)) {
				string query = $"SELECT * FROM Perfil WHERE Nombre=@nom";
				SqlCommand Cmnd = new SqlCommand(query, DataBase.Conexion());
				Cmnd.Parameters.Add(new SqlParameter("@nom", nombre));
				SetDatos(Cmnd);
			}
		}
		[JsonConstructor]
		public Perfil(int idPerfil = 0, string nombre = "", string descripcion = "", string area = "", bool activo = false, bool valid = false) {
			IdPerfil = idPerfil;
			Nombre = nombre;
			Descripcion = descripcion;
			Activo = activo;
			Area = area;
			Valid = valid;
			SqlCommand comando = new SqlCommand("SELECT * FROM PerfRol WHERE IdPerfil=@idp", Conexion);
			comando.Parameters.AddWithValue("@idp", IdPerfil);
			RespuestaQuery resPR = DataBase.Query(comando);
			foreach (var rl in resPR.Rows) {
				Rol rol = new Rol(rl.RoleId);
				Roles.Add(rol);
			}
		}
		public Respuesta Save() {
			Respuesta res = new Respuesta();
			if (!string.IsNullOrEmpty(Nombre) && !string.IsNullOrEmpty(Descripcion)) {
				res.Error = "";
				SqlCommand Cmnd = new SqlCommand($"SELECT IdPerfil FROM Perfil WHERE IdPerfil = @idp OR Nombre = @nom", DataBase.Conexion());
				Cmnd.Parameters.Add(new SqlParameter("@idp", IdPerfil));
				Cmnd.Parameters.Add(new SqlParameter("@nom", Nombre));
				var existe = DataBase.Query(Cmnd);
				res.Mensaje = "Perfil ";
				string SqlStr = "";
				bool Insr = false;
				if (existe.Valid) {
					IdPerfil = int.Parse(existe.Row.IdPerfil.ToString());
					SqlStr = @"UPDATE Perfil SET Nombre = @nom, Descripcion = @des, Area = @area, Activo = @act WHERE IdPerfil=@idp";
					res.Mensaje += "Actualizado Correctamente";
				}
				else {
					if (!string.IsNullOrEmpty(existe.Error)) {
						res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{existe.Error}";
						return res;
					}
					SqlStr = @"INSERT INTO Perfil(Nombre, Descripcion, Area, Activo) VALUES(@nom, @des, @area, @act)";
					res.Mensaje += "Registrado Correctamente";
					Insr = true;
				}
				SqlCommand Command = new SqlCommand(SqlStr, DataBase.Conexion());
				Command.Parameters.Add(new SqlParameter("@idp", IdPerfil));
				Command.Parameters.Add(new SqlParameter("@nom", Nombre));
				Command.Parameters.Add(new SqlParameter("@des", Descripcion));
				Command.Parameters.Add(new SqlParameter("@area", String.IsNullOrEmpty(Area) ? SqlString.Null: Area));
				Command.Parameters.Add(new SqlParameter("@act", Activo));
				using (TransactionScope scope = new TransactionScope()) {
					RespuestaQuery rInUp = DataBase.Insert(Command);
					if (rInUp.Valid) {
						if (Insr) {
							if (rInUp.IdRegistro == 0) {
								res.Error = $"No se pudo obtener el Id Insertado (CS.{this.GetType().Name}-Save.Err.03)<br>Error: {rInUp.Error}";
								return res;
							}
							IdPerfil = rInUp.IdRegistro;
							res.Mensaje += $"<br><h3>Id: {rInUp.IdRegistro}</h3>";
							Valid = true;
						}
						//	Elimina los usuarios que tengan algun rol de este perfil (Si existia previamente el Perfil)
						Command = new SqlCommand("DELETE webpages_UsersInRoles WHERE UserId IN (SELECT IdUsuario FROM Usuario WHERE IdPerfil=@idp)", Conexion);
						Command.Parameters.AddWithValue("@idp", IdPerfil);
						var rP = DataBase.Execute(Command);
						if (!rP.Valid || !string.IsNullOrEmpty(rP.Error)) {
							res.Error = $"Error al Eliminar Users In Rol: (CS.{this.GetType().Name}-Save.Err.04)<br>Error: {rInUp.Error}";
							return res;
						}
						//	Elimina la definicion de roles del Perfil antes de actualizarla (Si existia previamente el Perfil)
						Command = new SqlCommand("DELETE PerfRol WHERE IdPerfil=@idp", Conexion);
						Command.Parameters.AddWithValue("@idp", IdPerfil);
						rP = DataBase.Execute(Command);
						if (!rP.Valid || !string.IsNullOrEmpty(rP.Error)) {
							res.Error = $"Error al Eliminar Roles del Perfil: (CS.{this.GetType().Name}-Save.Err.05)<br>Error: {rInUp.Error}";
							return res;
						}
						//	Inserta los nuevos roles al perfil
						foreach (var rol in Roles) {
							Command = new SqlCommand("SELECT IdPerfil FROM PerfRol WHERE IdPerfil=@idp AND RoleId=@rid", Conexion);
							Command.Parameters.AddWithValue("@idp", IdPerfil);
							Command.Parameters.AddWithValue("@rid", rol.RoleId);
							var exist = DataBase.Query(Command);
							if (!string.IsNullOrEmpty(exist.Error)) {
								res.Error = $"Error al Registrar buscar el rol en el perfil: (CS.{this.GetType().Name}-Save.Err.06)<br>Error: {rInUp.Error}";
								return res;
							}
							if (exist.Afectados == 0) {
								Command = new SqlCommand("INSERT INTO PerfRol(IdPerfil, RoleId) VALUES(@idp, @rid)", Conexion);
								Command.Parameters.AddWithValue("@idp", IdPerfil);
								Command.Parameters.AddWithValue("@rid", rol.RoleId);
								var rPf = DataBase.Execute(Command);
								if (!rPf.Valid) {
									res.Error = $"Error al Registrar Rol del Perfil: (CS.{this.GetType().Name}-Save.Err.07)<br>Error: {rInUp.Error}";
									return res;
								}
							}
						}
						//		Actualiza los Usuarios Existentes con Este Perfil a sus nuevos roles (Si existia previamente el Perfil)
						Command = new SqlCommand("SELECT IdUsuario FROM Usuario WHERE IdPerfil=@idp", Conexion);
						Command.Parameters.AddWithValue("@idp", IdPerfil);
						var uir = DataBase.Query(Command);
						if (!string.IsNullOrEmpty(uir.Error)) {
							res.Error = $"Error al actualizar Perfil de Usuarios: (CS.{this.GetType().Name}-Save.Err.08)<br>Error: {rInUp.Error}";
							return res;
						}
						foreach (var usu in uir.Rows) {
							var rsp = Usuario.SetPerfil(usu.IdUsuario, IdPerfil);
							if (!string.IsNullOrEmpty(rsp.Error)) {
								res.Error = $"Error al Asignar el Perfil. (CS.{this.GetType().Name}-Save.Err.09).<br>{rsp.Error}";
								return res;
							}
						}
					}
					else {
						res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>Error: {rInUp.Error}";
						return res;
					}
					res.Elemento = this;
					res.Valid = true;
					scope.Complete();
				}
			}
			else {
				res.Error = $"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)";
				if (string.IsNullOrEmpty(Nombre)) {
					res.Error += "<br>Falta el Nombre del Perfil.";
				}
				if (string.IsNullOrEmpty(Descripcion)) {
					res.Error += "<br>Falta la Descripcion del Perfil.";
				}
			}
			return res;
		}
		/// <summary>
		/// Elimina la entidad de la DB
		/// </summary>
		/// <returns>Valoor logico indicando si la eliminacion fue correcta o no.</returns>
		public Respuesta Delete() {
			Respuesta res = new Respuesta("El Perfil NO se ha Eliminado");
			SqlCommand Command = new SqlCommand("DELETE Perfil WHERE IdPerfil=@idp", DataBase.Conexion());
			Command.Parameters.Add(new SqlParameter("@idp", IdPerfil));
			var resD = DataBase.Execute(Command);
			if (resD.Valid && resD.Afectados > 0) {
				res.Valid = true;
				res.Error = "";
				res.Mensaje = "Perfil Eliminada";
				//	Elimina los roles en los usuarios que tengan este perfil
				Command = new SqlCommand("DELETE webpages_UsersInRoles WHERE UserId IN (SELECT UserId FROM Usuario WHERE IdPerfil=@idp)", Conexion);
				Command.Parameters.AddWithValue("@idp", IdPerfil);
				var rP = DataBase.Execute(Command);
				//	Actualiza el perfil de los usuarios que tienen este perfil y lo pone en 0
				Command = new SqlCommand("UPDATE Usuario SET IdPerfil=0 WHERE IdPerfil=@idp", Conexion);
				Command.Parameters.AddWithValue("@idp", IdPerfil);
				rP = DataBase.Execute(Command);
				//	Elimina la definicion de los roles del perfil
				Command = new SqlCommand("DELETE PerfRol WHERE IdPerfil=@idp", Conexion);
				Command.Parameters.AddWithValue("@idp", IdPerfil);
				rP = DataBase.Execute(Command);
				Inicializar();
			}
			else {
				if (!string.IsNullOrEmpty(resD.Error)) {
					res.Error = resD.Error;
				}
				else {
					res.Mensaje = "No se encontraron coincidencias para elminar.";
				}
			}
			return res;
		}
		private void SetDatos(SqlCommand Command) {
			var res = DataBase.Query(Command);
			if (res.Valid) {
				var Registro = res.Row;
				IdPerfil = Registro.IdPerfil;
				Nombre = Registro.Nombre;
				Descripcion = Registro.Descripcion;
				Activo = Registro.Activo;
				Area = Registro.Area;
				Valid = true;
				SqlCommand comando = new SqlCommand("SELECT * FROM PerfRol WHERE IdPerfil=@idp", Conexion);
				comando.Parameters.AddWithValue("@idp", IdPerfil);
				RespuestaQuery resPR = DataBase.Query(comando);
				foreach (var rl in resPR.Rows) {
					Rol rol = new Rol(rl.RoleId);
					Roles.Add(rol);
				}
			}
		}
		private void Inicializar() {
			IdPerfil = 0;
			Nombre = "";
			Descripcion = "";
			Area = null;
			Activo = false;
			Valid = false;
		}
		public static List<Perfil> GetPerfiles(string area = null) {
			List<Perfil> perfiles = new List<Perfil>();
			SqlCommand comando = new SqlCommand($"SELECT * FROM Perfil {(!string.IsNullOrEmpty(area) ? "WHERE Area = @area " : "")}ORDER BY Nombre", Conexion);
			comando.Parameters.Add(new SqlParameter("@area", string.IsNullOrEmpty(area) ? SqlString.Null : area));
			RespuestaQuery res = DataBase.Query(comando);
			foreach (var rPer in res.Rows) {
				Perfil per = JsonConvert.DeserializeObject<Perfil>(JsonConvert.SerializeObject(rPer));
				per.Valid = true;
				perfiles.Add(per);
			}
			return perfiles;
		}
	}
}