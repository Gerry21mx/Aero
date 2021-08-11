using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api.Operacion
{
    public class BitacoraParametrosMotorController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>/id
        public Answer Get(int id) {
            answer.Data = new BitacoraParametrosMotor(id);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(BitacoraParametrosMotor iClase) {
            answer = Funciones.VRoles("cBitacora");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(BitacoraParametrosMotor iClase) {
            answer = Funciones.VRoles("dBitacora");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
