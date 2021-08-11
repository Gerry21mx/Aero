using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace ATSM.Cuentas {
	public class Devolucion {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdAccount { get; set; }
		public int IdMovimiento { get; set; }
		public string Fecha { get; set; }
		public decimal Monto { get; set; }
		public int IdMoneda { get; set; }
		public decimal TipoCambio { get; set; }
		public int IdSaldo { get; set; }
		public int Usuario { get; set; }
		public string Observaciones { get; set; }
		public bool Valid { get; set; }
        public Devolucion(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Devolucion WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Devolucion(int id = 0, int idAccount = 0, int idMovimiento = 0, string fecha = "", decimal monto = 0, int idMoneda = 0, decimal tipoCambio = 0, int idSaldo = 0, int usuario = 0, string observaciones = "", bool valid = false) {
            Id = id;
            IdAccount = idAccount;
            IdMovimiento = idMovimiento;
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
            if (IdAccount > 0 && Monto > 0 && IdMoneda > 0 && IdSaldo > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Devolucion WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Devolucion ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Devolucion SET IdAccount = @idaccount, IdMovimiento = @idmovimiento, Fecha = @fecha, Monto = @monto, IdMoneda = @idmoneda, TipoCambio = @tipocambio, IdSaldo = @idsaldo, Usuario = @usuario, Observaciones = @observaciones WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Devolucion(IdAccount, IdMovimiento, Fecha, Monto, IdMoneda, TipoCambio, IdSaldo, Usuario, Observaciones) VALUES(@idaccount, @idmovimiento, @fecha, @monto, @idmoneda, @tipocambio, @idsaldo, @usuario, @observaciones)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idaccount", IdAccount));
                Command.Parameters.Add(new SqlParameter("@idmovimiento", IdMovimiento));
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
                    res.Error += "<br>Falta Cuenta a la cual Aplicar la Devolucion.";
                }
                if (IdMoneda == 0) {
                    res.Error += "<br>Falta la Moneda en que se aplica la Devolucion.";
                }
                if (IdSaldo == 0) {
                    res.Error += "<br>Falta El Saldo al que se Aplica la Devolucion.";
                }
                if (Monto == 0) {
                    res.Error += "<br>Falta el Monto de la Devolucion.";
                }
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Devolucion NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Devolucion WHERE Id = @id", Conexion);
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
            Fecha = "";
            Monto = 0;
            IdMoneda = 0;
            TipoCambio = 0;
            IdSaldo = 0;
            Usuario = 0;
            Observaciones = "";
            Valid = false;
        }
        public static List<Devolucion> GetDevolucions() {
            List<Devolucion> devolucions = new List<Devolucion>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Devolucion", Conexion));
            foreach (var reg in res.Rows) {
                Devolucion devolucion = JsonConvert.DeserializeObject<Devolucion>(JsonConvert.SerializeObject(reg));
                devolucion.Valid = true;
                devolucions.Add(devolucion);
            }
            return devolucions;
        }
    }
}