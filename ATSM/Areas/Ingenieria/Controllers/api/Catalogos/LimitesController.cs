
using ATSM.Ingenieria;

using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api {
	public class LimitesController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            answer.Data = Limites.GetLimites();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Limites(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/Limites/ByCadena")]
        public Answer Get(string cadena) {
            answer.Data = new Limites(cadena);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Limites iClase) {
            answer = Funciones.VRoles("cLimites");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Limites iClase) {
            answer = Funciones.VRoles("dLimites");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
