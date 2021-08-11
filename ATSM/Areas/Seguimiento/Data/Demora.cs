using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Newtonsoft.Json;

namespace ATSM.Seguimiento {
	public class Demora {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int IdDemora { get; set; }
		public string Codigo { get; set; }
		public string Clasificacion { get; set; }
		public string Descripcion { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; set; }
        public Demora(int? iddemora = null) {
            Inicializar();
            if (iddemora > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Demora WHERE IdDemora = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", iddemora));
                SetDatos(comando);
            }
        }
        public Demora(string codigo) {
            Inicializar();
            if (!string.IsNullOrEmpty(codigo)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Demora WHERE Codigo = @codigo", Conexion);
                comando.Parameters.Add(new SqlParameter("@codigo", codigo));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Demora(int idDemora, string codigo, string clasificacion = null, string descripcion = null, bool activo = false, bool valid = false) {
            IdDemora = idDemora;
            Codigo = codigo;
            Clasificacion = clasificacion;
            Descripcion = descripcion;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta();
            if (!string.IsNullOrEmpty(Codigo) && !string.IsNullOrEmpty(Descripcion)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT IdDemora FROM Demora WHERE IdDemora = @iddemora OR Codigo = @codigo", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@iddemora", IdDemora));
                Cmnd.Parameters.Add(new SqlParameter("@codigo", Codigo));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Demora ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Demora SET Codigo = @codigo, Clasificacion = @clasificacion, Descripcion = @descripcion, Activo = @activo WHERE IdDemora=@iddemora";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Demora(Codigo, Clasificacion, Descripcion, Activo) VALUES(@codigo, @clasificacion, @descripcion, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@iddemora", IdDemora));
                Command.Parameters.Add(new SqlParameter("@codigo", Codigo));
                Command.Parameters.Add(new SqlParameter("@clasificacion", string.IsNullOrEmpty(Clasificacion) ? DBNull.Value : Clasificacion));
                Command.Parameters.Add(new SqlParameter("@descripcion", string.IsNullOrEmpty(Descripcion) ? DBNull.Value : Descripcion));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid) {
                    if (Insr) {
                        if (rInUp.IdRegistro == 0) {
                            res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
                            return res;
                        }
                        IdDemora = rInUp.IdRegistro;
                        Valid = true;
                    }
                }
                else {
                    res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br> Error: {rInUp.Error}";
                    return res;
                }
                res.Elemento = this;
                res.Valid = true;
            }
            else {
                res.Error = $"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)";
                if (string.IsNullOrEmpty(Codigo)) {
                    res.Error += "<br>Falta Codigo de Demora.";
                }
                if (string.IsNullOrEmpty(Descripcion)) {
                    res.Error += "<br>Falta la Descripcion de la Demora.";
                }
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Demora NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Demora WHERE IdDemora = @id", Conexion);
            Command.Parameters.Add(new SqlParameter("@id", IdDemora));
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
                IdDemora = Registro.IdDemora;
                Codigo = Registro.Codigo;
                Clasificacion = Registro.Clasificacion;
                Descripcion = Registro.Descripcion;
                Activo = Registro.Activo;
                Valid = true;
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdDemora = 0;
            Codigo = "";
            Clasificacion = null;
            Descripcion = null;
            Activo = false;
            Valid = false;
        }
        public static List<Demora> GetDemoras() {
            List<Demora> demoras = new List<Demora>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Demora", Conexion));
            foreach (var reg in res.Rows) {
                Demora demora = JsonConvert.DeserializeObject<Demora>(JsonConvert.SerializeObject(reg));
                demora.Valid = true;
                demoras.Add(demora);
            }
            return demoras;
        }
    }
}