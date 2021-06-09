using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class OLevelGradeController :BasicSetupBaseController<OLevelGrade,O_LEVEL_GRADE>
    {
        public OLevelGradeController()
            : base(new OLevelGradeLogic())
        {
            ModelName = "O-Level Grade";
            Selector = o => o.O_Level_Grade_Id == Id;
        }

        protected override bool ModifyModel(OLevelGrade model)
        {
            try
            {
                var oLevelGradeLogic = new OLevelGradeLogic();
                return oLevelGradeLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}