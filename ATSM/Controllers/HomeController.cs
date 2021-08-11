using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;

using WebMatrix.WebData;

namespace ATSM.Controllers {
	public class HomeController : Controller {
		public ActionResult Index() {
			return View();
		}
	}
}