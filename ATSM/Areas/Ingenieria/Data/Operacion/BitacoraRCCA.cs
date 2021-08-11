using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class BitacoraRCCA {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdBitacora { get; set; }
		public int No { get; set; }
		public string NoVuelo { get; set; }
		public bool Desviacion { get; set; }
		public int Altitud { get; set; }
		public int DIF1 { get; set; }
		public int DIF2 { get; set; }
		public bool Valid { get; set; }
        public BitacoraRCCA() { Inicializar(); }
        public BitacoraRCCA(int id) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM BitacoraRCCA WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public BitacoraRCCA(int idbitacora, int no) {
            Inicializar();
            if (idbitacora > 0 && no > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM BitacoraRCCA WHERE IdBitacora = @idbitacora AND No = @no", Conexion);
                comando.Parameters.Add(new SqlParameter("@idbitacora", idbitacora));
                comando.Parameters.Add(new SqlParameter("@no", no));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public BitacoraRCCA(int id, int idBitacora, int no, string noVuelo, bool desviacion = false, int altitud = 0, int dIF1 = 0, int dIF2 = 0, bool valid = false) {
            Id = id;
            IdBitacora = idBitacora;
            No = no;
            NoVuelo = noVuelo;
            Desviacion = desviacion;
            Altitud = altitud;
            DIF1 = dIF1;
            DIF2 = dIF2;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdBitacora > 0 && No > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM BitacoraRCCA WHERE Id = @id OR (IdBitacora = @idbitacora AND No = @no)", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@idbitacora", IdBitacora));
                Cmnd.Parameters.Add(new SqlParameter("@no", No));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "BitacoraRCCA ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE BitacoraRCCA SET NoVuelo = @novuelo, Desviacion = @desviacion, Altitud = @altitud, DIF1 = @dif1, DIF2 = @dif2 WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO BitacoraRCCA(IdBitacora, No, NoVuelo, Desviacion, Altitud, DIF1, DIF2) VALUES(@idbitacora, @no, @novuelo, @desviacion, @altitud, @dif1, @dif2)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idbitacora", IdBitacora));
                Command.Parameters.Add(new SqlParameter("@no", No));
                Command.Parameters.Add(new SqlParameter("@novuelo", NoVuelo));
                Command.Parameters.Add(new SqlParameter("@desviacion", Desviacion));
                Command.Parameters.Add(new SqlParameter("@altitud", Altitud));
                Command.Parameters.Add(new SqlParameter("@dif1", DIF1));
                Command.Parameters.Add(new SqlParameter("@dif2", DIF2));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid) {
                    if (Insr) {
                        if (rInUp.IdRegistro == 0) {
                            res.Error = $"No se pudo obtener el IdBitacora Insertado(CS.{this.GetType().Name}-Save.Err.03)<br>{SqlStr}<br> Error: {rInUp.Error}";
                            return res;
                        }
                        IdBitacora = rInUp.IdRegistro;
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
                if (IdBitacora <= 0)
                    res.Error += $"<br>Falta la Bitacora";
                if (No <= 0)
                    res.Error += $"<br>Falta Numero de Registro";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("BitacoraRCCA NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE BitacoraRCCA WHERE Id = @id", Conexion);
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
                IdBitacora = Registro.IdBitacora;
                No = Registro.No;
                NoVuelo = Registro.NoVuelo;
                Desviacion = Registro.Desviacion;
                Altitud = Registro.Altitud;
                DIF1 = Registro.DIF1;
                DIF2 = Registro.DIF2;
                Valid = true;
            }
        }
        private void Inicializar() {
            Id = 0;
            IdBitacora = 0;
            No = 0;
            NoVuelo = "";
            Desviacion = false;
            Altitud = 0;
            DIF1 = 0;
            DIF2 = 0;
            Valid = false;
        }
        public static List<BitacoraRCCA> GetBitacoraRCCAs(int idBitacora) {
            List<BitacoraRCCA> bitacorarccas = new List<BitacoraRCCA>();
            SqlCommand comando = new SqlCommand("SELECT * FROM BitacoraRCCA WHERE IdBitacora = @idBitacora", Conexion);
            comando.Parameters.Add(new SqlParameter("@idBitacora", idBitacora));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                BitacoraRCCA bitacorarcca = JsonConvert.DeserializeObject<BitacoraRCCA>(JsonConvert.SerializeObject(reg));
                bitacorarcca.Valid = true;
                bitacorarccas.Add(bitacorarcca);
            }
            return bitacorarccas;
        }
    }
}