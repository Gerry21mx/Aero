using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;

using ATSM.Ingenieria;
using ATSM.Tripulaciones;

using Newtonsoft.Json;

namespace ATSM.Seguimiento {
	public class Vuelo {
        private static SqlConnection Conexion = DataBase.Conexion();
        public int IdVuelo { get; set; }
        public string Trip { get; set; }
        public string CodigoVuelo { get; set; }
        public int? IdTipoVuelo { get; set; }
        public int? NoVuelo { get; set; }
        public bool Cerrado { get; set; }
        public int? IdRuta { get; set; }
        public DateTime? Salida { get; set; }
        public int? IdCapitan { get; set; }
        public int? IdCopiloto { get; set; }
        public bool Valid { get; set; }
        public TipoVuelo Tipo { get; set; }
        public Ruta Ruta { get; set; }
        public Crew Capitan { get; set; }
        public Crew Copiloto { get; set; }
        public Aircraft Aeronave { get; set; }
        public List<VueloTramo> Tramos = new List<VueloTramo>();
        public Vuelo(int? idvuelo = null) {
            Inicializar();
            if (idvuelo > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Vuelo WHERE IdVuelo = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", idvuelo));
                SetDatos(comando);
            }
        }
        public Vuelo(string trip) {
            Inicializar();
            if (!string.IsNullOrEmpty(trip)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Vuelo WHERE Trip = @trip", Conexion);
                comando.Parameters.Add(new SqlParameter("@trip", trip));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Vuelo(int idVuelo, string trip, string codigoVuelo = null, int? idTipoVuelo = null, int? noVuelo = null, bool cerrado = false, int? idRuta = null, DateTime? salida = null, int? idCapitan = null, int? idCopiloto = null, bool valid = false) {
            IdVuelo = idVuelo;
            Trip = trip;
            CodigoVuelo = codigoVuelo;
            IdTipoVuelo = idTipoVuelo;
            NoVuelo = noVuelo;
            Cerrado = cerrado;
            IdRuta = idRuta;
            Salida = salida;
            IdCapitan = idCapitan;
            IdCopiloto = idCopiloto;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Trip)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT IdVuelo, Cerrado FROM Vuelo WHERE IdVuelo = @idvuelo OR Trip = @trip", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idvuelo", IdVuelo));
                Cmnd.Parameters.Add(new SqlParameter("@trip", Trip));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Vuelo ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
					if (existe.Row.Cerrado) {
                        res.Mensaje = "El Vuelo YA Esta Cerrado y NO se puede modificar.";
                        return res;
					}
                    SqlStr = @"UPDATE Vuelo SET Trip = @trip, CodigoVuelo = @codigovuelo, IdTipoVuelo = @idtipovuelo, NoVuelo = @novuelo, Cerrado = @cerrado, IdRuta = @idruta, Salida = @salida, IdCapitan = @idcapitan, IdCopiloto = @idcopiloto WHERE IdVuelo=@idvuelo";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Vuelo(Trip, CodigoVuelo, IdTipoVuelo, NoVuelo, Cerrado, IdRuta, Salida, IdCapitan, IdCopiloto) VALUES(@trip, @codigovuelo, @idtipovuelo, @novuelo, @cerrado, @idruta, @salida, @idcapitan, @idcopiloto)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                if (Tramos.Count > 0) {
					if (Tramos[0].Aeronave == null) {
                        Tramos[0].GetAeronave();
                    }
                    CodigoVuelo = Tramos[0].Aeronave.Empresa.CodigoVuelo;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idvuelo", IdVuelo));
                Command.Parameters.Add(new SqlParameter("@trip", Trip));
                Command.Parameters.Add(new SqlParameter("@codigovuelo", string.IsNullOrEmpty(CodigoVuelo) ? SqlString.Null : CodigoVuelo));
                Command.Parameters.Add(new SqlParameter("@idtipovuelo", IdTipoVuelo ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@novuelo", NoVuelo ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@cerrado", Cerrado));
                Command.Parameters.Add(new SqlParameter("@idruta", IdRuta ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@salida", Salida ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@idcapitan", IdCapitan ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idcopiloto", IdCopiloto ?? SqlInt32.Null));
                using (TransactionScope scope = new TransactionScope()) {
                    RespuestaQuery rInUp = DataBase.Insert(Command);
                    if (rInUp.Valid) {
                        if (Insr) {
                            if (rInUp.IdRegistro == 0) {
                                res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
                                return res;
                            }
                            IdVuelo = rInUp.IdRegistro;
                            Valid = true;
                        }
                        var trantes = VueloTramo.GetTramosVuelo(IdVuelo);
                        foreach (var trAnt in trantes) {
                            var idx = Tramos.FindIndex(i => i.IdTramo == trAnt.IdTramo);
                            if (idx == -1) {
                                var rsDel = trAnt.Delete();
                                if (!rsDel.Valid || !string.IsNullOrEmpty(rsDel.Error)) {
                                    res.Error = $"Error al Eliminar Tramo de Vuelo: (CS.{this.GetType().Name}-Save.Err.04)<br> Error: {rsDel.Error}";
                                    return res;
                                }
                            }
                        }
                        foreach (var tramo in Tramos) {
                            tramo.IdVuelo = IdVuelo;
                            var rSt = tramo.Save();
                            if (!rSt.Valid || !string.IsNullOrEmpty(rSt.Error)) {
                                res.Error = $"Error al Registrar Tramo de Vuelo: (CS.{this.GetType().Name}-Save.Err.05)<br> Error: {res.Error}";
                                return res;
                            }
                        }
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br> Error: {rInUp.Error}";
                        return res;
                    }
                    GetTipoVuelo();
                    GetRuta();
                    GetTripulacion();
                    GetAeronave();
                    res.Elemento = this;
                    res.Valid = true;
                    scope.Complete();
                }
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Vuelo NO se Elimino");
            /// GASTOS
            SqlCommand Command = new SqlCommand("SELECT IdComprobacion WHERE IdVuelo = @idvuelo", Conexion);
            Command.Parameters.Add(new SqlParameter("@idvuelo", IdVuelo));
            var rComp = DataBase.Query(Command);
			if (rComp.Valid) {
                res.Error += "<br>Existe un registro de Comprobacion de Gastos de Piloto por lo que no se Puede eliminar el Vuelo.";
                return res;
			}
            using (TransactionScope scope = new TransactionScope()) {
                foreach (var tra in Tramos) {
                    var resd=tra.Delete();
                    if (!resd.Valid) {
                        res.Error += $"<br>Error al eliminar el Tramo {tra.Pierna}.";
                        return res;
				    }
			    }
                Command = new SqlCommand("DELETE Vuelo WHERE IdVuelo = @id", Conexion);
                Command.Parameters.Add(new SqlParameter("@id", IdVuelo));
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
        public Respuesta Close() {
            Respuesta res = new Respuesta();
            SqlCommand comando = new SqlCommand("UPDATE Vuelo SET Cerrado = @cerrado WHERE IdVuelo = @idvuelo", Conexion);
            comando.Parameters.Add(new SqlParameter("@cerrado", Cerrado));
            comando.Parameters.Add(new SqlParameter("@idvuelo", IdVuelo));
            var r = DataBase.Query(comando);
            res.Valid = r.Valid;
            res.Mensaje = $"Vuelo {(Cerrado ? "Cerrado" : "Abierto")}";
            if(!r.Valid || !string.IsNullOrEmpty(r.Error)) {
                res.Error = r.Error;
			}
            return res;
		}
        private void SetDatos(SqlCommand Command) {
            RespuestaQuery res = DataBase.Query(Command);
            if (res.Valid) {
                var Registro = res.Row;
                IdVuelo = Registro.IdVuelo;
                Trip = Registro.Trip;
                CodigoVuelo = Registro.CodigoVuelo;
                IdTipoVuelo = Registro.IdTipoVuelo;
                NoVuelo = Registro.NoVuelo;
                Cerrado = Registro.Cerrado;
                IdRuta = Registro.IdRuta;
                Salida = Registro.Salida;
                IdCapitan = Registro.IdCapitan;
                IdCopiloto = Registro.IdCopiloto;
                Valid = true;
                Tramos = VueloTramo.GetTramosVuelo(IdVuelo);
                GetTipoVuelo();
                GetRuta();
                GetTripulacion();
                GetAeronave();
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdVuelo = 0;
            Trip = "";
            CodigoVuelo = null;
            IdTipoVuelo = null;
            NoVuelo = null;
            Cerrado = false;
            IdRuta = null;
            Salida = null;
            IdCapitan = null;
            IdCopiloto = null;
            Valid = false;
            Tipo = new TipoVuelo();
            Ruta = new Ruta();
            Capitan = new Crew();
            Copiloto = new Crew();
            Aeronave = new Aircraft();
        }
        public void GetTipoVuelo() {
            Tipo = new TipoVuelo(IdTipoVuelo);
        }
        public void GetRuta() {
            Ruta = new Ruta(IdRuta ?? 0);
        }
        public void GetTripulacion() {
            Capitan = new Crew(IdCapitan ?? 0);
            Copiloto = new Crew(IdCopiloto ?? 0);
        }
        public void GetAeronave() {
            SqlCommand comando = new SqlCommand($"SELECT TOP 1 IdAeronave FROM VueloTramo WHERE IdVuelo = @idv ORDER BY Pierna", Conexion);
            comando.Parameters.Add(new SqlParameter("@idv", IdVuelo));
            RespuestaQuery res = DataBase.Query(comando);
            if(res.Valid && res.Afectados > 0) {
                Aeronave = new Aircraft(res.Row.IdAeronave);
            }
        }
        public static List<Vuelo> GetVuelos(DateTime? desde = null, DateTime? hasta = null, bool cerrado = false) {
            List<Vuelo> vuelos = new List<Vuelo>();
            SqlCommand comando = new SqlCommand($"SELECT * FROM Vuelo WHERE Cerrado = @cerrado{(desde!=null?" AND Salida >= @desde":"")}{(hasta != null ? " AND Salida <= @hasta" : "")}", Conexion);
            comando.Parameters.Add(new SqlParameter("@cerrado", cerrado));
            comando.Parameters.Add(new SqlParameter("@desde", desde ?? SqlDateTime.Null));
            comando.Parameters.Add(new SqlParameter("@hasta", hasta ?? SqlDateTime.Null));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                Vuelo vuelo = JsonConvert.DeserializeObject<Vuelo>(JsonConvert.SerializeObject(reg));
                vuelo.Valid = true;
                vuelo.GetTipoVuelo();
                vuelo.GetRuta();
                vuelo.GetTripulacion();
                vuelo.GetAeronave();
                vuelos.Add(vuelo);
            }
            return vuelos;
        }
        /// <summary>
        /// Genera consulta del reporte en base a los filtros seleccionados.
        /// Consulta la Vista "Vuelos"
        /// </summary>
        /// <param name="trip">Trip de Vuelo</param>
        /// <param name="idaeronave">Id de la Matricula de la Aeronave</param>
        /// <param name="desde">Desde la Fecha basado en el Campo de Salida del Vuelo</param>
        /// <param name="hasta">Hasta la Fecha basado en el Campo de Salida del Vuelo</param>
        /// <param name="idtipovuelo">Tipo de Vuelo</param>
        /// <param name="idorigen">Partiendo del Aeropueto de Origen</param>
        /// <param name="iddestino">Llegando al Aeropuerto de Destino</param>
        /// <param name="cerrado">Vuelos Cerrados/Abiertos o Todos si el Parametro es Null</param>
        /// <param name="tipo_reporte">
        ///     <list type="bullet">
        ///     <item>false - Resumen</item>
        ///     <item>true - Desglose</item>
        ///     </list>
        /// </param>
        /// <returns></returns>
        public static List<dynamic> GetReporte(string trip=null, int? idaeronave=null, DateTime? desde = null, DateTime? hasta = null, int? idtipovuelo = null, int? idorigen = null, int? iddestino = null, int? idcapitan = null, int? idcopiloto = null, int? iddemora = null, int? idruta = null, int? idcapacidad = null, int? estado = null, bool tipo_reporte = false) {
            string ce = "(SELECT A.Matricula FROM BD_MTTO.dbo.Aeronave A WHERE A.IdAeronave = V.IdAeronave) AS Matricula, ";
            ce += "(SELECT TV.Descripcion FROM TipoVuelo TV WHERE TV.IdTipo = V.IdTipoVuelo) AS TipoVuelo,  ";
            ce += "(SELECT R.Codigo FROM Ruta R WHERE R.IdRuta = V.IdRuta) AS Ruta,  ";
            ce += "(SELECT AO.IATA FROM Aeropuerto AO WHERE AO.IdAeropuerto = V.IdOrigen) AS Origen, ";
            ce += "(SELECT AD.IATA FROM Aeropuerto AD WHERE AD.IdAeropuerto = V.IdDestino) AS Destino, ";
            ce += "(SELECT TU.Nombre_Personal + ' ' + TU.Apellidos_Personal FROM GTSM.dbo.Table_Usuarioss TU WHERE TU.Id_Personal = V.IdCapitan) AS Capitan, ";
            ce += "(SELECT TU.Nombre_Personal + ' ' + TU.Apellidos_Personal FROM GTSM.dbo.Table_Usuarioss TU WHERE TU.Id_Personal = V.IdCopiloto) AS Copiloto ";
            string sqlstr = $"SELECT *, {ce} FROM Vuelos_Desglose V WHERE ";
            if (!tipo_reporte) {
                sqlstr = $"SELECT *, {ce} FROM Vuelos_Resumen V WHERE ";
            }
            if (!string.IsNullOrEmpty(trip)) {
                sqlstr += "Trip = @trip";
			} else {
                if (desde != null) {
                    sqlstr += "Salida >= @desde AND ";
                }
                if (hasta != null) {
                    sqlstr += "Salida <= @hasta AND ";
                }
                if (idaeronave > 0) {
                    sqlstr += "IdAeronave = @idaeronave AND ";
                }
                if (idtipovuelo > 0) {
                    sqlstr += "IdTipoVuelo = @idtipovuelo AND ";
                }
                if (idorigen > 0) {
                    sqlstr += "IdOrigen = @idorigen AND ";
                }
                if (iddestino > 0) {
                    sqlstr += "IdDestino = @iddestino AND ";
                }
                if (idcapitan > 0) {
                    sqlstr += "IdCapitan = @idcapitan AND ";
                }
                if (idcopiloto > 0) {
                    sqlstr += "IdCopiloto = @idcopiloto AND ";
                }
                if (iddemora > 0) {
                    sqlstr += "IdTramo IN (SELECT VD.IdTramo FROM VueloDemora VD WHERE VD.IdDemora = @iddemora) AND ";
                }
                if (idruta > 0) {
                    sqlstr += "IdRuta = @idruta AND ";
                }
                if (idcapacidad > 0) {
                    sqlstr += "(SELECT id FROM catCap WHERE cap = (SELECT capacidad FROM BD_DESPACHO.dbo.modelo_avion WHERE IdModeloAvion=(SELECT IdModelo FROM BD_MTTO.dbo.Aeronave WHERE IdAeronave = V.IdAeronave))) = @idcapacidad AND ";
                }
                if (estado != null) {
                    sqlstr += "Cerrado = @cerrado AND ";
                }
                sqlstr = sqlstr.Trim();
                string[] subs = sqlstr.Split(' ');
				if (subs[subs.Length - 1] == "WHERE") {
                    sqlstr = sqlstr.Replace("WHERE", "");
                } else if (subs[subs.Length - 1] == "AND") {
                    sqlstr = sqlstr.Substring(0, sqlstr.LastIndexOf("AND"));
                }
            }
            sqlstr += " ORDER BY dbo.Date_Time2DateTime2(Salida,SalidaPlataforma,0)";
            SqlCommand comando = new SqlCommand(sqlstr, Conexion);
            comando.Parameters.Add(new SqlParameter("@trip", string.IsNullOrEmpty(trip) ? SqlDateTime.Null : trip));
            comando.Parameters.Add(new SqlParameter("@idaeronave", idaeronave ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@desde", desde ?? SqlDateTime.Null));
            comando.Parameters.Add(new SqlParameter("@hasta", hasta ?? SqlDateTime.Null));
            comando.Parameters.Add(new SqlParameter("@idtipovuelo", idtipovuelo ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@idorigen", idorigen ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@iddestino", iddestino ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@idcapitan", idcapitan ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@idcopiloto", idcopiloto ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@iddemora", iddemora ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@idruta", idruta ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@idcapacidad", idcapacidad ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@cerrado", estado ?? SqlInt32.Null));
            RespuestaQuery res = DataBase.Query(comando);
            return res.Rows.ToList();
        }
        public static List<dynamic> GetEmisiones(DateTime desde, DateTime hasta, int tipo = 1) {
            List<dynamic> reporte = new List<dynamic>();

            SqlCommand comando = new SqlCommand($@"SELECT DISTINCT IdOrigen, IdDestino, 
    (SELECT IdCapacidad FROM BD_MTTO.dbo.ModeloAeronave WHERE IdModelo = (SELECT IdModelo FROM BD_MTTO.dbo.Aeronave WHERE IdAeronave = VueloTramo.IdAeronave)) AS Capacidad, 
    (SELECT IIF(Pais='MX',1,0) FROM Aeropuerto WHERE Aeropuerto.IdAeropuerto = VueloTramo.IdOrigen)+(SELECT IIF(Pais='MX',1,0) FROM Aeropuerto WHERE Aeropuerto.IdAeropuerto = VueloTramo.IdDestino) AS NaIn, 
    (SELECT (mil * 1.852) AS km FROM BD_ASTRIP.dbo.rutvue WHERE ori = (SELECT IATA FROM Aeropuerto WHERE IdAeropuerto=IdOrigen) AND des = (SELECT IATA FROM Aeropuerto WHERE IdAeropuerto=IdDestino)) AS DKM
        FROM VueloTramo WHERE Salida BETWEEN @desde AND @hasta AND IdOrigen IS NOT NULL AND IdDestino IS NOT NULL", Conexion);
            comando.Parameters.Add(new SqlParameter("@desde", desde));
            comando.Parameters.Add(new SqlParameter("@hasta", hasta));
            var res = DataBase.Query(comando);
            foreach(var tra in res.Rows) {

			}

            return reporte;
		}
    }
}