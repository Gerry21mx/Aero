using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

using WebMatrix.WebData;

namespace ATSM.Ingenieria {
	public class Tiempos {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdLimite { get; set; }
		public int? IdItemMayor { get; set; }
		public int? IdItemMenor { get; set; }
		public int? Limite_Individual_Horas { get; set; }
		public int? Limite_Individual_Ciclos { get; set; }
		public int? Limite_Individual_Dias { get; set; }
		public decimal? Horas_Last { get; set; }
		public int? Ciclos_Last { get; set; }
		public DateTime? Fecha_Last { get; set; }
        public decimal? Horas_TSO_Instalacion { get; set; }
        public int? Ciclos_TSO_Instalacion { get; set; }
        public int? Dias_TSO_Instalacion { get; set; }
        public decimal? Horas_Elapsed { get; set; }
		public int? Ciclos_Elapsed { get; set; }
		public int? Dias_Elapsed { get; set; }
		public decimal? Horas_Next { get; set; }
		public int? Ciclos_Next { get; set; }
		public DateTime? Fecha_Next { get; set; }
		public decimal? Horas_Remain { get; set; }
		public int? Ciclos_Remain { get; set; }
		public int? Dias_Remain { get; set; }
		public int Usuario { get; set; }
		public DateTime Fecha { get; set; }
		public bool Valid { get; set; }
        public Limite Limite { get; set; }
        public Tiempos(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Tiempos WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
		public Tiempos(int idLimite, int idItem, int tipo = 0) {
            Inicializar();
            if (idItem > 0 && tipo > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Tiempos WHERE IdLimite = @idLimite AND {(tipo==1?"IdItemMayor":"IdItemMenor")} = @idItem", Conexion);
                comando.Parameters.Add(new SqlParameter("@idLimite", idLimite));
                comando.Parameters.Add(new SqlParameter("@idItem", idItem));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Tiempos(int id, int idLimite, int? idItemMayor, int? idItemMenor, int? limite_Individual_Horas = null, int? limite_Individual_Ciclos = null, int? limite_Individual_Dias = null, decimal? horas_Last = null, int? ciclos_Last = null, DateTime? fecha_Last = null, decimal? horas_Elapsed = null, int? ciclos_Elapsed = null, int? dias_Elapsed = null,  decimal? horas_TSO_Instalacion = null, int? ciclos_TSO_Instalacion = null, int? dias_TSO_Instalacion = null, decimal? horas_Next = null, int? ciclos_Next = null, DateTime? fecha_Next = null, decimal? horas_Remain = null, int? ciclos_Remain = null, int? dias_Remain = null, int usuario = 0, DateTime? fecha = null) {
            Id = id;
            IdLimite = idLimite;
            IdItemMayor = idItemMayor;
            IdItemMenor = idItemMenor;
            Limite_Individual_Horas = limite_Individual_Horas;
            Limite_Individual_Ciclos = limite_Individual_Ciclos;
            Limite_Individual_Dias = limite_Individual_Dias;
            Horas_Last = horas_Last;
            Ciclos_Last = ciclos_Last;
            Fecha_Last = fecha_Last;
            Horas_TSO_Instalacion = horas_TSO_Instalacion;
            Ciclos_TSO_Instalacion = ciclos_TSO_Instalacion;
            Dias_TSO_Instalacion = dias_TSO_Instalacion;
            Horas_Elapsed = horas_Elapsed;
            Ciclos_Elapsed = ciclos_Elapsed;
            Dias_Elapsed = dias_Elapsed;
            Horas_Next = horas_Next;
            Ciclos_Next = ciclos_Next;
            Fecha_Next = fecha_Next;
            Horas_Remain = horas_Remain;
            Ciclos_Remain = ciclos_Remain;
            Dias_Remain = dias_Remain;
            Usuario = usuario;
            Fecha = fecha ?? default(DateTime);
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdLimite > 0 && (IdItemMayor > 0 || IdItemMenor > 0) && WebSecurity.IsAuthenticated) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Tiempos WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Tiempos ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Tiempos SET IdLimite = @idlimite, IdItemMayor = @iditemmayor, IdItemMenor = @iditemmenor, Limite_Individual_Horas = @limite_individual_horas, Limite_Individual_Ciclos = @limite_individual_ciclos, Limite_Individual_Dias = @limite_individual_dias, Horas_Last = @horas_last, Ciclos_Last = @ciclos_last, Fecha_Last = @fecha_last, Horas_TSO_Instalacion = @horas_TSO_Instalacion, Ciclos_TSO_Instalacion = @ciclos_TSO_Instalacion, Dias_TSO_Instalacion = @dias_TSO_Instalacion, Horas_Elapsed = @horas_elapsed, Ciclos_Elapsed = @ciclos_elapsed, Dias_Elapsed = @dias_elapsed, Horas_Next = @horas_next, Ciclos_Next = @ciclos_next, Fecha_Next = @fecha_next, Horas_Remain = @horas_remain, Ciclos_Remain = @ciclos_remain, Dias_Remain = @dias_remain, Usuario = @usuario, Fecha = GETDATE() WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Tiempos(IdLimite, IdItemMayor, IdItemMenor, Limite_Individual_Horas, Limite_Individual_Ciclos, Limite_Individual_Dias, Horas_Last, Ciclos_Last, Fecha_Last, Horas_TSO_Instalacion, Ciclos_TSO_Instalacion, Dias_TSO_Instalacion, Horas_Elapsed, Ciclos_Elapsed, Dias_Elapsed, Horas_Next, Ciclos_Next, Fecha_Next, Horas_Remain, Ciclos_Remain, Dias_Remain, Usuario, Fecha) VALUES(@idlimite, @iditemmayor, @iditemmenor, @limite_individual_horas, @limite_individual_ciclos, @limite_individual_dias, @horas_last, @ciclos_last, @fecha_last, @horas_TSO_Instalacion, @ciclos_TSO_Instalacion, @dias_TSO_Instalacion, @horas_elapsed, @ciclos_elapsed, @dias_elapsed, @horas_next, @ciclos_next, @fecha_next, @horas_remain, @ciclos_remain, @dias_remain, @usuario, GETDATE())";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idlimite", IdLimite));
                Command.Parameters.Add(new SqlParameter("@iditemmayor", IdItemMayor ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@iditemmenor", IdItemMenor ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@limite_individual_horas", Limite_Individual_Horas ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@limite_individual_ciclos", Limite_Individual_Ciclos ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@limite_individual_dias", Limite_Individual_Dias ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@horas_last", Horas_Last ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@ciclos_last", Ciclos_Last ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@fecha_last", Fecha_Last ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@horas_TSO_Instalacion", Horas_TSO_Instalacion ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@ciclos_TSO_Instalacion", Ciclos_TSO_Instalacion ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@dias_TSO_Instalacion", Dias_TSO_Instalacion ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@horas_elapsed", Horas_Elapsed ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@ciclos_elapsed", Ciclos_Elapsed ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@dias_elapsed", Dias_Elapsed ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@horas_next", Horas_Next ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@ciclos_next", Ciclos_Next ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@fecha_next", Fecha_Next ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@horas_remain", Horas_Remain ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@ciclos_remain", Ciclos_Remain ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@dias_remain", Dias_Remain ?? SqlInt32.Null));
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
                if (IdLimite <= 0)
                    res.Error += $"<br>Falta el Limite";
                if (IdItemMayor <= 0 && IdItemMenor <= 0)
                    res.Error += $"<br>Falta el Id del Item Vinculado";
                if (!WebSecurity.IsAuthenticated)
                    res.Error += $"<br>Debe Iniciar Sesion Primero";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Tiempos NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Tiempos WHERE Id = @id", Conexion);
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
                IdItemMayor = Registro.IdItemMayor;
                IdItemMenor = Registro.IdItemMenor;
                Limite_Individual_Horas = Registro.Limite_Individual_Horas;
                Limite_Individual_Ciclos = Registro.Limite_Individual_Ciclos;
                Limite_Individual_Dias = Registro.Limite_Individual_Dias;
                Horas_Last = Registro.Horas_Last;
                Ciclos_Last = Registro.Ciclos_Last;
                Fecha_Last = Registro.Fecha_Last;
                Horas_TSO_Instalacion = Registro.Horas_TSO_Instalacion;
                Ciclos_TSO_Instalacion = Registro.Ciclos_TSO_Instalacion;
                Dias_TSO_Instalacion = Registro.Dias_TSO_Instalacion;
                Horas_Elapsed = Registro.Horas_Elapsed;
                Ciclos_Elapsed = Registro.Ciclos_Elapsed;
                Dias_Elapsed = Registro.Dias_Elapsed;
                Horas_Next = Registro.Horas_Next;
                Ciclos_Next = Registro.Ciclos_Next;
                Fecha_Next = Registro.Fecha_Next;
                Horas_Remain = Registro.Horas_Remain;
                Ciclos_Remain = Registro.Ciclos_Remain;
                Dias_Remain = Registro.Dias_Remain;
                Usuario = Registro.Usuario;
                Fecha = Registro.Fecha;
                Valid = true;
                SetLimite();
            }
        }
        private void Inicializar() {
            Id = 0;
            IdLimite = 0;
            IdItemMayor = null;
            IdItemMenor = null;
            Limite_Individual_Horas = null;
            Limite_Individual_Ciclos = null;
            Limite_Individual_Dias = null;
            Horas_Last = null;
            Ciclos_Last = null;
            Fecha_Last = null;
            Horas_TSO_Instalacion = null;
            Ciclos_TSO_Instalacion = null;
            Dias_TSO_Instalacion = null;
            Horas_Elapsed = null;
            Ciclos_Elapsed = null;
            Dias_Elapsed = null;
            Horas_Next = null;
            Ciclos_Next = null;
            Fecha_Next = null;
            Horas_Remain = null;
            Ciclos_Remain = null;
            Dias_Remain = null;
            Usuario = 0;
            Fecha = default(DateTime);
        }
        /// <summary>
        /// Lista de Tiempos de Un Componente
        /// </summary>
        /// <param name="idItem">Id de Item de Componente</param>
        /// <param name="tipoItem">
        /// Tipo de Componente:
        /// <list type="bullet">
        /// <item>0-Item Menor</item>
        /// <item>1-Item Mayor</item>
        /// </list>
        /// </param>
        /// <returns></returns>
        public static List<Tiempos> GetTiempos(int idItem, int tipoItem = 0) {
            List<Tiempos> tiempos = new List<Tiempos>();
            SqlCommand comando = new SqlCommand($"SELECT * FROM Tiempos WHERE {(tipoItem==1?"IdItemMayor":"IdItemMenor")} = @idItem", Conexion);
            comando.Parameters.Add(new SqlParameter("@idItem", idItem));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                Tiempos tiempo = JsonConvert.DeserializeObject<Tiempos>(JsonConvert.SerializeObject(reg));
                tiempo.Valid = true;
                tiempos.Add(tiempo);
            }
            return tiempos;
        }
        public void SetLimite() {
            Limite = new Limite(IdLimite);
		}
    }
}