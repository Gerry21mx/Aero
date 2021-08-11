using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Transactions;

using ATSM.Ingenieria;
using ATSM.Tripulaciones;

using Newtonsoft.Json;

namespace ATSM.Seguimiento {
	public class VueloTramo {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int IdTramo { get; set; }
		public int IdVuelo { get; set; }
		public int Pierna { get; set; }
		public int IdAeronave { get; set; }
		public int? NoVuelo { get; set; }
		public int IdOrigen { get; set; }
		public int IdDestino { get; set; }
		public int? IdCapitan { get; set; }
		public int? IdCopiloto { get; set; }
		public int? NivelVuelo { get; set; }
		public int? SOB { get; set; }
		public DateTime? Salida { get; set; }
		public DateTime? Llegada { get; set; }
		public int? PiezasEquipaje { get; set; }
		public int? PesoEquipaje { get; set; }
		public int? PiezasCarga { get; set; }
		public int? PesoCarga { get; set; }
		public int? CombustibleRampa { get; set; }
		public int? CombustibleCargado { get; set; }
		public int? CombustibleRemanente { get; set; }
		public TimeSpan? InicioCarga { get; set; }
        public TimeSpan? TerminaCarga { get; set; }
		public TimeSpan? SalidaPlataforma { get; set; }
		public TimeSpan? LlegadaPlataforma { get; set; }
		public TimeSpan? Despegue { get; set; }
		public TimeSpan? Aterrizaje { get; set; }
		public string ObservacionesSaldia { get; set; }
		public string ObservacionesLlegada { get; set; }
		public TimeSpan? InicioDescarga { get; set; }
		public TimeSpan? TerminaDescarga { get; set; }
		public int? IdAlterno1 { get; set; }
		public int? IdAlterno2 { get; set; }
		public TimeSpan? ItinerarioDespegue { get; set; }
		public TimeSpan? ItinerarioAterrizaje { get; set; }
		public TimeSpan? ETA { get; set; }
		public int? IdDestinoOriginal { get; set; }
		public int? IdBitacora { get; set; }
		public bool Valid { get; set; }
        public List<VueloDemora> Demoras = new List<VueloDemora>();
        public List<VueloJumpSeat> JumpSeats = new List<VueloJumpSeat>();
		public Aircraft Aeronave { get; set; }
        public Aeropuerto Origen { get; set; }
        public Aeropuerto Destino { get; set; }
        public Crew Capitan { get; set; }
        public Crew Copiloto { get; set; }
        public VueloTramo(int? idtramo = null) {
            Inicializar();
            if (idtramo > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM VueloTramo WHERE IdTramo = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", idtramo));
                SetDatos(comando);
            }
        }
        public VueloTramo(int idVuelo, int pierna) {
            Inicializar();
            if (idVuelo > 0 && pierna>0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM VueloTramo WHERE IdVuelo = @idVuelo AND Pierna = @pierna", Conexion);
                comando.Parameters.Add(new SqlParameter("@idVuelo", idVuelo));
                comando.Parameters.Add(new SqlParameter("@pierna", pierna));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public VueloTramo(int idTramo, int idVuelo, int pierna, int idAeronave = 0, int? noVuelo = null, int idOrigen = 0, int idDestino = 0, int? idCapitan = null, int? idCopiloto = null, int? nivelVuelo = null, int? sOB = null, DateTime? salida = null, DateTime? llegada = null, int? piezasEquipaje = null, int? pesoEquipaje = null, int? piezasCarga = null, int? pesoCarga = null, int? combustibleRampa = null, int? combustibleCargado = null, int? combustibleRemanente = null, TimeSpan? inicioCarga = null, TimeSpan? terminaCarga = null, TimeSpan? salidaPlataforma = null, TimeSpan? llegadaPlataforma = null, TimeSpan? despegue = null, TimeSpan? aterrizaje = null, string observacionesSaldia = null, string observacionesLlegada = null, TimeSpan? inicioDescarga = null, TimeSpan? terminaDescarga = null, int? idAlterno1 = null, int? idAlterno2 = null, TimeSpan? itinerarioDespegue = null, TimeSpan? itinerarioAterrizaje = null, TimeSpan? eTA = null, int? idDestinoOriginal = null, int? idBitacora = null, bool valid = false) {
            IdTramo = idTramo;
            IdVuelo = idVuelo;
            Pierna = pierna;
            IdAeronave = idAeronave;
            NoVuelo = noVuelo;
            IdOrigen = idOrigen;
            IdDestino = idDestino;
            IdCapitan = idCapitan;
            IdCopiloto = idCopiloto;
            NivelVuelo = nivelVuelo;
            SOB = sOB;
            Salida = salida;
            Llegada = llegada;
            PiezasEquipaje = piezasEquipaje;
            PesoEquipaje = pesoEquipaje;
            PiezasCarga = piezasCarga;
            PesoCarga = pesoCarga;
            CombustibleRampa = combustibleRampa;
            CombustibleCargado = combustibleCargado;
            CombustibleRemanente = combustibleRemanente;
            InicioCarga = inicioCarga;
            TerminaCarga = terminaCarga;
            SalidaPlataforma = salidaPlataforma;
            LlegadaPlataforma = llegadaPlataforma;
            Despegue = despegue;
            Aterrizaje = aterrizaje;
            ObservacionesSaldia = observacionesSaldia;
            ObservacionesLlegada = observacionesLlegada;
            InicioDescarga = inicioDescarga;
            TerminaDescarga = terminaDescarga;
            IdAlterno1 = idAlterno1;
            IdAlterno2 = idAlterno2;
            ItinerarioDespegue = itinerarioDespegue;
            ItinerarioAterrizaje = itinerarioAterrizaje;
            ETA = eTA;
            IdDestinoOriginal = idDestinoOriginal;
            IdBitacora = idBitacora;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta();
            if (IdVuelo>0 && Pierna>=0 && IdAeronave>0 && IdOrigen>0 && IdDestino>0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT IdTramo, IdOrigen, IdDestino FROM VueloTramo WHERE IdTramo = @idtramo OR (IdVuelo = @idVuelo AND Pierna = @pierna)", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idtramo", IdTramo));
                Cmnd.Parameters.Add(new SqlParameter("@idVuelo", IdVuelo));
                Cmnd.Parameters.Add(new SqlParameter("@pierna", Pierna));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Tramo de Vuelo ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE VueloTramo SET IdVuelo = @idvuelo, Pierna = @pierna, IdAeronave = @idaeronave, NoVuelo = @novuelo, IdOrigen = @idorigen, IdDestino = @iddestino, IdCapitan = @idcapitan, IdCopiloto = @idcopiloto, NivelVuelo = @nivelvuelo, SOB = @sob, Salida = @salida, Llegada = @llegada, PiezasEquipaje = @piezasequipaje, PesoEquipaje = @pesoequipaje, PiezasCarga = @piezascarga, PesoCarga = @pesocarga, CombustibleRampa = @combustiblerampa, CombustibleCargado = @combustiblecargado, CombustibleRemanente = @combustibleremanente, InicioCarga = @iniciocarga, TerminaCarga = @terminacarga, SalidaPlataforma = @salidaplataforma, LlegadaPlataforma = @llegadaplataforma, Despegue = @despegue, Aterrizaje = @aterrizaje, ObservacionesSaldia = @observacionessaldia, ObservacionesLlegada = @observacionesllegada, InicioDescarga = @iniciodescarga, TerminaDescarga = @terminadescarga, IdAlterno1 = @idalterno1, IdAlterno2 = @idalterno2, ItinerarioDespegue = @itinerariodespegue, ItinerarioAterrizaje = @itinerarioaterrizaje, ETA = @eta, IdDestinoOriginal = @iddestinooriginal, IdBitacora = @idbitacora WHERE IdTramo=@idtramo";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO VueloTramo(IdVuelo, Pierna, IdAeronave, NoVuelo, IdOrigen, IdDestino, IdCapitan, IdCopiloto, NivelVuelo, SOB, Salida, Llegada, PiezasEquipaje, PesoEquipaje, PiezasCarga, PesoCarga, CombustibleRampa, CombustibleCargado, CombustibleRemanente, InicioCarga, TerminaCarga, SalidaPlataforma, LlegadaPlataforma, Despegue, Aterrizaje, ObservacionesSaldia, ObservacionesLlegada, InicioDescarga, TerminaDescarga, IdAlterno1, IdAlterno2, ItinerarioDespegue, ItinerarioAterrizaje, ETA, IdDestinoOriginal, IdBitacora) VALUES(@idvuelo, @pierna, @idaeronave, @novuelo, @idorigen, @iddestino, @idcapitan, @idcopiloto, @nivelvuelo, @sob, @salida, @llegada, @piezasequipaje, @pesoequipaje, @piezascarga, @pesocarga, @combustiblerampa, @combustiblecargado, @combustibleremanente, @iniciocarga, @terminacarga, @salidaplataforma, @llegadaplataforma, @despegue, @aterrizaje, @observacionessaldia, @observacionesllegada, @iniciodescarga, @terminadescarga, @idalterno1, @idalterno2, @itinerariodespegue, @itinerarioaterrizaje, @eta, @iddestinooriginal, @idbitacora)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                //DBNull.Value
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idtramo", IdTramo));
                Command.Parameters.Add(new SqlParameter("@idvuelo", IdVuelo));
                Command.Parameters.Add(new SqlParameter("@pierna", Pierna));
                Command.Parameters.Add(new SqlParameter("@idaeronave", IdAeronave));
                Command.Parameters.Add(new SqlParameter("@novuelo", NoVuelo ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idorigen", IdOrigen));
                Command.Parameters.Add(new SqlParameter("@iddestino", IdDestino));
                Command.Parameters.Add(new SqlParameter("@idcapitan", IdCapitan ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idcopiloto", IdCopiloto ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@nivelvuelo", NivelVuelo ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@sob", SOB ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@salida", Salida ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@llegada", Llegada ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@piezasequipaje", PiezasEquipaje ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@pesoequipaje", PesoEquipaje ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@piezascarga", PiezasCarga ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@pesocarga", PesoCarga ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@combustiblerampa", CombustibleRampa ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@combustiblecargado", CombustibleCargado ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@combustibleremanente", CombustibleRemanente ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@iniciocarga", InicioCarga == null ? DBNull.Value : InicioCarga));
                Command.Parameters.Add(new SqlParameter("@terminacarga", TerminaCarga == null ? DBNull.Value : TerminaCarga));
                Command.Parameters.Add(new SqlParameter("@salidaplataforma", SalidaPlataforma == null ? DBNull.Value : SalidaPlataforma));
                Command.Parameters.Add(new SqlParameter("@llegadaplataforma", LlegadaPlataforma == null ? DBNull.Value : LlegadaPlataforma));
                Command.Parameters.Add(new SqlParameter("@despegue", Despegue == null ? DBNull.Value : Despegue));
                Command.Parameters.Add(new SqlParameter("@aterrizaje", Aterrizaje == null ? DBNull.Value : Aterrizaje));
                Command.Parameters.Add(new SqlParameter("@observacionessaldia", string.IsNullOrEmpty(ObservacionesSaldia) ? SqlString.Null : ObservacionesSaldia));
                Command.Parameters.Add(new SqlParameter("@observacionesllegada", string.IsNullOrEmpty(ObservacionesLlegada) ? SqlString.Null : ObservacionesLlegada));
                Command.Parameters.Add(new SqlParameter("@iniciodescarga", InicioDescarga == null ? DBNull.Value : InicioDescarga));
                Command.Parameters.Add(new SqlParameter("@terminadescarga", TerminaDescarga == null ? DBNull.Value : TerminaDescarga));
                Command.Parameters.Add(new SqlParameter("@idalterno1", IdAlterno1 ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idalterno2", IdAlterno2 ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@itinerariodespegue", ItinerarioDespegue == null ? DBNull.Value : ItinerarioDespegue));
                Command.Parameters.Add(new SqlParameter("@itinerarioaterrizaje", ItinerarioAterrizaje == null ? DBNull.Value : ItinerarioAterrizaje));
                Command.Parameters.Add(new SqlParameter("@eta", ETA == null ? DBNull.Value : ETA));
                Command.Parameters.Add(new SqlParameter("@iddestinooriginal", IdDestinoOriginal ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idbitacora", IdBitacora ?? SqlInt32.Null));
                using (TransactionScope scope2 = new TransactionScope()) {
                    RespuestaQuery rInUp = DataBase.Insert(Command);
                    if (rInUp.Valid) {
                        if (Insr) {
                            if (rInUp.IdRegistro == 0) {
                                res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
                                return res;
                            }
                            IdTramo = rInUp.IdRegistro;
                            Valid = true;
                        }
                        else {
                            if (IdDestino != existe.Row.IdDestino) {
                                VueloTramo tra = new VueloTramo(IdVuelo, Pierna + 1);
                                if (tra.Valid) {
                                    tra.IdOrigen = IdDestino;
                                    var rats = tra.Save();
                                    if (!rats.Valid || !string.IsNullOrEmpty(rats.Error)) {
                                        res.Error = $"Se actualizo la pierna correctamente pero no se actualizo el origen del Destino de la siguiente Pierna: (CS.{this.GetType().Name}-Save.Err.04)<br> Error: {rats.Error}";
                                        return res;
                                    }
                                }
                            }
                            var demvlo = VueloDemora.GetDemorasTramo(IdTramo);
                            foreach (var dem in demvlo) {
                                if (!Demoras.Exists(x => x.Id == dem.Id)) {
                                    dem.Delete();
                                }
                            }
                            var jsvlo = VueloJumpSeat.GetVueloJumpSeats(IdTramo);
                            foreach (var js in jsvlo) {
                                if (!JumpSeats.Exists(x => x.Id == js.Id)) {
                                    js.Delete();
                                }
                            }
                        }
                        foreach(var dem in Demoras) {
                            dem.IdTramo = IdTramo;
                            var rSD=dem.Save();
                            if (!rSD.Valid || !string.IsNullOrEmpty(rSD.Error)) {
                                res.Error = $"Error al Registrar la Demora: (CS.{this.GetType().Name}-Save.Err.04)<br> Error: {rSD.Error}";
                                return res;
                            }
                        }
                        foreach (var js in JumpSeats) {
                            js.IdTramo = IdTramo;
                            var rSD = js.Save();
                            if (!rSD.Valid || !string.IsNullOrEmpty(rSD.Error)) {
                                res.Error = $"Error al Registrar el Jump Seat: (CS.{this.GetType().Name}-Save.Err.05)<br> Error: {rSD.Error}";
                                return res;
                            }
                        }
                        if (Pierna == 1) {
                            Command = new SqlCommand("UPDATE Vuelo SET IdCapitan = @idca, IdCopiloto = @idco WHERE IdVuelo = @idv", Conexion);
                            Command.Parameters.Add(new SqlParameter("@idca", IdCapitan ?? SqlInt32.Null));
                            Command.Parameters.Add(new SqlParameter("@idco", IdCopiloto ?? SqlInt32.Null));
                            Command.Parameters.Add(new SqlParameter("@idv", IdVuelo));
                            var rutv = DataBase.Execute(Command);
                            if (!rutv.Valid || !string.IsNullOrEmpty(rutv.Error)) {
                                res.Error = $"No se actualizo la Tripulacion en el Vuelo: (CS.{this.GetType().Name}-Save.Err.06)<br> Error: {rutv.Error}";
                                return res;
                            }
                        }
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br> Error: {rInUp.Error}";
                        return res;
                    }
                    res.Elemento = this;
                    res.Valid = true;
                    scope2.Complete();
                }
            }
            else {
                res.Error = $"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)";
				if (IdVuelo==0) {
                    res.Error += "<br>Falta el Identificador de Vuelo.";
                }
                if (Pierna == 0) {
                    res.Error += "<br>Falta el Numero de Pierna.";
                }
                if (IdAeronave == 0) {
                    res.Error += "<br>Falta la Matricula de la Aeronave.";
                }
                if (IdOrigen == 0) {
                    res.Error += "<br>Falta el Origen.";
                }
                if (IdDestino == 0) {
                    res.Error += "<br>Falta el Destino.";
                }
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Tramo de Vuelo NO se Elimino");
            using (TransactionScope scope = new TransactionScope()) {
                foreach (var dem in Demoras) {
                    var resd = dem.Delete();
                    if (!resd.Valid) {
                        res.Error += $"<br>Error al eliminar la demora.";
                        return res;
                    }
                }
                foreach (var js in JumpSeats) {
                    var resd = js.Delete();
                    if (!resd.Valid) {
                        res.Error += $"<br>Error al eliminar el Jump Seat de {js.Nombre}.";
                        return res;
                    }
                }
                SqlCommand Command = new SqlCommand("DELETE VueloTramo WHERE IdTramo = @id", Conexion);
                Command.Parameters.Add(new SqlParameter("@id", IdTramo));
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
                if (res.Valid) {
                    scope.Complete();
                }
            }
            return res;
        }
        private void SetDatos(SqlCommand Command) {
            RespuestaQuery res = DataBase.Query(Command);
            if (res.Valid) {
                var Registro = res.Row;
                IdTramo = Registro.IdTramo;
                IdVuelo = Registro.IdVuelo;
                Pierna = Registro.Pierna;
                IdAeronave = Registro.IdAeronave;
                NoVuelo = Registro.NoVuelo;
                IdOrigen = Registro.IdOrigen;
                IdDestino = Registro.IdDestino;
                IdCapitan = Registro.IdCapitan;
                IdCopiloto = Registro.IdCopiloto;
                NivelVuelo = Registro.NivelVuelo;
                SOB = Registro.SOB;
                Salida = Registro.Salida;
                Llegada = Registro.Llegada;
                PiezasEquipaje = Registro.PiezasEquipaje;
                PesoEquipaje = Registro.PesoEquipaje;
                PiezasCarga = Registro.PiezasCarga;
                PesoCarga = Registro.PesoCarga;
                CombustibleRampa = Registro.CombustibleRampa;
                CombustibleCargado = Registro.CombustibleCargado;
                CombustibleRemanente = Registro.CombustibleRemanente;
                InicioCarga = Registro.InicioCarga;
                TerminaCarga = Registro.TerminaCarga;
                SalidaPlataforma = Registro.SalidaPlataforma;
                LlegadaPlataforma = Registro.LlegadaPlataforma;
                Despegue = Registro.Despegue;
                Aterrizaje = Registro.Aterrizaje;
                ObservacionesSaldia = Registro.ObservacionesSaldia;
                ObservacionesLlegada = Registro.ObservacionesLlegada;
                InicioDescarga = Registro.InicioDescarga;
                TerminaDescarga = Registro.TerminaDescarga;
                IdAlterno1 = Registro.IdAlterno1;
                IdAlterno2 = Registro.IdAlterno2;
                ItinerarioDespegue = Registro.ItinerarioDespegue;
                ItinerarioAterrizaje = Registro.ItinerarioAterrizaje;
                ETA = Registro.ETA;
                IdDestinoOriginal = Registro.IdDestinoOriginal;
                IdBitacora = Registro.IdBitacora;
                Valid = true;
                Demoras = VueloDemora.GetDemorasTramo(IdTramo);
                JumpSeats = VueloJumpSeat.GetVueloJumpSeats(IdTramo);
                GetAeronave();
                GetOrigen();
                GetDestino();
                GetTripulacion();
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdTramo = 0;
            IdVuelo = 0;
            Pierna = 0;
            IdAeronave = 0;
            NoVuelo = null;
            IdOrigen = 0;
            IdDestino = 0;
            IdCapitan = null;
            IdCopiloto = null;
            NivelVuelo = null;
            SOB = null;
            Salida = null;
            Llegada = null;
            PiezasEquipaje = null;
            PesoEquipaje = null;
            PiezasCarga = null;
            PesoCarga = null;
            CombustibleRampa = null;
            CombustibleCargado = null;
            CombustibleRemanente = null;
            ObservacionesSaldia = null;
            ObservacionesLlegada = null;
            IdAlterno1 = null;
            IdAlterno2 = null;
            IdDestinoOriginal = null;
            IdBitacora = null;
            Valid = false;
        }
        public static List<VueloTramo> GetTramosVuelo(int idVuelo) {
            List<VueloTramo> vuelotramos = new List<VueloTramo>();
            SqlCommand comando = new SqlCommand("SELECT * FROM VueloTramo WHERE IdVuelo = @idVuelo ORDER BY Pierna", Conexion);
            comando.Parameters.Add(new SqlParameter("@idVuelo", idVuelo));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                VueloTramo vuelotramo = JsonConvert.DeserializeObject<VueloTramo>(JsonConvert.SerializeObject(reg));
                vuelotramo.Valid = true;
                vuelotramo.GetAeronave();
                vuelotramo.GetOrigen();
                vuelotramo.GetDestino();
                vuelotramo.GetTripulacion();
                vuelotramos.Add(vuelotramo);
            }
            return vuelotramos;
        }
        public void GetAeronave() {
            Aeronave = new Aircraft(IdAeronave);
            Aeronave.SetModelo();
        }
        public void GetOrigen() {
            Origen = new Aeropuerto(IdOrigen);
        }
        public void GetDestino() {
            Destino = new Aeropuerto(IdDestino);
        }
        public void GetTripulacion() {
            Capitan = new Crew(IdCapitan ?? 0);
            Copiloto = new Crew(IdCopiloto ?? 0);
        }
        public static List<dynamic> GetTimeLine(DateTime desde, DateTime hasta, int? idtramo = null) {
            string cond = "";
            if(idtramo != null) {
                cond = "VT.IdTramo = @idtramo";
			} else {
                cond = "VT.Salida IS NOT NULL AND ItinerarioDespegue IS NOT NULL AND ItinerarioAterrizaje IS NOT NULL AND VT.Salida >= @desde AND VT.Salida <= @hasta";
			}
            string sqlstr = "SELECT VT.IdTramo, VT.IdVuelo, VT.Pierna, VT.IdAeronave, VT.ItinerarioDespegue AS ID,  VT.ItinerarioAterrizaje AS IA, VT.Despegue AS D, VT.Aterrizaje AS A, ";
            sqlstr += "(SELECT A.Matricula FROM BD_MTTO.dbo.Aeronave A WHERE A.IdAeronave = VT.IdAeronave) AS Matricula, ";
            sqlstr += "(SELECT V.Trip FROM Vuelo V WHERE V.IdVuelo = VT.IdVuelo) AS Trip, ";
            sqlstr += "(SELECT V.Cerrado FROM Vuelo V WHERE V.IdVuelo = VT.IdVuelo) AS Cerrado, ";

            sqlstr += "DATEDIFF(MINUTE,ItinerarioDespegue,Despegue) AS MinRDes, DATEDIFF(MINUTE,ItinerarioAterrizaje,Aterrizaje) AS MinRAte, ";
            sqlstr += "dbo.Date_Time2DateTime2(VT.Salida, VT.ItinerarioDespegue, 3) AS ItinerarioDespegue, ";
            //sqlstr += "dbo.Date_Time2DateTime2(VT.Llegada, VT.ItinerarioAterrizaje, 3) AS ItinerarioAterrizaje, ";
            sqlstr += "dbo.Date_Time2DateTime2(IIF(VT.Llegada IS NULL, IIF(VT.ItinerarioDespegue>VT.ItinerarioAterrizaje, DATEADD(DAY, 1, VT.Salida), VT.Salida), VT.Llegada), VT.ItinerarioAterrizaje, 3) AS ItinerarioAterrizaje, ";
            sqlstr += "dbo.Date_Time2DateTime2(VT.Llegada, VT.ETA, 2) AS ETA, ";
            sqlstr += "dbo.Date_Time2DateTime2(VT.Salida, VT.SalidaPlataforma, 2) AS SalidaPlataforma, ";
            sqlstr += "dbo.Date_Time2DateTime2(VT.Llegada, VT.LlegadaPlataforma, 1) AS LlegadaPlataforma, ";
            sqlstr += "dbo.Date_Time2DateTime2(VT.Salida, VT.Despegue, 3) AS Despegue, ";
            //sqlstr += "dbo.Date_Time2DateTime2(VT.Llegada, VT.Aterrizaje, 3) AS Aterrizaje ";
            sqlstr += "dbo.Date_Time2DateTime2(IIF(VT.Llegada IS NULL, IIF(VT.Despegue>VT.Aterrizaje, DATEADD(DAY, 1, VT.Salida), VT.Salida), VT.Llegada), VT.Aterrizaje, 3) AS Aterrizaje ";
            sqlstr += $"FROM VueloTramo VT WHERE {cond} ORDER BY Matricula";
            SqlCommand comando = new SqlCommand(sqlstr, Conexion);
            comando.Parameters.Add(new SqlParameter("@desde", desde));
            comando.Parameters.Add(new SqlParameter("@hasta", hasta));
            comando.Parameters.Add(new SqlParameter("@idtramo", idtramo ?? SqlInt32.Null));
            RespuestaQuery res = DataBase.Query(comando);
            List<dynamic> datos = new List<dynamic>();
            var idx = 0;
			foreach (var reg in res.Rows) {
                //      ITINERARIOS
                if (reg.ItinerarioDespegue == null || reg.ItinerarioAterrizaje == null) continue;
                var oi = new { start = reg.ItinerarioDespegue, end = reg.ItinerarioAterrizaje, className = $"text-center sz-font-725 thead-blue", content = $"{reg.Trip} - {reg.Pierna}", group = reg.Matricula, editable = false, type = "range", idC = idx, tipo = 1, pierna = reg };
                datos.Add(oi);
                idx++;
                //      VUELOS
                string classname = reg.MinRDes > 15 ? "bg-warning" : reg.MinRAte > 15 ? "bg-danger" : "bg-success";
                dynamic ov = null;
                if(reg.Despegue!=null && reg.Aterrizaje != null) {
                    ov = new { start = reg.Despegue, end = reg.Aterrizaje, className = $"text-center sz-font-725 {classname}", content = $"{reg.Trip} - {reg.Pierna}", group = reg.Matricula, editable = false, type = "range", idC = idx, tipo = 0, pierna = reg };
				} else if (reg.Despegue != null) {
                    ov = new { start = reg.Despegue, className = $"text-center sz-font-725 {classname}", content = $"{reg.Trip} - {reg.Pierna}", group = reg.Matricula, editable = false, type = "floatingRange", idC = idx, tipo = -1, pierna = reg };
                } else {
                    continue;
				}
				if (ov != null) {
                    datos.Add(ov);
                    idx++;
				}
            }
            return datos;
            //return res.Rows;
        }
        public Vuelo GetVuelo() {
            return new Vuelo(IdVuelo);
		}
    }
}