using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.PGApplicant
{
    public class PGApplicantAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PGApplicant";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PGApplicant_default",
                "PGApplicant/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}