using ATSM.Operaciones;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Operaciones.Controllers.api.Catalogo
{
    public class ClienteController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            answer = Funciones.VAuth();
            if (answer.Status) {
                answer.Data = Cliente.GetClientes();
                return answer;
            }
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Cliente(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/Cliente/ByCadena")]
        public Answer Get(string cadena) {
            answer.Data = new Cliente(cadena);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Cliente iClase) {
            answer = Funciones.VRoles("cCliente");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Cliente iClase) {
            answer = Funciones.VRoles("dCliente");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
