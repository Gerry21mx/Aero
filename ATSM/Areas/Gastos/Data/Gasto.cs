using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using ATSM.Seguimiento;

using Newtonsoft.Json;

using WebMatrix.WebData;
using ATSM.Cuentas;

namespace ATSM.Gastos {
	public class Gasto {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public DateTime Fecha { get; set; }
		public int IdComprobacion { get; set; }
		public int? IdAeropuerto { get; set; }
		public string Factura { get; set; }
		public string Concepto { get; set; }
		public decimal? LTS_Combustible { get; set; }
		public decimal? Subtotal { get; set; }
		public decimal? ImpHospedaje { get; set; }
		public decimal? P_IVA { get; set; }
		public decimal? IVA { get; set; }
		public decimal Total { get; set; }
		public decimal TotalAutorizado { get; set; }
		public string Observaciones { get; set; }
		public int IdMoneda { get; set; }
		public int IdSaldo { get; set; }
		public int IdTipoGasto { get; set; }
		public bool? Cancelado { get; set; }
		public int? Usuario_Cancela { get; set; }
		public DateTime? Fecha_Cancela { get; set; }
		public string Observaciones_Cancela { get; set; }
		public int? IdPago { get; set; }
		public bool Valid { get; set; }
		public Aeropuerto Aeropuerto { get; set; }
		public TipoGasto Tipo { get; set; }
		public Moneda Moneda { get; set; }
		public Saldo Saldo { get; set; }
        
