using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class ComponenteMenorVinculadoController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>/Id
        public Answer Get(int idComponente, int idVinculado) {
			answer.Data = new ComponenteMenorVinculado(idComponente, idVinculado);
            return answer;
        }

        // GET api/<controller>
        [Route("api/ComponenteMenorVinculado/Vinculados")]
        public Answer GetVinculados(int idComponente) {
            answer.Data = ComponenteMenorVinculado.GetComponenteMenorVinculados(idComponente);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(ComponenteMenorVinculado iClase) {
            answer = Funciones.VRoles("cComponenteMenorVinculado");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(ComponenteMenorVinculado iClase) {
            answer = Funciones.VRoles("dComponenteMenorVinculado");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
