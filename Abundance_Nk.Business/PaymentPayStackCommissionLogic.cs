using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class PaymentPayStackCommissionLogic : BusinessBaseLogic<PaymentPaystackCommission,PAYMENT_PAYSTACK_COMMISSION>
    {
       
        public PaymentPayStackCommissionLogic()
        {
            translator = new PaystackCommissionTranslator();
        }

        public bool Modify(PaymentPaystackCommission paymentPaystackCommission)
        {
            try
            {

                Expression<Func<PAYMENT_PAYSTACK_COMMISSION, bool>> selector = a => a.Payment_PayStack_Commission_Id == paymentPaystackCommission.Id;
                PAYMENT_PAYSTACK_COMMISSION entity = GetEntityBy(selector);

                entity.FeeType_Id = paymentPaystackCommission.FeeType.Id;
                //entity.Fee_Id = paymentPaystackCommission.Fee.Id;
                entity.Session_Id = paymentPaystackCommission.Session.Id;
                entity.Activated = paymentPaystackCommission.Activated;
                entity.Amount = paymentPaystackCommission.Amount;
                entity.Programme_Id = paymentPaystackCommission.Programme.Id;
                if (paymentPaystackCommission.User != null)
                {
                    entity.SetUpBy = paymentPaystackCommission.User.Id;
                }
                
                entity.AddedFee = paymentPaystackCommission.AddOnFee;

                int modifiedCount = Save();
                if (modifiedCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return false;
        }
           
        
    }
}
