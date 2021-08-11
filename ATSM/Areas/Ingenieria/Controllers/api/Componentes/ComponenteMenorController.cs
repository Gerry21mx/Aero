using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class ComponenteMenorController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new ComponenteMenor(id);
            return answer;
        }

		//GET api/<controller>
        public Answer Get(int idMayor, int? idModelo) {
			answer.Data = ComponenteMenor.GetComponentes(idMayor,idModelo);
			return answer;
		}

		//GET api/<controller>
        [Route("api/ComponenteMenor/ByModelo")]
        public Answer GetByModelo(int idModelo) {
			answer.Data = ComponenteMenor.GetComponentesByModelo(idModelo);
			return answer;
		}

		//GET api/<controller>
        [Route("api/ComponenteMenor/Compatibles")]
        public Answer GetCompatibles(int idMenor) {
			answer.Data = ComponenteMenor.GetCompatibles(idMenor);
			return answer;
		}

		// GET api/<controller>/ByCadena
		[Route("api/ComponenteMenor/ByCadena")]
		public Answer Get(string cadena) {
			answer.Data = new ComponenteMenor(cadena);
			return answer;
		}

		// GET api/<controller>/GenerarPN
		[Route("api/ComponenteMenor/GenerarPN")]
		public Answer GetGenerarPN(string cadena) {
            (string PNAD, string PNSB) PNS = ComponenteMenor.GenerarPartNumber(cadena);
            answer.Data = new { PNAD = PNS.PNAD, PNSB = PNS.PNSB };
            return answer;
        }

        //GET api/<controller>
        [Route("api/ComponenteMenor/query")]
        public Answer Getquery(int idMayor, int idFamilia, int idModelo, int idTipo, int ata1) {
            answer.Data = ComponenteMenor.queryReport(idMayor, idFamilia, idModelo, idTipo, ata1);
            return answer;
        }

        //GET api/<controller>
        [Route("api/ComponenteMenor/vinculados")]
        public Answer GetVinculados(int idComponenteMenor) {
            answer.Data = ComponenteMenor.GetVinculados(idComponenteMenor);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(ComponenteMenor iClase) {
            answer = Funciones.VRoles("cComponenteMenor");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(ComponenteMenor iClase) {
            answer = Funciones.VRoles("dComponenteMenor");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
