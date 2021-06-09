using System;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class PaymentTerminalLogic : BusinessBaseLogic<PaymentTerminal, PAYMENT_TERMINAL>
    {
        public PaymentTerminalLogic()
        {
            translator = new PaymentTerminalTranslator();
        }
        public PaymentTerminal GetBy(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT_TERMINAL, bool>> selector = p => p.Fee_Type_Id == payment.FeeType.Id && p.Session_Id == payment.Session.Id;
                return GetModelsBy(selector).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(PaymentTerminal model)
        {
            try
            {
                Expression<Func<PAYMENT_TERMINAL, bool>> selector = s => s.Payment_Terminal_Id == model.Id;
                PAYMENT_TERMINAL entity = GetEntityBy(selector);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.Session != null && model.Session.Id > 0)
                {
                    entity.Session_Id = model.Session.Id;
                }
                if (model.FeeType != null && model.FeeType.Id > 0)
                {
                    entity.Fee_Type_Id = model.FeeType.Id;
                }

                entity.Terminal_Id = model.TerminalId;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}