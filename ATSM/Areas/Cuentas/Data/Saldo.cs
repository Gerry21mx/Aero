using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace ATSM.Cuentas {
	public class Saldo {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Codigo { get; set; }
		public string Nombre { get; set; }
		public bool? Combustible { get; set; }
		public bool Valid { get; set; }
        public Saldo(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Saldo WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Saldo(string codigo) {
            Inicializar();
            if (!string.IsNullOrEmpty(codigo)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Saldo WHERE Codigo = @codigo", Conexion);
                comando.Parameters.Add(new SqlParameter("@codigo", codigo));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Saldo(int id, string codigo, string nombre = "", bool? combustible = null, bool valid = false) {
            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            Combustible = combustible;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Codigo) && !string.IsNullOrEmpty(Nombre)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Saldo WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Saldo ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Saldo SET Codigo = @codigo, Nombre = @nombre, Combustible = @combustible WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Saldo(Codigo, Nombre, Combustible) VALUES(@codigo, @nombre, @combustible)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@codigo", Codigo));
                Command.Parameters.Add(new SqlParameter("@nombre", Nombre));
                Command.Parameters.Add(new SqlParameter("@combustible", Combustible));
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
                if (string.IsNullOrEmpty(Codigo)) {
                    res.Error += "<br>Falta Codigo.";
                }
                if (string.IsNullOrEmpty(Nombre)) {
                    res.Error += "<br>Falta Nombre.";
                }
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Saldo NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Saldo WHERE Id = @id", Conexion);
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
                Codigo = Registro.Codigo;
                Nombre = Registro.Nombre;
                Combustible = Registro.Combustible;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            Codigo = "";
            Nombre = "";
            Combustible = null;
            Valid = false;
        }
        public static List<Saldo> GetSaldos() {
            List<Saldo> monedas = new List<Saldo>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Saldo", Conexion));
            foreach (var reg in res.Rows) {
                Saldo moneda = JsonConvert.DeserializeObject<Saldo>(JsonConvert.SerializeObject(reg));
                moneda.Valid = true;
                monedas.Add(moneda);
            }
            return monedas;
        }
    }
}