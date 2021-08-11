using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class Limite {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdLimite { get; set; }
		public int? IdComponenteMayor { get; set; }
		public int? IdModelo { get; set; }
		public int? IdComponenteMenor { get; set; }
		public int? Horas { get; set; }
		public int? Ciclos { get; set; }
		public int? Dias { get; set; }
		public bool? Activo { get; set; }
		public bool Valid { get; set; }
        public Limites Limit { get; set; }
        public Limite() { Inicializar(); }
        public Limite(int id) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Limite WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Limite(int idlimite, int idcomponente, int idmodelo, int tipo = 0) {
            Inicializar();
            if (idlimite > 0 && idcomponente > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Limite WHERE IdLimite = @idLimte AND {(tipo==0? "IdComponenteMenor" : "IdModelo = @idmodelo AND IdComponenteMayor")} = @idcomponente", Conexion);
                comando.Parameters.Add(new SqlParameter("@idLimte", idlimite));
                comando.Parameters.Add(new SqlParameter("@idcomponente", idcomponente));
                comando.Parameters.Add(new SqlParameter("@idmodelo", idmodelo));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Limite(int id, int idLimte, int? idComponenteMayor, int? idModelo, int? idComponenteMenor, int? horas, int? ciclos = 0, int? dias = 0, bool? activo = null) {
            Id = id;
            IdLimite = idLimte;
            IdComponenteMayor = idComponenteMayor;
            IdModelo = idModelo;
            IdComponenteMenor = idComponenteMenor;
            Horas = horas;
            Ciclos = ciclos;
            Dias = dias;
            Activo = activo;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdLimite > 0 && (IdComponenteMenor > 0 || (IdComponenteMayor > 0 && IdModelo > 0))) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT * FROM Limite WHERE Id = @id OR (IdLimite = @idLimte AND {(IdComponenteMenor > 0 ? "IdComponenteMenor = @idComponenteMenor" : "IdComponenteMayor = @idComponenteMayor AND IdModelo = @idmodelo")})", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@idLimte", IdLimite));
                Cmnd.Parameters.Add(new SqlParameter("@idComponenteMenor", IdComponenteMenor ?? SqlInt32.Null));
                Cmnd.Parameters.Add(new SqlParameter("@idComponenteMayor", IdComponenteMayor ?? SqlInt32.Null));
                Cmnd.Parameters.Add(new SqlParameter("@idmodelo", IdModelo ?? SqlInt32.Null));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Limite ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Limite SET IdLimite = @idlimte, IdComponenteMayor = @idComponenteMayor, IdModelo = @idModelo, IdComponenteMenor = @idcomponentemenor, Horas = @horas, Ciclos = @ciclos, Dias = @dias, Activo = @activo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Limite(IdLimite, IdComponenteMayor, IdModelo, IdComponenteMenor, Horas, Ciclos, Dias, Activo) VALUES(@idlimte, @idComponenteMayor, @idModelo, @idcomponentemenor, @horas, @ciclos, @dias, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idlimte", IdLimite));
                Command.Parameters.Add(new SqlParameter("@idcomponentemenor", IdComponenteMenor ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idComponenteMayor", IdComponenteMayor ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idModelo", IdModelo ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@horas", Horas ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@ciclos", Ciclos ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@dias", Dias ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@activo", Activo ?? SqlBoolean.Null));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid) {
                    Valid = true;
                }
                else {
                    res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                    return res;
                }
                SetLimit();
                res.Elemento = this;
                res.Valid = true;
            }
            else {
                if (IdLimite<=0)
                    res.Error += $"<br>Falta el Limite";
                if (IdComponenteMenor<=0 && IdComponenteMayor <= 0)
                    res.Error += $"<br>Falta especificar el Componente.";
                if (IdComponenteMenor == 0 && IdModelo<=0)
                    res.Error += $"<br>Falta el Modelo";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Limite NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Limite WHERE Id = @id", Conexion);
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
                IdLimite = Registro.IdLimite;
                IdComponenteMayor = Registro.IdComponenteMayor;
                IdModelo = Registro.IdModelo;
                IdComponenteMenor = Registro.IdComponenteMenor;
                Horas = Registro.Horas;
                Ciclos = Registro.Ciclos;
                Dias = Registro.Dias;
                Activo = Registro.Activo;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            IdLimite = 0;
            IdComponenteMayor = null;
            IdModelo = null;
            IdComponenteMenor = null;
            Horas = null;
            Ciclos = null;
            Dias = null;
            Activo = null;
        }
        /// <summary>
        /// Obtiene los Limites Especificados de Un Componente
        /// </summary>
        /// <param name="idcomponente">Id del Componente</param>
        /// <param name="tipo">Tipo del Componente
        /// <list type="bullet">
        /// <item>0 - Componente Menor</item>
        /// <item>1 - Componente Mayor</item>
        /// </list>
        /// </param>
        /// <returns>Lista de Limites</returns>
        public static List<Limite> GetLimites(int idcomponente, int? idmodelo = 0, int tipo = 0) {
            List<Limite> limites = new List<Limite>();
            SqlCommand command = new SqlCommand($"SELECT * FROM Limite WHERE {(tipo==0? "IdComponenteMenor" : "IdModelo = @idModelo AND IdComponenteMayor")} = @idcomponente", Conexion);
            command.Parameters.AddWithValue("@idcomponente", idcomponente);
            command.Parameters.AddWithValue("@idModelo", idmodelo ?? SqlInt32.Null);
            RespuestaQuery res = DataBase.Query(command);
            foreach (var reg in res.Rows) {
                Limite limite = JsonConvert.DeserializeObject<Limite>(JsonConvert.SerializeObject(reg));
                limite.Valid = true;
                limites.Add(limite);
            }
            return limites;
        }
        public void SetLimit() {
            Limit = new Limites(IdLimite);
		}
    }
}