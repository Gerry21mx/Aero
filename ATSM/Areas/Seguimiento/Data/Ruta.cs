using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Transactions;

using Newtonsoft.Json;

namespace ATSM.Seguimiento {
	public class Ruta {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int IdRuta { get; set; }
		public string Codigo { get; set; }
		public string Descripcion { get; set; }
		public int? IdTipoVuelo { get; set; }
        public TipoVuelo TipoVuelo { get; set; }
        public int? NoVuelo { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; set; }
        public List<RutaTramo> Tramos = new List<RutaTramo>();
        public Ruta(int? idruta = null) {
            Inicializar();
            if (idruta > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Ruta WHERE IdRuta = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", idruta));
                SetDatos(comando);
            }
        }
        public Ruta(string codigo) {
            Inicializar();
            if (!string.IsNullOrEmpty(codigo)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Ruta WHERE Codigo = @codigo", Conexion);
                comando.Parameters.Add(new SqlParameter("@codigo", codigo));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Ruta(int idRuta, string codigo, string descripcion = "", int? idTipoVuelo = null, int? noVuelo = null, bool activo = false, bool valid = false) {
            IdRuta = idRuta;
            Codigo = codigo;
            Descripcion = descripcion;
            IdTipoVuelo = idTipoVuelo;
            NoVuelo = noVuelo;
            Activo = activo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Codigo) && !string.IsNullOrEmpty(Descripcion)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT IdRuta FROM Ruta WHERE IdRuta = @idruta OR Codigo = @codigo", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idruta", IdRuta));
                Cmnd.Parameters.Add(new SqlParameter("@codigo", Codigo));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Ruta ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Ruta SET Codigo = @codigo, Descripcion = @descripcion, IdTipoVuelo = @idtipovuelo, NoVuelo = @novuelo, Activo = @activo WHERE IdRuta=@idruta";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Ruta(Codigo, Descripcion, IdTipoVuelo, NoVuelo, Activo) VALUES(@codigo, @descripcion, @idtipovuelo, @novuelo, @activo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idruta", IdRuta));
                Command.Parameters.Add(new SqlParameter("@codigo", Codigo));
                Command.Parameters.Add(new SqlParameter("@descripcion", Descripcion));
                Command.Parameters.Add(new SqlParameter("@idtipovuelo", IdTipoVuelo ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@novuelo", NoVuelo ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                using (TransactionScope scope = new TransactionScope()) {
                    RespuestaQuery rInUp = DataBase.Insert(Command);
                    if (rInUp.Valid) {
                        if (Insr) {
                            if (rInUp.IdRegistro == 0) {
                                res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
                                return res;
                            }
                            IdRuta = rInUp.IdRegistro;
                            Valid = true;
                        }
                        var trantes = RutaTramo.GetTramosRuta(IdRuta);
                        foreach (var trAnt in trantes) {
                            var idx = Tramos.FindIndex(i => i.IdRutaTramo == trAnt.IdRutaTramo);
                            if (idx == -1) {
                                var rsDel = trAnt.Delete();
                                if (!rsDel.Valid || !string.IsNullOrEmpty(rsDel.Error)) {
                                    res.Error = $"Error al Eliminar Tramo de Ruta: (CS.{this.GetType().Name}-Save.Err.04)<br> Error: {rsDel.Error}";
                                    return res;
                                }
                            }
                        }
                        foreach (var tramo in Tramos) {
                            tramo.IdRuta = IdRuta;
                            var rSt = tramo.Save();
							if (!rSt.Valid || !string.IsNullOrEmpty(rSt.Error)) {
                                res.Error = $"Error al Registrar Tramo de Ruta: (CS.{this.GetType().Name}-Save.Err.05)<br> Error: {res.Error}";
                                return res;
                            }
						}
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br> Error: {rInUp.Error}";
                        return res;
                    }
                    res.Elemento = this;
                    res.Valid = true;
                    scope.Complete();
                }
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Ruta NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Ruta WHERE IdRuta = @id;DELETE RutaTramo WHERE IdRuta = @id;", Conexion);
            Command.Parameters.Add(new SqlParameter("@id", IdRuta));
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
                IdRuta = Registro.IdRuta;
                Codigo = Registro.Codigo;
                Descripcion = Registro.Descripcion;
                IdTipoVuelo = Registro.IdTipoVuelo;
                NoVuelo = Registro.NoVuelo;
                Activo = Registro.Activo;
                Valid = true;
                GetTipo();
                Tramos = RutaTramo.GetTramosRuta(IdRuta);
                foreach(var t in Tramos) {
                    t.GetOrigen();
                    t.GetDestino();
				}
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdRuta = 0;
            Codigo = "";
            Descripcion = "";
            IdTipoVuelo = null;
            NoVuelo = null;
            Activo = false;
            Valid = false;
        }
        public static List<Ruta> GetRutas() {
            List<Ruta> rutas = new List<Ruta>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Ruta", Conexion));
            foreach (var reg in res.Rows) {
                Ruta ruta = JsonConvert.DeserializeObject<Ruta>(JsonConvert.SerializeObject(reg));
                ruta.Valid = true;
                rutas.Add(ruta);
            }
            return rutas;
        }
        public TipoVuelo GetTipo() {
            TipoVuelo = new TipoVuelo(IdTipoVuelo);
            return TipoVuelo;
		}
    }
}