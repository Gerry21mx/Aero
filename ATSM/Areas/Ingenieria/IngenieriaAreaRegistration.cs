using System.Web.Mvc;

namespace ATSM.Areas.Ingenieria {
    public class IngenieriaAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "Ingenieria";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "Ingenieria_default",
                "Ingenieria/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "ATSM.Areas.Ingenieria.Controllers" }
            );
        }
    }
}