using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class StudentTypeController :BasicSetupBaseController<StudentType,STUDENT_TYPE>
    {
        public StudentTypeController()
            : base(new StudentTypeLogic())
        {
            ModelName = "Student Type";
            Selector = r => r.Student_Type_Id == Id;
        }

        protected override bool ModifyModel(StudentType model)
        {
            try
            {
                var modelLogic = new StudentTypeLogic();
                return modelLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}