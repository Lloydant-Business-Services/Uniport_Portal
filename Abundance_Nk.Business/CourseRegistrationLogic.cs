using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseRegistrationLogic : BusinessBaseLogic<CourseRegistration, STUDENT_COURSE_REGISTRATION>
    {
        private readonly CourseRegistrationDetailLogic courseRegistrationDetailLogic;

        public CourseRegistrationLogic()
        {
            translator = new CourseRegistrationTranslator();
            courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
        }


        public CourseRegistration GetBy(Student student, Level level, Programme programme, Department department,
            Session session)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> selector =
                    cr =>
                        cr.Person_Id == student.Id  && cr.Programme_Id == programme.Id &&
                        cr.Department_Id == department.Id && cr.Session_Id == session.Id;
                var registeredCourse = new CourseRegistration();
                registeredCourse = GetModelBy(selector);
                if (registeredCourse != null && registeredCourse.Id > 0)
                {
                    registeredCourse.Details = courseRegistrationDetailLogic.GetBy(registeredCourse);
                }

                return registeredCourse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CourseRegistration GetBy(Student student, Programme programme, Department department)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> selector =
                    cr =>
                        cr.Person_Id == student.Id && cr.Programme_Id == programme.Id &&
                        cr.Department_Id == department.Id;
                var registeredCourse = new CourseRegistration();
                registeredCourse = GetModelBy(selector);
                if (registeredCourse != null && registeredCourse.Id > 0)
                {
                    registeredCourse.Details = courseRegistrationDetailLogic.GetBy(registeredCourse);
                }

                return registeredCourse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CourseRegistration> GetListBy(Student student, Level level, Programme programme,Department department, Session session)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> selector =
                    cr =>
                        cr.Person_Id == student.Id  && cr.Programme_Id == programme.Id &&
                        cr.Department_Id == department.Id && cr.Session_Id == session.Id;
                List<CourseRegistration> registeredCourses = base.GetModelsBy(selector);
                if (registeredCourses != null)
                {
                    foreach (CourseRegistration registeredCourse in registeredCourses)
                    {
                        registeredCourse.Details = courseRegistrationDetailLogic.GetBy(registeredCourse);
                    }
                }

                return registeredCourses;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CourseRegistration> GetListBy(Student student, Session session)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> selector =
                    cr =>
                        cr.Person_Id == student.Id && cr.Session_Id == session.Id;
                List<CourseRegistration> registeredCourses = base.GetModelsBy(selector);
                if (registeredCourses != null)
                {
                    foreach (CourseRegistration registeredCourse in registeredCourses)
                    {
                        registeredCourse.Details = courseRegistrationDetailLogic.GetBy(registeredCourse);
                    }
                }

                return registeredCourses;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CourseRegistration> GetListBy(Student student, Programme programme, Department department)
        {
            try
            {
                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> selector =
                    cr =>
                        cr.Person_Id == student.Id && cr.Programme_Id == programme.Id &&
                        cr.Department_Id == department.Id;
                var registeredCourses = new List<CourseRegistration>();
                registeredCourses = GetModelsBy(selector);
                if (registeredCourses != null && registeredCourses.Count > 0)
                {
                    foreach (CourseRegistration registeredCourse in registeredCourses)
                    {
                        registeredCourse.Details = courseRegistrationDetailLogic.GetBy(registeredCourse);
                    }
                }

                return registeredCourses;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetExamSheetBy(Level level, Programme programme, Department department, Session session,
            Semester semester)
        {
            try
            {
                if (session == null || session.Id <= 0 || level == null || level.Id <= 0 || programme == null ||
                    programme.Id <= 0 || department == null || department.Id <= 0 || semester == null ||
                    semester.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Exam Sheet not set! Please check your input criteria selection and try again.");
                }


                List<Result> results =
                    (from sr in
                        repository.GetBy<VW_REGISTERED_COURSES>(
                            x =>
                                x.Session_Id == session.Id && x.Semester_Id == semester.Id && x.Level_Id == level.Id &&
                                x.Programme_Id == programme.Id && x.Department_Id == department.Id)
                        select new Result
                        {
                            StudentId = sr.Person_Id,
                            Sex = sr.Sex_Name,
                            Name = sr.Name,
                            MatricNumber = sr.Matric_Number,
                            CourseId = sr.Course_Id,
                            CourseCode = sr.Course_Code,
                            CourseName = sr.Course_Name,
                            CourseUnit = sr.Course_Unit,
                        }).ToList();

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override CourseRegistration Create(CourseRegistration courseRegistration)
        {
            try
            {
                int rowAdded = 0;
                CourseRegistration newCourseRegistration = null;
                var courseLogic = new CourseLogic();
                using (
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions {IsolationLevel = IsolationLevel.Snapshot}))
                {
                    newCourseRegistration =
                        GetModelBy(
                            a =>
                                a.Person_Id == courseRegistration.Student.Id &&
                                a.Session_Id == courseRegistration.Session.Id &&
                                a.Programme_Id == courseRegistration.Programme.Id &&
                                a.Department_Id == courseRegistration.Department.Id &&
                                a.Level_Id == courseRegistration.Level.Id);
                    if (newCourseRegistration == null)
                    {
                        newCourseRegistration = base.Create(courseRegistration);
                        if (courseRegistration.Details != null && courseRegistration.Details.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistration.Details)
                            {
                                courseRegistrationDetail.CourseRegistration = newCourseRegistration;
                                Course course =
                                    courseLogic.GetModelBy(p => p.Course_Id == courseRegistrationDetail.Course.Id);
                                courseRegistrationDetail.CourseUnit = course.Unit;
                            }

                            rowAdded = courseRegistrationDetailLogic.Create(courseRegistration.Details);
                            if (rowAdded > 0 && rowAdded == courseRegistration.Details.Count)
                            {
                                transaction.Complete();
                            }
                        }
                    }
                }

                return rowAdded > 0 ? newCourseRegistration : null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public CourseRegistration Create(CourseRegistration courseRegistration, CourseRegistrationDetailAudit courseRegistrationDetailAudit)
        {
            try
            {
                int rowAdded = 0;
                CourseRegistration newCourseRegistration = null;
                CourseLogic courseLogic = new CourseLogic();
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                {
                    newCourseRegistration = GetModelBy(a => a.Person_Id == courseRegistration.Student.Id && a.Session_Id == courseRegistration.Session.Id && a.Programme_Id == courseRegistration.Programme.Id && a.Department_Id == courseRegistration.Department.Id && a.Level_Id == courseRegistration.Level.Id);
                    if (newCourseRegistration == null)
                    {
                        newCourseRegistration = base.Create(courseRegistration);
                        if (courseRegistration.Details != null && courseRegistration.Details.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistration.Details)
                            {
                                courseRegistrationDetail.CourseRegistration = newCourseRegistration;
                                Course course = courseLogic.GetModelBy(p => p.Course_Id == courseRegistrationDetail.Course.Id);
                                courseRegistrationDetail.CourseUnit = course.Unit;
                            }

                            rowAdded = courseRegistrationDetailLogic.Create(courseRegistration.Details, courseRegistrationDetailAudit);
                            if (rowAdded > 0)
                            {
                                transaction.Complete();
                            }
                        }
                    }

                }

                return rowAdded > 0 ? newCourseRegistration : null;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool Modify(CourseRegistration courseRegistration)
        {
            try
            {
                string errorMessage = "Course Registration modification failed!";
                bool modified = false;
                if (courseRegistration.Details != null && courseRegistration.Details.Count > 0)
                {
                    using (var transaction = new TransactionScope(TransactionScopeOption.Required,new TransactionOptions {IsolationLevel = IsolationLevel.Snapshot}))
                    {
                        List<CourseRegistrationDetail> existingCourseRegistrationDetails = courseRegistration.Details.Where(c => c.Id > 0 && c.Course.IsRegistered).ToList();
                        List<CourseRegistrationDetail> removedCourseRegistrationDetails = courseRegistration.Details.Where(c => c.Id > 0 && c.Course.IsRegistered == false).ToList();
                        List<CourseRegistrationDetail> newCourseRegistrationDetails = courseRegistration.Details.Where(c => c.Id <= 0 && c.Course.IsRegistered).ToList();

                        if (removedCourseRegistrationDetails.Count > 0 && removedCourseRegistrationDetails[0].CourseRegistration.Id <= 0)
                        {
                            for (int i = 0; i < removedCourseRegistrationDetails.Count; i++)
                            {
                                removedCourseRegistrationDetails[i].CourseRegistration.Id = courseRegistration.Id;
                                removedCourseRegistrationDetails[i].CourseRegistration.Level = courseRegistration.Level;
                                removedCourseRegistrationDetails[i].CourseRegistration.Programme = courseRegistration.Programme;
                                removedCourseRegistrationDetails[i].CourseRegistration.Session = courseRegistration.Session;
                                removedCourseRegistrationDetails[i].CourseRegistration.Student = courseRegistration.Student;
                                removedCourseRegistrationDetails[i].CourseRegistration.Department = courseRegistration.Department;
                            }
                        }

                        if (newCourseRegistrationDetails.Count > 0 && newCourseRegistrationDetails[0].CourseRegistration.Id <=0)
                        {
                             for (int i = 0; i < newCourseRegistrationDetails.Count; i++)
                            {
                                newCourseRegistrationDetails[i].CourseRegistration.Id = courseRegistration.Id;
                                newCourseRegistrationDetails[i].CourseRegistration.Level = courseRegistration.Level;
                                newCourseRegistrationDetails[i].CourseRegistration.Programme = courseRegistration.Programme;
                                newCourseRegistrationDetails[i].CourseRegistration.Session = courseRegistration.Session;
                                newCourseRegistrationDetails[i].CourseRegistration.Student = courseRegistration.Student;
                                newCourseRegistrationDetails[i].CourseRegistration.Department = courseRegistration.Department;
                            }
                        }

                        int newCourseRegistrationDetailCount = newCourseRegistrationDetails.Count;
                        int removedCourseRegistrationDetailCount = removedCourseRegistrationDetails.Count;

                        if (newCourseRegistrationDetailCount <= 0 && removedCourseRegistrationDetailCount <= 0)
                        {
                            throw new Exception("No changes detected! You need to make some changes before cliking the Register Course button.");
                        }

                        if (newCourseRegistrationDetailCount == removedCourseRegistrationDetailCount)
                        {
                            if (!courseRegistrationDetailLogic.Modify(removedCourseRegistrationDetails,newCourseRegistrationDetails))
                            {
                                throw new Exception(errorMessage);
                            }
                        }
                        else if (newCourseRegistrationDetailCount > removedCourseRegistrationDetailCount)
                        {
                            int difference = newCourseRegistrationDetailCount - removedCourseRegistrationDetailCount;
                            List<CourseRegistrationDetail> removedCourseRegistrationDetailsToModify = removedCourseRegistrationDetails.Take(removedCourseRegistrationDetailCount).ToList();
                            List<CourseRegistrationDetail> newCourseRegistrationDetailsToModify = newCourseRegistrationDetails.Take(removedCourseRegistrationDetailCount).ToList();
                            List<CourseRegistrationDetail> newCourseRegistrationDetailsToAdd = newCourseRegistrationDetails.Skip(removedCourseRegistrationDetailCount).Take(difference).ToList();

                            if (removedCourseRegistrationDetailsToModify == null ||removedCourseRegistrationDetailsToModify.Count <= 0 ||removedCourseRegistrationDetailCount <= 0)
                            {
                                if (courseRegistrationDetailLogic.Create(newCourseRegistrationDetailsToAdd) > 0)
                                {
                                    modified = true;
                                }
                                else
                                {
                                    throw new Exception(errorMessage);
                                }
                            }
                            else if (removedCourseRegistrationDetailsToModify != null && removedCourseRegistrationDetailsToModify.Count > 0 && newCourseRegistrationDetailsToModify != null && newCourseRegistrationDetailsToModify.Count > 0 && newCourseRegistrationDetailsToModify.Count ==removedCourseRegistrationDetailsToModify.Count)
                            {
                                modified = courseRegistrationDetailLogic.Modify(removedCourseRegistrationDetailsToModify,newCourseRegistrationDetailsToModify);
                                if (modified)
                                {
                                    if (courseRegistrationDetailLogic.Create(newCourseRegistrationDetailsToAdd) > 0)
                                    {
                                        modified = true;
                                    }
                                    else
                                    {
                                        throw new Exception(errorMessage);
                                    }
                                }
                                else
                                {
                                    throw new Exception(errorMessage);
                                }
                            }
                        }
                        else if (newCourseRegistrationDetailCount < removedCourseRegistrationDetailCount)
                        {
                            int difference = removedCourseRegistrationDetailCount - newCourseRegistrationDetailCount;
                            List<CourseRegistrationDetail> newCourseRegistrationDetailsToModify =newCourseRegistrationDetails.Take(newCourseRegistrationDetailCount).ToList();
                            List<CourseRegistrationDetail> removedCourseRegistrationDetailsToModify =removedCourseRegistrationDetails.Take(newCourseRegistrationDetailCount).ToList();
                            List<CourseRegistrationDetail> courseRegistrationDetailsToDelete =removedCourseRegistrationDetails.Skip(newCourseRegistrationDetailCount).Take(difference).ToList();

                            if (newCourseRegistrationDetailCount <= 0)
                            {
                                if (courseRegistrationDetailLogic.Remove(courseRegistrationDetailsToDelete))
                                {
                                    modified = true;
                                }
                                else
                                {
                                    throw new Exception(errorMessage);
                                }
                            }
                            else
                            {
                                modified = courseRegistrationDetailLogic.Modify(removedCourseRegistrationDetailsToModify, newCourseRegistrationDetailsToModify);
                                if (modified)
                                {
                                    if (!courseRegistrationDetailLogic.Remove(courseRegistrationDetailsToDelete))
                                    {
                                        throw new Exception(errorMessage);
                                    }
                                }
                                else
                                {
                                    throw new Exception(errorMessage);
                                }
                            }
                        }

                        transaction.Complete();
                    }
                }

                return modified;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CourseRegistration CreateCourseRegistration(CourseRegistration courseRegistration)
        {
            try
            {
                CourseRegistration newCourseRegistration = null;
                newCourseRegistration =
                    GetModelBy(
                        a =>
                            a.Person_Id == courseRegistration.Student.Id &&
                            a.Session_Id == courseRegistration.Session.Id &&
                            a.Programme_Id == courseRegistration.Programme.Id &&
                            a.Department_Id == courseRegistration.Department.Id &&
                            a.Level_Id == courseRegistration.Level.Id);
                if (newCourseRegistration == null)
                {
                    newCourseRegistration = base.Create(courseRegistration);
                }
                return newCourseRegistration;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CarryOverReportModel> GetCarryOverList(Department department, Programme programme, Session session,
            Level level, Semester semester)
        {
            try
            {
                var CarryOverStudentList = new List<CarryOverReportModel>();

                var studentLogic = new StudentLogic();
                var courseLogic = new CourseLogic();
                var departmentLogic = new DepartmentLogic();
                var programmeLogic = new ProgrammeLogic();
                var sessionLogic = new SessionLogic();
                var semesterLogic = new SemesterLogic();
                var levelLogic = new LevelLogic();

                Department departmentNew = departmentLogic.GetModelBy(p => p.Department_Id == department.Id);
                Programme programmeNew = programmeLogic.GetModelBy(p => p.Programme_Id == programme.Id);
                Session sessionNew = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                Semester semesterNew = semesterLogic.GetModelBy(p => p.Semester_Id == semester.Id);
                Level levelNew = levelLogic.GetModelBy(p => p.Level_Id == level.Id);

                List<CourseRegistration> courseRegistration =
                    GetModelsBy(
                        p =>
                            p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id &&
                            p.Programme_Id == programme.Id);

                foreach (CourseRegistration itemList in courseRegistration)
                {
                    List<CourseRegistrationDetail> courseRegistrationDetailList =
                        courseRegistrationDetailLogic.GetModelsBy(
                            p =>
                                p.Student_Course_Registration_Id == itemList.Id && p.Semester_Id == semester.Id &&
                                (p.Exam_Score + p.Test_Score) <= 39);
                    foreach (CourseRegistrationDetail courseRegDetail in courseRegistrationDetailList)
                    {
                        string name =
                            courseRegistrationDetailLogic.GetModelBy(
                                p => p.Student_Course_Registration_Detail_Id == courseRegDetail.Id)
                                .CourseRegistration.Student.FullName;
                        string matricNo =
                            courseRegistrationDetailLogic.GetModelBy(
                                p => p.Student_Course_Registration_Detail_Id == courseRegDetail.Id)
                                .CourseRegistration.Student.MatricNumber;
                        Course course = courseLogic.GetModelBy(p => p.Course_Id == courseRegDetail.Course.Id);
                        CarryOverStudentList.Add(new CarryOverReportModel
                        {
                            CourseCode = course.Code,
                            CourseName = course.Name,
                            CourseUnit = course.Unit,
                            Department = departmentNew.Name,
                            Programme = programmeNew.Name,
                            Fullname = name,
                            MatricNo = matricNo,
                            Semester = semesterNew.Name,
                            Session = sessionNew.Name,
                            Level = levelNew.Name,
                        });
                    }
                }
                return CarryOverStudentList.OrderBy(p => p.Fullname).ToList();
            }

            catch (Exception)
            {
                throw;
            }
        }

        public List<CarryOverReportModel> GetCarryOverCourseList(Session session, Semester semester, Programme programme,
            Department department, Level level, Course course)
        {
            try
            {
                var CarryOverCourseList = new List<CarryOverReportModel>();

                var courseLogic = new CourseLogic();
                var departmentLogic = new DepartmentLogic();
                var programmeLogic = new ProgrammeLogic();
                var sessionLogic = new SessionLogic();
                var semesterLogic = new SemesterLogic();
                var levelLogic = new LevelLogic();

                Department departmentNew = departmentLogic.GetModelBy(p => p.Department_Id == department.Id);
                Programme programmeNew = programmeLogic.GetModelBy(p => p.Programme_Id == programme.Id);
                Course courseNew = courseLogic.GetModelBy(p => p.Course_Id == course.Id);
                Session sessionNew = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                Semester semesterNew = semesterLogic.GetModelBy(p => p.Semester_Id == semester.Id);
                Level levelNew = levelLogic.GetModelBy(p => p.Level_Id == level.Id);

                List<CourseRegistration> courseRegistration =
                    GetModelsBy(
                        p =>
                            p.Session_Id == session.Id && p.Level_Id == level.Id && p.Department_Id == department.Id &&
                            p.Programme_Id == programme.Id);

                foreach (CourseRegistration itemList in courseRegistration)
                {
                    List<CourseRegistrationDetail> courseRegistrationDetailList =
                        courseRegistrationDetailLogic.GetModelsBy(
                            p =>
                                p.Student_Course_Registration_Id == itemList.Id && p.Course_Id == course.Id &&
                                p.Semester_Id == semester.Id && (p.Exam_Score + p.Test_Score) <= 39);
                    foreach (CourseRegistrationDetail courseRegDetail in courseRegistrationDetailList)
                    {
                        string name =
                            courseRegistrationDetailLogic.GetModelBy(
                                p => p.Student_Course_Registration_Detail_Id == courseRegDetail.Id)
                                .CourseRegistration.Student.FullName;
                        string matricNo =
                            courseRegistrationDetailLogic.GetModelBy(
                                p => p.Student_Course_Registration_Detail_Id == courseRegDetail.Id)
                                .CourseRegistration.Student.MatricNumber;
                        CarryOverCourseList.Add(new CarryOverReportModel
                        {
                            CourseCode = courseNew.Code,
                            CourseName = courseNew.Name,
                            CourseUnit = courseNew.Unit,
                            Department = departmentNew.Name,
                            Programme = programmeNew.Name,
                            Fullname = name,
                            MatricNo = matricNo,
                            Semester = semesterNew.Name,
                            Session = sessionNew.Name,
                            Level = levelNew.Name,
                        });
                    }
                }
                return CarryOverCourseList.OrderBy(p => p.MatricNo).ToList();
            }

            catch (Exception)
            {
                throw;
            }
        }

        public List<PaymentReport> GetRegistrationBy(Session session, Semester semester)
        {
            var courseRegistrationLogic = new CourseRegistrationLogic();
            var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
            var courseRegistrationList = new List<STUDENT_COURSE_REGISTRATION>();
            var courseRegistrationDetail = new STUDENT_COURSE_REGISTRATION_DETAIL();

            var PaymentReportList = new List<PaymentReport>();
            var studentLevelLogic = new StudentLevelLogic();


            try
            {
                if (session != null && semester != null)
                {
                    courseRegistrationList = GetEntitiesBy(p => p.Session_Id == session.Id);
                    //List<Payment> payments = GetModelsBy(p => p.Fee_Type_Id == 3 && p.Session_Id == 1);
                    foreach (STUDENT_COURSE_REGISTRATION courseRegistration in courseRegistrationList)
                    {
                        int studentNumber = 0;

                        courseRegistrationDetail =
                            courseRegistrationDetailLogic.GetEntitiesBy(
                                p =>
                                    p.Semester_Id == semester.Id &&
                                    p.Student_Course_Registration_Id ==
                                    courseRegistration.Student_Course_Registration_Id).FirstOrDefault();
                        if (courseRegistrationDetail != null)
                        {
                            var paymentReport = new PaymentReport();
                            paymentReport.Department = courseRegistration.DEPARTMENT.Department_Name;
                            if (courseRegistration.Level_Id == 1 && courseRegistration.Programme_Id == 2)
                            {
                                paymentReport.Programme = "PART TIME 1";

                                paymentReport.StudentNumber = studentNumber += 1;
                                PaymentReportList.Add(paymentReport);
                            }
                            else if (courseRegistration.Level_Id == 2 && courseRegistration.Programme_Id == 2)
                            {
                                paymentReport.Programme = "PART TIME 2";
                                paymentReport.StudentNumber = studentNumber += 1;
                                PaymentReportList.Add(paymentReport);
                            }
                            else
                            {
                                paymentReport.Programme = courseRegistration.LEVEL.Level_Name;
                                paymentReport.StudentNumber = studentNumber += 1;
                                PaymentReportList.Add(paymentReport);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PaymentReportList;
        }

        public List<ResultFormat> GetDownloadResultFormats(CourseAllocation courseAllocation)
        {
            try
            {
                var departmentLogic = new DepartmentLogic();
                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                var optionName = string.Empty;
                IEnumerable<ResultFormat> resultFormatData;
                if (courseAllocation.DepartmentOption?.Id > 0)
                {
                    optionName=departmentOptionLogic.GetModelBy(f => f.Department_Option_Id == courseAllocation.DepartmentOption.Id).Name;
                    resultFormatData =
                from a in
                    repository.GetBy<VW_REGISTERED_COURSES>(
                        a =>
                            a.Course_Id == courseAllocation.Course.Id &&
                            a.Department_Id == courseAllocation.Department.Id
                            && a.Session_Id == courseAllocation.Session.Id &&
                            a.Programme_Id == courseAllocation.Programme.Id && a.Department_Option_Id==courseAllocation.DepartmentOption.Id)
                select new ResultFormat
                {
                    MatricNo = a.Matric_Number,
                    Fullname = a.Name,
                    CA = a.Test_Score,
                    Exam = a.Exam_Score,
                    CourseCode = a.Course_Code,
                    Department = departmentLogic.GetModelBy(d => d.Department_Id == a.Department_Id).Name,
                };
                }
                else
                {
                    resultFormatData =
                from a in
                    repository.GetBy<VW_REGISTERED_COURSES>(
                        a =>
                            a.Course_Id == courseAllocation.Course.Id &&
                            a.Department_Id == courseAllocation.Department.Id
                            && a.Session_Id == courseAllocation.Session.Id &&
                            a.Programme_Id == courseAllocation.Programme.Id)
                select new ResultFormat
                {
                    MatricNo = a.Matric_Number,
                    Fullname = a.Name,
                    CA = a.Test_Score,
                    Exam = a.Exam_Score,
                    CourseCode = a.Course_Code,
                    Department = departmentLogic.GetModelBy(d => d.Department_Id == a.Department_Id).Name+ " " + optionName,
                };
                }
                List<ResultFormat> resultFormatList = resultFormatData.OrderBy(p => p.MatricNo).ToList();
                return resultFormatList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ResultFormat> GetDownloadResultFormats(CourseAllocation courseAllocation, int courseModeId)
        {
            try
            {
                if (courseModeId != 4)
                {
                    var departmentLogic = new DepartmentLogic();
                    IEnumerable<ResultFormat> resultFormatData =
                        from a in
                            repository.GetBy<VW_REGISTERED_COURSES>(
                                a =>
                                    a.Course_Id == courseAllocation.Course.Id &&
                                    a.Department_Id == courseAllocation.Department.Id &&
                                    a.Level_Id == courseAllocation.Level.Id &&
                                    a.Session_Id == courseAllocation.Session.Id &&
                                    a.Programme_Id == courseAllocation.Programme.Id && a.Course_Mode_Id == courseModeId)
                        select new ResultFormat
                        {
                            MatricNo = a.Matric_Number,
                            Fullname = a.Name,
                            CA = a.Test_Score,
                            Exam = a.Exam_Score,
                            CourseCode = a.Course_Code,
                            Department = departmentLogic.GetModelBy(d => d.Department_Id == a.Department_Id).Name,
                        };

                    List<ResultFormat> resultFormatList = resultFormatData.OrderBy(p => p.MatricNo).ToList();
                    return resultFormatList;
                }
                else
                {
                    var resultFormatList = new List<ResultFormat>();
                    var sampleFormat = new ResultFormat();
                    sampleFormat.MatricNo = "N/XX/15/12345";
                    resultFormatList.Add(sampleFormat);

                    return resultFormatList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<CourseRegistration> GetUnregisteredStudents(Session session, Programme programme,
            Department department, Level level)
        {
            try
            {
                var courseRegistrations = new List<CourseRegistration>();

                return courseRegistrations;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}