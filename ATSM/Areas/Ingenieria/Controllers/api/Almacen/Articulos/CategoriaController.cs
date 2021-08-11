using ATSM.Almacen;

using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api.Almacen.Articulos {
	public class CategoriaController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            answer.Data = Categoria.GetCategorias();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Categoria(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/Categoria/ByCadena")]
        public Answer Get(string cadena) {
            answer.Data = new Categoria(cadena);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Categoria iClase) {
            answer = Funciones.VRoles("cCategoria");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Categoria iClase) {
            answer = Funciones.VRoles("dCategoria");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
