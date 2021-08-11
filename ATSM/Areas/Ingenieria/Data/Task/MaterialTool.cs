using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using WebMatrix.WebData;

namespace ATSM.Ingenieria {
	public class MaterialTool {
		private SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
		public int Id { get; private set; }
		public int TaskId { get; set; }
		public string NP { get; set; }
		public string Descripcion { get; set; }
		public decimal Cantidad { get; set; }
		public bool MAR { get; set; }
		private int _type { get; set; }
		public int Type {
			get {
				return _type;
			}
			set {
				_type = value;
				switch(value) {
					case 1:
						TypeDesc = "Material";
						break;

					case 2:
						TypeDesc = "Tool";
						break;

					default:
						_type = 0;
						TypeDesc = "";
						break;
				}
			}
		}
		public string TypeDesc { get; set; }
		public int UserId { get; set; }
		public bool delete { get; set; }
		public bool Valid { get; set; }
		public MaterialTool(int id = 0) {
			Inicializar();
			if(id > 0) {
				string query = $"SELECT * FROM TaskMaterial WHERE Id=@id";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", id));
				SetDatos(Cmnd);
			}
		}
		public MaterialTool(int taskId, string NP) {
			Inicializar();
			if(taskId > 0 && !string.IsNullOrEmpty(NP)) {
				string query = $"SELECT * FROM TaskMaterial WHERE TaskId=@tid AND NP=@np";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@tid", taskId));
				Cmnd.Parameters.Add(new SqlParameter("@np", NP));
				SetDatos(Cmnd);
			}
		}
		[JsonConstructor]
		public MaterialTool(int id, int taskId, string np, string descripcion, decimal cantidad = 0, bool mar = false, int type = 0, bool dele = false, bool valid = false) {
			Id = id;
			TaskId = taskId;
			NP = np;
			Descripcion = descripcion;
			Cantidad = cantidad;
			MAR = mar;
			Type = type;
			Valid = valid;
			delete = dele;
		}
		public Respuesta Save() {
			Respuesta res = new Respuesta(false, "No se Guardaron los Datos. Faltan Informacion. (CS_TskMaterialTool_Err.00)");
			if(TaskId > 0 && !string.IsNullOrEmpty(NP) && Type > 0) {
				SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM TaskMaterial WHERE Id=@id OR (NP=@np AND TaskId=@tid)", Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", Id));
				Cmnd.Parameters.Add(new SqlParameter("@np", NP));
				Cmnd.Parameters.Add(new SqlParameter("@tid", TaskId));
				var existe = DataBase.Query(Cmnd);
				res.Mensaje = $"{TypeDesc} ";
				string SqlStr = "";
				bool swReg = false;
				if(existe.Valid) {
					SqlStr = @"UPDATE TaskMaterial SET NP = @np, Descripcion = @des, Cantidad = @can, MAR = @mar, Type = @typ, UserId = @usu WHERE Id = @id";
					res.Mensaje += "Actualizada Correctamente";
					Id = existe.Row.Id;
				} else {
					if(!string.IsNullOrEmpty(existe.Error)) {
						res.Error = $"Error al Consultar la existencia de la TaskMaterial. (CS_TskMaterialTool_Err.01)";
						return res;
					}
					SqlStr = @"INSERT INTO TaskMaterial(TaskId, NP, Descripcion, Cantidad, MAR, Type , UserId) VALUES(@tid, @np, @des, @can, @mar, @typ, @usu)";
					res.Mensaje += "Registrada Correctamente";
					swReg = true;
				}
				SqlCommand Command = new SqlCommand(SqlStr, Conexion);
				Command.Parameters.Add(new SqlParameter("@id", Id));
				Command.Parameters.Add(new SqlParameter("@tid", TaskId));
				Command.Parameters.Add(new SqlParameter("@np", NP));
				Command.Parameters.Add(new SqlParameter("@des", Descripcion));
				Command.Parameters.Add(new SqlParameter("@can", Cantidad));
				Command.Parameters.Add(new SqlParameter("@mar", MAR));
				Command.Parameters.Add(new SqlParameter("@typ", Type));
				Command.Parameters.Add(new SqlParameter("@usu", WebSecurity.CurrentUserId));
				var regAfe = DataBase.Execute(Command);
				if(regAfe.Afectados > 0) {
					if(swReg) {
						Command = new SqlCommand($"SELECT MAX(Id) FROM TaskMaterial", Conexion);
						var resEx = DataBase.QueryValue(Command);
						if(resEx != null) {
							Id = int.Parse(resEx.ToString());
							Valid = true;
						} else {
							res.Error = $"Error al Consultar el Id de la TaskMaterial. (CS_TskMaterialTool_Err.03)";
							return res;
						}
					}
					res.Valid = true;
					res.Elemento = this;
				} else if(!regAfe.Valid && !string.IsNullOrEmpty(regAfe.Error)) {
					res.Error = $"Error al Registrar la Instruccion: (CS_TskMaterialTool_Err.02) {Environment.NewLine + regAfe.Error}";
				}
			}
			return res;
		}
		public Respuesta Delete() {
			Respuesta res = new Respuesta("Material NO se ha Eliminado");
			SqlCommand Command = new SqlCommand("DELETE TaskMaterial WHERE Id=@id", Conexion);
			Command.Parameters.Add(new SqlParameter("@id", Id));
			var resD = DataBase.Execute(Command);
			if (resD.Valid && resD.Afectados > 0) {
				res.Valid = true;
				res.Error = "";
				res.Mensaje = "Material Elminado";
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
				NP = Registro.NP;
				Descripcion = Registro.Descripcion;
				Cantidad = Registro.Cantidad;
				MAR = Registro.MAR;
				Type = Registro.Type;
				Valid = true;
			}
		}
		private void Inicializar() {
			Id = 0;
			TaskId = 0;
			NP = "";
			Descripcion = "";
			Cantidad = 0;
			MAR = false;
			Type = 0;
			UserId = 0;
			Valid = false;
			delete = false;
		}
		public static List<MaterialTool> GetMaterialTools(int taskId) {
			List<MaterialTool> LMT = new List<MaterialTool>();
			if(taskId > 0) {
				SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
				SqlCommand comando = new SqlCommand("SELECT * FROM TaskMaterial WHERE TaskId=@tid", Conexion);
				comando.Parameters.Add(new SqlParameter("@tid", taskId));
				RespuestaQuery res = DataBase.Query(comando);
				foreach(var reg in res.Rows) {
					MaterialTool MT = JsonConvert.DeserializeObject<MaterialTool>(JsonConvert.SerializeObject(reg, Formatting.Indented));
					MT.Valid = true;
					LMT.Add(MT);
				}
			}
			return LMT;
		}
	}
}