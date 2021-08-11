using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class MaterialToolController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        //public Answer Get() {
        //    answer.Data = MaterialTool.GetMaterialTools();
        //    return answer;
        //}

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new MaterialTool(id);
            return answer;
        }

        //// GET api/<controller>/ByCadena
        //[Route("api/MaterialTool/ByCadena")]
        //public Answer Get(string cadena) {
        //    answer.Data = new MaterialTool(cadena);
        //    return answer;
        //}

        // POST api/<controller>
        public Respuesta Post(MaterialTool iClase) {
            answer = Funciones.VRoles("cMaterialTool");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(MaterialTool iClase) {
            answer = Funciones.VRoles("dMaterialTool");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
