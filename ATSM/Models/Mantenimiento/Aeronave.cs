using ATSM.Seguimiento;
using ATSM.Tripulaciones;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net;
using System.Threading.Tasks;
using WebMatrix.WebData;

namespace ATSM.Mantenimiento {
	/// <summary>
	/// Aeronave de Flota
	/// </summary>
	public class Aeronave {
		private static SqlConnection Conexion = DataBase.Conexion("BD_MTTO");
		public int IdAeronave { get; private set; }
		public string Matricula { get; set; }
		public int? IdModelo { get; set; }
		public int? IdEmpresa { get; set; }
		public string Serie { get; set; }
		public string Fabricante { get; set; }
		public int? PesoMaximo { get; set; }
		public int? JumpSeat { get; set; }
		public int? Anio { get; set; }
		public int? Pasajeros { get; set; }
		public string Seguro { get; set; }
		public bool Estado { get; set; }
		public decimal? TIN { get; set; }
		public int? CIN { get; set; }
		public decimal? TSN { get; set; }
		public int? CSN { get; set; }
		public ModeloAeronave Modelo { get; set; }
		public Empresa Empresa { get; set; }
		public bool Valid { get; set; }
		public Aeronave(int? idaeronave = null) {
			Inicializar();
			if (idaeronave > 0) {
				SqlCommand Cmnd = new SqlCommand($"SELECT * FROM Aeronave WHERE IdAeronave = @idaeronave", Conexion);
				Cmnd.Parameters.Add(new SqlParameter("@idaeronave", idaeronave));
				SetDatos(Cmnd);
			}
		}
		public Aeronave(string matricula) {
			Inicializar();
			if(!string.IsNullOrEmpty(matricula)) {
				SqlCommand Comando = new SqlCommand("SELECT * FROM Aeronave WHERE Matricula = @mat", Conexion);
				Comando.Parameters.Add(new SqlParameter("@mat", matricula));
				SetDatos(Comando);
			}
		}
		[JsonConstructor]
		public Aeronave(int idAeronave, string matricula, int? idModelo = null, int? idEmpresa = null, string serie = null, string fabricante = null, int? pesoMaximo = null, int? jumpSeat = null, int? anio = null, int? pasajeros = null, string seguro = null, bool estado = false, decimal? tin = null, int? cin = null, decimal? tsn = null, int? csn = null) {
			IdAeronave = idAeronave;
			Matricula = matricula;
			IdModelo = idModelo ?? 0;
			IdEmpresa = idEmpresa;
			Serie = serie;
			Fabricante = fabricante;
			PesoMaximo = pesoMaximo;
			JumpSeat = jumpSeat;
			Anio = anio;
			Pasajeros = pasajeros;
			Seguro = seguro;
			Estado = estado;
			TSN = tsn;
			CSN = csn;
			TIN = tin;
			CIN = cin;
			Valid = false;
		}
		private void SetDatos(SqlCommand Command) {
			RespuestaQuery res = DataBase.Query(Command);
			if(res.Valid) {
				var Registro = res.Row;
				IdAeronave = Registro.IdAeronave;
				Matricula = Registro.Matricula;
				IdModelo = Registro.IdModelo;
				IdEmpresa = Registro.IdEmpresa;
				Serie = Registro.Serie;
				Fabricante = Registro.Fabricante;
				PesoMaximo = Registro.PesoMaximo;
				JumpSeat = Registro.JumpSeat;
				Anio = Registro.Anio;
				Pasajeros = Registro.Pasajeros;
				Seguro = Registro.Seguro;
				Estado = Registro.Estado;
				TIN = Registro.TIN;
				CIN = Registro.CIN;
				TSN = Registro.TSN ?? 0;
				CSN = Registro.CSN ?? 0;
				Valid = true;
				GetEmpresa();
				GetModelo();
			}
		}
		public string GetJSON() {
			return JsonConvert.SerializeObject(this);
		}
		public void Inicializar() {
			Matricula = "";
			IdModelo = null;
			IdEmpresa = 0;
			Serie = "";
			Fabricante = "";
			PesoMaximo = 0;
			JumpSeat = 0;
			Anio = 0;
			Pasajeros = 0;
			Seguro = "";
			Estado = false;
			TIN = 0;
			CIN = 0;
			TSN = 0;
			CSN = 0;
			Valid = false;
		}
		public static List<Aeronave> GetAeronaves() {
			Usuario usuario = new Usuario(WebSecurity.CurrentUserId);
			List<Aeronave> aeronaves = new List<Aeronave>();
			SqlCommand comando = new SqlCommand($"SELECT * FROM Aeronave {(usuario.IdEmpresa > 0 ? $"WHERE empresa = {usuario.IdEmpresa}" : "")} ORDER BY IdModelo, Matricula", Conexion);
			var res = DataBase.Query(comando);
			foreach(var avi in res.Rows) {
				var oJson = JsonConvert.SerializeObject(avi);
				Aeronave avion = JsonConvert.DeserializeObject<Aeronave>(oJson);
				avion.Valid = true;
				aeronaves.Add(avion);
			}
			return aeronaves;
		}
		public void GetEmpresa() {
			Empresa = new Empresa(IdEmpresa);
		}
		public void GetModelo() {
			Modelo = new ModeloAeronave(IdModelo);
		}
		public Vuelo PrimerVuelo() {
			Vuelo vuelo = new Vuelo();
			SqlCommand comando = new SqlCommand("SELECT TOP 1 IdVuelo FROM VueloTramo WHERE IdAeronave = @ida ORDER BY Salida", DataBase.Conexion("Seguimiento"));
			comando.Parameters.Add(new SqlParameter("@ida", IdAeronave));
			var rQuery = DataBase.Query(comando);
			if (rQuery.Valid) {
				vuelo = new Vuelo(rQuery.Row.IdVuelo);
			}
			return vuelo;
		}
		public Vuelo UltimoVuelo() {
			Vuelo vuelo = new Vuelo();
			SqlCommand comando = new SqlCommand("SELECT TOP 1 IdVuelo FROM VueloTramo WHERE IdAeronave = @ida ORDER BY Salida DESC", DataBase.Conexion("Seguimiento"));
			comando.Parameters.Add(new SqlParameter("@ida", IdAeronave));
			var rQuery = DataBase.Query(comando);
			if (rQuery.Valid) {
				vuelo = new Vuelo(rQuery.Row.IdVuelo);
			}
			return vuelo;
		}
		public VueloTramo UltimoTramo() {
			VueloTramo tramo = new VueloTramo(0);
			SqlCommand comando = new SqlCommand("SELECT TOP 1 * FROM VueloTramo WHERE IdAeronave=@ida ORDER BY dbo.Date_Time2DateTime2(Salida,Despegue,0) DESC, IdVuelo DESC, Pierna DESC", DataBase.Conexion());
			comando.Parameters.Add(new SqlParameter("@ida", IdAeronave));
			var rQuery = DataBase.Query(comando);
			if (rQuery.Valid) {
				tramo = JsonConvert.DeserializeObject<VueloTramo>(JsonConvert.SerializeObject(rQuery.Row));
				tramo.GetAeronave();
				tramo.GetOrigen();
				tramo.GetDestino();
				tramo.GetTripulacion();
				tramo.Valid = true;
			}
			return tramo;
		}
		public (List<Crew> Capitanes, List<Crew> Copilotos) GetTripulacion() {
			List<Crew> Capitanes = new List<Crew>();
			List<Crew> Copilotos = new List<Crew>();
			if (Modelo == null) {
				GetModelo();
			}
			if (Modelo.Capacidad==null) {
				return new(Capitanes, Copilotos);
			}
			var crews= Crew.GetCrew(Modelo.Capacidad.IdCapacidad);
			foreach (var crew in crews) {
				if (crew.IdCapacidad_1 == Modelo.Capacidad.IdCapacidad) {
					if (crew.Nivel_1 == 1) {
						Capitanes.Add(crew);
					}
					else {
						Copilotos.Add(crew);
					}
				}
				else if (crew.IdCapacidad_2 == Modelo.Capacidad.IdCapacidad) {
					if (crew.Nivel_2 == 1) {
						Capitanes.Add(crew);
					}
					else {
						Copilotos.Add(crew);
					}
				}
				else if (crew.IdCapacidad_3 == Modelo.Capacidad.IdCapacidad) {
					if (crew.Nivel_3 == 1) {
						Capitanes.Add(crew);
					}
					else {
						Copilotos.Add(crew);
					}
				}
			}
			return new(Capitanes, Copilotos);
		}
	}
}