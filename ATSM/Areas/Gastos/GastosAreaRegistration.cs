using System.Web.Mvc;

namespace ATSM.Areas.Gastos {
    public class GastosAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "Gastos";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "Gastos_default",
                "Gastos/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "ATSM.Areas.Gastos.Controllers" }
            );
        }
    }
}