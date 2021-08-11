using System;
using System.Web.Http;

using ATSM.Tripulaciones;

namespace ATSM.Controllers.api.Tripulaciones {
	public class CrewController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Crew(id);
			return answer;
		}
		
		// GET api/<controller>
		[Route("api/Crew/")]
		public Answer Post(dynamic datos) {
			int idCap = 0;
			string cap = "";
			try {
				idCap = (int)datos.idCapacidad;
				cap = (string)datos.capacidad;
			}
			catch (Exception ex) {
				answer.Message = ex.Message;
			}
			answer.Data = Crew.GetCrew(idCap, cap);
			return answer;
		}
	}
}
