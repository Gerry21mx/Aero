using ATSM.Ingenieria;

using HtmlAgilityPack;

using ScrapySharp.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api.Catalogos
{
    public class BiweeklyController : ApiController
    {
        private Answer answer = new Answer();
        private Respuesta respuesta = new Respuesta();

        // GET api/<controller>
        public Answer Get() {
            answer.Data = Biweekly.GetBiweeklys();
            return answer;
        }

        // GET api/<controller>/Id
        public Answer Get(int id) {
            answer.Data = new Biweekly(id);
            return answer;
        }

        // GET api/<controller>/ByCadena
        [Route("api/Biweekly/ByCadena")]
        public Answer Get(string cadena) {
            answer.Data = new Biweekly(cadena);
            return answer;
        }

        // GET api/<controller>/faa
        [Route("api/Biweekly/faa")]
        public Answer GetFAA() {
            List<dynamic> datos = new List<dynamic>();
            HtmlWeb oWeb = new HtmlWeb();
            HtmlDocument doc = oWeb.Load("https://rgl.faa.gov/Regulatory_and_Guidance_Library/rgAD.nsf/webADBiweekly!OpenView&Start=1&Count=200&Expand=1#1");
            //var nodos = doc.DocumentNode.CssSelect("td > font");
            var nodos = doc.DocumentNode.CssSelect("td[width=280] > font[size=2]");
            foreach(var font in nodos) {
                HtmlNode Link = font.CssSelect("a").First();
				if (Link.HasChildNodes) {
					string text = Link.InnerText;
					string link = Link.Attributes["Href"].Value;
                    var data = text.Split(',');
                    DateTime? F1 = null, F2 = null;
					if (data.Length == 3) {
                        var periodo = data[2].Split('-');
						if (periodo.Length == 2) {
                            var df = periodo[0].Split('/');
                            F1 = new DateTime(Int32.Parse(df[2]), Int32.Parse(df[0]), Int32.Parse(df[1]));
                            df = periodo[1].Split('/');
                            F2 = new DateTime(Int32.Parse(df[2]), Int32.Parse(df[0]), Int32.Parse(df[1]));
						}
					}
                    string documentos = link.Replace("\\", "/");
                    string biew = data[1].Replace("Biweekly", "").Trim();
                    var cdxl = biew.Split('-');

                    string pdf = documentos.Replace("?OpenDocument", $"/$FILE/{(data[0].IndexOf("Large") > -1 ?"LG":"SM")}{biew}.pdf");
                    string excel = documentos.Replace("?OpenDocument", $"/$FILE/{(data[0].IndexOf("Large") > -1 ?"LG":"SM")}{cdxl[0]}_TOC.xlsx");
                    var res = new { Tipo = data[0], Biweekly = biew, Fecha1 = F1, Fecha2 = F2, Documentos = $"https://rgl.faa.gov{documentos}", PDF = $"https://rgl.faa.gov{pdf}", Excel = $"https://rgl.faa.gov{excel}" };
                    datos.Add(res);
				}

            }
            answer.Data = datos;
            return answer;
        }

        // POST api/<controller>
        public Respuesta Post(Biweekly iClase) {
            answer = Funciones.VRoles("cBiweekly");
            if (answer.Status) {
                return iClase.Save();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }

        // DELETE api/<controller>/5
        public Respuesta Delete(Biweekly iClase) {
            answer = Funciones.VRoles("dBiweekly");
            if (answer.Status) {
                return iClase.Delete();
            }
            respuesta.Error = answer.Message;
            return respuesta;
        }
    }
}
