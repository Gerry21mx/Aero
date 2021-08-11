using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class FamilyController : ApiController {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();
		// GET api/<controller>
		public Answer Get() {
			answer.Data = Family.GetFamilys();
			return answer;
		}
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Family(id);
			return answer;
		}

		// GET api/<controller>/ByCadena
		[Route("api/Family/ByCadena")]
		public Answer Get(string cadena) {
			answer.Data = new Family(cadena);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(Family iClase) {
			answer = Funciones.VRoles("cFamily");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Family iClase) {
			answer = Funciones.VRoles("dFamily");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
