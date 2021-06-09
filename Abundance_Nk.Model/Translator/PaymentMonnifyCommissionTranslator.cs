using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentMonnifyCommissionTranslator : TranslatorBase<PaymentMonnifyCommission,PAYMENT_MONNIFY_COMMISSION>
    {
        DepartmentTranslator DepartmentTranslator;
        ProgrammeTranslator ProgrammeTranslator;
        LevelTranslator levelTranslator;
        FeeTypeTranslator feeTypeTranslator;
        PaymentModeTranslator PaymentModeTranslator;

        public PaymentMonnifyCommissionTranslator()
        {
            DepartmentTranslator = new DepartmentTranslator();
            ProgrammeTranslator = new ProgrammeTranslator();
            levelTranslator = new LevelTranslator();
            feeTypeTranslator = new FeeTypeTranslator();
        }

        public override PAYMENT_MONNIFY_COMMISSION TranslateToEntity(PaymentMonnifyCommission model)
        {
            try
            {
                PAYMENT_MONNIFY_COMMISSION entity = null;
                if (model != null)
                {
                    entity = new PAYMENT_MONNIFY_COMMISSION();
                    entity.Commission = model.Commission;
                    entity.Department_Id = model.Department.Id;
                    entity.Fee_Type_Id = model.FeeType.Id;
                    entity.Level_Id = model.FeeType.Id;
                    entity.Payment_Mode_Id = model.PaymentMode.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Use_Level = model.UseLevel;
                }
                return entity;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override PaymentMonnifyCommission TranslateToModel(PAYMENT_MONNIFY_COMMISSION entity)
        {
            try
            {
                PaymentMonnifyCommission model = null;
                if (entity != null)
                {
                    model = new PaymentMonnifyCommission();
                    model.Commission = entity.Commission;
                    model.Department = DepartmentTranslator.Translate(entity.DEPARTMENT);
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Id = entity.Id;
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.PaymentMode = PaymentModeTranslator.Translate(entity.PAYMENT_MODE);
                    model.Programme = ProgrammeTranslator.Translate(entity.PROGRAMME);
                    model.UseLevel = entity.Use_Level;

                }
                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
