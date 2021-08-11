using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Newtonsoft.Json;

namespace ATSM.Seguimiento {
	public class Aeropuerto {
        private static SqlConnection Conexion = DataBase.Conexion();
        public int IdAeropuerto { get; private set; }
		public string Nombre { get; set; }
		public string ICAO { get; set; }
		public string IATA { get; set; }
		public string Pais { get; set; }
		public string Estado { get; set; }
		public string Latitud { get; set; }
		public string Longitud { get; set; }
		public int? Elevacion { get; set; }
		public TimeSpan? Abre { get; set; }
		public TimeSpan? Cierra { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; set; }
		public Aeropuerto(int? idaeropuerto = null) {
            Inicializar();
            if (idaeropuerto > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Aeropuerto WHERE IdAeropuerto = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", idaeropuerto));
                SetDatos(comando);
            }
        }
		public Aeropuerto(string aeropuerto) {
			Inicializar();
			if (!string.IsNullOrEmpty(aeropuerto)) {
				SqlCommand comando = new SqlCommand($"SELECT * FROM Aeropuerto WHERE IATA = @cod OR ICAO = @cod OR Nombre = @cod", Conexion);
				comando.Parameters.Add(new SqlParameter("@cod", aeropuerto));
				SetDatos(comando);
			}
		}
        [JsonConstructor]
        public Aeropuerto(int idaeropuerto, string nombre, string iCAO = "", string iATA = "", string pais = null, string estado = null, string latitud = null, string longitud = null, int? elevacion = null, TimeSpan? abre=null, TimeSpan? cierra=null, bool activo = false, bool valid = false) {
            IdAeropuerto = idaeropuerto;
            Nombre = nombre;
            ICAO = iCAO;
            IATA = iATA;
            Pais = pais;
            Estado = estado;
            Latitud = latitud;
            Longitud = longitud;
            Elevacion = elevacion;
            Abre = abre;
            Cierra = cierra;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nombre) && !string.IsNullOrEmpty(IATA) && !string.IsNullOrEmpty(ICAO)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT IdAeropuerto FROM Aeropuerto WHERE IdAeropuerto = @idaeropuerto", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idaeropuerto", IdAeropuerto));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Aeropuerto ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Aeropuerto SET Nombre = @nombre, ICAO = @icao, IATA = @iata, Pais = @pais, Estado = @estado, Latitud = @latitud, Longitud = @longitud, Elevacion = @elevacion, Abre = @abre, Cierra = @cierra, Activo = @activo WHERE IdAeropuerto=@idaeropuerto";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Aeropuerto(Nombre, ICAO, IATA, Pais, Estado, Latitud, Longitud, Elevacion, Abre, Cierra, Activo) VALUES(@nombre, @icao, @iata, @pais, @estado, @latitud, @longitud, @elevacion, @abre, @cierra, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idaeropuerto", IdAeropuerto));
                Command.Parameters.Add(new SqlParameter("@nombre", Nombre));
                Command.Parameters.Add(new SqlParameter("@icao", ICAO));
                Command.Parameters.Add(new SqlParameter("@iata", IATA));
                Command.Parameters.Add(new SqlParameter("@pais", string.IsNullOrEmpty(Pais) ? DBNull.Value : Pais));
                Command.Parameters.Add(new SqlParameter("@estado", string.IsNullOrEmpty(Estado) ? DBNull.Value : Estado));
                Command.Parameters.Add(new SqlParameter("@latitud", string.IsNullOrEmpty(Latitud) ? DBNull.Value : Latitud));
                Command.Parameters.Add(new SqlParameter("@longitud", string.IsNullOrEmpty(Longitud) ? DBNull.Value : Longitud));
                Command.Parameters.Add(new SqlParameter("@elevacion", Elevacion ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@abre", Abre == null ? DBNull.Value : Abre ));
                Command.Parameters.Add(new SqlParameter("@cierra", Cierra == null ? DBNull.Value : Cierra));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid) {
                    if (Insr) {
                        if (rInUp.IdRegistro == 0) {
                            res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
                            return res;
                        }
                        IdAeropuerto = rInUp.IdRegistro;
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
            Respuesta res = new Respuesta("Aeropuerto NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Aeropuerto WHERE IdAeropuerto = @id", Conexion);
            Command.Parameters.Add(new SqlParameter("@id", IdAeropuerto));
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
                IdAeropuerto = Registro.IdAeropuerto;
                Nombre = Registro.Nombre;
                ICAO = Registro.ICAO;
                IATA = Registro.IATA;
                Pais = Registro.Pais;
                Estado = Registro.Estado;
                Latitud = Registro.Latitud;
                Longitud = Registro.Longitud;
                Elevacion = Registro.Elevacion;
                Abre = Registro.Abre;
                Cierra = Registro.Cierra;
                Activo = Registro.Activo;
                Valid = true;
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdAeropuerto = 0;
            Nombre = "";
            ICAO = "";
            IATA = "";
            Pais = null;
            Estado = null;
            Latitud = null;
            Longitud = null;
            Elevacion = null;
            Abre = null;
            Cierra = null;
            Activo = false;
            Valid = false;
        }
        public static List<Aeropuerto> GetAeropuertos(bool? activos = null) {
            List<Aeropuerto> aeropuertos = new List<Aeropuerto>();
            RespuestaQuery res = DataBase.Query(new SqlCommand($"SELECT * FROM Aeropuerto{(activos==true?" WHERE Activo = 1":"")}", Conexion));
            foreach (var reg in res.Rows) {
                Aeropuerto aeropuerto = JsonConvert.DeserializeObject<Aeropuerto>(JsonConvert.SerializeObject(reg));
                aeropuerto.Valid = true;
                aeropuertos.Add(aeropuerto);
            }
            return aeropuertos;
        }
    }
}