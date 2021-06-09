using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class PaymentTypeController :BasicSetupBaseController<PaymentType,PAYMENT_TYPE>
    {
        public PaymentTypeController()
            : base(new PaymentTypeLogic())
        {
            ModelName = "Person Type";
            Selector = p => p.Payment_Type_Id == Id;
        }

        protected override bool ModifyModel(PaymentType model)
        {
            try
            {
                var modelLogic = new PaymentTypeLogic();
                return modelLogic.Modify(model);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}