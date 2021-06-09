using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Transactions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Business
{
    public class MigrationLogic
    {
        private Abundance_NkEntities context;

        public void MigrateApplicants()
        {
            try
            {
                context = new Abundance_NkEntities();
                var migrationBiodatas = new List<MIGRATION_APPLICANTS>();
                migrationBiodatas = context.MIGRATION_APPLICANTS.ToList();
                foreach (MIGRATION_APPLICANTS migrationBiodata in migrationBiodatas)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        string Matric_Number = migrationBiodata.JAMB_REG_NO;
                        var studentLogic = new ApplicantJambDetailLogic();
                        ApplicantJambDetail student =studentLogic.GetModelsBy(a => a.Applicant_Jamb_Registration_Number == Matric_Number).LastOrDefault();
                        if (student == null || student.ApplicationForm == null)
                        {
                            Person applicantPerson = CreatePerson(migrationBiodata);
                            if (applicantPerson != null && applicantPerson.Id > 0)
                            {

                                AppliedCourse appliedCourse = CreateAppliedCourse(migrationBiodata, applicantPerson);
                                ApplicantJambDetail applicantJambDetail = CreateJambDetail(migrationBiodata,applicantPerson);
                                Payment payment = CreatePayment(applicantPerson);
                                ApplicationForm applicationForm = CreateApplicationForm(payment, applicantPerson);
                                CreateOlevel(migrationBiodata, applicationForm, applicantPerson);
                                ApplicantJambDetail formDetail =studentLogic.GetModelsBy(a =>a.Applicant_Jamb_Registration_Number == Matric_Number &&  a.APPLICATION_FORM == null).FirstOrDefault();
                                applicantJambDetail.Person = applicantPerson;
                                applicantJambDetail.ApplicationForm = new ApplicationForm();
                                applicantJambDetail.ApplicationForm.Id = applicationForm.Id;
                                studentLogic.Modify(applicantJambDetail);
                                Applicant applicant = new Applicant();
                                ApplicantLogic applicantLogic = new ApplicantLogic();
                                applicant.ApplicationForm = applicationForm;
                                applicant.Person = applicantPerson;
                                applicant.Status = new ApplicantStatus(){Id = 1};
                                applicant.Ability = new Ability(){Id = 1};
                                applicantLogic.Create(applicant);

                            }
                            transactionScope.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static StudentSponsor CreateSponsor(MIGRATION_BIODATA migrationBiodata, Student student)
        {
            try
            {
                string sponsorName = migrationBiodata.SPONSOR_NAME;
                string sponsorAddress = migrationBiodata.SPONSOR_ADDRESS;
                string sponsorPhone = migrationBiodata.SPONSOR_PHONE;
                var sponsor = new StudentSponsor();
                var sponsorLogic = new StudentSponsorLogic();
                sponsor.Student = student;
                sponsor.Name = sponsorName;
                sponsor.ContactAddress = sponsorAddress;
                if (sponsorPhone.Length <= 15)
                {
                    sponsor.MobilePhone = sponsorPhone;
                }
                else
                {
                    sponsor.MobilePhone = "--";
                }

                sponsor.Relationship = new Relationship {Id = 1};
                sponsor = sponsorLogic.Create(sponsor);
                return sponsor;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static ApplicantJambDetail CreateJambRecord(MIGRATION_BIODATA migrationBiodata, Person applicantPerson)
        {
            try
            {
                string jambNumber = migrationBiodata.JAMB_REG_NO;
                if (jambNumber != null && jambNumber.Length <= 20)
                {
                    var applicantJambDetail = new ApplicantJambDetail();
                    var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                    applicantJambDetail.Person = applicantPerson;
                    applicantJambDetail.JambRegistrationNumber = jambNumber;
                    applicantJambDetail = applicantJambDetailLogic.Create(applicantJambDetail);
                    return applicantJambDetail;
                }
                return new ApplicantJambDetail();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Student CreateStudent(MIGRATION_BIODATA migrationBiodata, Person applicantPerson,StudentLogic studentLogic)
        {
            try
            {
                Student student;
                student = new Student();
                string matricNo = migrationBiodata.REG_NO;
                if (matricNo != null && matricNo.Length <= 20)
                {
                    var studentType = new StudentType {Id = 1};
                    var studentCategory = new StudentCategory {Id = 2};
                    var studentStatus = new StudentStatus {Id = 1};
                    student.Id = applicantPerson.Id;
                    student.MatricNumber = matricNo;
                    student.Type = studentType;
                    student.Category = studentCategory;
                    student.Status = studentStatus;
                    student = studentLogic.Create(student);
                }


                if (student != null && student.Id > 0)
                {
                    string rawDepartment = migrationBiodata.DEPARTMENT;
                    string rawProgramme = migrationBiodata.PROGRAMME;
                    string rawLevel = migrationBiodata.LEVEL;

                    var department = new Department();
                    var departmentLogic = new DepartmentLogic();
                    department = departmentLogic.GetModelBy(a => a.Department_Name == rawDepartment);

                    var programme = new Programme();
                    var programmeLogic = new ProgrammeLogic();
                    programme = programmeLogic.GetModelBy(a => a.Programme_Name == rawProgramme);

                    var level = new Level();
                    var levelLogic = new LevelLogic();
                    level = levelLogic.GetModelBy(a => a.Level_Name == rawLevel);


                    if (department != null && department.Id > 0 && programme != null && programme.Id > 0 &&
                        level != null &&
                        level.Id > 0)
                    {
                        var studentLevel = new StudentLevel();
                        studentLevel.Student = student;
                        studentLevel.Department = department;
                        studentLevel.Programme = programme;
                        studentLevel.Level = level;
                        studentLevel.Session = new Session {Id = 1};
                        var studentLevelLogic = new StudentLevelLogic();
                        studentLevel = studentLevelLogic.Create(studentLevel);
                    }
                }
                return student;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void CreateOlevel(MIGRATION_APPLICANTS migrationApplicant, ApplicationForm applicationForm,Person applicantPerson)
        {
            string[] resultDetails1 = migrationApplicant.QUALIFICATION_1.Split(' ');
            string examType = resultDetails1[0];

            var oLevelType = new OLevelType();
            var oLevelTypeLogic = new OLevelTypeLogic();
            oLevelType = oLevelTypeLogic.GetModelBy(a => a.O_Level_Type_Name == examType);

            var oLevelResult = new OLevelResult();
            var oLevelResultLogic = new OLevelResultLogic();
            oLevelResult.ApplicationForm = applicationForm;
            oLevelResult.ExamNumber = "NAN";
            oLevelResult.ExamYear = 2016;
            oLevelResult.Person = applicantPerson;
            oLevelResult.PersonType = new PersonType {Id = 4};
            oLevelResult.Sitting = new OLevelExamSitting {Id = 1};
            oLevelResult.Type = oLevelType;
            oLevelResult = oLevelResultLogic.Create(oLevelResult);
            if (oLevelResult != null && oLevelResult.Id > 0)
            {
                var oLevelSubject = new OLevelSubject();
                var oLevelSubjectLogic = new OLevelSubjectLogic();
                var oLevelGrade = new OLevelGrade();
                var oLevelGradeLogic = new OLevelGradeLogic();
                var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                var oLevelResultDetails = new List<OLevelResultDetail>();
                string[] grades = resultDetails1;
                foreach (string gradex in grades)
                {
                    string[] subjectGrade = gradex.Split(':');
                    if (subjectGrade.Length > 1)
                    {
                        string grade = subjectGrade[1].Replace(",", "");
                        string subject = subjectGrade[0];
                        oLevelGrade = oLevelGradeLogic.GetModelBy(a => a.O_Level_Grade_Name == grade);
                        oLevelSubject = oLevelSubjectLogic.GetModelBy(a => a.O_Level_Subject_Description == subject);
                        if (oLevelSubject != null && oLevelSubject.Id > 0 && oLevelGrade != null && oLevelGrade.Id > 0)
                        {
                            var oLevelResultDetail = new OLevelResultDetail();
                            oLevelResultDetail.Header = oLevelResult;
                            oLevelResultDetail.Subject = oLevelSubject;
                            oLevelResultDetail.Grade = oLevelGrade;
                            oLevelResultDetails.Add(oLevelResultDetail);
                        }
                    }
                }
                oLevelResultDetailLogic.Create(oLevelResultDetails);
            }


            if (migrationApplicant.QUALIFICATION_2 != null)
            {
                string[] resultDetails2 = migrationApplicant.QUALIFICATION_2.Split(' ');
                string examType2 = resultDetails1[0];

                oLevelType = new OLevelType();
                oLevelType = oLevelTypeLogic.GetModelBy(a => a.O_Level_Type_Name == examType2);

                oLevelResult.ApplicationForm = applicationForm;
                oLevelResult.ExamNumber = "NAN";
                oLevelResult.ExamYear = 2016;
                oLevelResult.Person = applicantPerson;
                oLevelResult.PersonType = new PersonType {Id = 4};
                oLevelResult.Sitting = new OLevelExamSitting {Id = 2};
                oLevelResult.Type = oLevelType;
                oLevelResult = oLevelResultLogic.Create(oLevelResult);
                if (oLevelResult != null && oLevelResult.Id > 0)
                {
                    var oLevelSubject = new OLevelSubject();
                    var oLevelSubjectLogic = new OLevelSubjectLogic();
                    var oLevelGrade = new OLevelGrade();
                    var oLevelGradeLogic = new OLevelGradeLogic();
                    var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                    var oLevelResultDetails = new List<OLevelResultDetail>();
                    string[] grades = resultDetails1;
                    foreach (string gradex in grades)
                    {
                        string[] subjectGrade = gradex.Split(':');
                        if (subjectGrade.Length > 1)
                        {
                            string grade = subjectGrade[1].Replace(",", "");
                            string subject = subjectGrade[0];
                            oLevelGrade = oLevelGradeLogic.GetModelBy(a => a.O_Level_Grade_Name == grade);
                            oLevelSubject = oLevelSubjectLogic.GetModelBy(a => a.O_Level_Subject_Description == subject);
                            if (oLevelSubject != null && oLevelSubject.Id > 0 && oLevelGrade != null &&
                                oLevelGrade.Id > 0)
                            {
                                var oLevelResultDetail = new OLevelResultDetail();
                                oLevelResultDetail.Header = oLevelResult;
                                oLevelResultDetail.Subject = oLevelSubject;
                                oLevelResultDetail.Grade = oLevelGrade;
                                oLevelResultDetails.Add(oLevelResultDetail);
                            }
                        }
                    }
                    oLevelResultDetailLogic.Create(oLevelResultDetails);
                }
            }
        }

        private static ApplicationForm CreateApplicationForm(Payment newPayment, Person applicantPerson)
        {
            try
            {
                var applicationForm = new ApplicationForm();
                var applicationFormLogic = new ApplicationFormLogic();
                applicationForm.DateSubmitted = DateTime.Now;
                applicationForm.Payment = newPayment;
                applicationForm.Person = applicantPerson;
                applicationForm.ProgrammeFee = new ApplicationProgrammeFee {Id = 1};
                applicationForm.RejectReason = "";
                applicationForm.Rejected = false;
                applicationForm.Release = true;
                applicationForm.Setting = new ApplicationFormSetting {Id = 2};
                applicationForm = applicationFormLogic.Create(applicationForm);
                return applicationForm;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Payment CreatePayment(Person applicantPerson)
        {
            try
            {
                var applicantPayment = new Payment();
                var paymentLogic = new PaymentLogic();
                applicantPayment.PaymentMode = new PaymentMode {Id = 1};
                applicantPayment.PaymentType = new PaymentType {Id = 1};
                applicantPayment.PersonType = new PersonType {Id = 4};
                applicantPayment.FeeType = new FeeType {Id = 1};
                applicantPayment.DatePaid = DateTime.Now;
                applicantPayment.Person = applicantPerson;
                applicantPayment.Session = new Session {Id = 7};

                OnlinePayment newOnlinePayment = null;
                Payment newPayment = paymentLogic.Create(applicantPayment);
                if (newPayment != null)
                {
                    var channel = new PaymentChannel {Id = (int) PaymentChannel.Channels.Etranzact};
                    var onlinePaymentLogic = new OnlinePaymentLogic();
                    var onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = newPayment;
                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                }
                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static ApplicantJambDetail CreateJambDetail(MIGRATION_APPLICANTS migrationApplicant,Person applicantPerson)
        {
            try
            {
                string jambNumber = migrationApplicant.JAMB_REG_NO;
                string score = migrationApplicant.SCORE.ToString();
                string instituion = migrationApplicant.FIRST_CHOICE;
                var applicantJambDetail = new ApplicantJambDetail();
                var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                applicantJambDetail.Person = applicantPerson;
                applicantJambDetail.JambRegistrationNumber = jambNumber;
                applicantJambDetail.JambScore = Convert.ToInt16(score);
                if (instituion.ToUpper() == "ABSU")
                {
                    applicantJambDetail.InstitutionChoice = new InstitutionChoice {Id = 1};
                }
                else
                {
                    applicantJambDetail.InstitutionChoice = new InstitutionChoice {Id = 2};
                }
                applicantJambDetail = applicantJambDetailLogic.Create(applicantJambDetail);
                return applicantJambDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static AppliedCourse CreateAppliedCourse(MIGRATION_APPLICANTS migrationApplicant, Person applicantPerson)
        {
            string rawDepartment = migrationApplicant.DEPARTMENT;
            var department = new Department();
            var departmentLogic = new DepartmentLogic();
            department = departmentLogic.GetModelBy(a => a.Department_Name == rawDepartment);
            if (department != null && department.Id > 0)
            {
                var applicantAppliedCourse = new AppliedCourse();
                var appliedCourseLogic = new AppliedCourseLogic();
                applicantAppliedCourse.ApplicationForm = null;
                applicantAppliedCourse.Programme = new Programme {Id = 1};
                applicantAppliedCourse.Department = department;
                applicantAppliedCourse.Person = applicantPerson;
                applicantAppliedCourse = appliedCourseLogic.Create(applicantAppliedCourse);
                return applicantAppliedCourse;
            }
            return new AppliedCourse();
        }

        private static Person CreatePerson(MIGRATION_BIODATA migrationBiodata)
        {
            try
            {
                string[] fullname = migrationBiodata.STUDENTNAME.Split(' ');
                string rawstate = migrationBiodata.STATE_;
                string rawLga = migrationBiodata.LGA;

                string date_of_birth = migrationBiodata.DOB;
                IFormatProvider culture = new CultureInfo("fr-FR", true);
                DateTime dobDateTime = DateTime.Parse(date_of_birth, culture, DateTimeStyles.AssumeLocal);
                Sex applicantSex;
                string sex = migrationBiodata.SEX;
                if (sex == "M")
                {
                    applicantSex = new Sex {Id = 1};
                }
                else
                {
                    applicantSex = new Sex {Id = 2};
                }

                string mobile = "NAN";
                string hometown = "NAN";
                string permanentAddress = "NAN";
                string contact_address = "NAN";
                string email = "NAN";

                if (migrationBiodata.MOBILE_NO != null && migrationBiodata.MOBILE_NO.Length <= 15)
                {
                    mobile = migrationBiodata.MOBILE_NO;
                }
                if (migrationBiodata.HOME_TOWN != null && migrationBiodata.HOME_TOWN.Length <= 50)
                {
                    hometown = migrationBiodata.HOME_TOWN;
                }

                if (migrationBiodata.PERMANENT_ADDRESS != null)
                {
                    permanentAddress = migrationBiodata.PERMANENT_ADDRESS;
                }
                if (migrationBiodata.CONTACT_ADDRESS != null)
                {
                    contact_address = migrationBiodata.CONTACT_ADDRESS;
                }
                if (migrationBiodata.EMAIL != null)
                {
                    email = migrationBiodata.EMAIL;
                }


                var state = new State();
                var stateLogic = new StateLogic();
                state = stateLogic.GetModelBy(a => a.State_Name == rawstate);

                var localGovernment = new LocalGovernment();
                var localGovernmentLogic = new LocalGovernmentLogic();
                localGovernment =
                    localGovernmentLogic.GetModelBy(a => a.Local_Government_Name == rawLga && a.State_Id == state.Id);

                var role = new Role {Id = 6};
                var personType = new PersonType {Id = 4};
                var nationality = new Nationality {Id = 1};
                var religion = new Religion {Id = 1};

                var applicantPerson = new Person();
                applicantPerson.DateEntered = DateTime.Now;
                applicantPerson.MobilePhone = mobile;
                applicantPerson.Religion = religion;
                applicantPerson.HomeTown = hometown;
                applicantPerson.HomeAddress = permanentAddress;
                applicantPerson.Role = role;
                applicantPerson.Type = personType;
                applicantPerson.Nationality = nationality;
                applicantPerson.LastName = fullname[0];
                applicantPerson.FirstName = fullname[1];
                if (fullname[2] != null)
                {
                    applicantPerson.OtherName = fullname[2];
                }
                if (state != null && state.Id != null)
                {
                    applicantPerson.State = state;
                }
                if (localGovernment != null && localGovernment.Id > 0)
                {
                    applicantPerson.LocalGovernment = localGovernment;
                }
                applicantPerson.ContactAddress = contact_address;
                applicantPerson.DateOfBirth = dobDateTime;
                applicantPerson.Sex = applicantSex;
                applicantPerson.Email = email;
                var personLogic = new PersonLogic();
                applicantPerson = personLogic.Create(applicantPerson);
                return applicantPerson;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Person CreatePerson(MIGRATION_APPLICANTS migrationApplicant)
        {
            try
            {
                string[] fullname = migrationApplicant.NAMES.Split(' ');
                string rawstate = migrationApplicant.STATE;
                string rawLga = migrationApplicant.LGA;

                var applicantSex = new Sex {Id = 1};


                string mobile = "NAN";
                string hometown = "NAN";
                string permanentAddress = "NAN";
                string contact_address = "NAN";
                string email = "NAN";

                //var state = new State();
                //var stateLogic = new StateLogic();
                //state = stateLogic.GetModelBy(a => a.State_Name == rawstate);

                //var localGovernment = new LocalGovernment();
                //var localGovernmentLogic = new LocalGovernmentLogic();
                //localGovernment = localGovernmentLogic.GetModelBy(a => a.Local_Government_Name == rawLga && a.State_Id == state.Id);

                var role = new Role {Id = 6};
                var personType = new PersonType {Id = 4};
                var nationality = new Nationality {Id = 1};
                var religion = new Religion {Id = 1};

                var applicantPerson = new Person();
                applicantPerson.DateEntered = DateTime.Now;
                applicantPerson.MobilePhone = mobile;
                applicantPerson.Religion = religion;
                applicantPerson.HomeTown = hometown;
                applicantPerson.HomeAddress = permanentAddress;
                applicantPerson.Role = role;
                applicantPerson.Type = personType;
                applicantPerson.Nationality = nationality;
                applicantPerson.LastName = fullname[0];
                applicantPerson.FirstName = fullname[1];
                applicantPerson.State = new State(){Id = "AB"};
                applicantPerson.LocalGovernment = new LocalGovernment(){Id = 1};
                //if (fullname[2] != null)
                //{
                //    applicantPerson.OtherName = fullname[2];
                //}
                //if (state != null && state.Id != null)
                //{
                //    applicantPerson.State = state;
                //}
                //if (localGovernment != null && localGovernment.Id > 0)
                //{
                //    applicantPerson.LocalGovernment = localGovernment;
                //}
                applicantPerson.ContactAddress = contact_address;
                applicantPerson.Sex = applicantSex;
                applicantPerson.Email = email;
                var personLogic = new PersonLogic();
                applicantPerson = personLogic.Create(applicantPerson);
                return applicantPerson;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void MigrateCourseRegistration()
        {
            try
            {
                var courseRegistrationDetails = new List<CourseRegistrationDetail>();
                var courseRegistration = new CourseRegistration();
                var courseRegistrationLogic = new CourseRegistrationLogic();

                context = new Abundance_NkEntities();
                var migrationRegisteredCoursess = new List<MIGRATION_REGISTERED_COURSES>();
                List<string> data = context.MIGRATION_REGISTERED_COURSES.Select(x => x.Matric_No).Distinct().ToList();
                foreach (string s in data)
                {
                    var studentLogic = new StudentLogic();
                    Student student = studentLogic.GetBy(s);
                    courseRegistration.Details = new List<CourseRegistrationDetail>();

                    if (student != null)
                    {
                        migrationRegisteredCoursess =
                            context.MIGRATION_REGISTERED_COURSES.Where(a => a.Matric_No == s).ToList();
                        if (migrationRegisteredCoursess != null)
                        {
                            foreach (
                                MIGRATION_REGISTERED_COURSES migrationRegisteredCourses in migrationRegisteredCoursess)
                            {
                                string course_code = migrationRegisteredCourses.Course_Code.Trim();
                                string Department_Name = migrationRegisteredCourses.Department.Trim();
                                string Level_Name = migrationRegisteredCourses.Level.Trim();

                                var departmentLogic = new DepartmentLogic();
                                Department department =
                                    departmentLogic.GetModelBy(a => a.Department_Name.ToUpper() == Department_Name);

                                var levelLogic = new LevelLogic();
                                Level level = levelLogic.GetModelBy(a => a.Level_Name.ToUpper() == Level_Name);

                                var semester = new Semester {Id = 2};
                                var programme = new Programme {Id = 4};
                                var session = new Session {Id = 1};
                                if (department != null && level != null)
                                {
                                    var courseLogic = new CourseLogic();
                                    Course course =
                                        courseLogic.GetModelsBy(
                                            c =>
                                                c.Course_Code == course_code && c.Level_Id == level.Id &&
                                                c.Department_Id == department.Id && c.Semester_Id == semester.Id &&
                                                c.Programme_Id == programme.Id).FirstOrDefault();

                                    if (course != null)
                                    {
                                        CourseRegistration courseRegistrations = courseRegistrationLogic.GetBy(student,
                                            level, programme, department, session);
                                        if (courseRegistrations == null)
                                        {
                                            courseRegistration.Approved = true;
                                            courseRegistration.DateApproved = DateTime.Now;
                                            courseRegistration.Department = department;
                                            courseRegistration.Level = level;
                                            courseRegistration.Programme = programme;
                                            courseRegistration.Session = session;
                                            courseRegistration.Student = student;
                                            courseRegistrationLogic.CreateCourseRegistration(courseRegistration);
                                            //CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                                            //CourseRegistrationDetail courseRegistrationDetail = courseRegistrationDetailLogic.GetModelBy(a => a.Course_Id == course.Id && a.STUDENT_COURSE_REGISTRATION.Student_Course_Registration_Id == courseRegistration.Id);
                                            //if(courseRegistrationDetail == null)
                                            //{
                                            //    courseRegistrationDetail = new CourseRegistrationDetail();
                                            //    courseRegistrationDetail.Course = course;
                                            //    courseRegistrationDetail.CourseRegistration = courseRegistration;
                                            //    courseRegistrationDetail.CourseUnit = course.Unit;
                                            //    courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };
                                            //    courseRegistrationDetail.Semester = semester;
                                            //    courseRegistration.Details.Add(courseRegistrationDetail);
                                            //}
                                        }
                                    }
                                }
                            }
                            //List<CourseRegistrationDetail> cxRegistrationDetails = courseRegistration.Details.Distinct().ToList();
                            //courseRegistration.Details = cxRegistrationDetails;
                            //courseRegistration = courseRegistrationLogic.Create(courseRegistration);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MigrateFirstSemesterResult()
        {
            try
            {
                string path = @"\\psf\Home\Documents\LLOYDANT\ABSU\ABSU COMPOSITE EXCEL"; // Put the file Path here
                List<StudentDetails> ResultList = GenerateList(path);
                foreach (StudentDetails studentDetailse in ResultList)
                {
                    var migrationRegisteredCourses = new MIGRATION_REGISTERED_COURSES();
                    migrationRegisteredCourses.Course_Code = studentDetailse.CourseCode;
                    migrationRegisteredCourses.Course_Unit = studentDetailse.Unit;
                    migrationRegisteredCourses.Department = studentDetailse.Department;
                    migrationRegisteredCourses.ExamScore = studentDetailse.Score;
                    migrationRegisteredCourses.Fullname = studentDetailse.FullName;
                    migrationRegisteredCourses.Level = studentDetailse.Level;
                    migrationRegisteredCourses.Matric_No = studentDetailse.MatricNumber;
                    migrationRegisteredCourses.Semester = "FIRST SEMESTER";
                    migrationRegisteredCourses.Session = "2015/2016";

                    var contextEntities = new Abundance_NkEntities();
                    contextEntities.MIGRATION_REGISTERED_COURSES.Add(migrationRegisteredCourses);
                    contextEntities.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<StudentDetails> GenerateList(string mailFolderPath)
        {
            try
            {
                var ListOfstudentDetails = new List<StudentDetails>();
                string Mainpath = mailFolderPath;
                string[] MainfilePaths = Directory.GetDirectories(Mainpath);

                for (int a = 0; a < MainfilePaths.Count(); a++)
                {
                    string dirPath = MainfilePaths[a];
                    string[] SeconLevelfilePaths = Directory.GetDirectories(dirPath);
                    string DepartmentName = Path.GetFileName(Path.GetDirectoryName(SeconLevelfilePaths.FirstOrDefault()));

                    for (int b = 0; b < SeconLevelfilePaths.Count(); b++)
                    {
                        string FilePath = SeconLevelfilePaths[b];
                        string[] ExcelfilePaths = Directory.GetFiles(FilePath, "*.*",
                            SearchOption.AllDirectories);
                        string ProgramName = Path.GetFileName(Path.GetDirectoryName(ExcelfilePaths.FirstOrDefault()));

                        for (int c = 0; c < ExcelfilePaths.Count(); c++)
                        {
                            string path = ExcelfilePaths[c];
                            string classLevel = Path.GetFileName(path).Substring(0, 3);
                            DataSet UploadedStudentDetails = ReadExcel(path);
                            if (UploadedStudentDetails != null && UploadedStudentDetails.Tables[0].Rows.Count > 0)
                            {
                                int noOfColWithGrades = 0;

                                for (int j = 0; j < UploadedStudentDetails.Tables[0].Columns.Count; j++)
                                {
                                    if (UploadedStudentDetails.Tables[0].Columns[j].Caption != "SN"
                                        && UploadedStudentDetails.Tables[0].Columns[j].Caption != "FULLNAME"
                                        && UploadedStudentDetails.Tables[0].Columns[j].Caption != "MATRIC NO"
                                        && UploadedStudentDetails.Tables[0].Columns[j].Caption != "CARRY OVER"
                                        && UploadedStudentDetails.Tables[0].Columns[j].Caption != "TUR"
                                        && UploadedStudentDetails.Tables[0].Columns[j].Caption != "TGP"
                                        && UploadedStudentDetails.Tables[0].Columns[j].Caption != "GPA"
                                        && UploadedStudentDetails.Tables[0].Columns[j].Caption != " ")
                                    {
                                        noOfColWithGrades = noOfColWithGrades + 1;
                                    }
                                }
                                for (int i = 1; i < UploadedStudentDetails.Tables[0].Rows.Count; i++)
                                {
                                    for (int k = 3; k < (noOfColWithGrades + 3); k++)
                                    {
                                        var studentDetails = new StudentDetails();
                                        studentDetails.FullName = UploadedStudentDetails.Tables[0].Rows[i][1].ToString();
                                        studentDetails.MatricNumber =
                                            UploadedStudentDetails.Tables[0].Rows[i][2].ToString();

                                        studentDetails.CourseCode =
                                            UploadedStudentDetails.Tables[0].Columns[k].Caption.Replace("_", " ");
                                        string scoreGrade = UploadedStudentDetails.Tables[0].Rows[i][k].ToString();

                                        int lenght = scoreGrade.Length;
                                        if (lenght > 1 && lenght == 3)
                                        {
                                            int ignore;
                                            if (int.TryParse(scoreGrade.Substring(0, 2), out ignore))
                                            {
                                                studentDetails.Score = scoreGrade.Substring(0, 2);
                                            }
                                            studentDetails.Grade = scoreGrade.Substring(2, 1);
                                        }
                                        else if (lenght > 1 && lenght == 2)
                                        {
                                            int ignore;
                                            if (int.TryParse(scoreGrade.Substring(0, 1), out ignore))
                                            {
                                                studentDetails.Score = scoreGrade.Substring(0, 1);
                                            }
                                            studentDetails.Grade = scoreGrade.Substring(1, 1);
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                        studentDetails.Unit = UploadedStudentDetails.Tables[0].Rows[0][k].ToString();
                                        studentDetails.Level = classLevel;
                                        studentDetails.Program = ProgramName;
                                        studentDetails.Department = DepartmentName;
                                        ListOfstudentDetails.Add(studentDetails);
                                    }
                                }
                            }
                        }
                    }
                }
                return ListOfstudentDetails;
                Console.ReadLine();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static DataSet ReadExcel(string filepath)
        {
            DataSet Result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath +
                                  ";Extended Properties='Excel 12.0;IMEX=1;'";
                var connection = new OleDbConnection(xConnStr);

                connection.Open();
                DataTable sheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow dataRow in sheet.Rows)
                {
                    string sheetName = dataRow[2].ToString();

                    var command = new OleDbCommand("Select * FROM [" + sheetName + "]", connection);
                    // Create DbDataReader to Data Worksheet

                    var MyData = new OleDbDataAdapter();
                    MyData.SelectCommand = command;
                    var ds = new DataSet();
                    ds.Clear();
                    ds.DataSetName = sheetName;
                    MyData.Fill(ds);
                    connection.Close();

                    Result = ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        //public void Move()
        //{
        //    try
        //    {
        //        var semester = new Semester {Id = 1};
        //        var session = new Session {Id = 1};
        //        CourseLogic courseLogic = new CourseLogic();  
        
        //        context = new Abundance_NkEntities();
        //        List<MIGRATE_STUDENTS> allMigrateStudents = context.MIGRATE_STUDENTS.ToList();
        //        foreach (var migrateStudent in allMigrateStudents)
        //        {
        //            List<MIGRATE_FIRST_SEMESTER_RESULT> StudentResults = context.MIGRATE_FIRST_SEMESTER_RESULT.Where(a => a.MATRIC_NO == migrateStudent.MATRIC_NO).ToList();
        //            var studentRecord = context.STUDENT_LEVEL.Where(b => b.STUDENT.Matric_Number == migrateStudent.MATRIC_NO).FirstOrDefault();
        //            if (studentRecord != null)
        //            {
        //                CourseRegistration courseRegistration = new CourseRegistration();
        //                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
        //                courseRegistration = courseRegistrationLogic.GetModelsBy(c => c.Person_Id == studentRecord.Person_Id && c.Session_Id == 1).FirstOrDefault();
        //                if (courseRegistration != null)
        //                {
        //                    List<CourseRegistrationDetail> listcCourseRegistrationDetails = null;
        //                    foreach (var result in StudentResults)
        //                    {
        //                        CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
        //                        var courseRegistrationDetail =
        //                            courseRegistrationDetailLogic.GetModelsBy(
        //                                a =>
        //                                    a.STUDENT_COURSE_REGISTRATION.Student_Course_Registration_Id == courseRegistration.Id &&
        //                                    a.COURSE.Course_Code == result.COURSE_CODE).FirstOrDefault();

        //                        if (courseRegistrationDetail != null)
        //                        {
        //                            courseRegistrationDetail.TestScore =  Convert.ToDecimal(result.TEST_SCORE);
        //                            courseRegistrationDetail.ExamScore = Convert.ToDecimal(result.EXAM_SCORE);
        //                            courseRegistrationDetailLogic.Modify(courseRegistrationDetail);
        //                        }
        //                        else
        //                        {
        //                            var courseDetail =courseLogic.GetModelsBy(d => d.Course_Code == result.COURSE_CODE && d.Department_Id ==studentRecord.Department_Id && d.Programme_Id == studentRecord.Programme_Id ).FirstOrDefault();
        //                            if (courseDetail != null && courseDetail.Id > 0)
        //                            {
        //                                courseRegistrationDetail = new CourseRegistrationDetail();
        //                                courseRegistrationDetail.CourseRegistration = courseRegistration;
        //                                courseRegistrationDetail.Course = courseDetail;
        //                                courseRegistrationDetail.Mode = new CourseMode() {Id = 1};
        //                                courseRegistrationDetail.Semester = semester;
        //                                courseRegistrationDetail.TestScore = Convert.ToDecimal(result.TEST_SCORE);
        //                                courseRegistrationDetail.ExamScore = Convert.ToDecimal(result.EXAM_SCORE);
        //                                courseRegistrationDetailLogic.Create(courseRegistrationDetail);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Level level = null;
        //                    List<CourseRegistrationDetail>  listcCourseRegistrationDetails = new List<CourseRegistrationDetail>();
        //                    foreach (var result in StudentResults)
        //                    {
                               
        //                        CourseRegistrationDetail detail = new CourseRegistrationDetail();
        //                        var courseDetail =courseLogic.GetModelsBy(d => d.Course_Code == result.COURSE_CODE && d.Department_Id ==studentRecord.Department_Id && d.Programme_Id == studentRecord.Programme_Id ).FirstOrDefault();
        //                        if (courseDetail != null && courseDetail.Id > 0)
        //                        {
        //                            level = courseDetail.Level;
        //                            detail.Course = courseDetail;
        //                            detail.CourseUnit = courseDetail.Unit;
        //                            detail.Mode = new CourseMode(){Id = 1};
        //                            detail.Semester = semester;
        //                            detail.TestScore =  Convert.ToDecimal(result.TEST_SCORE);
        //                            detail.ExamScore = Convert.ToDecimal(result.EXAM_SCORE);
        //                            listcCourseRegistrationDetails.Add(detail);
        //                        }
                                
        //                    }

        //                    if (listcCourseRegistrationDetails.Count > 0)
        //                    {
        //                        courseRegistration = new CourseRegistration();
        //                        courseRegistration.Approved = true;
        //                        courseRegistration.DateApproved = DateTime.Now;
        //                        courseRegistration.Department = new Department(){Id = studentRecord.Department_Id };
        //                        courseRegistration.Level = level;
        //                        courseRegistration.Programme = new Programme(){Id = studentRecord.Programme_Id };
        //                        courseRegistration.Session = session;
        //                        courseRegistration.Student = new Student(){Id = studentRecord.STUDENT.Person_Id};
        //                        courseRegistration.Details = listcCourseRegistrationDetails;
        //                        courseRegistrationLogic.Create(courseRegistration);
        //                    }

                           

        //                }
        //            }
        //            context.MIGRATE_STUDENTS.Remove(migrateStudent);
        //            context.SaveChanges();
        //        }
                
        //    }
        //    catch (Exception)
        //    {
                
        //        throw;
        //    }
        //}

        //   public void Move2()
        //{
        //    try
        //    {
        //        var semester = new Semester {Id = 2};
        //        var session = new Session {Id = 1};
        //        CourseLogic courseLogic = new CourseLogic();  
        
        //        context = new Abundance_NkEntities();
        //        List<MIGRATE_STUDENTS> allMigrateStudents = context.MIGRATE_STUDENTS.ToList();
        //        foreach (var migrateStudent in allMigrateStudents)
        //        {
        //            List<MIGRATE_SECOND_SEMESTER_RESULT> StudentResults = context.MIGRATE_SECOND_SEMESTER_RESULT.Where(a => a.MATRIC_NO == migrateStudent.MATRIC_NO).ToList();
        //            var studentRecord = context.STUDENT_LEVEL.Where(b => b.STUDENT.Matric_Number == migrateStudent.MATRIC_NO).FirstOrDefault();
        //            if (studentRecord != null)
        //            {
        //                CourseRegistration courseRegistration = new CourseRegistration();
        //                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
        //                courseRegistration = courseRegistrationLogic.GetModelsBy(c => c.Person_Id == studentRecord.Person_Id && c.Session_Id == 1).FirstOrDefault();
        //                if (courseRegistration != null)
        //                {
        //                    List<CourseRegistrationDetail> listcCourseRegistrationDetails = null;
        //                    foreach (var result in StudentResults)
        //                    {
        //                        CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
        //                        var courseRegistrationDetail =
        //                            courseRegistrationDetailLogic.GetModelsBy(
        //                                a =>
        //                                    a.STUDENT_COURSE_REGISTRATION.Student_Course_Registration_Id == courseRegistration.Id &&
        //                                    a.COURSE.Course_Code == result.COURSE_CODE).FirstOrDefault();

        //                        if (courseRegistrationDetail != null)
        //                        {
        //                            courseRegistrationDetail.TestScore =  Convert.ToDecimal(result.TEST_SCORE);
        //                            courseRegistrationDetail.ExamScore = Convert.ToDecimal(result.EXAM_SCORE);
        //                            courseRegistrationDetailLogic.Modify(courseRegistrationDetail);
        //                        }
        //                        else
        //                        {
        //                            var courseDetail =courseLogic.GetModelsBy(d => d.Course_Code == result.COURSE_CODE && d.Department_Id ==studentRecord.Department_Id && d.Programme_Id == studentRecord.Programme_Id ).FirstOrDefault();
        //                            if (courseDetail != null && courseDetail.Id > 0)
        //                            {
        //                                courseRegistrationDetail = new CourseRegistrationDetail();
        //                                courseRegistrationDetail.CourseRegistration = courseRegistration;
        //                                courseRegistrationDetail.Course = courseDetail;
        //                                courseRegistrationDetail.Mode = new CourseMode() {Id = 1};
        //                                courseRegistrationDetail.Semester = semester;
        //                                courseRegistrationDetail.TestScore = Convert.ToDecimal(result.TEST_SCORE);
        //                                courseRegistrationDetail.ExamScore = Convert.ToDecimal(result.EXAM_SCORE);
        //                                courseRegistrationDetailLogic.Create(courseRegistrationDetail);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Level level = null;
        //                    List<CourseRegistrationDetail>  listcCourseRegistrationDetails = new List<CourseRegistrationDetail>();
        //                    foreach (var result in StudentResults)
        //                    {
                               
        //                        CourseRegistrationDetail detail = new CourseRegistrationDetail();
        //                        var courseDetail =courseLogic.GetModelsBy(d => d.Course_Code == result.COURSE_CODE && d.Department_Id ==studentRecord.Department_Id && d.Programme_Id == studentRecord.Programme_Id ).FirstOrDefault();
        //                        if (courseDetail != null && courseDetail.Id > 0)
        //                        {
        //                            level = courseDetail.Level;
        //                            detail.Course = courseDetail;
        //                            detail.CourseUnit = courseDetail.Unit;
        //                            detail.Mode = new CourseMode(){Id = 1};
        //                            detail.Semester = semester;
        //                            detail.TestScore =  Convert.ToDecimal(result.TEST_SCORE);
        //                            detail.ExamScore = Convert.ToDecimal(result.EXAM_SCORE);
        //                            listcCourseRegistrationDetails.Add(detail);
        //                        }
                                
        //                    }

        //                    if (listcCourseRegistrationDetails.Count > 0)
        //                    {
        //                        courseRegistration = new CourseRegistration();
        //                        courseRegistration.Approved = true;
        //                        courseRegistration.DateApproved = DateTime.Now;
        //                        courseRegistration.Department = new Department(){Id = studentRecord.Department_Id };
        //                        courseRegistration.Level = level;
        //                        courseRegistration.Programme = new Programme(){Id = studentRecord.Programme_Id };
        //                        courseRegistration.Session = session;
        //                        courseRegistration.Student = new Student(){Id = studentRecord.STUDENT.Person_Id};
        //                        courseRegistration.Details = listcCourseRegistrationDetails;
        //                        courseRegistrationLogic.Create(courseRegistration);
        //                    }

                           

        //                }
        //            }
        //            context.MIGRATE_STUDENTS.Remove(migrateStudent);
        //            context.SaveChanges();
        //        }
                
        //    }
        //    catch (Exception)
        //    {
                
        //        throw;
        //    }
        //}

        public void fixDups()
        {
            try
            {
                context = new Abundance_NkEntities();
                List<COURSE_DUPLICATE> duplicates = new List<COURSE_DUPLICATE>();
                duplicates = context.COURSE_DUPLICATE.ToList();
                foreach (var courseDuplicate in duplicates)
                {
                    var item = new STUDENT_COURSE_REGISTRATION();
                    List<STUDENT_COURSE_REGISTRATION> registrations = context.STUDENT_COURSE_REGISTRATION.Where(a => a.Person_Id == courseDuplicate.Person_Id && a.Session_Id == 1).ToList();
                    if (registrations.Count > 0)
                    {
                        for (int i = 0; i < registrations.Count - 1; i++)
                        {
                            item = registrations[i];
                            List < STUDENT_COURSE_REGISTRATION_DETAIL > del =
                                context.STUDENT_COURSE_REGISTRATION_DETAIL.Where(
                                    a =>
                                        a.Student_Course_Registration_Id ==
                                        item.Student_Course_Registration_Id).ToList();

                            if (del != null)
                            {
                              context.STUDENT_COURSE_REGISTRATION_DETAIL.RemoveRange(del);
                            }
                        }
                        context.STUDENT_COURSE_REGISTRATION.Remove(item);
                        context.COURSE_DUPLICATE.Remove(courseDuplicate);
                         context.SaveChanges();
                    }

                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    

    
    }
}