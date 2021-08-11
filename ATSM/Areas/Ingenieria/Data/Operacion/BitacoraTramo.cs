using ATSM.Seguimiento;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class BitacoraTramo {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdBitacora { get; set; }
		public int Tramo { get; set; }
		public int IdOrigen { get; set; }
		public int IdDestino { get; set; }
		public DateTime Fecha { get; set; }
		public TimeSpan? SalidaPlataforma { get; set; }
		public TimeSpan? LlegadaPlataforma { get; set; }
		public TimeSpan? Calzo { get; set; }
		public TimeSpan? Despegue { get; set; }
		public TimeSpan? Aterrizaje { get; set; }
		public TimeSpan? Tiempo { get; set; }
		public string Demora { get; set; }
		public int Velocidad { get; set; }
		public int Altitud { get; set; }
		public int CombustibleSalida { get; set; }
		public int CombustibleLlegada { get; set; }
		public int Peso { get; set; }
		public int Pasajeros { get; set; }
		public int IdVuelo { get; set; }
		public int IdTramo { get; set; }
		public bool Valid { get; set; }
		public Aeropuerto Origen { get; set; }
		public Aeropuerto Destino { get; set; }
        public BitacoraTramo(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM BitacoraTramos WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public BitacoraTramo(int idBitacora, int tramo) {
            Inicializar();
            if (idBitacora > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM BitacoraTramos WHERE IdBitacora = @idBitacora AND Tramo = @tramo", Conexion);
                comando.Parameters.Add(new SqlParameter("@idBitacora", idBitacora));
                comando.Parameters.Add(new SqlParameter("@tramo", tramo));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public BitacoraTramo(int id, int idBitacora, int tramo, int idOrigen, int idDestino, DateTime fecha = default(DateTime), TimeSpan? salidaPlataforma = null, TimeSpan? llegadaPlataforma = null, TimeSpan? calzo = null, TimeSpan? despegue = null, TimeSpan? aterrizaje = null, TimeSpan? tiempo = null, string demora = "", int velocidad = 0, int altitud = 0, int combustibleSalida = 0, int combustibleLlegada = 0, int peso = 0, int pasajeros = 0, int idVuelo = 0, int idTramo = 0, bool valid = false) {
            Id = id;
            IdBitacora = idBitacora;
            Tramo = tramo;
            IdOrigen = idOrigen;
            IdDestino = idDestino;
            Fecha = fecha;
            SalidaPlataforma = salidaPlataforma;
            LlegadaPlataforma = llegadaPlataforma;
            Calzo = calzo;
            Despegue = despegue;
            Aterrizaje = aterrizaje;
            Tiempo = tiempo;
            Demora = demora;
            Velocidad = velocidad;
            Altitud = altitud;
            CombustibleSalida = combustibleSalida;
            CombustibleLlegada = combustibleLlegada;
            Peso = peso;
            Pasajeros = pasajeros;
            IdVuelo = idVuelo;
            IdTramo = idTramo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdBitacora > 0 && Tramo > 0 && IdOrigen > 0 && IdDestino > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM BitacoraTramos WHERE Id = @id OR (IdBitacora = @idBitacora AND Tramo = @tramo)", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@idBitacora", IdBitacora));
                Cmnd.Parameters.Add(new SqlParameter("@tramo", Tramo));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "BitacoraTramo ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE BitacoraTramos SET IdBitacora = @idbitacora, Tramo = @tramo, IdOrigen = @idorigen, IdDestino = @iddestino, Fecha = @fecha, SalidaPlataforma = @salidaPlataforma, LlegadaPlataforma = @llegadaPlataforma, Calzo = @calzo, Despegue = @despegue, Aterrizaje = @aterrizaje, Tiempo = @tiempo, Demora = @demora, Velocidad = @velocidad, Altitud = @altitud, CombustibleSalida = @combustiblesalida, CombustibleLlegada = @combustiblellegada, Peso = @peso, Pasajeros = @pasajeros, IdVuelo = @idvuelo, IdTramo = @idtramo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO BitacoraTramos(IdBitacora, Tramo, IdOrigen, IdDestino, Fecha, SalidaPlataforma, LlegadaPlataforma, Calzo, Despegue, Aterrizaje, Tiempo, Demora, Velocidad, Altitud, CombustibleSalida, CombustibleLlegada, Peso, Pasajeros, IdVuelo, IdTramo) VALUES(@idbitacora, @tramo, @idorigen, @iddestino, @fecha, @salidaPlataforma, @llegadaPlataforma, @calzo, @despegue, @aterrizaje, @tiempo, @demora, @velocidad, @altitud, @combustiblesalida, @combustiblellegada, @peso, @pasajeros, @idvuelo, @idtramo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idbitacora", IdBitacora));
                Command.Parameters.Add(new SqlParameter("@tramo", Tramo));
                Command.Parameters.Add(new SqlParameter("@idorigen", IdOrigen));
                Command.Parameters.Add(new SqlParameter("@iddestino", IdDestino));
                Command.Parameters.Add(new SqlParameter("@fecha", Fecha));
                Command.Parameters.Add(new SqlParameter("@salidaPlataforma", SalidaPlataforma == null ? DBNull.Value : SalidaPlataforma));
                Command.Parameters.Add(new SqlParameter("@llegadaPlataforma", LlegadaPlataforma == null ? DBNull.Value : LlegadaPlataforma));
                Command.Parameters.Add(new SqlParameter("@calzo", Calzo == null ? DBNull.Value : Calzo));
                Command.Parameters.Add(new SqlParameter("@despegue", Despegue == null ? DBNull.Value : Despegue));
                Command.Parameters.Add(new SqlParameter("@aterrizaje", Aterrizaje == null ? DBNull.Value : Aterrizaje));
                Command.Parameters.Add(new SqlParameter("@tiempo", Tiempo == null ? DBNull.Value : Tiempo));
                Command.Parameters.Add(new SqlParameter("@demora", string.IsNullOrEmpty(Demora) ? SqlString.Null : Demora));
                Command.Parameters.Add(new SqlParameter("@velocidad", Velocidad));
                Command.Parameters.Add(new SqlParameter("@altitud", Altitud));
                Command.Parameters.Add(new SqlParameter("@combustiblesalida", CombustibleSalida));
                Command.Parameters.Add(new SqlParameter("@combustiblellegada", CombustibleLlegada));
                Command.Parameters.Add(new SqlParameter("@peso", Peso));
                Command.Parameters.Add(new SqlParameter("@pasajeros", Pasajeros));
                Command.Parameters.Add(new SqlParameter("@idvuelo", IdVuelo));
                Command.Parameters.Add(new SqlParameter("@idtramo", IdTramo));
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
                if (IdBitacora <= 0)
                    res.Error += $"<br>Falta la Bitacora";
                if (Tramo <= 0)
                    res.Error += $"<br>Falta la tramo";
                if (IdOrigen <= 0)
                    res.Error += $"<br>Falta el Otrigen del Tramo";
                if (IdDestino <= 0)
                    res.Error += $"<br>Falta el Destino del Tramo";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("BitacoraTramo NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE BitacoraTramos WHERE Id = @id", Conexion);
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
                IdBitacora = Registro.IdBitacora;
                Tramo = Registro.Tramo;
                IdOrigen = Registro.IdOrigen;
                IdDestino = Registro.IdDestino;
                Fecha = Registro.Fecha;
                SalidaPlataforma = Registro.SalidaPlataforma;
                LlegadaPlataforma = Registro.LlegadaPlataforma;
                Calzo = Registro.Calzo;
                Despegue = Registro.Despegue;
                Aterrizaje = Registro.Aterrizaje;
                Tiempo = Registro.Tiempo;
                Demora = Registro.Demora;
                Velocidad = Registro.Velocidad;
                Altitud = Registro.Altitud;
                CombustibleSalida = Registro.CombustibleSalida;
                CombustibleLlegada = Registro.CombustibleLlegada;
                Peso = Registro.Peso;
                Pasajeros = Registro.Pasajeros;
                IdVuelo = Registro.IdVuelo;
                IdTramo = Registro.IdTramo;
                Valid = true;
                SetOrigen();
                SetDestino();
            }
        }
        private void Inicializar() {
            Id = 0;
            IdBitacora = 0;
            Tramo = 0;
            IdOrigen = 0;
            IdDestino = 0;
            Fecha = default(DateTime);
            SalidaPlataforma = null;
            LlegadaPlataforma = null;
            Calzo = null;
            Despegue = null;
            Aterrizaje = null;
            Tiempo = null;
            Demora = "";
            Velocidad = 0;
            Altitud = 0;
            CombustibleSalida = 0;
            CombustibleLlegada = 0;
            Peso = 0;
            Pasajeros = 0;
            IdVuelo = 0;
            IdTramo = 0;
            Valid = false;
            SetOrigen();
            SetDestino();
        }
        public static List<BitacoraTramo> GetBitacoraTramos(int idBitacora) {
            List<BitacoraTramo> bitacoratramos = new List<BitacoraTramo>();
            SqlCommand comando = new SqlCommand("SELECT * FROM BitacoraTramos WHERE IdBitacora = @idBitacora ORDER BY Tramo", Conexion);
            comando.Parameters.Add(new SqlParameter("@idBitacora", idBitacora));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                BitacoraTramo bitacoratramo = JsonConvert.DeserializeObject<BitacoraTramo>(JsonConvert.SerializeObject(reg));
                bitacoratramo.Valid = true;
                bitacoratramos.Add(bitacoratramo);
            }
            return bitacoratramos;
        }
        public void SetOrigen() {
            Origen = new Aeropuerto(IdOrigen);
		}
        public void SetDestino() {
            Destino = new Aeropuerto(IdDestino);
		}
    }
}