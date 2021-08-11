using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace ATSM.Cuentas {
	public class Reposicion {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdAccount { get; set; }
		public int IdMovimiento { get; set; }
		public int IdGasto { get; set; }
		public string Fecha { get; set; }
		public decimal Monto { get; set; }
		public int IdMoneda { get; set; }
		public decimal TipoCambio { get; set; }
		public int IdSaldo { get; set; }
		public int Usuario { get; set; }
		public string Observaciones { get; set; }
		public bool Valid { get; set; }
        public Reposicion(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Reposicion WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Reposicion(int id, int idAccount, int idMovimiento = 0, int idGasto = 0, string fecha = "", decimal monto = 0, int idMoneda = 0, decimal tipoCambio = 0, int idSaldo = 0, int usuario = 0, string observaciones = "", bool valid = false) {
            Id = id;
            IdAccount = idAccount;
            IdMovimiento = idMovimiento;
            IdGasto = idGasto;
            Fecha = fecha;
            Monto = monto;
            IdMoneda = idMoneda;
            TipoCambio = tipoCambio;
            IdSaldo = idSaldo;
            Usuario = usuario;
            Observaciones = observaciones;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdAccount > 0 && IdGasto > 0 && IdMoneda > 0 && IdSaldo > 0 && Monto > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Reposicion WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Reposicion ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Reposicion SET IdAccount = @idaccount, IdMovimiento = @idmovimiento, IdGasto = @idgasto, Fecha = @fecha, Monto = @monto, IdMoneda = @idmoneda, TipoCambio = @tipocambio, IdSaldo = @idsaldo, Usuario = @usuario, Observaciones = @observaciones WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Reposicion(IdAccount, IdMovimiento, IdGasto, Fecha, Monto, IdMoneda, TipoCambio, IdSaldo, Usuario, Observaciones) VALUES(@idaccount, @idmovimiento, @idgasto, @fecha, @monto, @idmoneda, @tipocambio, @idsaldo, @usuario, @observaciones)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idaccount", IdAccount));
                Command.Parameters.Add(new SqlParameter("@idmovimiento", IdMovimiento));
                Command.Parameters.Add(new SqlParameter("@idgasto", IdGasto));
                Command.Parameters.Add(new SqlParameter("@fecha", Fecha));
                Command.Parameters.Add(new SqlParameter("@monto", Monto));
                Command.Parameters.Add(new SqlParameter("@idmoneda", IdMoneda));
                Command.Parameters.Add(new SqlParameter("@tipocambio", TipoCambio));
                Command.Parameters.Add(new SqlParameter("@idsaldo", IdSaldo));
                Command.Parameters.Add(new SqlParameter("@usuario", Usuario));
                Command.Parameters.Add(new SqlParameter("@observaciones", Observaciones));
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
                if (IdAccount == 0) {
                    res.Error += "<br>Falta Cuenta a la cual Aplicar la Reposicion.";
                }
                if (IdGasto == 0) {
                    res.Error += "<br>Falta la referencia del Gasto al que se Aplica la Reposicion.";
                }
                if (IdMoneda == 0) {
                    res.Error += "<br>Falta la Moneda en que se aplica la Reposicion.";
                }
                if (IdSaldo == 0) {
                    res.Error += "<br>Falta El Saldo al que se Aplica la Reposicion.";
                }
                if (Monto == 0) {
                    res.Error += "<br>Falta el Monto de la Reposicion.";
                }
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Reposicion NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Reposicion WHERE Id = @id", Conexion);
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
                IdAccount = Registro.IdAccount;
                IdMovimiento = Registro.IdMovimiento;
                IdGasto = Registro.IdGasto;
                Fecha = Registro.Fecha;
                Monto = Registro.Monto;
                IdMoneda = Registro.IdMoneda;
                TipoCambio = Registro.TipoCambio;
                IdSaldo = Registro.IdSaldo;
                Usuario = Registro.Usuario;
                Observaciones = Registro.Observaciones;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            IdAccount = 0;
            IdMovimiento = 0;
            IdGasto = 0;
            Fecha = "";
            Monto = 0;
            IdMoneda = 0;
            TipoCambio = 0;
            IdSaldo = 0;
            Usuario = 0;
            Observaciones = "";
            Valid = false;
        }
        public static List<Reposicion> GetReposicions() {
            List<Reposicion> reposicions = new List<Reposicion>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Reposicion", Conexion));
            foreach (var reg in res.Rows) {
                Reposicion reposicion = JsonConvert.DeserializeObject<Reposicion>(JsonConvert.SerializeObject(reg));
                reposicion.Valid = true;
                reposicions.Add(reposicion);
            }
            return reposicions;
        }
    }
}