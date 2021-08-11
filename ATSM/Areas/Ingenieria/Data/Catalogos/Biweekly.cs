using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

using WebMatrix.WebData;

namespace ATSM.Ingenieria {
	public class Biweekly {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Codigo { get; set; }
		public DateTime Fecha { get; set; }
		public int Usuario { get; set; }
		public bool Valid { get; set; }
		public object Websecurity { get; private set; }

		public Biweekly(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Biweekly WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Biweekly(string codigo) {
            Inicializar();
            if (!string.IsNullOrEmpty(codigo)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Biweekly WHERE Codigo = @codigo", Conexion);
                comando.Parameters.Add(new SqlParameter("@codigo", codigo));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Biweekly(int id, string codigo, DateTime fecha = default(DateTime), int usuario = 0, bool valid = false) {
            Id = id;
            Codigo = codigo;
            Fecha = fecha;
            Usuario = usuario;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Codigo)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Biweekly WHERE Id = @id OR Codigo = @codigo", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@codigo", Codigo));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Biweekly ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Biweekly SET Codigo = @codigo, Fecha = GETDATE(), Usuario = @usuario WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Biweekly(Codigo, Fecha, Usuario) VALUES(@codigo, GETDATE(), @usuario)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@codigo", string.IsNullOrEmpty(Codigo) ? SqlString.Null : Codigo));
                Command.Parameters.Add(new SqlParameter("@usuario", WebSecurity.CurrentUserId));
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
                if (!string.IsNullOrEmpty(Codigo))
                    res.Error += $"<br>Falta el codigo.";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Biweekly NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Biweekly WHERE Id = @id", Conexion);
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
                Fecha = Registro.Fecha;
                Usuario = Registro.Usuario;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            Codigo = "";
            Fecha = default(DateTime);
            Usuario = 0;
            Valid = false;
        }
        public static List<Biweekly> GetBiweeklys() {
            List<Biweekly> biweeklys = new List<Biweekly>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Biweekly", Conexion));
            foreach (var reg in res.Rows) {
                Biweekly biweekly = JsonConvert.DeserializeObject<Biweekly>(JsonConvert.SerializeObject(reg));
                biweekly.Valid = true;
                biweeklys.Add(biweekly);
            }
            return biweeklys;
        }
    }
}