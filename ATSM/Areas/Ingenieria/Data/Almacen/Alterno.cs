using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ATSM.Almacen {
	public class Alterno {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdArticulo1 { get; set; }
		public int IdArticulo2 { get; set; }
		public int Usuario { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; set; }
        public Alterno(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Alternos WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Alterno(int id = 0, int idArticulo1 = 0, int idArticulo2 = 0, int usuario = 0, bool activo = false, bool valid = false) {
            Id = id;
            IdArticulo1 = idArticulo1;
            IdArticulo2 = idArticulo2;
            Usuario = usuario;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdArticulo1 > 0 && IdArticulo2 > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Alternos WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Alterno ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Alternos SET IdArticulo1 = @idarticulo1, IdArticulo2 = @idarticulo2, Usuario = @usuario, Activo = @activo WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Alternos(IdArticulo1, IdArticulo2, Usuario, Activo) VALUES(@idarticulo1, @idarticulo2, @usuario, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idarticulo1", IdArticulo1));
                Command.Parameters.Add(new SqlParameter("@idarticulo2", IdArticulo2));
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
                res.Elemento = this;
                res.Valid = true;
            }
            else {
                if (IdArticulo1 > 0)
                    res.Error += $"<br>Falta el articulo Principal";
                if (IdArticulo2 > 0)
                    res.Error += $"<br>Falta el articulo Alterno";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Alterno NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Alternos WHERE Id = @id", Conexion);
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
                IdArticulo1 = Registro.IdArticulo1;
                IdArticulo2 = Registro.IdArticulo2;
                Usuario = Registro.Usuario;
                Activo = Registro.Activo;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            IdArticulo1 = 0;
            IdArticulo2 = 0;
            Usuario = 0;
            Activo = false;
            Valid = false;
        }
        public static List<Alterno> GetAlternos() {
            List<Alterno> alternos = new List<Alterno>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Alternos", Conexion));
            foreach (var reg in res.Rows) {
                Alterno alterno = JsonConvert.DeserializeObject<Alterno>(JsonConvert.SerializeObject(reg));
                alterno.Valid = true;
                alternos.Add(alterno);
            }
            return alternos;
        }
    }
}