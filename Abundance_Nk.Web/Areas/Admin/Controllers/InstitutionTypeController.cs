using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class InstitutionTypeController :BasicSetupBaseController<InstitutionType,INSTITUTION_TYPE>
    {
        public InstitutionTypeController()
            : base(new InstitutionTypeLogic())
        {
            ModelName = "School Type";
            Selector = i => i.Institution_Type_Id == Id;
        }

        protected override bool ModifyModel(InstitutionType model)
        {
            try
            {
                var modelLogic = new InstitutionTypeLogic();
                return modelLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}