using ATSM.Operaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ATSM.Areas.Operaciones.Controllers.api.Vuelos
{
    public class CotizacionController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();
         
        // GET api/<controller>
        public Answer Get()
        {
            answer.Data = Cotizacion.GetCotizacions();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id)
        {
            answer.Data = new Cotizacion(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        //[Route("api/Cotizacion/ByCadena")]
        //public Answer Get(string cadena)
        //{
            //answer.Data = new Cotizacion(cadena);
            //return answer;
        //}

        // POST api/<controller>
        public Cotizacion Post(Cotizacion iClase)
        {
            answer = Funciones.VRoles("cCotizacion");
            return iClase;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Cotizacion iClase)
        {
            answer = Funciones.VRoles("dCotizacion");
            if (answer.Status)
            {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}