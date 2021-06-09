using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicantRefereeGradingResponseTranslator : TranslatorBase<ApplicantRefereeGradingResponse, APPLICANT_REFEREE_GRADING_RESPONSE>
    {
        private RefereeGradingSystemTranslator refereeGradingSystemTranslator;
        private RefereeGradingCategoryTranslator refereeGradingCategoryTranslator;
        private ApplicantRefereeResponseTranslator applicantRefereeResponseTranslator;

        public ApplicantRefereeGradingResponseTranslator()
        {
            refereeGradingSystemTranslator = new RefereeGradingSystemTranslator();
            refereeGradingCategoryTranslator = new RefereeGradingCategoryTranslator();
            applicantRefereeResponseTranslator = new ApplicantRefereeResponseTranslator();
        }

        public override APPLICANT_REFEREE_GRADING_RESPONSE TranslateToEntity(ApplicantRefereeGradingResponse model)
        {
            try
            {
                APPLICANT_REFEREE_GRADING_RESPONSE entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_REFEREE_GRADING_RESPONSE();
                    entity.Id = model.Id;
                    entity.Referee_Grading_System_Id = model.RefereeGradingSystem.Id;
                    entity.Referee_Grading_Category_Id = model.RefereeGradingCategory.Id;
                    entity.Applicant_Response_Id = model.ApplicantRefereeResponse.RefereeResponseId;
                    
                }
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override ApplicantRefereeGradingResponse TranslateToModel(APPLICANT_REFEREE_GRADING_RESPONSE entity)
        {
            try
            {
                ApplicantRefereeGradingResponse model = null;
                if (entity != null)
                {
                    model = new ApplicantRefereeGradingResponse();
                    model.Id = entity.Id;
                    model.RefereeGradingCategory = refereeGradingCategoryTranslator.Translate(entity.REFEREE_GRADING_CATEGORY);
                    model.RefereeGradingSystem = refereeGradingSystemTranslator.Translate(entity.REFEREE_GRADING_SYSTEM);
                    model.ApplicantRefereeResponse = applicantRefereeResponseTranslator.Translate(entity.APPLICANT_REFEREE_RESPONSE);

                }
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
