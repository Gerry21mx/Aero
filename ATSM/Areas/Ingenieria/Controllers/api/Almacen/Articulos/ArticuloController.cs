using ATSM.Almacen;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api.Almacen.Articulos
{
    public class ArticuloController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();
        HttpRequest Request = HttpContext.Current.Request;

        // GET api/<controller>
        public Answer Get() {
            answer.Data = Articulo.GetArticulos();
            return answer;
        }

        // GET api/<controller>
        [Route("api/Articulo/vart")]
        public Answer GetVart() {
            answer.Data = Articulo.GetVArticulos();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Articulo(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/Articulo/ByCadena")]
        public Answer Get(string cadena) {
            answer.Data = new Articulo(cadena);
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post() {
            answer = Funciones.VRoles("cArticulo");
            if (answer.Status) {
                string dat = Request["json"];
                Articulo articulo = JsonConvert.DeserializeObject<Articulo>(dat);
                foreach (string file in Request.Files) {
                    var postedFile = Request.Files[file];
                    if (!string.IsNullOrEmpty(postedFile.FileName)) {
                        string arc = postedFile.FileName.Trim();
                        string ext = Path.GetExtension(arc);
                        articulo.FileName = articulo.Id + ext;
                        string ruta = Request.MapPath("~/Files/Almacen/Articulos/");
                        postedFile.SaveAs(ruta + articulo.FileName);
                        break;
                    }
                }
                return articulo.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Articulo iClase) {
            answer = Funciones.VRoles("dArticulo");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
