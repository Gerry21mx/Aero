using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ATSM.Controllers
{
    public class SystemController : ApiController {
		private Answer answer = new Answer();
		//private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public dynamic Get()
		{
			return new { System = "ATSM", Author = "Erick Mireles", Mail = "javierickmr@gmail.com", Cell = "8441864873" };
		}

		// GET api/<controller>/FileExist
		[Route("api/System/FileExist")]
		public Answer GetFileExist(string ruta, string archivo) {
			string file = HttpContext.Current.Request.MapPath($"~/{ruta}{archivo}");
			bool ex = File.Exists(file);
			if (!ex) {
				string fil0 = archivo;
				string ext0 = Path.GetExtension(fil0);
				string nar0 = string.IsNullOrEmpty(ext0) ? fil0 : fil0.Replace(ext0, "");

				string dire = HttpContext.Current.Request.MapPath($"~/{ruta}");
				var flsInDir = Directory.EnumerateFiles(dire);
				foreach (var arc in flsInDir) {
					string fil = arc.Replace(dire, "");
					string ext = Path.GetExtension(fil);
					string na = fil.Replace(ext, "");
					if (nar0 == na) {
						archivo = nar0 + ext;
						ex = true;
						break;
					}
				}
			}
			answer.Data = archivo;
			answer.Status = ex;
			return answer;
		}
	}
}
