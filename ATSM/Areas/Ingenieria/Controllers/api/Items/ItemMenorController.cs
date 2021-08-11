using ATSM.Ingenieria;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api {
    public class ItemMenorController : ApiController {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get(int idAircraft, int? idComponenteMayor, int? idMayor, int? idFamilia) {
            answer.Data = ItemMenor.GetItemMenors(idAircraft, idComponenteMayor, idMayor, idFamilia);
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new ItemMenor(id);
            return answer;
        }

        // GET api/<controller>/Id
        [Route("api/ItemMenor/ByMayorMenorPic")]
        public Answer GetByMayorMenorPic(int idMayor, int idMenor, int? idPic) {
            answer.Data = new ItemMenor(idMayor, idMenor, idPic);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(ItemMenor iClase) {
        //public Respuesta Post(dynamic objeto) {
            //ItemMenor iClase = JsonConvert.DeserializeObject<ItemMenor>(JsonConvert.SerializeObject(objeto));
            answer = Funciones.VRoles("cItemMenor");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(ItemMenor iClase) {
            answer = Funciones.VRoles("dItemMenor");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}