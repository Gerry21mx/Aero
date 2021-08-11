using System.Web.Mvc;

namespace ATSM.Areas.Pilotos
{
    public class PilotosAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Pilotos";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Pilotos_default",
                "Pilotos/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "ATSM.Areas.Pilotos.Controllers" }
            );
        }
    }
}