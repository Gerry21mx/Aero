using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class LimiteController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        [Route("api/Limite/ByIdComponenteMenor")]
        public Answer GetByIdComponenteMenor(int idComponenteMenor) {
            answer.Data = Limite.GetLimites(idComponenteMenor);
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Limite(id);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Limite iClase) {
            answer = Funciones.VRoles("cLimite");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Limite iClase) {
            answer = Funciones.VRoles("dLimite");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
