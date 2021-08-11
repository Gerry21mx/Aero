using ATSM.Ingenieria;

using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api {
	public class ModeloController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            answer.Data = Modelo.GetModelos();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Modelo(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/Modelo/ByCadena")]
        public Answer Get(string cadena, int idComponenteMayor) {
            answer.Data = new Modelo(cadena, idComponenteMayor);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Modelo iClase) {
            answer = Funciones.VRoles("cModelo");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Modelo iClase) {
            answer = Funciones.VRoles("dModelo");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
