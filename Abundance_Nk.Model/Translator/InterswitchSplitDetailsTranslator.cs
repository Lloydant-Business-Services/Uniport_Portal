using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class InterswitchSplitDetailsTranslator : TranslatorBase<InterswitchSplitDetails,INTERSWITCH_SPLIT_DETAILS>
    {
        private FeeTypeTranslator feeTypeTranslator;

        public InterswitchSplitDetailsTranslator()
        {
            feeTypeTranslator = new FeeTypeTranslator();
        }
        public override InterswitchSplitDetails TranslateToModel(INTERSWITCH_SPLIT_DETAILS entity)
        {
            try
            {
                InterswitchSplitDetails model = null;
                if (entity != null)
                {
                    model = new InterswitchSplitDetails();
                    model.Id = entity.Interswitch_Split_Details_Id;
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.BankCode = entity.Bank_Code;
                    model.Activated = entity.Activated;
                    model.BeneficiaryAccount = entity.Beneficiary_Account;
                    model.BeneficiaryAmount = entity.Beneficiary_Amount;
                    model.BeneficiaryName = entity.Beneficiary_Name;
                    
                }
                return model;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public override INTERSWITCH_SPLIT_DETAILS TranslateToEntity(InterswitchSplitDetails model)
        {
            try
            {
                INTERSWITCH_SPLIT_DETAILS entity = null;
                if (model != null)
                {
                    entity = new INTERSWITCH_SPLIT_DETAILS();
                    entity.Interswitch_Split_Details_Id = model.Id;
                    entity.FeeType_Id = model.FeeType.Id;
                    entity.Bank_Code = entity.Bank_Code;
                    entity.Activated = model.Activated;
                    entity.Beneficiary_Account = model.BeneficiaryAccount;
                    entity.Beneficiary_Amount = model.BeneficiaryAmount;
                    entity.Beneficiary_Name = entity.Beneficiary_Name;
                }
                return entity;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
    }
}
