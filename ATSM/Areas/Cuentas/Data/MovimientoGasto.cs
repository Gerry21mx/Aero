using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace ATSM.Cuentas {
	public class MovimientoGasto {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Nombre { get; set; }
		public int Operacion { get; set; }
		public bool Valid { get; set; }
        public MovimientoGasto(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM MovimientoGasto WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public MovimientoGasto(string nombre) {
            Inicializar();
            if (!string.IsNullOrEmpty(nombre)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM MovimientoGasto WHERE Nombre = @nombre", Conexion);
                comando.Parameters.Add(new SqlParameter("@nombre", nombre));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public MovimientoGasto(int id, string nombre, int operacion) {
            Id = id;
            Nombre = nombre;
            Operacion = operacion;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nombre)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM MovimientoGasto WHERE Id = @id OR Nombre = @nombre", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@nombre", Nombre));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "MovimientoGasto ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE MovimientoGasto SET Nombre = @nombre, Operacion = @operacion WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO MovimientoGasto(Nombre, Operacion) VALUES(@nombre, @operacion)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@nombre", string.IsNullOrEmpty(Nombre) ? SqlString.Null : Nombre));
                Command.Parameters.Add(new SqlParameter("@operacion", Operacion));
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
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("MovimientoGasto NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE MovimientoGasto WHERE Id = @id", Conexion);
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
                Operacion = Registro.Operacion;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            Nombre = "";
            Operacion = 0;
        }
        public static List<MovimientoGasto> GetMovimientoGastos() {
            List<MovimientoGasto> movimientogastos = new List<MovimientoGasto>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM MovimientoGasto", Conexion));
            foreach (var reg in res.Rows) {
                MovimientoGasto movimientogasto = JsonConvert.DeserializeObject<MovimientoGasto>(JsonConvert.SerializeObject(reg));
                movimientogasto.Valid = true;
                movimientogastos.Add(movimientogasto);
            }
            return movimientogastos;
        }
    }
}