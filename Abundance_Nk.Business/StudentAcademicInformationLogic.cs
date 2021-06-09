using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentAcademicInformationLogic :
        BusinessBaseLogic<StudentAcademicInformation, STUDENT_ACADEMIC_INFORMATION>
    {
        public StudentAcademicInformationLogic()
        {
            translator = new StudentAcademicInformationTranslator();
        }


        public bool Modify(StudentAcademicInformation studentAcademicInformation)
        {
            try
            {
                Expression<Func<STUDENT_ACADEMIC_INFORMATION, bool>> selector =
                    p => p.Person_Id == studentAcademicInformation.Student.Id;
                STUDENT_ACADEMIC_INFORMATION entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Person_Id = studentAcademicInformation.Student.Id;
                entity.Year_Of_Admission = studentAcademicInformation.YearOfAdmission;
                entity.Year_Of_Graduation = studentAcademicInformation.YearOfGraduation;

                if (studentAcademicInformation.Level != null && studentAcademicInformation.Level.Id > 0)
                {
                    entity.Level_Id = studentAcademicInformation.Level.Id;
                }

                if (studentAcademicInformation.ModeOfEntry != null && studentAcademicInformation.ModeOfEntry.Id > 0)
                {
                    entity.Mode_Of_Entry_Id = studentAcademicInformation.ModeOfEntry.Id;
                }
                if (studentAcademicInformation.ModeOfStudy != null && studentAcademicInformation.ModeOfStudy.Id > 0)
                {
                    entity.Mode_Of_Study_Id = studentAcademicInformation.ModeOfStudy.Id;
                }
                if(studentAcademicInformation.DateEntered != null && studentAcademicInformation.DateEntered.Value.Year > 1)
                {
                    entity.Date_Entered = studentAcademicInformation.DateEntered;
                }
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