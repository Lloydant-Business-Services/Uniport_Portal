using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Abundance_Nk.Web
{
    public class MvcApplication :HttpApplication
    {
        protected void Application_Start()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            AreaRegistration.RegisterAllAreas();
           // GlobalConfiguration.Configure(WebApiConfig.Register);//for api call
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new AuthorizeAttribute());
        }
    }
}