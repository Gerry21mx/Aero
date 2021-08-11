using ATSM.Ingenieria;

using Newtonsoft.Json;

using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api {
	public class PICController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

		// GET api/<controller>/Id
		public Answer Get(int id) {
            answer.Data = new PIC(id);
            return answer;
        }

        //GET api/<controller>/Componentes
        [Route("api/PIC/Componentes")]
        public Answer GetComponente(int idpic) {
            PIC pic = new PIC(idpic);
            answer.Data = pic.Componentes;
            return answer;
        }

        //GET api/<controller>/ByComponente
        [Route("api/PIC/ByComponente")]
        public Answer GetByComponente(int idComponente) {
			answer.Data = PIC.GetPICsByComponente(idComponente);
			return answer;
		}

        //GET api/<controller>/ByComponenteMenor
        [Route("api/PIC/ByComponenteMenor")]
        public Answer GetByComponenteMenor(int idMenor) {
			answer.Data = PIC.GetPICsByComponenteMenor(idMenor);
			return answer;
		}

        //GET api/<controller>/MenorEnPICMayorModelo
        [Route("api/PIC/MenorEnPICMayorModelo")]
        public Answer GetMenorEnPICMayorModelo(int idmayor, int idmodelo, int idmenor) {
			answer.Data = PIC.MenorEnPICMayorModelo(idmayor, idmodelo, idmenor);
			return answer;
		}

		//GET api/<controller>/ByModelo
        [Route("api/PIC/ByModelo")]
        public Answer GetByModelo(int idmodelo) {
			answer.Data = PIC.GetPICsByModelo(idmodelo);
			return answer;
		}

		//GET api/<controller>/ByModelo
        [Route("api/PIC/ByMMA")]
        public Answer GetByMMA(int idmayor, int idmodelo, int ata1) {
			var lisPic = PIC.GetPICsByMayorModeloAata(idmayor, idmodelo, ata1);
            foreach(var pic in lisPic) {
                pic.SetMayor();
                pic.SetModelo();
                pic.SetPosicion();
                pic.SetComponentes();
			}
            answer.Data = lisPic;
            return answer;
		}

        // POST api/<controller>
        public Respuesta Post(PIC iClase) {
            answer = Funciones.VRoles("cPIC");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(PIC iClase) {
            answer = Funciones.VRoles("dPIC");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
