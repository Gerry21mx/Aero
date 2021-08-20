
using ATSM.Operaciones;
using ATSM.Seguimiento;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace ATSM.Areas.Operaciones.Controllers.api.Vuelos {
	public class Cotizacion {
		private static SqlConnection Conexion = DataBase.Conexion();

        public int Id { get; set; }
		public int IdCliente { get; set; }
		public int IdContactoCliente { get; set; }
		public int IdOrigen { get; set; }
		public int IdDestino { get; set; }
		public int Bultos { get; set; }
		public int Largo { get; set; }
		public int Alto { get; set; }
		public int Ancho { get; set; }
		public string UMEDimensiones { get; set; }
		public int Peso { get; set; }
		public string UMEPeso { get; set; }
		public int TipoServicio { get; set; }
		public bool Hazmat { get; set; }
		public bool Despaletizable { get; set; }
		public bool Valid { get; set; }
		public int Usuario { get; set; }
		public DateTime FechaSolicitud { get; set; }
        public DateTime FechaAceptacion { get; set; }
        public DateTime FechaRechazo { get; set; }
        public string Observacion { get; set; }
		public Cliente Cliente { get; set; }
		public ClienteContacto Contacto { get; set; }
		public Aeropuerto Origen { get; set; }
		public Aeropuerto Destino { get; set; }

        public Cotizacion(int? id = null)
        {
            Inicializar();
            if (id > 0)
            {
                SqlCommand comando = new SqlCommand($"SELECT * FROM Cotizacion WHERE Id = @id", Conexion);
                comando.Parameters.Add(new SqlParameter("@id", id));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public Cotizacion(int id = 0, int idCliente = 0, int idContactoCliente = 0, int idOrigen = 0, int idDestino = 0, int bultos = 0, int largo = 0, int alto = 0, int ancho = 0,
                            string uMEDimensiones = "", int peso = 0, string uMEPeso = "", int tipoServicio = 0, bool hazmat = false, bool despaletizable = false, 
                            bool valid = false, int usuario = 0, string observacion = "", DateTime fechaSolicitud = default(DateTime), DateTime fechaAceptacion = default(DateTime), 
                            DateTime fechaRechazo = default(DateTime))
        {
            Id = id;
            IdCliente = idCliente;
            IdContactoCliente = idContactoCliente;
            IdOrigen = idOrigen;
            IdDestino = idDestino;
            Bultos = bultos;
            Largo = largo;
            Alto = alto;
            Ancho = ancho;
            UMEDimensiones = uMEDimensiones;
            Peso = peso;
            UMEPeso = uMEPeso;
            TipoServicio = tipoServicio;
            Hazmat = hazmat;
            Despaletizable = despaletizable;
            Valid = valid;
            Usuario = usuario;
            FechaSolicitud = fechaSolicitud;
            FechaAceptacion = fechaAceptacion;
            FechaRechazo = fechaRechazo;
            Observacion = observacion;
        }
        public Respuesta Save()
        {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdCliente != 0 )
            {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT Id FROM Cotizacion WHERE Id = @id", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@id", Id));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "Cotizacion ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid)
                {
                    SqlStr = @"UPDATE Cotizacion SET IdCliente = @idcliente, IdContactoCliente = @idcontactocliente, IdOrigen = @idorigen, IdDestino = @iddestino, Bultos = @bultos, Largo = @largo, Alto = @alto, Ancho = @ancho, UMEDimensiones = @umedimensiones, Peso = @peso, UMEPeso = @umepeso, TipoServicio = @tiposervicio, Hazmat = @hazmat, Despaletizable = @despaletizable, Usuario = @usuario, Fecha = @fecha WHERE Id = @id";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else
                {
                    if (!string.IsNullOrEmpty(existe.Error))
                    {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO Cotizacion(IdCliente, IdContactoCliente, IdOrigen, IdDestino, Bultos, Largo, Alto, Ancho, UMEDimensiones, Peso, UMEPeso, TipoServicio, Hazmat, Despaletizable, Usuario, Fecha) VALUES(@idcliente, @idcontactocliente, @idorigen, @iddestino, @bultos, @largo, @alto, @ancho, @umedimensiones, @peso, @umepeso, @tiposervicio, @hazmat, @despaletizable, @usuario, @fecha)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@id", Id));
                Command.Parameters.Add(new SqlParameter("@idcliente", IdCliente));
                Command.Parameters.Add(new SqlParameter("@idcontactocliente", IdContactoCliente));
                Command.Parameters.Add(new SqlParameter("@idorigen", IdOrigen));
                Command.Parameters.Add(new SqlParameter("@iddestino", IdDestino));
                Command.Parameters.Add(new SqlParameter("@bultos", Bultos));
                Command.Parameters.Add(new SqlParameter("@largo", Largo));
                Command.Parameters.Add(new SqlParameter("@alto", Alto));
                Command.Parameters.Add(new SqlParameter("@ancho", Ancho));
                Command.Parameters.Add(new SqlParameter("@umedimensiones", string.IsNullOrEmpty(UMEDimensiones) ? SqlString.Null : UMEDimensiones));
                Command.Parameters.Add(new SqlParameter("@peso", Peso));
                Command.Parameters.Add(new SqlParameter("@umepeso", string.IsNullOrEmpty(UMEPeso) ? SqlString.Null : UMEPeso));
                Command.Parameters.Add(new SqlParameter("@tiposervicio", TipoServicio));
                Command.Parameters.Add(new SqlParameter("@hazmat", Hazmat));
                Command.Parameters.Add(new SqlParameter("@despaletizable", Despaletizable));
                Command.Parameters.Add(new SqlParameter("@usuario", Usuario));
                Command.Parameters.Add(new SqlParameter("@fechaSolicitud", FechaSolicitud));
                Command.Parameters.Add(new SqlParameter("@fechaAceptacion", FechaAceptacion));
                Command.Parameters.Add(new SqlParameter("@fechaRechazo", FechaRechazo));
                Command.Parameters.Add(new SqlParameter("@observacion", Observacion));
                RespuestaQuery rInUp = DataBase.Insert(Command);
                if (rInUp.Valid)
                {
                    if (Insr)
                    {
                        if (rInUp.IdRegistro == 0)
                        {
                            res.Error = $"No se pudo obtener el Id Insertado(CS.{this.GetType().Name}-Save.Err.03)<br>{SqlStr}<br> Error: {rInUp.Error}";
                            return res;
                        }
                        Id = rInUp.IdRegistro;
                        Valid = true;
                    }
                }
                else
                {
                    res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                    return res;
                }
                res.Elemento = this;
                res.Valid = true;
            }
            else
            {
                if (IdCliente < 0 )
                    res.Error += $"<br>Falta el Valor de Cliente";
            }
            return res;
        }
        public Respuesta Delete()
        {
            Respuesta res = new Respuesta("Cotizacion NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE Cotizacion WHERE Id = @id", Conexion);
            Command.Parameters.Add(new SqlParameter("@id", Id));
            var resD = DataBase.Execute(Command);
            if (resD.Valid && resD.Afectados > 0)
            {
                res.Valid = true;
                res.Error = "";
                res.Mensaje = "Eliminado Correctamente";
                Inicializar();
            }
            else
            {
                if (!string.IsNullOrEmpty(resD.Error))
                {
                    res.Error = resD.Error;
                }
                else
                {
                    res.Mensaje = "No se encontraron coincidencias para elminar.";
                }
            }
            return res;
        }
        private void SetDatos(SqlCommand Command)
        {
            RespuestaQuery res = DataBase.Query(Command);
            if (res.Valid)
            {
                var Registro = res.Row;
                Id = Registro.Id;
                IdCliente = Registro.IdCliente;
                IdContactoCliente = Registro.IdContactoCliente;
                IdOrigen = Registro.IdOrigen;
                IdDestino = Registro.IdDestino;
                Bultos = Registro.Bultos;
                Largo = Registro.Largo;
                Alto = Registro.Alto;
                Ancho = Registro.Ancho;
                UMEDimensiones = Registro.UMEDimensiones;
                Peso = Registro.Peso;
                UMEPeso = Registro.UMEPeso;
                TipoServicio = Registro.TipoServicio;
                Hazmat = Registro.Hazmat;
                Despaletizable = Registro.Despaletizable;
                Usuario = Registro.Usuario;
                FechaSolicitud = Registro.FechaSolicutud;
                FechaAceptacion = Registro.FechaAceptacion;
                FechaRechazo = Registro.FechaRechazo;
                Observacion = Registro.Observacion;
                Valid = true;
            }
        }
        private void Inicializar()
        {
            Id = 0;
            IdCliente = 0;
            IdContactoCliente = 0;
            IdOrigen = 0;
            IdDestino = 0;
            Bultos = 0;
            Largo = 0;
            Alto = 0;
            Ancho = 0;
            UMEDimensiones = "";
            Peso = 0;
            UMEPeso = "";
            TipoServicio = 0;
            Hazmat = false;
            Despaletizable = false;
            Valid = false;
            Usuario = 0;
            FechaSolicitud = default(DateTime);
            FechaAceptacion = default(DateTime);
            FechaRechazo = default(DateTime);
            Observacion = "";
        }
        public static List<Cotizacion> GetCotizacions()
        {
            List<Cotizacion> cotizacions = new List<Cotizacion>();
            RespuestaQuery res = DataBase.Query(new SqlCommand("SELECT * FROM Cotizacion", Conexion));
            foreach (var reg in res.Rows)
            {
                Cotizacion cotizacion = JsonConvert.DeserializeObject<Cotizacion>(JsonConvert.SerializeObject(reg));
                cotizacion.Valid = true;
                cotizacions.Add(cotizacion);
            }
            return cotizacions;
        }

    }

}