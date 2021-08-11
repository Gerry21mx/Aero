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
	public class ItemMenor {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public int? IdPIC { get; set; }
		public int IdItemMayor { get; set; }
		public int IdComponenteMenor { get; set; }
		public string Serie { get; set; }
		public bool? Controlado { get; set; }
		private int? _idStatus { get; set; }
		public int? IdStatus { 
			get {
				return _idStatus;
			} 
			set {
				Status = new Status(value);
				_idStatus = Status.Id;
			} 
		}
		public string Metodo_Cumplimiento { get; set; }
		public decimal? TSN { get; set; }
		public int? CSN { get; set; }
		public int? Dias { get; set; }
		public decimal? TSN_Componente_Instalacion { get; set; }
		public int? CSN_Componente_Instalacion { get; set; }
		public DateTime? Fecha_Componente_Instalacion { get; set; }
		public decimal? TSN_Airframe_Instalacion { get; set; }
		public int? CSN_Airframe_Instalacion { get; set; }
		public DateTime? Fecha_Airframe_Instalacion { get; set; }
        //public List<Limite> Limites { get; set; }
        public List<Tiempos> Tiempos = new List<Tiempos>();
		public int IdUsuario { get; set; }
		public DateTime Fecha { get; set; }
		public bool Valid { get; set; }
		public PIC PIC { get; set; }
		public ItemMayor Mayor { get; set; }
		public ComponenteMenor Menor { get; set; }
		public Status Status { get; set; }
        public ItemMenor(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ItemMenor WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public ItemMenor(int idmayor, int idMenor, int? idPic = null) {
            Inicializar();
            if (idmayor > 0 && idMenor > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ItemMenor WHERE IdItemMayor = @iditemmayor AND IdComponenteMenor = @idMenor AND IdPIC {(idPic==null?" IS NULL": " = @idPic")}", Conexion);
                comando.Parameters.Add(new SqlParameter("@iditemmayor", idmayor));
                comando.Parameters.Add(new SqlParameter("@idMenor", idMenor));
                comando.Parameters.Add(new SqlParameter("@idPic", idPic ?? SqlInt32.Null));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public ItemMenor(int id, int idItemMayor, int idComponenteMenor, string serie, int? idPIC = null, bool? controlado = null, int? idStatus = null, string metodo_Cumplimiento = "", decimal? tSN = null, int? cSN = null, decimal? tSN_Componente_Instalacion = null, int? cSN_Componente_Instalacion = null, DateTime? fecha_Componente_Instalacion = null, decimal? tSN_Airframe_Instalacion = null, int? cSN_Airframe_Instalacion = null, DateTime? fecha_Airframe_Instalacion = null, int idUsuario = 0, DateTime? fecha = null, bool valid = false) {
            Id = id;
            IdPIC = idPIC;
            IdItemMayor = idItemMayor;
            IdComponenteMenor = idComponenteMenor;
            Serie = serie;
            Controlado = controlado;
            IdStatus = idStatus;
            Metodo_Cumplimiento = metodo_Cumplimiento;
            TSN = tSN;
            CSN = cSN;
            TSN_Componente_Instalacion = tSN_Componente_Instalacion;
            CSN_Componente_Instalacion = cSN_Componente_Instalacion;
            Fecha_Componente_Instalacion = fecha_Componente_Instalacion;
            TSN_Airframe_Instalacion = tSN_Airframe_Instalacion;
            CSN_Airframe_Instalacion = cSN_Airframe_Instalacion;
            Fecha_Airframe_Instalacion = fecha_Airframe_Instalacion;
            IdUsuario = idUsuario;
            Fecha = fecha?? default(DateTime);
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdItemMayor > 0 && IdComponenteMenor > 0) {
                res.Error = "";
                //      VALIDACIONES
                SetItemMayor();
				if (!Mayor.Valid) {
                    res.Error = "Hay un error al Identificar el Componente Mayor en donde se Instalara el Item.";
                    return res;
                }
                SetComponenteMenor();
				if (Menor.Valid) {
                    if(Menor.IdTipoMenor==1) {
                        Controlado = true;
                        if (IdPIC == null || IdPIC <= 0) {
                            res.Error = "Falta PIC del Item";
                            return res;
						}
						if (Menor.Serie) {
                            SqlCommand Comand = new SqlCommand($"SELECT Id FROM ItemMenor WHERE IdComponenteMenor = @idMenor AND Serie = @serie",Conexion);
                            Comand.Parameters.Add(new SqlParameter("@idMenor", IdComponenteMenor));
                            Comand.Parameters.Add(new SqlParameter("@serie", Serie));
                            var rex = DataBase.Query(Comand);
							if (rex.Valid) {
								if (rex.Row.Id != Id) {
                                    res.Error = "Este Numero de Serie ya esta Instalado en otro Item.";
                                    return res;
                                }
							}
                        }
                    }
                    else {
                        IdPIC = null;
                    }
				}
                else {
                    res.Error = "Hay un error al Identificar el Componente Menor al que pertenece el Item";
                    return res;
                }


                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM ItemMenor WHERE Id = @id OR (IdItemMayor = @iditemmayor AND {(Menor.IdTipoMenor == 1 ? "IdPIC = @idpic": "IdComponenteMenor = @idcomponentemenor")})", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                Cmnd.Parameters.Add(new SqlParameter("@iditemmayor", IdItemMayor));
                Cmnd.Parameters.Add(new SqlParameter("@idpic", IdPIC ?? SqlInt32.Null));
                Cmnd.Parameters.Add(new SqlParameter("@idcomponentemenor", IdComponenteMenor));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "ItemMenor ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    Id = existe.Row.Id;
                    SqlStr = @"UPDATE ItemMenor SET IdPIC = @idpic, IdItemMayor = @iditemmayor, IdComponenteMenor = @idcomponentemenor, Serie = @serie, Controlado = @controlado, IdStatus = @idstatus, Metodo_Cumplimiento = @metodo_cumplimiento, TSN = @tsn, CSN = @csn, TSN_Componente_Instalacion = @tsn_componente_instalacion, CSN_Componente_Instalacion = @csn_componente_instalacion, Fecha_Componente_Instalacion = @fecha_componente_instalacion, TSN_Airframe_Instalacion = @tsn_airframe_instalacion, CSN_Airframe_Instalacion = @csn_airframe_instalacion, Fecha_Airframe_Instalacion = @fecha_airframe_instalacion, IdUsuario = @idusuario, Fecha = GETDATE() WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO ItemMenor(IdPIC, IdItemMayor, IdComponenteMenor, Serie, Controlado, IdStatus, Metodo_Cumplimiento, TSN, CSN, TSN_Componente_Instalacion, CSN_Componente_Instalacion, Fecha_Componente_Instalacion, TSN_Airframe_Instalacion, CSN_Airframe_Instalacion, Fecha_Airframe_Instalacion, IdUsuario, Fecha) VALUES(@idpic, @iditemmayor, @idcomponentemenor, @serie, @controlado, @idstatus, @metodo_cumplimiento, @tsn, @csn, @tsn_componente_instalacion, @csn_componente_instalacion, @fecha_componente_instalacion, @tsn_airframe_instalacion, @csn_airframe_instalacion, @fecha_airframe_instalacion, @idusuario, GETDATE())";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idpic", IdPIC ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@iditemmayor", IdItemMayor));
                Command.Parameters.Add(new SqlParameter("@idcomponentemenor", IdComponenteMenor));
                Command.Parameters.Add(new SqlParameter("@serie", string.IsNullOrEmpty(Serie) ? SqlString.Null : Serie));
                Command.Parameters.Add(new SqlParameter("@controlado", Controlado ?? SqlBoolean.Null));
                Command.Parameters.Add(new SqlParameter("@idstatus", IdStatus ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@metodo_cumplimiento", string.IsNullOrEmpty(Metodo_Cumplimiento) ? SqlString.Null : Metodo_Cumplimiento));
                Command.Parameters.Add(new SqlParameter("@tsn", TSN ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@csn", CSN ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@tsn_componente_instalacion", TSN_Componente_Instalacion ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@csn_componente_instalacion", CSN_Componente_Instalacion ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@fecha_componente_instalacion", Fecha_Componente_Instalacion ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@tsn_airframe_instalacion", TSN_Airframe_Instalacion ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@csn_airframe_instalacion", CSN_Airframe_Instalacion ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@fecha_airframe_instalacion", Fecha_Airframe_Instalacion ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@idusuario", WebSecurity.CurrentUserId));
                using (TransactionScope scope = new TransactionScope()) {
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
                            var tmps = ATSM.Ingenieria.Tiempos.GetTiempos(Id);
                            foreach (var tiempo in tmps) {
                                var idx = this.Tiempos.FindIndex(t => { return t.Id == tiempo.Id; });
                                if (idx == -1)
                                    tiempo.Delete();
                            }
                        }
                        if (this.Tiempos != null) {
                            foreach (var tiempo in this.Tiempos) {
                                tiempo.IdItemMayor = null;
                                tiempo.IdItemMenor = Id;
                                var rST = tiempo.Save();
                                if (!rST.Valid || !string.IsNullOrEmpty(rST.Error)) {
                                    res.Error = $"Error al Registrar los Tiempos: (CS.{this.GetType().Name}-Save.Err.04)<br>Error: {rST.Error}";
                                    return res;
                                }
                            }
                        }
                    }
                    else {
                        res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                        return res;
                    }
                    SetPIC();
                    SetItemMayor();
                    SetComponenteMenor();
                    res.Elemento = this;
                    res.Valid = true;
                    scope.Complete();
				}
            }
            else {
                if (IdItemMayor > 0)
                    res.Error += $"<br>Falta el Item Mayor en Donde esta Instalado";
                if (IdComponenteMenor > 0)
                    res.Error += $"<br>No se Ha Identificado el Tipo de Componente que desea Instalar";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("ItemMenor NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE ItemMenor WHERE Id = @id;DELETE Tiempos WHERE IdItemMenor = @id;", Conexion);
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
                IdPIC = Registro.IdPIC;
                IdItemMayor = Registro.IdItemMayor;
                IdComponenteMenor = Registro.IdComponenteMenor;
                Serie = Registro.Serie;
                Controlado = Registro.Controlado;
                IdStatus = Registro.IdStatus;
                Metodo_Cumplimiento = Registro.Metodo_Cumplimiento;
                TSN = Registro.TSN;
                CSN = Registro.CSN;
                TSN_Componente_Instalacion = Registro.TSN_Componente_Instalacion;
                CSN_Componente_Instalacion = Registro.CSN_Componente_Instalacion;
                Fecha_Componente_Instalacion = Registro.Fecha_Componente_Instalacion;
                TSN_Airframe_Instalacion = Registro.TSN_Airframe_Instalacion;
                CSN_Airframe_Instalacion = Registro.CSN_Airframe_Instalacion;
                Fecha_Airframe_Instalacion = Registro.Fecha_Airframe_Instalacion;
                IdUsuario = Registro.IdUsuario;
                Fecha = Registro.Fecha;
                Valid = true;
                SetPIC();
                SetItemMayor();
                SetComponenteMenor();
                SetTiempos();
            }
        }
        private void Inicializar() {
            Id = 0;
            IdPIC = null;
            IdItemMayor = 0;
            IdComponenteMenor = 0;
            Serie = "";
            Controlado = null;
            IdStatus = null;
            Metodo_Cumplimiento = "";
            TSN = null;
            CSN = null;
            TSN_Componente_Instalacion = null;
            CSN_Componente_Instalacion = null;
            Fecha_Componente_Instalacion = null;
            TSN_Airframe_Instalacion = null;
            CSN_Airframe_Instalacion = null;
            Fecha_Airframe_Instalacion = null;
            IdUsuario = 0;
            Fecha = default(DateTime);
            Valid = false;
            //Tiempos = new List<Tiempos>();
        }
        public void SetPIC() {
			PIC = new PIC(IdPIC);
		}
		public void SetItemMayor() {
			Mayor = new ItemMayor(IdItemMayor);
		}
		public void SetComponenteMenor() {
			Menor = new ComponenteMenor(IdComponenteMenor);
        }
        public void SetTiempos() {
            Tiempos = ATSM.Ingenieria.Tiempos.GetTiempos(Id);
        }
        public static dynamic GetItemMenors(int idAircraft, int? idComponenteMayor, int? idMayor, int? idFamilia) {
            //string fields= "IM.*, CM.Part, CM.Description, CM.IdFamily, CM.IdTipoMenor, CM.ATA1, CM.ATA2, CM.ATA3, CM.Directive, CM.Amendment, CM.AD_Date, CM.Efectivity, CM.ServiceBulletin, CM.Review, CM.SB_Date, CM.Threshold, IMY.IdComponente, IMY.IdAircraft, IMY.IdModelo, IMY.IdPosicion, IMY.IdEstadoMayor, (SELECT Codigo FROM Family WHERE Id = (SELECT IdFamily FROM ComponenteMenor WHERE Id = IdComponenteMenor)) AS Family, (SELECT Descripcion FROM PIC WHERE Id = IdPIC) AS PIC";

            //string conditions = "WHERE Controlado = 1 AND IMY.IdAircraft = @idAircraft";
            //conditions += idFamilia != null ? $" AND CM.IdFamily = @idFamily" : "";
            //conditions += $" AND IdItemMayor {(idMayor != null && idMayor > 0? "= @idMayor" : idComponenteMayor==null?"> 0 ": "IN (SELECT Id FROM ItemMayor WHERE IdAircraft=@idAircraft AND IdComponente = @idComponenteMayor)")}";

            //string sqlQry = $"SELECT {fields} FROM ItemMenor IM LEFT JOIN ComponenteMenor CM ON IM.IdComponenteMenor = CM.Id LEFT JOIN ItemMayor IMY ON IMY.Id = IM.IdItemMayor {conditions} ORDER BY ATA1,ATA2,ATA3";

            SqlCommand comando = new SqlCommand($"EXEC ING_Items_Menores_Filtro_01 @idAircraft, @idComponenteMayor, @idMayor, @idFamily", Conexion);
            comando.Parameters.Add(new SqlParameter("@idAircraft", idAircraft));
            comando.Parameters.Add(new SqlParameter("@idComponenteMayor", idComponenteMayor ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@idMayor", idMayor ?? SqlInt32.Null));
            comando.Parameters.Add(new SqlParameter("@idFamily", idFamilia ?? SqlInt32.Null));
            RespuestaQuery res = DataBase.Query(comando);
            return res.Rows;
        }
    }
}