using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class ComponenteMenorModeloController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get(int idMayor, int idModelo) {
            answer.Data = ComponenteMenorModelo.GetComponenteMenorModelos(idMayor, idModelo);
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new ComponenteMenorModelo(id);
            return answer;
        }

        // GET api/<controller>/ByMayorMenorModelo
        [Route("api/ComponenteMenorModelo/ByMayorMenorModelo")]
        public Answer Get(int idMayor, int idMenor, int idModelo) {
            answer.Data = new ComponenteMenorModelo(idMayor, idMenor, idModelo);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(ComponenteMenorModelo iClase) {
            answer = Funciones.VRoles("cComponenteMenorModelo");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(ComponenteMenorModelo iClase) {
            answer = Funciones.VRoles("dComponenteMenorModelo");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
