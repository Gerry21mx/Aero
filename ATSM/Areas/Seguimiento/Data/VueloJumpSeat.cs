using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace ATSM.Seguimiento {
	public class VueloJumpSeat {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdTramo { get; set; }
		public string Nombre { get; set; }
		public bool Valid { get; set; }
        public VueloJumpSeat(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM VueloJumpSeat WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public VueloJumpSeat(int idtramo, string nombre) {
            Inicializar();
            if (idtramo > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM VueloJumpSeat WHERE IdTramo = @idtramo AND Nombre = @nombre", Conexion);
                comando.Parameters.Add(new SqlParameter("@idtramo", idtramo));
                comando.Parameters.Add(new SqlParameter("@nombre", string.IsNullOrEmpty(nombre) ? SqlString.Null : nombre));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public VueloJumpSeat(int id, int idTramo, string nombre, bool valid = false) {
            Id = id;
            IdTramo = idTramo;
            Nombre = nombre;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nombre) && IdTramo > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM VueloJumpSeat WHERE Id = @id OR (IdTramo = @idtramo AND Nombre = @nombre)", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@idtramo", IdTramo));
                Cmnd.Parameters.Add(new SqlParameter("@nombre", string.IsNullOrEmpty(Nombre) ? SqlString.Null : Nombre));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "VueloJumpSeat ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE VueloJumpSeat SET IdTramo = @idtramo, Nombre = @nombre WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO VueloJumpSeat(IdTramo, Nombre) VALUES(@idtramo, @nombre)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idtramo", IdTramo));
                Command.Parameters.Add(new SqlParameter("@nombre", string.IsNullOrEmpty(Nombre) ? SqlString.Null : Nombre));
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
                if (string.IsNullOrEmpty(Nombre))
                    res.Error += "<br>Falta Nombre del Pasajero.";
                if (IdTramo<=0)
                    res.Error += "<br>Falta Tramo Volado.";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("VueloJumpSeat NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE VueloJumpSeat WHERE Id = @id", Conexion);
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
                IdTramo = Registro.IdTramo;
                Nombre = Registro.Nombre;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            IdTramo = 0;
            Nombre = "";
            Valid = false;
        }
        public static List<VueloJumpSeat> GetVueloJumpSeats(int idtramo) {
            List<VueloJumpSeat> vuelojumpseats = new List<VueloJumpSeat>();
            SqlCommand comando = new SqlCommand("SELECT * FROM VueloJumpSeat WHERE IdTramo = @idtramo", Conexion);
            comando.Parameters.Add(new SqlParameter("@idtramo", idtramo));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                VueloJumpSeat vuelojumpseat = JsonConvert.DeserializeObject<VueloJumpSeat>(JsonConvert.SerializeObject(reg));
                vuelojumpseat.Valid = true;
                vuelojumpseats.Add(vuelojumpseat);
            }
            return vuelojumpseats;
        }
    }
}