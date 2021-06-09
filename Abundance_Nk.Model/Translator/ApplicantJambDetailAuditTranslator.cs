using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class ApplicantJambDetailAuditTranslator : TranslatorBase<ApplicantJambDetailAudit, APPLICANT_JAMB_DETAIL_AUDIT>
    {
        private readonly ApplicationFormTranslator applicationFormTranslator;
        private readonly InstitutionChoiceTranslator institutionChoiceTranslator;
        private readonly OLevelSubjectTranslator oLevelSubjectTranslator;
        private readonly PersonTranslator personTranslator;

        public ApplicantJambDetailAuditTranslator()
        {
            personTranslator = new PersonTranslator();
            institutionChoiceTranslator = new InstitutionChoiceTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            oLevelSubjectTranslator = new OLevelSubjectTranslator();
        }
        public override APPLICANT_JAMB_DETAIL_AUDIT TranslateToEntity(ApplicantJambDetailAudit model)
        {
            try
            {
                APPLICANT_JAMB_DETAIL_AUDIT entity = null;
                if (model != null)
                {
                    entity = new APPLICANT_JAMB_DETAIL_AUDIT();
                    entity.Applicant_Jamb_Audit_Id = model.Id;
                    entity.Person_Id = model.Person.Id;
                    entity.Applicant_Jamb_Registration_Number = model.JambRegistrationNumber;
                    entity.Applicant_Jamb_Score = model.JambScore;
                    entity.Action = model.Action;
                    entity.Client = model.Client;
                    entity.Operation = model.Operation;
                    entity.Time = model.Time;
                    entity.User_Id = model.User.Id;
                    if (model.Subject1 != null)
                    {
                        entity.Subject1 = model.Subject1.Id;
                    }
                    if (model.Subject2 != null)
                    {
                        entity.Subject2 = model.Subject2.Id;
                    }
                    if (model.Subject3 != null)
                    {
                        entity.Subject3 = model.Subject3.Id;
                    }
                    if (model.Subject4 != null)
                    {
                        entity.Subject4 = model.Subject4.Id;
                    }

                    if (model.InstitutionChoice != null)
                    {
                        entity.Institution_Choice_Id = model.InstitutionChoice.Id;
                    }

                    if (model.ApplicationForm != null)
                    {
                        entity.Application_Form_Id = model.ApplicationForm.Id;
                    }
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }


        }

        public override ApplicantJambDetailAudit TranslateToModel(APPLICANT_JAMB_DETAIL_AUDIT entity)
        {
            try
            {
                ApplicantJambDetailAudit model = null;
                if (entity != null)
                {
                    model = new ApplicantJambDetailAudit();
                    model.Id = entity.Applicant_Jamb_Audit_Id;
                    model.Person = new Person();
                    model.Person.Id = entity.Person_Id;
                    model.JambRegistrationNumber = entity.Applicant_Jamb_Registration_Number;
                    if(entity.Institution_Choice_Id != null && entity.Institution_Choice_Id > 0)
                    {
                        model.InstitutionChoice = new InstitutionChoice();
                        model.InstitutionChoice.Id = (int)entity.Institution_Choice_Id;
                    }
                    
                    model.JambScore = entity.Applicant_Jamb_Score;
                    if(entity.Application_Form_Id != null && entity.Application_Form_Id > 0)
                    {
                        model.ApplicationForm = new ApplicationForm();
                        model.ApplicationForm.Id = (long)entity.Application_Form_Id;
                    }
                    
                    if(entity.Subject1 != null && entity.Subject1 > 0)
                    {
                        model.Subject1 = new OLevelSubject();
                        model.Subject1.Id = (int)entity.Subject1;
                    }

                    if (entity.Subject2 != null && entity.Subject2 > 0)
                    {
                        model.Subject2 = new OLevelSubject();
                        model.Subject2.Id = (int)entity.Subject2;
                    }

                    if (entity.Subject3 != null && entity.Subject3 > 0)
                    {
                        model.Subject3 = new OLevelSubject();
                        model.Subject3.Id = (int)entity.Subject3;
                    }

                    if (entity.Subject4 != null && entity.Subject4 > 0)
                    {
                        model.Subject4 = new OLevelSubject();
                        model.Subject4.Id = (int)entity.Subject4;
                    }
                    
                    model.Client = entity.Client;
                    model.Operation = entity.Operation;
                    model.Action = entity.Action;
                    model.Time = entity.Time;
                    model.User = new User();
                    model.User.Id = entity.User_Id;
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
