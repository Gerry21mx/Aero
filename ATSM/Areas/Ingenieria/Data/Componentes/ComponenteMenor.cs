using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ATSM.Ingenieria {
	public class ComponenteMenor {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int Id { get; set; }
		public string Part { get; set; }
		public string Description { get; set; }
		public int IdFamily { get; set; }
        private int _idTipoMenor { get; set; }
        public int IdTipoMenor {
            get {
                return _idTipoMenor;
            }
            set {
                Tipo = new TipoMenor(value);
                _idTipoMenor = Tipo.Id;
            } 
        }
		public int? ATA1 { get; set; }
		public int? ATA2 { get; set; }
		public int? ATA3 { get; set; }
		public string Directive { get; set; }
		public string Amendment { get; set; }
		public DateTime? AD_Date { get; set; }
		public string Efectivity { get; set; }
		public string ServiceBulletin { get; set; }
		public string Review { get; set; }
		public DateTime? SB_Date { get; set; }
		public string Threshold { get; set; }
		public string Observaciones { get; set; }
		public decimal? Condition_From_Hours { get; set; }
		public decimal? Condition_To_Hours { get; set; }
		public int? Condition_From_Cycles { get; set; }
		public int? Condition_To_Cycles { get; set; }
		public DateTime? Condition_From_Date { get; set; }
		public DateTime? Condition_To_Date { get; set; }
		public int Usuario { get; set; }
		public bool Serie { get; set; }
		public int NoPadres { get; set; }
		public bool Valid { get; set; }
		public TipoMenor Tipo { get; set; }
        public Family Familia { get; set; }
        public List<ComponenteMenorModelo> Modelos { get; set; }
        public List<ComponenteMenorVinculado> Vinculados { get; set; }
		public List<Limite> Limites { get; set; }
        public ComponenteMenor(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ComponenteMenor WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        public ComponenteMenor(string part) {
            Inicializar();
            if (!string.IsNullOrEmpty(part)) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM ComponenteMenor WHERE Part = @part", Conexion);
                comando.Parameters.Add(new SqlParameter("@part", part));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public ComponenteMenor(int id, string part, string description, int idFamily = 0, int idTipoMenor = 0, int? aTA1 = null, int? aTA2 = null, int? aTA3 = null, string directive = null, string amendment = null, DateTime? aD_Date = null, string efectivity = null, string serviceBulletin = null, string review = null, DateTime? sB_Date = null, string threshold = null, string observaciones = null, decimal? condition_From_Hours = null, decimal? condition_To_Hours = null, int? condition_From_Cycles = null, int? condition_To_Cycles = null, DateTime? condition_From_Date = null, DateTime? condition_To_Date = null, int usuario = 0, bool serie = false, bool valid = false) {
            Id = id;
            Part = part;
            Description = description;
            IdFamily = idFamily;
            IdTipoMenor = idTipoMenor;
            ATA1 = aTA1;
            ATA2 = aTA2;
            ATA3 = aTA3;
            Directive = directive;
            Amendment = amendment;
            AD_Date = aD_Date;
            Efectivity = efectivity;
            ServiceBulletin = serviceBulletin;
            Review = review;
            SB_Date = sB_Date;
            Threshold = threshold;
            Observaciones = observaciones;
            Condition_From_Hours = condition_From_Hours;
            Condition_To_Hours = condition_To_Hours;
            Condition_From_Cycles = condition_From_Cycles;
            Condition_To_Cycles = condition_To_Cycles;
            Condition_From_Date = condition_From_Date;
            Condition_To_Date = condition_To_Date;
            Usuario = usuario;
            Serie = serie;
            Valid = valid;
            NoPadres = 0;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (!string.IsNullOrEmpty(Part) && !string.IsNullOrEmpty(Description) && IdFamily > 0 && Modelos.Count > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM ComponenteMenor WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "ComponenteMenor ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE ComponenteMenor SET Part = @part, Description = @description, IdFamily = @idfamilia, IdTipoMenor = @idTipoMenor, ATA1 = @ata1, ATA2 = @ata2, ATA3 = @ata3, Directive = @directive, Amendment = @amendment, AD_Date = @ad_date, Efectivity = @efectivity, ServiceBulletin = @servicebulletin, Review = @review, SB_Date = @sb_date, Threshold = @threshold, Observaciones = @observaciones, Condition_From_Hours = @condition_from_hours, Condition_To_Hours = @condition_to_hours, Condition_From_Cycles = @condition_from_cycles, Condition_To_Cycles = @condition_to_cycles, Condition_From_Date = @condition_from_date, Condition_To_Date = @condition_to_date, Usuario = @usuario, Serie = @serie WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO ComponenteMenor(Part, Description, IdFamily, IdTipoMenor, ATA1, ATA2, ATA3, Directive, Amendment, AD_Date, Efectivity, ServiceBulletin, Review, SB_Date, Threshold, Observaciones, Condition_From_Hours, Condition_To_Hours, Condition_From_Cycles, Condition_To_Cycles, Condition_From_Date, Condition_To_Date, Usuario, Serie) VALUES(@part, @description, @idfamilia, @idTipoMenor, @ata1, @ata2, @ata3, @directive, @amendment, @ad_date, @efectivity, @servicebulletin, @review, @sb_date, @threshold, @observaciones, @condition_from_hours, @condition_to_hours, @condition_from_cycles, @condition_to_cycles, @condition_from_date, @condition_to_date, @usuario, @serie)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@part", Part));
                Command.Parameters.Add(new SqlParameter("@description", Description));
                Command.Parameters.Add(new SqlParameter("@idfamilia", IdFamily));
                Command.Parameters.Add(new SqlParameter("@idTipoMenor", IdTipoMenor));
                Command.Parameters.Add(new SqlParameter("@ata1", ATA1 ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@ata2", ATA2 ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@ata3", ATA3 ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@directive", string.IsNullOrEmpty(Directive) ? SqlString.Null : Directive));
                Command.Parameters.Add(new SqlParameter("@amendment", string.IsNullOrEmpty(Amendment) ? SqlString.Null : Amendment));
                Command.Parameters.Add(new SqlParameter("@ad_date", AD_Date ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@efectivity", string.IsNullOrEmpty(Efectivity) ? SqlString.Null : Efectivity));
                Command.Parameters.Add(new SqlParameter("@servicebulletin", string.IsNullOrEmpty(ServiceBulletin) ? SqlString.Null : ServiceBulletin));
                Command.Parameters.Add(new SqlParameter("@review", string.IsNullOrEmpty(Review) ? SqlString.Null : Review));
                Command.Parameters.Add(new SqlParameter("@sb_date", SB_Date ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@threshold", string.IsNullOrEmpty(Threshold) ? SqlString.Null : Threshold));
                Command.Parameters.Add(new SqlParameter("@observaciones", string.IsNullOrEmpty(Observaciones) ? SqlString.Null : Observaciones));
                Command.Parameters.Add(new SqlParameter("@condition_from_hours", Condition_From_Hours ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@condition_to_hours", Condition_To_Hours ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@condition_from_cycles", Condition_From_Cycles ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@condition_to_cycles", Condition_To_Cycles ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@condition_from_date", Condition_From_Date ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@condition_to_date", Condition_To_Date ?? SqlDateTime.Null));
                Command.Parameters.Add(new SqlParameter("@usuario", Usuario));
                Command.Parameters.Add(new SqlParameter("@serie", Serie));
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
                            Cmnd = new SqlCommand("DELETE ComponenteMenorVinculado WHERE IdComponenteMenor = @idmenor;", Conexion);
                            Cmnd.Parameters.Add(new SqlParameter("@idmenor", Id));
                            var rDH = DataBase.Execute(Cmnd);
                            if (!rDH.Valid || !string.IsNullOrEmpty(rDH.Error)) {
                                res.Error = $"Error al limpiar el Historial: (CS.{this.GetType().Name}-Save.Err.03)<br>Error: {rDH.Error}";
                                return res;
                            }
                            var mdlsA = ComponenteMenorModelo.GetModelosMenor(Id);
                            foreach(var modelo in mdlsA) {
                                var idx = Modelos.FindIndex(m => { return m.Id == modelo.Id; });
								if (idx == -1)
                                    modelo.Delete();
							}
                            var limA = Limite.GetLimites(Id);
                            foreach(var limite in limA) {
                                var idx = Limites.FindIndex(l => { return l.Id == limite.Id; });
								if (idx == -1)
                                    limite.Delete();
							}
                        }
                        foreach (var mod in Modelos) {
                            mod.IdComponenteMenor = Id;
                            var rIC = mod.Save();
                            if (!rIC.Valid || !string.IsNullOrEmpty(rIC.Error)) {
                                res.Error = $"Error al Registrar el Modelo en el Componente: (CS.{this.GetType().Name}-Save.Err.04)<br>Error: {rIC.Error}";
                                return res;
                            }
                        }
						if (Limites!=null) {
                            foreach (var lim in Limites) {
                                lim.IdModelo = null;
                                lim.IdComponenteMayor = null;
                                lim.IdComponenteMenor = Id;
                                var rSL = lim.Save();
                                if (!rSL.Valid || !string.IsNullOrEmpty(rSL.Error)) {
                                    res.Error = $"Error al Guardar Limites: (CS.{this.GetType().Name}-Save.Err.05)<br>Error: {rSL.Error}";
                                    return res;
                                }
                            }
						}
                        if (Vinculados != null) {
                            foreach (var vinculado in Vinculados) {
                                vinculado.IdComponenteMenor = Id;
                                var rIC = vinculado.Save();
                                if (!rIC.Valid || !string.IsNullOrEmpty(rIC.Error)) {
                                    res.Error = $"Error al Registrar el Vinculado en el Componente: (CS.{this.GetType().Name}-Save.Err.06)<br>Error: {rIC.Error}";
                                    return res;
                                }
                            }
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
                if (!string.IsNullOrEmpty(Part))
                    res.Error += $"<br>Falta el Numero de Parte";
                if (!string.IsNullOrEmpty(Description))
                    res.Error += $"<br>Falta la Descripcion";
                if (IdFamily <= 0)
                    res.Error += $"<br>Falta la Familia";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("Componente NO se Elimino");
            using(TransactionScope scope = new TransactionScope()) {
                SqlCommand Command = new SqlCommand("DELETE ComponenteMenor WHERE Id = @idmenor", Conexion);
                Command.Parameters.Add(new SqlParameter("@idmenor", Id));
                var resD = DataBase.Execute(Command);
                if (!resD.Valid || resD.Afectados == 0) {
                    if (!string.IsNullOrEmpty(resD.Error)) {
                        res.Error = resD.Error;
                        return res;
                    }
                }
                //  Modelo
                Command = new SqlCommand("DELETE ComponenteMenorModelo WHERE IdComponenteMenor = @idmenor", Conexion);
                Command.Parameters.Add(new SqlParameter("@idmenor", Id));
                resD = DataBase.Execute(Command);
                if (!resD.Valid || resD.Afectados == 0) {
                    if (!string.IsNullOrEmpty(resD.Error)) {
                        res.Error = resD.Error;
                        return res;
                    }
                }
                //  Vinculado
                Command = new SqlCommand("DELETE ComponenteMenorVinculado WHERE IdComponenteMenor = @idmenor OR IdVinculado = @idmenor", Conexion);
                Command.Parameters.Add(new SqlParameter("@idmenor", Id));
                resD = DataBase.Execute(Command);
                if (!resD.Valid || resD.Afectados == 0) {
                    if (!string.IsNullOrEmpty(resD.Error)) {
                        res.Error = resD.Error;
                        return res;
                    }
                }
                //  Limite
                Command = new SqlCommand("DELETE Limite WHERE IdComponenteMenor = @idmenor", Conexion);
                Command.Parameters.Add(new SqlParameter("@idmenor", Id));
                resD = DataBase.Execute(Command);
                if (!resD.Valid || resD.Afectados == 0) {
                    if (!string.IsNullOrEmpty(resD.Error)) {
                        res.Error = resD.Error;
                        return res;
                    }
                }
                //  Tiempos
                Command = new SqlCommand("DELETE Tiempos WHERE IdItemMenor IN (SELECT Id FROM ItemMenor WHERE IdComponenteMenor = @idmenor)", Conexion);
                Command.Parameters.Add(new SqlParameter("@idmenor", Id));
                resD = DataBase.Execute(Command);
                if (!resD.Valid || resD.Afectados == 0) {
                    if (!string.IsNullOrEmpty(resD.Error)) {
                        res.Error = resD.Error;
                        return res;
                    }
                }
                scope.Complete();
                //  ItemMenor
                Command = new SqlCommand("DELETE ItemMenor WHERE IdComponenteMenor = @idmenor", Conexion);
                Command.Parameters.Add(new SqlParameter("@idmenor", Id));
                resD = DataBase.Execute(Command);
                if (!resD.Valid || resD.Afectados == 0) {
                    if (!string.IsNullOrEmpty(resD.Error)) {
                        res.Error = resD.Error;
                        return res;
                    }
                }
                //  Tareas
                //  Ordenes de Trabajo
                scope.Complete();
			}
			res.Valid = true;
			res.Error = "";
			res.Mensaje = "Eliminado Correctamente";
			Inicializar();
            return res;
        }
        private void SetDatos(SqlCommand Command) {
            RespuestaQuery res = DataBase.Query(Command);
            if (res.Valid) {
                var Registro = res.Row;
                Id = Registro.Id;
                Part = Registro.Part;
                Description = Registro.Description;
                IdFamily = Registro.IdFamily;
                IdTipoMenor = Registro.IdTipoMenor;
                ATA1 = Registro.ATA1;
                ATA2 = Registro.ATA2;
                ATA3 = Registro.ATA3;
                Directive = Registro.Directive;
                Amendment = Registro.Amendment;
                AD_Date = Registro.AD_Date;
                Efectivity = Registro.Efectivity;
                ServiceBulletin = Registro.ServiceBulletin;
                Review = Registro.Review;
                SB_Date = Registro.SB_Date;
                Threshold = Registro.Threshold;
                Observaciones = Registro.Observaciones;
                Condition_From_Hours = Registro.Condition_From_Hours;
                Condition_To_Hours = Registro.Condition_To_Hours;
                Condition_From_Cycles = Registro.Condition_From_Cycles;
                Condition_To_Cycles = Registro.Condition_To_Cycles;
                Condition_From_Date = Registro.Condition_From_Date;
                Condition_To_Date = Registro.Condition_To_Date;
                Usuario = Registro.Usuario;
                Serie = Registro.Serie;
                Valid = true;
                SetLimites();
                SetModelos();
                SetFamilia();
                SetVinculados();
            }
        }
        private void Inicializar() {
            Id = 0;
            Part = "";
            Description = "";
            IdFamily = 0;
            IdTipoMenor = 0;
            ATA1 = null;
            ATA2 = null;
            ATA3 = null;
            Directive = null;
            Amendment = null;
            AD_Date = null;
            Efectivity = null;
            ServiceBulletin = null;
            Review = null;
            SB_Date = null;
            Threshold = null;
            Observaciones = null;
            Condition_From_Hours = null;
            Condition_To_Hours = null;
            Condition_From_Cycles = null;
            Condition_To_Cycles = null;
            Condition_From_Date = null;
            Condition_To_Date = null;
            Usuario = 0;
            Serie = false;
            Valid = false;
            NoPadres = 0;
        }
        public void SetLimites() {
            Limites = Limite.GetLimites(Id);
            foreach(var lim in Limites) {
                lim.SetLimit();
			}
		}
        public void SetModelos() {
            Modelos = ComponenteMenorModelo.GetModelosMenor(Id);
            foreach (var mod in Modelos) {
                mod.SetMayor();
                mod.SetModelo();
            }
		}
        public void SetVinculados() {
            Vinculados = ComponenteMenorVinculado.GetComponenteMenorVinculados(Id);
		}
        public void SetFamilia() {
            Familia = new Family(IdFamily);
		}
		public static List<ComponenteMenor> GetComponentes(int idmayor, int? idModelo = null) {
			List<ComponenteMenor> componentemenors = new List<ComponenteMenor>();
			SqlCommand comando = new SqlCommand($"SELECT (SELECT COUNT(*) FROM ComponenteMenorVinculado WHERE IdVinculado = Id) AS Padres,* FROM ComponenteMenor WHERE Id IN (SELECT IdComponenteMenor FROM ComponenteMenorModelo WHERE  IdComponenteMayor = @idmayor{(idModelo != null ? " AND IdModelo = @idmodelo" : "")})", Conexion);
            comando.Parameters.Add(new SqlParameter("@idmayor", idmayor));
            comando.Parameters.Add(new SqlParameter("@idmodelo", idModelo));
			RespuestaQuery res = DataBase.Query(comando);
			foreach (var reg in res.Rows) {
				ComponenteMenor componentemenor = JsonConvert.DeserializeObject<ComponenteMenor>(JsonConvert.SerializeObject(reg));
				componentemenor.Valid = true;
                componentemenor.NoPadres = reg.Padres;
				componentemenors.Add(componentemenor);
			}
			return componentemenors;
        }
        public static List<ComponenteMenor> GetComponentesByModelo(int idModelo) {
            List<ComponenteMenor> componentemenors = new List<ComponenteMenor>();
            SqlCommand comando = new SqlCommand($"SELECT (SELECT COUNT(*) FROM ComponenteMenorVinculado WHERE IdVinculado = Id) AS Padres, * FROM ComponenteMenor WHERE Id IN (SELECT IdComponenteMenor FROM ComponenteMenorModelo WHERE IdModelo = @idmodelo)", Conexion);
            comando.Parameters.Add(new SqlParameter("@idmodelo", idModelo));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ComponenteMenor componentemenor = JsonConvert.DeserializeObject<ComponenteMenor>(JsonConvert.SerializeObject(reg));
                componentemenor.Valid = true;
                componentemenor.NoPadres = reg.Padres;
                componentemenors.Add(componentemenor);
            }
            return componentemenors;
        }
        public static List<ComponenteMenor> GetCompatibles(int idComponenteMenor) {
            
            List<ComponenteMenor> componentemenors = new List<ComponenteMenor>();
            //SqlCommand comando = new SqlCommand($"SELECT (SELECT COUNT(*) FROM ComponenteMenorVinculado WHERE IdVinculado = Id) AS Padres, IIF((SELECT COUNT(*) FROM ComponenteMenorVinculado WHERE IdComponenteMenor = @idComponenteMenor AND IdVinculado = Id) = 1,1,0) AS Vinculado, * FROM ComponenteMenor WHERE Id IN (SELECT DISTINCT IdComponenteMenor FROM ComponenteMenorModelo WHERE IdModelo IN (SELECT DISTINCT IdModelo FROM ComponenteMenorModelo WHERE IdComponenteMenor = @idComponenteMenor))", Conexion);
            SqlCommand comando = new SqlCommand($"SELECT (SELECT COUNT(*) FROM ComponenteMenorVinculado WHERE IdVinculado = Id) AS Padres, * FROM ComponenteMenor WHERE Id IN (SELECT DISTINCT IdComponenteMenor FROM ComponenteMenorModelo WHERE IdModelo IN (SELECT DISTINCT IdModelo FROM ComponenteMenorModelo WHERE IdComponenteMenor = @idComponenteMenor)) AND Id != @idComponenteMenor", Conexion);
            comando.Parameters.Add(new SqlParameter("@idComponenteMenor", idComponenteMenor));
            RespuestaQuery res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                dynamic componentemenor = JsonConvert.DeserializeObject<ComponenteMenor>(JsonConvert.SerializeObject(reg));
                componentemenor.Valid = true;
                componentemenor.NoPadres = reg.Padres;
                //componentemenor.Vinculado = reg.Vinculado;
                componentemenors.Add(componentemenor);
            }
            return componentemenors;
        }
        public static (string PNAD, string PNSB) GenerarPartNumber(string cadena) {
            string npnD = "";
            string npnS = "";
            SqlCommand comando = new SqlCommand("SELECT ISNULL(COUNT(Id),0)+1 AS Contador FROM ComponenteMenor WHERE Directive = @cadena", Conexion);
            comando.Parameters.Add(new SqlParameter("@cadena", cadena));
            var res = DataBase.Query(comando);
			if (res.Valid) {
                npnD = $"{cadena}-{res.Row.Contador.ToString().PadLeft(3, '0')}";
            }
            comando = new SqlCommand("SELECT ISNULL(COUNT(Id),0)+1 AS Contador FROM ComponenteMenor WHERE ServiceBulletin = @cadena", Conexion);
            comando.Parameters.Add(new SqlParameter("@cadena", cadena));
            res = DataBase.Query(comando);
            if (res.Valid) {
                npnS = $"{cadena}-{res.Row.Contador.ToString().PadLeft(3, '0')}";
            }
            return (npnD,npnS);
		}
        public static dynamic queryReport(int idMayor, int idFamilia, int idModelo, int idTipo, int ata1) {
            string sqlQuery = $"SELECT *, (SELECT Codigo FROM Family WHERE Id = IdFamily) AS Familia FROM ComponenteMenor WHERE";
            sqlQuery += idMayor > 0 ? $" Id IN (SELECT DISTINCT IdComponenteMenor FROM ComponenteMenorModelo WHERE IdComponenteMayor = @idMayor) AND" : "";
            sqlQuery += idFamilia > 0 ? $" IdFamily = @idFamilia AND" : "";
            sqlQuery += idModelo > 0 ? $" Id IN (SELECT DISTINCT IdComponenteMenor FROM ComponenteMenorModelo WHERE IdModelo = @idModelo) AND" : "";
            sqlQuery += idTipo > 0 ? $" IdTipoMenor = @idTipo AND" : "";
            sqlQuery += ata1 > 0 ? $" ATA1 = @ata1 AND" : "";
            sqlQuery = sqlQuery.Substring(sqlQuery.Length - 3) == "ERE" ? sqlQuery.Substring(0, sqlQuery.Length - 6) : sqlQuery.Substring(0, sqlQuery.Length - 4);
            SqlCommand comando = new SqlCommand(sqlQuery, Conexion);
            comando.Parameters.AddWithValue("@idMayor", idMayor);
            comando.Parameters.AddWithValue("@idFamilia", idFamilia);
            comando.Parameters.AddWithValue("@idModelo", idModelo);
            comando.Parameters.AddWithValue("@idTipo", idTipo);
            comando.Parameters.AddWithValue("@ata1", ata1);
            var res = DataBase.Query(comando);
            return res.Rows;
		}
        public static List<ComponenteMenor> GetVinculados(int idComponenteMenor) {
            List<ComponenteMenor> hijos = new List<ComponenteMenor>();
            SqlCommand comando = new SqlCommand("SELECT (SELECT COUNT(*) FROM ComponenteMenorVinculado WHERE IdVinculado = Id) AS Padres,* FROM ComponenteMenor WHERE Id IN (SELECT IdVinculado FROM ComponenteMenorVinculado WHERE IdComponenteMenor = @idComponenteMenor)", Conexion);
            comando.Parameters.AddWithValue("@idComponenteMenor", idComponenteMenor);
            var res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                ComponenteMenor componentemenor = JsonConvert.DeserializeObject<ComponenteMenor>(JsonConvert.SerializeObject(reg));
                componentemenor.Valid = true;
                componentemenor.NoPadres = reg.Padres;
                hijos.Add(componentemenor);
            }
            return hijos;
		}
    }
}