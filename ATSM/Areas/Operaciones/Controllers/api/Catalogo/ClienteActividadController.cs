using ATSM.Operaciones;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Operaciones.Controllers.api.Catalogo
{
    public class ClienteActividadController : ApiController {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new ClienteActividad(id);
            return answer;
        }

        // GET api/<controller>
        [Route("api/ClienteActividad/ByCliente")]
        public Answer GetByCliente(int idcliente) {
            answer.Data = ClienteActividad.GetClienteActividads(idcliente);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(ClienteActividad iClase) {
            answer = Funciones.VRoles("cClienteActividad");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(ClienteActividad iClase) {
            answer = Funciones.VRoles("dClienteActividad");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
