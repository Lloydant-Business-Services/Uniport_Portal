using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class PersonTypeController :BasicSetupBaseController<PersonType,PERSON_TYPE>
    {
        public PersonTypeController()
            : base(new PersonTypeLogic())
        {
            ModelName = "Person Type";
            Selector = r => r.Person_Type_Id == Id;
        }

        protected override bool ModifyModel(PersonType model)
        {
            try
            {
                var modelLogic = new PersonTypeLogic();
                return modelLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}