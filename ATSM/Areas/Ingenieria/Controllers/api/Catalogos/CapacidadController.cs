using ATSM.Ingenieria;

using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api {
	public class CapacidadController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            answer.Data = Capacidad.GetCapacidades();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Capacidad(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/Capacidad/ByCadena")]
        public Answer GetByCadena(string cadena) {
            answer.Data = new Capacidad(cadena);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Capacidad iClase) {
            answer = Funciones.VRoles("cCapacidad");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Capacidad iClase) {
            answer = Funciones.VRoles("dCapacidad");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
