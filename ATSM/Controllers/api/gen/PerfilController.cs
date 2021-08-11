using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Controllers.api.gen
{
    public class PerfilController : ApiController
    {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public Answer Get(string area) {
			answer = Funciones.VRoles("Perfil");
			if (answer.Status) {
				answer.Data = Perfil.GetPerfiles(area);
			}
			return answer;
		}

		// GET api/<controller>/5
		public Answer Get(int id) {
			answer = Funciones.VRoles("Perfil");
			if (answer.Status) {
				answer.Data = new Perfil(id);
			}
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(Perfil iClase) {
			answer = Funciones.VRoles("cPerfil");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Perfil iClase) {
			answer = Funciones.VRoles("dPerfil");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// GET api/<controller>/<nombre>/ByNombre
		[Route("api/Perfil/{nombre}/ByNombre")]
		public Answer GetByNombre(string Nombre) {
			answer = Funciones.VRoles("Perfil");
			if (answer.Status) {
				answer.Data = new Perfil(Nombre);
			}
			return answer;
		}
	}
}
