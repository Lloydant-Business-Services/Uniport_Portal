using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.PGStudent
{
    public class PGStudentAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PGStudent";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PGStudent_default",
                "PGStudent/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}