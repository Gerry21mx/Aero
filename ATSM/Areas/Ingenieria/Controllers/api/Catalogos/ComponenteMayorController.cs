using ATSM.Ingenieria;

using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api {
	public class ComponenteMayorController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            var mayores = ComponenteMayor.GetComponentesMayores();
            foreach(var com in mayores) {
                com.GetCapacidades();
                com.Capacidades.ForEach(cap => { cap.SetCapacidad(); });
                com.GetLimites();
			}
            answer.Data = mayores;
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new ComponenteMayor(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/ComponenteMayor/ByCadena")]
        public Answer Get(string cadena) {
            answer.Data = new ComponenteMayor(cadena);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(ComponenteMayor iClase) {
            answer = Funciones.VRoles("cComponenteMayor");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(ComponenteMayor iClase) {
            answer = Funciones.VRoles("dComponenteMayor");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
