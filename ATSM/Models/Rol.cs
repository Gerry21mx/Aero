using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using WebMatrix.WebData;

namespace ATSM {
	public class Rol {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int RoleId { get; private set; }
		public string RoleName { get; set; }
		public string Descripcion { get; set; }
		public int Padre { get; set; }
		public string Icon { get; set; }
		public string Area { get; set; }
		public int? Orden { get; set; }
		public bool Valid { get; set; }
		public Rol(int roleId) {
			Inicializar();
			if (roleId > 0) {
				string query = $"SELECT * FROM webpages_Roles WHERE RoleId=@rid";
				SqlCommand Cmnd = new SqlCommand(query, DataBase.Conexion());
				Cmnd.Parameters.Add(new SqlParameter("@rid", roleId));
				SetDatos(Cmnd);
			}
		}
		public Rol(string roleName) {
			Inicializar();
			if (!string.IsNullOrEmpty(roleName)) {
				string query = $"SELECT * FROM webpages_Roles WHERE RoleName=@rnm";
				SqlCommand Cmnd = new SqlCommand(query, DataBase.Conexion());
				Cmnd.Parameters.Add(new SqlParameter("@rnm", roleName));
				SetDatos(Cmnd);
			}
		}
		[JsonConstructor]
		public Rol(int roleId = 0, string roleName = "", string descripcion = null, int padre = 0, string icon = null, int? orden = null, string area = null, bool valid = false) {
			RoleId = roleId;
			RoleName = roleName;
			Descripcion = descripcion;
			Padre = padre;
			Icon = icon;
			Orden = orden;
			Area = area;
			Valid = valid;
		}
		public Respuesta Save() {
			Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
			if (!string.IsNullOrEmpty(RoleName) && !string.IsNullOrEmpty(Descripcion)) {
				res.Error = "";
				SqlCommand Cmnd = new SqlCommand($"SELECT RoleId FROM webpages_Roles WHERE RoleId = @rid OR RoleName = @nom", DataBase.Conexion());
				Cmnd.Parameters.Add(new SqlParameter("@rid", RoleId));
				Cmnd.Parameters.Add(new SqlParameter("@nom", RoleName));
				var existe = DataBase.Query(Cmnd);
				res.Mensaje = "Rol ";
				string SqlStr = "";
				bool Insr = false;
				if (existe.Valid) {
					RoleId = int.Parse(existe.Row.RoleId.ToString());
					SqlStr = @"UPDATE webpages_Roles SET RoleName = @nom, Descripcion = @des, Padre = @pad, Icon = @ico, Orden = @orden, Area = @area WHERE RoleId=@rid";
					res.Mensaje += "Actualizado Correctamente";
				}
				else {
					if (!string.IsNullOrEmpty(existe.Error)) {
						res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
						return res;
					}
					SqlStr = @"INSERT INTO webpages_Roles(RoleName, Descripcion, Padre, Icon, Orden, Area) VALUES(@nom, @des, @pad, @ico, @orden, @area)";
					res.Mensaje += "Registrado Correctamente";
					Insr = true;
				}
				SqlCommand Command = new SqlCommand(SqlStr, DataBase.Conexion());
				Command.Parameters.Add(new SqlParameter("@rid", RoleId));
				Command.Parameters.Add(new SqlParameter("@nom", RoleName));
				Command.Parameters.Add(new SqlParameter("@des", Descripcion));
				Command.Parameters.Add(new SqlParameter("@pad", Padre));
				Command.Parameters.Add(new SqlParameter("@ico", Icon));
				Command.Parameters.Add(new SqlParameter("@orden", Orden));
				Command.Parameters.Add(new SqlParameter("@area", Area));
				RespuestaQuery rInUp = DataBase.Insert(Command);
				if (rInUp.Valid) {
					if (Insr) {
						if (rInUp.IdRegistro == 0) {
							res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
							return res;
						}
						RoleId = rInUp.IdRegistro;
						Valid = true;
					}
				}
				else {
					res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br> Error: {rInUp.Error}";
					return res;
				}
				res.Elemento = this;
				res.Valid = true;
			}
			return res;
		}
		/// <summary>
		/// Elimina la entidad de la DB
		/// </summary>
		/// <returns>Valoor logico indicando si la eliminacion fue correcta o no.</returns>
		public Respuesta Delete() {
			Respuesta res = new Respuesta("El Rol NO se ha Eliminado");
			SqlCommand Command = new SqlCommand("DELETE webpages_Roles WHERE RoleId=@rid", DataBase.Conexion());
			Command.Parameters.Add(new SqlParameter("@rid", RoleId));
			var resD = DataBase.Execute(Command);
			if (resD.Valid && resD.Afectados > 0) {
				res.Valid = true;
				res.Error = "";
				res.Mensaje = "Rol Eliminado";
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
				RoleId = Registro.RoleId;
				RoleName = Registro.RoleName;
				Descripcion = Registro.Descripcion;
				Padre = Registro.Padre;
				Icon = Registro.Icon;
				Area = Registro.Area;
				Valid = true;
			}
		}
		private void Inicializar() {
			RoleId = 0;
			RoleName = "";
			Descripcion = "";
			Padre = 0;
			Icon = "";
			Area = null;
			Valid = false;
		}
		public static List<Rol> GetRoles(string area = null) {
			List<Rol> Roles = new List<Rol>();
			int uid = WebSecurity.CurrentUserId;
			string rolesUsuario = "";
			if (uid > 1) {
				rolesUsuario = "AND RoleId IN (SELECT RoleId FROM webpages_UsersInRoles WHERE UserId = @uid)";
			}
			SqlCommand comando = new SqlCommand($"SELECT * FROM webpages_Roles WHERE {(!string.IsNullOrEmpty(area)?"Area = 'ATSM' OR Area = @area AND ":"")}Orden>=0 {rolesUsuario} ORDER BY Padre,Orden", Conexion);
			comando.Parameters.Add(new SqlParameter("@area", string.IsNullOrEmpty(area) ? SqlString.Null : area));
			comando.Parameters.Add(new SqlParameter("@uid", uid));
			RespuestaQuery res = DataBase.Query(comando);
			foreach (var rPer in res.Rows) {
				Rol per = JsonConvert.DeserializeObject<Rol>(JsonConvert.SerializeObject(rPer));
				per.Valid = true;
				Roles.Add(per);
			}
			return Roles;
		}
	}
}