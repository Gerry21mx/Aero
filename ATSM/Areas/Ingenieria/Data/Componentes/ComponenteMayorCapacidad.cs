using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ATSM.Ingenieria {
	public class ComponenteMayorCapacidad {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int IdComponenteMayor { get; set; }
		public int IdCapacidad { get; set; }
		public int Cantidad { get; set; }
		public bool Valid { get; set; }
		public Capacidad Capacidad { get; set; }
		ComponenteMayorCapacidad() { Inicializar(); }
		public ComponenteMayorCapacidad(int idmayor, int idcapacidad) {
            Inicializar();
            if (idmayor > 0 && idcapacidad > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ComponenteMayorCapacidad WHERE IdComponenteMayor = @idmayor AND IdCapacidad = @idcapacidad", Conexion);
                comando.Parameters.Add(new SqlParameter("@idmayor", idmayor));
                comando.Parameters.Add(new SqlParameter("@idcapacidad", idcapacidad));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public ComponenteMayorCapacidad(int idComponenteMayor, int idCapacidad, int cantidad) {
            IdComponenteMayor = idComponenteMayor;
            IdCapacidad = idCapacidad;
            Cantidad = cantidad;
		}
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdComponenteMayor > 0 && IdCapacidad > 0 && Cantidad > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT * FROM ComponenteMayorCapacidad WHERE IdComponenteMayor = @idmayor AND IdCapacidad = @idcapacidad", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idmayor", IdComponenteMayor));
                Cmnd.Parameters.Add(new SqlParameter("@idcapacidad", IdCapacidad));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "ComponenteMayorCapacidad ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE ComponenteMayorCapacidad SET IdComponenteMayor = @idcomponentemayor, IdCapacidad = @idcapacidad, Cantidad = @cantidad WHERE IdComponenteMayor = @idcomponentemayor AND IdCapacidad = @idcapacidad";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO ComponenteMayorCapacidad(IdComponenteMayor, IdCapacidad, Cantidad) VALUES(@idcomponentemayor, @idcapacidad, @cantidad)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idcomponentemayor", IdComponenteMayor));
                Command.Parameters.Add(new SqlParameter("@idcapacidad", IdCapacidad));
                Command.Parameters.Add(new SqlParameter("@cantidad", Cantidad));
                RespuestaQuery rInUp = DataBase.Execute(Command);
                if (rInUp.Valid)
                    if (Insr)
                        Valid = true;
                else {
                    res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                    return res;
                }
                res.Elemento = this;
                res.Valid = true;
            }
            else {
                if (IdComponenteMayor > 0)
                    res.Error += $"<br>Falta el Componente Mayor";
                if (IdCapacidad > 0)
                    res.Error += $"<br>Falta la Capacidad";
                if (Cantidad > 0)
                    res.Error += $"<br>Falta la Cantidad";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Capacidad NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE ComponenteMayorCapacidad WHERE IdComponenteMayor = @idmayor AND IdCapacidad = @idcapacidad", Conexion);
            Command.Parameters.Add(new SqlParameter("@idmayor", IdComponenteMayor));
            Command.Parameters.Add(new SqlParameter("@idcapacidad", IdCapacidad));
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
                IdComponenteMayor = Registro.IdComponenteMayor;
                IdCapacidad = Registro.IdCapacidad;
                Cantidad = Registro.Cantidad;
                Valid = true;
                SetCapacidad();
            }
        }
        private void Inicializar() {
            IdComponenteMayor = 0;
            IdCapacidad = 0;
            Cantidad = 0;
        }
        public void SetCapacidad() {
            Capacidad = new Capacidad(IdCapacidad);
        }
        public static List<ComponenteMayorCapacidad> GetComponenteMayorCapacidades(int idMayor) {
            List<ComponenteMayorCapacidad> capacidades = new List<ComponenteMayorCapacidad>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ComponenteMayorCapacidad WHERE IdComponenteMayor = @idMayor", Conexion);
            comando.Parameters.Add(new SqlParameter("@idMayor", idMayor));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ComponenteMayorCapacidad CmC = JsonConvert.DeserializeObject<ComponenteMayorCapacidad>(JsonConvert.SerializeObject(reg));
                CmC.Valid = true;
                capacidades.Add(CmC);
            }
            return capacidades;
        }
    }
}