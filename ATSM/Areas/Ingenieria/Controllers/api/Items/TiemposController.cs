using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class TiemposController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Tiempos(id);
            return answer;
        }

        // GET api/<controller>
        public Answer Get(int idItem,int tipo) {
            var tmps = Tiempos.GetTiempos(idItem,tipo);
            foreach(var t in tmps) {
                t.SetLimite();
			}
            answer.Data = tmps;
            return answer;
        }

        // GET api/<controller>
        //[Route("api/Tiempos/ByLimiteItem")]
        public Answer Get(int idLimite, int idItem, int tipo) {
            answer.Data = new Tiempos(idLimite, idItem, tipo);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Tiempos iClase) {
            answer = Funciones.VRoles("cTiempos");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Tiempos iClase) {
            answer = Funciones.VRoles("dTiempos");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
