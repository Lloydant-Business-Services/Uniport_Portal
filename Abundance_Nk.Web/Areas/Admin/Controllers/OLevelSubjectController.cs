using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class OLevelSubjectController :BasicSetupBaseController<OLevelSubject,O_LEVEL_SUBJECT>
    {
        public OLevelSubjectController()
            : base(new OLevelSubjectLogic())
        {
            ModelName = "O-Level Subject";
            Selector = o => o.O_Level_Subject_Id == Id;
        }

        protected override bool ModifyModel(OLevelSubject model)
        {
            try
            {
                var oLevelSubjectLogic = new OLevelSubjectLogic();
                return oLevelSubjectLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}