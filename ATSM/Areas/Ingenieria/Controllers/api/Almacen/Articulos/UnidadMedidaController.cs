using ATSM.Almacen;

using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api.Almacen.Articulos {
	public class UnidadMedidaController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            answer.Data = UnidadMedida.GetUnidadMedidas();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new UnidadMedida(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/UnidadMedida/ByCadena")]
        public Answer Get(string cadena) {
            answer.Data = new UnidadMedida(cadena);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(UnidadMedida iClase) {
            answer = Funciones.VRoles("cUnidadMedida");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(UnidadMedida iClase) {
            answer = Funciones.VRoles("dUnidadMedida");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
