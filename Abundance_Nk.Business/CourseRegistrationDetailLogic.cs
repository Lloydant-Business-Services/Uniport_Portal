using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Business
{
    public class CourseRegistrationDetailLogic :
        BusinessBaseLogic<CourseRegistrationDetail, STUDENT_COURSE_REGISTRATION_DETAIL>
    {
        private readonly SessionSemesterLogic sessionSemesterLogic;

        public CourseRegistrationDetailLogic()
        {
            translator = new CourseRegistrationDetailTranslator();
            sessionSemesterLogic = new SessionSemesterLogic();
        }


        public bool UpdateCourseRegistrationScore(List<StudentResultDetail> results, CourseRegistrationDetailAudit courseRegistrationDetailAudit)
        {
            try
            {
                CourseRegistrationDetailAuditLogic courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();
                List<STUDENT_COURSE_REGISTRATION_DETAIL> registeredCourseEntities = new List<STUDENT_COURSE_REGISTRATION_DETAIL>();
                SessionSemester sessionSemester = sessionSemesterLogic.GetBy(results[0].Header.SessionSemester.Id);
                foreach (StudentResultDetail result in results)
                {
                    STUDENT_COURSE_REGISTRATION_DETAIL registeredCourseEntity = GetBy(result.Student, result.Header.Level, result.Header.Programme, result.Header.Department, result.Course, sessionSemester.Session, sessionSemester.Semester);
                    if (registeredCourseEntity==null)
                    {
                        registeredCourseEntity = GetBy(result.Student, result.Header.Level, result.Header.Programme, result.Course, sessionSemester.Session, sessionSemester.Semester);
                    }
                    if (registeredCourseEntity != null && registeredCourseEntity.Student_Course_Registration_Detail_Id > 0)
                    {
                        courseRegistrationDetailAudit.Course = new Course() { Id = registeredCourseEntity.Course_Id };
                        courseRegistrationDetailAudit.CourseRegistration = new CourseRegistration() { Id = (long)registeredCourseEntity.Student_Course_Registration_Id };
                        courseRegistrationDetailAudit.CourseUnit = registeredCourseEntity.Course_Unit;
                        courseRegistrationDetailAudit.Mode = new CourseMode() { Id = registeredCourseEntity.Course_Mode_Id };
                        courseRegistrationDetailAudit.Semester = new Semester() { Id = registeredCourseEntity.Semester_Id };
                        courseRegistrationDetailAudit.CourseRegistrationDetail = new CourseRegistrationDetail() { Id = registeredCourseEntity.Student_Course_Registration_Detail_Id };

                        if (result.Header.Type.Id == 1)
                        {
                            courseRegistrationDetailAudit.TestScore = result.Score;
                            registeredCourseEntity.Test_Score = result.Score;
                            registeredCourseEntity.Special_Case = result.SpecialCaseMessage;
                        }
                        else if (result.Header.Type.Id == 2)
                        {
                            courseRegistrationDetailAudit.ExamScore = result.Score;
                            courseRegistrationDetailAudit.SpecialCase = result.SpecialCaseMessage;
                            registeredCourseEntity.Exam_Score = result.Score;
                            registeredCourseEntity.Special_Case = result.SpecialCaseMessage;
                        }

                        registeredCourseEntities.Add(registeredCourseEntity);
                        courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);
                    }
                }

                // return Save() > 0 ? true : false;
                Save();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateCourseRegistrationScore(List<CourseRegistrationDetail> results, CourseRegistrationDetailAudit courseRegistrationDetailAudit)
        {
            try
            {
                List<STUDENT_COURSE_REGISTRATION_DETAIL> registeredCourseEntities = new List<STUDENT_COURSE_REGISTRATION_DETAIL>();
                foreach (CourseRegistrationDetail result in results)
                {
                    STUDENT_COURSE_REGISTRATION_DETAIL registeredCourseEntity = GetEntityBy(a => a.Student_Course_Registration_Detail_Id == result.Id);
                    if (registeredCourseEntity != null && registeredCourseEntity.Student_Course_Registration_Detail_Id > 0)
                    {
                        string strTestScore = result.TestScore.ToString();
                        string strExamScore = result.ExamScore.ToString();
                        if (result.TestScore == null || result.ExamScore == null)
                        {
                            continue;
                        }
                        registeredCourseEntity.Test_Score = result.TestScore;
                        registeredCourseEntity.Exam_Score = result.ExamScore;
                        if (result.TestScore == 101 || result.ExamScore == 101)
                        {
                            SessionLogic sessionLogic = new SessionLogic();
                            SESSION session = sessionLogic.GetEntityBy(p => p.Activated == true);
                            List<STUDENT_COURSE_REGISTRATION_DETAIL> courseRegDetailCheck = GetEntitiesBy(p => p.Course_Id == registeredCourseEntity.Course_Id && p.Semester_Id == registeredCourseEntity.Semester_Id && p.STUDENT_COURSE_REGISTRATION.Session_Id == session.Session_Id && p.STUDENT_COURSE_REGISTRATION.Person_Id == registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Person_Id);
                            if (courseRegDetailCheck != null)
                            {
                                //bool isCarryOverDeleted = Delete(p => p.Course_Id == registeredCourseEntity.Course_Id && p.Semester_Id == registeredCourseEntity.Semester_Id && p.STUDENT_COURSE_REGISTRATION.Session_Id == session.Session_Id && p.Student_Course_Registration_Id == registeredCourseEntity.Student_Course_Registration_Id);
                                //bool isCurrenCourseDeleted = Delete(p => p.Student_Course_Registration_Detail_Id == registeredCourseEntity.Student_Course_Registration_Detail_Id);

                                if (registeredCourseEntity.Test_Score == 101)
                                {
                                    registeredCourseEntity.Test_Score = 0;
                                }
                                if (registeredCourseEntity.Exam_Score == 101)
                                {
                                    registeredCourseEntity.Exam_Score = 0;
                                }

                                registeredCourseEntity.Special_Case = "SICK";
                                registeredCourseEntities.Add(registeredCourseEntity);
                            }
                        }
                        else if (result.TestScore == 201 || result.ExamScore == 201)
                        {
                            SessionLogic sessionLogic = new SessionLogic();
                            SESSION session = sessionLogic.GetEntityBy(p => p.Activated == true);
                            List<STUDENT_COURSE_REGISTRATION_DETAIL> courseRegDetailCheck = GetEntitiesBy(p => p.Course_Id == registeredCourseEntity.Course_Id && p.Semester_Id == registeredCourseEntity.Semester_Id && p.STUDENT_COURSE_REGISTRATION.Session_Id == session.Session_Id && p.STUDENT_COURSE_REGISTRATION.Person_Id == registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Person_Id);
                            if (courseRegDetailCheck != null)
                            {
                                //bool isCarryOverDeleted = Delete(p => p.Course_Id == registeredCourseEntity.Course_Id && p.Semester_Id == registeredCourseEntity.Semester_Id && p.STUDENT_COURSE_REGISTRATION.Session_Id == session.Session_Id && p.Student_Course_Registration_Id == registeredCourseEntity.Student_Course_Registration_Id);
                                //bool isCurrenCourseDeleted = Delete(p => p.Student_Course_Registration_Detail_Id == registeredCourseEntity.Student_Course_Registration_Detail_Id);

                                if (registeredCourseEntity.Test_Score == 201)
                                {
                                    registeredCourseEntity.Test_Score = 0;
                                }
                                if (registeredCourseEntity.Exam_Score == 201)
                                {
                                    registeredCourseEntity.Exam_Score = 0;
                                }

                                registeredCourseEntity.Special_Case = "ABSENT";
                                registeredCourseEntities.Add(registeredCourseEntity);
                            }
                        }
                        else
                        {
                            if ((result.TestScore + result.ExamScore) > 0)
                            {
                                registeredCourseEntity.Special_Case = null;
                            }
                            else
                            {
                                registeredCourseEntity.Special_Case = result.SpecialCase;
                            }

                            registeredCourseEntities.Add(registeredCourseEntity);
                        }

                        StudentExamRawScoreSheetResultLogic studentExamRawScoreSheetResultLogic = new StudentExamRawScoreSheetResultLogic();

                        if (registeredCourseEntity.STUDENT_COURSE_REGISTRATION != null)
                        {


                            STUDENT_EXAM_RAW_SCORE_SHEET_RESULT rawScoreEntity =
                                new STUDENT_EXAM_RAW_SCORE_SHEET_RESULT();
                            rawScoreEntity =
                                studentExamRawScoreSheetResultLogic.GetEntitiesBy(
                                    a =>
                                        a.Course_Id == registeredCourseEntity.Course_Id &&
                                        a.Student_Id == registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Person_Id)
                                    .FirstOrDefault();

                            if (rawScoreEntity != null)
                            {
                                rawScoreEntity.QU1 = (double)result.ExamScore;
                                rawScoreEntity.T_EX = (double)result.ExamScore;
                                rawScoreEntity.T_CA = (double)result.TestScore;
                                rawScoreEntity.EX_CA = (double)result.TestScore + (double)result.ExamScore;
                            }
                            else
                            {
                                rawScoreEntity = new STUDENT_EXAM_RAW_SCORE_SHEET_RESULT();
                                rawScoreEntity.Course_Id = registeredCourseEntity.Course_Id;
                                rawScoreEntity.Student_Id = registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Person_Id;
                                rawScoreEntity.Session_Id =
                                    registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Session_Id;
                                rawScoreEntity.Semester_Id = registeredCourseEntity.Semester_Id;
                                rawScoreEntity.Level_Id = registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Level_Id;
                                rawScoreEntity.Uploader_Id = 1;
                                rawScoreEntity.QU1 = (double)result.ExamScore;
                                rawScoreEntity.T_EX = (double)result.ExamScore;
                                rawScoreEntity.T_CA = (double)result.TestScore;
                                rawScoreEntity.EX_CA = (double)result.TestScore + (double)result.ExamScore;

                            }
                            var courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();
                            courseRegistrationDetailAudit.Course = new Course();
                            courseRegistrationDetailAudit.CourseRegistration = new CourseRegistration();
                            courseRegistrationDetailAudit.CourseRegistrationDetail = new CourseRegistrationDetail();
                            courseRegistrationDetailAudit.Mode = new CourseMode();
                            courseRegistrationDetailAudit.Semester = new Semester();
                            courseRegistrationDetailAudit.Course.Id = registeredCourseEntity.Course_Id;
                            courseRegistrationDetailAudit.CourseRegistration.Id =
                                (long)registeredCourseEntity.Student_Course_Registration_Id;
                            courseRegistrationDetailAudit.CourseUnit = registeredCourseEntity.Course_Unit;
                            courseRegistrationDetailAudit.Mode.Id = registeredCourseEntity.Course_Mode_Id;
                            courseRegistrationDetailAudit.TestScore = registeredCourseEntity.Test_Score;
                            courseRegistrationDetailAudit.ExamScore = registeredCourseEntity.Exam_Score;
                            courseRegistrationDetailAudit.SpecialCase = registeredCourseEntity.Special_Case;
                            courseRegistrationDetailAudit.CourseRegistrationDetail.Id =
                                registeredCourseEntity.Student_Course_Registration_Detail_Id;
                            courseRegistrationDetailAudit.Semester.Id = registeredCourseEntity.Semester_Id;
                            courseRegistrationDetailAudit.Time = DateTime.Now;
                            courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);
                            //StudentResultDetailLogic resultDetailLogic = new StudentResultDetailLogic();
                            //SessionSemester sessionSemeter = sessionSemesterLogic.GetModelBy(a => a.Session_Id == registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Session_Id && a.Semester_Id == registeredCourseEntity.Semester_Id);
                            //Person person = new Person() { Id = registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Person_Id };
                            //Course course = new Course() { Id = registeredCourseEntity.Course_Id };

                            //resultDetailLogic.Modify(person, course, sessionSemeter, registeredCourseEntity.Test_Score, registeredCourseEntity.Exam_Score);
                        }
                    }
                }

                Save();
                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public CourseRegistrationDetail Create(CourseRegistrationDetail courseRegistrationDetail, CourseRegistrationDetailAudit courseRegistrationDetailAudit)
        {
            CourseRegistrationDetail detail = new CourseRegistrationDetail();
            try
            {

                CourseRegistrationDetailAuditLogic courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();
                detail = base.Create(courseRegistrationDetail);
                if (courseRegistrationDetailAudit.Client != null && courseRegistrationDetailAudit.Action != null)
                {
                    courseRegistrationDetailAudit.Course = courseRegistrationDetail.Course;
                    courseRegistrationDetailAudit.CourseRegistration = courseRegistrationDetail.CourseRegistration;
                    courseRegistrationDetailAudit.CourseRegistrationDetail = detail;
                    courseRegistrationDetailAudit.CourseUnit = courseRegistrationDetail.CourseUnit;
                    courseRegistrationDetailAudit.SpecialCase = courseRegistrationDetail.SpecialCase;
                    courseRegistrationDetailAudit.TestScore = courseRegistrationDetail.TestScore;
                    courseRegistrationDetailAudit.ExamScore = courseRegistrationDetail.ExamScore;
                    courseRegistrationDetailAudit.Mode = courseRegistrationDetail.Mode;
                    courseRegistrationDetailAudit.Semester = courseRegistrationDetail.Semester;
                    courseRegistrationDetailAudit.Time = DateTime.Now;

                    courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);
                }
                return detail;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int Create(List<CourseRegistrationDetail> courseRegistrationDetaillList, CourseRegistrationDetailAudit courseRegistrationDetailAudit)
        {
            try
            {
                var courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();
                foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationDetaillList)
                {
                    var detail = new CourseRegistrationDetail();
                    detail = base.Create(courseRegistrationDetail);
                    if (courseRegistrationDetailAudit.Client != null && courseRegistrationDetailAudit.Action != null)
                    {
                        courseRegistrationDetailAudit.Time = DateTime.Now;
                        courseRegistrationDetailAudit.Course = courseRegistrationDetail.Course;
                        courseRegistrationDetailAudit.CourseRegistration = courseRegistrationDetail.CourseRegistration;
                        courseRegistrationDetailAudit.CourseRegistrationDetail = detail;
                        courseRegistrationDetailAudit.CourseUnit = courseRegistrationDetail.CourseUnit;
                        courseRegistrationDetailAudit.SpecialCase = courseRegistrationDetail.SpecialCase;
                        courseRegistrationDetailAudit.TestScore = courseRegistrationDetail.TestScore;
                        courseRegistrationDetailAudit.ExamScore = courseRegistrationDetail.ExamScore;
                        courseRegistrationDetailAudit.Mode = courseRegistrationDetail.Mode;
                        courseRegistrationDetailAudit.Semester = courseRegistrationDetail.Semester;
                        courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);
                    }


                }
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
       public bool UpdateCourseRegistrationScore(List<StudentResultDetail> results)
        {
            try
            {
                var registeredCourseEntities = new List<STUDENT_COURSE_REGISTRATION_DETAIL>();
                SessionSemester sessionSemester = sessionSemesterLogic.GetBy(results[0].Header.SessionSemester.Id);
                foreach (StudentResultDetail result in results)
                {
                    STUDENT_COURSE_REGISTRATION_DETAIL registeredCourseEntity = GetBy(result.Student,
                        result.Header.Level, result.Header.Programme, result.Header.Department, result.Course,
                        sessionSemester.Session, sessionSemester.Semester);
                    if (registeredCourseEntity == null)
                    {
                        registeredCourseEntity = GetBy(result.Student,
                        result.Header.Level, result.Header.Programme, result.Course,
                        sessionSemester.Session, sessionSemester.Semester);
                    }
                    if (registeredCourseEntity != null &&
                        registeredCourseEntity.Student_Course_Registration_Detail_Id > 0)
                    {
                        if (result.Header.Type.Id == 1)
                        {
                            registeredCourseEntity.Test_Score = result.Score;
                            registeredCourseEntity.Special_Case = result.SpecialCaseMessage;
                        }
                        else if (result.Header.Type.Id == 2)
                        {
                            registeredCourseEntity.Exam_Score = result.Score;
                            registeredCourseEntity.Special_Case = result.SpecialCaseMessage;
                        }

                        registeredCourseEntities.Add(registeredCourseEntity);
                    }
                }

                // return Save() > 0 ? true : false;
                Save();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateCourseRegistrationScore(List<CourseRegistrationDetail> results)
        {
            try
            {
                var registeredCourseEntities = new List<STUDENT_COURSE_REGISTRATION_DETAIL>();
                foreach (CourseRegistrationDetail result in results)
                {
                    STUDENT_COURSE_REGISTRATION_DETAIL registeredCourseEntity =
                        GetEntityBy(a => a.Student_Course_Registration_Detail_Id == result.Id);
                    if (registeredCourseEntity != null &&
                        registeredCourseEntity.Student_Course_Registration_Detail_Id > 0)
                    {
                        string strTestScore = result.TestScore.ToString();
                        string strExamScore = result.ExamScore.ToString();
                        registeredCourseEntity.Test_Score = result.TestScore;
                        registeredCourseEntity.Exam_Score = result.ExamScore;
                        if (result.TestScore == 101 && result.ExamScore == 101)
                        {
                            var sessionLogic = new SessionLogic();
                            SESSION session = sessionLogic.GetEntityBy(p => p.Activated == true);
                            List<STUDENT_COURSE_REGISTRATION_DETAIL> courseRegDetailCheck =
                                GetEntitiesBy(
                                    p =>
                                        p.Course_Id == registeredCourseEntity.Course_Id &&
                                        p.Semester_Id == registeredCourseEntity.Semester_Id &&
                                        p.STUDENT_COURSE_REGISTRATION.Session_Id == session.Session_Id &&
                                        p.STUDENT_COURSE_REGISTRATION.Person_Id ==
                                        registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Person_Id);
                            if (courseRegDetailCheck != null)
                            {
                                bool isCarryOverDeleted =
                                    Delete(
                                        p =>
                                            p.Course_Id == registeredCourseEntity.Course_Id &&
                                            p.Semester_Id == registeredCourseEntity.Semester_Id &&
                                            p.STUDENT_COURSE_REGISTRATION.Session_Id == session.Session_Id);
                                bool isCurrenCourseDeleted =
                                    Delete(
                                        p =>
                                            p.Student_Course_Registration_Detail_Id ==
                                            registeredCourseEntity.Student_Course_Registration_Detail_Id);
                            }
                        }
                        else
                        {
                            if ((result.TestScore + result.ExamScore) > 0)
                            {
                                registeredCourseEntity.Special_Case = null;
                            }

                            else
                            {
                                registeredCourseEntity.Special_Case = result.SpecialCase;
                            }
                            registeredCourseEntities.Add(registeredCourseEntity);
                        }


                        //StudentResultDetailLogic resultDetailLogic = new StudentResultDetailLogic();
                        //SessionSemester sessionSemeter = sessionSemesterLogic.GetModelBy(a => a.Session_Id == registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Session_Id && a.Semester_Id == registeredCourseEntity.Semester_Id);
                        //Person person = new Person() { Id = registeredCourseEntity.STUDENT_COURSE_REGISTRATION.Person_Id };
                        //Course course = new Course() { Id = registeredCourseEntity.Course_Id };

                        //resultDetailLogic.Modify(person, course, sessionSemeter, registeredCourseEntity.Test_Score, registeredCourseEntity.Exam_Score);
                    }
                }

                Save();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private STUDENT_COURSE_REGISTRATION_DETAIL GetBy(Student student, Level level, Programme programme,
            Department department, Course course, Session session, Semester semester)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                    cr =>
                        cr.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id &&
                        cr.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id &&
                        cr.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && cr.Course_Id == course.Id &&
                        cr.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id && cr.Semester_Id == semester.Id;
                return base.GetEntityBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private STUDENT_COURSE_REGISTRATION_DETAIL GetBy(Student student, Level level, Programme programme,
     Course course, Session session, Semester semester)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                    cr =>
                        cr.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id &&
                        cr.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id &&
                        cr.Course_Id == course.Id &&
                        cr.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id && cr.Semester_Id == semester.Id;
                return base.GetEntityBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CourseRegistrationDetail> GetBy(CourseRegistration courseRegistration)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                    crd => crd.Student_Course_Registration_Id == courseRegistration.Id;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CourseRegistrationDetail> GetBy(Student student, Course course, Level level, Session session,
            Semester semester)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                    crd =>
                        crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && crd.Course_Id == course.Id &&
                        crd.STUDENT_COURSE_REGISTRATION.Level_Id == level.Id &&
                        crd.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id && crd.Semester_Id == semester.Id;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

          public List<CourseRegistrationDetail> GetBy(Student student,Session session,
            Semester semester, CourseMode courseMode)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                    crd =>
                        crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id &&
                        crd.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id && 
                        crd.Semester_Id == semester.Id && 
                        crd.Course_Mode_Id == courseMode.Id;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

       

        public List<CourseRegistrationDetail> GetResultsBy(Student student, SessionSemester sessionSemester)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                    crd =>
                        crd.STUDENT_COURSE_REGISTRATION.STUDENT.Matric_Number == student.MatricNumber &&
                        crd.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id &&
                        crd.Semester_Id == sessionSemester.Semester.Id;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ELearningView> GetEcontentBy(Level level, Semester semester, Session session, Student student)
        {
             List<ELearningView> eLearningContent;
            try
            {
                eLearningContent = (from p in repository.GetBy<VW_ELEARNING_COURSE_CONTENT>(p => p.Level_Id == level.Id && p.Semester_Id == semester.Id && p.Session_Id == session.Id && p.Person_Id == student.Id)
                                select new ELearningView
                                {
                                    StudentCourseRegistrationDetailId = p.Student_Course_Registration_Detail_Id,
                                    CourseId = p.Course_Id,
                                    SessionId = p.Session_Id,
                                    SemesterId =  p.Semester_Id,
                                    EContentTypeId =  p.E_Content_Type_Id,
                                    Views = p.Views,
                                    Url = p.Url,
                                    Name =  p.Name,
                                    Description = p.Description,
                                    Active = p.Active,
                                    LevelId = p.Level_Id,
                                    PersonId = p.Person_Id,
                                    CourseCode = p.Course_Code,
                                    CourseName = p.Course_Name,

                                }).OrderBy(b => b.SessionId).ToList();

               
            }
            catch (Exception)
            {
                throw;
            }
            return eLearningContent;
        }

        public List<Course> GetCarryOverCoursesBy(CourseRegistration courseRegistration, Semester semester)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                    cr =>
                        cr.Student_Course_Registration_Id == courseRegistration.Id && cr.Course_Mode_Id == 2 &&
                        cr.Semester_Id == semester.Id;
                List<CourseRegistrationDetail> courseRegistrationDetails = GetModelsBy(selector);

                List<Course> courses = null;
                if (courseRegistrationDetails != null && courseRegistrationDetails.Count > 0)
                {
                    courses = new List<Course>();
                    foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationDetails)
                    {
                        courses.Add(courseRegistrationDetail.Course);
                    }
                }

                return courses;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CourseRegistrationDetail> GetCarryOverBy(Student student, Session session)
        {
            try
            {

                List<CourseRegistrationDetail> carryOverCourses = (from cr in repository.GetBy<VW_STUDENT_REGISTERED_COURSE_CARRYOVER>(cr => cr.Person_Id == student.Id && cr.Session_Id != session.Id)
                                                                   select new CourseRegistrationDetail
                                                                   {
                                                                       Id = cr.Student_Course_Registration_Detail_Id,

                                                                   }).ToList();

                List<CourseRegistrationDetail> courseRegistrationDetails = null;
                if (carryOverCourses != null && carryOverCourses.Count > 0)
                {
                    int totalUnit = 0;
                    courseRegistrationDetails = new List<CourseRegistrationDetail>();
                    foreach (CourseRegistrationDetail carryOverCourse in carryOverCourses)
                    {
                        Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector = cr => cr.Student_Course_Registration_Detail_Id == carryOverCourse.Id;
                        CourseRegistrationDetail courseRegistrationDetail = GetModelBy(selector);

                        if (courseRegistrationDetail != null)
                        {
                            totalUnit += courseRegistrationDetail.Course.Unit;
                            courseRegistrationDetail.Mode = new CourseMode() { Id = 2 };

                            int countAlreadyAddedCourse = 0;
                            countAlreadyAddedCourse = courseRegistrationDetails.Count(c => c.Course.Id == courseRegistrationDetail.Course.Id);

                            if (countAlreadyAddedCourse >= 1)
                            {
                                //Do nothing
                            }
                            else
                            {
                                courseRegistrationDetails.Add(courseRegistrationDetail);
                            }
                        }
                    }

                    //BS::16/1/2017
                    List<CourseRegistrationDetail> cleardCourses = new List<CourseRegistrationDetail>();

                    for (int i = 0; i < courseRegistrationDetails.Count; i++)
                    {
                        CourseRegistrationDetail RegDetail = courseRegistrationDetails[i];
                        List<CourseRegistrationDetail> RegDetailsToCheck = GetModelsBy(c => c.COURSE.Course_Code.Trim() == RegDetail.Course.Code.Trim() && c.STUDENT_COURSE_REGISTRATION.Person_Id == RegDetail.CourseRegistration.Student.Id).ToList();
                        if (RegDetailsToCheck != null && RegDetailsToCheck.Count > 0)
                        {
                            for (int j = 0; j < RegDetailsToCheck.Count; j++)
                            {
                                if (RegDetailsToCheck[j].TestScore + RegDetailsToCheck[j].ExamScore >= 40)
                                {
                                    List<CourseRegistrationDetail> RegDetails = courseRegistrationDetails.Where(c => c.Course.Code.Trim() == RegDetailsToCheck[j].Course.Code.Trim()).ToList();
                                    foreach (CourseRegistrationDetail courseRegistrationDetail in RegDetails)
                                    {
                                        cleardCourses.Add(courseRegistrationDetail);
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < cleardCourses.Count; i++)
                    {
                        courseRegistrationDetails.Remove(cleardCourses[i]);
                    }
                }

                return courseRegistrationDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<CourseRegistrationDetail> GetCarryOverBy(Student student, Session session, Semester semester)
        {
            try
            {

                List<CourseRegistrationDetail> carryOverCourses = (from cr in repository.GetBy<VW_STUDENT_REGISTERED_COURSE_CARRYOVER>(cr => cr.Person_Id == student.Id && cr.Session_Id != session.Id)
                                                                   select new CourseRegistrationDetail
                                                                   {
                                                                       Id = cr.Student_Course_Registration_Detail_Id,

                                                                   }).ToList();

                List<CourseRegistrationDetail> courseRegistrationDetails = null;
                if (carryOverCourses != null && carryOverCourses.Count > 0)
                {
                    int totalUnit = 0;
                    courseRegistrationDetails = new List<CourseRegistrationDetail>();
                    foreach (CourseRegistrationDetail carryOverCourse in carryOverCourses)
                    {
                        Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector = cr => cr.Student_Course_Registration_Detail_Id == carryOverCourse.Id && cr.Semester_Id == semester.Id;
                        CourseRegistrationDetail courseRegistrationDetail = GetModelBy(selector);

                        if (courseRegistrationDetail != null)
                        {
                            totalUnit += courseRegistrationDetail.Course.Unit;
                            courseRegistrationDetail.Mode = new CourseMode() { Id = 2 };

                            int countAlreadyAddedCourse = 0;
                            countAlreadyAddedCourse = courseRegistrationDetails.Count(c => c.Course.Id == courseRegistrationDetail.Course.Id);

                            if (countAlreadyAddedCourse >= 1)
                            {
                                //Do nothing
                            }
                            else
                            {
                                courseRegistrationDetails.Add(courseRegistrationDetail);
                            }
                        }
                    }

                    //BS::16/1/2017
                    List<CourseRegistrationDetail> cleardCourses = new List<CourseRegistrationDetail>();

                    for (int i = 0; i < courseRegistrationDetails.Count; i++)
                    {
                        CourseRegistrationDetail RegDetail = courseRegistrationDetails[i];
                        List<CourseRegistrationDetail> RegDetailsToCheck = GetModelsBy(c => c.COURSE.Course_Code.Trim() == RegDetail.Course.Code.Trim() && c.STUDENT_COURSE_REGISTRATION.Person_Id == RegDetail.CourseRegistration.Student.Id).ToList();
                        if (RegDetailsToCheck != null && RegDetailsToCheck.Count > 0)
                        {
                            for (int j = 0; j < RegDetailsToCheck.Count; j++)
                            {
                                if (RegDetailsToCheck[j].TestScore + RegDetailsToCheck[j].ExamScore >= 44)
                                {
                                    List<CourseRegistrationDetail> RegDetails = courseRegistrationDetails.Where(c => c.Course.Code.Trim() == RegDetailsToCheck[j].Course.Code.Trim()).ToList();
                                    foreach (CourseRegistrationDetail courseRegistrationDetail in RegDetails)
                                    {
                                        cleardCourses.Add(courseRegistrationDetail);
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < cleardCourses.Count; i++)
                    {
                        courseRegistrationDetails.Remove(cleardCourses[i]);
                    }
                }

                return courseRegistrationDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CourseRegistrationDetail> GetCarryOverBy(Student student, Session session, Semester semester,bool UseNewGradingSystem)
        {
            try
            {

                List<CourseRegistrationDetail> carryOverCourses = (from cr in repository.GetBy<VW_STUDENT_REGISTERED_COURSE_CARRYOVER>(cr => cr.Person_Id == student.Id && cr.Session_Id != session.Id)
                                                                   select new CourseRegistrationDetail
                                                                   {
                                                                       Id = cr.Student_Course_Registration_Detail_Id,

                                                                   }).ToList();

                List<CourseRegistrationDetail> courseRegistrationDetails = null;
                if (carryOverCourses != null && carryOverCourses.Count > 0)
                {
                    int totalUnit = 0;
                    courseRegistrationDetails = new List<CourseRegistrationDetail>();
                    foreach (CourseRegistrationDetail carryOverCourse in carryOverCourses)
                    {
                        Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector = cr => cr.Student_Course_Registration_Detail_Id == carryOverCourse.Id && cr.Semester_Id == semester.Id;
                        CourseRegistrationDetail courseRegistrationDetail = GetModelBy(selector);

                        if (courseRegistrationDetail != null)
                        {
                            totalUnit += courseRegistrationDetail.Course.Unit;
                            courseRegistrationDetail.Mode = new CourseMode() { Id = 2 };

                            int countAlreadyAddedCourse = 0;
                            countAlreadyAddedCourse = courseRegistrationDetails.Count(c => c.Course.Id == courseRegistrationDetail.Course.Id);

                            if (countAlreadyAddedCourse >= 1)
                            {
                                //Do nothing
                            }
                            else
                            {
                                courseRegistrationDetails.Add(courseRegistrationDetail);
                            }
                        }
                    }

                    //BS::16/1/2017
                    List<CourseRegistrationDetail> cleardCourses = new List<CourseRegistrationDetail>();

                    for (int i = 0; i < courseRegistrationDetails.Count; i++)
                    {
                        decimal passMark = 41;
                        CourseRegistrationDetail RegDetail = courseRegistrationDetails[i];
                        List<CourseRegistrationDetail> RegDetailsToCheck = GetModelsBy(c => c.COURSE.Course_Code.Trim() == RegDetail.Course.Code.Trim() && c.STUDENT_COURSE_REGISTRATION.Person_Id == RegDetail.CourseRegistration.Student.Id).ToList();
                        if (RegDetailsToCheck != null && RegDetailsToCheck.Count > 0)
                        {
                            for (int j = 0; j < RegDetailsToCheck.Count; j++)
                            {
                                if (UseNewGradingSystem)
                                {
                                    passMark = 45;
                                }
                                if (RegDetailsToCheck[j].TestScore + RegDetailsToCheck[j].ExamScore >= passMark)
                                {
                                    List<CourseRegistrationDetail> RegDetails = courseRegistrationDetails.Where(c => c.Course.Code.Trim() == RegDetailsToCheck[j].Course.Code.Trim()).ToList();
                                    foreach (CourseRegistrationDetail courseRegistrationDetail in RegDetails)
                                    {
                                        cleardCourses.Add(courseRegistrationDetail);
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < cleardCourses.Count; i++)
                    {
                        courseRegistrationDetails.Remove(cleardCourses[i]);
                    }
                }

                return courseRegistrationDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }


        //public List<CourseRegistrationDetail> GetCarryOverBy(Student student, Session session, Semester semester)
        //{
        //    try
        //    {
        //        List<CourseRegistrationDetail> carryOverCourses =
        //            (from cr in
        //                repository.GetBy<VW_STUDENT_REGISTERED_COURSE_CARRYOVER>(
        //                    cr => cr.Person_Id == student.Id && cr.Session_Id != session.Id && cr.Semester_Id == semester.Id)
        //             select new CourseRegistrationDetail
        //             {
        //                 Id = cr.Student_Course_Registration_Detail_Id,
        //             }).ToList();

        //        List<CourseRegistrationDetail> courseRegistrationDetails = null;
        //        if (carryOverCourses != null && carryOverCourses.Count > 0)
        //        {
        //            int totalUnit = 0;
        //            courseRegistrationDetails = new List<CourseRegistrationDetail>();
        //            foreach (CourseRegistrationDetail carryOverCourse in carryOverCourses)
        //            {
        //                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
        //                    cr => cr.Student_Course_Registration_Detail_Id == carryOverCourse.Id;
        //                CourseRegistrationDetail courseRegistrationDetail = GetModelBy(selector);

        //                if (courseRegistrationDetail != null)
        //                {
        //                    totalUnit += courseRegistrationDetail.Course.Unit;
        //                    courseRegistrationDetail.Mode = new CourseMode { Id = 2 };
        //                    courseRegistrationDetails.Add(courseRegistrationDetail);
        //                }
        //            }
        //        }

        //        return courseRegistrationDetails;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        public List<CourseRegistrationDetail> GetAllCarryOverBy(Student student, Level level, Programme programme,Department department, Session session, Semester semester)
        {
            try
            {
                List<CourseRegistrationDetail> courseRegistrationDetails = null;
                CourseLogic courseLogic = new CourseLogic();
                List<Course> courses =
                    courseLogic.GetModelsBy(
                        a =>
                            a.Level_Id < level.Id && a.Programme_Id == programme.Id && a.Department_Id == department.Id &&
                            a.Semester_Id == semester.Id);
                if (courses != null && courses.Count > 0)
                {
                    courseRegistrationDetails = new List<CourseRegistrationDetail>();
                    foreach (Course carryoverCourse in courses)
                    {
                        CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                        courseRegistrationDetail.Course = carryoverCourse;
                        courseRegistrationDetail.Course.IsRegistered = false;
                        courseRegistrationDetail.Mode = new CourseMode(){Id = 2};
                        courseRegistrationDetail.CourseUnit = carryoverCourse.Unit;
                        courseRegistrationDetail.Semester = carryoverCourse.Semester;
                        courseRegistrationDetails.Add(courseRegistrationDetail);

                    }
                }
                return courseRegistrationDetails;
            }
            catch (Exception)
            {
                
                throw;
            }
        }


        public bool Modify(List<CourseRegistrationDetail> oldCourseRegistrationDetails,List<CourseRegistrationDetail> newCourseRegistrationDetails)
        {
            try
            {
                for (int i = 0; i < oldCourseRegistrationDetails.Count; i++)
                {
                    oldCourseRegistrationDetails[i].Mode = newCourseRegistrationDetails[i].Mode;
                    oldCourseRegistrationDetails[i].Course.Unit = newCourseRegistrationDetails[i].Course.Unit;
                    oldCourseRegistrationDetails[i].Course.Id = newCourseRegistrationDetails[i].Course.Id;
                    if (!Modify(oldCourseRegistrationDetails[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(CourseRegistrationDetail courseRegistrationDetail)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> predicate = m => m.Student_Course_Registration_Detail_Id == courseRegistrationDetail.Id;
                STUDENT_COURSE_REGISTRATION_DETAIL entity = GetEntityBy(predicate);
                entity.Course_Id = courseRegistrationDetail.Course.Id;

                if (courseRegistrationDetail.ExamScore >= 0)
                {
                    entity.Exam_Score = courseRegistrationDetail.ExamScore;
                }

                if (courseRegistrationDetail.TestScore >= 0)
                {
                    entity.Test_Score = courseRegistrationDetail.TestScore;
                }

                if (courseRegistrationDetail.Mode != null)
                {
                    entity.Course_Mode_Id = courseRegistrationDetail.Mode.Id;
                }

                int rowsAffected = base.Save();
                if (rowsAffected > 0)
                {
                    return true;
                }
                return false;
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException(ArgumentNullException);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Remove(List<CourseRegistrationDetail> courseRegistrationDetails)
        {
            try
            {
                int rowsDeleted = 0;
                if (courseRegistrationDetails != null && courseRegistrationDetails.Count > 0)
                {
                    foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationDetails)
                    {
                        if (courseRegistrationDetail.ExamScore == null && courseRegistrationDetail.TestScore == null)
                        {
                            Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> predicate = m => m.Student_Course_Registration_Detail_Id == courseRegistrationDetail.Id;
                            rowsDeleted += base.Delete(predicate) ? 1 : 0;
                        }
                    }
                }

                if (rowsDeleted > 0 && rowsDeleted == courseRegistrationDetails.Count)
                {
                    return true;
                }
                throw new Exception(
                    "Number of deletable rows does not match the actual number rows deleted! Please try again.");
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException(ArgumentNullException);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AttendanceFormat> GetAttendanceList(Session session, Semester semester, Programme programme,
            Department department, Level level, Course course)
        {
            try
            {
                var attendanceFormatList = new List<AttendanceFormat>();
                var courseRegistrationList = new List<CourseRegistration>();
                var courseRegistrationDetailList = new List<CourseRegistrationDetail>();

                var courseRegistrationLogic = new CourseRegistrationLogic();
                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                courseRegistrationList =
                    courseRegistrationLogic.GetModelsBy(
                        p =>
                            p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id &&
                            p.Programme_Id == programme.Id);
                if (course != null && semester != null && courseRegistrationList.Count > 0)
                {
                    foreach (CourseRegistration courseRegistration in courseRegistrationList)
                    {
                        courseRegistrationDetailList =
                            courseRegistrationDetailLogic.GetModelsBy(
                                p =>
                                    p.Course_Id == course.Id && p.Semester_Id == semester.Id &&
                                    p.Student_Course_Registration_Id == courseRegistration.Id);
                        if (courseRegistrationDetailList.Count > 0)
                        {
                            foreach (
                                CourseRegistrationDetail courseRegistrationDetailItem in courseRegistrationDetailList)
                            {
                                var attendanceFormat = new AttendanceFormat();
                                attendanceFormat.COURSE = courseRegistrationDetailItem.Course.Name;
                                attendanceFormat.MATRICNO =
                                    courseRegistrationDetailItem.CourseRegistration.Student.MatricNumber;
                                attendanceFormat.NAME = courseRegistrationDetailItem.CourseRegistration.Student.FullName;
                                attendanceFormat.DEPARTMENT =
                                    courseRegistrationDetailItem.CourseRegistration.Department.Name;
                                attendanceFormat.LEVEL = courseRegistrationDetailItem.CourseRegistration.Level.Name;
                                attendanceFormat.PROGRAMME =
                                    courseRegistrationDetailItem.CourseRegistration.Programme.Name;
                                attendanceFormat.SEMESTER = courseRegistrationDetailItem.Semester.Name;
                                attendanceFormat.SESSION = courseRegistrationDetailItem.CourseRegistration.Session.Name;

                                attendanceFormatList.Add(attendanceFormat);
                                attendanceFormatList.OrderBy(p => p.MATRICNO);
                            }
                        }
                    }
                }

                return attendanceFormatList.OrderBy(p => p.NAME).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CourseRegistrationReportModel> GetCourseRegistrationBy(Session session, Programme programme,
            Department department, Level level)
        {
            try
            {
                var courseLogic = new CourseLogic();
                List<Course> courses = courseLogic.GetModelsBy(c => c.Department_Id == department.Id);
                var registeredCourses = new List<CourseRegistrationReportModel>();

                foreach (Course course in courses)
                {
                    if (level != null && level.Id > 0)
                    {
                        List<CourseRegistrationDetail> courseRegistrationDetails =
                            GetModelsBy(
                                crd =>
                                    crd.Course_Id == course.Id &&
                                    crd.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id &&
                                    crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id &&
                                    crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id &&
                                    crd.STUDENT_COURSE_REGISTRATION.Level_Id == level.Id);
                        if (courseRegistrationDetails != null && courseRegistrationDetails.Count > 0)
                        {
                            var courseRegistrationReportModel = new CourseRegistrationReportModel();
                            courseRegistrationReportModel.CourseCode = course.Code;
                            courseRegistrationReportModel.CourseName = course.Name;
                            courseRegistrationReportModel.CourseUnit = course.Unit;
                            courseRegistrationReportModel.Department = department.Name;
                            courseRegistrationReportModel.Level = level.Name;
                            courseRegistrationReportModel.Programme = programme.Name;
                            courseRegistrationReportModel.Semester = course.Semester.Name;
                            courseRegistrationReportModel.Session = session.Name;

                            registeredCourses.Add(courseRegistrationReportModel);
                        }
                    }
                    else if (level == null || level.Id <= 0)
                    {
                        List<CourseRegistrationDetail> courseRegistrationDetails =
                            GetModelsBy(
                                crd =>
                                    crd.Course_Id == course.Id &&
                                    crd.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id &&
                                    crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id &&
                                    crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id);
                        if (courseRegistrationDetails != null && courseRegistrationDetails.Count > 0)
                        {
                            var courseRegistrationReportModel = new CourseRegistrationReportModel();
                            courseRegistrationReportModel.CourseCode = course.Code;
                            courseRegistrationReportModel.CourseName = course.Name;
                            courseRegistrationReportModel.CourseUnit = course.Unit;
                            courseRegistrationReportModel.Department = department.Name;
                            //courseRegistrationReportModel.Level = level.Name;
                            courseRegistrationReportModel.Programme = programme.Name;
                            courseRegistrationReportModel.Semester = course.Semester.Name;
                            courseRegistrationReportModel.Session = session.Name;

                            registeredCourses.Add(courseRegistrationReportModel);
                        }
                    }
                }

                return registeredCourses.OrderBy(rc => rc.CourseName).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AttendanceFormat> GetCourseAttendanceSheet(Session session, Semester semester, Programme programme,
           Department department, Level level, Course course)
        {
            try
            {
                var attendanceFormatList = new List<AttendanceFormat>();

                if (session != null && semester != null && programme != null && department != null && level != null &&
                    course != null)
                {
                    attendanceFormatList =
                        (from crd in
                             repository.GetBy<STUDENT_COURSE_REGISTRATION_DETAIL>(
                                c =>
                                    c.Course_Id == course.Id && c.Semester_Id == semester.Id &&
                                    c.STUDENT_COURSE_REGISTRATION.Session_Id == session.Id && c.STUDENT_COURSE_REGISTRATION.Level_Id == level.Id &&
                                    c.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id && c.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id)
                         select new AttendanceFormat
                         {
                             COURSE = crd.COURSE.Course_Name,
                             MATRICNO = crd.STUDENT_COURSE_REGISTRATION.STUDENT.Matric_Number,
                             NAME = crd.STUDENT_COURSE_REGISTRATION.STUDENT.PERSON.Last_Name + " " + crd.STUDENT_COURSE_REGISTRATION.STUDENT.PERSON.First_Name + " " + crd.STUDENT_COURSE_REGISTRATION.STUDENT.PERSON.Other_Name,
                             DEPARTMENT = crd.STUDENT_COURSE_REGISTRATION.DEPARTMENT.Department_Name,
                             LEVEL = crd.STUDENT_COURSE_REGISTRATION.LEVEL.Level_Name,
                             PROGRAMME = crd.STUDENT_COURSE_REGISTRATION.PROGRAMME.Programme_Name,
                             SEMESTER = crd.SEMESTER.Semester_Name,
                             SESSION = crd.STUDENT_COURSE_REGISTRATION.SESSION.Session_Name,
                             COURSE_CODE = crd.COURSE.Course_Code,
                             COURSE_UNIT = crd.COURSE.Course_Unit,
                             SCHOOL = crd.STUDENT_COURSE_REGISTRATION.DEPARTMENT.FACULTY.Faculty_Name,
                             IMAGE_URL = crd.STUDENT_COURSE_REGISTRATION.STUDENT.PERSON.Image_File_Url,

                         }).ToList();


                    //attendanceFormatList =
                    //  (from crd in
                    //       repository.GetBy<VW_STUDENT_RESULT_2>(
                    //          c =>
                    //              c.Course_Id == course.Id && c.Semester_Id == semester.Id &&
                    //              c.Session_Id == session.Id && c.Level_Id == level.Id &&
                    //              c.Department_Id == department.Id && c.Programme_Id == programme.Id)
                    //   select new AttendanceFormat
                    //   {
                    //       COURSE = crd.Course_Name,
                    //       MATRICNO = crd.Matric_Number,
                    //       NAME = crd.Name,
                    //       DEPARTMENT = crd.Department_Name,
                    //       LEVEL = crd.Level_Name,
                    //       PROGRAMME = crd.Programme_Name,
                    //       SEMESTER = crd.Semester_Name,
                    //       SESSION = crd.Session_Name,
                    //       COURSE_CODE = crd.Course_Code,
                    //       COURSE_UNIT = crd.Course_Unit,
                    //       SCHOOL = crd.Faculty_Name,
                    //       IMAGE_URL = crd.Image_File_Url,
                    //   }).ToList();
                }

                return attendanceFormatList.OrderBy(a => a.NAME).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<AttendanceFormat>> GetCourseAttendanceSheetAsync(Session session, Semester semester, Programme programme,
            Department department, Level level, Course course)
        {
            try
            {
                var attendanceFormatList = new List<AttendanceFormat>();

                if (session != null && semester != null && programme != null && department != null && level != null &&
                    course != null)
                {
                    attendanceFormatList =
                        (from crd in
                            await repository.GetByAsync<VW_STUDENT_RESULT_2>(
                                c =>
                                    c.Course_Id == course.Id && c.Semester_Id == semester.Id &&
                                    c.Session_Id == session.Id && c.Level_Id == level.Id &&
                                    c.Department_Id == department.Id && c.Programme_Id == programme.Id)
                            select new AttendanceFormat
                            {
                                COURSE = crd.Course_Name,
                                MATRICNO = crd.Matric_Number,
                                NAME = crd.Name,
                                DEPARTMENT = crd.Department_Name,
                                LEVEL = crd.Level_Name,
                                PROGRAMME = crd.Programme_Name,
                                SEMESTER = crd.Semester_Name,
                                SESSION = crd.Session_Name,
                                COURSE_CODE = crd.Course_Code,
                                COURSE_UNIT = crd.Course_Unit,
                                SCHOOL = crd.Faculty_Name,
                                IMAGE_URL = crd.Image_File_Url,
                            }).ToList();
                }

                return attendanceFormatList.OrderBy(a => a.NAME).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool RemoveDuplicateCourseRegistration(List<CourseRegistration> courseRegistrationList)
        {
            try
            {
                var courseRegistrationLogic = new CourseRegistrationLogic();
                bool isDeleted = false;
                if (courseRegistrationList.Count > 1)
                {
                    long firstCourseRegisteredId = courseRegistrationList.FirstOrDefault().Id;
                    long secondCourseRegisteredId = courseRegistrationList.LastOrDefault().Id;

                    List<STUDENT_COURSE_REGISTRATION_DETAIL> courseRegistrationDetailList =
                        GetEntitiesBy(p => p.Student_Course_Registration_Id == secondCourseRegisteredId);
                    foreach (STUDENT_COURSE_REGISTRATION_DETAIL courseRegDetailItem in courseRegistrationDetailList)
                    {
                        courseRegDetailItem.Student_Course_Registration_Id = firstCourseRegisteredId;
                    }
                    int count = Save();

                    isDeleted =
                        courseRegistrationLogic.Delete(p => p.Student_Course_Registration_Id == secondCourseRegisteredId);

                    //Save();
                }
                return isDeleted;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetRegisteredCourses(long CourseRegistrationId)
        {
            try
            {
                string courses = "";
                IEnumerable<CourseRegistrationDetail> registredCourses = GetModelsBy(a => a.Student_Course_Registration_Id == CourseRegistrationId);
                foreach(var courseRegistered in registredCourses)
                {
                    courses += courseRegistered.Course.Code.Trim() + "|";
                }

                return courses;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<CourseRegistrationDetail> GetGSTResultsBy(Student student, SessionSemester sessionSemester)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                    crd =>
                        crd.STUDENT_COURSE_REGISTRATION.STUDENT.Matric_Number == student.MatricNumber &&
                        crd.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id &&
                        crd.Semester_Id == sessionSemester.Semester.Id && crd.COURSE.Course_Code.Contains("GST");
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}