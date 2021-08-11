using ATSM.Ingenieria;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using WebMatrix.WebData;

namespace ATSM.Almacen {
	public class Articulo {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; private set; }
		public string Part { get; set; }
		public string Description { get; set; }
		public int? IdCategoria { get; set; }
		public int? IdUnidadMedida { get; set; }
		public bool Seriado { get; set; }
		public bool Caducidad { get; set; }
		public bool Calibracion { get; set; }
		public bool StandBy { get; set; }
		public decimal? Minimo { get; set; }
		public decimal? Maximo { get; set; }
		public decimal? Reorden { get; set; }
		public decimal? Costo { get; set; }
		public decimal? Equivalencia_Unitaria { get; set; }
		public string? FileName { get; set; }
		public int Usuario { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; private set; }
		public Categoria Categoria { get; set; }
		public UnidadMedida UnidadMedida { get; set; }
		public Articulo(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Articulos WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Articulo(string parte) {
            Inicializar();
            if (!string.IsNullOrEmpty(parte)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Articulos WHERE Part = @parte", Conexion);
                comando.Parameters.Add(new SqlParameter("@parte", parte));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Articulo(int id, string part, string description, int? idCategoria = null, int? idUnidadMedida = null, bool seriado = false, bool caducidad = false, bool calibracion = false, bool standBy = false, decimal? minimo = null, decimal? maximo = null, decimal? reorden = null, decimal? costo = null, decimal? equivalencia_Unitaria = null, string fileName = "", int usuario = 0, bool activo = false, bool valid = false) {
            Id = id;
            Part = part;
            Description = description;
            IdCategoria = idCategoria;
            IdUnidadMedida = idUnidadMedida;
            Seriado = seriado;
            Caducidad = caducidad;
            Calibracion = calibracion;
            StandBy = standBy;
            Minimo = minimo;
            Maximo = maximo;
            Reorden = reorden;
            Costo = costo;
            Equivalencia_Unitaria = equivalencia_Unitaria;
            FileName = fileName;
            Usuario = usuario;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Part) && !string.IsNullOrEmpty(Description)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Articulos WHERE Id = @id OR Part = @parte", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@parte", Part));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Articulo ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Articulos SET Part = @part, Description = @description, IdCategoria = @idcategoria, IdUnidadMedida = @idunidadmedida, Seriado = @seriado, Caducidad = @caducidad, Calibracion = @calibracion, StandBy = @standby, Minimo = @minimo, Maximo = @maximo, Reorden = @reorden, Costo = @costo, Equivalencia_Unitaria = @equivalencia_unitaria, FileName = @filename, Usuario = @usuario, Activo = @activo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Articulos(Part, Description, IdCategoria, IdUnidadMedida, Seriado, Caducidad, Calibracion, StandBy, Minimo, Maximo, Reorden, Costo, Equivalencia_Unitaria, FileName, Usuario, Activo) VALUES(@part, @description, @idcategoria, @idunidadmedida, @seriado, @caducidad, @calibracion, @standby, @minimo, @maximo, @reorden, @costo, @equivalencia_unitaria, @filename, @usuario, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@part", string.IsNullOrEmpty(Part) ? SqlString.Null : Part));
                Command.Parameters.Add(new SqlParameter("@description", string.IsNullOrEmpty(Description) ? SqlString.Null : Description));
                Command.Parameters.Add(new SqlParameter("@idcategoria", IdCategoria ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idunidadmedida", IdUnidadMedida ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@seriado", Seriado));
                Command.Parameters.Add(new SqlParameter("@caducidad", Caducidad));
                Command.Parameters.Add(new SqlParameter("@calibracion", Calibracion));
                Command.Parameters.Add(new SqlParameter("@standby", StandBy));
                Command.Parameters.Add(new SqlParameter("@minimo", Minimo ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@maximo", Maximo ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@reorden", Reorden ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@costo", Costo ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@equivalencia_unitaria", Equivalencia_Unitaria ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@filename", string.IsNullOrEmpty(FileName) ? SqlString.Null : FileName));
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
                SetCategoria();
                SetUnidadMedida();
                res.Elemento = this;
                res.Valid = true;
            }
            else {
                if (!string.IsNullOrEmpty(Part))
                    res.Error += $"<br>Falta el Numero de Parte en el Articulo";
                if (!string.IsNullOrEmpty(Description))
                    res.Error += $"<br>Falta la Descripcion del Articulo";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Articulo NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Articulos WHERE Id = @id", Conexion);
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
                Part = Registro.Part;
                Description = Registro.Description;
                IdCategoria = Registro.IdCategoria;
                IdUnidadMedida = Registro.IdUnidadMedida;
                Seriado = Registro.Seriado;
                Caducidad = Registro.Caducidad;
                Calibracion = Registro.Calibracion;
                StandBy = Registro.StandBy;
                Minimo = Registro.Minimo;
                Maximo = Registro.Maximo;
                Reorden = Registro.Reorden;
                Costo = Registro.Costo;
                Equivalencia_Unitaria = Registro.Equivalencia_Unitaria;
                FileName = Registro.FileName;
                Usuario = Registro.Usuario;
                Activo = Registro.Activo;
                Valid = true;
                SetCategoria();
                SetUnidadMedida();
            }
        }
        private void Inicializar() {
            Id = 0;
            Part = "";
            Description = "";
            IdCategoria = null;
            IdUnidadMedida = null;
            Seriado = false;
            Caducidad = false;
            Calibracion = false;
            StandBy = false;
            Minimo = null;
            Maximo = null;
            Reorden = null;
            Costo = null;
            Equivalencia_Unitaria = null;
            FileName = "";
            Usuario = 0;
            Activo = false;
            Valid = false;
        }
        public static List<Articulo> GetArticulos() {
            List<Articulo> articulos = new List<Articulo>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Articulos", Conexion));
            foreach (var reg in res.Rows) {
                Articulo articulo = JsonConvert.DeserializeObject<Articulo>(JsonConvert.SerializeObject(reg));
                articulo.Valid = true;
                articulos.Add(articulo);
            }
            return articulos;
        }
        public static dynamic GetVArticulos() {
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM V_Articulos", Conexion));
            return res.Rows;
        }
        public void SetCategoria() {
            Categoria = new Categoria(IdCategoria);
		}
        public void SetUnidadMedida() {
            UnidadMedida = new UnidadMedida(IdUnidadMedida);
        }
    }
}