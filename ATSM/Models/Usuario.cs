using ATSM.Cuentas;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web.Security;

using WebMatrix.WebData;

namespace ATSM {
	/// <summary>
	/// Clase de Usuario del Sistema
	/// DB: GTSM
	/// Tabla: Usuario
	/// </summary>
	public class Usuario {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int IdUsuario { get; private set; }
		public string Nickname { get; set; }
		public string Nombre { get; set; }
        public string Password { get; set; }
        public int IdPerfil { get; set; }
		public int? IdEmpresa { get; set; }
		public string Correo { get; set; }
		public string Telefono { get; set; }
		public bool Activo { get; set; }
		public string IP { get; set; }
		public DateTime? LastLog { get; set; }
        public string Sistema { get; set; }
        [JsonIgnore]
        public Perfil Perfil { get; private set; }
        [JsonIgnore]
		public Empresa Empresa { get; set; }
		public bool Valid { get; private set; }
		public Usuario(int? usuarioId = null) {
			Inicializar();
			if (usuarioId > 0) {
				string query = $"SELECT * FROM Usuario WHERE IdUsuario=@usuId";
				SqlCommand Cmnd = new SqlCommand(query, DataBase.Conexion());
				Cmnd.Parameters.Add(new SqlParameter("@usuId", usuarioId));
				SetDatos(Cmnd);
			}
		}
		public Usuario(string nickname) {
			Inicializar();
			if (!string.IsNullOrEmpty(nickname)) {
				string query = $"SELECT * FROM Usuario WHERE Nickname=@nickname";
				SqlCommand Cmnd = new SqlCommand(query, DataBase.Conexion());
				Cmnd.Parameters.Add(new SqlParameter("@nickname", nickname));
				SetDatos(Cmnd);
			}
		}
        [JsonConstructor]
        public Usuario(int idUsuario, string nickname, string nombre = "", int idPerfil = 0, int? idEmpresa = null, string correo = "", string telefono = "", bool activo = false, string iP = "", string lastLog = null, string sistema = null, bool valid = false) {
            IdUsuario = idUsuario;
            Nickname = nickname;
            Nombre = nombre;
            IdPerfil = idPerfil;
            IdEmpresa = idEmpresa;
            Correo = correo;
            Telefono = telefono;
            Activo = activo;
            IP = iP;
            LastLog = string.IsNullOrEmpty(lastLog) ? null : Funciones.Str2Fec(lastLog);
            Sistema = sistema;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nickname) && !string.IsNullOrEmpty(Correo)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT IdUsuario FROM Usuario WHERE IdUsuario = @idusuario", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idusuario", IdUsuario));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Usuario ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Usuario SET Nickname = @nickname, Nombre = @nombre, IdPerfil = @idperfil, IdEmpresa = @idempresa, Correo = @correo, Telefono = @telefono, Activo = @activo, IP = @ip, LastLog = @lastlog, Sistema = @sistema WHERE IdUsuario=@idusuario";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    if (string.IsNullOrEmpty(Password)) {
                        res.Mensaje = $"El password no puede quedar vacio en usuarios Nuevos. (CS.{this.GetType().Name}-Save.Err.02)";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Usuario(Nickname, Nombre, IdPerfil, IdEmpresa, Correo, Telefono, Activo, IP, LastLog, Sistema) VALUES(@nickname, @nombre, @idperfil, @idempresa, @correo, @telefono, @activo, @ip, @lastlog, @sistema)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idusuario", IdUsuario));
                Command.Parameters.Add(new SqlParameter("@nickname", Nickname));
                Command.Parameters.Add(new SqlParameter("@nombre", Nombre));
                Command.Parameters.Add(new SqlParameter("@idperfil", IdPerfil));
                Command.Parameters.Add(new SqlParameter("@idempresa", IdEmpresa ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@correo", Correo));
                Command.Parameters.Add(new SqlParameter("@telefono", string.IsNullOrEmpty(Telefono) ? SqlString.Null : Telefono));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                Command.Parameters.Add(new SqlParameter("@ip", string.IsNullOrEmpty(IP) ? SqlString.Null : IP));
                Command.Parameters.Add(new SqlParameter("@lastlog", LastLog ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@sistema", string.IsNullOrEmpty(Sistema) ? SqlString.Null : Sistema));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid) {
                    if (Insr) {
                        if (rInUp.IdRegistro == 0) {
                            res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.04)<br> Error: {rInUp.Error}";
                            return res;
                        }
                        WebSecurity.CreateAccount(Nickname, Password);
                        IdUsuario = rInUp.IdRegistro;
                        Valid = true;
                        res.Mensaje += $"<br><h3>Id: {rInUp.IdRegistro}</h3>";
                    }
                    else {
                        if (!string.IsNullOrEmpty(Password)) {
                            string token = WebSecurity.GeneratePasswordResetToken(Nickname, 2);
                            WebSecurity.ResetPassword(token, Password);
                        }
                    }
                    var rsp = SetPerfil(IdUsuario, IdPerfil);
                    if (!string.IsNullOrEmpty(rsp.Error)) {
                        res.Error = $"Error al Asignar el Perfil. (CS.{this.GetType().Name}-Save.Err.05).<br>{rsp.Error}";
                        return res;
                    }
                }
                else {
                    res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
                    return res;
                }
                GetPerfil();
                res.Elemento = this;
                res.Valid = true;
            }
            else {
                if(string.IsNullOrEmpty(Nickname))
                    res.Error += "<br> Falta Usuario";
                if (string.IsNullOrEmpty(Correo))
                    res.Error += "<br> Falta Correo";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Usuario NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Usuario WHERE IdUsuario = @id", Conexion);
            Command.Parameters.Add(new SqlParameter("@id", IdUsuario));
            var resD = DataBase.Execute(Command);
            if (resD.Valid && resD.Afectados > 0) {
                res.Valid = true;
                res.Error = "";
                res.Mensaje = "Eliminado Correctamente";
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
            RespuestaQuery res = DataBase.Query(Command);
            if (res.Valid) {
                var Registro = res.Row;
                IdUsuario = Registro.IdUsuario;
                Nickname = Registro.Nickname;
                Nombre = Registro.Nombre;
                IdPerfil = Registro.IdPerfil;
                IdEmpresa = Registro.IdEmpresa;
                Correo = Registro.Correo;
                Telefono = Registro.Telefono;
                Activo = Registro.Activo;
                IP = Registro.IP;
                LastLog = Registro.LastLog;
                Sistema = Registro.Sistema;
                Valid = true;
                GetPerfil();
                GetEmpresa();
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdUsuario = 0;
            Nickname = "";
            Nombre = "";
            IdPerfil = 0;
            IdEmpresa = null;
            Correo = "";
            Telefono = null;
            Activo = false;
            IP = null;
            LastLog = null;
            Sistema = null;
            Valid = false;
            Perfil = new Perfil();
            Empresa = new Empresa();
        }
        public static List<Usuario> GetUsuarios(string area = null) {
            List<Usuario> usuarios = new List<Usuario>();
            string sqlstr = string.IsNullOrEmpty(area) ? "SELECT * FROM Usuario ORDER BY Nombre" : $"SELECT * FROM Usuario WHERE IdUsuario IN (SELECT DISTINCT UserId FROM webpages_UsersInRoles WHERE RoleId IN (SELECT RoleId FROM webpages_Roles WHERE Area = @area)) ORDER BY Nombre";
            SqlCommand comando = new SqlCommand(sqlstr, Conexion);
            comando.Parameters.Add(new SqlParameter("@area", string.IsNullOrEmpty(area) ? SqlString.Null : area));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                Usuario usuario = JsonConvert.DeserializeObject<Usuario>(JsonConvert.SerializeObject(reg));
                usuario.Valid = true;
                usuarios.Add(usuario);
            }
            return usuarios;
        }
        public bool InRole(string rolName) {
			bool res = false;
			if (!string.IsNullOrEmpty(rolName)) {
				SqlCommand Comando = new SqlCommand("SELECT webpages_UsersInRoles.*,webpages_Roles.RoleName FROM webpages_UsersInRoles LEFT JOIN webpages_Roles ON webpages_UsersInRoles.RoleId=webpages_Roles.RoleId WHERE IdUsuario=@uid AND RoleName=@rnm", DataBase.Conexion());
				Comando.Parameters.AddWithValue("@uid", IdUsuario);
				Comando.Parameters.AddWithValue("@rnm", rolName);
				var resC = DataBase.Query(Comando);
				if (resC.Valid) {
					if (resC.Row.IdUsuario == IdUsuario && resC.Row.RoleName == rolName) {
						res = true;
					}
				}
			}
			return res;
		}
		public void SetLastLogin(string ip) {
			if (!string.IsNullOrEmpty(ip)) {
				SqlCommand comando = new SqlCommand("UPDATE Usuario SET IP=@ip, LastLog=DATETIME() WHERE IdUsuario=@uid", DataBase.Conexion());
				comando.Parameters.AddWithValue("@uid", IdUsuario);
				comando.Parameters.AddWithValue("@ip", ip);
				var res = DataBase.Execute(comando);
			}
		}
        public Perfil GetPerfil() {
            Perfil = new Perfil(IdPerfil);
            return Perfil;
		}
		public Empresa GetEmpresa() {
			Empresa = new Empresa(IdEmpresa);
			return Empresa;
		}
        public List<Account> GetAccounts()
		{
            List<Account> cuentas = new List<Account>();
            SqlCommand comando = new SqlCommand("SELECT * FROM Account WHERE IdUsuario = @idusuario ORDER BY IdTipo", Conexion);
            comando.Parameters.Add(new SqlParameter("@idusuario", IdUsuario));
            var res = DataBase.Query(comando);
            foreach(var reg in res.Rows)
			{
                Account cta = JsonConvert.DeserializeObject<Account>(JsonConvert.SerializeObject(reg));
                cta.GetTipo();
                cta.Valid = true;
                cuentas.Add(cta);
            }
            return cuentas;
		}
		public static Respuesta SetPerfil(int idUsuario, int idPerfil) {
			Respuesta res = new Respuesta($"No se Pudo Asignar el Perfil u.{idUsuario}-p.{idPerfil}");
			Usuario usu = new Usuario(idUsuario);
			Perfil per = new Perfil(idPerfil);
			if (per.Valid && usu.Valid) {
				res.Error = "";
				SqlCommand Comando = new SqlCommand("DELETE webpages_UsersInRoles WHERE UserId=@uid", DataBase.Conexion());
				Comando.Parameters.AddWithValue("@uid", usu.IdUsuario);
				var rq = DataBase.Execute(Comando);
				if (!string.IsNullOrEmpty(rq.Error)) {
					res.Error = $"Error al eliminar el perfil del usuario:<br>{rq.Error}";
					return res;
				}
				foreach (var rol in per.Roles) {
					Comando = new SqlCommand("INSERT INTO webpages_UsersInRoles VALUES(@uid, @rid)", DataBase.Conexion());
					Comando.Parameters.AddWithValue("@uid", usu.IdUsuario);
					Comando.Parameters.AddWithValue("@rid", rol.RoleId);
					rq = DataBase.Execute(Comando);
					if (!string.IsNullOrEmpty(rq.Error)) {
						res.Error = $"Error al crear el perfil del usuario:<br>{rq.Error}";
						return res;
					}
				}
			}
			if (idPerfil == 0) {
                res.Valid = true;
                res.Error = "";
			}
			return res;
		}
        public static bool CurrentInRole(string rolename) {
            var srp = new SimpleRoleProvider(Roles.Provider);
            return srp.IsUserInRole(WebSecurity.CurrentUserName, rolename);
        }
	}
}