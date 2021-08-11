using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

using WebMatrix.WebData;

namespace ATSM.Ingenieria {
	public class PIC {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int IdComponente { get; set; }
		public int IdModelo { get; set; }
		public int ATA1 { get; set; }
		public int ATA2 { get; set; }
		public int Consecutivo { get; set; }
		public int IdPosicion { get; set; }
		public string Descripcion { get; set; }
		public bool Activo { get; set; }
		public int IdUsuario { get; set; }
		public DateTime Fecha { get; set; }
		public List<ComponenteMenor> Componentes { get; set; }
		public bool Valid { get; set; }
		public ComponenteMayor Mayor { get; set; }
		public Modelo Modelo { get; set; }
		public Position Posicion { get; set; }
		public PIC(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM PIC WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        //public PIC(int idcomponente, int idmodelo, int ata1, int idposicion) {
        //    Inicializar();
        //    if (idcomponente > 0 && idmodelo > 0 && ata1 > 0 && idposicion > 0) {
        //        SqlCommand comando = new SqlCommand($"SELECT * FROM PIC WHERE IdComponente = @idcomponente AND IdModelo = @idmodelo AND ATA1 = @ata1 AND IdPosicion = @idposicion", Conexion);
        //        comando.Parameters.Add(new SqlParameter("@idcomponente", idcomponente));
        //        comando.Parameters.Add(new SqlParameter("@idmodelo", idmodelo));
        //        comando.Parameters.Add(new SqlParameter("@ata1", ata1));
        //        comando.Parameters.Add(new SqlParameter("@idposicion", idposicion));
        //        SetDatos(comando);
        //    }
        //}
        [JsonConstructor]
        public PIC(int id, int idComponente, int idModelo, int ata1, int ata2 = 0, int consecutivo = 0, int idPosicion = 0, string descripcion = "", bool activo = false, int idUsuario = 0, DateTime? fecha = null, bool valid = false) {
            Id = id;
            IdComponente = idComponente;
            IdModelo = idModelo;
            ATA1 = ata1;
            ATA2 = ata2;
            Consecutivo = consecutivo;
            IdPosicion = idPosicion;
            Descripcion = descripcion;
            Activo = activo;
            IdUsuario = idUsuario;
            Fecha = fecha ?? default(DateTime);
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Descripcion) && IdComponente > 0 && IdModelo > 0 && ATA1 > 0 && IdPosicion > 0) {
                res.Error = "";
                //SqlCommand Cmnd = new SqlCommand($"SELECT * FROM PIC WHERE Id = @id OR (IdComponente = @idcomponente AND IdModelo = @idmodelo AND ATA1 = @ata1 AND ATA2 = @ata2 AND IdPosicion = @idposicion)", Conexion);
                SqlCommand Cmnd = new SqlCommand($"SELECT * FROM PIC WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                //Cmnd.Parameters.Add(new SqlParameter("@idcomponente", IdComponente));
                //Cmnd.Parameters.Add(new SqlParameter("@idmodelo", IdModelo));
                //Cmnd.Parameters.Add(new SqlParameter("@ata1", ATA1));
                //Cmnd.Parameters.Add(new SqlParameter("@ata2", ATA2));
                //Cmnd.Parameters.Add(new SqlParameter("@idposicion", IdPosicion));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "PIC ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    if (Id != existe.Row.Id) {
                        Id = existe.Row.Id;
                        Consecutivo = existe.Row.Consecutivo;
                    }
                    SqlStr = @"UPDATE PIC SET IdComponente = @idcomponente, IdModelo = @idmodelo, ATA1 = @ata1, ATA2 = @ata2, Consecutivo = @consecutivo, IdPosicion = @idposicion, Descripcion = @descripcion, Activo = @activo, IdUsuario = @idusuario, Fecha = GETDATE() WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO PIC(IdComponente, IdModelo, ATA1, ATA2, Consecutivo, IdPosicion, Descripcion, Activo, IdUsuario, Fecha) VALUES(@idcomponente, @idmodelo, @ata1, @ata2, (SELECT ISNULL(MAX(Consecutivo),0)+1 FROM PIC WHERE IdComponente = @idcomponente AND IdModelo = @idmodelo AND ATA1 = @ata1 AND ATA2 = @ata2), @idposicion, @descripcion, @activo, @idusuario, GETDATE())";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idcomponente", IdComponente));
                Command.Parameters.Add(new SqlParameter("@idmodelo", IdModelo));
                Command.Parameters.Add(new SqlParameter("@ata1", ATA1));
                Command.Parameters.Add(new SqlParameter("@ata2", ATA2));
                Command.Parameters.Add(new SqlParameter("@consecutivo", Consecutivo));
                Command.Parameters.Add(new SqlParameter("@idposicion", IdPosicion));
                Command.Parameters.Add(new SqlParameter("@descripcion", string.IsNullOrEmpty(Descripcion) ? SqlString.Null : Descripcion));
                Command.Parameters.Add(new SqlParameter("@activo", Activo));
                Command.Parameters.Add(new SqlParameter("@idusuario", WebSecurity.CurrentUserId));
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
                        else {
                            Cmnd = new SqlCommand($"DELETE PICMenores WHERE IdPIC = @idpic", Conexion);
                            Cmnd.Parameters.Add(new SqlParameter("@idpic", Id));
                            var rDM = DataBase.Query(Cmnd);
                            if(!rDM.Valid || !string.IsNullOrEmpty(rDM.Error)) {
                                res.Error = $"Error al limpiar los Componentes Menores: (CS.{this.GetType().Name}-Save.Err.04)<br>Error: {rDM.Error}";
                                return res;
                            }
                        }
                        foreach(var menor in Componentes) {
                            Cmnd = new SqlCommand($"INSERT INTO PICMenores(IdPIC, IdComponenteMenor) VALUES(@idpic, @idmenor)", Conexion);
                            Cmnd.Parameters.Add(new SqlParameter("@idpic", Id));
                            Cmnd.Parameters.Add(new SqlParameter("@idmenor", menor.Id));
                            var rCM = DataBase.Query(Cmnd);
                            if (!rCM.Valid || !string.IsNullOrEmpty(rCM.Error)) {
                                res.Error = $"Error al Registrar el Componente Menor: (CS.{this.GetType().Name}-Save.Err.04)<br>Error: {rCM.Error}";
                                return res;
                            }
                        }
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                        return res;
                    }
                    SetMayor();
                    SetModelo();
                    SetPosicion();
                    SetComponentes();
                    Command = new SqlCommand("SELECT Consecutivo FROM PIC WHERE Id = @id", Conexion);
                    Command.Parameters.Add(new SqlParameter("id", Id));
                    var rcon = DataBase.Query(Command);
                    if (!rcon.Valid || !string.IsNullOrEmpty(rcon.Error)) {
                        res.Mensaje += "<br>No se Pudo Obtener el Consecutivo, recargue la pagina.";
                    }
                    else {
                        Consecutivo = rcon.Row.Consecutivo;
                    }
                    res.Elemento = this;
                    res.Valid = true;
                    scope.Complete();
				}
            }
            else {
                if (string.IsNullOrEmpty(Descripcion))
                    res.Error += $"<br>Falta la Descripcion del PIC";
                if (IdComponente<=0)
                    res.Error += $"<br>Falta el Componente Mayor";
                if (IdModelo <= 0)
                    res.Error += $"<br>Falta el Modelo";
                if (ATA1 <= 0)
                    res.Error += $"<br>Falta el Codigo ATA";
                if (ATA2 <= 0)
                    res.Error += $"<br>Falta el Subcodigo ATA";
                if (IdPosicion <= 0)
                    res.Error += $"<br>Falta la Posicion";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("PIC NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE PIC WHERE Id = @id;DELETE PICMenores WHERE IdPIC = @id;", Conexion);
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
                IdComponente = Registro.IdComponente;
                IdModelo = Registro.IdModelo;
                ATA1 = Registro.ATA1;
                ATA2 = Registro.ATA2;
                Consecutivo = Registro.Consecutivo;
                IdPosicion = Registro.IdPosicion;
                Descripcion = Registro.Descripcion;
                Activo = Registro.Activo;
                IdUsuario = Registro.IdUsuario;
                Fecha = Registro.Fecha;
                Valid = true;
                SetMayor();
                SetModelo();
                SetPosicion();
                SetComponentes();
            }
        }
        private void Inicializar() {
            Id = 0;
            IdComponente = 0;
            IdModelo = 0;
            ATA1 = 0;
            ATA2 = 0;
            Consecutivo = 0;
            IdPosicion = 0;
            Descripcion = "";
            Activo = false;
            IdUsuario = 0;
            Fecha = default(DateTime);
            Valid = false;
            Componentes = new List<ComponenteMenor>();
        }
        public void SetMayor() {
            Mayor = new ComponenteMayor(IdComponente);
		}
        public void SetModelo() {
            Modelo = new Modelo(IdModelo);
		}
        public void SetPosicion() {
            Posicion = new Position(IdPosicion);
		}
        public void SetComponentes() {
            Componentes = new List<ComponenteMenor>();
            SqlCommand comando = new SqlCommand("SELECT * FROM ComponenteMenor WHERE Id IN (SELECT IdComponenteMenor FROM PICMenores WHERE IdPIC = @idpic)", Conexion);
            comando.Parameters.Add(new SqlParameter("@idpic", Id));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ComponenteMenor menor = JsonConvert.DeserializeObject<ComponenteMenor>(JsonConvert.SerializeObject(reg));
                menor.Valid = true;
                Componentes.Add(menor);
            }
        }
        public static List<PIC> GetPICsByComponente(int idComponente) {
            List<PIC> pics = new List<PIC>();
            SqlCommand comando = new SqlCommand("SELECT * FROM PIC WHERE IdComponente = @idComponente", Conexion);
            comando.Parameters.Add(new SqlParameter("@idComponente", idComponente));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                PIC pic = JsonConvert.DeserializeObject<PIC>(JsonConvert.SerializeObject(reg));
                pic.Valid = true;
                pics.Add(pic);
            }
            return pics;
        }
        public static List<PIC> GetPICsByModelo(int idmodelo) {
            List<PIC> pics = new List<PIC>();
            SqlCommand comando = new SqlCommand("SELECT * FROM PIC WHERE IdModelo = @idmodelo", Conexion);
            comando.Parameters.Add(new SqlParameter("@idmodelo", idmodelo));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                PIC pic = JsonConvert.DeserializeObject<PIC>(JsonConvert.SerializeObject(reg));
                pic.Valid = true;
                pics.Add(pic);
            }
            return pics;
        }
        public static List<PIC> GetPICsByComponenteMenor(int idMenor) {
            List<PIC> pics = new List<PIC>();
            SqlCommand comando = new SqlCommand("SELECT * FROM PIC WHERE Id IN (SELECT DISTINCT IdPIC FROM PICMenores WHERE IdComponenteMenor = @idMenor)", Conexion);
            comando.Parameters.Add(new SqlParameter("@idMenor", idMenor));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                PIC pic = JsonConvert.DeserializeObject<PIC>(JsonConvert.SerializeObject(reg));
                pic.Valid = true;
                pics.Add(pic);
            }
            return pics;
        }
        public static List<PIC> GetPICsByMayorModeloAata(int idmayor,int idmodelo,int ata1) {
            List<PIC> pics = new List<PIC>();
            SqlCommand comando = new SqlCommand("SELECT * FROM PIC WHERE IdComponente = @idmayor AND IdModelo = @idmodelo AND ATA1 = @ata1", Conexion);
            comando.Parameters.Add(new SqlParameter("@idmayor", idmayor));
            comando.Parameters.Add(new SqlParameter("@idmodelo", idmodelo));
            comando.Parameters.Add(new SqlParameter("@ata1", ata1));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                PIC pic = JsonConvert.DeserializeObject<PIC>(JsonConvert.SerializeObject(reg));
                pic.Valid = true;
                pics.Add(pic);
            }
            return pics;
        }
        /// <summary>
        /// Cuenta el Numero de Componentes Menores vinculados a un PIC de un Modelo dado en un Componente Mayor
        /// </summary>
        /// <param name="idmayor">Id del Componente Mayor</param>
        /// <param name="idmodelo">Modelo del Componente Mayor Vinculado</param>
        /// <param name="idmenor">Id del Componente Menor a Contar</param>
        /// <returns></returns>
        public static dynamic MenorEnPICMayorModelo(int idmayor, int idmodelo, int idmenor) {
            int cuantos = 0;
            ComponenteMenor menor = new ComponenteMenor(idmenor);
			if (menor.Valid) {
                SqlCommand comando = new SqlCommand("SELECT ISNULL(COUNT(IdComponenteMenor),0) AS Cuantos FROM PICMenores WHERE IdComponenteMenor = @idmenor AND IdPIC IN (SELECT Id FROM PIC WHERE IdComponente = @idmayor AND IdModelo = @idmodelo)", Conexion);
                comando.Parameters.Add(new SqlParameter("@idmenor", idmenor));
                comando.Parameters.Add(new SqlParameter("@idmayor", idmayor));
                comando.Parameters.Add(new SqlParameter("@idmodelo", idmodelo));
                RespuestaQuery res = DataBase.Query(comando);
			    if (res.Valid) {
                    cuantos = res.Row.Cuantos;
                }
			}
            return new { Count = cuantos, Menor = menor };
        }
    }
}