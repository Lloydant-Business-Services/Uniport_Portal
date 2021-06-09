using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentResultDetailLogic : BusinessBaseLogic<StudentResultDetail, STUDENT_RESULT_DETAIL>
    {
        public StudentResultDetailLogic()
        {
            translator = new StudentResultDetailTranslator();
        }

        public List<StudentResultDetail> GetBy(Student student, SessionSemester sessionSemester)
        {
            try
            {
                Expression<Func<STUDENT_RESULT_DETAIL, bool>> selector =
                    a =>
                        a.STUDENT.Matric_Number == student.MatricNumber &&
                        a.STUDENT_RESULT.Session_Semester_Id == sessionSemester.Id;
                return GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool Modify(Person Person, Course Course, SessionSemester sessionSemester, decimal? TestScore,
            decimal? ExamScore)
        {
            try
            {
                Expression<Func<STUDENT_RESULT_DETAIL, bool>> selector =
                    a =>
                        a.Person_Id == Person.Id && a.Course_Id == Course.Id &&
                        a.STUDENT_RESULT.Session_Semester_Id == sessionSemester.Id;
                List<STUDENT_RESULT_DETAIL> studentResultDetailEntity = GetEntitiesBy(selector);
                if (studentResultDetailEntity == null)
                {
                    throw new Exception(NoItemFound);
                }

                studentResultDetailEntity[0].Score = TestScore;
                if (studentResultDetailEntity.Count > 1)
                {
                    studentResultDetailEntity[1].Score = ExamScore;
                }
                int modifiedCount = Save();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
    }
}