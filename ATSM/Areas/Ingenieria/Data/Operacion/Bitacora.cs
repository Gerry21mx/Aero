using ATSM.Tripulaciones;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

using WebMatrix.WebData;

namespace ATSM.Ingenieria {
	public class Bitacora {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int Folio { get; set; }
		public int IdAircraft { get; set; }
		public DateTime? Fecha { get; set; }
		public int Cancelada { get; set; }
		public int IdComandante { get; set; }
		public int IdPrimerOficial { get; set; }
		public string Extra { get; set; }
		public string Observaciones { get; set; }
		public List<BitacoraTramo> Tramos { get; set; }
		public BitacoraParametrosMotor ParametrosMotor { get; set; }
		public List<BitacoraRCCA> RCCA { get; set; }
		public int Usuario { get; set; }
		public bool Valid { get; set; }
		public Aircraft Avion { get; set; }
		public Crew Comandante { get; set; }
		public Crew PrimerOficial { get; set; }
		public Bitacora(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Bitacoras WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Bitacora(int idAircraft, int folio) {
            Inicializar();
            if (idAircraft > 0 && folio > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Bitacoras WHERE IdAircraft = @idAircraft AND Folio = @folio", Conexion);
                comando.Parameters.Add(new SqlParameter("@idAircraft", idAircraft));
                comando.Parameters.Add(new SqlParameter("@folio", folio));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Bitacora(int id, int folio, int idAircraft, DateTime? fecha, int cancelada = 0, int idComandante = 0, int idPrimerOficial = 0, string extra = "", string observaciones = "", int usuario = 0, bool valid = false) {
            Id = id;
            Folio = folio;
            IdAircraft = idAircraft;
            Fecha = fecha;
            Cancelada = cancelada;
            IdComandante = idComandante;
            IdPrimerOficial = idPrimerOficial;
            Extra = extra;
            Observaciones = observaciones;
            Usuario = usuario;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (Folio > 0 && IdAircraft > 0 && Fecha != null) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Bitacoras WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Bitacora ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Bitacoras SET Folio = @folio, IdAircraft = @idaircraft, Fecha = @fecha, Cancelada = @cancelada, IdComandante = @idcomandante, IdPrimerOficial = @idprimeroficial, Extra = @extra, Observaciones = @observaciones, Usuario = @usuario WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Bitacoras(Folio, IdAircraft, Fecha, Cancelada, IdComandante, IdPrimerOficial, Extra, Observaciones, Usuario) VALUES(@folio, @idaircraft, @fecha, @cancelada, @idcomandante, @idprimeroficial, @extra, @observaciones, @usuario)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@folio", Folio));
                Command.Parameters.Add(new SqlParameter("@idaircraft", IdAircraft));
                Command.Parameters.Add(new SqlParameter("@fecha", Fecha ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@cancelada", Cancelada));
                Command.Parameters.Add(new SqlParameter("@idcomandante", IdComandante));
                Command.Parameters.Add(new SqlParameter("@idprimeroficial", IdPrimerOficial));
                Command.Parameters.Add(new SqlParameter("@extra", string.IsNullOrEmpty(Extra) ? SqlString.Null : Extra));
                Command.Parameters.Add(new SqlParameter("@observaciones", string.IsNullOrEmpty(Observaciones) ? SqlString.Null : Observaciones));
                Command.Parameters.Add(new SqlParameter("@usuario", WebSecurity.CurrentUserId));
                using(TransactionScope scope = new TransactionScope()) {
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
                        if (Cancelada > 0) {
                            string sqlComand = $"DELETE BitacoraTramos WHERE IdBitacora = @idBitacora;";
                            sqlComand += $"DELETE BitacoraParametrosMotor WHERE IdBitacora = @idBitacora;";
                            sqlComand += $"DELETE BitacoraRCCA WHERE IdBitacora = @idBitacora;";
                            Command = new SqlCommand(sqlComand, Conexion);
                            Command.Parameters.AddWithValue("@idBitacora", Id);
                        }
                        else {
                            Respuesta rSvMod = new Respuesta();
                            var tramosA = BitacoraTramo.GetBitacoraTramos(Id);
                            foreach (var trAn in tramosA) {
                                var tr = Tramos.FindIndex(t => { return t.Id == trAn.Id; });
                                if (tr == -1) {
                                    var rDelTrAn = trAn.Delete();
                                    if (!rDelTrAn.Valid || !string.IsNullOrEmpty(rDelTrAn.Error)) {
                                        res.Error = $"No se pudo Eliminar el Tramo Anterior(CS.{this.GetType().Name}-Save.Err.04)<br>Error: {rDelTrAn.Error}";
                                        return res;
                                    }
                                }
                            }
                            var rccaAs = BitacoraRCCA.GetBitacoraRCCAs(Id);
                            foreach (var rca in rccaAs) {
                                var tr = RCCA.FindIndex(r => { return r.Id == rca.Id; });
                                if (tr == -1) {
                                    var rDelRCA = rca.Delete();
                                    if (!rDelRCA.Valid || !string.IsNullOrEmpty(rDelRCA.Error)) {
                                        res.Error = $"No se pudo Eliminar el Registro de Comparaciones Cruzadas de Altimetro.(CS.{this.GetType().Name}-Save.Err.05)<br>Error: {rDelRCA.Error}";
                                        return res;
                                    }
                                }
                            }
                            foreach (var tra in Tramos) {
                                tra.IdBitacora = this.Id;
                                rSvMod = tra.Save();
                                if (!rSvMod.Valid || !string.IsNullOrEmpty(rSvMod.Error)) {
                                    res.Error = $"No se pudo Guardar el Tramo.(CS.{this.GetType().Name}-Save.Err.06)<br>Error: {rSvMod.Error}";
                                    return res;
                                }
                            }
                            ParametrosMotor.IdBitacora = this.Id;
                            rSvMod = ParametrosMotor.Save();
                            if (!rSvMod.Valid || !string.IsNullOrEmpty(rSvMod.Error)) {
                                res.Error = $"No se pudo Registrar los Parametros de Motor.(CS.{this.GetType().Name}-Save.Err.07)<br>Error: {rSvMod.Error}";
                                return res;
                            }
                            foreach (var rca in RCCA) {
                                rca.IdBitacora = this.Id;
                                rSvMod = rca.Save();
                                if (!rSvMod.Valid || !string.IsNullOrEmpty(rSvMod.Error)) {
                                    res.Error = $"No se pudo Guardar el Registro de Comparaciones Cruzadas de Altimetro.(CS.{this.GetType().Name}-Save.Err.08)<br>Error: {rSvMod.Error}";
                                    return res;
                                }
                            }
                        }
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                        return res;
                    }
                    res.Elemento = this;
                    res.Valid = true;
                    scope.Complete();
				}
            }
            else {
                if (IdAircraft <= 0)
                    res.Error += $"<br>Falta la Aeronave de la Matricula";
                if (Folio <= 0)
                    res.Error += $"<br>Falta el Folio de la Bitacora";
                if (Fecha==null)
                    res.Error += $"<br>Falta la Fecha de la Bitacora";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Bitacora NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Bitacoras WHERE Id = @id", Conexion);
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
                Folio = Registro.Folio;
                IdAircraft = Registro.IdAircraft;
                Fecha = Registro.Fecha;
                Cancelada = Registro.Cancelada;
                IdComandante = Registro.IdComandante;
                IdPrimerOficial = Registro.IdPrimerOficial;
                Extra = Registro.Extra;
                Observaciones = Registro.Observaciones;
                Usuario = Registro.Usuario;
                Valid = true;
                SetAircraft();
                SetComandante();
                SetPrimerOficial();
                SetTramos(true);
                SetParametros();
                SetRCCA();
			}
        }
        private void Inicializar() {
            Id = 0;
            Folio = 0;
            IdAircraft = 0;
            Fecha = null;
            Cancelada = 0;
            IdComandante = 0;
            IdPrimerOficial = 0;
            Extra = "";
            Observaciones = "";
            Usuario = 0;
            Valid = false;
            Tramos = new List<BitacoraTramo>();
        }
        public static List<Bitacora> GetBitacoras() {
            List<Bitacora> bitacoras = new List<Bitacora>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Bitacoras", Conexion));
            foreach (var reg in res.Rows) {
                Bitacora bitacora = JsonConvert.DeserializeObject<Bitacora>(JsonConvert.SerializeObject(reg));
                bitacora.Valid = true;
                bitacoras.Add(bitacora);
            }
            return bitacoras;
        }
        public void SetAircraft() {
            Avion = new Aircraft(IdAircraft);
		}
        public void SetComandante() {
            Comandante = new Crew(IdComandante);
		}
        public void SetPrimerOficial() {
            PrimerOficial = new Crew(IdPrimerOficial);
		}
        public void SetTramos(bool conAeropuertos) {
            Tramos = BitacoraTramo.GetBitacoraTramos(Id);
			if (conAeropuertos) {
                foreach (var tramo in Tramos) {
                    tramo.SetOrigen();
                    tramo.SetDestino();
                }
            }
		}
        public void SetParametros() {
            ParametrosMotor = new BitacoraParametrosMotor(Id);
		}
        public void SetRCCA() {
            RCCA = BitacoraRCCA.GetBitacoraRCCAs(Id);

        }
        public static int BitacoraInicial(int idAircraft) {
            int folio = 0;
            if (idAircraft>0) {
                SqlCommand Comando = new SqlCommand("SELECT TOP 1 Folio FROM Bitacoras WHERE IdAircraft = @idAircraft ORDER BY Folio", Conexion);
                Comando.Parameters.Add(new SqlParameter("@idAircraft", idAircraft));
                RespuestaQuery res = DataBase.Query(Comando);
                folio = res.Valid ? res.Row.Folio : folio;
            }
            return folio;
        }
        public static int BitacoraFinal(int idAircraft) {
            int folio = 0;
            if (idAircraft > 0) {
                SqlCommand Comando = new SqlCommand("SELECT TOP 1 Folio FROM Bitacoras WHERE IdAircraft = @idAircraft ORDER BY Folio DESC", Conexion);
                Comando.Parameters.Add(new SqlParameter("@idAircraft", idAircraft));
                RespuestaQuery res = DataBase.Query(Comando);
                folio = res.Valid ? res.Row.Folio : folio;
            }
            return folio;
        }
    }
}