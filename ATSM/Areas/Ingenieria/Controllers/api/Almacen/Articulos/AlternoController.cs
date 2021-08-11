using ATSM.Almacen;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api.Almacen.Articulos
{
    public class AlternoController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            answer.Data = Alterno.GetAlternos();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Alterno(id);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Alterno iClase) {
            answer = Funciones.VRoles("cAlterno");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Alterno iClase) {
            answer = Funciones.VRoles("dAlterno");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
