using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{

    public class ApplicantRefereeResponseTranslator : TranslatorBase<ApplicantRefereeResponse, APPLICANT_REFEREE_RESPONSE>
    {
        private RefereeGradingSystemTranslator refereeGradingSystemTranslator;
        private ApplicantRefereeTranslator applicantRefereeTranslator;

        public ApplicantRefereeResponseTranslator()
        {
            applicantRefereeTranslator = new ApplicantRefereeTranslator();
            refereeGradingSystemTranslator = new RefereeGradingSystemTranslator();
        }

        public override APPLICANT_REFEREE_RESPONSE TranslateToEntity(ApplicantRefereeResponse model)
        {
            try
            {
                APPLICANT_REFEREE_RESPONSE entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_REFEREE_RESPONSE();
                    entity.Referee_Response_Id = model.RefereeResponseId;
                    entity.Applicant_Referee_Id = model.ApplicantReferee.Id;
                    entity.CanAcceptApplicant = model.CanAcceptApplicant;
                    entity.Date_Of_Response = model.DateOfResponse;
                    entity.Disclose_Response = model.DiscloseResponse;
                    entity.DurationKnownApplicant = model.DurationKnownApplicant;
                    entity.Full_Name = model.FullName;
                    entity.Overall_Rating = model.RefereeGradingSystem.Id;
                    entity.Relevant_Information = model.RelevantInformation;
                    entity.Remark = model.Remark;
                    if (model.SignatureUrl != null)
                    {
                        entity.Signature_Url = model.SignatureUrl;
                    }
                }
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override ApplicantRefereeResponse TranslateToModel(APPLICANT_REFEREE_RESPONSE entity)
        {
            try
            {
                ApplicantRefereeResponse model = null;
                if (entity != null)
                {
                    model = new ApplicantRefereeResponse();
                    model.RefereeResponseId = entity.Referee_Response_Id;
                    model.CanAcceptApplicant = entity.CanAcceptApplicant;
                    model.DateOfResponse = entity.Date_Of_Response;
                    model.DiscloseResponse = entity.Disclose_Response;
                    model.DurationKnownApplicant = entity.DurationKnownApplicant;
                    model.FullName = entity.Full_Name;
                    model.RelevantInformation = entity.Relevant_Information;
                    model.Remark = entity.Remark;
                    model.SignatureUrl = entity.Signature_Url;

                    model.RefereeGradingSystem = refereeGradingSystemTranslator.Translate(entity.REFEREE_GRADING_SYSTEM);
                    model.ApplicantReferee = applicantRefereeTranslator.Translate(entity.APPLICANT_REFEREE);

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
