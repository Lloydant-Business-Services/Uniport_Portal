using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class OLevelTypeController :BasicSetupBaseController<OLevelType,O_LEVEL_TYPE>
    {
        public OLevelTypeController()
            : base(new OLevelTypeLogic())
        {
            ModelName = "O-Level Type";
            Selector = o => o.O_Level_Type_Id == Id;
        }

        protected override bool ModifyModel(OLevelType model)
        {
            try
            {
                var modelLogic = new OLevelTypeLogic();
                return modelLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}