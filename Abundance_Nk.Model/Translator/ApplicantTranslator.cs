using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicantTranslator : TranslatorBase<Applicant, APPLICANT>
    {
        private readonly AbilityTranslator abilityTranslator;
        private readonly ApplicantStatusTranslator applicantStatusTranslator;
        private readonly ApplicationFormTranslator applicationFormTranslator;
        private readonly PersonTranslator personTranslator;

        public ApplicantTranslator()
        {
            personTranslator = new PersonTranslator();
            abilityTranslator = new AbilityTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            applicantStatusTranslator = new ApplicantStatusTranslator();
        }

        public override Applicant TranslateToModel(APPLICANT entity)
        {
            try
            {
                Applicant model = null;
                if (entity != null)
                {
                    model = new Applicant();
                    model.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.Ability = abilityTranslator.Translate(entity.ABILITY);
                    model.OtherAbility = entity.Other_Ability;
                    model.ExtraCurricullarActivities = entity.Extra_Curricular_Activities;
                    model.Status = applicantStatusTranslator.Translate(entity.APPLICANT_STATUS);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT TranslateToEntity(Applicant model)
        {
            try
            {
                APPLICANT entity = null;
                if (model != null)
                {
                    entity = new APPLICANT();
                    entity.Application_Form_Id = model.ApplicationForm.Id;
                    entity.Person_Id = model.Person.Id;
                    entity.Ability_Id = model.Ability.Id;
                    entity.Other_Ability = model.OtherAbility;
                    entity.Extra_Curricular_Activities = model.ExtraCurricullarActivities;
                    entity.Applicant_Status_Id = model.Status.Id;
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