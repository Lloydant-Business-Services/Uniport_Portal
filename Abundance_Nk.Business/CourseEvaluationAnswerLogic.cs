using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseEvaluationAnswerLogic : BusinessBaseLogic<CourseEvaluationAnswer, COURSE_EVALUATION_ANSWER>
    {
        public CourseEvaluationAnswerLogic()
        {
            translator = new CourseEvaluationAnswerTranslator();
        }



        public List<CourseEvaluationReport> GetCourseEvaluationReport(Programme programme, Department department, Level level, SessionSemester sessionSemester)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception("One or more criteria to get this Report is not set! Please check your input criteria selection and try again.");
                }

                List<CourseEvaluationReport> evaluationScores = new List<CourseEvaluationReport>();
                List<CourseEvaluationReport> masterList = new List<CourseEvaluationReport>();

                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);

                evaluationScores = (from sr in repository.GetBy<VW_COURSE_EVALUATION_SCORES>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
                                    select new CourseEvaluationReport
                                    {
                                        Programme = sr.Programme_Name,
                                        Department = sr.Department_Name,
                                        Level = sr.Level_Name,
                                        Faculty = sr.Faculty_Name,
                                        Session = sr.Session_Name,
                                        Semester = sr.Semester_Name,
                                        CourseCode = sr.Course_Code,
                                        CourseName = sr.Course_Name,
                                        Score = sr.Score,
                                        LecturerName = sr.User_Name,
                                        PersonId = sr.Person_Id,
                                        CourseId = sr.Course_Id

                                    }).ToList();

                List<long> distinctCourseId = evaluationScores.Select(e => e.CourseId).Distinct().ToList();

                StaffLogic staffLogic = new StaffLogic();

                for (int i = 0; i < distinctCourseId.Count; i++)
                {
                    long courseId = distinctCourseId[i];
                    List<CourseEvaluationReport> evaluationCourse = evaluationScores.Where(e => e.CourseId == courseId).ToList();

                    CourseEvaluationReport firstEvaluationCourse = evaluationCourse[0];

                    Staff staff = staffLogic.GetModelBy(s => s.USER.User_Name == firstEvaluationCourse.LecturerName);

                    int studentCount = evaluationCourse.Select(p => p.PersonId).Distinct().Count();
                    long? score = evaluationCourse.Where(e => e.Score != null).Sum(e => e.Score);

                    for (int j = 0; j < evaluationCourse.Count; j++)
                    {
                        evaluationCourse[j].NumberOfStudent = studentCount;
                        evaluationCourse[j].Score = score;
                        CourseEvaluationReport evaluation = evaluationCourse[j];
                        evaluation.LecturerName = staff != null ? staff.Person.FullName.ToUpper() : firstEvaluationCourse.LecturerName;
                    }
                    masterList.Add(evaluationCourse.FirstOrDefault());
                }

                return masterList.OrderBy(a => a.CourseCode).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public  List<CourseEvaluationAnswer> GetCourseEvaluationReport(Course course)
        {
            try
            {
                if (course == null || course.Id <= 0)
                {
                    throw new Exception("One or more criteria to get this Report is not set! Please check your input criteria selection and try again.");
                }

                CourseEvaluationAnswerLogic courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();
                var evaluationScores = courseEvaluationAnswerLogic.GetModelsBy(s => s.Course_Id == course.Id);

                return evaluationScores;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}