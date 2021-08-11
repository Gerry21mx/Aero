using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class TaskTypeController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        //public Answer Get() {
        //    answer.Data = TaskType.GetTaskTypes();
        //    return answer;
        //}

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new TaskType(id);
            return answer;
        }

        //// GET api/<controller>/ByCadena
        //[Route("api/TaskType/ByCadena")]
        //public Answer Get(string cadena) {
        //    answer.Data = new TaskType(cadena);
        //    return answer;
        //}

        // POST api/<controller>
        //public Respuesta Post(TaskType iClase) {
        //    answer = Funciones.VRoles("cTaskType");
        //    if (answer.Status) {
        //        return iClase.Save();
        //    }
        //    respuesta.Error = answer.Message;
        //    return respuesta;
        //}

        // DELETE api/<controller>/5
        //public Respuesta Delete(TaskType iClase) {
        //    answer = Funciones.VRoles("dTaskType");
        //    if (answer.Status) {
        //        return iClase.Delete();
        //    }
        //    respuesta.Error = answer.Message;
        //    return respuesta;
        //}
    }
}
