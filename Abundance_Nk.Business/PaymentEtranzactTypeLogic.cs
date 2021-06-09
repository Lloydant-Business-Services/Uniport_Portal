using System;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class PaymentEtranzactTypeLogic : BusinessBaseLogic<PaymentEtranzactType, PAYMENT_ETRANZACT_TYPE>
    {
        public PaymentEtranzactTypeLogic()
        {
            translator = new PaymentEtranzactTypeTranslator();
        }

        public PaymentEtranzactType GetBy(FeeType feeType)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT_TYPE, bool>> selector = m => m.Fee_Type_Id == feeType.Id;
                return GetModelsBy(selector).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(PaymentEtranzactType model)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT_TYPE, bool>> selector = s => s.Payment_Etranzact_Type_Id == model.Id;
                PAYMENT_ETRANZACT_TYPE entity = GetEntityBy(selector);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.Session != null && model.Session.Id > 0)
                {
                    entity.Session_Id = model.Session.Id;
                }
                if (model.programme != null && model.programme.Id > 0)
                {
                    entity.Programme_Id = model.programme.Id;
                }
                if (model.FeeType != null && model.FeeType.Id > 0)
                {
                    entity.Fee_Type_Id = model.FeeType.Id;
                }
                if (model.PaymentMode != null && model.PaymentMode.Id > 0)
                {
                    entity.Payment_Mode_Id = model.PaymentMode.Id;
                }
                entity.Payment_Etranzact_Type_Name = model.Name;

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