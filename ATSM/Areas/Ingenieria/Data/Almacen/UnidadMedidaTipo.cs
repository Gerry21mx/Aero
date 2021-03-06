using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

using WebMatrix.WebData;

namespace ATSM.Almacen {
	public class UnidadMedidaTipo {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; private set; }
		public string Codigo { get; set; }
		public string Nombre { get; set; }
		public int Usuario { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; private set; }
        public UnidadMedidaTipo(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM UnidadMedidaTipo WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public UnidadMedidaTipo(string cadena) {
            Inicializar();
            if (!string.IsNullOrEmpty(cadena)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM UnidadMedidaTipo WHERE Codigo = @cadena OR Nombre = @cadena", Conexion);
                comando.Parameters.Add(new SqlParameter("@cadena", cadena));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public UnidadMedidaTipo(int id, string codigo, string nombre, int usuario = 0, bool activo = false, bool valid = false) {
            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            Usuario = usuario;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Codigo) && !string.IsNullOrEmpty(Nombre)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM UnidadMedidaTipo WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "UnidadMedidaTipo ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE UnidadMedidaTipo SET Codigo = @codigo, Nombre = @nombre, Usuario = @usuario, Activo = @activo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO UnidadMedidaTipo(Codigo, Nombre, Usuario, Activo) VALUES(@codigo, @nombre, @usuario, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@codigo", string.IsNullOrEmpty(Codigo) ? SqlString.Null : Codigo));
                Command.Parameters.Add(new SqlParameter("@nombre", string.IsNullOrEmpty(Nombre) ? SqlString.Null : Nombre));
                Command.Parameters.Add(new SqlParameter("@usuario", WebSecurity.CurrentUserId));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
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
                if (!string.IsNullOrEmpty(Codigo))
                    res.Error += $"<br>Falta el Codigo del Tipo";
                if (!string.IsNullOrEmpty(Nombre))
                    res.Error += $"<br>Falta el Nombre del Tipo";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("UnidadMedidaTipo NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE UnidadMedidaTipo WHERE Id = @id", Conexion);
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
                Usuario = Registro.Usuario;
                Activo = Registro.Activo;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            Codigo = "";
            Nombre = "";
            Usuario = 0;
            Activo = false;
            Valid = false;
        }
        public static List<UnidadMedidaTipo> GetUnidadMedidaTipos() {
            List<UnidadMedidaTipo> unidadmedidatipos = new List<UnidadMedidaTipo>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM UnidadMedidaTipo", Conexion));
            foreach (var reg in res.Rows) {
                UnidadMedidaTipo unidadmedidatipo = JsonConvert.DeserializeObject<UnidadMedidaTipo>(JsonConvert.SerializeObject(reg));
                unidadmedidatipo.Valid = true;
                unidadmedidatipos.Add(unidadmedidatipo);
            }
            return unidadmedidatipos;
        }
    }
}