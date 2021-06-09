using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class PreviousEducationLogic : BusinessBaseLogic<PreviousEducation, APPLICANT_PREVIOUS_EDUCATION>
    {
        public PreviousEducationLogic()
        {
            translator = new PreviousEducationTranslator();
        }

        public bool Modify(PreviousEducation previousEducation)
        {
            try
            {
                Expression<Func<APPLICANT_PREVIOUS_EDUCATION, bool>> selector =
                    p => p.Applicant_Previous_Education_Id == previousEducation.Id;
                APPLICANT_PREVIOUS_EDUCATION entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }


                if (previousEducation.Person != null && previousEducation.Person.Id > 0)
                {
                    entity.Person_Id = previousEducation.Person.Id;
                }
                if (previousEducation.SchoolName != null)
                {
                    entity.Previous_School_Name = previousEducation.SchoolName;
                }

                if (previousEducation.Course != null)
                {
                    entity.Previous_Course = previousEducation.Course;
                }

                if (previousEducation.StartDate != null)
                {
                    entity.Previous_Education_Start_Date = previousEducation.StartDate;
                }

                if (previousEducation.EndDate != null)
                {
                    entity.Previous_Education_End_Date = previousEducation.EndDate;
                }

                if (previousEducation.Qualification != null && previousEducation.Qualification.Id > 0)
                {
                    entity.Educational_Qualification_Id = previousEducation.Qualification.Id;
                }

                if (previousEducation.ResultGrade != null && previousEducation.ResultGrade.Id > 0)
                {
                    entity.Result_Grade_Id = previousEducation.ResultGrade.Id;
                }

                if (previousEducation.ITDuration != null && previousEducation.ITDuration.Id > 0)
                {
                    entity.IT_Duration_Id = previousEducation.ITDuration.Id;
                }

                if (previousEducation.ApplicationForm != null && previousEducation.ApplicationForm.Id > 0)
                {
                    entity.Application_Form_Id = previousEducation.ApplicationForm.Id;
                }
                entity.Previous_Matric_No = previousEducation.MatricNumber;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}