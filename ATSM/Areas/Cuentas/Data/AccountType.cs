using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace ATSM.Cuentas {
	public class AccountType {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Codigo { get; set; }
		public string Nombre { get; set; }
		public string RoleName { get; set; }
		public bool Valid { get; set; }
        public AccountType(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM AccountType WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public AccountType(string cadena) {
            Inicializar();
            if (!string.IsNullOrEmpty(cadena)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM AccountType WHERE Codigo = @cadena OR Nombre = @cadena", Conexion);
                comando.Parameters.Add(new SqlParameter("@cadena", cadena));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public AccountType(int id, string codigo, string nombre, string rolename, bool valid = false) {
            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            RoleName = rolename;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Codigo) && !string.IsNullOrEmpty(Nombre)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM AccountType WHERE Id = @id OR Codigo = @codigo OR Nombre = @nombre", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@codigo", Codigo));
                Cmnd.Parameters.Add(new SqlParameter("@nombre", Nombre));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "AccountType ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE AccountType SET Codigo = @codigo, Nombre = @nombre, RoleNombre = @rolename WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO AccountType(Codigo, Nombre, RoleName) VALUES(@codigo, @nombre, @rolename)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@codigo", string.IsNullOrEmpty(Codigo) ? SqlString.Null : Codigo));
                Command.Parameters.Add(new SqlParameter("@nombre", string.IsNullOrEmpty(Nombre) ? SqlString.Null : Nombre));
                Command.Parameters.Add(new SqlParameter("@rolename", string.IsNullOrEmpty(RoleName) ? SqlString.Null : RoleName));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid) {
                    if (Insr) {
                        if (rInUp.IdRegistro == 0) {
                            res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br>{SqlStr}<br> Error: {rInUp.Error}";
                            return res;
                        }
                        Id = rInUp.IdRegistro;
                        Valid = true;
                    }
                }
                else {
                    res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                    return res;
                }
                res.Elemento = this;
                res.Valid = true;
            }
			else {
                if (string.IsNullOrEmpty(Codigo))
                    res.Error += "<br>Falta Codigo.";
                if (string.IsNullOrEmpty(Nombre))
                    res.Error += "<br>Falta Nombre.";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("AccountType NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE AccountType WHERE Id = @id", Conexion);
            Command.Parameters.Add(new SqlParameter("@id", Id));
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
                Id = Registro.Id;
                Codigo = Registro.Codigo;
                Nombre = Registro.Nombre;
                RoleName = Registro.RoleName;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            Codigo = "";
            Nombre = "";
            RoleName = "";
            Valid = false;
        }
        public static List<AccountType> GetAccountTypes() {
            List<AccountType> accounttypes = new List<AccountType>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM AccountType", Conexion));
            foreach (var reg in res.Rows) {
                AccountType accounttype = JsonConvert.DeserializeObject<AccountType>(JsonConvert.SerializeObject(reg));
                accounttype.Valid = true;
                accounttypes.Add(accounttype);
            }
            return accounttypes;
        }
    }
}