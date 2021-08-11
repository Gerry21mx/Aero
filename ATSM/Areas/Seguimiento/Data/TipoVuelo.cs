using System.Collections.Generic;
using System.Data.SqlClient;

using Newtonsoft.Json;

namespace ATSM.Seguimiento {
	public class TipoVuelo {
        private static SqlConnection Conexion = DataBase.Conexion();
        public int IdTipo { get; set; }
		public string Descripcion { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; set; }
        public TipoVuelo(int? idtipo = null) {
            Inicializar();
            if (idtipo > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM TipoVuelo WHERE IdTipo = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", idtipo));
                SetDatos(comando);
            }
        }
        public TipoVuelo(string descripcion) {
            Inicializar();
            if (!string.IsNullOrEmpty(descripcion)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM TipoVuelo WHERE Descripcion = @des", Conexion);
                comando.Parameters.Add(new SqlParameter("@des", descripcion));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public TipoVuelo(int idTipo, string descripcion, bool activo = false, bool valid = false) {
            IdTipo = idTipo;
            Descripcion = descripcion;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Descripcion)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT IdTipo FROM TipoVuelo WHERE IdTipo = @idtipo OR Descripcion = @descripcion", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idtipo", IdTipo));
                Cmnd.Parameters.Add(new SqlParameter("@descripcion", Descripcion));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Tipo de Vuelo ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE TipoVuelo SET Descripcion = @descripcion, Activo = @activo WHERE IdTipo=@idtipo";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO TipoVuelo(Descripcion, Activo) VALUES(@descripcion, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idtipo", IdTipo));
                Command.Parameters.Add(new SqlParameter("@descripcion", Descripcion));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid) {
                    if (Insr) {
                        if (rInUp.IdRegistro == 0) {
                            res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
                            return res;
                        }
                        IdTipo = rInUp.IdRegistro;
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
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Tipo de Vuelo NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE TipoVuelo WHERE IdTipo = @id", Conexion);
            Command.Parameters.Add(new SqlParameter("@id", IdTipo));
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
                IdTipo = Registro.IdTipo;
                Descripcion = Registro.Descripcion;
                Activo = Registro.Activo;
                Valid = true;
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdTipo = 0;
            Descripcion = "";
            Activo = false;
            Valid = false;
        }
        public static List<TipoVuelo> GetTipoVuelos(bool? activos=null) {
            List<TipoVuelo> tipovuelos = new List<TipoVuelo>();
            RespuestaQuery res = DataBase.Query(new SqlCommand($"SELECT * FROM TipoVuelo{(activos == true ? " WHERE Activo = 1" : "")}", Conexion));
            foreach (var reg in res.Rows) {
                TipoVuelo tipovuelo = JsonConvert.DeserializeObject<TipoVuelo>(JsonConvert.SerializeObject(reg));
                tipovuelo.Valid = true;
                tipovuelos.Add(tipovuelo);
            }
            return tipovuelos;
        }
    }
}