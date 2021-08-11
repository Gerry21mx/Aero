using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Owin;

using Owin;

[assembly: OwinStartup(typeof(ATSM.Startup))]
namespace ATSM {
	public class Startup {
		public void Configuration(IAppBuilder app) {
			// Para obtener más información sobre cómo configurar la aplicación, visite https://go.microsoft.com/fwlink/?LinkID=316888
			app.MapSignalR();
		}
	}
}