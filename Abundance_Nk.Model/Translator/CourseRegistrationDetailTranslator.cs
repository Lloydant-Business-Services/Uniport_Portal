using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class CourseRegistrationDetailTranslator :
        TranslatorBase<CourseRegistrationDetail, STUDENT_COURSE_REGISTRATION_DETAIL>
    {
        private readonly CourseModeTranslator courseModeTranslator;
        private readonly CourseRegistrationTranslator courseRegistrationTranslator;
        private readonly CourseTranslator courseTranslator;
        private readonly SemesterTranslator semesterTranslator;

        public CourseRegistrationDetailTranslator()
        {
            courseTranslator = new CourseTranslator();
            courseModeTranslator = new CourseModeTranslator();
            semesterTranslator = new SemesterTranslator();
            courseRegistrationTranslator = new CourseRegistrationTranslator();
        }

        public override CourseRegistrationDetail TranslateToModel(STUDENT_COURSE_REGISTRATION_DETAIL entity)
        {
            try
            {
                CourseRegistrationDetail model = null;
                if (entity != null)
                {
                    model = new CourseRegistrationDetail();
                    model.Id = entity.Student_Course_Registration_Detail_Id;
                    model.CourseRegistration = courseRegistrationTranslator.Translate(entity.STUDENT_COURSE_REGISTRATION);
                    model.Course = courseTranslator.Translate(entity.COURSE);
                    model.Mode = courseModeTranslator.Translate(entity.COURSE_MODE);
                    model.Semester = semesterTranslator.Translate(entity.SEMESTER);
                    model.TestScore = entity.Test_Score;
                    model.ExamScore = entity.Exam_Score;
                    model.CourseUnit = entity.Course_Unit;
                    model.SpecialCase = entity.Special_Case;
                    //  model.DateTime = entity.Date_Uploaded.ToString();
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_COURSE_REGISTRATION_DETAIL TranslateToEntity(CourseRegistrationDetail model)
        {
            try
            {
                STUDENT_COURSE_REGISTRATION_DETAIL entity = null;
                if (model != null)
                {
                    entity = new STUDENT_COURSE_REGISTRATION_DETAIL();
                    entity.Student_Course_Registration_Detail_Id = model.Id;
                    entity.Student_Course_Registration_Id = model.CourseRegistration.Id;
                    entity.Course_Id = model.Course.Id;
                    entity.Course_Mode_Id = model.Mode.Id;
                    entity.Semester_Id = model.Semester.Id;
                    entity.Test_Score = model.TestScore;
                    entity.Exam_Score = model.ExamScore;
                    entity.Course_Unit = model.CourseUnit;
                    entity.Special_Case = model.SpecialCase;
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