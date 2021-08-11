using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ATSM.Ingenieria {
	public class ComponenteMenorVinculado {
		private static SqlConnection Conexion = DataBase.Conexion();
		//public int Id { get; set; }
		public int IdComponenteMenor { get; set; }
		public int IdVinculado { get; set; }
		public bool Valid { get; set; }
		public ComponenteMenor Componente { get; set; }
		public ComponenteMenor Vinculado { get; set; }
		public ComponenteMenorVinculado() { Inicializar(); }
        [JsonConstructor]
        public ComponenteMenorVinculado(int idcomponente, int idvinculado) {
			IdComponenteMenor = idcomponente;
			IdVinculado = idvinculado;
            if (idcomponente > 0 && idvinculado > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ComponenteMenorVinculado WHERE IdComponenteMenor = @idcm AND IdVinculado = @idv", Conexion);
                comando.Parameters.Add(new SqlParameter("@idcm", idcomponente));
                comando.Parameters.Add(new SqlParameter("@idv", idvinculado));
                RespuestaQuery res = DataBase.Query(comando);
                Valid = res.Valid;
            }
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if(IdComponenteMenor == IdVinculado) {
                res.Error = "No se Puede Vincular un Componente Sobre Si mismo";
                return res;
			}
            if (IdComponenteMenor > 0 && IdVinculado>0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT * FROM ComponenteMenorVinculado WHERE IdComponenteMenor = @idcm AND IdVinculado = @idv", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idcm", IdComponenteMenor));
                Cmnd.Parameters.Add(new SqlParameter("@idv", IdVinculado));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Vinculado ";
                string SqlStr = "";
                if (!existe.Valid) {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO ComponenteMenorVinculado(IdComponenteMenor, IdVinculado) VALUES(@idcomponentemenor, @idvinculado)";
                    res.Mensaje += "Registrada Correctamente";
                    SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                    Command.Parameters.Add(new SqlParameter("@idcomponentemenor", IdComponenteMenor));
                    Command.Parameters.Add(new SqlParameter("@idvinculado", IdVinculado));
                    RespuestaQuery rInUp = DataBase.Execute(Command);
                    if (rInUp.Valid) {
                        Valid = true;
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                        return res;
                    }
                    res.Elemento = this;
                    res.Valid = true;
                }
            }
            else {
                if (IdComponenteMenor<=0)
                    res.Error += $"<br>Falta el Componente";
                if (IdVinculado <= 0)
                    res.Error += $"<br>Falta el Vinculado";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Vinculado NO se Elimino");
            SqlCommand Cmnd = new SqlCommand($"DELETE ComponenteMenorVinculado WHERE IdComponenteMenor = @idcm AND IdVinculado = @idv", Conexion);
            Cmnd.Parameters.Add(new SqlParameter("@idcm", IdComponenteMenor));
            Cmnd.Parameters.Add(new SqlParameter("@idv", IdVinculado));
            var resD = DataBase.Execute(Cmnd);
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
        private void Inicializar() {
            IdComponenteMenor = 0;
            IdVinculado = 0;
            Componente = new ComponenteMenor();
            Vinculado = new ComponenteMenor();
        }
        public static List<ComponenteMenorVinculado> GetComponenteMenorVinculados(int idComponenteMenor) {
            List<ComponenteMenorVinculado> vinculados = new List<ComponenteMenorVinculado>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ComponenteMenorVinculado WHERE IdComponenteMenor = @idComponenteMenor", Conexion);
            comando.Parameters.AddWithValue("@idComponenteMenor", idComponenteMenor);
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ComponenteMenorVinculado vinculado = JsonConvert.DeserializeObject<ComponenteMenorVinculado>(JsonConvert.SerializeObject(reg));
                vinculado.Valid = true;
                vinculado.SetVinculado();
                vinculados.Add(vinculado);
            }
            return vinculados;
        }
        public void SetVinculado() {
			Vinculado = new ComponenteMenor(IdVinculado);
		}
		public void SetComponente() {
			Componente = new ComponenteMenor(IdComponenteMenor);
		}
	}
}