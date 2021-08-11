using ATSM.Tripulaciones;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ATSM.Ingenieria {
	public class ComponenteMayor {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Codigo { get; set; }
		public string Descripcion { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; set; }
		public List<ComponenteMayorCapacidad> Capacidades { get; set; }
		public List<Limites> Limites { get; set; }
        public ComponenteMayor(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ComponenteMayor WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public ComponenteMayor(string cadena) {
            Inicializar();
            if (!string.IsNullOrEmpty(cadena)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ComponenteMayor WHERE Codigo = @cadena", Conexion);
                comando.Parameters.Add(new SqlParameter("@cadena", cadena));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public ComponenteMayor(int id, string codigo, string descripcion = "", bool activo = false, bool valid = false) {
            Id = id;
            Codigo = codigo;
            Descripcion = descripcion;
            Activo = activo;
            Valid = valid;
            Capacidades = new List<ComponenteMayorCapacidad>();
            Limites = new List<Limites>();
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Codigo) && !string.IsNullOrEmpty(Descripcion)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM ComponenteMayor WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "ComponenteMayor ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE ComponenteMayor SET Codigo = @codigo, Descripcion = @descripcion, Activo = @activo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO ComponenteMayor(Codigo, Descripcion, Activo) VALUES(@codigo, @descripcion, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@codigo", string.IsNullOrEmpty(Codigo) ? SqlString.Null : Codigo));
                Command.Parameters.Add(new SqlParameter("@descripcion", string.IsNullOrEmpty(Descripcion) ? SqlString.Null : Descripcion));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                using(TransactionScope scope = new TransactionScope()) {
                    RespuestaQuery rInUp = DataBase.Insert(Command);
                    if (rInUp.Valid) {
                        if (Insr) {
                            if (rInUp.IdRegistro == 0) {
                                res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br>{SqlStr}<br> Error: {rInUp.Error}";
                                return res;
                            }
                            Id = rInUp.IdRegistro;
                            Valid = true;
                        } else {
                            Cmnd = new SqlCommand("DELETE ComponenteMayorCapacidad WHERE IdComponenteMayor = @icm;DELETE ComponenteMayorLimites WHERE IdComponenteMayor = @icm;", Conexion);
                            Cmnd.Parameters.Add(new SqlParameter("@icm",Id));
                            var rDH = DataBase.Execute(Cmnd);
                            if(!rDH.Valid || !string.IsNullOrEmpty(rDH.Error)) {
                                res.Error = $"Error al limpiar el Historial: (CS.{this.GetType().Name}-Save.Err.03)<br>Error: {rDH.Error}";
                                return res;
						    }
						}
                        foreach(var cap in Capacidades) {
                            cap.IdComponenteMayor = Id;
                            if (cap.Cantidad > 0) {
                                var rSC=cap.Save();
                                if(!rSC.Valid || !string.IsNullOrEmpty(rSC.Error)) {
                                    res.Error = $"Error al Registrar la Capacidad en el Componente Mayor: (CS.{this.GetType().Name}-Save.Err.04)<br>Error: {rSC.Error}";
                                    return res;
                                }
							}
                        }
                        foreach (var lim in Limites) {
                            Cmnd = new SqlCommand("INSERT INTO ComponenteMayorLimites VALUES(@idcm, @idl)", Conexion);
                            Cmnd.Parameters.Add(new SqlParameter("@idcm", Id));
                            Cmnd.Parameters.Add(new SqlParameter("@idl", lim.Id));
                            var rIC = DataBase.Execute(Cmnd);
                            if (!rIC.Valid || !string.IsNullOrEmpty(rIC.Error)) {
                                res.Error = $"Error al Registrar el Limite en el Componente Mayor: (CS.{this.GetType().Name}-Save.Err.05)<br>Error: {rIC.Error}";
                                return res;
                            }
                        }
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                        return res;
                    }
                    res.Elemento = this;
                    res.Valid = true;
                    scope.Complete();
				}
            }
            else {
                if (!string.IsNullOrEmpty(Codigo))
                    res.Error += $"<br>Falta el Valor del Codigo";
                if (!string.IsNullOrEmpty(Descripcion))
                    res.Error += $"<br>Falta el Valor de la Descripcion";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("ComponenteMayor NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE ComponenteMayor WHERE Id = @id", Conexion);
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
                Descripcion = Registro.Descripcion;
                Activo = Registro.Activo;
                Valid = true;
                GetCapacidades();
                GetLimites();
            }
        }
        private void Inicializar() {
            Id = 0;
            Codigo = "";
            Descripcion = "";
            Activo = false;
            Valid = false;
            Capacidades = new List<ComponenteMayorCapacidad>();
            Limites = new List<Limites>();
        }
        public static List<ComponenteMayor> GetComponentesMayores() {
            List<ComponenteMayor> comays = new List<ComponenteMayor>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM ComponenteMayor", Conexion));
            foreach (var reg in res.Rows) {
                ComponenteMayor coma = JsonConvert.DeserializeObject<ComponenteMayor>(JsonConvert.SerializeObject(reg));
                coma.Valid = true;
                comays.Add(coma);
            }
            return comays;
        }
        public void GetCapacidades() {
            Capacidades = ComponenteMayorCapacidad.GetComponenteMayorCapacidades(Id);
        }
        public void GetLimites() {
            SqlCommand comando = new SqlCommand("SELECT * FROM Limites WHERE Id IN (SELECT IdLimites FROM ComponenteMayorLimites WHERE IdComponenteMayor = @idcm)", Conexion);
            comando.Parameters.Add(new SqlParameter("@idcm", Id));
            var qry = DataBase.Query(comando);
            foreach (var reg in qry.Rows) {
                Limites lim = JsonConvert.DeserializeObject<Limites>(JsonConvert.SerializeObject(reg));
                lim.Valid = true;
                Limites.Add(lim);
            }
        }
        public int GetCantidad(int idCapacidad) {
            int cantidad = 0;
            if (Capacidades == null)
                GetCapacidades();
            foreach(var cap in Capacidades) {
                if (cap.IdCapacidad == idCapacidad) {
                    cantidad = cap.Cantidad;
                    break;
				}
			}
            return cantidad;
		}
    }
}