        public Gasto(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Gasto WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Gasto(int id = 0, DateTime? fecha = null, int idComprobacion = 0, int? idAeropuerto = null, string factura = "", string concepto = "", decimal? lTS_Combustible = null, decimal? subtotal = null, decimal? impHospedaje = null, decimal? p_IVA = null, decimal? iVA = null, decimal total = 0, decimal totalAutorizado = 0, string observaciones = null, int idMoneda = 0, int idSaldo = 0, int idTipoGasto = 0, bool? cancelado = null, int? usuario_Cancela = null, DateTime? fecha_Cancela = null, string observaciones_Cancela = null, int? idpago = null, bool valid = false) {
            Id = id;
            Fecha = fecha ?? DateTime.Now;
            IdComprobacion = idComprobacion;
            IdAeropuerto = idAeropuerto;
            Factura = factura;
            Concepto = concepto;
            LTS_Combustible = lTS_Combustible;
            Subtotal = subtotal;
            ImpHospedaje = impHospedaje;
            P_IVA = p_IVA;
            IVA = iVA;
            Total = total;
            TotalAutorizado = totalAutorizado;
            Observaciones = observaciones;
            IdMoneda = idMoneda;
            IdSaldo = idSaldo;
            IdTipoGasto = idTipoGasto;
            Cancelado = cancelado;
            Usuario_Cancela = usuario_Cancela;
            Fecha_Cancela = fecha_Cancela;
            Observaciones_Cancela = observaciones_Cancela;
            IdPago = idpago;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Concepto) && IdComprobacion > 0 && IdTipoGasto > 0 && IdMoneda > 0 && IdSaldo > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Gasto WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Gasto ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = $@"UPDATE Gasto SET Fecha = @fecha, IdComprobacion = @idcomprobacion, IdAeropuerto = @idaeropuerto, Factura = @factura, Concepto = @concepto, LTS_Combustible = @lts_combustible, Subtotal = @subtotal, ImpHospedaje = @imphospedaje, P_IVA = @p_iva, IVA = @iva, Total = @total, TotalAutorizado = @totalautorizado, Observaciones = @observaciones, IdMoneda = @idmoneda, IdSaldo = @idsaldo, IdTipoGasto = @idtipogasto{(Cancelado == true ? $", Cancelado = @cancelado, Usuario_Cancela = @usuario_cancela, Fecha_Cancela = @fecha_cancela, Observaciones_Cancela = @observaciones_cancela" : "")} WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Gasto(Fecha, IdComprobacion, IdAeropuerto, Factura, Concepto, LTS_Combustible, Subtotal, ImpHospedaje, P_IVA, IVA, Total, TotalAutorizado, Observaciones, IdMoneda, IdSaldo, IdTipoGasto) VALUES(GETDATE(), @idcomprobacion, @idaeropuerto, @factura, @concepto, @lts_combustible, @subtotal, @imphospedaje, @p_iva, @iva, @total, @totalautorizado, @observaciones, @idmoneda, @idsaldo, @idtipogasto)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@fecha", Fecha));
                Command.Parameters.Add(new SqlParameter("@idcomprobacion", IdComprobacion));
                Command.Parameters.Add(new SqlParameter("@idaeropuerto", IdAeropuerto ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@factura", string.IsNullOrEmpty(Factura) ? SqlString.Null : Factura));
                Command.Parameters.Add(new SqlParameter("@concepto", string.IsNullOrEmpty(Concepto) ? "" : Concepto));
                Command.Parameters.Add(new SqlParameter("@lts_combustible", LTS_Combustible ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@subtotal", Subtotal ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@imphospedaje", ImpHospedaje ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@p_iva", P_IVA ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@iva", IVA ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@total", Total));
                Command.Parameters.Add(new SqlParameter("@totalautorizado", TotalAutorizado));
                Command.Parameters.Add(new SqlParameter("@observaciones", string.IsNullOrEmpty(Observaciones) ? SqlString.Null : Observaciones));
                Command.Parameters.Add(new SqlParameter("@idmoneda", IdMoneda));
                Command.Parameters.Add(new SqlParameter("@idsaldo", IdSaldo));
                Command.Parameters.Add(new SqlParameter("@idtipogasto", IdTipoGasto));
                Command.Parameters.Add(new SqlParameter("@cancelado", Cancelado ?? SqlBoolean.Null));
                Command.Parameters.Add(new SqlParameter("@usuario_cancela", WebSecurity.CurrentUserId));
                Command.Parameters.Add(new SqlParameter("@fecha_cancela", Fecha_Cancela ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@observaciones_cancela", string.IsNullOrEmpty(Observaciones_Cancela) ? SqlString.Null : Observaciones_Cancela));
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
                if (string.IsNullOrEmpty(Concepto))
                    res.Error += $"<br>Falta el Concepto.";
                if (IdComprobacion==0)
                    res.Error += $"<br>Falta la Comprobacion.";
                if (IdTipoGasto == 0)
                    res.Error += $"<br>Falta el Tipo de Gasto.";
                if (IdMoneda == 0)
                    res.Error += $"<br>Falta la Divisa.";
                if (IdSaldo == 0)
                    res.Error += $"<br>Falta el Saldo a Aplicar.";
			}
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Gasto NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Gasto WHERE Id = @id", Conexion);
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
                Fecha = Registro.Fecha;
                IdComprobacion = Registro.IdComprobacion;
                IdAeropuerto = Registro.IdAeropuerto;
                Factura = Registro.Factura;
                Concepto = Registro.Concepto;
                LTS_Combustible = Registro.LTS_Combustible;
                Subtotal = Registro.Subtotal;
                ImpHospedaje = Registro.ImpHospedaje;
                P_IVA = Registro.P_IVA;
                IVA = Registro.IVA;
                Total = Registro.Total;
                TotalAutorizado = Registro.TotalAutorizado;
                Observaciones = Registro.Observaciones;
                IdMoneda = Registro.IdMoneda;
                IdSaldo = Registro.IdSaldo;
                IdTipoGasto = Registro.IdTipoGasto;
                Cancelado = Registro.Cancelado;
                Usuario_Cancela = Registro.Usuario_Cancela;
                Fecha_Cancela = Registro.Fecha_Cancela;
                Observaciones_Cancela = Registro.Observaciones_Cancela;
                IdPago = Registro.IdPago;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            Fecha = default(DateTime);
            IdComprobacion = 0;
            IdAeropuerto = null;
            Factura = null;
            Concepto = "";
            LTS_Combustible = null;
            Subtotal = null;
            ImpHospedaje = null;
            P_IVA = null;
            IVA = null;
            Total = 0;
            TotalAutorizado = 0;
            Observaciones = null;
            IdMoneda = 0;
            IdSaldo = 0;
            IdTipoGasto = 0;
            Cancelado = null;
            Usuario_Cancela = null;
            Fecha_Cancela = null;
            Observaciones_Cancela = null;
            IdPago = null;
            Valid = false;
        }
        public static List<Gasto> GetGastos(int idcomprobacion) {
            List<Gasto> gastos = new List<Gasto>();
            SqlCommand comando = new SqlCommand("SELECT * FROM Gasto WHERE IdComprobacion = @idcomprobacion", Conexion);
            comando.Parameters.Add(new SqlParameter("@idcomprobacion", idcomprobacion));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                Gasto gasto = JsonConvert.DeserializeObject<Gasto>(JsonConvert.SerializeObject(reg));
                gasto.Valid = true;
                gasto.SetAeropuerto();
                gasto.SetTipoGasto();
                gastos.Add(gasto);
            }
            return gastos;
        }
        public void SetAeropuerto() {
            Aeropuerto = new Aeropuerto(IdAeropuerto);
		}
        public void SetTipoGasto() {
            Tipo = new TipoGasto(IdTipoGasto);
		}
        public void SetMoneda() {
            Moneda = new Moneda(IdMoneda);
		}
        public void SetSaldo() {
            Saldo = new Saldo(IdSaldo);
		}
    }
}