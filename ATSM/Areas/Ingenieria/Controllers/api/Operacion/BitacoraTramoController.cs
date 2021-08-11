using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api.Operacion
{
    public class BitacoraTramoController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new BitacoraTramo(id);
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int idBitacora, int idPierna) {
            answer.Data = new BitacoraTramo(idBitacora, idPierna);
            return answer;
        }

        // GET api/<controller>/Tramos
        [Route("api/BitacoraTramo/Tramos")]
        public Answer GetTramos(int idBitacora) {
            answer.Data = BitacoraTramo.GetBitacoraTramos(idBitacora);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(BitacoraTramo iClase) {
            answer = Funciones.VRoles("cBitacora");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(BitacoraTramo iClase) {
            answer = Funciones.VRoles("dBitacora");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
