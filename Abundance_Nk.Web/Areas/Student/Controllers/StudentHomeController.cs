using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    public class StudentHomeController :StudentBaseController
    {
        // GET: Student/StudentHome
        public ActionResult Index()
        {
            if(ValidateStudent("15/98498","1234567"))
            {
                return View();
            }
            return RedirectToAction("Index","Home",new { Area = " " });
            return View();
        }
    }
}