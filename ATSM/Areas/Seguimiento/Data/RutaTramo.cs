using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Newtonsoft.Json;

namespace ATSM.Seguimiento {
	public class RutaTramo {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int IdRutaTramo { get; private set; }
		public int IdRuta { get; set; }
		public int Pierna { get; set; }
		public int IdOrigen { get; set; }
		public Aeropuerto Origen { get; set; }
		public int IdDestino { get; set; }
        public Aeropuerto Destino { get; set; }
        public TimeSpan? ItinerarioSalida { get; set; }
		public TimeSpan? ItinerarioLlegada { get; set; }
		public int? NoVuelo { get; set; }
		public bool Valid { get; set; }
        public RutaTramo(int? idrutatramo = null) {
            Inicializar();
            if (idrutatramo > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM RutaTramo WHERE IdRutaTramo = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", idrutatramo));
                SetDatos(comando);
            }
        }
        public RutaTramo(int idruta, int pierna) {
            Inicializar();
            if (idruta > 0 && pierna>0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM RutaTramo WHERE IdRuta = @idruta AND Pierna = @pierna", Conexion);
                comando.Parameters.Add(new SqlParameter("@idruta", idruta));
                comando.Parameters.Add(new SqlParameter("@pierna", pierna));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public RutaTramo(int idRutaTramo, int idRuta, int pierna, int idOrigen = 0, int idDestino = 0, TimeSpan? itinerarioSalida = null, TimeSpan? itinerarioLlegada = null, int noVuelo = 0, bool valid = false) {
            IdRutaTramo = idRutaTramo;
            IdRuta = idRuta;
            Pierna = pierna;
            IdOrigen = idOrigen;
            IdDestino = idDestino;
            ItinerarioSalida = itinerarioSalida;
            ItinerarioLlegada = itinerarioLlegada;
            NoVuelo = noVuelo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdRuta>0 && IdOrigen>0 && IdDestino>0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT IdRutaTramo FROM RutaTramo WHERE IdRutaTramo = @idrutatramo OR (IdRuta = @idruta AND Pierna = @pierna)", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idrutatramo", IdRutaTramo));
                Cmnd.Parameters.Add(new SqlParameter("@idruta", IdRuta));
                Cmnd.Parameters.Add(new SqlParameter("@pierna", Pierna));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Tramo de Ruta ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE RutaTramo SET IdRuta = @idruta, Pierna = @pierna, IdOrigen = @idorigen, IdDestino = @iddestino, ItinerarioSalida = @itinerariosalida, ItinerarioLlegada = @itinerariollegada, NoVuelo = @novuelo WHERE IdRutaTramo=@idrutatramo";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO RutaTramo(IdRuta, Pierna, IdOrigen, IdDestino, ItinerarioSalida, ItinerarioLlegada, NoVuelo) VALUES(@idruta, @pierna, @idorigen, @iddestino, @itinerariosalida, @itinerariollegada, @novuelo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idrutatramo", IdRutaTramo));
                Command.Parameters.Add(new SqlParameter("@idruta", IdRuta));
                Command.Parameters.Add(new SqlParameter("@pierna", Pierna));
                Command.Parameters.Add(new SqlParameter("@idorigen", IdOrigen));
                Command.Parameters.Add(new SqlParameter("@iddestino", IdDestino));
                Command.Parameters.Add(new SqlParameter("@itinerariosalida", ItinerarioSalida == null ? DBNull.Value : ItinerarioSalida));
                Command.Parameters.Add(new SqlParameter("@itinerariollegada", ItinerarioLlegada == null ? DBNull.Value : ItinerarioLlegada));
                Command.Parameters.Add(new SqlParameter("@novuelo", NoVuelo ?? SqlInt32.Null));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid) {
                    if (Insr) {
                        if (rInUp.IdRegistro == 0) {
                            res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
                            return res;
                        }
                        IdRutaTramo = rInUp.IdRegistro;
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
            Respuesta res = new Respuesta("Tramo de Ruta NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE RutaTramo WHERE IdRutaTramo = @id", Conexion);
            Command.Parameters.Add(new SqlParameter("@id", IdRutaTramo));
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
                IdRutaTramo = Registro.IdRutaTramo;
                IdRuta = Registro.IdRuta;
                Pierna = Registro.Pierna;
                IdOrigen = Registro.IdOrigen;
                IdDestino = Registro.IdDestino;
                ItinerarioSalida = Registro.ItinerarioSalida;
                ItinerarioLlegada = Registro.ItinerarioLlegada;
                NoVuelo = Registro.NoVuelo;
                Valid = true;
                GetOrigen();
                GetDestino();
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdRutaTramo = 0;
            IdRuta = 0;
            Pierna = 0;
            IdOrigen = 0;
            IdDestino = 0;
            ItinerarioSalida = null;
            ItinerarioLlegada = null;
            NoVuelo = null;
            Valid = false;
        }
        //public static List<RutaTramo> GetRutaTramos() {
        //    List<RutaTramo> rutatramos = new List<RutaTramo>();
        //    RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM RutaTramo", Conexion));
        //    foreach (var reg in res.Rows) {
        //        RutaTramo rutatramo = JsonConvert.DeserializeObject<RutaTramo>(JsonConvert.SerializeObject(reg));
        //        rutatramo.Valid = true;
        //        rutatramos.Add(rutatramo);
        //    }
        //    return rutatramos;
        //}
        public static List<RutaTramo> GetTramosRuta(int idRuta) {
            List<RutaTramo> rutatramos = new List<RutaTramo>();
            SqlCommand comando = new SqlCommand("SELECT * FROM RutaTramo WHERE IdRuta = @idRuta ORDER BY Pierna", Conexion);
            comando.Parameters.Add(new SqlParameter("@idRuta", idRuta));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                RutaTramo rutatramo = JsonConvert.DeserializeObject<RutaTramo>(JsonConvert.SerializeObject(reg));
                rutatramo.Valid = true;
                rutatramos.Add(rutatramo);
            }
            return rutatramos;
        }
        public Aeropuerto GetOrigen() {
            Origen = new Aeropuerto(IdOrigen);
            return Origen;
        }
        public Aeropuerto GetDestino() {
            Destino = new Aeropuerto(IdDestino);
            return Destino;
        }
    }
}