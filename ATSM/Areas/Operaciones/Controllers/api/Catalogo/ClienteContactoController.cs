using ATSM.Operaciones;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Operaciones.Controllers.api.Catalogo
{
    public class ClienteContactoController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new ClienteContacto(id);
            return answer;
        }

        // GET api/<controller>
        [Route("api/ClienteContacto/ContactosCLiente")]
        public Answer GetContactosCLiente(int idcliente) {
            answer.Data = ClienteContacto.GetClienteContactos(idcliente);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/ClienteContacto/ByNombre")]
        public Answer Get(int idcliente, string nombre) {
            answer.Data = new ClienteContacto(idcliente, nombre);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(ClienteContacto iClase) {
            answer = Funciones.VRoles("cClienteContacto");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(ClienteContacto iClase) {
            answer = Funciones.VRoles("dClienteContacto");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
