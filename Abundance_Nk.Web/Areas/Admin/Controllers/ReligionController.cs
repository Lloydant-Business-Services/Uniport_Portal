using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class ReligionController :BasicSetupBaseController<Religion,RELIGION>
    {
        public ReligionController()
            : base(new ReligionLogic())
        {
            ModelName = "Religion";
            Selector = r => r.Religion_Id == Id;
        }

        protected override bool ModifyModel(Religion model)
        {
            try
            {
                var ReligionLogic = new ReligionLogic();
                return ReligionLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}