using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

using ATSM.Tripulaciones;

using Newtonsoft.Json;

using WebMatrix.WebData;

namespace ATSM.Cuentas {
	public class Account {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Nombre { get; set; }
		public int IdTipo { get; set; }
        public string Banco { get; set; }
        public string Cuenta { get; set; }
        public string CLABE { get; set; }
        public string AMEX { get; set; }
		public string Celular { get; set; }
		public string Correo { get; set; }
		public int? IdCrew { get; set; }
		public int? IdUsuario { get; set; }
		public int IdUsu { get; set; }
		public DateTime Alta { get; set; }
		public bool Activo { get; set; }
		public string Estacion { get; set; }
		public bool Valid { get; set; }
        public AccountType Tipo { get; set; }
        public Crew Crew { get; set; }
        public Account(string nombre, int idTipo) {
            Inicializar();
            if (!string.IsNullOrEmpty(nombre)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Account WHERE Nombre = @nombre AND IdTipo = @idtipo", Conexion);
                comando.Parameters.Add(new SqlParameter("@nombre", nombre));
                comando.Parameters.Add(new SqlParameter("@idtipo", idTipo));
                SetDatos(comando);
            }
        }
        public Account(int? id = null)
        {
            Inicializar();
            if (id > 0)
            {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Account WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
		public Account(int idusuario, int idtipo)
		{
			Inicializar();
			if (idusuario > 0 && idtipo > 0)
			{
				SqlCommand comando = new SqlCommand($"SELECT * FROM Account WHERE IdTipo = @idtipo AND IdUsuario = @idusuario", Conexion);
				comando.Parameters.Add(new SqlParameter("@idtipo", idtipo));
				comando.Parameters.Add(new SqlParameter("@idusuario", idusuario));
				SetDatos(comando);
			}
		}
		[JsonConstructor]
        public Account(int id, string nombre, int idTipo = 0, string banco = "", string cuenta = "", string cLABE = "", string aMEX = "", string celular = "", string correo = "", int? idCrew = null, int? idusuario = null, int idUsu = 0, DateTime alta = default(DateTime), bool activo = false, string estacion = "", bool valid = false) {
            Id = id;
            Nombre = nombre;
            IdTipo = idTipo;
            Banco = banco;
            Cuenta = cuenta;
            CLABE = cLABE;
            AMEX = aMEX;
            Celular = celular;
            Correo = correo;
            IdCrew = idCrew;
            IdUsuario = idusuario;
            IdUsu = idUsu;
            Alta = alta;
            Activo = activo;
            Estacion = estacion;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Nombre) && IdTipo > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Account WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Account ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE Account SET Nombre = @nombre, IdTipo = @idtipo, Banco = @banco, Cuenta = @cuenta, CLABE = @clabe, AMEX = @amex, Celular = @celular, Correo = @correo, IdCrew = @idcrew, IdUsuario = @idusuario, IdUsu = @idusu, Activo = @activo, Estacion = @estacion WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Account(Nombre, IdTipo, Banco, Cuenta, CLABE, AMEX, Celular, Correo, IdCrew, IdUsuario, IdUsu, Alta, Activo, Estacion) VALUES(@nombre, @idtipo, @banco, @cuenta, @clabe, @amex, @celular, @correo, @idcrew, @idusuario, @idusu, GETDATE(), @activo, @estacion)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@nombre", string.IsNullOrEmpty(Nombre) ? SqlString.Null : Nombre));
                Command.Parameters.Add(new SqlParameter("@idtipo", IdTipo));
                Command.Parameters.Add(new SqlParameter("@banco", string.IsNullOrEmpty(Banco) ? SqlString.Null : Banco));
                Command.Parameters.Add(new SqlParameter("@cuenta", string.IsNullOrEmpty(Cuenta) ? SqlString.Null : Cuenta));
                Command.Parameters.Add(new SqlParameter("@clabe", string.IsNullOrEmpty(CLABE) ? SqlString.Null : CLABE));
                Command.Parameters.Add(new SqlParameter("@amex", string.IsNullOrEmpty(AMEX) ? SqlString.Null : AMEX));
                Command.Parameters.Add(new SqlParameter("@celular", string.IsNullOrEmpty(Celular) ? SqlString.Null : Celular));
                Command.Parameters.Add(new SqlParameter("@correo", string.IsNullOrEmpty(Correo) ? SqlString.Null : Correo));
                Command.Parameters.Add(new SqlParameter("@idcrew", IdCrew ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idusuario", IdUsuario ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@idusu", WebSecurity.CurrentUserId));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                Command.Parameters.Add(new SqlParameter("@estacion", string.IsNullOrEmpty(Estacion) ? SqlString.Null : Estacion));
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
            }
            else {
                if (string.IsNullOrEmpty(Nombre))
                    res.Error = "<br>Falta el Nombre del Beneficiario de la Cuenta.";
                if (IdTipo==0)
                    res.Error = "<br>Falta el Tipo de Cuenta.";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Account NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Account WHERE Id = @id", Conexion);
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
        public Respuesta SetUsuario(int idUsuario)
		{
            Respuesta res = new Respuesta("No se puede Asignar usuario a la cuenta.");
			if (Valid)
			{
                SqlCommand comando = new SqlCommand($"UPDATE Account SET IdUsuario = @idusuario WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@idusuario", idUsuario));
                comando.Parameters.Add(new SqlParameter("@id", Id));
                var r = DataBase.Query(comando);
                if(!r.Valid || !string.IsNullOrEmpty(r.Error))
                    res.Error = r.Error;
                else 
                    res.Valid = true;
			}
            return res;
		}
        public Respuesta SetCrew(int idCrew)
        {
            Respuesta res = new Respuesta("Aun no existe la cuenta para Asignar el Capitan");
            if (Valid)
            {
                SqlCommand comando = new SqlCommand($"UPDATE Account SET IdCrew = @idcrew WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@idcrew", idCrew));
                comando.Parameters.Add(new SqlParameter("@id", Id));
                var r = DataBase.Query(comando);
                if (!r.Valid || !string.IsNullOrEmpty(r.Error))
                    res.Error = r.Error;
                else
                    res.Valid = true;
            }
            return res;
        }
        private void SetDatos(SqlCommand Command) {
            RespuestaQuery res = DataBase.Query(Command);
            if (res.Valid) {
                var Registro = res.Row;
                Id = Registro.Id;
                Nombre = Registro.Nombre;
                IdTipo = Registro.IdTipo;
                Banco = Registro.Banco;
                Cuenta = Registro.Cuenta;
                CLABE = Registro.CLABE;
                AMEX = Registro.AMEX;
                Celular = Registro.Celular;
                Correo = Registro.Correo;
                IdCrew = Registro.IdCrew;
                IdUsuario = Registro.IdUsuario;
                IdUsu = Registro.IdUsu;
                Alta = Registro.Alta;
                Activo = Registro.Activo;
                Estacion = Registro.Estacion;
                Valid = true;
                GetTipo();
                GetCrew();
            }
        }
        private void Inicializar() {
            Id = 0;
            Nombre = "";
            IdTipo = 0;
            Banco = null;
            Cuenta = null;
            CLABE = null;
            AMEX = null;
            Celular = null;
            Correo = null;
            IdCrew = null;
            IdUsuario = null;
            IdUsu = 0;
            Alta = default(DateTime);
            Activo = false;
            Estacion = null;
            Valid = false;
            Tipo = new AccountType();
        }
        public static List<Account> GetAccounts(int? tipo) {
            List<Account> accounts = new List<Account>();
            SqlCommand comando = new SqlCommand($"SELECT * FROM Account{(tipo != null ? $" WHERE IdTipo = @tipo" : "")} ORDER BY IdTipo, Nombre", Conexion);
            comando.Parameters.Add(new SqlParameter("@tipo", tipo ?? SqlInt32.Null));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                Account account = JsonConvert.DeserializeObject<Account>(JsonConvert.SerializeObject(reg));
                account.Valid = true;
                account.GetTipo();
                account.GetCrew();
                accounts.Add(account);
            }
            return accounts;
        }
        public void GetTipo() {
            Tipo = new AccountType(IdTipo);
		}
        public void GetCrew() {
            Crew = new Crew(IdCrew);
		}
    }
}