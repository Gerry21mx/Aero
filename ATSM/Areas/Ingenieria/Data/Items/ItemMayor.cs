using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ATSM.Ingenieria {
	public class ItemMayor {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdComponente { get; set; }
		public int? IdAircraft { get; set; }
		public int IdModelo { get; set; }
		public int? IdPosicion { get; set; }
		private int _idEstadoMayor { get; set; }
		public int IdEstadoMayor {
			get {
				return _idEstadoMayor;
			}
			set {
				Estado = new EstadoMayor(value);
				_idEstadoMayor = Estado.Id;
			}
		}
		public string Serie { get; set; }
		public string Capacity { get; set; }
		public decimal? TSN { get; set; }
		public int? CSN { get; set; }
		public int? Anio { get; set; }
		public decimal? TSN_Componente_Instalacion { get; set; }
		public int? CSN_Componente_Instalacion { get; set; }
		public DateTime? Fecha_Componente_Instalacion { get; set; }
		public decimal? TSN_Airframe_Instalacion { get; set; }
		public int? CSN_Airframe_Instalacion { get; set; }
		public DateTime? Fecha_Airframe_Instalacion { get; set; }
        public List<Limite> Limites { get; set; }
		public List<Tiempos> Tiempos { get; set; }
		public bool Valid { get; set; }
        public Modelo Modelo { get; set; }
        public Position Posicion { get; set; }
		public EstadoMayor Estado { get; set; }
		public ComponenteMayor Componente { get; set; }
		public Aircraft Aircraft { get; set; }
        public ItemMayor(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ItemMayor WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public ItemMayor(int idMayor, int idModelo, string serie) {
            Inicializar();
            if (idMayor >0 && idModelo > 0 && !string.IsNullOrEmpty(serie)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ItemMayor WHERE IdComponente = @idComponente AND IdModelo = @idModelo AND Serie = @serie", Conexion);
                comando.Parameters.Add(new SqlParameter("@idComponente", IdComponente));
                comando.Parameters.Add(new SqlParameter("@idModelo", IdModelo));
                comando.Parameters.Add(new SqlParameter("@serie", Serie));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public ItemMayor(int id, int idComponente, int? idAircraft = null, int idModelo = 0, int? idPosicion = null, int idEstadoMayor = 0, string serie = "", string capacity = "", decimal? tSN = null, int? cSN = null, int? anio = null, decimal? tSN_Componente_Instalacion = null, int? cSN_Componente_Instalacion = null, DateTime? fecha_Componente_Instalacion = null, decimal? tSN_Airframe_Instalacion = null, int? cSN_Airframe_Instalacion = null, DateTime? fecha_Airframe_Instalacion = null, bool valid = false) {
            Id = id;
            IdComponente = idComponente;
            IdAircraft = idAircraft;
            IdModelo = idModelo;
            IdPosicion = idPosicion;
            IdEstadoMayor = idEstadoMayor;
            Serie = serie;
            Capacity = capacity;
            TSN = tSN;
            CSN = cSN;
            Anio = anio;
            TSN_Componente_Instalacion = tSN_Componente_Instalacion;
            CSN_Componente_Instalacion = cSN_Componente_Instalacion;
            Fecha_Componente_Instalacion = fecha_Componente_Instalacion;
            TSN_Airframe_Instalacion = tSN_Airframe_Instalacion;
            CSN_Airframe_Instalacion = cSN_Airframe_Instalacion;
            Fecha_Airframe_Instalacion = fecha_Airframe_Instalacion;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Serie) && IdComponente > 0 && IdModelo > 0 && IdEstadoMayor > 0) {
                res.Error = "";
                if (Componente == null)
                    SetComponente();
                if (!Componente.Valid)
                    SetComponente();
                if (Modelo == null)
                    SetModelo();
                if (!Modelo.Valid)
                    SetModelo();
				SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM ItemMayor WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = $"{Componente.Descripcion} ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE ItemMayor SET IdComponente = @idcomponente, IdAircraft = @idaircraft, IdModelo = @idmodelo, IdPosicion = @idposicion, IdEstadoMayor = @idestadomayor, Serie = @serie, Capacity = @capacity, TSN = @tsn, CSN = @csn, Anio = @anio, TSN_Componente_Instalacion = @tsn_componente_instalacion, CSN_Componente_Instalacion = @csn_componente_instalacion, Fecha_Componente_Instalacion = @fecha_componente_instalacion, TSN_Airframe_Instalacion = @tsn_airframe_instalacion, CSN_Airframe_Instalacion = @csn_airframe_instalacion, Fecha_Airframe_Instalacion = @fecha_airframe_instalacion WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    if (IdEstadoMayor == 1) {
                        int ida = IdAircraft ?? 0;
                        List<ItemMayor> instalados = GetItemsByAircraft(ida, IdComponente);
                        int cantidad = Componente.GetCantidad(Modelo.Capacidad.Id);
                        if (instalados.Count >= cantidad) {
                            res.Error = $"No se Pueden Instalar Mas Componentes \"{Componente.Descripcion}\" a esta Aeronave";
                            return res;
                        }
                    }
                    SqlStr = @"INSERT INTO ItemMayor(IdComponente, IdAircraft, IdModelo, IdPosicion, IdEstadoMayor, Serie, Capacity, TSN, CSN, Anio, TSN_Componente_Instalacion, CSN_Componente_Instalacion, Fecha_Componente_Instalacion, TSN_Airframe_Instalacion, CSN_Airframe_Instalacion, Fecha_Airframe_Instalacion) VALUES(@idcomponente, @idaircraft, @idmodelo, @idposicion, @idestadomayor, @serie, @capacity, @tsn, @csn, @anio, @tsn_componente_instalacion, @csn_componente_instalacion, @fecha_componente_instalacion, @tsn_airframe_instalacion, @csn_airframe_instalacion, @fecha_airframe_instalacion)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idcomponente", IdComponente));
                Command.Parameters.Add(new SqlParameter("@idaircraft", IdAircraft ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idmodelo", IdModelo));
                Command.Parameters.Add(new SqlParameter("@idposicion", IdPosicion ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idestadomayor", IdEstadoMayor));
                Command.Parameters.Add(new SqlParameter("@serie", string.IsNullOrEmpty(Serie) ? SqlString.Null : Serie));
                Command.Parameters.Add(new SqlParameter("@capacity", string.IsNullOrEmpty(Capacity) ? SqlString.Null : Capacity));
                Command.Parameters.Add(new SqlParameter("@tsn", TSN ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@csn", CSN ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@anio", Anio ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@tsn_componente_instalacion", TSN_Componente_Instalacion ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@csn_componente_instalacion", CSN_Componente_Instalacion ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@fecha_componente_instalacion", Fecha_Componente_Instalacion ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@tsn_airframe_instalacion", TSN_Airframe_Instalacion ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@csn_airframe_instalacion", CSN_Airframe_Instalacion ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@fecha_airframe_instalacion", Fecha_Airframe_Instalacion ?? SqlDateTime.Null));
                using(TransactionScope scope=new TransactionScope()) {
                    RespuestaQuery rInUp = DataBase.Insert(Command);
                    if (rInUp.Valid) {
                        if (Insr) {
                            if (rInUp.IdRegistro == 0) {
                                res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br>Error: {rInUp.Error}";
                                return res;
                            }
                            Id = rInUp.IdRegistro;
                            Valid = true;
                        }
                        else {
                            if (Tiempos != null && Tiempos.Count > 0) {
                                var tmps = ATSM.Ingenieria.Tiempos.GetTiempos(Id, 1);
                                foreach (var tiempo in tmps) {
                                    var idx = this.Tiempos.FindIndex(t => { return t.Id == tiempo.Id; });
                                    if (idx == -1)
                                        tiempo.Delete();
                                }
                            }
                        }
                        if (this.Tiempos != null) {
                            foreach (var tiempo in this.Tiempos) {
                                tiempo.IdItemMayor = Id;
                                tiempo.IdItemMenor = null;
                                var rST = tiempo.Save();
                                if (!rST.Valid || !string.IsNullOrEmpty(rST.Error)) {
                                    res.Error = $"Error al Registrar los Tiempos: (CS.{this.GetType().Name}-Save.Err.04)<br>Error: {rST.Error}";
                                    return res;
                                }
                            }
                        }
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>Error: {rInUp.Error}";
                        return res;
                    }
                    SetAircraft();
                    SetModelo();
                    SetPosicion();
                    SetComponente();
                    res.Elemento = this;
                    res.Valid = true;
                    scope.Complete();
				}
            }
            else {
                if (string.IsNullOrEmpty(Serie))
                    res.Error += $"<br>Falta el Numero de Serie";
                if (IdComponente <= 0)
                    res.Error += $"<br>Falta el Componente Mayor";
                if (IdModelo <= 0)
                    res.Error += $"<br>Falta el Modelo";
                if (IdEstadoMayor <= 0)
                    res.Error += $"<br>Falta el Estado";
            }
            return res;
        }
        public Respuesta Delete() {
            if (Componente == null)
                SetComponente();
            if (!Componente.Valid)
                SetComponente();
            Respuesta res = new Respuesta($"{Componente.Descripcion} NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE ItemMayor WHERE Id = @id", Conexion);
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
                IdComponente = Registro.IdComponente;
                IdAircraft = Registro.IdAircraft;
                IdModelo = Registro.IdModelo;
                IdPosicion = Registro.IdPosicion;
                IdEstadoMayor = Registro.IdEstadoMayor;
                Serie = Registro.Serie;
                Capacity = Registro.Capacity;
                TSN = Registro.TSN;
                CSN = Registro.CSN;
                Anio = Registro.Anio;
                TSN_Componente_Instalacion = Registro.TSN_Componente_Instalacion;
                CSN_Componente_Instalacion = Registro.CSN_Componente_Instalacion;
                Fecha_Componente_Instalacion = Registro.Fecha_Componente_Instalacion;
                TSN_Airframe_Instalacion = Registro.TSN_Airframe_Instalacion;
                CSN_Airframe_Instalacion = Registro.CSN_Airframe_Instalacion;
                Fecha_Airframe_Instalacion = Registro.Fecha_Airframe_Instalacion;
                Valid = true;
                SetComponente();
                SetAircraft();
                SetModelo();
                SetPosicion();
                SetLimites();
                SetTiempos();
            }
        }
        private void Inicializar() {
            Id = 0;
            IdComponente = 0;
            IdAircraft = null;
            IdModelo = 0;
            IdPosicion = null;
            IdEstadoMayor = 0;
            Serie = "";
            Capacity = "";
            TSN = null;
            CSN = null;
            Anio = null;
            TSN_Componente_Instalacion = null;
            CSN_Componente_Instalacion = null;
            Fecha_Componente_Instalacion = null;
            TSN_Airframe_Instalacion = null;
            CSN_Airframe_Instalacion = null;
            Fecha_Airframe_Instalacion = null;
            Valid = false;
            Limites = new List<Limite>();
            Tiempos = new List<Tiempos>();
        }
        public void SetComponente() {
            Componente = new ComponenteMayor(IdComponente);
        }
        public void SetAircraft() {
            Aircraft = new Aircraft(IdAircraft);
        }
		public void SetModelo() {
            Modelo = new Modelo(IdModelo);
        }
        public void SetPosicion() {
            Posicion = new Position(IdPosicion);
        }
        public void SetLimites() {
            Limites = Limite.GetLimites(IdComponente, IdModelo, 1);
            foreach(var lim in Limites) { lim.SetLimit(); }
		}
        public void SetTiempos() {
            Tiempos = ATSM.Ingenieria.Tiempos.GetTiempos(Id, 1);
		}

        public static List<ItemMayor> GetItemMayors(int idComponente) {
            List<ItemMayor> itemmayors = new List<ItemMayor>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ItemMayor WHERE IdComponente = @idComponente", Conexion);
            comando.Parameters.Add(new SqlParameter("@idComponente", idComponente));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ItemMayor itemmayor = JsonConvert.DeserializeObject<ItemMayor>(JsonConvert.SerializeObject(reg));
                itemmayor.Valid = true;
                itemmayors.Add(itemmayor);
            }
            return itemmayors;
        }
		public static List<ItemMayor> GetItemMayoresByAircraft(int idAirgraft) {
            List<ItemMayor> itemmayors = new List<ItemMayor>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ItemMayor WHERE IdAircraft = @idAirgraft ORDER BY IdComponente", Conexion);
            comando.Parameters.Add(new SqlParameter("@idAirgraft", idAirgraft));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ItemMayor itemmayor = JsonConvert.DeserializeObject<ItemMayor>(JsonConvert.SerializeObject(reg));
                itemmayor.Valid = true;
                itemmayors.Add(itemmayor);
            }
            return itemmayors;
        }
        public static List<ItemMayor> GetItemsByAircraft(int idAirgraft, int idComponente) {
            List<ItemMayor> itemmayors = new List<ItemMayor>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ItemMayor WHERE IdComponente = @idComponente AND IdAircraft = @idAirgraft", Conexion);
            comando.Parameters.Add(new SqlParameter("@idComponente", idComponente));
            comando.Parameters.Add(new SqlParameter("@idAirgraft", idAirgraft));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ItemMayor itemmayor = JsonConvert.DeserializeObject<ItemMayor>(JsonConvert.SerializeObject(reg));
                itemmayor.Valid = true;
                itemmayors.Add(itemmayor);
            }
            return itemmayors;
        }
        public static List<ItemMayor> GetItemMayoresByModelo(int idComponente, int idModelo) {
            List<ItemMayor> itemmayors = new List<ItemMayor>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ItemMayor WHERE IdComponente = @idComponente AND IdModelo = @idModelo", Conexion);
            comando.Parameters.Add(new SqlParameter("@idComponente", idComponente));
            comando.Parameters.Add(new SqlParameter("@idModelo", idModelo));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ItemMayor itemmayor = JsonConvert.DeserializeObject<ItemMayor>(JsonConvert.SerializeObject(reg));
                itemmayor.Valid = true;
                itemmayors.Add(itemmayor);
            }
            return itemmayors;
        }
    }
}