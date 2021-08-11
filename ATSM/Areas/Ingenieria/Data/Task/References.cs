using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using WebMatrix.WebData;

namespace ATSM.Ingenieria {
	public class References {
		private SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
		public int Id { get; private set; }
		public int TaskId { get; set; }
		public string Reference { get; set; }
		public string Designation { get; set; }
		public int UserId { get; set; }
		public bool delete { get; set; }
		public bool Valid { get; set; }
		public References(int id = 0) {
			Inicializar();
			if(id > 0) {
				string query = $"SELECT * FROM TaskReference WHERE Id=@id";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", id));
				SetDatos(Cmnd);
			}
		}
		[JsonConstructor]
		public References(int id, int taskId, string referencia, string designation, int userId, bool dele = false, bool valid = false) {
			Id = id;
			TaskId = taskId;
			Reference = referencia;
			Designation = designation;
			UserId = userId;
			Valid = valid;
			delete = dele;
		}
		public Respuesta Save() {
			Respuesta res = new Respuesta(false, "No se Guardaron los Datos. Faltan Informacion. (CS_TskReference_Err.00)");
			if(TaskId > 0 && !string.IsNullOrEmpty(Reference)) {
				SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM TaskReference WHERE Id=@id OR (TaskId=@tid AND Reference=@ref)", Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", Id));
				Cmnd.Parameters.Add(new SqlParameter("@tid", TaskId));
				Cmnd.Parameters.Add(new SqlParameter("@ref", Reference));
				var existe = DataBase.Query(Cmnd);
				res.Mensaje = $"Reference ";
				string SqlStr = "";
				bool swReg = false;
				if(existe.Valid) {
					SqlStr = @"UPDATE TaskReference SET Reference = @ref, Designation = @des, UserId = @usu WHERE Id = @id";
					res.Mensaje += "Actualizada Correctamente";
					Id = existe.Row.Id;
				} else {
					if(!string.IsNullOrEmpty(existe.Error)) {
						res.Error = $"Error al Consultar la existencia de la TaskReference. (CS_TskReference_Err.01)";
						return res;
					}
					SqlStr = @"INSERT INTO TaskReference(TaskId, Reference, Designation, UserId) VALUES(@tid, @ref, @des, @usu)";
					res.Mensaje += "Registrada Correctamente";
					swReg = true;
				}
				SqlCommand Command = new SqlCommand(SqlStr, Conexion);
				Command.Parameters.Add(new SqlParameter("@id", Id));
				Command.Parameters.Add(new SqlParameter("@tid", TaskId));
				Command.Parameters.Add(new SqlParameter("@ref", Reference));
				Command.Parameters.Add(new SqlParameter("@des", Designation));
				Command.Parameters.Add(new SqlParameter("@usu", WebSecurity.CurrentUserId));
				var regAfe = DataBase.Execute(Command);
				if(regAfe.Afectados > 0) {
					if(swReg) {
						Command = new SqlCommand($"SELECT MAX(Id) FROM TaskReference", Conexion);
						var resEx = DataBase.QueryValue(Command);
						if(resEx != null) {
							Id = int.Parse(resEx.ToString());
							Valid = true;
						} else {
							res.Error = $"Error al Consultar el Id de la TaskReference. (CS_TskReference_Err.03)";
							return res;
						}
					}
					res.Valid = true;
					res.Elemento = this;
				} else if(!regAfe.Valid && !string.IsNullOrEmpty(regAfe.Error)) {
					res.Error = $"Error al Registrar la Instruccion: (CS_TskReference_Err.02) {Environment.NewLine + regAfe.Error}";
				}
			}
			return res;
		}
		public Respuesta Delete() {
			Respuesta res = new Respuesta("La Referencia NO se ha Eliminado");
			SqlCommand Command = new SqlCommand("DELETE TaskReference WHERE Id=@id", Conexion);
			Command.Parameters.Add(new SqlParameter("@id", Id));
			var resD = DataBase.Execute(Command);
			if (resD.Valid && resD.Afectados > 0) {
				res.Valid = true;
				res.Error = "";
				res.Mensaje = "Referencia Elminado";
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
				Reference = Registro.Reference;
				Designation = Registro.Designation;
				UserId = Registro.UserId;
				Valid = true;
			}
		}
		private void Inicializar() {
			Id = 0;
			TaskId = 0;
			Reference = "";
			Designation = "";
			UserId = 0;
			Valid = false;
			delete = false;
		}
		public static List<References> GetReferences(int taskId) {
			List<References> LRef = new List<References>();
			if(taskId > 0) {
				SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
				SqlCommand comando = new SqlCommand("SELECT * FROM TaskReference WHERE TaskId=@tid", Conexion);
				comando.Parameters.Add(new SqlParameter("@tid", taskId));
				RespuestaQuery res = DataBase.Query(comando);
				foreach(var reg in res.Rows) {
					References Ref = JsonConvert.DeserializeObject<References>(JsonConvert.SerializeObject(reg, Formatting.Indented));
					Ref.Valid = true;
					LRef.Add(Ref);
				}
			}
			return LRef;
		}
	}
}