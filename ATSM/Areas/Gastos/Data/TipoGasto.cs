using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Newtonsoft.Json;

namespace ATSM.Gastos {
	public class TipoGasto {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Codigo { get; set; }
		public string Nombre { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; set; }
        public TipoGasto(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM TipoGasto WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public TipoGasto(string cadena) {
            Inicializar();
            if (!string.IsNullOrEmpty(cadena)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM TipoGasto WHERE Codigo = @cadena OR Nombre = @cadena", Conexion);
                comando.Parameters.Add(new SqlParameter("@cadena", cadena));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public TipoGasto(int id, string codigo, string nombre, bool activo = false, bool valid = false) {
            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Codigo) && !string.IsNullOrEmpty(Nombre)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM TipoGasto WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "TipoGasto ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE TipoGasto SET Codigo = @codigo, Nombre = @nombre, Activo = @activo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO TipoGasto(Codigo, Nombre, Activo) VALUES(@codigo, @nombre, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@codigo", string.IsNullOrEmpty(Codigo) ? SqlString.Null : Codigo));
                Command.Parameters.Add(new SqlParameter("@nombre", string.IsNullOrEmpty(Nombre) ? SqlString.Null : Nombre));
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
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Tipo de Gasto NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE TipoGasto WHERE Id = @id", Conexion);
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
                Activo = Registro.Activo;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            Codigo = "";
            Nombre = "";
            Activo = false;
            Valid = false;
        }
        public static List<TipoGasto> GetTipoGastos() {
            List<TipoGasto> tipogastos = new List<TipoGasto>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM TipoGasto", Conexion));
            foreach (var reg in res.Rows) {
                TipoGasto tipogasto = JsonConvert.DeserializeObject<TipoGasto>(JsonConvert.SerializeObject(reg));
                tipogasto.Valid = true;
                tipogastos.Add(tipogasto);
            }
            return tipogastos;
        }
    }
}