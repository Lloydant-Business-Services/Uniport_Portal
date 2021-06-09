using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class JambRecordTranslator : TranslatorBase<JambRecord, JAMB_RECORD>
    {
        public override JambRecord TranslateToModel(JAMB_RECORD entity)
        {
            try
            {
                JambRecord model = null;
                if (entity != null)
                {
                    model = new JambRecord();
                    model.Id = entity.Jamb_Record_Id;
                    model.CandidateName = entity.Candidate_Name;
                    model.FirstChoiceInstitution = entity.First_Choice_Institution;
                    model.JambRegistrationNumber = entity.Jamb_Registration_Number;
                    model.Subject1 = entity.Subject1;
                    model.Subject2 = entity.Subject2;
                    model.Subject3 = entity.Subject3;
                    model.Subject4 = entity.Subject4;
                    model.TotalJambScore = entity.Total_Jamb_Score;
                    model.DepartmentId = entity.Department_Id;
                    model.ImageFileUrl = entity.Passport_Url;
                    model.LgaId = entity.Local_Government_Id;
                    model.ProgrammeId = entity.Programme_Id;
                    model.SessionId = entity.Session_Id;
                    model.SexId = entity.Sex_Id;
                    model.StateId = entity.State_Id;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override JAMB_RECORD TranslateToEntity(JambRecord model)
        {
            try
            {
                JAMB_RECORD entity = null;
                if (model != null)
                {
                    entity = new JAMB_RECORD();
                    entity.Jamb_Record_Id = model.Id;
                    entity.Candidate_Name = model.CandidateName;
                    entity.First_Choice_Institution = model.FirstChoiceInstitution;
                    entity.Jamb_Registration_Number = model.JambRegistrationNumber;
                    entity.Subject1 = model.Subject1;
                    entity.Subject2 = model.Subject2;
                    entity.Subject3 = model.Subject3;
                    entity.Subject4 = model.Subject4;
                    entity.Total_Jamb_Score = model.TotalJambScore;
                    entity.Sex_Id = model.SexId;
                    entity.Session_Id = model.SessionId;
                    entity.Programme_Id = model.ProgrammeId;
                    entity.Passport_Url = model.ImageFileUrl;
                    entity.Local_Government_Id = model.LgaId;
                    entity.Department_Id = model.DepartmentId;
                    entity.State_Id = model.StateId;
                    
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