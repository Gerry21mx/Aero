using ATSM.Ingenieria;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api.Operacion {
	public class BitacoraController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Bitacora(id);
            return answer;
        }

        // GET api/<controller>/byAircraftFolio
        [Route("api/Bitacora/byAircraftFolio")]
        public Answer Get(int idAircraft, int idFolio) {
            answer.Data = new Bitacora(idAircraft, idFolio);
            return answer;
        }

        // GET api/<controller>/inicial
        [Route("api/Bitacora/inicial")]
        public Answer GetInicial(int idAircraft) {
            answer.Data = Bitacora.BitacoraInicial(idAircraft);
            return answer;
        }

        // GET api/<controller>/final
        [Route("api/Bitacora/final")]
        public Answer GetFinal(int idAircraft) {
            answer.Data = Bitacora.BitacoraFinal(idAircraft);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Bitacora iClase) {
            answer = Funciones.VRoles("cBitacora");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Bitacora iClase) {
            answer = Funciones.VRoles("dBitacora");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
