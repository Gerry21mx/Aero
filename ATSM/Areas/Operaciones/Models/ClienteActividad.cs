using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

using WebMatrix.WebData;

namespace ATSM.Operaciones {
	public class ClienteActividad {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdCliente { get; set; }
		public DateTime Fecha { get; set; }
		public string Tipo { get; set; }
		public string Comentarios { get; set; }
        public int Usuario { get; set; }
        public bool Valid { get; set; }
        public ClienteActividad(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ClienteActividad WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public ClienteActividad(int id = 0, int idCliente = 0, DateTime fecha = default(DateTime), string tipo = "", string comentarios = "", int usuario = 0, bool valid = false) {
            Id = id;
            IdCliente = idCliente;
            Fecha = fecha;
            Tipo = tipo;
            Comentarios = comentarios;
            Usuario = usuario;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Comentarios) && !string.IsNullOrEmpty(Tipo)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM ClienteActividad WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "ClienteActividad ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE ClienteActividad SET IdCliente = @idcliente, Fecha = @fecha, Tipo = @tipo, Comentarios = @comentarios, Usuario = @usuario WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO ClienteActividad(IdCliente, Fecha, Tipo, Comentarios, Usuario) VALUES(@idcliente, @fecha, @tipo, @comentarios, @usuario)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idcliente", IdCliente));
                Command.Parameters.Add(new SqlParameter("@fecha", Fecha));
                Command.Parameters.Add(new SqlParameter("@tipo", Tipo));
                Command.Parameters.Add(new SqlParameter("@comentarios", string.IsNullOrEmpty(Comentarios) ? SqlString.Null : Comentarios));
                Command.Parameters.Add(new SqlParameter("@usuario", WebSecurity.CurrentUserId));
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
                if (string.IsNullOrEmpty(Comentarios))
                    res.Error += $"<br>Las Comentarios no pueden quedar Vacias.";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("ClienteActividad NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE ClienteActividad WHERE Id = @id", Conexion);
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
                IdCliente = Registro.IdCliente;
                Fecha = Registro.Fecha;
                Tipo = Registro.Tipo;
                Comentarios = Registro.Comentarios;
                Usuario = Registro.Usuario;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            IdCliente = 0;
            Fecha = default(DateTime);
            Tipo = "";
            Comentarios = "";
            Usuario = 0;
            Valid = false;
        }
        public static List<ClienteActividad> GetClienteActividads(int idcliente) {
            List<ClienteActividad> clienteactividads = new List<ClienteActividad>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ClienteActividad WHERE IdCliente = @idcliente", Conexion);
            comando.Parameters.AddWithValue("@idcliente", idcliente);
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ClienteActividad clienteactividad = JsonConvert.DeserializeObject<ClienteActividad>(JsonConvert.SerializeObject(reg));
                clienteactividad.Valid = true;
                clienteactividads.Add(clienteactividad);
            }
            return clienteactividads;
        }
    }
}