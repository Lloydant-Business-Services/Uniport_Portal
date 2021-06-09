using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicantRefereeTranslator:TranslatorBase<ApplicantReferee,APPLICANT_REFEREE>
    {
        private readonly ApplicationFormTranslator applicationFormTranslator;

        public ApplicantRefereeTranslator()
        {
            applicationFormTranslator = new ApplicationFormTranslator();
        }
        public override ApplicantReferee TranslateToModel(APPLICANT_REFEREE entity)
        {
            try
            {
                ApplicantReferee model = null;
                if (entity != null)
                {
                    model = new ApplicantReferee();
                    model.Id = entity.Applicant_Referee_Id;
                    model.Name = entity.Name;
                    model.Rank = entity.Rank;
                    model.Department = entity.Department;
                    model.Institution = entity.Institution;
                    model.PhoneNo = entity.Phone;
                    model.Email = entity.Email;
                    model.ApplicationForm = applicationFormTranslator.TranslateToModel(entity.APPLICATION_FORM);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_REFEREE TranslateToEntity(ApplicantReferee model)
        {
            try
            {
                APPLICANT_REFEREE entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_REFEREE();
                    entity.Applicant_Referee_Id = model.Id;
                    entity.Name = model.Name;
                    entity.Rank = model.Rank;
                    entity.Department = model.Department;
                    entity.Institution = model.Institution;
                    entity.Phone = model.PhoneNo;
                    entity.Email = model.Email;
                    entity.Application_Form_Id = model.ApplicationForm.Id;
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
