using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using ATSM.Tripulaciones;

using Newtonsoft.Json;

namespace ATSM.Mantenimiento {
	public class ModeloAeronave {
		private static SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
		public int IdModelo { get; set; }
		public string Modelo { get; set; }
		public string Planeador { get; set; }
		public int? PesoMaximo { get; set; }
		public string Tipo { get; set; }
		public int IdCapacidad { get; set; }
		public string capacidad { get; set; }
		public bool Valid { get; set; }
		public Capacidad Capacidad { get; set; }
		public ModeloAeronave(int? idModelo = null) {
			Inicializar();
			if (idModelo > 0) {
				SqlCommand Comando = new SqlCommand("SELECT * FROM ModeloAeronave WHERE IdModelo = @idm", Conexion);
				Comando.Parameters.Add(new SqlParameter("@idm", idModelo));
				SetDatos(Comando);
			}
		}
		public ModeloAeronave(string modelo) {
			Inicializar();
			if (!string.IsNullOrEmpty(modelo)) {
				SqlCommand Comando = new SqlCommand("SELECT * FROM ModeloAeronave WHERE modelo=@mod", Conexion);
				Comando.Parameters.Add(new SqlParameter("@mod", modelo));
				SetDatos(Comando);
			}
		}
		[JsonConstructor]
		public ModeloAeronave(int idmodelo, string modelo, string planeador = null, int? pesomaximo = null, string tipo = null, int idcapacidad = 0, string capacidad = "", bool valid = false) {
			IdModelo = idmodelo;
			Modelo = modelo;
			Planeador = planeador;
			PesoMaximo = pesomaximo;
			Tipo = tipo;
			IdCapacidad = idcapacidad;
			this.capacidad = capacidad;
			Valid = valid;
		}
		public Respuesta Save() {
			Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
			if (!string.IsNullOrEmpty(Modelo)) {
				res.Error = "";
				SqlCommand Cmnd = new SqlCommand($"SELECT IdModelo FROM ModeloAeronave WHERE IdModelo = @id OR Modelo = @mod", Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", IdModelo));
				Cmnd.Parameters.Add(new SqlParameter("@mod", Modelo));
				var existe = DataBase.Query(Cmnd);
				res.Mensaje = "Modelo Aeronave ";
				string SqlStr = "";
				bool Insr = false;
				if (existe.Valid) {
					SqlStr = @"UPDATE ModeloAeronave SET Modelo = @modelo, Planeador = @planeador, PesoMaximo = @pesomaximo, Tipo = @tipo, IdCapacidad = @idcapacidad, Capacidad = @capacidad WHERE IdModelo=@id";
					res.Mensaje += "Actualizada Correctamente";
				}
				else {
					if (!string.IsNullOrEmpty(existe.Error)) {
						res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
						return res;
					}
					SqlStr = @"INSERT INTO ModeloAeronave(modelo, Planeador, PesoMaximo, Tipo, IdCapacidad, Capacidad) VALUES(@modelo, @planeador, @pesomaximo, @tipo, @idcapacidad, @capacidad)";
					res.Mensaje += "Registrada Correctamente";
					Insr = true;
				}
				SqlCommand Command = new SqlCommand(SqlStr, Conexion);
				Command.Parameters.Add(new SqlParameter("@id", IdModelo));
				Command.Parameters.Add(new SqlParameter("@modelo", Modelo));
				Command.Parameters.Add(new SqlParameter("@planeador", string.IsNullOrEmpty(Planeador) ? SqlString.Null : Planeador));
				Command.Parameters.Add(new SqlParameter("@pesomaximo", PesoMaximo ?? SqlInt32.Null));
				Command.Parameters.Add(new SqlParameter("@tipo", string.IsNullOrEmpty(Tipo) ? SqlString.Null : Tipo));
				Command.Parameters.Add(new SqlParameter("@idcapacidad", IdCapacidad));
				Command.Parameters.Add(new SqlParameter("@capacidad", capacidad));
				RespuestaQuery rInUp = DataBase.Insert(Command);
				if (rInUp.Valid) {
					if (Insr) {
						if (rInUp.IdRegistro == 0) {
							res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
							return res;
						}
						IdModelo = rInUp.IdRegistro;
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
			Respuesta res = new Respuesta("Modelo Aeronave NO se Elimino");
			SqlCommand Command = new SqlCommand("DELETE ModeloAeronave WHERE IdModelo = @id", Conexion);
			Command.Parameters.Add(new SqlParameter("@id", IdModelo));
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
				IdModelo = Registro.IdModelo;
				Modelo = Registro.Modelo;
				Planeador = Registro.Planeador;
				PesoMaximo = Registro.PesoMaximo;
				Tipo = Registro.Tipo;
				IdCapacidad = Registro.IdCapacidad;
				capacidad = Registro.Capacidad;
				Valid = true;
				GetCapacidad();
			}
		}
		public string GetJSON() {
			return JsonConvert.SerializeObject(this);
		}
		private void Inicializar() {
			IdModelo = 0;
			Modelo = "";
			Planeador = null;
			PesoMaximo = null;
			Tipo = null;
			capacidad = "";
			Valid = false;
		}
		public static List<ModeloAeronave> GetModeloAeronaves() {
			List<ModeloAeronave> modeloaeronaves = new List<ModeloAeronave>();
			RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT IdModelo, Modelo, Planeador, PesoMaximo, Tipo, IdCapacidad, Capacidad, 1 AS valid FROM ModeloAeronave", Conexion));
			foreach (var reg in res.Rows) {
				ModeloAeronave modeloaeronave = JsonConvert.DeserializeObject<ModeloAeronave>(JsonConvert.SerializeObject(reg));
				//modeloaeronave.Valid = true;
				modeloaeronaves.Add(modeloaeronave);
			}
			return modeloaeronaves;
		}
		public void GetCapacidad() {
			Capacidad = new Capacidad(IdCapacidad);
		}
	}
}