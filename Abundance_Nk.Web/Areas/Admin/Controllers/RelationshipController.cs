using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class RelationshipController :BasicSetupBaseController<Relationship,RELATIONSHIP>
    {
        public RelationshipController()
            : base(new RelationshipLogic())
        {
            ModelName = "Relationship";
            Selector = r => r.Relationship_Id == Id;
        }

        protected override bool ModifyModel(Relationship model)
        {
            try
            {
                var relationshipLogic = new RelationshipLogic();
                return relationshipLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}