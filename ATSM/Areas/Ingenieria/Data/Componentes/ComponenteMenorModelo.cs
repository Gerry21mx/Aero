using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ATSM.Ingenieria {
	public class ComponenteMenorModelo {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdComponenteMayor { get; set; }
		public int IdComponenteMenor { get; set; }
		public int IdModelo { get; set; }
		public int Cantidad { get; set; }
		public bool Valid { get; set; }
		public Modelo Modelo { get; set; }
		public ComponenteMayor Mayor { get; set; }
		public ComponenteMenor Menor { get; set; }
        public ComponenteMenorModelo(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ComponenteMenorModelo WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public ComponenteMenorModelo(int idMayor, int idMenor, int idModelo) {
            Inicializar();
            if (idMayor > 0 && idMenor > 0 && idModelo > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ComponenteMenorModelo WHERE IdComponenteMayor = @mayor AND IdComponenteMenor = @menor AND IdModelo = @modelo", Conexion);
                comando.Parameters.Add(new SqlParameter("@mayor", idMayor));
                comando.Parameters.Add(new SqlParameter("@menor", idMenor));
                comando.Parameters.Add(new SqlParameter("@modelo", idModelo));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public ComponenteMenorModelo(int id, int idComponenteMayor, int idComponenteMenor, int idModelo, int cantidad = 0, bool valid = false) {
            Id = id;
            IdComponenteMayor = idComponenteMayor;
            IdComponenteMenor = idComponenteMenor;
            IdModelo = idModelo;
            Cantidad = cantidad;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdComponenteMayor > 0 && IdComponenteMenor > 0 && IdModelo > 0 && Cantidad > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM ComponenteMenorModelo WHERE Id = @id OR (IdComponenteMayor = @mayor AND IdComponenteMenor = @menor AND IdModelo = @modelo)", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@mayor", IdComponenteMayor));
                Cmnd.Parameters.Add(new SqlParameter("@menor", IdComponenteMenor));
                Cmnd.Parameters.Add(new SqlParameter("@modelo", IdModelo));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "ComponenteMenorModelo ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE ComponenteMenorModelo SET IdComponenteMayor = @idcomponentemayor, IdComponenteMenor = @idcomponentemenor, IdModelo = @idmodelo, Cantidad = @cantidad WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO ComponenteMenorModelo(IdComponenteMayor, IdComponenteMenor, IdModelo, Cantidad) VALUES(@idcomponentemayor, @idcomponentemenor, @idmodelo, @cantidad)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idcomponentemayor", IdComponenteMayor));
                Command.Parameters.Add(new SqlParameter("@idcomponentemenor", IdComponenteMenor));
                Command.Parameters.Add(new SqlParameter("@idmodelo", IdModelo));
                Command.Parameters.Add(new SqlParameter("@cantidad", Cantidad));
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
                if (IdComponenteMayor <= 0)
                    res.Error += $"<br>Falta el Componente Mayor";
                if (IdComponenteMenor <= 0)
                    res.Error += $"<br>Falta el Componente Menor";
                if (IdModelo <= 0)
                    res.Error += $"<br>Falta el Modelo";
                if (Cantidad <= 0)
                    res.Error += $"<br>Falta la Cantidad";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("ComponenteMenorModelo NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE ComponenteMenorModelo WHERE Id = @id", Conexion);
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
                IdComponenteMayor = Registro.IdComponenteMayor;
                IdComponenteMenor = Registro.IdComponenteMenor;
                IdModelo = Registro.IdModelo;
                Cantidad = Registro.Cantidad;
                Valid = true;
                SetMayor();
                SetMenor();
                SetModelo();
            }
        }
        private void Inicializar() {
            Id = 0;
            IdComponenteMayor = 0;
            IdComponenteMenor = 0;
            IdModelo = 0;
            Cantidad = 0;
            Valid = false;
            Modelo = new Modelo();
        }
        public static List<ComponenteMenorModelo> GetModelosMenor(int idMenor) {
            List<ComponenteMenorModelo> componentemenormodelos = new List<ComponenteMenorModelo>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ComponenteMenorModelo WHERE IdComponenteMenor = @menor", Conexion);
            comando.Parameters.Add(new SqlParameter("@menor", idMenor));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ComponenteMenorModelo cMenorModelo = JsonConvert.DeserializeObject<ComponenteMenorModelo>(JsonConvert.SerializeObject(reg));
                cMenorModelo.Valid = true;
                componentemenormodelos.Add(cMenorModelo);
            }
            return componentemenormodelos;
        }
        public static List<ComponenteMenorModelo> GetComponenteMenorModelos(int idMayor, int idModelo) {
            ComponenteMayor Mayor = new ComponenteMayor(idMayor);
            Modelo Modelo = new Modelo(idModelo);
            List<ComponenteMenorModelo> componentemenormodelos = new List<ComponenteMenorModelo>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ComponenteMenorModelo WHERE IdComponenteMayor = @mayor AND IdModelo = @modelo", Conexion);
            comando.Parameters.Add(new SqlParameter("@mayor", idMayor));
            comando.Parameters.Add(new SqlParameter("@modelo", idModelo));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ComponenteMenorModelo cMenorModelo = JsonConvert.DeserializeObject<ComponenteMenorModelo>(JsonConvert.SerializeObject(reg));
                cMenorModelo.Mayor = Mayor;
                cMenorModelo.Modelo = Modelo;
                cMenorModelo.SetMenor();
                cMenorModelo.Valid = true;
                componentemenormodelos.Add(cMenorModelo);
            }
            return componentemenormodelos;
        }
        public void SetModelo() {
			Modelo = new Modelo(IdModelo);
		}
		public void SetMayor() {
			Mayor = new ComponenteMayor(IdComponenteMayor);
		}
		public void SetMenor() {
			Menor = new ComponenteMenor(IdComponenteMenor);
		}
	}
}