using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using WebMatrix.WebData;

namespace ATSM.Ingenieria {
	public class Instruction {
		private SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
		public int Id { get; private set; }
		public int TaskId { get; set; }
		public int No { get; set; }
		public string? Title { get; set; }
		public string Contenido { get; set; }
		public bool Tecnico { get; set; }
		public bool Inspector { get; set; }
		public int UserId { get; set; }
		public bool delete { get; set; }
		public bool Valid { get; set; }
		public Instruction(int id = 0) {
			Inicializar();
			if(id > 0) {
				string query = $"SELECT * FROM TaskInstructions WHERE Id=@id";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", id));
				SetDatos(Cmnd);
			}
		}
		public Instruction(int taskId, int no) {
			Inicializar();
			if(taskId > 0 && No > 0) {
				string query = $"SELECT * FROM TaskInstructions WHERE TaskId=@tid AND No=@no";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@tid", taskId));
				Cmnd.Parameters.Add(new SqlParameter("@no", no));
				SetDatos(Cmnd);
			}
		}
		[JsonConstructor]
		public Instruction(int id, int taskId, int no, string? title = null, string contenido = "", bool tecnico = false, bool inspector = false, int userId = 0, bool dele = false, bool valid = false) {
			Id = id;
			TaskId = taskId;
			No = no;
			Title = title;
			Contenido = contenido;
			Tecnico = tecnico;
			Inspector = inspector;
			UserId = userId;
			Valid = valid;
			delete = dele;
		}
		public Respuesta Save() {
			Respuesta res = new Respuesta(false, "No se Guardaron los Datos. Faltan Informacion. (CS_TskInstruccion_Err.00)");
			if(TaskId > 0 && !string.IsNullOrEmpty(Contenido) && No >= 0) {
				SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM TaskInstructions WHERE Id=@id OR (TaskId=@tid AND No=@no)", Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", Id));
				Cmnd.Parameters.Add(new SqlParameter("@tid", TaskId));
				Cmnd.Parameters.Add(new SqlParameter("@no", No));
				var existe = DataBase.Query(Cmnd);
				res.Mensaje = "Instruccion ";
				string SqlStr = "";
				bool swReg = false;
				if(existe.Valid) {
					SqlStr = @"UPDATE TaskInstructions SET Title = @tit, Contenido = @con, Tecnico = @tec, Inspector = @ins, UserId = @usu WHERE Id = @id";
					res.Mensaje += "Actualizada Correctamente";
					Id = existe.Row.Id;
				} else {
					if(!string.IsNullOrEmpty(existe.Error)) {
						res.Error = $"Error al Consultar la existencia de la TaskInstructions. (CS_TskInstruccion_Err.01)";
						return res;
					}
					SqlStr = @"INSERT INTO TaskInstructions(TaskId, No, Title, Contenido, Tecnico, Inspector, UserId) VALUES(@tid, @no, @tit, @con, @tec, @ins, @usu)";
					res.Mensaje += "Registrada Correctamente";
					swReg = true;
				}
				SqlCommand Command = new SqlCommand(SqlStr, Conexion);
				Command.Parameters.Add(new SqlParameter("@id", Id));
				Command.Parameters.Add(new SqlParameter("@tid", TaskId));
				Command.Parameters.Add(new SqlParameter("@no", No));
				Command.Parameters.Add(new SqlParameter("@tit", Title));
				Command.Parameters.Add(new SqlParameter("@con", Contenido));
				Command.Parameters.Add(new SqlParameter("@tec", Tecnico));
				Command.Parameters.Add(new SqlParameter("@ins", Inspector));
				Command.Parameters.Add(new SqlParameter("@usu", WebSecurity.CurrentUserId));
				var regAfe = DataBase.Execute(Command);
				if(regAfe.Afectados > 0) {
					if(swReg) {
						Command = new SqlCommand($"SELECT MAX(Id) FROM TaskInstructions", Conexion);
						var resEx = DataBase.QueryValue(Command);
						if(resEx != null) {
							Id = int.Parse(resEx.ToString());
							Valid = true;
						} else {
							res.Error = $"Error al Consultar el Id de la TaskInstructions. (CS_TskInstruccion_Err.03)";
							return res;
						}
					}
					res.Valid = true;
					res.Elemento = this;
				} else if(!regAfe.Valid && !string.IsNullOrEmpty(regAfe.Error)) {
					res.Error = $"Error al Registrar la Instruccion: (CS_TskInstruccion_Err.02) {Environment.NewLine + regAfe.Error}";
				}
			}
			return res;
		}
		public Respuesta Delete() {
			Respuesta res = new Respuesta("La Instruccion NO se ha Eliminado");
			SqlCommand Command = new SqlCommand("DELETE TaskInstructions WHERE Id=@id", Conexion);
			Command.Parameters.Add(new SqlParameter("@id", Id));
			var resD = DataBase.Execute(Command);
			if (resD.Valid && resD.Afectados > 0) {
				res.Valid = true;
				res.Error = "";
				res.Mensaje = "Instruccion Elminada";
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
		public string GetJSON() {
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
		private void SetDatos(SqlCommand Command) {
			var Resultado = DataBase.Query(Command);
			if(Resultado.Valid) {
				var Registro = Resultado.Row;
				Id = Registro.Id;
				TaskId = Registro.TaskId;
				No = Registro.No;
				Title = Registro.Title;
				Contenido = Registro.Contenido;
				Tecnico = Registro.Tecnico;
				Inspector = Registro.Inspector;
				UserId = Registro.UserId;
				Valid = true;
			}
		}
		private void Inicializar() {
			Id = 0;
			TaskId = 0;
			No = 0;
			Title = null;
			Contenido = "";
			Tecnico = false;
			Inspector = false;
			UserId = 0;
			Valid = false;
			delete = false;
		}
		public static List<Instruction> GetInstructions(int taskId) {
			List<Instruction> LINS = new List<Instruction>();
			if(taskId > 0) {
				SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
				SqlCommand comando = new SqlCommand("SELECT * FROM TaskInstructions WHERE TaskId=@tid", Conexion);
				comando.Parameters.Add(new SqlParameter("@tid", taskId));
				RespuestaQuery res = DataBase.Query(comando);
				foreach(var reg in res.Rows) {
					Instruction Ins = JsonConvert.DeserializeObject<Instruction>(JsonConvert.SerializeObject(reg, Formatting.Indented));
					Ins.Valid = true;
					LINS.Add(Ins);
				}
			}
			return LINS;
		}
	}
}