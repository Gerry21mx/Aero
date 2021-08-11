using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using WebMatrix.WebData;

namespace ATSM.Ingenieria {
	public class Tarea {
		private SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
		public int TaskId { get; private set; }
		public int IdComponenteMenor { get; set; }
		public string IdModelo { get; set; }
		public string Repeat { get; set; }
		public string Zone { get; set; }
		public string Rev { get; set; }
		private int _Type { get; set; }
		public int TypeId {
			get {
				return _Type;
			}
			set {
				Type = new TaskType(value);
				_Type = Type.Valid ? value : 0;
			}
		}
		public TaskType Type { get; set; }
		public ComponenteMenor ComponenteMenor { get; set; }
		public List<MaterialTool> MaterialTools = new List<MaterialTool>();
		public List<Instruction> Instructions = new List<Instruction>();
		public List<References> ReferenceS = new List<References>();
		public List<TImage> Images = new List<TImage>();

		public int UserId { get; set; }
		public bool Valid { get; set; }
		public Tarea(int idTask = 0) {
			Inicializar();
			if(idTask > 0) {
				string query = $"SELECT * FROM Tasks WHERE TaskId=@id";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", idTask));
				SetDatos(Cmnd);
			}
		}
		public Tarea(int idC, string idM) {
			Inicializar();
			if(idC > 0 && !string.IsNullOrEmpty(idM)) {
				string query = $"SELECT * FROM Tasks WHERE IdComponenteMenor=@idc AND IdModelo=@idm";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@idc", idC));
				Cmnd.Parameters.Add(new SqlParameter("@idm", idM));
				SetDatos(Cmnd);
			}
		}
		[JsonConstructor]
		public Tarea(int taskId, int idc, string idm, string repeat = "", string zone = "", string rev = "", int typeId = 0, bool valid = false, dynamic[] materialTools = null, dynamic[] instructions = null, dynamic[] referencias = null, dynamic[] images = null) {
			TaskId = taskId;
			IdComponenteMenor = idc;
			IdModelo = idm;
			Repeat = repeat;
			Zone = zone;
			Rev = rev;
			TypeId = typeId;
			Valid = valid;
			if(materialTools != null) {
				foreach(var MTL in materialTools) {
					MaterialTool MT = MTL.ToObject<MaterialTool>();
					MaterialTools.Add(MT);
				}
			}
			if(instructions != null) {
				foreach(var inst in instructions) {
					Instruction Ins = inst.ToObject<Instruction>();
					Instructions.Add(Ins);
				}
			}
			if(referencias != null) {
				foreach(var refe in referencias) {
					References Refe = refe.ToObject<References>();
					ReferenceS.Add(Refe);
				}
			}
			if(images != null) {
				foreach(var img in images) {
					TImage Imag = img.ToObject<TImage>();
					Images.Add(Imag);
				}
			}
		}
		public Respuesta Save() {
			Respuesta res = new Respuesta(false, "No se Guardaron los Datos. Faltan Informacion. (CS_TASK_Err.00)");
			if(IdComponenteMenor > 0 && !string.IsNullOrEmpty(IdModelo) && Instructions.Count > 0) {
				SqlCommand Cmnd = new SqlCommand($"SELECT TaskId FROM Tasks WHERE TaskId=@tid OR (IdComponenteMenor=@idc AND IdModelo=@idm)", Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@tid", TaskId));
				Cmnd.Parameters.Add(new SqlParameter("@idc", IdComponenteMenor));
				Cmnd.Parameters.Add(new SqlParameter("@idm", IdModelo));
				var existe = DataBase.Query(Cmnd);
				res.Mensaje = "Tarea ";
				string SqlStr = "";
				bool swReg = false;
				if(existe.Valid) {
					SqlStr = @"UPDATE Tasks SET Repeat = @rep, Zone = @zon, TypeId = @typ, Rev = @rev, UserId = @usu WHERE TaskId=@tid";
					res.Mensaje += "Actualizada Correctamente";
					TaskId = existe.Row.TaskId;
				} else {
					if(!string.IsNullOrEmpty(existe.Error)) {
						res.Error = $"Error al Consultar la existencia de la Tarea. (CS_TASK_Err.01)";
						return res;
					}
					SqlStr = @"INSERT INTO Tasks(IdComponenteMenor, IdModelo, Repeat, Zone, TypeId, Rev, UserId) VALUES(@idc, @idm, @rep, @zon, @typ, @rev, @usu)";
					res.Mensaje += "Registrada Correctamente";
					swReg = true;
				}
				SqlCommand Command = new SqlCommand(SqlStr, Conexion);
				Command.Parameters.Add(new SqlParameter("@tid", TaskId));
				Command.Parameters.Add(new SqlParameter("@idc", IdComponenteMenor));
				Command.Parameters.Add(new SqlParameter("@idm", IdModelo));
				Command.Parameters.Add(new SqlParameter("@rep", Repeat));
				Command.Parameters.Add(new SqlParameter("@zon", Zone));
				Command.Parameters.Add(new SqlParameter("@typ", TypeId));
				Command.Parameters.Add(new SqlParameter("@rev", Rev));
				Command.Parameters.Add(new SqlParameter("@usu", WebSecurity.CurrentUserId));
				var regAfe = DataBase.Execute(Command);
				if(regAfe.Afectados > 0) {
					if(swReg) {
						Command = new SqlCommand($"SELECT MAX(TaskId) FROM Tasks", Conexion);
						var resEx = DataBase.QueryValue(Command);
						if(resEx != null) {
							TaskId = int.Parse(resEx.ToString());
							Valid = true;
						} else {
							res.Error = $"Error al Consultar el ID de la Tarea. (CS_TASK_Err.03)";
							return res;
						}
					}
					foreach(var MaTo in MaterialTools) {
						MaTo.TaskId = TaskId;
						Respuesta reSD = new Respuesta();
						if(MaTo.delete) {
							reSD = MaTo.Delete();
						} else {
							reSD = MaTo.Save();
						}
						if(!reSD.Valid || !string.IsNullOrEmpty(reSD.Error)) {
							res.Error += $"{Environment.NewLine}Error al Registrar Material o Tools: (CS_TASK_Err.04). {Environment.NewLine + MaTo.NP} - {MaTo.Descripcion}. -E: {Environment.NewLine} {reSD.Error}.-M: {Environment.NewLine} {reSD.Mensaje}.";
							return res;
						}
					}
					foreach(var Instr in Instructions) {
						Instr.TaskId = TaskId;
						Respuesta reSD = new Respuesta();
						if(Instr.delete) {
							reSD = Instr.Delete();
						} else {
							reSD = Instr.Save();
						}
						if(!string.IsNullOrEmpty(reSD.Error)) {
							res.Error += $"{Environment.NewLine}Error al Registrar Instruccion: (CS_TASK_Err.05). {Environment.NewLine + Instr.Title}. -E: {Environment.NewLine} {reSD.Error}. -M: {Environment.NewLine} {reSD.Mensaje}.";
							return res;
						}
					}
					foreach(var Refes in ReferenceS) {
						Refes.TaskId = TaskId;
						Respuesta reSD = new Respuesta();
						if(Refes.delete) {
							reSD = Refes.Delete();
						} else {
							reSD = Refes.Save();
						}
						if(!string.IsNullOrEmpty(reSD.Error)) {
							res.Error += $"{Environment.NewLine}Error al Registrar la Referencia: (CS_TASK_Err.06). {Environment.NewLine + Refes.Reference}. -E: {Environment.NewLine} {reSD.Error}.-M: {Environment.NewLine} {reSD.Mensaje}.";
							return res;
						}
					}
					foreach(var Imagen in Images) {
						Imagen.TaskId = TaskId;
						Respuesta reSD = new Respuesta();
						if(Imagen.delete) {
							reSD = Imagen.Delete();
						} else {
							reSD = Imagen.Save();
						}
						if(!string.IsNullOrEmpty(reSD.Error)) {
							res.Error += $"{Environment.NewLine}Error al Registrar la Imagen: (CS_TASK_Err.07). {Environment.NewLine + Imagen.Titulo}. -E: {Environment.NewLine} {reSD.Error}.-M: {Environment.NewLine} {reSD.Mensaje}.";
							return res;
						}
					}
					res.Valid = true;
					res.Elemento = this;
				} else if(!regAfe.Valid && !string.IsNullOrEmpty(regAfe.Error)) {
					res.Error = $"Error al Registrar la Tarea: (CS_TASK_Err.02) {Environment.NewLine + regAfe.Error}";
				}
			}
			return res;
		}
		public Respuesta Delete() {
			Respuesta res = new Respuesta("Tarea NO se Elimino");
			SqlCommand Command = new SqlCommand("DELETE Tasks WHERE TaskId=@id;DELETE TaskMaterial WHERE TaskId=@id;DELETE TaskInstructions WHERE TaskId=@id;", Conexion);
			Command.Parameters.Add(new SqlParameter("@id", TaskId));
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
		public string GetJSON() {
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
		private void SetDatos(SqlCommand Command) {
			var Resultado = DataBase.Query(Command);
			if(Resultado.Valid) {
				var Registro = Resultado.Row;
				TaskId = Registro.TaskId;
				IdComponenteMenor = Registro.IdComponenteMenor;
				IdModelo = Registro.IdModelo;
				Repeat = Registro.Repeat;
				Zone = Registro.Zone;
				Rev = Registro.Rev;
				TypeId = Registro.TypeId;
				UserId = Registro.UserId;
				Valid = true;
				MaterialTools = MaterialTool.GetMaterialTools(TaskId);
				Instructions = Instruction.GetInstructions(TaskId);
				ReferenceS = References.GetReferences(TaskId);
				Images = TImage.GetImages(TaskId);
				SetComponente();
			}
		}
		private void Inicializar() {
			TaskId = 0;
			IdComponenteMenor = 0;
			IdModelo = "";
			Repeat = "";
			Zone = "";
			Rev = "";
			TypeId = 0;
			UserId = 0;
			Valid = false;
		}
		public void SetComponente() {
			ComponenteMenor = new ComponenteMenor(IdComponenteMenor);
		}
	}
}