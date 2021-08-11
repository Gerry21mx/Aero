using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ATSM.Ingenieria;
namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class PositionController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            answer.Data = Position.GetPositions();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Position(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/Position/ByCadena")]
        public Answer Get(string cadena) {
            answer.Data = new Position(cadena);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Position iClase) {
            answer = Funciones.VRoles("cPosition");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Position iClase) {
            answer = Funciones.VRoles("dPosition");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
