using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

using WebMatrix.WebData;

namespace ATSM.Operaciones {
	public class Cliente {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Nombre { get; set; }
		public string RazonSocial { get; set; }
		public string Domicilio { get; set; }
		public string CP { get; set; }
		public string Estado { get; set; }
		public string Ciudad { get; set; }
		public string Parque { get; set; }
		public string Grupo { get; set; }
		public string WebSite { get; set; }
		public string Giro { get; set; }
		public string RFC { get; set; }
		public string Conmutador { get; set; }
        private int _idTipo { get; set; }
		public int IdTipo {
			get { return _idTipo; }
            set {
                Tipo = new TipoCliente(value);
                _idTipo = Tipo.Id;
            } 
        }
		public TipoCliente Tipo { get; set; }
		public bool Activo { get; set; }
		public int Usuario { get; set; }
		public bool Valid { get; set; }
		public List<ClienteContacto> Contactos { get; set; }
		public List<ClienteActividad> Registro { get; set; }
		public Cliente(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Cliente WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Cliente(string cadena) {
            Inicializar();
            if (!string.IsNullOrEmpty(cadena)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Cliente WHERE Nombre = @cadena", Conexion);
                comando.Parameters.Add(new SqlParameter("@cadena", cadena));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Cliente(int id, string nombre, string razonSocial, string domicilio = null, string cP = null, string estado = null, string ciudad = null, string parque = null, string grupo = null, string webSite = null, string giro = null, string rFC = null, string conmutador = null, int idtipo = 0, bool activo = false, int usuario = 0, bool valid = false) {
            Id = id;
            Nombre = nombre;
            RazonSocial = razonSocial;
            Domicilio = domicilio;
            CP = cP;
            Estado = estado;
            Ciudad = ciudad;
            Parque = parque;
            Grupo = grupo;
            WebSite = webSite;
            Giro = giro;
            RFC = rFC;
            Conmutador = conmutador;
            IdTipo = idtipo;
            Activo = activo;
            Usuario = usuario;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nombre)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Cliente WHERE Id = @id OR Nombre = @nombre", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@nombre", Nombre));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Cliente ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Cliente SET Nombre = @nombre, RazonSocial = @razonsocial, Domicilio = @domicilio, CP = @cp, Estado = @estado, Ciudad = @ciudad, Parque = @parque, Grupo = @grupo, WebSite = @website, Giro = @giro, RFC = @rfc, Conmutador = @conmutador, IdTipo = @idtipo, Activo = @activo, Usuario = @usuario WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Cliente(Nombre, RazonSocial, Domicilio, CP, Estado, Ciudad, Parque, Grupo, WebSite, Giro, RFC, Conmutador, IdTipo, Activo, Usuario) VALUES(@nombre, @razonsocial, @domicilio, @cp, @estado, @ciudad, @parque, @grupo, @website, @giro, @rfc, @conmutador, @idtipo, @activo, @usuario)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@nombre", Nombre));
                Command.Parameters.Add(new SqlParameter("@razonsocial", string.IsNullOrEmpty(RazonSocial) ? SqlString.Null : RazonSocial));
                Command.Parameters.Add(new SqlParameter("@domicilio", string.IsNullOrEmpty(Domicilio) ? SqlString.Null : Domicilio));
                Command.Parameters.Add(new SqlParameter("@cp", string.IsNullOrEmpty(CP) ? SqlString.Null : CP));
                Command.Parameters.Add(new SqlParameter("@estado", string.IsNullOrEmpty(Estado) ? SqlString.Null : Estado));
                Command.Parameters.Add(new SqlParameter("@ciudad", string.IsNullOrEmpty(Ciudad) ? SqlString.Null : Ciudad));
                Command.Parameters.Add(new SqlParameter("@parque", string.IsNullOrEmpty(Parque) ? SqlString.Null : Parque));
                Command.Parameters.Add(new SqlParameter("@grupo", string.IsNullOrEmpty(Grupo) ? SqlString.Null : Grupo));
                Command.Parameters.Add(new SqlParameter("@website", string.IsNullOrEmpty(WebSite) ? SqlString.Null : WebSite));
                Command.Parameters.Add(new SqlParameter("@giro", string.IsNullOrEmpty(Giro) ? SqlString.Null : Giro));
                Command.Parameters.Add(new SqlParameter("@rfc", string.IsNullOrEmpty(RFC) ? SqlString.Null : RFC));
                Command.Parameters.Add(new SqlParameter("@conmutador", string.IsNullOrEmpty(Conmutador) ? SqlString.Null : Conmutador));
                Command.Parameters.Add(new SqlParameter("@idtipo", IdTipo));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                Command.Parameters.Add(new SqlParameter("@usuario", WebSecurity.CurrentUserId));
                using (TransactionScope scope = new TransactionScope()) {
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
                        else {
                            var contacts = ClienteContacto.GetClienteContactos(Id);
                            foreach(var con in contacts) {
                                var idx = Contactos.FindIndex(c => { return c.Id == con.Id; });
								if (idx == -1)
                                    con.Delete();
							}
                            var activity = ClienteActividad.GetClienteActividads(Id);
                            foreach(var act in activity) {
                                var idx = Registro.FindIndex(a => { return a.Id == act.Id; });
								if (idx == -1)
                                    act.Delete();
							}
                        }
                        foreach(var con in Contactos) {
                            con.IdCliente = Id;
                            var rSv = con.Save();
                            if(!rSv.Valid || !string.IsNullOrEmpty(rSv.Error)) {
                                res.Error = $"Error al Registrar el Contacto: ${con.Nombre}(CS.{this.GetType().Name}-Save.Err.04)<br>Error: {rSv.Error}";
                                return res;
							}
						}
                        foreach(var act in Registro) {
                            act.IdCliente = Id;
                            var rSv = act.Save();
                            if(!rSv.Valid || !string.IsNullOrEmpty(rSv.Error)) {
                                res.Error = $"Error al Registrar la Actividad del dia ${act.Fecha.ToString("dd MM yyyy")}(CS.{this.GetType().Name}-Save.Err.05)<br>Error: {rSv.Error}";
                                return res;
							}
						}
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                        return res;
                    }
                    SetContactos();
                    SetRegistro();
                    res.Elemento = this;
                    res.Valid = true;
                    scope.Complete();
				}
            }
            else {
                if (string.IsNullOrEmpty(Nombre))
                    res.Error += $"<br>Falta el Nombre del Cliente";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Cliente NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Cliente WHERE Id = @id", Conexion);
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
                Nombre = Registro.Nombre;
                RazonSocial = Registro.RazonSocial;
                Domicilio = Registro.Domicilio;
                CP = Registro.CP;
                Estado = Registro.Estado;
                Ciudad = Registro.Ciudad;
                Parque = Registro.Parque;
                Grupo = Registro.Grupo;
                WebSite = Registro.WebSite;
                Giro = Registro.Giro;
                RFC = Registro.RFC;
                Conmutador = Registro.Conmutador;
                IdTipo = Registro.IdTipo;
                Activo = Registro.Activo;
                Usuario = Registro.Usuario;
                Valid = true;
                SetContactos();
                SetRegistro();
            }
        }
        private void Inicializar() {
            Id = 0;
            Nombre = "";
            RazonSocial = "";
            Domicilio = "";
            CP = "";
            Estado = "";
            Ciudad = "";
            Parque = "";
            Grupo = "";
            WebSite = "";
            Giro = "";
            RFC = "";
            Conmutador = "";
            IdTipo = 0;
            Activo = false;
            Usuario = 0;
            Valid = false;
            Contactos = new List<ClienteContacto>();
            Registro = new List<ClienteActividad>();
        }
        public static List<Cliente> GetClientes() {
            List<Cliente> clientes = new List<Cliente>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Cliente", Conexion));
            foreach (var reg in res.Rows) {
                Cliente cliente = JsonConvert.DeserializeObject<Cliente>(JsonConvert.SerializeObject(reg));
                cliente.Valid = true;
                clientes.Add(cliente);
            }
            return clientes;
        }
        public void SetContactos() {
            Contactos = ClienteContacto.GetClienteContactos(Id);
		}
        public void SetRegistro() {
            Registro = ClienteActividad.GetClienteActividads(Id);
		}
    }
}