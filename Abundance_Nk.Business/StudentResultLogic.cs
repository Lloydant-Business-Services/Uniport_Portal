using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Business
{
    public class StudentResultLogic : BusinessBaseLogic<StudentResult, STUDENT_RESULT>
    {
        private readonly StudentResultDetailLogic studentResultDetailLogic;

        public StudentResultLogic()
        {
            translator = new StudentResultTranslator();
            studentResultDetailLogic = new StudentResultDetailLogic();
        }

        public List<StudentResult> GetBy(Level level, Programme programme, Department department,
            SessionSemester sessionSemester)
        {
            try
            {
                Expression<Func<STUDENT_RESULT, bool>> selector =
                    sr =>
                        sr.Level_Id == level.Id && sr.Programme_Id == programme.Id && sr.Department_Id == department.Id &&
                        sr.Session_Semester_Id == sessionSemester.Id;
                return GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetTotalMaximumObtainableScore(Level level, Programme programme, Department department,
            SessionSemester sessionSemester)
        {
            try
            {
                //List<int?> result = (from sr in repository.GetBy<VW_MAXIMUM_OBTAINABLE_SCORE>(x => x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id)
                //                     select sr.Maximum_Score_Obtainable.Value);


                Expression<Func<STUDENT_RESULT, bool>> selector =
                    sr =>
                        sr.Level_Id == level.Id && sr.Programme_Id == programme.Id && sr.Department_Id == department.Id &&
                        sr.Session_Semester_Id == sessionSemester.Id;
                List<StudentResult> studentResults = GetModelsBy(selector);
                return studentResults.Sum(s => s.MaximumObtainableScore);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Save(StudentResult resultHeader)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    if (resultHeader.Results != null && resultHeader.Results.Count > 0)
                    {
                        Add(resultHeader);
                        transaction.Complete();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override StudentResult Add(StudentResult resultHeader)
        {
            try
            {
                StudentResult newResultHeader = base.Create(resultHeader);
                if (newResultHeader == null || newResultHeader.Id == 0)
                {
                    throw new Exception("Result Header add opeartion failed! Please try again");
                }

                resultHeader.Id = newResultHeader.Id;

                List<StudentResultDetail> results = SetHeader(resultHeader);
                int rowsAdded = studentResultDetailLogic.Create(results);
                if (rowsAdded == 0)
                {
                    throw new Exception(
                        "Result Header was succesfully added, but Result Detail Add operation failed! Please try again");
                }

                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                if (courseRegistrationDetailLogic.UpdateCourseRegistrationScore(results))
                {
                    return resultHeader;
                }
                throw new Exception("Registered course failed on update! Please try again");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public StudentResult Add(StudentResult resultHeader, CourseRegistrationDetailAudit courseRegistrationDetailAudit)
        {
            try
            {
                StudentResult newResultHeader = base.Create(resultHeader);
                if (newResultHeader == null || newResultHeader.Id == 0)
                {
                    throw new Exception("Result Header add opeartion failed! Please try again");
                }

                resultHeader.Id = newResultHeader.Id;
                //List<StudentResultDetail> results = SetHeader(resultHeader, newResultHeader);

                List<StudentResultDetail> results = SetHeader(resultHeader);
                int rowsAdded = studentResultDetailLogic.Create(results);
                if (rowsAdded == 0)
                {
                    throw new Exception("Result Header was succesfully added, but Result Detail Add operation failed! Please try again");
                }

                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                if (courseRegistrationDetailLogic.UpdateCourseRegistrationScore(results, courseRegistrationDetailAudit))
                {
                    //return newResultHeader;
                    return resultHeader;
                }
                else
                {
                    throw new Exception("Registered course failed on update! Please try again");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<StudentResultDetail> SetHeader(StudentResult resultHeader)
        {
            try
            {
                foreach (StudentResultDetail result in resultHeader.Results)
                {
                    result.Header = resultHeader;
                    //result.Header.Id = newResultHeader.Id;
                }

                return resultHeader.Results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetTranscriptBy(Student student)
        {
            try
            {
                if (student == null || student.Id <= 0)
                {
                    throw new Exception("Student not set! Please select student and try again.");
                }

                List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Person_Id == student.Id)
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
                        FacultyName = sr.Faculty_Name,
                        TestScore = sr.Test_Score,
                        ExamScore = sr.Exam_Score,
                        Score = sr.Total_Score,
                        Grade = sr.Grade,
                        GradePoint = sr.Grade_Point,
                        Email = sr.Email,
                        Address = sr.Contact_Address,
                        MobilePhone = sr.Mobile_Phone,
                        PassportUrl = sr.Image_File_Url,
                        GPCU = sr.Grade_Point*sr.Course_Unit,
                        TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                        SessionName = sr.Session_Name,
                        Semestername = sr.Semester_Name,
                        LevelName = sr.Level_Name,
                        ProgrammeName = sr.Programme_Name,
                        SessionSemesterId = sr.Session_Semester_Id,
                        SessionSemesterSequenceNumber = sr.Sequence_Number,
                    }).OrderBy(x => x.LevelId).ToList();

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StatementOfResultSummary> GetStatementOfResultSummaryBy(SessionSemester sessionSemester, Level level,
            Programme programme, Department department, Student student)
        {
            try
            {
                if (level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null ||
                    department.Id <= 0 || student == null || student.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Master Result Sheet not set! Please check your input criteria selection and try again.");
                }


                List<Result> results =
                    (from sr in
                        repository.GetBy<VW_STUDENT_RESULT_SUMMARY>(
                            x =>
                                x.Level_Id == level.Id && x.Programme_Id == programme.Id &&
                                x.Department_Id == department.Id && x.Person_Id == student.Id)
                        select new Result
                        {
                            StudentId = sr.Person_Id,
                            Sex = sr.Sex_Name,
                            Name = sr.Name,
                            MatricNumber = sr.Matric_Number,
                            CourseUnit = (int) sr.Course_Unit,
                            FacultyName = sr.Faculty_Name,
                            TestScore = sr.Test_Score,
                            ExamScore = sr.Exam_Score,
                            Score = sr.Total_Score,
                            SessionSemesterId = sr.Session_Semester_Id,
                            SessionSemesterSequenceNumber = sr.Session_Semester_Sequence_Number,
                            GradePoint = sr.Grade_Point,
                            GPA = sr.GPA,
                            WGP = sr.WGP,
                            UnitPassed = sr.Unit_Passed,
                            UnitOutstanding = sr.Unit_Outstanding,
                            GPCU = sr.Grade_Point*sr.Course_Unit,
                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                        }).ToList();

                var resultSummaries = new List<StatementOfResultSummary>();
                if (results != null && results.Count > 0)
                {
                    var sessionSemesterLogic = new SessionSemesterLogic();
                    SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);

                    Result currentSemesterResult =results.Where(r => r.SessionSemesterId == sessionSemester.Id).FirstOrDefault();
                    Result previousSemesterResult =results.Where(r => r.SessionSemesterSequenceNumber == ss.SequenceNumber - 1).FirstOrDefault();

                    var unitsAttempted = new StatementOfResultSummary();
                    var wgp = new StatementOfResultSummary();
                    var gpa = new StatementOfResultSummary();
                    var unitPassed = new StatementOfResultSummary();
                    var unitsOutstanding = new StatementOfResultSummary();

                    unitsAttempted.Item = "UNITS REGISTERED";
                    wgp.Item = "SUM OF GRADE POINTS";
                    gpa.Item = "GRADE POINT AVERAGE";
                    unitPassed.Item = "UNITS PASSED";
                    unitsOutstanding.Item = "CARRYOVER UNITS";

                    if (previousSemesterResult != null)
                    {
                        unitsAttempted.PreviousSemester = previousSemesterResult.CourseUnit.ToString();
                        wgp.PreviousSemester = previousSemesterResult.WGP.ToString();
                        gpa.PreviousSemester = Math.Round((decimal) previousSemesterResult.GPA, 2).ToString();

                        unitPassed.PreviousSemester = previousSemesterResult.UnitPassed.ToString();
                        unitsOutstanding.PreviousSemester = previousSemesterResult.UnitOutstanding.ToString();
                    }

                    if (currentSemesterResult != null)
                    {
                        unitsAttempted.CurrentSemester = currentSemesterResult.CourseUnit.ToString();
                        wgp.CurrentSemester = currentSemesterResult.WGP.ToString();

                        gpa.CurrentSemester = Math.Round((decimal) currentSemesterResult.GPA, 2).ToString();
                        unitPassed.CurrentSemester = currentSemesterResult.UnitPassed.ToString();
                        unitsOutstanding.CurrentSemester = currentSemesterResult.UnitOutstanding.ToString();
                    }

                    unitsAttempted.AllSemester = results.Sum(r => r.CourseUnit).ToString();
                    wgp.AllSemester = results.Sum(r => r.WGP).ToString();

                    gpa.AllSemester = Math.Round((decimal) results.Sum(r => r.GPA), 2).ToString();
                    unitPassed.AllSemester = results.Sum(r => r.UnitPassed).ToString();
                    unitsOutstanding.AllSemester = results.Sum(r => r.UnitOutstanding).ToString();

                    resultSummaries.Add(unitsAttempted);
                    resultSummaries.Add(wgp);
                    resultSummaries.Add(gpa);
                    resultSummaries.Add(unitPassed);
                    resultSummaries.Add(unitsOutstanding);
                }

                return resultSummaries;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetProcessedResutBy(Session session, Semester semester, Level level, Department department,
            Programme programme)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null ||
                    department.Id <= 0 || programme == null || programme.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                if (sessionNameInt >= 2015)
                {
                    List<Result> results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_2>(
                                p =>
                                    p.Programme_Id == programme.Id && p.Session_Id == session.Id &&
                                    p.Level_Id == level.Id && p.Department_Id == department.Id &&
                                    p.Semester_Id == semester.Id && (p.Activated == true || p.Activated == null))
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
                                FacultyName = sr.Faculty_Name,
                                TestScore = sr.Test_Score,
                                ExamScore = sr.Exam_Score,
                                Score = sr.Total_Score,
                                Grade = sr.Grade,
                                GradePoint = sr.Grade_Point,
                                Email = sr.Email,
                                SpecialCase = sr.Special_Case,
                                Address = sr.Contact_Address,
                                MobilePhone = sr.Mobile_Phone,
                                PassportUrl = sr.Image_File_Url,
                                GPCU = sr.Grade_Point*sr.Course_Unit,
                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                Student_Type_Id = sr.Student_Type_Id,
                                SessionName = sr.Session_Name,
                                Semestername = sr.Semester_Name,
                                LevelName = sr.Level_Name,
                                WGP = sr.WGP,
                                Activated = sr.Activated,
                                Reason = sr.Reason,
                                RejectCategory = sr.Reject_Category,
                                firstname_middle = sr.Othernames,
                                ProgrammeName = sr.Programme_Name,
                                Surname = sr.Last_Name,
                                Firstname = sr.First_Name,
                                Othername = sr.Other_Name,
                                TotalScore = sr.Total_Score,
                                SessionSemesterId = sr.Session_Semester_Id,
                                SessionSemesterSequenceNumber = sr.Sequence_Number,
                            }).ToList();

                    return results;
                }
                else
                {
                    List<Result> results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_OLD_GRADING_SYSTEM>(
                                p =>
                                    p.Programme_Id == programme.Id && p.Session_Id == session.Id &&
                                    p.Level_Id == level.Id && p.Department_Id == department.Id &&
                                    p.Semester_Id == semester.Id && (p.Activated == true || p.Activated == null))
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
                                FacultyName = sr.Faculty_Name,
                                TestScore = sr.Test_Score,
                                ExamScore = sr.Exam_Score,
                                Score = sr.Total_Score,
                                Grade = sr.Grade,
                                GradePoint = sr.Grade_Point,
                                Email = sr.Email,
                                SpecialCase = sr.Special_Case,
                                Address = sr.Contact_Address,
                                MobilePhone = sr.Mobile_Phone,
                                PassportUrl = sr.Image_File_Url,
                                GPCU = sr.Grade_Point*sr.Course_Unit,
                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                Student_Type_Id = sr.Student_Type_Id,
                                SessionName = sr.Session_Name,
                                Semestername = sr.Semester_Name,
                                LevelName = sr.Level_Name,
                                WGP = sr.WGP,
                                Activated = sr.Activated,
                                Reason = sr.Reason,
                                RejectCategory = sr.Reject_Category,
                                firstname_middle = sr.Othernames,
                                ProgrammeName = sr.Programme_Name,
                                Surname = sr.Last_Name,
                                Firstname = sr.First_Name,
                                Othername = sr.Other_Name,
                                TotalScore = sr.Total_Score,
                                SessionSemesterId = sr.Session_Semester_Id,
                                SessionSemesterSequenceNumber = sr.Sequence_Number,
                            }).ToList();

                    return results;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetStudentProcessedResultBy(Session session, Level level, Department department,
            Student student, Semester semester, Programme programme)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null ||
                    department.Id <= 0 || student == null || student.Id <= 0 || semester == null || semester.Id <= 0 ||
                    programme == null || programme.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                if (sessionNameInt >= 2015)
                {
                    List<Result> results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_2>(
                                p =>
                                    p.Person_Id == student.Id && p.Programme_Id == programme.Id &&
                                    p.Session_Id == session.Id && p.Level_Id == level.Id &&
                                    p.Department_Id == department.Id && p.Semester_Id == semester.Id &&
                                    (p.Activated == true || p.Activated == null))
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
                                FacultyName = sr.Faculty_Name,
                                DepartmentName = sr.Department_Name,
                                ProgrammeId = sr.Programme_Id,
                                TestScore = sr.Test_Score,
                                ExamScore = sr.Exam_Score,
                                Score = sr.Total_Score,
                                Grade = sr.Grade,
                                GradePoint = sr.Grade_Point,
                                Email = sr.Email,
                                Address = sr.Contact_Address,
                                MobilePhone = sr.Mobile_Phone,
                                PassportUrl = sr.Image_File_Url,
                                GPCU = sr.Grade_Point*sr.Course_Unit,
                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                Student_Type_Id = sr.Student_Type_Id,
                                SessionName = sr.Session_Name,
                                Semestername = sr.Semester_Name,
                                LevelName = sr.Level_Name,
                                WGP = sr.WGP,
                                SpecialCase = sr.Special_Case,
                                Activated = sr.Activated,
                                Reason = sr.Reason,
                                RejectCategory = sr.Reject_Category,
                                firstname_middle = sr.Othernames,
                                ProgrammeName = sr.Programme_Name,
                                Surname = sr.Last_Name,
                                Firstname = sr.First_Name,
                                Othername = sr.Other_Name,
                                TotalScore = sr.Total_Score,
                                SessionSemesterId = sr.Session_Semester_Id,
                                SessionSemesterSequenceNumber = sr.Sequence_Number,
                            }).ToList();

                    return results;
                }
                else
                {
                    List<Result> results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_OLD_GRADING_SYSTEM>(
                                p =>
                                    p.Person_Id == student.Id && p.Programme_Id == programme.Id &&
                                    p.Session_Id == session.Id && p.Level_Id == level.Id &&
                                    p.Department_Id == department.Id && p.Semester_Id == semester.Id &&
                                    (p.Activated == true || p.Activated == null))
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
                                FacultyName = sr.Faculty_Name,
                                DepartmentName = sr.Department_Name,
                                ProgrammeId = sr.Programme_Id,
                                TestScore = sr.Test_Score,
                                ExamScore = sr.Exam_Score,
                                Score = sr.Total_Score,
                                Grade = sr.Grade,
                                GradePoint = sr.Grade_Point,
                                Email = sr.Email,
                                Address = sr.Contact_Address,
                                MobilePhone = sr.Mobile_Phone,
                                PassportUrl = sr.Image_File_Url,
                                GPCU = sr.Grade_Point*sr.Course_Unit,
                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                Student_Type_Id = sr.Student_Type_Id,
                                SessionName = sr.Session_Name,
                                Semestername = sr.Semester_Name,
                                LevelName = sr.Level_Name,
                                WGP = sr.WGP,
                                SpecialCase = sr.Special_Case,
                                Activated = sr.Activated,
                                Reason = sr.Reason,
                                RejectCategory = sr.Reject_Category,
                                firstname_middle = sr.Othernames,
                                ProgrammeName = sr.Programme_Name,
                                Surname = sr.Last_Name,
                                Firstname = sr.First_Name,
                                Othername = sr.Other_Name,
                                TotalScore = sr.Total_Score,
                                SessionSemesterId = sr.Session_Semester_Id,
                                SessionSemesterSequenceNumber = sr.Sequence_Number,
                            }).ToList();

                    return results;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetDeactivatedProcessedResutBy(Session session, Semester semester, Level level,
            Department department, Programme programme)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null ||
                    department.Id <= 0 || programme == null || programme.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                if (sessionNameInt >= 2015)
                {
                    List<Result> results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_2>(
                                p =>
                                    p.Programme_Id == programme.Id && p.Session_Id == session.Id &&
                                    p.Level_Id == level.Id && p.Department_Id == department.Id &&
                                    p.Semester_Id == semester.Id && p.Activated == false)
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
                                FacultyName = sr.Faculty_Name,
                                TestScore = sr.Test_Score,
                                ExamScore = sr.Exam_Score,
                                Score = sr.Total_Score,
                                Grade = sr.Grade,
                                GradePoint = sr.Grade_Point,
                                Email = sr.Email,
                                SpecialCase = sr.Special_Case,
                                Address = sr.Contact_Address,
                                MobilePhone = sr.Mobile_Phone,
                                PassportUrl = sr.Image_File_Url,
                                GPCU = sr.Grade_Point*sr.Course_Unit,
                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                Student_Type_Id = sr.Student_Type_Id,
                                SessionName = sr.Session_Name,
                                Semestername = sr.Semester_Name,
                                LevelName = sr.Level_Name,
                                WGP = sr.WGP,
                                Activated = sr.Activated,
                                Reason = sr.Reason,
                                RejectCategory = sr.Reject_Category,
                                firstname_middle = sr.Othernames,
                                ProgrammeName = sr.Programme_Name,
                                Surname = sr.Last_Name,
                                Firstname = sr.First_Name,
                                Othername = sr.Other_Name,
                                TotalScore = sr.Total_Score,
                                SessionSemesterId = sr.Session_Semester_Id,
                                SessionSemesterSequenceNumber = sr.Sequence_Number,
                            }).ToList();

                    return results;
                }
                else
                {
                    List<Result> results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_OLD_GRADING_SYSTEM>(
                                p =>
                                    p.Programme_Id == programme.Id && p.Session_Id == session.Id &&
                                    p.Level_Id == level.Id && p.Department_Id == department.Id &&
                                    p.Semester_Id == semester.Id && p.Activated == false)
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
                                FacultyName = sr.Faculty_Name,
                                TestScore = sr.Test_Score,
                                ExamScore = sr.Exam_Score,
                                Score = sr.Total_Score,
                                Grade = sr.Grade,
                                GradePoint = sr.Grade_Point,
                                Email = sr.Email,
                                SpecialCase = sr.Special_Case,
                                Address = sr.Contact_Address,
                                MobilePhone = sr.Mobile_Phone,
                                PassportUrl = sr.Image_File_Url,
                                GPCU = sr.Grade_Point*sr.Course_Unit,
                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                Student_Type_Id = sr.Student_Type_Id,
                                SessionName = sr.Session_Name,
                                Semestername = sr.Semester_Name,
                                LevelName = sr.Level_Name,
                                WGP = sr.WGP,
                                Activated = sr.Activated,
                                Reason = sr.Reason,
                                RejectCategory = sr.Reject_Category,
                                firstname_middle = sr.Othernames,
                                ProgrammeName = sr.Programme_Name,
                                Surname = sr.Last_Name,
                                Firstname = sr.First_Name,
                                Othername = sr.Other_Name,
                                TotalScore = sr.Total_Score,
                                SessionSemesterId = sr.Session_Semester_Id,
                                SessionSemesterSequenceNumber = sr.Sequence_Number,
                            }).ToList();

                    return results;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetDeactivatedStudentProcessedResultBy(Session session, Level level, Department department,
            Student student, Semester semester, Programme programme)
        {
            try
            {
                if (session == null || session.Id < 0 || level == null || level.Id <= 0 || department == null ||
                    department.Id <= 0 || student == null || student.Id <= 0 || semester == null || semester.Id <= 0 ||
                    programme == null || programme.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Result not set! Please check your input criteria selection and try again.");
                }
                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                string[] sessionItems = sessions.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                if (sessionNameInt >= 2015)
                {
                    List<Result> results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_2>(
                                p =>
                                    p.Person_Id == student.Id && p.Programme_Id == programme.Id &&
                                    p.Session_Id == session.Id && p.Level_Id == level.Id &&
                                    p.Department_Id == department.Id && p.Semester_Id == semester.Id &&
                                    p.Activated == false)
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
                                FacultyName = sr.Faculty_Name,
                                DepartmentName = sr.Department_Name,
                                ProgrammeId = sr.Programme_Id,
                                TestScore = sr.Test_Score,
                                ExamScore = sr.Exam_Score,
                                Score = sr.Total_Score,
                                Grade = sr.Grade,
                                GradePoint = sr.Grade_Point,
                                Email = sr.Email,
                                Address = sr.Contact_Address,
                                MobilePhone = sr.Mobile_Phone,
                                PassportUrl = sr.Image_File_Url,
                                GPCU = sr.Grade_Point*sr.Course_Unit,
                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                Student_Type_Id = sr.Student_Type_Id,
                                SessionName = sr.Session_Name,
                                Semestername = sr.Semester_Name,
                                LevelName = sr.Level_Name,
                                WGP = sr.WGP,
                                SpecialCase = sr.Special_Case,
                                Activated = sr.Activated,
                                Reason = sr.Reason,
                                RejectCategory = sr.Reject_Category,
                                firstname_middle = sr.Othernames,
                                ProgrammeName = sr.Programme_Name,
                                Surname = sr.Last_Name,
                                Firstname = sr.First_Name,
                                Othername = sr.Other_Name,
                                TotalScore = sr.Total_Score,
                                SessionSemesterId = sr.Session_Semester_Id,
                                SessionSemesterSequenceNumber = sr.Sequence_Number,
                            }).ToList();

                    return results;
                }
                else
                {
                    List<Result> results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_OLD_GRADING_SYSTEM>(
                                p =>
                                    p.Person_Id == student.Id && p.Programme_Id == programme.Id &&
                                    p.Session_Id == session.Id && p.Level_Id == level.Id &&
                                    p.Department_Id == department.Id && p.Semester_Id == semester.Id &&
                                    p.Activated == false)
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
                                FacultyName = sr.Faculty_Name,
                                DepartmentName = sr.Department_Name,
                                ProgrammeId = sr.Programme_Id,
                                TestScore = sr.Test_Score,
                                ExamScore = sr.Exam_Score,
                                Score = sr.Total_Score,
                                Grade = sr.Grade,
                                GradePoint = sr.Grade_Point,
                                Email = sr.Email,
                                Address = sr.Contact_Address,
                                MobilePhone = sr.Mobile_Phone,
                                PassportUrl = sr.Image_File_Url,
                                GPCU = sr.Grade_Point*sr.Course_Unit,
                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                                Student_Type_Id = sr.Student_Type_Id,
                                SessionName = sr.Session_Name,
                                Semestername = sr.Semester_Name,
                                LevelName = sr.Level_Name,
                                WGP = sr.WGP,
                                SpecialCase = sr.Special_Case,
                                Activated = sr.Activated,
                                Reason = sr.Reason,
                                RejectCategory = sr.Reject_Category,
                                firstname_middle = sr.Othernames,
                                ProgrammeName = sr.Programme_Name,
                                Surname = sr.Last_Name,
                                Firstname = sr.First_Name,
                                Othername = sr.Other_Name,
                                TotalScore = sr.Total_Score,
                                SessionSemesterId = sr.Session_Semester_Id,
                                SessionSemesterSequenceNumber = sr.Sequence_Number,
                            }).ToList();

                    return results;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Result>> GetStudentResultByAsync(SessionSemester sessionSemester, Level level, Programme programme,
            Department department, Student student)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 ||
                    programme == null || programme.Id <= 0 || department == null || department.Id <= 0 ||
                    student == null || student.Id <= 0)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                var sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester ss = await sessionSemesterLogic.GetByAsync(sessionSemester.Id);
                var sessionLogic = new SessionLogic();
                Session sessions = await sessionLogic.GetModelByAsync(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results =
                    (from sr in
                        await repository.GetByAsync<VW_STUDENT_RESULT_2>(
                            x =>
                                x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                 x.Programme_Id == programme.Id &&
                                 x.Person_Id == student.Id)
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
                            FacultyName = sr.Faculty_Name,
                            TestScore = sr.Test_Score,
                            ExamScore = sr.Exam_Score,
                            Score = sr.Total_Score,
                            Grade = sr.Grade,
                            GradePoint = sr.Grade_Point,
                            Email = sr.Email,
                            Address = sr.Contact_Address,
                            MobilePhone = sr.Mobile_Phone,
                            PassportUrl = sr.Image_File_Url,
                            GPCU = sr.Grade_Point*sr.Course_Unit,
                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                            Semestername = sr.Semester_Name,
                            SessionName = sr.Session_Name,
                            LevelName = sr.Level_Name,
                            ProgrammeName = sr.Programme_Name,
                            DepartmentName=sr.Department_Name
                        }).ToList();

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetStudentResultBy(SessionSemester sessionSemester, Level level, Programme programme,
          Department department, Student student)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 ||
                    programme == null || programme.Id <= 0 || department == null || department.Id <= 0 ||
                    student == null || student.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                var sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results =
                    (from sr in
                        repository.GetBy<VW_STUDENT_RESULT_2>(
                            x =>
                                x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                 x.Programme_Id == programme.Id &&
                                x.Department_Id == department.Id && x.Person_Id == student.Id)
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
                         FacultyName = sr.Faculty_Name,
                         TestScore = sr.Test_Score,
                         ExamScore = sr.Exam_Score,
                         Score = sr.Total_Score,
                         Grade = sr.Grade,
                         GradePoint = sr.Grade_Point,
                         Email = sr.Email,
                         Address = sr.Contact_Address,
                         MobilePhone = sr.Mobile_Phone,
                         PassportUrl = sr.Image_File_Url,
                         GPCU = sr.Grade_Point * sr.Course_Unit,
                         TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                         Semestername = sr.Semester_Name,
                         SessionName = sr.Session_Name,
                         LevelName = sr.Level_Name,
                         ProgrammeName = sr.Programme_Name
                     }).ToList();

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<Result> GetStudentResultBy(SessionSemester sessionSemester, Level level, Programme programme,
            Department department, Course course)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 ||
                    programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || course == null ||
                    course.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                var sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);

                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);

                List<Result> results =
                    (from sr in
                        repository.GetBy<VW_STUDENT_RESULT_2>(
                            x =>
                                x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                x.Level_Id == level.Id && x.Programme_Id == programme.Id &&
                                x.Department_Id == department.Id && x.Course_Id == course.Id)
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
                            FacultyName = sr.Faculty_Name,
                            TestScore = sr.Test_Score,
                            ExamScore = sr.Exam_Score,
                            Score = sr.Total_Score,
                            Grade = sr.Grade,
                            GradePoint = sr.Grade_Point,
                            GPCU = sr.Grade_Point*sr.Course_Unit,
                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                        }).ToList();

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetStudentResultBy(Session session, Semester semester, Level level, Programme programme,
            Department department, Course course)
        {
            try
            {
                if (session == null || session.Id <= 0 || semester == null || semester.Id <= 0 || level == null ||
                    level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 ||
                    course == null || course.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }


                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);


                List<Result> results =
                    (from sr in
                        repository.GetBy<VW_STUDENT_RESULT_2>(
                            x =>
                                x.Session_Id == session.Id && x.Semester_Id == semester.Id &&
                                x.Programme_Id == programme.Id && x.Department_Id == department.Id &&
                                x.Course_Id == course.Id)
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
                            FacultyName = sr.Faculty_Name,
                            TestScore = sr.Test_Score,
                            ExamScore = sr.Exam_Score,
                            Score = sr.Total_Score,
                            Grade = sr.Grade,
                            GradePoint = sr.Grade_Point,
                            DepartmentName = sr.Department_Name,
                            SessionName = sr.Session_Name,
                            Semestername = sr.Semester_Name,
                            ProgrammeName = sr.Programme_Name,
                            GPCU = sr.Grade_Point*sr.Course_Unit,
                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                            LevelName = sr.Level_Name,
                            DateUploaded = sr.Date_Uploaded.ToString(),
                        }).OrderBy(a => a.MatricNumber).ToList();

                StudentResultDetailLogic studentResultDetailLogic = new StudentResultDetailLogic();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();

                SessionSemester sessionSemester = sessionSemesterLogic.GetModelBy(s => s.Session_Id == session.Id && s.Semester_Id == semester.Id);

                List<StudentResultDetail> resultDetails = studentResultDetailLogic.GetModelsBy(
                                                                        s =>
                                                                            s.Course_Id == course.Id && s.STUDENT_RESULT.Programme_Id == programme.Id &&
                                                                            s.STUDENT_RESULT.Department_Id == department.Id
                                                                            && s.STUDENT_RESULT.Level_Id == level.Id &&
                                                                            s.STUDENT_RESULT.Session_Semester_Id == sessionSemester.Id);
                DateTime uploadDate =  DateTime.Now;
                //DateTime uploadDate = resultDetails.FirstOrDefault().Header.DateUploaded ;

                string aa = results.Count(s => s.Grade == "A ").ToString();
                string b = results.Count(s => s.Grade == "B ").ToString();
                string c = results.Count(s => s.Grade == "C ").ToString();
                string d = results.Count(s => s.Grade == "D ").ToString();
                //string e = results.Count(s => s.Grade == "E ").ToString();
                string f = results.Count(s => s.Grade == "F ").ToString();

                for (int i = 0; i < results.Count; i++)
                {
                    results[i].DateUploaded = uploadDate.ToLongDateString();
                    results[i].Remark = "A: " + aa + ", B: " + b + ", C: " + c + ", D: " + d + ", F: " + f;
                }

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetStudentGSTResultBy(Session session, Semester semester, Level level, Programme programme,
        Department department, Course course)
    {
        try
        {
            if ( level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || course == null )
            {
                throw new Exception(
                    "One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
            }

                SessionLogic sessionLogic = new SessionLogic();
                SemesterLogic semesterLogic = new SemesterLogic();

                session = sessionLogic.GetModelBy(s => s.Session_Id == session.Id);
                semester = semesterLogic.GetModelBy(s => s.Semester_Id == semester.Id);
                var semesterName = semester.Name.ToUpper();

            int? value = 0;
            if (level.Id == 1)
            {
                  List<Result> results =
                (from sr in
                    repository.GetBy<VW_GST_RESULT>(
                        x =>
                            x.Level_Id == level.Id && 
                            x.Programme_Id == programme.Id  &&
                            x.Department_Id == department.Id &&
                            x.Session_Id == session.Id &&
                            x.SESSION_NAME == semesterName &&
                            x.Semester_Id == semester.Id &&
                            x.COURSE_CODE == course.Name)
                    select new Result
                    {
                        StudentId = (Int64)sr.ID,
                        Name = sr.CORRECT_NAME ?? sr.FULLNAME,
                        MatricNumber = sr.Matric_Number ?? "-",
                        CourseCode = sr.COURSE_CODE,
                        CourseName = sr.COURSE_TITLE,
                        FacultyName = sr.Faculty_Name ?? "-",
                        TestScore = Convert.ToInt64(sr.CA),
                        ExamScore = Convert.ToInt64(sr.RAW_SCORE),
                        Score = Convert.ToInt64(sr.CA) + Convert.ToInt64(sr.RAW_SCORE),
                        Grade = GetGrade(Convert.ToInt64(sr.CA) + Convert.ToInt64(sr.RAW_SCORE), sr.Matric_Number) ?? "",
                        DepartmentName = sr.Department_Name ?? "-",
                        SessionName = session.Name.ToUpper(),
                        Semestername = semester.Name.ToUpper(),
                        ProgrammeName = sr.Programme_Name ?? "",
                        LevelName = GetLevel(level.Id)
                    }).OrderByDescending(a => a.Name).ToList();

                  List<Result> results2 = GetUnregisteredStudents(session, semester, programme, department, course, semesterName);


                   results.AddRange(results2);
                   return results;
            }
            else
            {
                 List<Result> results =
                (from sr in
                    repository.GetBy<VW_GST_RESULT>(
                        x =>
                            x.Level_Id == level.Id &&
                            x.Programme_Id == programme.Id &&
                            x.Department_Id == department.Id &&
                            x.Session_Id == session.Id &&
                            x.SESSION_NAME == semesterName &&
                            x.Semester_Id == semester.Id &&
                            x.COURSE_CODE == course.Name)
                    select new Result
                    {
                        StudentId = (Int64)sr.ID,
                        Name = sr.CORRECT_NAME ?? sr.FULLNAME,
                        MatricNumber = sr.Matric_Number ?? "-",
                        CourseCode = sr.COURSE_CODE,
                        CourseName = sr.COURSE_TITLE,
                        FacultyName = sr.Faculty_Name,
                        TestScore = Convert.ToInt64(sr.CA),
                        ExamScore = Convert.ToInt64(sr.RAW_SCORE),
                        Score = Convert.ToInt64(sr.CA) + Convert.ToInt64(sr.RAW_SCORE) ,
                        Grade = GetGrade(Convert.ToInt64(sr.CA) + Convert.ToInt64(sr.RAW_SCORE),sr.Matric_Number),
                        DepartmentName = sr.Department_Name,
                        SessionName = session.Name.ToUpper(),
                        Semestername = semester.Name.ToUpper(),
                        ProgrammeName = sr.Programme_Name,
                        LevelName = GetLevel(level.Id)
                    }).OrderBy(a => a.Name).ToList();

            return results;
            }
    
          
        }
        catch (Exception)
        {
            throw;
        }
    }

        private List<Result> GetUnregisteredStudents(Session session, Semester semester, Programme programme, Department department, Course course, string semesterName)
        {
            List<Result> results2 =
          (from sx in
               repository.GetBy<GST_SCAN_RESULT>(x =>
                       x.DEPARTMENT == department.Name &&
                       x.Session_Id == session.Id &&
                       x.SESSION_NAME == semesterName &&
                       x.Semester_Id == semester.Id &&
                       x.COURSE_CODE == course.Name &&
                       x.EXAMNO.StartsWith("7"))
           select new Result
           {
               StudentId = (Int64)sx.ID,
               Name = sx.FULLNAME,
               MatricNumber = sx.EXAMNO ?? "-",
               CourseCode = sx.COURSE_CODE,
               CourseName = sx.COURSE_TITLE,
               FacultyName = "-",
               TestScore = Convert.ToInt64(sx.CA),
               ExamScore = Convert.ToInt64(sx.RAW_SCORE),
               Score = Convert.ToInt64(sx.CA) + Convert.ToInt64(sx.RAW_SCORE),
               Grade = GetGrade(Convert.ToInt64(sx.CA) + Convert.ToInt64(sx.RAW_SCORE), sx.EXAMNO) ?? "",
               DepartmentName = department.Name.ToUpper(),
               SessionName = session.Name.ToUpper(),
               Semestername = semester.Name.ToUpper(),
               ProgrammeName = programme.Name.ToUpper(),
               LevelName = GetLevel(1)
           }).OrderByDescending(a => a.Name).ToList();
            return results2;
        }
        public List<Result> GetStudentGSTResult(Session session, Semester semester, Level level, Faculty faculty, Department department, Course course)
        {
            try
            {
                if (level == null || level.Id <= 0 || faculty == null || faculty.Id <= 0 || department == null || department.Id <= 0 || course == null)
                {
                    throw new Exception("One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }


                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == session.Id);
                SemesterLogic semesterLogic = new SemesterLogic();
                semester = semesterLogic.GetModelBy(s => s.Semester_Id == semester.Id);
                var semesterName = semester.Name.ToUpper();
                int? value = 0;
                if (level.Id == 1)
                {
                    List<Result> results = (from sr in repository.GetBy<VW_GST_RESULT>(x => x.Level_Id == level.Id && x.Faculty_Id == faculty.Id && x.Department_Id == department.Id &&
                                                                                          x.SESSION_NAME == semester.Name.ToUpper() &&
                                                                                          x.COURSE_CODE == course.Name && x.Session_Id==sessions.Id && x.Semester_Id == semester.Id)
                                            select new Result
                                            {
                                                StudentId = (Int64)sr.ID,
                                                Name = sr.CORRECT_NAME ?? sr.FULLNAME,
                                                MatricNumber = sr.Matric_Number ?? "-",
                                                CourseCode = sr.COURSE_CODE,
                                                CourseName = sr.COURSE_TITLE,
                                                FacultyName = sr.Faculty_Name ?? "-",
                                                TestScore = Convert.ToInt64(sr.CA),
                                                ExamScore = Convert.ToInt64(sr.RAW_SCORE),
                                                Score = Convert.ToInt64(sr.CA) + Convert.ToInt64(sr.RAW_SCORE),
                                                Grade = GetGrade(Convert.ToInt64(sr.CA) + Convert.ToInt64(sr.RAW_SCORE), sr.Matric_Number) ?? "",
                                                DepartmentName = sr.Department_Name ?? "-",
                                                SessionName = session.Name,
                                                Semestername = semester.Name,
                                                ProgrammeName = sr.Programme_Name ?? "-",
                                                LevelName = level.Name.ToUpper() ?? "-"
                                            }).OrderByDescending(a => a.Name).ToList();
                    List<Result> results2 = GetUnregisteredStudentsByBulk(session, semester, department, course, semesterName);
                    results.AddRange(results2);
                    return results;
                }
                else
                {
                    List<Result> results = (from sr in repository.GetBy<VW_GST_RESULT>(x => x.Level_Id == level.Id && x.Faculty_Id == faculty.Id && x.Department_Id == department.Id &&
                                                                                       x.SESSION_NAME == semester.Name.ToUpper() &&
                                                                                       x.COURSE_CODE == course.Name && x.Session_Id == sessions.Id && x.Semester_Id == semester.Id)
                                            select new Result
                                            {
                                                StudentId = (Int64)sr.ID,
                                                Name = sr.CORRECT_NAME ?? sr.FULLNAME,
                                                MatricNumber = sr.Matric_Number ?? "-",
                                                CourseCode = sr.COURSE_CODE,
                                                CourseName = sr.COURSE_TITLE,
                                                FacultyName = sr.Faculty_Name,
                                                TestScore = Convert.ToInt64(sr.CA),
                                                ExamScore = Convert.ToInt64(sr.RAW_SCORE),
                                                Score = Convert.ToInt64(sr.CA) + Convert.ToInt64(sr.RAW_SCORE),
                                                Grade = GetGrade(Convert.ToInt64(sr.CA) + Convert.ToInt64(sr.RAW_SCORE), sr.Matric_Number),
                                                DepartmentName = sr.Department_Name,
                                                SessionName = session.Name,
                                                Semestername = semester.Name,
                                                ProgrammeName = sr.Programme_Name,
                                                LevelName = level.Name.ToUpper() ?? "-"
                                            }).OrderBy(a => a.Name).ToList();

                    return results;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<Result> GetUnregisteredStudentsByBulk(Session session, Semester semester, Department department, Course course, string semesterName )
        {
            List<Result> results2 =
          (from sx in
               repository.GetBy<GST_SCAN_RESULT>(x =>
                       x.DEPARTMENT == department.Name &&
                       x.Session_Id == session.Id &&
                       x.SESSION_NAME == semesterName &&
                       x.Semester_Id == semester.Id &&
                       x.COURSE_CODE == course.Name &&
                       x.EXAMNO.StartsWith("8"))
           select new Result
           {
               StudentId = (Int64)sx.ID,
               Name = sx.FULLNAME,
               MatricNumber = sx.EXAMNO ?? "-",
               CourseCode = sx.COURSE_CODE,
               CourseName = sx.COURSE_TITLE,
               FacultyName = "-",
               TestScore = Convert.ToInt64(sx.CA),
               ExamScore = Convert.ToInt64(sx.RAW_SCORE),
               Score = Convert.ToInt64(sx.CA) + Convert.ToInt64(sx.RAW_SCORE),
               Grade = GetGrade(Convert.ToInt64(sx.CA) + Convert.ToInt64(sx.RAW_SCORE), sx.EXAMNO) ?? "",
               DepartmentName = department.Name.ToUpper(),
               SessionName = session.Name.ToUpper(),
               Semestername = semester.Name.ToUpper(),
               LevelName = GetLevel(1)
           }).OrderByDescending(a => a.Name).ToList();
            return results2;
        }
        private string GetGrade(decimal? total, string MatricNo)
        {
            try
            {
                string grade = "F";
                if (total != null && total > 0)
                {
                    if (MatricNo.Contains("15/") || MatricNo.Contains("2016") || MatricNo.Contains("2017") || MatricNo.Contains("2018"))
                    {
                        if (total >= 70M && total <= 100M)
                        {
                            grade = "A";
                        }
                        if (total >= 60M && total <= 69M)
                        {
                            grade = "B";
                        }
                        if (total >= 50M && total <= 59M)
                        {
                            grade = "C";
                        }
                        if (total >= 45M && total <= 49M)
                        {
                            grade = "D";
                        }
                        if (total >= 40M && total <= 44M)
                        {
                            grade = "F";
                        }   
                        if (total >= 0 && total <= 39M)
                        {
                            grade = "F";
                        }
                    }
                    else
                    {
                        if (total >= 70M && total <= 100M)
                        {
                            grade = "A";
                        }
                        if (total >= 60M && total <= 69M)
                        {
                            grade = "B";
                        }
                        if (total >= 50M && total <= 59M)
                        {
                            grade = "C";
                        }
                        if (total >= 45M && total <= 49M)
                        {
                            grade = "D";
                        }
                        if (total >= 40M && total <= 44M)
                        {
                            grade = "E";
                        }   
                        if (total >= 0 && total <= 39M)
                        {
                            grade = "F";
                        }
                    }
                    
                }

                return grade;
            }
            catch (Exception)
            {    
                throw;
            }
        }

        private string GetLevel(int levelId)
        {
            string Level = "100 Level";
            try
            {

                if (levelId == 1)
                {
                    Level = "100 Level";
                }
                else if (levelId == 2)
                {
                    Level = "200 Level";
                }
                else if (levelId == 3)
                {
                    Level = "300 Level";
                }
                else if (levelId == 4)
                {
                    Level = "400 Level";
                }
                else if (levelId == 5)
                {
                    Level = "500 Level";
                }
                else if (levelId == 6)
                {
                    Level = "600 Level";
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Level;
        }

        public List<Result> GetMaterSheetBy(SessionSemester sessionSemester, Level level, Programme programme,
            Department department)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 ||
                    programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                var sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                if (sessionNameInt > 2015)
                {
                    List<Result> results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_2>(
                                x =>
                                    x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                    x.Level_Id == level.Id && x.Programme_Id == programme.Id &&
                                    x.Department_Id == department.Id)
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
                                SpecialCase = sr.Special_Case,
                                TestScore = sr.Test_Score,
                                ExamScore = sr.Exam_Score,
                                Score = sr.Total_Score,
                                Grade = sr.Grade,
                                GradePoint = sr.Grade_Point,
                                GPCU = sr.Grade_Point*sr.Course_Unit,
                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                            }).ToList();


                    return results;
                }
                else
                {
                    List<Result> results =
                        (from sr in
                            repository.GetBy<VW_STUDENT_RESULT_OLD_GRADING_SYSTEM>(
                                x =>
                                    x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                    x.Level_Id == level.Id && x.Programme_Id == programme.Id &&
                                    x.Department_Id == department.Id)
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
                                SpecialCase = sr.Special_Case,
                                TestScore = sr.Test_Score,
                                ExamScore = sr.Exam_Score,
                                Score = sr.Total_Score,
                                Grade = sr.Grade,
                                GradePoint = sr.Grade_Point,
                                GPCU = sr.Grade_Point*sr.Course_Unit,
                                TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                            }).ToList();


                    return results;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Result> GetMaterSheetDetailsBy(SessionSemester sessionSemester, Level level, Programme programme,
            Department department)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 ||
                    programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                var sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;


                results =
                    (from sr in
                        repository.GetBy<VW_STUDENT_RESULT_2>(
                            x =>
                                x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                x.Level_Id == level.Id && x.Programme_Id == programme.Id &&
                                x.Department_Id == department.Id)
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
                            SpecialCase = sr.Special_Case,
                            TestScore = sr.Test_Score,
                            ExamScore = sr.Exam_Score,
                            Score = sr.Total_Score,
                            Grade = sr.Grade,
                            GradePoint = sr.Grade_Point,
                            DepartmentName = sr.Department_Name,
                            ProgrammeName = sr.Programme_Name,
                            LevelName = levels,
                            Semestername = sr.Semester_Name,
                            GPCU = sr.Grade_Point*sr.Course_Unit,
                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                            LevelId = sr.Level_Id,
                            FacultyName = sr.Faculty_Name,
                            SessionName = sr.Session_Name
                        }).ToList();


                sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
                List<Result> studentsResult = GetResultList(sessionSemester, level, programme, department).ToList();
                var masterSheetResult = new List<Result>();
                foreach (Result resultItem in studentsResult)
                {
                    resultItem.Identifier = identifier;
                    Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme,
                        department);
                    masterSheetResult.Add(result);
                }


                foreach (Result result in masterSheetResult)
                {
                    List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
                    foreach (Result resultItem in studentResults)
                    {
                        resultItem.Identifier = identifier;
                        resultItem.CGPA = result.CGPA;
                        resultItem.Remark = result.Remark;
                        resultItem.GPA = result.GPA;
                    }
                }

                return results.OrderBy(a => a.MatricNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    
             public List<Result> GetMaterSheetCourseDetailsBy(SessionSemester sessionSemester, Level level, Programme programme,
            Department department)
        {
            try
            {
                if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 ||
                    programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get Mater Result Sheet not set! Please check your input criteria selection and try again.");
                }

                var sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);
                var sessionLogic = new SessionLogic();
                Session sessions = sessionLogic.GetModelBy(p => p.Session_Id == ss.Session.Id);
                string[] sessionItems = ss.Session.Name.Split('/');
                string sessionNameStr = sessionItems[0];
                int sessionNameInt = Convert.ToInt32(sessionNameStr);
                List<Result> results = null;

                string identifier = null;
                string departmentCode = GetDepartmentCode(department.Id);
                string levels = GetLevelName(level.Id);
                string semesterCode = GetSemesterCodeBy(ss.Semester.Id);
                string sessionCode = GetSessionCodeBy(ss.Session.Name);
                identifier = departmentCode + levels + semesterCode + sessionCode;


                results =
                    (from sr in
                        repository.GetBy<VW_STUDENT_RESULT_2>(
                            x =>
                                x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id &&
                                x.Level_Id == level.Id && x.Programme_Id == programme.Id &&
                                x.Department_Id == department.Id)
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
                            SpecialCase = sr.Special_Case,
                            TestScore = sr.Test_Score,
                            ExamScore = sr.Exam_Score,
                            Score = 70,
                            Grade = sr.Grade,
                            GradePoint = sr.Grade_Point,
                            DepartmentName = sr.Department_Name,
                            ProgrammeName = sr.Programme_Name,
                            LevelName = levels,
                            Semestername = sr.Semester_Name,
                            GPCU = sr.Grade_Point*sr.Course_Unit,
                            TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
                            LevelId = sr.Level_Id,
                            FacultyName = sr.Faculty_Name,
                            SessionName = sr.Session_Name
                        }).ToList();

                return results.OrderBy(a => a.MatricNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<Result> GetResultList(SessionSemester sessionSemester, Level level, Programme programme,
            Department department)
        {
            try
            {
                var filteredResult = new List<Result>();
                var studentResultLogic = new StudentResultLogic();
                List<string> resultList =
                    studentResultLogic.GetProcessedResutBy(sessionSemester.Session, sessionSemester.Semester, level,
                        department, programme).Select(p => p.MatricNumber).AsParallel().Distinct().ToList();
                List<Result> result = studentResultLogic.GetProcessedResutBy(sessionSemester.Session,
                    sessionSemester.Semester, level, department, programme);
                foreach (string item in resultList)
                {
                    Result resultItem = result.Where(p => p.MatricNumber == item).FirstOrDefault();
                    filteredResult.Add(resultItem);
                }
                return filteredResult.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Result ViewProcessedStudentResult(long id, SessionSemester sessionSemester, Level level,
            Programme programme, Department department)
        {
            var ProcessedResult = new Result();
            string Remark = null;
            try
            {
                if (id > 0)
                {
                    var student = new Student {Id = id};
                    var studentLogic = new StudentLogic();
                    var studentResultLogic = new StudentResultLogic();
                    if (sessionSemester.Semester != null && sessionSemester.Session != null && programme != null &&
                        department != null && level != null)
                    {
                        if (level.Id == 1 || level.Id == 3)
                        {
                            Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == id);
                            if (sessionSemester.Semester.Id == 1)
                            {
                                List<Result> result = null;
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session,
                                        level, department, student, sessionSemester.Semester, programme);
                                }
                                //else
                                //{
                                //    result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(sessionSemester.Session, level, department, student, sessionSemester.Semester, programme);
                                //}
                                var modifiedResultList = new List<Result>();

                                int totalSemesterCourseUnit = 0;
                                foreach (Result resultItem in result)
                                {
                                    decimal WGP = 0;

                                    if (resultItem.SpecialCase != null)
                                    {
                                        resultItem.GPCU = 0;
                                        if (totalSemesterCourseUnit == 0)
                                        {
                                            totalSemesterCourseUnit = (int) resultItem.TotalSemesterCourseUnit -
                                                                      resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                        else
                                        {
                                            totalSemesterCourseUnit -= resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                    }
                                    if (totalSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    }
                                    modifiedResultList.Add(resultItem);
                                }
                                decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                                int? firstSemesterTotalSemesterCourseUnit = 0;
                                Result firstYearFirstSemesterResult = modifiedResultList.FirstOrDefault();
                                firstSemesterTotalSemesterCourseUnit =
                                    modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                                decimal? firstSemesterGPA = firstSemesterGPCUSum/firstSemesterTotalSemesterCourseUnit;
                                firstYearFirstSemesterResult.GPA = Decimal.Round((decimal) firstSemesterGPA, 2);
                                firstYearFirstSemesterResult.CGPA = Decimal.Round((decimal) firstSemesterGPA, 2);
                                firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
                                firstYearFirstSemesterResult.TotalSemesterCourseUnit =
                                    firstSemesterTotalSemesterCourseUnit;
                                Remark = GetGraduationStatus(firstYearFirstSemesterResult.CGPA,
                                    GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student));
                                firstYearFirstSemesterResult.Remark = Remark;
                                ProcessedResult = firstYearFirstSemesterResult;
                            }
                            else
                            {
                                List<Result> result = null;
                                var firstSemester = new Semester {Id = 1};
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session,
                                        level, department, student, firstSemester, programme);
                                }
                                else
                                {
                                    result =
                                        studentResultLogic.GetDeactivatedStudentProcessedResultBy(
                                            sessionSemester.Session, level, department, student, firstSemester,
                                            programme);
                                }
                                var firstSemesterModifiedResultList = new List<Result>();
                                int totalFirstSemesterCourseUnit = 0;
                                foreach (Result resultItem in result)
                                {
                                    decimal WGP = 0;

                                    if (resultItem.SpecialCase != null)
                                    {
                                        resultItem.GPCU = 0;
                                        if (totalFirstSemesterCourseUnit == 0)
                                        {
                                            totalFirstSemesterCourseUnit = (int) resultItem.TotalSemesterCourseUnit -
                                                                           resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalFirstSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                        else
                                        {
                                            totalFirstSemesterCourseUnit -= resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalFirstSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                    }
                                    if (totalFirstSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalFirstSemesterCourseUnit;
                                    }
                                    firstSemesterModifiedResultList.Add(resultItem);
                                }
                                decimal? firstSemesterGPCUSum = firstSemesterModifiedResultList.Sum(p => p.GPCU);
                                int? firstSemesterTotalSemesterCourseUnit = 0;
                                Result firstYearFirstSemesterResult = firstSemesterModifiedResultList.FirstOrDefault();
                                firstSemesterTotalSemesterCourseUnit =
                                    firstSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                                decimal? firstSemesterGPA = firstSemesterGPCUSum/firstSemesterTotalSemesterCourseUnit;
                                firstYearFirstSemesterResult.GPA = Decimal.Round((decimal) firstSemesterGPA);

                                var secondSemester = new Semester {Id = 2};
                                List<Result> secondSemesterResult = null;
                                if (studentCheck.Activated == true || studentCheck.Activated == null)
                                {
                                    secondSemesterResult =
                                        studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level,
                                            department, student, secondSemester, programme);
                                }
                                else
                                {
                                    secondSemesterResult =
                                        studentResultLogic.GetDeactivatedStudentProcessedResultBy(
                                            sessionSemester.Session, level, department, student, secondSemester,
                                            programme);
                                }
                                var secondSemesterModifiedResultList = new List<Result>();

                                int totalSecondSemesterCourseUnit = 0;
                                foreach (Result resultItem in secondSemesterResult)
                                {
                                    decimal WGP = 0;

                                    if (resultItem.SpecialCase != null)
                                    {
                                        resultItem.GPCU = 0;
                                        if (totalSecondSemesterCourseUnit == 0)
                                        {
                                            totalSecondSemesterCourseUnit = (int) resultItem.TotalSemesterCourseUnit -
                                                                            resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                        else
                                        {
                                            totalSecondSemesterCourseUnit -= resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                    }
                                    if (totalSecondSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                    }
                                    secondSemesterModifiedResultList.Add(resultItem);
                                }
                                decimal? secondSemesterGPCUSum = secondSemesterModifiedResultList.Sum(p => p.GPCU);
                                Result secondSemesterStudentResult = secondSemesterModifiedResultList.FirstOrDefault();

                                secondSemesterStudentResult.GPA =
                                    Decimal.Round(
                                        (decimal)
                                            (secondSemesterGPCUSum/
                                             (decimal)
                                                 (secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit))),
                                        2);
                                secondSemesterStudentResult.CGPA =
                                    Decimal.Round(
                                        (decimal)
                                            ((firstSemesterGPCUSum + secondSemesterGPCUSum)/
                                             (secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit) +
                                              firstSemesterTotalSemesterCourseUnit)), 2);
                                if (secondSemesterStudentResult.GPA < 2.0M && firstYearFirstSemesterResult.GPA < 2.0M)
                                {
                                    Remark = GetGraduationStatus(firstYearFirstSemesterResult.CGPA,
                                        firstYearFirstSemesterResult.GPA, secondSemesterStudentResult.GPA,
                                        GetFirstYearCarryOverCourses(sessionSemester, level, programme, department,
                                            student));
                                }
                                else
                                {
                                    Remark = GetGraduationStatus(firstYearFirstSemesterResult.CGPA,
                                        GetFirstYearCarryOverCourses(sessionSemester, level, programme, department,
                                            student));
                                }
                                secondSemesterStudentResult.Remark = Remark;
                                ProcessedResult = secondSemesterStudentResult;
                            }
                        }
                        else
                        {
                            decimal firstYearFirstSemesterGPCUSum = 0;
                            int firstYearFirstSemesterTotalCourseUnit = 0;
                            decimal firstYearSecondSemesterGPCUSum = 0;
                            int firstYearSecondSemesterTotalCourseUnit = 0;
                            decimal secondYearFirstSemesterGPCUSum = 0;
                            int secondYearFirstSemesterTotalCourseUnit = 0;
                            decimal secondYearSecondSemesterGPCUSum = 0;
                            int secondYearSecondSemesterTotalCourseUnit = 0;

                            Result firstYearFirstSemester = GetFirstYearFirstSemesterResultInfo(sessionSemester, level,
                                programme, department, student);
                            Result firstYearSecondSemester = GetFirstYearSecondSemesterResultInfo(sessionSemester, level,
                                programme, department, student);
                            if (sessionSemester.Semester.Id == 1)
                            {
                                List<Result> result = null;
                                Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                                var semester = new Semester {Id = 1};
                                if (student.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session,
                                        level, department, student, semester, programme);
                                }
                                else
                                {
                                    result =
                                        studentResultLogic.GetDeactivatedStudentProcessedResultBy(
                                            sessionSemester.Session, level, department, student, semester, programme);
                                }
                                var modifiedResultList = new List<Result>();
                                int totalSemesterCourseUnit = 0;
                                foreach (Result resultItem in result)
                                {
                                    decimal WGP = 0;
                                    if (resultItem.SpecialCase != null)
                                    {
                                        resultItem.GPCU = 0;
                                        if (totalSemesterCourseUnit == 0)
                                        {
                                            totalSemesterCourseUnit = (int) resultItem.TotalSemesterCourseUnit -
                                                                      resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                        else
                                        {
                                            totalSemesterCourseUnit -= resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                    }
                                    if (totalSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    }
                                    modifiedResultList.Add(resultItem);
                                }
                                var secondYearFirstSemesterResult = new Result();
                                decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                                int? secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                                secondYearFirstSemesterResult = modifiedResultList.FirstOrDefault();
                                secondYearfirstSemesterTotalSemesterCourseUnit =
                                    modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                                secondYearFirstSemesterResult.TotalSemesterCourseUnit =
                                    secondYearfirstSemesterTotalSemesterCourseUnit;
                                secondYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
                                decimal? firstSemesterGPA = firstSemesterGPCUSum/
                                                            secondYearfirstSemesterTotalSemesterCourseUnit;
                                secondYearFirstSemesterResult.GPA = Decimal.Round((decimal) firstSemesterGPA);
                                if (firstYearFirstSemester != null && firstYearFirstSemester.GPCU != null &&
                                    firstYearSecondSemester != null && firstYearSecondSemester.GPCU != null)
                                {
                                    secondYearFirstSemesterResult.CGPA =
                                        Decimal.Round(
                                            (decimal)
                                                ((firstSemesterGPCUSum + firstYearFirstSemester.GPCU +
                                                  firstYearSecondSemester.GPCU)/
                                                 (firstYearSecondSemester.TotalSemesterCourseUnit +
                                                  firstYearFirstSemester.TotalSemesterCourseUnit +
                                                  secondYearfirstSemesterTotalSemesterCourseUnit)), 2);
                                }
                                else
                                {
                                    secondYearFirstSemesterResult.CGPA = secondYearFirstSemesterResult.GPA;
                                }
                                // List<string> firstYearCarryOverCourses = GetFirstYearCarryOverCourses(sessionSemester, level, programme, department, student);
                                List<string> secondYearFirstSemetserCarryOverCourses =
                                    GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);
                                secondYearFirstSemesterResult.Remark =
                                    GetGraduationStatus(secondYearFirstSemesterResult.CGPA,
                                        secondYearFirstSemetserCarryOverCourses);

                                ProcessedResult = secondYearFirstSemesterResult;
                            }
                            else if (sessionSemester.Semester.Id == 2)
                            {
                                List<Result> result = null;


                                Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                                var semester = new Semester {Id = 1};

                                if (student.Activated == true || studentCheck.Activated == null)
                                {
                                    result = studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session,
                                        level, department, student, semester, programme);
                                }
                                else
                                {
                                    result =
                                        studentResultLogic.GetDeactivatedStudentProcessedResultBy(
                                            sessionSemester.Session, level, department, student, semester, programme);
                                }
                                var modifiedResultList = new List<Result>();
                                int totalSemesterCourseUnit = 0;
                                foreach (Result resultItem in result)
                                {
                                    decimal WGP = 0;

                                    if (resultItem.SpecialCase != null)
                                    {
                                        resultItem.GPCU = 0;
                                        if (totalSemesterCourseUnit == 0)
                                        {
                                            totalSemesterCourseUnit = (int) resultItem.TotalSemesterCourseUnit -
                                                                      resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                        else
                                        {
                                            totalSemesterCourseUnit -= resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                    }
                                    if (totalSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                    }
                                    modifiedResultList.Add(resultItem);
                                }
                                var secondYearFirstSemesterResult = new Result();
                                decimal? secondYearfirstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                                int? secondYearfirstSemesterTotalSemesterCourseUnit = 0;
                                secondYearfirstSemesterTotalSemesterCourseUnit =
                                    modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                                secondYearFirstSemesterResult.TotalSemesterCourseUnit =
                                    secondYearfirstSemesterTotalSemesterCourseUnit;
                                secondYearFirstSemesterResult.GPCU = secondYearfirstSemesterGPCUSum;
                                decimal? firstSemesterGPA = secondYearfirstSemesterGPCUSum/
                                                            secondYearfirstSemesterTotalSemesterCourseUnit;
                                secondYearFirstSemesterResult.GPA = Decimal.Round((decimal) firstSemesterGPA, 2);

                                //Second semester second year

                                List<Result> secondSemesterResult = null;


                                var secondSemester = new Semester {Id = 2};

                                if (student.Activated == true || studentCheck.Activated == null)
                                {
                                    secondSemesterResult =
                                        studentResultLogic.GetStudentProcessedResultBy(sessionSemester.Session, level,
                                            department, student, secondSemester, programme);
                                }
                                else
                                {
                                    secondSemesterResult =
                                        studentResultLogic.GetDeactivatedStudentProcessedResultBy(
                                            sessionSemester.Session, level, department, student, secondSemester,
                                            programme);
                                }
                                var secondSemesterModifiedResultList = new List<Result>();
                                int totalSecondSemesterCourseUnit = 0;
                                foreach (Result resultItem in secondSemesterResult)
                                {
                                    decimal WGP = 0;

                                    if (resultItem.SpecialCase != null)
                                    {
                                        resultItem.GPCU = 0;
                                        if (totalSecondSemesterCourseUnit == 0)
                                        {
                                            totalSecondSemesterCourseUnit = (int) resultItem.TotalSemesterCourseUnit -
                                                                            resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                        else
                                        {
                                            totalSecondSemesterCourseUnit -= resultItem.CourseUnit;
                                            resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                            resultItem.Grade = "-";
                                        }
                                    }
                                    if (totalSecondSemesterCourseUnit > 0)
                                    {
                                        resultItem.TotalSemesterCourseUnit = totalSecondSemesterCourseUnit;
                                    }
                                    secondSemesterModifiedResultList.Add(resultItem);
                                }
                                var secondYearSecondtSemesterResult = new Result();
                                decimal? secondYearSecondtSemesterGPCUSum =
                                    secondSemesterModifiedResultList.Sum(p => p.GPCU);
                                int? secondYearSecondSemesterTotalSemesterCourseUnit = 0;
                                secondYearSecondSemesterTotalSemesterCourseUnit =
                                    secondSemesterModifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                                secondYearSecondtSemesterResult = secondSemesterModifiedResultList.FirstOrDefault();
                                secondYearSecondtSemesterResult.TotalSemesterCourseUnit =
                                    secondYearSecondSemesterTotalSemesterCourseUnit;
                                secondYearSecondtSemesterResult.GPCU = secondYearSecondtSemesterGPCUSum;
                                decimal? secondYearSecondSmesterGPA = secondYearSecondtSemesterGPCUSum/
                                                                      secondYearSecondSemesterTotalSemesterCourseUnit;
                                // viewModel.Result = secondSemesterModifiedResultList.FirstOrDefault();
                                secondYearSecondtSemesterResult.GPA = Decimal.Round(
                                    (decimal) secondYearSecondSmesterGPA, 2);
                                secondYearSecondtSemesterResult.CGPA =
                                    Decimal.Round(
                                        (decimal)
                                            ((secondYearfirstSemesterGPCUSum + firstYearFirstSemester.GPCU +
                                              firstYearSecondSemester.GPCU + secondYearSecondtSemesterGPCUSum)/
                                             (firstYearSecondSemester.TotalSemesterCourseUnit +
                                              firstYearFirstSemester.TotalSemesterCourseUnit +
                                              secondYearfirstSemesterTotalSemesterCourseUnit +
                                              secondYearSecondSemesterTotalSemesterCourseUnit)), 2);
                                List<string> secondYearSecondSemetserCarryOverCourses =
                                    GetSecondYearCarryOverCourses(sessionSemester, level, programme, department, student);
                                if (secondYearSecondtSemesterResult.GPA < 2.0M &&
                                    secondYearFirstSemesterResult.GPA < 2.0M)
                                {
                                    secondYearSecondtSemesterResult.Remark =
                                        GetGraduationStatus(secondYearFirstSemesterResult.CGPA,
                                            secondYearFirstSemesterResult.GPA, secondYearSecondtSemesterResult.GPA,
                                            secondYearSecondSemetserCarryOverCourses);
                                }
                                else
                                {
                                    secondYearSecondtSemesterResult.Remark =
                                        GetGraduationStatus(secondYearFirstSemesterResult.CGPA,
                                            secondYearSecondSemetserCarryOverCourses);
                                }

                                ProcessedResult = secondYearSecondtSemesterResult;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return ProcessedResult;
        }

        private List<string> GetFirstYearCarryOverCourses(SessionSemester sessionSemester, Level lvl,
            Programme programme, Department department, Student student)
        {
            try
            {
                var courseRegistrationdetails = new List<CourseRegistrationDetail>();
                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                var courseCodes = new List<string>();
                if (lvl.Id == 1 || lvl.Id == 3)
                {
                    courseRegistrationdetails =
                        courseRegistrationDetailLogic.GetModelsBy(
                            crd =>
                                crd.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id &&
                                crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id &&
                                crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id &&
                                crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id &&
                                (crd.Test_Score + crd.Exam_Score) < 40 && crd.Special_Case == null);
                    if (sessionSemester.Semester.Id == 1)
                    {
                        courseRegistrationdetails = courseRegistrationdetails.Where(p => p.Semester.Id == 1).ToList();
                        if (courseRegistrationdetails.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            {
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (courseRegistrationdetails.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            {
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                }
                            }
                        }
                    }
                }

                return courseCodes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<string> GetSecondYearCarryOverCourses(SessionSemester sessionSemester, Level lvl,
            Programme programme, Department department, Student student)
        {
            try
            {
                var courseRegistrationdetails = new List<CourseRegistrationDetail>();
                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                var studentLevelLogic = new StudentLevelLogic();
                List<string> courseCodes = courseCodes = new List<string>();
                List<string> firstYearCarryOverCourseCodes = null;
                StudentLevel studentLevel = null;
                if (lvl.Id == 2)
                {
                    studentLevel =
                        studentLevelLogic.GetModelsBy(
                            p =>
                                p.Person_Id == student.Id && p.Level_Id == 1 && p.Department_Id == department.Id &&
                                p.Programme_Id == programme.Id).FirstOrDefault(); //ND1
                    if (studentLevel != null)
                    {
                        var ss = new SessionSemester();
                        ss.Session = studentLevel.Session;
                        ss.Semester = new Semester {Id = 2}; // Second semester to get all carry over for first year
                        firstYearCarryOverCourseCodes = GetFirstYearCarryOverCourses(ss, studentLevel.Level,
                            studentLevel.Programme, studentLevel.Department, studentLevel.Student);
                    }
                }
                else if (lvl.Id == 4)
                {
                    studentLevel =
                        studentLevelLogic.GetModelsBy(
                            p =>
                                p.Person_Id == student.Id && p.Level_Id == 3 && p.Department_Id == department.Id &&
                                p.Programme_Id == programme.Id).FirstOrDefault(); //HND1
                    if (studentLevel != null)
                    {
                        var ss = new SessionSemester();
                        ss.Session = studentLevel.Session;
                        ss.Semester = new Semester {Id = 2}; // Second semester to get all carry over for first year
                        firstYearCarryOverCourseCodes = GetFirstYearCarryOverCourses(ss, studentLevel.Level,
                            studentLevel.Programme, studentLevel.Department, studentLevel.Student);
                    }
                }

                if (lvl.Id == 2 || lvl.Id == 4)
                {
                    courseRegistrationdetails =
                        courseRegistrationDetailLogic.GetModelsBy(
                            crd =>
                                crd.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id &&
                                crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id &&
                                crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id &&
                                crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id &&
                                (crd.Test_Score + crd.Exam_Score) < 40 && crd.Special_Case == null);
                    if (sessionSemester.Semester.Id == 1)
                    {
                        courseRegistrationdetails = courseRegistrationdetails.Where(p => p.Semester.Id == 1).ToList();
                        if (courseRegistrationdetails.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            {
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (courseRegistrationdetails.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            {
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                }
                            }
                        }
                    }
                }
                //compare courses
                courseCodes = CompareCourses(courseCodes, firstYearCarryOverCourseCodes, sessionSemester, lvl, programme,
                    department, student);
                return courseCodes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<string> CompareCourses(List<string> courseCodes, List<string> firstYearCarryOverCourseCodes,
            SessionSemester sessionSemester, Level lvl, Programme programme, Department department, Student student)
        {
            try
            {
                var courseRegistrationDetail = new CourseRegistrationDetailLogic();

                if (courseCodes != null && firstYearCarryOverCourseCodes != null)
                {
                    for (int i = 0; i < firstYearCarryOverCourseCodes.Count(); i++)
                    {
                        if (courseCodes.Contains(firstYearCarryOverCourseCodes[i]))
                        {
                            courseCodes.Add(firstYearCarryOverCourseCodes[i]);
                            firstYearCarryOverCourseCodes.RemoveAt(i);
                        }
                        else
                        {
                            string Coursecode = firstYearCarryOverCourseCodes[i];
                            CourseRegistrationDetail course =
                                courseRegistrationDetail.GetModelBy(
                                    p =>
                                        p.COURSE.Course_Code == Coursecode &&
                                        p.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id &&
                                        p.Semester_Id == sessionSemester.Semester.Id &&
                                        p.STUDENT_COURSE_REGISTRATION.Session_Id == sessionSemester.Session.Id);
                            if (course != null)
                            {
                                firstYearCarryOverCourseCodes.RemoveAt(i);
                            }
                            else
                            {
                                courseCodes.Add(firstYearCarryOverCourseCodes[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return courseCodes;
        }

        private Result GetFirstYearFirstSemesterResultInfo(SessionSemester sessionSemester, Level lvl,
            Programme programme, Department department, Student student)
        {
            try
            {
                List<Result> result = null;
                var firstYearFirstSemesterResult = new Result();
                var studentLogic = new StudentLogic();
                var studentResultLogic = new StudentResultLogic();
                Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);

                var semester = new Semester {Id = 1};
                Level level = null;
                if (lvl.Id == 2)
                {
                    level = new Level {Id = 1};
                }
                else
                {
                    level = new Level {Id = 3};
                }
                var studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel =
                    studentLevelLogic.GetModelsBy(
                        p =>
                            p.Person_Id == student.Id && p.Level_Id == level.Id && p.Department_Id == department.Id &&
                            p.Programme_Id == programme.Id).FirstOrDefault();
                if (studentLevel != null && studentLevel.Session != null)
                {
                    if (student.Activated == true || studentCheck.Activated == null)
                    {
                        result = studentResultLogic.GetStudentProcessedResultBy(studentLevel.Session, level,
                            studentLevel.Department, student, semester, studentLevel.Programme);
                    }
                    else
                    {
                        result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(studentLevel.Session, level,
                            studentLevel.Department, student, semester, studentLevel.Programme);
                    }


                    var modifiedResultList = new List<Result>();
                    int totalSemesterCourseUnit = 0;
                    foreach (Result resultItem in result)
                    {
                        decimal WGP = 0;
                        if (resultItem.SpecialCase != null)
                        {
                            resultItem.GPCU = 0;
                            if (totalSemesterCourseUnit == 0)
                            {
                                totalSemesterCourseUnit = (int) resultItem.TotalSemesterCourseUnit -
                                                          resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }
                            else
                            {
                                totalSemesterCourseUnit -= resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }
                        }
                        if (totalSemesterCourseUnit > 0)
                        {
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                        }
                        modifiedResultList.Add(resultItem);
                    }

                    decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                    int? firstSemesterTotalSemesterCourseUnit = 0;
                    firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                    firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
                    firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
                }
                return firstYearFirstSemesterResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Result GetFirstYearSecondSemesterResultInfo(SessionSemester sessionSemester, Level lvl,
            Programme programme, Department department, Student student)
        {
            try
            {
                var firstYearFirstSemesterResult = new Result();
                var modifiedResultList = new List<Result>();
                List<Result> result = null;
                var studentLogic = new StudentLogic();
                var studentResultLogic = new StudentResultLogic();
                Student studentCheck = studentLogic.GetModelBy(p => p.Person_Id == student.Id);
                var semester = new Semester {Id = 2};
                Level level = null;
                if (lvl.Id == 2)
                {
                    level = new Level {Id = 1};
                }
                else
                {
                    level = new Level {Id = 3};
                }
                var studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel =
                    studentLevelLogic.GetModelsBy(
                        p =>
                            p.Person_Id == student.Id && p.Level_Id == level.Id && p.Department_Id == department.Id &&
                            p.Programme_Id == programme.Id).FirstOrDefault();
                if (studentLevel != null && studentLevel.Session != null)
                {
                    if (student.Activated == true || studentCheck.Activated == null)
                    {
                        result = studentResultLogic.GetStudentProcessedResultBy(studentLevel.Session, level,
                            studentLevel.Department, student, semester, studentLevel.Programme);
                    }
                    else
                    {
                        result = studentResultLogic.GetDeactivatedStudentProcessedResultBy(studentLevel.Session, level,
                            studentLevel.Department, student, semester, studentLevel.Programme);
                    }

                    int totalSemesterCourseUnit = 0;
                    foreach (Result resultItem in result)
                    {
                        decimal WGP = 0;

                        if (resultItem.SpecialCase != null)
                        {
                            resultItem.GPCU = 0;
                            if (totalSemesterCourseUnit == 0)
                            {
                                totalSemesterCourseUnit = (int) resultItem.TotalSemesterCourseUnit -
                                                          resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }
                            else
                            {
                                totalSemesterCourseUnit -= resultItem.CourseUnit;
                                resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                                resultItem.Grade = "-";
                            }
                        }
                        if (totalSemesterCourseUnit > 0)
                        {
                            resultItem.TotalSemesterCourseUnit = totalSemesterCourseUnit;
                        }
                        modifiedResultList.Add(resultItem);
                    }

                    decimal? firstSemesterGPCUSum = modifiedResultList.Sum(p => p.GPCU);
                    int? firstSemesterTotalSemesterCourseUnit = 0;
                    firstSemesterTotalSemesterCourseUnit = modifiedResultList.Min(p => p.TotalSemesterCourseUnit);
                    firstYearFirstSemesterResult.TotalSemesterCourseUnit = firstSemesterTotalSemesterCourseUnit;
                    firstYearFirstSemesterResult.GPCU = firstSemesterGPCUSum;
                }
                return firstYearFirstSemesterResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<string> GetCarryOverCourseCodes(SessionSemester ss, Level lvl, Programme programme,
            Department department, Student student)
        {
            try
            {
                var courseCodes = new List<string>();
                var courseRegistrationdetails = new List<CourseRegistrationDetail>();
                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                var studentLevel = new StudentLevel();
                var studentLevelLogic = new StudentLevelLogic();


                Level level = null;
                if (lvl.Id == 1 || lvl.Id == 3)
                {
                    level = new Level {Id = 1};
                    courseCodes = GetFirstYearCarryOverCourses(ss, lvl, programme, department, student);
                }
                else
                {
                    level = new Level {Id = 3};
                }
                courseRegistrationdetails =
                    courseRegistrationDetailLogic.GetModelsBy(
                        crd =>
                            crd.STUDENT_COURSE_REGISTRATION.Session_Id == ss.Session.Id &&
                            crd.STUDENT_COURSE_REGISTRATION.Department_Id == department.Id &&
                            crd.STUDENT_COURSE_REGISTRATION.Programme_Id == programme.Id &&
                            crd.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id &&
                            (crd.Test_Score + crd.Exam_Score) < 40 && crd.Special_Case == null);

                if (courseRegistrationdetails != null)
                {
                    if (ss.Semester.Id == 1)
                    {
                        courseRegistrationdetails = courseRegistrationdetails.Where(p => p.Semester.Id == 1).ToList();
                        if (courseRegistrationdetails.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            {
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (courseRegistrationdetails.Count > 0)
                        {
                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationdetails)
                            {
                                if (courseRegistrationDetail.SpecialCase == null)
                                {
                                    courseCodes.Add(courseRegistrationDetail.Course.Code);
                                }
                            }
                        }
                    }
                }
                return courseCodes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetGraduationStatus(decimal? CGPA, List<string> courseCodes)
        {
            string remark = null;
            try
            {
                if (courseCodes.Count == 0 && CGPA != null)
                {
                    if (CGPA >= 4.5M && CGPA <= 5.0M)
                    {
                        remark = "PASS";
                    }
                    else if (CGPA >= 3.5M && CGPA <= 4.49M)
                    {
                          remark = "PASS";
                    }
                    else if (CGPA >= 2.5M && CGPA <= 3.49M)
                    {
                          remark = "PASS";
                    }
                    else if (CGPA >= 1.50M && CGPA <= 2.49M)
                    {
                          remark = "PASS";
                    }
                    else if (CGPA < 2.0M)
                    {
                        remark = "FAIL";
                    }
                }
                else
                {
                    remark = "CO-";
                    for (int i = 0; i < courseCodes.Count(); i++)
                    {
                        remark += ("|" + courseCodes[i]);
                    }
                }
            }
            catch (Exception)
            {
            }
            return remark;
        }

        private string GetGraduationStatus(decimal? CGPA, decimal? firstSemesterGPA, decimal? secondSemesterGPA,
            List<string> courseCodes)
        {
            string remark = null;
            try
            {
                if (firstSemesterGPA != null && secondSemesterGPA != null)
                {
                    if (firstSemesterGPA < 2.0M && secondSemesterGPA < 2.0M)
                    {
                        remark = "PROBATION ";
                    }
                    if (courseCodes.Count != 0)
                    {
                        remark += "CO-";
                        for (int i = 0; i < courseCodes.Count(); i++)
                        {
                            remark += ("|" + courseCodes[i]);
                        }
                    }
                }
                //if (courseCodes.Count == 0)
                //{
                //    if (CGPA >= 3.5M && CGPA <= 4.0M)
                //    {
                //        remark = "RHL; PASSED: DISTICTION";
                //    }
                //    else if (CGPA >= 3.25M && CGPA <= 3.49M)
                //    {
                //        remark = "DHL; PASSED: UPPER CREDIT";
                //    }
                //    else if (CGPA >= 3.0M && CGPA < 3.25M)
                //    {
                //        remark = "PAS; PASSED: UPPER CREDIT";
                //    }
                //    else if (CGPA >= 2.5M && CGPA <= 2.99M)
                //    {
                //        remark = "PAS; PASSED: LOWER CREDIT";
                //    }
                //    else if (CGPA >= 2.0M && CGPA <= 2.49M)
                //    {
                //        remark = "PAS; PASSED: PASS";
                //    }
                //    else if (CGPA < 2.0M)
                //    {
                //        remark = "FAIL";
                //    }
                //}
                //else
                //{
                //   c
                //}
            }
            catch (Exception)
            {
            }
            return remark;
        }

        public List<UploadedCourseFormat> GetUploadedCourses(Session session, Semester semester)
        {
            try
            {
                if (session == null || session.Id <= 0 || semester == null || semester.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get the uploaded courses is not set! Please check your input criteria selection and try again.");
                }
                List<UploadedCourseFormat> uploadedCourses =
                    (from uc in
                        repository.GetBy<VW_UPLOADED_COURSES>(
                            x => x.Session_Id == session.Id && x.Semester_Id == semester.Id)
                        select new UploadedCourseFormat
                        {
                            Programme = uc.Programme_Name,
                            Level = uc.Level_Name,
                            Department = uc.Department_Name,
                            CourseCode = uc.Course_Code,
                            CourseTitle = uc.Course_Name,
                            LecturerName = uc.User_Name,
                            ProgrammeId = uc.Programme_Id,
                            DepartmentId = uc.Department_Id,
                            SessionId = uc.Session_Id,
                            SemesterId = uc.Semester_Id,
                            LevelId = uc.Level_Id,
                            CourseId = uc.Course_Id,
                        }).ToList();
                return uploadedCourses.OrderBy(uc => uc.Programme).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<UploadedCourseFormat> GetUploadedAlternateCourses(Session session, Semester semester)
        {
            try
            {
                if (session == null || session.Id <= 0 || semester == null || semester.Id <= 0)
                {
                    throw new Exception(
                        "One or more criteria to get the uploaded courses is not set! Please check your input criteria selection and try again.");
                }
                List<UploadedCourseFormat> uploadedCourses =
                    (from uc in
                        repository.GetBy<VW_STUDENT_RESULT_RAW_SCORE_SHEET_UNREGISTERED>(
                            x => x.Session_Id == session.Id && x.Semester_Id == semester.Id)
                        select new UploadedCourseFormat
                        {
                            Programme = uc.Programme_Name,
                            Level = uc.Level_Name,
                            Department = uc.Department_Name,
                            CourseCode = uc.Course_Code,
                            CourseTitle = uc.Course_Name,
                            LecturerName = "",
                            ProgrammeId = uc.Programme_Id,
                            DepartmentId = 1,
                            SessionId = uc.Session_Id,
                            SemesterId = uc.Semester_Id,
                            LevelId = uc.Level_Id,
                            CourseId = uc.Course_Id
                        }).ToList();

                return uploadedCourses.OrderBy(uc => uc.Programme).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetIdentifierBy(STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED rawscoresheetItem)
        {
            try
            {
                string identifier = null;
                string departmentCode = rawscoresheetItem.COURSE.DEPARTMENT.Department_Code;
                string level = rawscoresheetItem.LEVEL.Level_Name;
                string semesterCode = GetSemesterCodeBy(rawscoresheetItem.Semester_Id);
                string sessionCode = GetSessionCodeBy(rawscoresheetItem.SESSION.Session_Name);
                identifier = departmentCode + level + semesterCode + sessionCode;
                return identifier;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetSessionCodeBy(string sessionName)
        {
            try
            {
                string sessionCode = null;
                string[] sessionArray = sessionName.Split('/');
                string sessionYear = sessionArray[1];
                sessionCode = sessionYear.Substring(2, 2);
                return sessionCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetSemesterCodeBy(int semesterId)
        {
            try
            {
                if (semesterId == 1)
                {
                    return "F";
                }
                return "S";
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetDepartmentCode(int departmentid)
        {
            string code = "";
            try
            {
                var departmentLogic = new DepartmentLogic();
                code = departmentLogic.GetModelBy(m => m.Department_Id == departmentid).Code;
            }
            catch (Exception)
            {
                throw;
            }
            return code;
        }

        private string GetLevelName(int levelId)
        {
            string code = "";
            try
            {
                var levelLogic = new LevelLogic();
                code = levelLogic.GetModelBy(m => m.Level_Id == levelId).Name;
            }
            catch (Exception)
            {
                throw;
            }
            return code;
        }
        //public List<Result> GetExamPassList(SessionSemester sessionSemester, Level level, Programme programme, Department department)
        //{
        //    try
        //    {
        //        if (sessionSemester == null || sessionSemester.Id <= 0 || level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0)
        //        {
        //            throw new Exception("One or more criteria to get Exam Pass List not set! Please check your input criteria selection and try again.");
        //        }

        //        StudentLogic studentLogic = new StudentLogic();
        //        SessionSemesterLogic sessionSemesterLogic = new Business.SessionSemesterLogic();
        //        SessionSemester ss = sessionSemesterLogic.GetBy(sessionSemester.Id);

        //        string[] failureGrades = { "F", "CD", "D", "E" };

        //        List<Result> results = (from sr in repository.GetBy<VW_STUDENT_RESULT_ALT>(x => x.Session_Id == ss.Session.Id && x.Semester_Id == ss.Semester.Id && x.Level_Id == level.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && !failureGrades.Contains(x.Grade))
        //                                select new Result
        //                                {
        //                                    StudentId = sr.Person_Id,
        //                                    Name = sr.Name,
        //                                    MatricNumber = sr.Matric_Number,
        //                                    SessionName = sr.Session_Name,
        //                                    FacultyName = sr.Faculty_Name,
        //                                    ProgrammeName = sr.Programme_Name,
        //                                    LevelName = sr.Level_Name,
        //                                    CourseId = sr.Course_Id,
        //                                    CourseCode = sr.Course_Code,
        //                                    CourseName = sr.Course_Name,
        //                                    //CourseUnit = sr.Course_Unit,

        //                                    TestScore = sr.Test_Score,
        //                                    ExamScore = sr.Exam_Score,
        //                                    Score = sr.Total_Score,
        //                                    Grade = sr.Grade,
        //                                    GradePoint = sr.Grade_Point,

        //                                    GPCU = sr.WGP,
        //                                    TotalSemesterCourseUnit = sr.Total_Semester_Course_Unit,
        //                                }).ToList();

        //        sessionSemester = sessionSemesterLogic.GetModelBy(p => p.Session_Semester_Id == sessionSemester.Id);
        //        List<Result> studentsResult = GetResultList(sessionSemester, level, programme, department).ToList();
        //        List<Result> masterSheetResult = new List<Result>();
        //        foreach (Result resultItem in studentsResult)
        //        {
        //            Result result = ViewProcessedStudentResult(resultItem.StudentId, sessionSemester, level, programme, department);
        //            masterSheetResult.Add(result);
        //        }


        //        foreach (Result result in masterSheetResult)
        //        {
        //            List<Result> studentResults = results.Where(p => p.StudentId == result.StudentId).ToList();
        //            foreach (Result resultItem in studentResults)
        //            {
        //                resultItem.CGPA = result.CGPA;
        //                resultItem.Remark = result.Remark;
        //                resultItem.GPA = result.GPA;
        //                resultItem.UnitOutstanding =
        //                    studentResults.Where(
        //                        s => s.Grade == "D" || s.Grade == "E" || s.Grade == "F")
        //                        .Sum(s => s.CourseUnit);
        //                resultItem.UnitPassed =
        //                    studentResults.Where(
        //                        s => s.Grade == "A" || s.Grade == "B" || s.Grade == "C")
        //                        .Sum(s => s.CourseUnit);
        //            }

        //        }

        //        return results.OrderBy(a => a.MatricNumber).ToList();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}