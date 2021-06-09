using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class ResultGradeController :BasicSetupBaseController<ResultGrade,RESULT_GRADE>
    {
        public ResultGradeController()
            : base(new ResultGradeLogic())
        {
            ModelName = "Result Grade";
            Selector = r => r.Result_Grade_Id == Id;
        }

        protected override bool ModifyModel(ResultGrade model)
        {
            try
            {
                var modelLogic = new ResultGradeLogic();
                return modelLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}