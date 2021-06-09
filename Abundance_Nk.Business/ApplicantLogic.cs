using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ApplicantLogic : BusinessBaseLogic<Applicant, APPLICANT>
    {
        private readonly StudentLogic studentLogic;
        private readonly StudentMatricNumberAssignmentLogic studentMatricNumberAssignmentLogic;
        private ApplicantClearanceLogic applicantClearedLogic;

        public ApplicantLogic()
        {
            translator = new ApplicantTranslator();
            applicantClearedLogic = new ApplicantClearanceLogic();
            studentMatricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
            studentLogic = new StudentLogic();
        }


        public List<ApplicationFormView> GetBy(ApplicantStatus.Status status, Faculty faculty)
        {
            try
            {
                List<ApplicationFormView> forms =
                    (from a in
                        repository.GetBy<VW_PROSPECTIVE_STUDENT>(
                            a =>
                                a.Rejected == false && a.Faculty_Id == faculty.Id &&
                                a.Applicant_Status_Id == (int) status)
                        //orderby a.Date_Submitted
                        select new ApplicationFormView
                        {
                            PersonId = a.Person_Id,
                            FormId = a.Application_Form_Id,
                            FormNumber = a.Application_Form_Number,
                            Name = a.Name,
                            DepartmentId = a.Department_Id,
                            DepartmentName = a.Department_Name,
                            ProgrammeName = a.Programme_Name,
                            ExamSerialNumber = a.Application_Exam_Serial_Number,
                            ExamNumber = a.Application_Exam_Number,
                            FacultyId = a.Faculty_Id,
                            FacultyName = a.Faculty_Name,
                            SessionId = a.Session_Id,
                            //LevelId = a.Level_Id,
                            //LevelName = a.Level_Name,
                            ProgrammeId = a.Programme_Id,
                            IsSelected = false,
                        }).ToList();

                SeRejectReason(forms);

                return forms;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ApplicationFormView> GetBy(string applicationFormNo)
        {
            try
            {
                List<ApplicationFormView> forms =
                    (from a in
                        repository.GetBy<VW_PROSPECTIVE_STUDENT>(
                            a => a.Application_Form_Number.ToLower() == applicationFormNo.ToLower())
                        select new ApplicationFormView
                        {
                            PersonId = a.Person_Id,
                            FormId = a.Application_Form_Id,
                            FormNumber = a.Application_Form_Number,
                            Name = a.Name,
                            DepartmentId = a.Department_Id,
                            DepartmentName = a.Department_Name,
                            ProgrammeName = a.Programme_Name,
                            ExamSerialNumber = a.Application_Exam_Serial_Number,
                            ExamNumber = a.Application_Exam_Number,
                            FacultyId = a.Faculty_Id,
                            FacultyName = a.Faculty_Name,
                            SessionId = a.Session_Id,
                            //LevelId = a.Level_Id,
                            //LevelName = a.Level_Name,
                            ProgrammeId = a.Programme_Id,
                            IsSelected = false,
                        }).Take(50).ToList();

                SeRejectReason(forms);

                return forms;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ApplicationFormView GetBy(long applicationFormId)
        {
            try
            {
                ApplicationFormView applicant =
                    (from a in repository.GetBy<VW_PROSPECTIVE_STUDENT>(a => a.Application_Form_Id == applicationFormId)
                        select new ApplicationFormView
                        {
                            PersonId = a.Person_Id,
                            FormId = a.Application_Form_Id,
                            FormNumber = a.Application_Form_Number,
                            Name = a.Name,
                            DepartmentId = a.Department_Id,
                            DepartmentName = a.Department_Name,
                            ProgrammeName = a.Programme_Name,
                            ExamSerialNumber = a.Application_Exam_Serial_Number,
                            ExamNumber = a.Application_Exam_Number,
                            FacultyId = a.Faculty_Id,
                            FacultyName = a.Faculty_Name,
                            SessionId = a.Session_Id,
                            //LevelId = a.Level_Id,
                            //LevelName = a.Level_Name,
                            ProgrammeId = a.Programme_Id,
                            IsSelected = false,
                        }).FirstOrDefault();

                return applicant;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ApplicationFormView GetBy(Person person)
        {
            try
            {
                ApplicationFormView applicant =
                    (from a in repository.GetBy<VW_PROSPECTIVE_STUDENT>(a => a.Person_Id == person.Id)
                        select new ApplicationFormView
                        {
                            PersonId = a.Person_Id,
                            FormId = a.Application_Form_Id,
                            FormNumber = a.Application_Form_Number,
                            Name = a.Name,
                            DepartmentId = a.Department_Id,
                            DepartmentName = a.Department_Name,
                            ProgrammeName = a.Programme_Name,
                            ExamSerialNumber = a.Application_Exam_Serial_Number,
                            ExamNumber = a.Application_Exam_Number,
                            FacultyId = a.Faculty_Id,
                            FacultyName = a.Faculty_Name,
                            SessionId = a.Session_Id,
                            //LevelId = a.Level_Id,
                            //LevelName = a.Level_Name,
                            ProgrammeId = a.Programme_Id,
                            IsSelected = false,
                        }).FirstOrDefault();

                return applicant;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Applicant GetApplicantsBy(string Application_Form_Number)
        {
            try
            {
                Expression<Func<APPLICANT, bool>> selector =
                    a => a.APPLICATION_FORM.Application_Form_Number == Application_Form_Number;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SeRejectReason(List<ApplicationFormView> forms)
        {
            try
            {
                if (forms != null && forms.Count > 0)
                {
                    var admissionCriteriaLogic = new AdmissionCriteriaLogic();
                    foreach (ApplicationFormView form in forms)
                    {
                        var appliedCourse = new AppliedCourse();
                        appliedCourse.Person = new Person {Id = form.PersonId};
                        appliedCourse.Programme = new Programme {Id = form.ProgrammeId, Name = form.ProgrammeName};
                        appliedCourse.Department = new Department {Id = form.DepartmentId, Name = form.DepartmentName};

                        form.RejectReason = admissionCriteriaLogic.EvaluateApplication(appliedCourse);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Applicant GetBy(ApplicationForm form)
        {
            try
            {
                Expression<Func<APPLICANT, bool>> selector = a => a.Application_Form_Id == form.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Applicant GetByFormId(long formId)
        {
            try
            {
                Expression<Func<APPLICANT, bool>> selector = a => a.Application_Form_Id == formId;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool UpdateStatus(ApplicationForm form, ApplicantStatus.Status status)
        {
            try
            {
                Expression<Func<APPLICANT, bool>> selector = a => a.Application_Form_Id == form.Id;
                APPLICANT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Applicant_Status_Id = (int) status;

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

        public bool Clear(List<ApplicationFormView> applicants, User clearedBy)
        {
            try
            {
                if (applicants == null)
                {
                    throw new Exception(
                        "List of Applicants to clear is empty! Please select at least one applicant from the list.");
                }

                using (var transaction = new TransactionScope())
                {
                    foreach (ApplicationFormView applicant in applicants)
                    {
                        //change applicant status
                        var applicationForm = new ApplicationForm {Id = applicant.FormId};

                        //assign matric no to applicant
                        var faculty = new Faculty {Id = applicant.FacultyId};
                        var department = new Department {Id = applicant.DepartmentId};
                        var session = new Session {Id = applicant.SessionId};
                        var level = new Level {Id = applicant.LevelId};
                        var programme = new Programme {Id = applicant.ProgrammeId};
                        StudentMatricNumberAssignment startMatricNo = studentMatricNumberAssignmentLogic.GetBy(faculty,
                            department, programme, level, session);
                        if (startMatricNo != null)
                        {
                            long studentNumber = 0;
                            string matricNumber = "";

                            if (startMatricNo.Used)
                            {
                                string[] matricNoArray = startMatricNo.MatricNoStartFrom.Split('/');

                                studentNumber = GetNextStudentNumber(faculty, department, level, session);
                                matricNoArray[matricNoArray.Length - 1] = UtilityLogic.PaddNumber(studentNumber, 4);
                                matricNumber = string.Join("/", matricNoArray);
                            }
                            else
                            {
                                matricNumber = startMatricNo.MatricNoStartFrom;
                                studentNumber = startMatricNo.MatricSerialNoStartFrom;
                                bool markedAsUsed = studentMatricNumberAssignmentLogic.MarkAsUsed(startMatricNo);
                            }

                            var person = new Person {Id = applicant.PersonId};
                            bool matricAssigned = studentLogic.AssignMatricNumber(person, studentNumber, matricNumber);
                        }
                        else
                        {
                            throw new Exception(applicant.LevelName + " for " + applicant.DepartmentName +
                                                " for the current academic session has not been set! Please contact your system administrator.");
                        }
                    }

                    transaction.Complete();
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long GetNextStudentNumber(Faculty faculty, Department department, Level level, Session session)
        {
            try
            {
                long newStudentNumber = 0;
                List<ApplicationFormView> applicationForms =
                    (from a in
                        repository.GetBy<VW_ASSIGNED_MATRIC_NUMBER>(
                            a =>
                                a.Faculty_Id == faculty.Id && a.Department_Id == department.Id && a.Level_Id == level.Id &&
                                a.Session_Id == session.Id)
                        select new ApplicationFormView
                        {
                            FormId = a.Application_Form_Id.Value,
                            StudentNumber = (long) a.Student_Number,
                            MatricNumber = a.Matric_Number,
                            PersonId = a.Person_Id,
                        }).ToList();

                if (applicationForms != null && applicationForms.Count > 0)
                {
                    long rawMaxStudentNumber = applicationForms.Max(s => s.StudentNumber);
                    newStudentNumber = rawMaxStudentNumber + 1;
                }

                return newStudentNumber;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}