using System.Web.Mvc;

namespace ATSM.Areas.Cuentas
{
    public class CuentasAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Cuentas";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Cuentas_default",
                "Cuentas/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "ATSM.Areas.Cuentas.Controllers" }
            );
        }
    }
}