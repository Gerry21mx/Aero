using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class TImageController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        //public Answer Get() {
        //    answer.Data = TImage.GetTImages();
        //    return answer;
        //}

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new TImage(id);
            return answer;
        }

		// GET api/<controller>/ByComponenteModelo
		[Route("api/TImage/ByComponenteModelo")]
		public Answer GetByComponenteModelo(int idComponente, int idModelo) {
			answer.Data = new TImage(idComponente, idModelo);
			return answer;
		}

		// POST api/<controller>
		public Respuesta Post(TImage iClase) {
            answer = Funciones.VRoles("cTImage");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(TImage iClase) {
            answer = Funciones.VRoles("dTImage");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
