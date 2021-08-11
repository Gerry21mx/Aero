using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ATSM.Ingenieria {
	public class Modelo {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Nombre { get; set; }
		public string Fabricante { get; set; }
		public int IdCapacidad { get; set; }
		public int IdComponenteMayor { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; set; }
        public List<Limite> Limites { get; set; }
        public Capacidad Capacidad { get; set; }
		public ComponenteMayor ComponenteMayor { get; set; }
		public Modelo(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Modelo WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Modelo(string nombre, int idComponenteMayor) {
            Inicializar();
            if (!string.IsNullOrEmpty(nombre) && idComponenteMayor > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Modelo WHERE Nombre = @nombre AND IdComponenteMayor = @idcm", Conexion);
                comando.Parameters.Add(new SqlParameter("@nombre", nombre));
                comando.Parameters.Add(new SqlParameter("@idcm", idComponenteMayor));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Modelo(int id, string nombre, string fabricante = "", int idCapacidad = 0, int idComponenteMayor = 0, bool activo = false, bool valid = false) {
            Id = id;
            Nombre = nombre;
            Fabricante = fabricante;
            IdCapacidad = idCapacidad;
            IdComponenteMayor = idComponenteMayor;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nombre) && IdCapacidad > 0 && IdComponenteMayor > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Modelo WHERE Id = @id OR (Nombre = @nom AND IdComponenteMayor = @idcm)", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@nom", Nombre));
                Cmnd.Parameters.Add(new SqlParameter("@idcm", IdComponenteMayor));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Modelo ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Modelo SET Nombre = @nombre, Fabricante = @fabricante, IdCapacidad = @idcapacidad, IdComponenteMayor = @idcomponentemayor, Activo = @activo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Modelo(Nombre, Fabricante, IdCapacidad, IdComponenteMayor, Activo) VALUES(@nombre, @fabricante, @idcapacidad, @idcomponentemayor, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@nombre", string.IsNullOrEmpty(Nombre) ? SqlString.Null : Nombre));
                Command.Parameters.Add(new SqlParameter("@fabricante", string.IsNullOrEmpty(Fabricante) ? SqlString.Null : Fabricante));
                Command.Parameters.Add(new SqlParameter("@idcapacidad", IdCapacidad));
                Command.Parameters.Add(new SqlParameter("@idcomponentemayor", IdComponenteMayor));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                using(TransactionScope scope=new TransactionScope()) {
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
                        else {
                            Cmnd = new SqlCommand("DELETE Limite WHERE IdComponenteMayor = @idmayor AND IdModelo = @idmodelo;", Conexion);
                            Cmnd.Parameters.Add(new SqlParameter("@idmodelo", Id));
                            Cmnd.Parameters.Add(new SqlParameter("@idmayor", IdComponenteMayor));
                            var rDH = DataBase.Execute(Cmnd);
                            if (!rDH.Valid || !string.IsNullOrEmpty(rDH.Error)) {
                                res.Error = $"Error al limpiar el Historial: (CS.{this.GetType().Name}-Save.Err.03)<br>Error: {rDH.Error}";
                                return res;
                            }
                        }
                        foreach (var lim in Limites) {
                            lim.IdModelo = Id;
                            lim.IdComponenteMayor = IdComponenteMayor;
                            lim.IdComponenteMenor = null;
                            var rSL = lim.Save();
                            if (!rSL.Valid || !string.IsNullOrEmpty(rSL.Error)) {
                                res.Error = $"Error al Guardar Limites: (CS.{this.GetType().Name}-Save.Err.05)<br>Error: {rSL.Error}";
                                return res;
                            }
                        }
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                        return res;
                    }
                    SetCapacidad();
                    SetComponentesMayores();
                    res.Elemento = this;
                    res.Valid = true;
                    scope.Complete();
				}
            }
            else {
                if (!string.IsNullOrEmpty(Nombre))
                    res.Error += $"<br>Falta el Valor de Nombre";
                if (IdCapacidad <=0)
                    res.Error += $"<br>Falta la Capacidad";
                if (IdComponenteMayor <= 0)
                    res.Error += $"<br>Falta el Componente Mayor Vinculado";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Modelo NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Modelo WHERE Id = @id", Conexion);
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
                Nombre = Registro.Nombre;
                Fabricante = Registro.Fabricante;
                IdCapacidad = Registro.IdCapacidad;
                IdComponenteMayor = Registro.IdComponenteMayor;
                Activo = Registro.Activo;
                Valid = true;
                SetCapacidad();
                SetComponentesMayores();
                SetLimites();
            }
        }
        private void Inicializar() {
            Id = 0;
            Nombre = "";
            Fabricante = "";
            IdCapacidad = 0;
            Activo = false;
            Valid = false;
            Limites = new List<Limite>();
        }
        public static List<Modelo> GetModelos() {
            List<Modelo> modelos = new List<Modelo>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Modelo", Conexion));
            foreach (var reg in res.Rows) {
                Modelo modelo = JsonConvert.DeserializeObject<Modelo>(JsonConvert.SerializeObject(reg));
                modelo.Valid = true;
                modelo.SetCapacidad();
                modelo.SetLimites();
                modelo.SetComponentesMayores();
                modelos.Add(modelo);
            }
            return modelos;
        }
        public void SetCapacidad() {
            Capacidad = new Capacidad(IdCapacidad);
		}
        public void SetComponentesMayores() {
            ComponenteMayor = new ComponenteMayor(IdComponenteMayor);
		}
        public void SetLimites() {
            Limites = Limite.GetLimites(IdComponenteMayor, Id, 1);
            foreach (var lim in Limites) {
                lim.SetLimit();
            }
        }
    }
}