using Newtonsoft.Json;

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace ATSM.Ingenieria {
	public class Capacidad {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; set; }
		public List<Modelo> Modelos { get; set; }
		public Capacidad(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Capacidad WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Capacidad(string cadena) {
            Inicializar();
            if (!string.IsNullOrEmpty(cadena)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Capacidad WHERE Nombre = @cadena OR Descripcion = @cadena", Conexion);
                comando.Parameters.Add(new SqlParameter("@cadena", cadena));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Capacidad(int id, string codigo, string descripcion = "", bool activo = false, bool valid = false) {
            Id = id;
            Nombre = codigo;
            Descripcion = descripcion;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nombre) && !string.IsNullOrEmpty(Descripcion)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Capacidad WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Capacidad ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Capacidad SET Nombre = @codigo, Descripcion = @descripcion, Activo = @activo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Capacidad(Nombre, Descripcion, Activo) VALUES(@codigo, @descripcion, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@codigo", string.IsNullOrEmpty(Nombre) ? SqlString.Null : Nombre));
                Command.Parameters.Add(new SqlParameter("@descripcion", string.IsNullOrEmpty(Descripcion) ? SqlString.Null : Descripcion));
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
                if (!string.IsNullOrEmpty(Nombre))
                    res.Error += $"<br>Falta el Valor del Nombre";
                if (!string.IsNullOrEmpty(Descripcion))
                    res.Error += $"<br>Falta el Valor de la Descripcion";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Capacidad NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Capacidad WHERE Id = @id", Conexion);
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
                Descripcion = Registro.Descripcion;
                Activo = Registro.Activo;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            Nombre = "";
            Descripcion = "";
            Activo = false;
            Valid = false;
        }
        public static List<Capacidad> GetCapacidades() {
            List<Capacidad> capacidads = new List<Capacidad>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Capacidad", Conexion));
            foreach (var reg in res.Rows) {
                Capacidad capacidad = JsonConvert.DeserializeObject<Capacidad>(JsonConvert.SerializeObject(reg));
                capacidad.Valid = true;
                capacidads.Add(capacidad);
            }
            return capacidads;
        }
        public void GetModelos() {
            Modelos = new List<Modelo>();
            SqlCommand comando = new SqlCommand("SELECT * FROM Modelo WHERE IdCapacidad = @idca", Conexion);
            comando.Parameters.AddWithValue("@idca", Id);
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                Modelo modelo = JsonConvert.DeserializeObject<Modelo>(JsonConvert.SerializeObject(reg));
                modelo.Valid = true;
                modelo.Capacidad = this;
                Modelos.Add(modelo);
            }
        }
        public static string Converter(string capa) {
            string def = "";
			switch (capa) {
                case "metro":
                def = "Metro";
                break;
                case "citation":
                def = "C500";
                break;
                case "crj":
                def = "CL600";
                break;
                case "cvtl":
                def = "CVL";
                break;
                case "dc9":
                def = "DC-9";
                break;
                case "hawker":
                def = "HS125";
                break;
                case "md80":
                def = "MD-80";
                break;
			}
            return def;
		}
    }
}