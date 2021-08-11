using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Newtonsoft.Json;

namespace ATSM {
	public class Empresa {
		private static SqlConnection Conexion = DataBase.Conexion("GTSM");
		public int IdEmpresa { get; set; }
		public string Nombre { get; set; }
		public string Calle { get; set; }
		public string Colonia { get; set; }
		public int? CP { get; set; }
		public string Ciudad { get; set; }
		public string Estado { get; set; }
		public string Pais { get; set; }
		public string RFC { get; set; }
		public string TaxId { get; set; }
		public string Telefono { get; set; }
		public string WEB { get; set; }
		public string Representante { get; set; }
		public string CodigoVuelo { get; set; }
		public string Biweekly { get; set; }
		public string Tipo { get; set; }
		public bool Valid { get; set; }
        public Empresa(int? idempresa = null) {
            Inicializar();
            if (idempresa > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Empresa WHERE Id_Empresa = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", idempresa));
                SetDatos(comando);
            }
        }
        public Empresa(string nombre) {
            Inicializar();
            if (!string.IsNullOrEmpty(nombre)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Empresa WHERE Nombre = @nombre", Conexion);
                comando.Parameters.Add(new SqlParameter("@nombre", nombre));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Empresa(int id_Empresa, string nombre, string calle = null, string colonia = null, int? cP = null, string ciudad = null, string estado = null, string pais = null, string rFC = null, string taxId = null, string telefono = null, string wEB = null, string representante = null, string codigoVuelo = null, string biweekly = null, string tipo = null, bool valid = false) {
            IdEmpresa = id_Empresa;
            Nombre = nombre;
            Calle = calle;
            Colonia = colonia;
            CP = cP;
            Ciudad = ciudad;
            Estado = estado;
            Pais = pais;
            RFC = rFC;
            TaxId = taxId;
            Telefono = telefono;
            WEB = wEB;
            Representante = representante;
            CodigoVuelo = codigoVuelo;
            Biweekly = biweekly;
            Tipo = tipo;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nombre)) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id_Empresa FROM Empresa WHERE Id_Empresa = @id OR Nombre = @nombre", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", IdEmpresa));
                Cmnd.Parameters.Add(new SqlParameter("@nombre", Nombre));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Empresa ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Empresa SET Nombre = @nombre, Calle = @calle, Colonia = @colonia, CP = @cp, Ciudad = @ciudad, Estado = @estado, Pais = @pais, RFC = @rfc, TaxId = @taxid, Telefono = @telefono, WEB = @web, Representante = @representante, CodigoVuelo = @codigovuelo, Biweekly = @biweekly, Tipo = @tipo WHERE Id_Empresa=@id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Empresa(Nombre, Calle, Colonia, CP, Ciudad, Estado, Pais, RFC, TaxId, Telefono, WEB, Representante, CodigoVuelo, Biweekly, Tipo) VALUES(@nombre, @calle, @colonia, @cp, @ciudad, @estado, @pais, @rfc, @taxid, @telefono, @web, @representante, @codigovuelo, @biweekly, @tipo)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idempresa", IdEmpresa));
                Command.Parameters.Add(new SqlParameter("@nombre", Nombre));
                Command.Parameters.Add(new SqlParameter("@calle", string.IsNullOrEmpty(Calle) ? SqlString.Null : Calle));
                Command.Parameters.Add(new SqlParameter("@colonia", string.IsNullOrEmpty(Colonia) ? SqlString.Null : Colonia));
                Command.Parameters.Add(new SqlParameter("@cp", CP ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@ciudad", string.IsNullOrEmpty(Ciudad) ? SqlString.Null : Ciudad));
                Command.Parameters.Add(new SqlParameter("@estado", string.IsNullOrEmpty(Estado) ? SqlString.Null : Estado));
                Command.Parameters.Add(new SqlParameter("@pais", string.IsNullOrEmpty(Pais) ? SqlString.Null : Pais));
                Command.Parameters.Add(new SqlParameter("@rfc", string.IsNullOrEmpty(RFC) ? SqlString.Null : RFC));
                Command.Parameters.Add(new SqlParameter("@taxid", string.IsNullOrEmpty(TaxId) ? SqlString.Null : TaxId));
                Command.Parameters.Add(new SqlParameter("@telefono", string.IsNullOrEmpty(Telefono) ? SqlString.Null : Telefono));
                Command.Parameters.Add(new SqlParameter("@web", string.IsNullOrEmpty(WEB) ? SqlString.Null : WEB));
                Command.Parameters.Add(new SqlParameter("@representante", string.IsNullOrEmpty(Representante) ? SqlString.Null : Representante));
                Command.Parameters.Add(new SqlParameter("@codigovuelo", string.IsNullOrEmpty(CodigoVuelo) ? SqlString.Null : CodigoVuelo));
                Command.Parameters.Add(new SqlParameter("@biweekly", string.IsNullOrEmpty(Biweekly) ? SqlString.Null : Biweekly));
                Command.Parameters.Add(new SqlParameter("@tipo", string.IsNullOrEmpty(Tipo) ? SqlString.Null : Tipo));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid) {
                    if (Insr) {
                        if (rInUp.IdRegistro == 0) {
                            res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br> Error: {rInUp.Error}";
                            return res;
                        }
                        IdEmpresa = rInUp.IdRegistro;
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
            Respuesta res = new Respuesta("La Empresa NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Empresa WHERE Id_Empresa = @id", Conexion);
            Command.Parameters.Add(new SqlParameter("@id", IdEmpresa));
            var resD = DataBase.Execute(Command);
            if (resD.Valid && resD.Afectados > 0) {
                res.Valid = true;
                res.Error = "";
                res.Mensaje = "Eliminada Correctamente";
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
                IdEmpresa = Registro.Id_Empresa;
                Nombre = Registro.Nombre;
                Calle = Registro.Calle;
                Colonia = Registro.Colonia;
                CP = Registro.CP;
                Ciudad = Registro.Ciudad;
                Estado = Registro.Estado;
                Pais = Registro.Pais;
                RFC = Registro.RFC;
                TaxId = Registro.TaxId;
                Telefono = Registro.Telefono;
                WEB = Registro.Pagina;
                Representante = Registro.Nombre_Rep;
                CodigoVuelo = Registro.codigo_vuelo;
                Biweekly = Registro.Biweekly;
                Tipo = Registro.tipo;
                Valid = true;
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdEmpresa = 0;
            Nombre = "";
            Calle = null;
            Colonia = null;
            CP = null;
            Ciudad = null;
            Estado = null;
            Pais = null;
            RFC = null;
            TaxId = null;
            Telefono = null;
            WEB = null;
            Representante = null;
            CodigoVuelo = null;
            Biweekly = null;
            Tipo = null;
            Valid = false;
        }
        public static List<Empresa> GetEmpresas() {
            List<Empresa> empresas = new List<Empresa>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Empresa", Conexion));
            foreach (var reg in res.Rows) {
                Empresa empresa = JsonConvert.DeserializeObject<Empresa>(JsonConvert.SerializeObject(reg));
                empresa.Valid = true;
                empresas.Add(empresa);
            }
            return empresas;
        }
    }
}