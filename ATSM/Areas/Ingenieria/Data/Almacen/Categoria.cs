using Newtonsoft.Json;

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using WebMatrix.WebData;

namespace ATSM.Almacen {
	public class Categoria {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; private set; }
		public string Codigo { get; set; }
		public string Descripcion { get; set; }
		public int? Jerarquia { get; set; }
		public int? Orden { get; set; }
		public int Usuario { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; private set; }
        public Categoria(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Categoria WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Categoria(string cadena) {
            Inicializar();
            if (!string.IsNullOrEmpty(cadena)) {
                string query = $"SELECT * FROM Categoria WHERE Codigo = @cadena || Descripcion = @cadena";
                SqlCommand Cmnd = new SqlCommand(query, DataBase.Conexion());
                Cmnd.Parameters.Add(new SqlParameter("@cadena", cadena));
                SetDatos(Cmnd);
            }
        }
        [JsonConstructor]
        public Categoria(int id, string codigo, string descripcion, int? jerarquia = null, int? orden = null, int usuario = 0, bool activo = false, bool valid = false) {
            Id = id;
            Codigo = codigo;
            Descripcion = descripcion;
            Jerarquia = jerarquia;
            Orden = orden;
            Usuario = usuario;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Codigo) && !string.IsNullOrEmpty(Descripcion)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Categoria WHERE Id = @id OR Codigo = @cod", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@cod", Codigo));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Categoria ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Categoria SET Codigo = @codigo, Descripcion = @descripcion, Jerarquia = @jerarquia, Orden = @orden, Usuario = @usuario, Activo = @activo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Categoria(Codigo, Descripcion, Jerarquia, Orden, Usuario, Activo) VALUES(@codigo, @descripcion, @jerarquia, @orden, @usuario, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@codigo", string.IsNullOrEmpty(Codigo) ? SqlString.Null : Codigo));
                Command.Parameters.Add(new SqlParameter("@descripcion", string.IsNullOrEmpty(Descripcion) ? SqlString.Null : Descripcion));
                Command.Parameters.Add(new SqlParameter("@jerarquia", Jerarquia ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@orden", Orden ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@usuario", WebSecurity.CurrentUserId));
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
                res.Elemento = this;
                res.Valid = true;
            }
            else {
                if (!string.IsNullOrEmpty(Codigo))
                    res.Error += $"<br>Falta el Codigo de la Categoria";
                if (!string.IsNullOrEmpty(Descripcion))
                    res.Error += $"<br>Falta la Descripcion de la Categoria";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Categoria NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Categoria WHERE Id = @id", Conexion);
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
                Jerarquia = Registro.Jerarquia;
                Orden = Registro.Orden;
                Activo = Registro.Activo;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            Codigo = "";
            Descripcion = "";
            Jerarquia = null;
            Orden = null;
            Activo = false;
            Valid = false;
        }
        public static List<Categoria> GetCategorias() {
            List<Categoria> categorias = new List<Categoria>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Categoria", Conexion));
            foreach (var reg in res.Rows) {
                Categoria categoria = JsonConvert.DeserializeObject<Categoria>(JsonConvert.SerializeObject(reg));
                categoria.Valid = true;
                categorias.Add(categoria);
            }
            return categorias;
        }
    }
}