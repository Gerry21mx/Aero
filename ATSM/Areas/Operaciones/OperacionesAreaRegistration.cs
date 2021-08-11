using System.Web.Mvc;

namespace ATSM.Areas.Operaciones
{
    public class OperacionesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Operaciones";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Operaciones_default",
                "Operaciones/{controller}/{action}/{id}",
                new { controller = "Default", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "ATSM.Areas.Operaciones.Controllers" }
            );
        }
    }
}