using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api {
    public class AircraftController : ApiController {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            answer.Data = Aircraft.GetAircrafts();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Aircraft(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/Aircraft/ByCadena")]
        public Answer Get(string cadena) {
            answer.Data = new Aircraft(cadena);
            return answer;
        }

        // GET api/<controller>/UltimoTramo
        [Route("api/Aircraft/UltimoTramo")]
        public Answer GetUltimoTramo(int idAeronave) {
            Aircraft avi = new Aircraft(idAeronave);
            if (avi.Valid) {
                answer.Data = avi.UltimoTramo();
            }
            else {
                answer.Message = $"La Aeronave No Existe, por favor revise la Informacion. {idAeronave}";
            }
            return answer;
        }

        // GET api/<controller>/Tripulacion
        [Route("api/Aircraft/Tripulacion")]
        public Answer GetTripulacion(int idaeronave) {
            var avi = new Aircraft(idaeronave);
            var tripulacion = avi.GetTripulacion();
            answer.Data = new { Capitanes = tripulacion.Capitanes, Copilotos = tripulacion.Copilotos };
            return answer;
        }

        // GET api/<controller>/recalcular
        [Route("api/Aircraft/recalcular")]
        public Answer GetRecalcular(int idAircraft) {
            Aircraft avion = new Aircraft(idAircraft);
            answer.Data = avion.Recalcular();
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Aircraft iClase) {
            answer = Funciones.VRoles("cAircraft");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Aircraft iClase) {
            answer = Funciones.VRoles("dAircraft");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}