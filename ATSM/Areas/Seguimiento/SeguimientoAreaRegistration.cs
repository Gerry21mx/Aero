using System.Web.Mvc;

namespace ATSM.Areas.Seguimiento
{
    public class SeguimientoAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Seguimiento";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Seguimiento_default",
                "Seguimiento/{controller}/{action}/{id}",
                new { controller="Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] {"ATSM.Areas.Seguimiento.Controllers"}
            );
        }
    }
}