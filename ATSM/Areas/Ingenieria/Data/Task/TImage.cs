using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using WebMatrix.WebData;

namespace ATSM.Ingenieria {
	public class TImage {
		private SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
		public int Id { get; private set; }
		public int TaskId { get; set; }
		public int No { get; set; }
		public string Titulo { get; set; }
		public string FileName { get; set; }
		public int UserId { get; set; }
		public bool delete { get; set; }
		public bool Valid { get; set; }
		public TImage(int id = 0) {
			Inicializar();
			if(id > 0) {
				string query = $"SELECT * FROM TaskImages WHERE Id=@id";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", id));
				SetDatos(Cmnd);
			}
		}
		public TImage(int taskId, int no) {
			Inicializar();
			if(taskId > 0) {
				string query = $"SELECT * FROM TaskImages WHERE TaskId=@tid AND No=@no";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@tid", taskId));
				Cmnd.Parameters.Add(new SqlParameter("@no", no));
				SetDatos(Cmnd);
			}
		}
		[JsonConstructor]
		public TImage(int id, int taskId, int no, string titulo, string fileName, int userId, bool dele = false, bool valid = false) {
			Id = id;
			TaskId = taskId;
			No = no;
			Titulo = titulo;
			FileName = fileName;
			UserId = userId;
			Valid = valid;
			delete = dele;
		}
		public Respuesta Save() {
			Respuesta res = new Respuesta(false, "No se Guardaron los Datos. Faltan Informacion. (CS_TImage_Err.00)");
			if(TaskId > 0 && !string.IsNullOrEmpty(Titulo)) {
				SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM TaskImages WHERE Id=@id OR (TaskId=@tid AND No=@no)", Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", Id));
				Cmnd.Parameters.Add(new SqlParameter("@tid", TaskId));
				Cmnd.Parameters.Add(new SqlParameter("@no", No));
				var existe = DataBase.Query(Cmnd);
				res.Mensaje = $"Imagen ";
				string SqlStr = "";
				bool swReg = false;
				if(existe.Valid) {
					SqlStr = @"UPDATE TaskImages SET Titulo = @tit, FileName = @fil, UserId = @usu WHERE Id = @id";
					res.Mensaje += "Actualizada Correctamente";
					Id = existe.Row.Id;
				} else {
					if(!string.IsNullOrEmpty(existe.Error)) {
						res.Error = $"Error al Consultar la existencia de la TaskImages. (CS_TImage_Err.01)";
						return res;
					}
					SqlStr = @"INSERT INTO TaskImages(TaskId, No, Titulo, FileName, UserId) VALUES(@tid, @no, @tit, @fil, @usu)";
					res.Mensaje += "Registrada Correctamente";
					swReg = true;
				}
				SqlCommand Command = new SqlCommand(SqlStr, Conexion);
				Command.Parameters.Add(new SqlParameter("@id", Id));
				Command.Parameters.Add(new SqlParameter("@tid", TaskId));
				Command.Parameters.Add(new SqlParameter("@no", No));
				Command.Parameters.Add(new SqlParameter("@tit", Titulo));
				Command.Parameters.Add(new SqlParameter("@fil", FileName));
				Command.Parameters.Add(new SqlParameter("@usu", WebSecurity.CurrentUserId));
				var regAfe = DataBase.Execute(Command);
				if(regAfe.Afectados > 0) {
					if(swReg) {
						Command = new SqlCommand($"SELECT MAX(Id) FROM TaskImages", Conexion);
						var resEx = DataBase.QueryValue(Command);
						if(resEx != null) {
							Id = int.Parse(resEx.ToString());
							Valid = true;
						} else {
							res.Error = $"Error al Consultar el Id de la TaskImages. (CS_TImage_Err.03)";
							return res;
						}
					}
					res.Valid = true;
					res.Elemento = this;
				} else if(!regAfe.Valid && !string.IsNullOrEmpty(regAfe.Error)) {
					res.Error = $"Error al Registrar la Instruccion: (CS_TImage_Err.02) {Environment.NewLine + regAfe.Error}";
				}
			}
			return res;
		}
		public Respuesta Delete() {
			Respuesta res = new Respuesta("La Imagen NO se ha Eliminado");
			SqlCommand Command = new SqlCommand("DELETE TaskImages WHERE Id=@id", Conexion);
			Command.Parameters.Add(new SqlParameter("@id", Id));
			var resD = DataBase.Execute(Command);
			if (resD.Valid && resD.Afectados > 0) {
				res.Valid = true;
				res.Error = "";
				res.Mensaje = "Imagen Elminado";
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
				Titulo = Registro.Titulo;
				FileName = Registro.FileName;
				UserId = Registro.UserId;
				Valid = true;
			}
		}
		private void Inicializar() {
			Id = 0;
			TaskId = 0;
			No = 0;
			Titulo = "";
			FileName = "";
			UserId = 0;
			Valid = false;
			delete = false;
		}
		public static List<TImage> GetImages(int taskId) {
			List<TImage> LIma = new List<TImage>();
			if(taskId > 0) {
				SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
				SqlCommand comando = new SqlCommand("SELECT * FROM TaskImages WHERE TaskId=@tid", Conexion);
				comando.Parameters.Add(new SqlParameter("@tid", taskId));
				RespuestaQuery res = DataBase.Query(comando);
				foreach(var reg in res.Rows) {
					TImage Ima = JsonConvert.DeserializeObject<TImage>(JsonConvert.SerializeObject(reg));
					Ima.Valid = true;
					LIma.Add(Ima);
				}
			}
			return LIma;
		}
	}
}