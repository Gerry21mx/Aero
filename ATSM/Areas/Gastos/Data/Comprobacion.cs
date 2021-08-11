using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

using ATSM.Cuentas;

using Newtonsoft.Json;

using WebMatrix.WebData;

namespace ATSM.Gastos {
    public class Comprobacion {
        private static SqlConnection Conexion = DataBase.Conexion();
        public int Id { get; set; }
        public int IdVuelo { get; set; }
        public int IdAccount { get; set; }
        public DateTime Fecha { get; set; }
        /// <summary>
        /// Estado de la Comprobacino
        /// <list type="number">
        ///     <item>Capturado</item>
        ///     <item>Validado</item>
        /// </list>
        /// </summary>
		public int Estado { get; set; }
        public string Observaciones { get; set; }
        public int? UsuValida { get; set; }
        public DateTime? Fecha_Valida { get; set; }
        public bool Valid { get; set; }
        public Account Cuenta { get; set; }
        public List<Gasto> Gastos { get; set; }
        public Comprobacion(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Comprobacion WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Comprobacion(int idvuelo, int idaccount) {
            Inicializar();
            if (idvuelo > 0 && idaccount > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Comprobacion WHERE IdVuelo = @idvuelo AND IdAccount = @idaccount", Conexion);
                comando.Parameters.Add(new SqlParameter("@idvuelo", idvuelo));
                comando.Parameters.Add(new SqlParameter("@idaccount", idaccount));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Comprobacion(int id, int idVuelo, int idAccount, DateTime? fecha = null, int estado = 0, string observaciones = null, int? usuvalida = null, DateTime? fecha_valida = null) {
            Id = id;
            IdVuelo = idVuelo;
            IdAccount = idAccount;
            Fecha = fecha ?? DateTime.Now;
            Estado = estado;
            Observaciones = observaciones;
            UsuValida = usuvalida;
            Fecha_Valida = fecha_valida;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdVuelo > 0 && IdAccount > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Comprobacion WHERE Id = @id OR (IdVuelo = @idvuelo AND IdAccount = @idaccount)", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@idvuelo", IdVuelo));
                Cmnd.Parameters.Add(new SqlParameter("@idaccount", IdAccount));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Comprobacion ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Comprobacion SET IdVuelo = @idvuelo, IdAccount = @idaccount, Fecha = @fecha, Estado = @estado, Observaciones = @observaciones, UsuValida = @usuvalida, Fecha_Valida = @fecha_valida WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Comprobacion(IdVuelo, IdAccount, Fecha, Estado, Observaciones, UsuValida, Fecha_Valida) VALUES(@idvuelo, @idaccount, GETDATE(), @estado, @observaciones, @usuvalida, @fecha_valida)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                    Estado = 1;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idvuelo", IdVuelo));
                Command.Parameters.Add(new SqlParameter("@idaccount", IdAccount));
                Command.Parameters.Add(new SqlParameter("@fecha", Fecha));
                Command.Parameters.Add(new SqlParameter("@estado", Estado));
                Command.Parameters.Add(new SqlParameter("@observaciones", string.IsNullOrEmpty(Observaciones) ? SqlString.Null : Observaciones));
                Command.Parameters.Add(new SqlParameter("@usuvalida", UsuValida ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@fecha_valida", Fecha_Valida ?? SqlDateTime.Null));
                using (TransactionScope scope = new TransactionScope()) {
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
                        else {
                            var gastos = Gasto.GetGastos(Id);
                            foreach (var gas in gastos) {
                                if (Gastos.FindIndex(g => { return g.Id == gas.Id; }) == -1) {
                                    var rDG = gas.Delete();
                                    if (!rDG.Valid || !string.IsNullOrEmpty(rDG.Error)) {
                                        res.Error = $"Error al Eliminar el Gasto: (CS.{this.GetType().Name}-Save.Err.04)<br>Error: {rDG.Error}";
                                        return res;
                                    }
                                }
                            }
						}
                        foreach (var gasto in Gastos) {
                            gasto.IdComprobacion = Id;
                            var rSG = gasto.Save();
                            if (!rSG.Valid || !string.IsNullOrEmpty(rSG.Error)) {
                                res.Error = $"Error al Registrar el Gasto: (CS.{this.GetType().Name}-Save.Err.05)<br>Error: {rSG.Error}";
                                return res;
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
                if (IdVuelo == 0)
                    res.Error += $"<br>Falta el Vuelo Referencado";
                if (IdAccount == 0)
                    res.Error += $"<br>Falta la CUenta.";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Comprobacion NO se Elimino");
            if (Estado > 1) {
                res.Error += "<br>La Comprobacion ya ha Sido Validada y no es posible eliminarla.";
                return res;
            }
            SqlCommand Command = new SqlCommand("DELETE Comprobacion WHERE Id = @id", Conexion);
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
                IdVuelo = Registro.IdVuelo;
                IdAccount = Registro.IdAccount;
                Fecha = Registro.Fecha;
                Estado = Registro.Estado;
                Observaciones = Registro.Observaciones;
                UsuValida = Registro.UsuValida;
                Fecha_Valida = Registro.Fecha_Valida;
                Valid = true;
                Gastos = Gasto.GetGastos(Id);
            }
        }
        private void Inicializar() {
            Id = 0;
            IdVuelo = 0;
            IdAccount = 0;
            Fecha = default(DateTime);
            Estado = 0;
            Observaciones = "";
            UsuValida = null;
            Fecha_Valida = null;
        }
        public static List<Comprobacion> GetComprobaciones(int idaccount) {
            List<Comprobacion> comprobacions = new List<Comprobacion>();
            SqlCommand comando = new SqlCommand("SELECT * FROM Comprobacion WHERE IdAccount = @idaccount", Conexion);
            comando.Parameters.Add(new SqlParameter("@idaccount", idaccount));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                Comprobacion comprobacion = JsonConvert.DeserializeObject<Comprobacion>(JsonConvert.SerializeObject(reg));
                comprobacion.Valid = true;
                comprobacions.Add(comprobacion);
            }
            return comprobacions;
        }
        /// <summary>
        /// Comprobacion de Gastos de Vuelos pendientes de los Pilotos
        /// </summary>
        /// <param name="idaccount">Cuenta del Piloto</param>
        /// <returns>Lista de Vuelos No comprobados</returns>
        public static dynamic GetVuelosPendientesComprobarPiloto(int idaccount) {
            Account cta = new Account(idaccount);
            //SqlCommand comando = new SqlCommand("SELECT IdVuelo, Trip, Salida FROM Vuelo WHERE IdVuelo NOT IN  (SELECT IdVuelo FROM Comprobacion) AND IdVuelo IN (SELECT DISTINCT IdVuelo FROM VueloTramo WHERE IdCapitan = @idcapitan)", Conexion);
            SqlCommand comando = new SqlCommand("SELECT IdVuelo, Trip, Salida FROM Vuelo WHERE IdVuelo NOT IN (SELECT IdVuelo FROM Comprobacion WHERE IdAccount = @idaccount) AND IdVuelo IN (SELECT DISTINCT IdVuelo FROM VueloTramo WHERE IdCapitan = @idcapitan)", Conexion);
            comando.Parameters.Add(new SqlParameter("@idaccount", cta.Id));
            comando.Parameters.Add(new SqlParameter("@idcapitan", cta.IdCrew ?? 0));
            RespuestaQuery res = DataBase.Query(comando);
            return res.Rows;
        }
        public void SetAccount() {
            Cuenta = new Account(IdAccount);
        }
    }
}