using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

using WebMatrix.WebData;

namespace ATSM.Operaciones {
	public class ClienteContacto {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdCliente { get; set; }
		public string Nombre { get; set; }
		public string Puesto { get; set; }
		public string Telefono { get; set; }
		public string Extension { get; set; }
		public string Celular { get; set; }
		public string Otro { get; set; }
		public string Correo { get; set; }
		public string Observaciones { get; set; }
		public int Usuario { get; set; }
        public bool Valid { get; set; }
        public ClienteContacto(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ClienteContacto WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public ClienteContacto(int idcliente, string nombre) {
            Inicializar();
            if (idcliente > 0 && !string.IsNullOrEmpty(nombre)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ClienteContacto WHERE IdCliente = @idcliente AND Nombre = @nombre", Conexion);
                comando.Parameters.Add(new SqlParameter("@idcliente", idcliente));
                comando.Parameters.Add(new SqlParameter("@nombre", nombre));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public ClienteContacto(int id, int idcliente, string nombre, string puesto, string telefono = "", string extension = "", string celular = "", string otro = "", string correo = "", string observaciones = "", int usuario = 0, bool valid = false) {
            Id = id;
            IdCliente = idcliente;
            Nombre = nombre;
            Puesto = puesto;
            Telefono = telefono;
            Extension = extension;
            Celular = celular;
            Otro = otro;
            Correo = correo;
            Observaciones = observaciones;
            Usuario = usuario;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nombre) && !string.IsNullOrEmpty(Puesto) && !string.IsNullOrEmpty(Telefono)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM ClienteContacto WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "ClienteContacto ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE ClienteContacto SET IdCliente = @idcliente, Nombre = @nombre, Puesto = @puesto, Telefono = @telefono, Extension = @extension, Celular = @celular, Otro = @otro, Correo = @correo, Observaciones = @observaciones, Usuario = @usuario WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO ClienteContacto(IdCliente, Nombre, Puesto, Telefono, Extension, Celular, Otro, Correo, Observaciones, Usuario) VALUES(@idcliente, @nombre, @puesto, @telefono, @extension, @celular, @otro, @correo, @observaciones, @usuario)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idcliente", IdCliente));
                Command.Parameters.Add(new SqlParameter("@nombre", Nombre));
                Command.Parameters.Add(new SqlParameter("@puesto", Puesto));
                Command.Parameters.Add(new SqlParameter("@telefono", string.IsNullOrEmpty(Telefono) ? SqlString.Null : Telefono));
                Command.Parameters.Add(new SqlParameter("@extension", string.IsNullOrEmpty(Extension) ? SqlString.Null : Extension));
                Command.Parameters.Add(new SqlParameter("@celular", string.IsNullOrEmpty(Celular) ? SqlString.Null : Celular));
                Command.Parameters.Add(new SqlParameter("@otro", string.IsNullOrEmpty(Otro) ? SqlString.Null : Otro));
                Command.Parameters.Add(new SqlParameter("@correo", string.IsNullOrEmpty(Correo) ? SqlString.Null : Correo));
                Command.Parameters.Add(new SqlParameter("@observaciones", string.IsNullOrEmpty(Observaciones) ? SqlString.Null : Observaciones));
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
                if (string.IsNullOrEmpty(Nombre))
                    res.Error += $"<br>Falta el Nombre del Contacto";
                if (string.IsNullOrEmpty(Puesto))
                    res.Error += $"<br>Falta el Puesto del Contacto";
                if (string.IsNullOrEmpty(Telefono))
                    res.Error += $"<br>Debe al menos tener el Telefono del Contacto.";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("ClienteContacto NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE ClienteContacto WHERE Id = @id", Conexion);
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
                Nombre = Registro.Nombre;
                Puesto = Registro.Puesto;
                Telefono = Registro.Telefono;
                Extension = Registro.Extension;
                Celular = Registro.Celular;
                Otro = Registro.Otro;
                Correo = Registro.Correo;
                Observaciones = Registro.Observaciones;
                Usuario = Registro.Usuario;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            IdCliente = 0;
            Nombre = "";
            Puesto = "";
            Telefono = null;
            Extension = null;
            Celular = null;
            Otro = null;
            Correo = null;
            Observaciones = null;
            Usuario = 0;
            Valid = false;
        }
        public static List<ClienteContacto> GetClienteContactos(int idcliente) {
            List<ClienteContacto> clientecontactos = new List<ClienteContacto>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ClienteContacto WHERE IdCliente = @idcliente", Conexion);
            comando.Parameters.AddWithValue("@idcliente", idcliente);
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ClienteContacto clientecontacto = JsonConvert.DeserializeObject<ClienteContacto>(JsonConvert.SerializeObject(reg));
                clientecontacto.Valid = true;
                clientecontactos.Add(clientecontacto);
            }
            return clientecontactos;
        }
    }
}