using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Newtonsoft.Json;

namespace ATSM.Seguimiento {
	public class VueloDemora {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; private set; }
		public int IdTramo { get; set; }
		public int IdDemora { get; set; }
		public string Observaciones { get; set; }
		public bool Valid { get; set; }
        public VueloDemora(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM VueloDemora WHERE id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public VueloDemora(int id, int idTramo = 0, int idDemora = 0, string observaciones = "", bool valid = false) {
            Id = id;
            IdTramo = idTramo;
            IdDemora = idDemora;
            Observaciones = observaciones;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdTramo>0 && IdDemora>0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT id FROM VueloDemora WHERE id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Demora de Vuelo ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE VueloDemora SET IdTramo = @idtramo, IdDemora = @iddemora, Observaciones = @observaciones WHERE id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO VueloDemora(IdTramo, IdDemora, Observaciones) VALUES(@idtramo, @iddemora, @observaciones)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idtramo", IdTramo));
                Command.Parameters.Add(new SqlParameter("@iddemora", IdDemora));
                Command.Parameters.Add(new SqlParameter("@observaciones", string.IsNullOrEmpty(Observaciones) ? SqlString.Null : Observaciones));
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
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Demora de Vuelo NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE VueloDemora WHERE id = @id", Conexion);
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
                IdDemora = Registro.IdDemora;
                Observaciones = Registro.Observaciones;
                Valid = true;
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            Id = 0;
            IdTramo = 0;
            IdDemora = 0;
            Observaciones = null;
            Valid = false;
        }
        public static List<VueloDemora> GetDemorasTramo(int idtramo) {
            List<VueloDemora> vuelodemoras = new List<VueloDemora>();
            SqlCommand comando = new SqlCommand("SELECT * FROM VueloDemora WHERE IdTramo = @idtramo", Conexion);
            comando.Parameters.Add(new SqlParameter("@idtramo", idtramo));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                VueloDemora vuelodemora = JsonConvert.DeserializeObject<VueloDemora>(JsonConvert.SerializeObject(reg));
                vuelodemora.Valid = true;
                vuelodemoras.Add(vuelodemora);
            }
            return vuelodemoras;
        }
    }
}