using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using ATSM.Ingenieria;

using Newtonsoft.Json;

namespace ATSM.Tripulaciones {
	public class Crew {
		private static SqlConnection Conexion = DataBase.Conexion("BD_ASTRIP");
		public int IdCrew { get; set; }
		public string Nombre { get; set; }
		public string Correo { get; set; }
		public string Telefono { get; set; }
        public string Licencia { get; set; }
        public DateTime? VencimientoLicencia { get; set; }
        public string Examen { get; set; }
        public DateTime? VencimientoExamen { get; set; }
        public string Visa { get; set; }
        public DateTime? VencimientoVisa { get; set; }
        public string Pasaporte { get; set; }
        public DateTime? VencimientoPasaporte { get; set; }

        public string cap_1 { get; set; }
        public string cap_2 { get; set; }
        public string cap_3 { get; set; }
		public int IdCapacidad_1 { get; set; }
		public int IdCapacidad_2 { get; set; }
		public int IdCapacidad_3 { get; set; }
		public int Nivel_1 { get; set; }
		public int Nivel_2 { get; set; }
		public int Nivel_3 { get; set; }
        public TimeSpan Dia { get; set; }
        public TimeSpan Semana { get; set; }
        public TimeSpan HorasMes { get; set; }
        public bool Activo { get; set; }
		public bool Valid { get; set; }
        public Capacidad Capacidad1 { get; set; }
        public Capacidad Capacidad2 { get; set; }
        public Capacidad Capacidad3 { get; set; }
        public Crew(int? id = null) {
            Inicializar();
            if (id > 0) {
                SqlCommand comando = new SqlCommand($"SELECT *,(SELECT Nombre_Personal+' '+Apellidos_Personal FROM GTSM.dbo.Table_Usuarioss WHERE Id_Personal = id) AS Nombre FROM pilco WHERE id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Crew(int id, string nombre, string correo = "", string telefono = "", string lic = "", DateTime? vlic = null, string exm = "", DateTime? vexm = null, string vis = "", DateTime? vvis = null, string pas = "", DateTime? vpas = null, string c1 = "", string c2 = "", string c3 = "", int idCapacidad_1 = 0, int idCapacidad_2 = 0, int idCapacidad_3 = 0, int nivel_1 = 0, int nivel_2 = 0, int nivel_3 = 0, bool act = false, bool valid = false) {
            this.IdCrew = id;
            this.Nombre = nombre;
            this.Correo = correo;
            this.Telefono = telefono;
            this.Licencia = lic;
            this.VencimientoLicencia = vlic;
            this.Examen = exm;
            this.VencimientoExamen = vexm;
            this.Visa = vis;
            this.VencimientoVisa = vvis;
            this.Pasaporte = pas;
            this.VencimientoPasaporte = vpas;
            this.cap_1 = c1;
            this.cap_2 = c2;
            this.cap_3 = c3;
            this.Activo = act;
            this.Valid = valid;
        }
        private void SetDatos(SqlCommand Command) {
            RespuestaQuery res = DataBase.Query(Command);
            if (res.Valid) {
                var Registro = res.Row;
                IdCrew = Registro.id;
                Nombre = Registro.Nombre;
                //Correo = Registro.Correo;
                //Telefono = Registro.Telefono;
                Licencia = Registro.lic;
                VencimientoLicencia = Registro.vlic;
                Examen = Registro.exm;
                VencimientoExamen = Registro.vexm;
                Visa = Registro.vis;
                VencimientoVisa = Registro.vvis;
                Pasaporte = Registro.pas;
                VencimientoPasaporte = Registro.vpas;
                cap_1 = Registro.c1;
                cap_2 = Registro.c2;
                cap_3 = Registro.c3;
                Activo = Registro.act;
                Valid = true;
                SetCapacidades();
            }
        }
        public string GetJSON() {
            return JsonConvert.SerializeObject(this);
        }
        private void Inicializar() {
            IdCrew = 0;
            Nombre = "";
            Correo = "";
            Telefono = "";
            Licencia = "";
            VencimientoLicencia = null;
            Examen = "";
            VencimientoExamen = null;
            Visa = "";
            VencimientoVisa = null;
            Pasaporte = "";
            VencimientoPasaporte = null;
            cap_1 = "";
            cap_2 = "";
            cap_3 = "";
            IdCapacidad_1 = 0;
            IdCapacidad_2 = 0;
            IdCapacidad_3 = 0;
            Nivel_1 = 0;
            Nivel_2 = 0;
            Nivel_3 = 0;
            Activo = false;
            Valid = false;
        }
        public static List<Crew> GetCrews(bool? activo = null) {
            List<Crew> crews = new List<Crew>();
            RespuestaQuery res = DataBase.Query(new SqlCommand($"SELECT *,(SELECT Nombre_Personal+' '+Apellidos_Personal FROM GTSM.dbo.Table_Usuarioss WHERE Id_Personal = id) AS Nombre FROM pilco{(activo==true?" WHERE act = 1":"")}", Conexion));
            foreach (var reg in res.Rows) {
                Crew crew = JsonConvert.DeserializeObject<Crew>(JsonConvert.SerializeObject(reg));
                crew.Valid = true;
                crew.SetCapacidades();
                crews.Add(crew);
            }
            return crews;
        }
        public void SetCapacidades() {
            if (!string.IsNullOrEmpty(cap_1)) {
                var def = cap_1.Split('_');
                Nivel_1 = def[1] == "p" ? 1 : 2;
                Capacidad1 = new Capacidad(Capacidad.Converter(def[0]));
                IdCapacidad_1 = Capacidad1.Id;
            }
            if (!string.IsNullOrEmpty(cap_2)) {
                var def = cap_2.Split('_');
                Nivel_2 = def[1] == "p" ? 1 : 2;
                Capacidad2 = new Capacidad(Capacidad.Converter(def[0]));
                IdCapacidad_2 = Capacidad2.Id;
            }
            if (!string.IsNullOrEmpty(cap_3)) {
                var def = cap_3.Split('_');
                Nivel_3 = def[1] == "p" ? 1 : 2;
                Capacidad3 = new Capacidad(Capacidad.Converter(def[0]));
                IdCapacidad_3 = Capacidad3.Id;
            }
        }
        public static List<Crew> GetCrew(int? idCapacidad = null, string capacidad = null, int? nivel=null) {
            //string capa = $"{capacidad.ToLower()}{(nivel == 1 ? "_c" : "")}";
            Capacidad cap = new Capacidad();
            if (!string.IsNullOrEmpty(capacidad)) {
                string capa = $"{capacidad.ToUpper()}";
                cap = new Capacidad(capacidad);
            }
            if (idCapacidad > 0) {
                cap = new Capacidad(idCapacidad);
            }
            string cd = cap.Nombre.ToLower().Trim();
            List<Crew> Crews = new List<Crew>();
            string sqlQuery = $"SELECT * FROM BD_ASTRIP.dbo.TripulacionActiva {(!string.IsNullOrEmpty(cd) ? $"WHERE (c1 LIKE '{cd}%' OR c2 LIKE '{cd}%' OR c3 LIKE '{cd}%')" : "")}";
            SqlCommand comando = new SqlCommand(sqlQuery, Conexion);
            var res = DataBase.Query(comando);
            foreach (var reg in res.Rows) {
                var json = JsonConvert.SerializeObject(reg);
                Crew crew = JsonConvert.DeserializeObject<Crew>(json);
                crew.Valid = true;
                crew.SetCapacidades();
                Crews.Add(crew);
            }
            return Crews;
        }
        public void SetHorasVuelo() {
            SqlCommand comando = new SqlCommand($"SELECT Salida, Llegada, SalidaPlataforma, LlegadaPlataforma, Despegue, Aterrizaje FROM VueloTramo WHERE Salida IS NOT NULL AND Llegada IS NOT NULL AND Despegue IS NOT NULL AND Aterrizaje IS NOT NULL AND IdCapitan = @idcrew OR IdCopiloto = @idcrew", Conexion);
            comando.Parameters.Add(new SqlParameter("@idcrew", IdCrew));
            var res = DataBase.Query(comando);
            Dia = new TimeSpan(0);
            Semana = new TimeSpan(0);
            HorasMes = new TimeSpan(0);
            DateTime d = DateTime.Now;
            DateTime s = DateTime.Now.AddDays(-7);
            DateTime m = DateTime.Now.AddMonths(-1);
            foreach (var vuelo in res.Rows) {
                DateTime des = vuelo.Salida + vuelo.Despegue;
                DateTime ate = vuelo.Llegada + vuelo.Aterrizaje;
                if (vuelo.Salida == d) {
                    Dia.Add(ate - des);
                }
                if (vuelo.Salida >= s) {
                    Semana.Add(ate - des);
                }
                if (vuelo.Salida >= m) {
                    HorasMes.Add(ate - des);
                }
            }
        }
    }
}