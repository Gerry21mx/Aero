using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api.Operacion
{
    public class BitacoraRCCAController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new BitacoraRCCA(id);
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int idBitacora, int no) {
            answer.Data = new BitacoraRCCA(idBitacora, no);
            return answer;
        }

        // GET api/<controller>
        [Route("api/BitacoraRCCA/RCCAs")]
        public Answer GetRCCAs(int idBitacora) {
            answer.Data = BitacoraRCCA.GetBitacoraRCCAs(idBitacora);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(BitacoraRCCA iClase) {
            answer = Funciones.VRoles("cBitacora");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(BitacoraRCCA iClase) {
            answer = Funciones.VRoles("dBitacora");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
