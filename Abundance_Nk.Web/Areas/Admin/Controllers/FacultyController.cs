using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class FacultyController :BasicSetupBaseController<Faculty,FACULTY>
    {
        public FacultyController()
            : base(new FacultyLogic())
        {
            ModelName = "Faculty";
            Selector = f => f.Faculty_Id == Id;
        }

        protected override bool ModifyModel(Faculty model)
        {
            try
            {
                var facultyLogic = new FacultyLogic();
                return facultyLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}