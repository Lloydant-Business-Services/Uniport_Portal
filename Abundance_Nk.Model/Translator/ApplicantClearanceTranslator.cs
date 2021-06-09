using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicantClearanceTranslator : TranslatorBase<ApplicantClearance, APPLICANT_CLEARANCE>
    {
        private readonly ApplicationFormTranslator applicationFormTranslator;
        private UserTranslator userTranslator;

        public ApplicantClearanceTranslator()
        {
            userTranslator = new UserTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
        }

        public override ApplicantClearance TranslateToModel(APPLICANT_CLEARANCE entity)
        {
            try
            {
                ApplicantClearance model = null;
                if (entity != null)
                {
                    model = new ApplicantClearance();
                    model.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.Cleared = entity.Cleared;
                    model.DateCleared = entity.Date_Cleared;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_CLEARANCE TranslateToEntity(ApplicantClearance model)
        {
            try
            {
                APPLICANT_CLEARANCE entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_CLEARANCE();
                    entity.Application_Form_Id = model.ApplicationForm.Id;
                    entity.Cleared = model.Cleared;
                    entity.Date_Cleared = model.DateCleared;
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