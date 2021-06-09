using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicationProgrammeFeeTranslator : TranslatorBase<ApplicationProgrammeFee, APPLICATION_PROGRAMME_FEE>
    {
        private readonly FeeTypeTranslator feeTypeTranslator;
        private readonly ProgrammeTranslator programmeTranslator;
        private readonly SessionTranslator sessionTranslator;

        public ApplicationProgrammeFeeTranslator()
        {
            feeTypeTranslator = new FeeTypeTranslator();
            programmeTranslator = new ProgrammeTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override ApplicationProgrammeFee TranslateToModel(APPLICATION_PROGRAMME_FEE entity)
        {
            try
            {
                ApplicationProgrammeFee model = null;
                if (entity != null)
                {
                    model = new ApplicationProgrammeFee();
                    model.Id = entity.Application_Programme_Fee_Id;
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.DateEntered = entity.Date_Entered;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICATION_PROGRAMME_FEE TranslateToEntity(ApplicationProgrammeFee model)
        {
            try
            {
                APPLICATION_PROGRAMME_FEE entity = null;
                if (model != null)
                {
                    entity = new APPLICATION_PROGRAMME_FEE();
                    entity.Application_Programme_Fee_Id = model.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Fee_Type_Id = model.FeeType.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Date_Entered = model.DateEntered;
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}