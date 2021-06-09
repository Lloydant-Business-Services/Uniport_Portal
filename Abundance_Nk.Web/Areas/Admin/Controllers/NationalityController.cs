using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class NationalityController :BasicSetupBaseController<Nationality,NATIONALITY>
    {
        public NationalityController()
            : base(new NationalityLogic())
        {
            ModelName = "Nationality";
            Selector = n => n.Nationality_Id == Id;
        }

        protected override bool ModifyModel(Nationality model)
        {
            try
            {
                var modelLogic = new NationalityLogic();
                return modelLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}