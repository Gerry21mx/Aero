using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace ATSM.Almacen {
	public class UnidadMedida {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; private set; }
		public string Codigo { get; set; }
		public string Descripcion { get; set; }
		public int IdTipo { get; set; }
		public int Usuario { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; private set; }
		public UnidadMedidaTipo Tipo { get; private set; }
        public UnidadMedida(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM UnidadMedida WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public UnidadMedida(string cadena) {
            Inicializar();
            if (!string.IsNullOrEmpty(cadena)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM UnidadMedida WHERE Codigo = @cadena", Conexion);
                comando.Parameters.Add(new SqlParameter("@cadena", cadena));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public UnidadMedida(int id, string codigo, string descripcion, int idTipo = 0, int usuario = 0, bool activo = false, bool valid = false) {
            Id = id;
            Codigo = codigo;
            Descripcion = descripcion;
            IdTipo = idTipo;
            Usuario = usuario;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Codigo) && !string.IsNullOrEmpty(Descripcion)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM UnidadMedida WHERE Id = @id OR Codigo = @cod", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@cod", Codigo));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "UnidadMedida ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE UnidadMedida SET Codigo = @codigo, Descripcion = @descripcion, IdTipo = @idtipo, Usuario = @usuario, Activo = @activo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO UnidadMedida(Codigo, Descripcion, IdTipo, Usuario, Activo) VALUES(@codigo, @descripcion, @idtipo, @usuario, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@codigo", string.IsNullOrEmpty(Codigo) ? SqlString.Null : Codigo));
                Command.Parameters.Add(new SqlParameter("@descripcion", string.IsNullOrEmpty(Descripcion) ? SqlString.Null : Descripcion));
                Command.Parameters.Add(new SqlParameter("@idtipo", IdTipo));
                Command.Parameters.Add(new SqlParameter("@usuario", Usuario));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
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
                SetTipo();
                res.Elemento = this;
                res.Valid = true;
            }
            else {
                if (!string.IsNullOrEmpty(Codigo))
                    res.Error += $"<br>Falta el Codigo de La Unidad de Medida";
                if (!string.IsNullOrEmpty(Descripcion))
                    res.Error += $"<br>Falta la Descripcion de La Unidad de Medida";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("UnidadMedida NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE UnidadMedida WHERE Id = @id", Conexion);
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
                Descripcion = Registro.Descripcion;
                IdTipo = Registro.IdTipo;
                Usuario = Registro.Usuario;
                Activo = Registro.Activo;
                Valid = true;
                SetTipo();
            }
        }
        private void Inicializar() {
            Id = 0;
            Codigo = "";
            Descripcion = "";
            IdTipo = 0;
            Usuario = 0;
            Activo = false;
            Valid = false;
        }
        public static List<UnidadMedida> GetUnidadMedidas() {
            List<UnidadMedida> unidadmedidas = new List<UnidadMedida>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM UnidadMedida", Conexion));
            foreach (var reg in res.Rows) {
                UnidadMedida unidadmedida = JsonConvert.DeserializeObject<UnidadMedida>(JsonConvert.SerializeObject(reg));
                unidadmedida.Valid = true;
                unidadmedidas.Add(unidadmedida);
            }
            return unidadmedidas;
        }
        public void SetTipo() {
            Tipo = new UnidadMedidaTipo(IdTipo);
		}
    }
}