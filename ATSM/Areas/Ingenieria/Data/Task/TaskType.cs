using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace ATSM.Ingenieria {
	public class TaskType {
		private SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
		public int Id { get; private set; }
		public string Descripcion { get; set; }
		public bool Activo { get; set; }
		public bool Valid { get; set; }
		public TaskType(int idTask = 0) {
			Inicializar();
			if(idTask > 0) {
				string query = $"SELECT * FROM TaskType WHERE Id=@id";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@id", idTask));
				SetDatos(Cmnd);
			}
		}
		public TaskType(string descripcion) {
			Inicializar();
			if(!string.IsNullOrEmpty(descripcion)) {
				string query = $"SELECT * FROM TaskType WHERE Descripcion=@des";
				SqlCommand Cmnd = new SqlCommand(query, Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@des", descripcion));
				SetDatos(Cmnd);
			}
		}
		[JsonConstructor]
		public TaskType(int id, string descripcion, bool activo = false, bool valid = false) {
			Id = id;
			Descripcion = descripcion;
			Activo = activo;
			Valid = valid;
		}
		public string GetJSON() {
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
		private void SetDatos(SqlCommand Command) {
			var Resultado = DataBase.Query(Command);
			if(Resultado.Valid) {
				var Registro = Resultado.Row;
				Id = Registro.Id;
				Descripcion = Registro.Descripcion;
				Activo = Registro.Activo;
				Valid = true;
			}
		}
		private void Inicializar() {
			Id = 0;
			Descripcion = "";
			Activo = false;
			Valid = false;
		}
		public static List<TaskType> GetTaskType() {
			SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
			List<TaskType> TTS = new List<TaskType>();
			SqlCommand comando = new SqlCommand("SELECT * FROM TaskType", Conexion);
			RespuestaQuery res = DataBase.Query(comando);
			foreach(var reg in res.Rows) {
				TaskType TT = JsonConvert.DeserializeObject<TaskType>(JsonConvert.SerializeObject(reg, Formatting.Indented));
				TT.Valid = true;
				TTS.Add(TT);
			}
			return TTS;
		}
	}
}