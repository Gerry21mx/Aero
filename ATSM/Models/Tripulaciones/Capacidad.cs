using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using ATSM.Areas.Ingenieria.Data;

using Newtonsoft.Json;

namespace ATSM.Tripulaciones {
	public class Capacidad {
		private static SqlConnection Conexion = DataBase.Conexion("BD_ASTRIP");
		public int IdCapacidad { get; private set; }
		public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public int? Consecutivo { get; set; }
		public int Velocidad { get; set; }
		public int? Velocidad2 { get; set; }
		public List<Modelo> Modelos { get; set; }
		public bool Valid { get; set; }
		public Capacidad(int? idcapacidad = null) {
            Inicializar();
            if (idcapacidad > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM catCap WHERE id = @idcapacidad", Conexion);
                comando.Parameters.Add(new SqlParameter("@idcapacidad", idcapacidad));
                SetDatos(comando);
            }
        }
		public Capacidad(string capacidad) {
			Inicializar();
			if (!string.IsNullOrEmpty(capacidad)) {
				SqlCommand comando = new SqlCommand($"SELECT * FROM catCap WHERE cap = @capacidad", Conexion);
				comando.Parameters.Add(new SqlParameter("@capacidad", capacidad));
				SetDatos(comando);
			}
		}
        [JsonConstructor]
        public Capacidad(int idCapacidad, string nombre, string descripcion = "", int? consecutivo = null, int velocidad = 0, int? velocidad2 = null, bool valid = false) {
            IdCapacidad = idCapacidad;
            Nombre = nombre;
            Descripcion = descripcion;
            Consecutivo = consecutivo;
            Velocidad = velocidad;
            Velocidad2 = velocidad2;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nombre)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT IdCapacidad FROM catCap WHERE IdCapacidad = @idcapacidad", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idcapacidad", IdCapacidad));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Capacidad ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE catCap SET cap = @nombre, des = @descripcion, noCons = @consecutivo, vel = @velocidad, vel2 = @velocidad2 WHERE id = @idcapacidad";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO catCap(cap, des, noCons, vel, vel2) VALUES(@nombre, @descripcion, @consecutivo, @velocidad, @velocidad2)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idcapacidad", IdCapacidad));
                Command.Parameters.Add(new SqlParameter("@nombre", Nombre));
                Command.Parameters.Add(new SqlParameter("@descripcion", Descripcion));
                Command.Parameters.Add(new SqlParameter("@consecutivo", Consecutivo ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@velocidad", Velocidad));
                Command.Parameters.Add(new SqlParameter("@velocidad2", Velocidad2 ?? SqlInt32.Null));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid) {
                    if (Insr) {
                        if (rInUp.IdRegistro == 0) {
                            res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
                            return res;
                        }
                        IdCapacidad = rInUp.IdRegistro;
                        Valid = true;
                    }
                }
                else {
                    res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br> Error: {rInUp.Error}";
                    return res;
                }
                res.Elemento = this;
                res.Valid = true;
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Capacidad NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE catCap WHERE id = @idcapacidad", Conexion);
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
                IdCapacidad = Registro.id;
                Nombre = Registro.cap;
                Descripcion = Registro.des;
                Consecutivo = Registro.noCons;
                Velocidad = Registro.vel;
                Velocidad2 = Registro.vel2;
                Valid = true;
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdCapacidad = 0;
            Nombre = "";
            Descripcion = "";
            Consecutivo = null;
            Velocidad = 0;
            Velocidad2 = null;
            Valid = false;
        }
        public static List<Capacidad> GetCapacidads() {
            List<Capacidad> capacidads = new List<Capacidad>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT id AS idCapacidad, cap AS nombre, des AS descripcion, noCons AS consecutivo, vel AS velocidad, vel2 AS velocidad2, 1 As valid FROM catCap", Conexion));
            foreach (var reg in res.Rows) {
                Capacidad capacidad = JsonConvert.DeserializeObject<Capacidad>(JsonConvert.SerializeObject(reg));
                capacidads.Add(capacidad);
            }
            return capacidads;
        }
        public List<Modelo> GetModelos() {
            Modelos = new List<Modelo>();
            SqlCommand comando = new SqlCommand("SELECT * FROM Modelo WHERE IdCapacidad = @cap", DataBase.Conexion()); ;
            comando.Parameters.Add(new SqlParameter("@cap", IdCapacidad));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                Modelo modelo = JsonConvert.DeserializeObject<Modelo>(JsonConvert.SerializeObject(reg));
                Modelos.Add(modelo);
            }
            return Modelos;
        }
    }
}