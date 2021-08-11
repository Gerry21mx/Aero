using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class InstructionController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        //public Answer Get() {
        //    answer.Data = Instruction.GetInstructions();
        //    return answer;
        //}

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Instruction(id);
            return answer;
        }

        //// GET api/<controller>/ByCadena
        //[Route("api/Instruction/ByCadena")]
        //public Answer Get(string cadena) {
        //    answer.Data = new Instruction(cadena);
        //    return answer;
        //}

        // POST api/<controller>
        public Respuesta Post(Instruction iClase) {
            answer = Funciones.VRoles("cInstruction");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Instruction iClase) {
            answer = Funciones.VRoles("dInstruction");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
