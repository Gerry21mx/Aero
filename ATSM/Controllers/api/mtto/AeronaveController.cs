using System.Web.Http;

using ATSM.Ingenieria;

namespace ATSM.Controllers.api.mtto {
	public class AeronaveController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();
		// GET api/<controller>
		public Answer Get() {
			answer.Data = Aircraft.GetAircrafts();
			return answer;
		}

		// GET api/<controller>/5
		public Answer Get(int id) {
			Aircraft avi = new Aircraft(id);
			answer.Data = avi;
			return answer;
		}

		// GET api/<controller>/<matricula>/ByMatricula
		[Route("api/Aeronave/{matricula}/ByMatricula")]
		public Answer GetByMatricula(string matricula) {
			answer.Data = new Aircraft(matricula);
			return answer;
		}

		// GET api/<controller>/UltimoTramo
		[Route("api/Aeronave/UltimoTramo")]
		public Answer GetUltimoTramo(int idAeronave) {
			Aircraft avi = new Aircraft(idAeronave);
			if (avi.Valid) {
				answer.Data = avi.UltimoTramo();
			} else {
				answer.Message = $"La Aeronave No Existe, por favor revise la Informacion. {idAeronave}";
			}
			return answer;
		}

		// GET api/<controller>/Tripulacion
		[Route("api/Aeronave/Tripulacion")]
		public Answer GetTripulacion(int idaeronave) {
			var avi= new Aircraft(idaeronave);
			var tripulacion = avi.GetTripulacion();
			answer.Data = new { Capitanes = tripulacion.Capitanes, Copilotos = tripulacion.Copilotos };
			return answer;
		}
	}
}
