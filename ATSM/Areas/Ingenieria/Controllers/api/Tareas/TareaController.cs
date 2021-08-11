using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class TareaController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        //public Answer Get() {
        //    answer.Data = Tarea.GetTareas();
        //    return answer;
        //}

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Tarea(id);
            return answer;
        }

        //// GET api/<controller>/ByCadena
        //[Route("api/Tarea/ByCadena")]
        //public Answer Get(string cadena) {
        //    answer.Data = new Tarea(cadena);
        //    return answer;
        //}

        // POST api/<controller>
        public Respuesta Post(Tarea iClase) {
            answer = Funciones.VRoles("cTarea");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Tarea iClase) {
            answer = Funciones.VRoles("dTarea");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
