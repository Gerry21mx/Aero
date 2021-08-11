using ATSM.Seguimiento;
using ATSM.Tripulaciones;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class Aircraft {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; private set; }
		public string Matricula { get; set; }
		private int? _idEmp { get; set; }
		public int? IdEmpresa {
			get {
				return _idEmp;
			}
			set {
				Empresa = new Empresa(value ?? 0);
				_idEmp = Empresa.Valid ? value : 0;
			}
		}
		public Empresa Empresa { get; set; }
		public int? JumpSeat { get; set; }
		public int? Pasajeros { get; set; }
		public string? Seguro { get; set; }
		public bool Estado { get; set; }
		public bool Valid { get; set; }
		public Modelo Modelo { get; set; }
		public List<ItemMayor> Mayores { get; set; }
		public Aircraft(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Aircraft WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Aircraft(string matricula) {
            Inicializar();
            if (!string.IsNullOrEmpty(matricula)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Aircraft WHERE Matricula = @matricula", Conexion);
                comando.Parameters.Add(new SqlParameter("@matricula", matricula));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Aircraft(int id, string matricula, int? idEmpresa = null, int? jumpSeat = null, int? pasajeros = null, string seguro = "", bool estado = false, bool valid = false) {
            Id = id;
            Matricula = matricula;
            IdEmpresa = idEmpresa;
            JumpSeat = jumpSeat;
            Pasajeros = pasajeros;
            Seguro = seguro;
            Estado = estado;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Matricula)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Aircraft WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Aircraft ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Aircraft SET Matricula = @matricula, IdEmpresa = @idempresa, JumpSeat = @jumpseat, Pasajeros = @pasajeros, Seguro = @seguro, Estado = @estado WHERE Id = @id"; res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Aircraft(Matricula, IdEmpresa, JumpSeat, Pasajeros, Seguro, Estado) VALUES(@matricula, @idempresa, @jumpseat, @pasajeros, @seguro, @estado)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@matricula", string.IsNullOrEmpty(Matricula) ? SqlString.Null : Matricula));
                Command.Parameters.Add(new SqlParameter("@idempresa", IdEmpresa ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@jumpseat", JumpSeat ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@pasajeros", Pasajeros ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@seguro", string.IsNullOrEmpty(Seguro) ? SqlString.Null : Seguro));
                Command.Parameters.Add(new SqlParameter("@estado", Estado));
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
                Empresa = new Empresa(IdEmpresa);
                res.Elemento = this;
                res.Valid = true;
            }
            else {
                if (!string.IsNullOrEmpty(Matricula))
                    res.Error += $"<br>Falta la Matricula de la Aeronave";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Aircraft NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Aircraft WHERE Id = @id", Conexion);
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
                Matricula = Registro.Matricula;
                IdEmpresa = Registro.IdEmpresa;
                JumpSeat = Registro.JumpSeat;
                Pasajeros = Registro.Pasajeros;
                Seguro = Registro.Seguro;
                Estado = Registro.Estado;
                Valid = true;
                SetMayores();
                SetModelo();
            }
        }
        private void Inicializar() {
            Id = 0;
            Matricula = "";
            IdEmpresa = null;
            JumpSeat = null;
            Pasajeros = null;
            Seguro = "";
            Estado = false;
            Valid = false;
            Modelo = new Modelo();
            Mayores = new List<ItemMayor>();
        }
        public static List<Aircraft> GetAircrafts() {
            List<Aircraft> aircrafts = new List<Aircraft>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Aircraft", Conexion));
            foreach (var reg in res.Rows) {
                Aircraft aircraft = JsonConvert.DeserializeObject<Aircraft>(JsonConvert.SerializeObject(reg));
                aircraft.Valid = true;
                aircrafts.Add(aircraft);
            }
            return aircrafts;
        }
        public void SetMayores() {
            Mayores = ItemMayor.GetItemMayoresByAircraft(Id);
            foreach(var may in Mayores) {
                may.SetComponente();
                may.SetPosicion();
			}
		}
        public void SetModelo() {
            Modelo = new Modelo();
            if (Mayores == null)
                SetMayores();
            ItemMayor IM = Mayores.Find(item => { return item.IdComponente == 1; });
			if (IM != null) {
                IM.SetModelo();
                Modelo = IM.Modelo;
			}
        }
        public Vuelo PrimerVuelo() {
            Vuelo vuelo = new Vuelo();
            SqlCommand comando = new SqlCommand("SELECT TOP 1 IdVuelo FROM VueloTramo WHERE IdAeronave = @ida ORDER BY Salida", DataBase.Conexion("Seguimiento"));
            comando.Parameters.Add(new SqlParameter("@ida", Id));
            var rQuery = DataBase.Query(comando);
            if (rQuery.Valid) {
                vuelo = new Vuelo(rQuery.Row.IdVuelo);
            }
            return vuelo;
        }
        public Vuelo UltimoVuelo() {
            Vuelo vuelo = new Vuelo();
            SqlCommand comando = new SqlCommand("SELECT TOP 1 IdVuelo FROM VueloTramo WHERE IdAeronave = @ida ORDER BY Salida DESC", DataBase.Conexion("Seguimiento"));
            comando.Parameters.Add(new SqlParameter("@ida", Id));
            var rQuery = DataBase.Query(comando);
            if (rQuery.Valid) {
                vuelo = new Vuelo(rQuery.Row.IdVuelo);
            }
            return vuelo;
        }
        public VueloTramo UltimoTramo() {
            VueloTramo tramo = new VueloTramo(0);
            SqlCommand comando = new SqlCommand("SELECT TOP 1 * FROM VueloTramo WHERE IdAeronave=@ida ORDER BY dbo.Date_Time2DateTime2(Salida,Despegue,0) DESC, IdVuelo DESC, Pierna DESC", DataBase.Conexion());
            comando.Parameters.Add(new SqlParameter("@ida", Id));
            var rQuery = DataBase.Query(comando);
            if (rQuery.Valid) {
                tramo = JsonConvert.DeserializeObject<VueloTramo>(JsonConvert.SerializeObject(rQuery.Row));
                tramo.GetAeronave();
                tramo.GetOrigen();
                tramo.GetDestino();
                tramo.GetTripulacion();
                tramo.Valid = true;
            }
            return tramo;
        }
        public (List<Crew> Capitanes, List<Crew> Copilotos) GetTripulacion() {
            List<Crew> Capitanes = new List<Crew>();
            List<Crew> Copilotos = new List<Crew>();
            if (Modelo == null) {
                Modelo.SetCapacidad();
            }
            if (Modelo.Capacidad == null || !Modelo.Capacidad.Valid) {
                return new(Capitanes, Copilotos);
            }
            var crews = Crew.GetCrew(Modelo.IdCapacidad);
            foreach (var crew in crews) {
                if (crew.IdCapacidad_1 == Modelo.IdCapacidad) {
                    if (crew.Nivel_1 == 1) {
                        Capitanes.Add(crew);
                    }
                    else {
                        Copilotos.Add(crew);
                    }
                }
                else if (crew.IdCapacidad_2 == Modelo.IdCapacidad) {
                    if (crew.Nivel_2 == 1) {
                        Capitanes.Add(crew);
                    }
                    else {
                        Copilotos.Add(crew);
                    }
                }
                else if (crew.IdCapacidad_3 == Modelo.IdCapacidad) {
                    if (crew.Nivel_3 == 1) {
                        Capitanes.Add(crew);
                    }
                    else {
                        Copilotos.Add(crew);
                    }
                }
            }
            return new(Capitanes, Copilotos);
        }
        public void ActualizarTAT_Mayores() {
            SqlCommand comando = new SqlCommand("SELECT ISNULL(SUM(DATEPART(MINUTE, [Tiempo]) + DATEPART(HOUR, [Tiempo]) * 60), 0) AS MV,  COUNT(*) AS CV FROM BitacoraTramos WHERE IdBitacora IN (SELECT Id FROM Bitacoras WHERE IdAircraft = @idAircraft)", Conexion);
            comando.Parameters.AddWithValue("@idAircraft", Id);
            var res = DataBase.Query(comando);
            if (res.Valid) {
                int MV = (int)res.Row.MV;
                int CV = (int)res.Row.CV;
                if (Mayores.Count <= 0) {
                    SetMayores();
                }
                ItemMayor aircraftActuales = Mayores.Find(im => { return im.IdComponente == 1; });
                if (aircraftActuales.Valid) {
                    aircraftActuales.TSN = (aircraftActuales.TSN_Airframe_Instalacion ?? 0) + (MV / 60);
                    aircraftActuales.CSN = (aircraftActuales.CSN_Airframe_Instalacion ?? 0) + CV;
                    aircraftActuales.Save();
                    foreach (var mayor in Mayores) {
                        if (mayor.IdComponente != 3 && mayor.IdComponente != 1) {
                            mayor.TSN = (mayor.TSN_Componente_Instalacion ?? 0) + (aircraftActuales.TSN ?? 0) - (mayor.TSN_Airframe_Instalacion ?? 0);
                            mayor.CSN = (mayor.CSN_Componente_Instalacion ?? 0) + (aircraftActuales.CSN ?? 0) - (mayor.CSN_Airframe_Instalacion ?? 0);
                            mayor.Save();

                        }
                    }
                }

            }
        }
        public dynamic Recalcular() {
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();
            ActualizarTAT_Mayores();
            if (Mayores.Count <= 0) {
                SetMayores();
            }
            RespuestaQuery res = new RespuestaQuery();
            int afectados = 0;
            foreach (var mayor in Mayores) {
                if (mayor.IdComponente != 3) {
                    //      TSN/CSN Mayores
                    string sqlStr = $"UPDATE ItemMenor SET TSN = ISNULL(TSN_Componente_Instalacion, 0) + (SELECT ISNULL(TSN, 0) FROM ItemMayor WHERE Id = IdItemMayor) - TSN_Airframe_Instalacion, CSN = ISNULL(CSN_Componente_Instalacion, 0) + (SELECT ISNULL(CSN, 0) FROM ItemMayor WHERE Id = IdItemMayor) - CSN_Airframe_Instalacion WHERE IdItemMayor = @idItemMayor;";
                    //      Tiempos Transcurridos y Remanentes
                    sqlStr += $"UPDATE Tiempos SET ";
                    sqlStr += $"Horas_Elapsed = ISNULL((SELECT TSN FROM ItemMayor WHERE Id = @idItemMayor), 0) - ISNULL(Horas_Last, 0) + ISNULL(Horas_TSO_Instalacion, 0)";
                    sqlStr += $", Ciclos_Elapsed = ISNULL((SELECT CSN FROM ItemMayor WHERE Id = @idItemMayor), 0) - ISNULL(Ciclos_Last, 0) + ISNULL(Ciclos_TSO_Instalacion, 0)";
                    sqlStr += $", Dias_Elapsed = DATEDIFF(DAY, ISNULL(Fecha_Last, GETDATE()), GETDATE()) + ISNULL(Dias_TSO_Instalacion, 0)";

                    sqlStr += $", Horas_Next = ISNULL(Horas_Last, 0) + ISNULL(IIF(ISNULL(Limite_Individual_Horas, 0)>0, ISNULL(Limite_Individual_Horas, 0), (SELECT Horas FROM Limite WHERE Id = Tiempos.IdLimite)), 0) - ISNULL(Horas_TSO_Instalacion, 0)";
                    sqlStr += $", Ciclos_Next = ISNULL(Ciclos_Last, 0) + ISNULL(IIF(ISNULL(Limite_Individual_Ciclos, 0)>0, ISNULL(Limite_Individual_Ciclos, 0), (SELECT Ciclos FROM Limite WHERE Id = Tiempos.IdLimite)), 0) - ISNULL(Ciclos_TSO_Instalacion, 0)";
                    sqlStr += $", Fecha_Next = IIF(Fecha_Last IS NULL, NULL, DATEADD(DAY, ISNULL(IIF(ISNULL(Limite_Individual_Dias, 0)>0, ISNULL(Limite_Individual_Dias, 0), (SELECT Dias FROM Limite WHERE Id = Tiempos.IdLimite)), 0), Fecha_Last))";

                    sqlStr += $", Horas_Remain = ISNULL(Horas_Next, 0) - ISNULL((SELECT TSN FROM ItemMayor WHERE Id = @idItemMayor), 0)";
                    sqlStr += $", Ciclos_Remain = ISNULL(Ciclos_Next, 0) - ISNULL((SELECT CSN FROM ItemMayor WHERE Id = @idItemMayor), 0)";
                    sqlStr += $", Dias_Remain = DATEDIFF(DAY, GETDATE(), Fecha_Next)";
                    sqlStr += $" WHERE IdItemMayor = @idItemMayor OR IdItemMenor IN (SELECT Id FROM ItemMenor WHERE IdItemMayor = @idItemMayor);";

                    SqlCommand comando = new SqlCommand(sqlStr, Conexion);
                    comando.Parameters.AddWithValue("@idItemMayor", mayor.Id);
                    res = DataBase.Query(comando);
                    if(!res.Valid || !string.IsNullOrEmpty(res.Error))
                        Console.WriteLine("Error");
					else
                        afectados += res.Afectados;
                }
            }
            timeMeasure.Stop();
            return new { Afectados = afectados, Milisegundos = timeMeasure.ElapsedMilliseconds };
        }
    }
}