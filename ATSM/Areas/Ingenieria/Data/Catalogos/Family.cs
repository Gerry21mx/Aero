using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ATSM.Ingenieria {
	public class Family {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; private set; }
		public string Codigo { get; set; }
		public string Nombre { get; set; }
		public bool Activo { get; set; }
		public bool? TM01 { get; set; }
		public bool? TM02 { get; set; }
		public bool? TM03 { get; set; }
		public bool Valid { get; set; }
        public List<TipoMenor> Tipos { get; set; }
        public Family(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Family WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public Family(string cadena) {
            Inicializar();
            if (!string.IsNullOrEmpty(cadena)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Family WHERE Codigo = @cadena OR Nombre = @cadena", Conexion);
                comando.Parameters.Add(new SqlParameter("@cadena", cadena));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Family(int id, string codigo, string nombre, bool activo = false, bool? tm01 = null, bool? tm02 = null, bool? tm03 = null) {
            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            Activo = activo;
            TM01 = tm01;
            TM02 = tm02;
            TM03 = tm03;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Codigo) && !string.IsNullOrEmpty(Nombre)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Family WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Family ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Family SET Codigo = @codigo, Nombre = @nombre, Activo = @activo, TM01 = @tm01, TM02 = @tm02, TM03 = @tm03 WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Family(Codigo, Nombre, Activo, TM01, TM02, TM03) VALUES(@codigo, @nombre, @activo, @tm01, @tm02, @tm03)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@codigo", string.IsNullOrEmpty(Codigo) ? SqlString.Null : Codigo));
                Command.Parameters.Add(new SqlParameter("@nombre", string.IsNullOrEmpty(Nombre) ? SqlString.Null : Nombre));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                Command.Parameters.Add(new SqlParameter("@tm01", TM01));
                Command.Parameters.Add(new SqlParameter("@tm02", TM02));
                Command.Parameters.Add(new SqlParameter("@tm03", TM03));
                using(TransactionScope scope=new TransactionScope()) {
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
                    scope.Complete();
				}
            }
			else {
                if (!string.IsNullOrEmpty(Codigo))
                    res.Error += $"<br>Falta el Valor del Codigo";
			}
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Family NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Family WHERE Id = @id", Conexion);
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
                Codigo = Registro.Codigo;
                Nombre = Registro.Nombre;
                Activo = Registro.Activo;
                TM01 = Registro.TM01;
                TM02 = Registro.TM02;
                TM03 = Registro.TM03;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            Codigo = "";
            Nombre = "";
            Activo = false;
            TM01 = false;
            TM01 = false;
            TM01 = false;
        }
        public static List<Family> GetFamilys(int? orden = null) {
            string ord = "";
			switch (orden) {
                case 1:
                ord = "Codigo";
                break;
                case 2:
                ord = "Nombre";
                break;
                case 3:
                ord = "TM01";
                break;
                case 4:
                ord = "TM02";
                break;
                case 5:
                ord = "TM03";
                break;
				default:
                ord = "Id";
				break;
			}
			List<Family> familys = new List<Family>();
            RespuestaQuery res = DataBase.Query(new SqlCommand($"SELECT * FROM Family ORDER BY {ord}", Conexion));
            foreach (var reg in res.Rows) {
                Family family = JsonConvert.DeserializeObject<Family>(JsonConvert.SerializeObject(reg));
                family.Valid = true;
                familys.Add(family);
            }
            return familys;
        }
    }
}