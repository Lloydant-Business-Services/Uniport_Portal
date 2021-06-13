using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Threading.Tasks;
using System.Globalization;

namespace Abundance_Nk.Business
{
    public class StudentLogic : BusinessBaseLogic<Student, STUDENT>
    {
        private readonly PersonLogic personLogic;
        private readonly StudentMatricNumberAssignmentLogic studentMatricNumberAssignmentLogic;
        private Abundance_NkEntities abundanceNkEntities;
        string appRoot = ConfigurationManager.AppSettings["AppRoot"];
        public StudentLogic()
        {
            personLogic = new PersonLogic();
            translator = new StudentTranslator();
            studentMatricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
            abundanceNkEntities = new Abundance_NkEntities();
        }

        public bool IsStudentInFinalYear(Department departmemt, Level level, Programme programme)
        {
            var degreeAwardedLogic = new DegreeAwardedLogic();
            var duration = degreeAwardedLogic.GetBy(departmemt, programme).Duration ?? 0;

            if ((level.Id * 12) >= duration)
            {
                return true;
            }

            return false;
        }

        public bool ValidateUser(string Username, string Password)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector = p => p.Matric_Number == Username && p.Password_hash == Password;
                List<Student> UserDetails = GetModelsBy(selector);
                if (UserDetails?.Count>0)
                {
                    //UpdateLastLogin(UserDetails);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

      
        public bool ChangeUserPassword(Student student)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector = p => p.Matric_Number == student.MatricNumber;
                STUDENT userEntity = GetEntityBy(selector);
                if (userEntity == null || userEntity.Person_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                userEntity.Password_hash = student.PasswordHash;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> ChangeUserPasswordAsync(Student student)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector = p => p.Matric_Number == student.MatricNumber;
                STUDENT userEntity = await GetEntityByAsync(selector);
                if (userEntity == null || userEntity.Person_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                userEntity.Password_hash = student.PasswordHash;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        public long legitPersonId { get; set; }

        public Student GetBy(string matricNumber, string password)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector =
                    s => s.Matric_Number == matricNumber && s.Password_hash == password;
                return base.GetModelsBy(selector).LastOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DoesStudentExist(string matricNumber)
        {
            try
            {
                 Expression<Func<STUDENT, bool>> selector = s => s.Matric_Number == matricNumber;
                 var student = GetModelsBy(selector).LastOrDefault();
                if (student != null)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return false;
        }
        public Student GetBy(string matricNumber)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector = s => s.Matric_Number == matricNumber;
                return base.GetModelsBy(selector).LastOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Student> GetByAsync(string matricNumber)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector = s => s.Matric_Number == matricNumber;
                return await base.GetModelByAsync(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Student GetBy(long id)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector = s => s.Person_Id == id;
                return base.GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Student> GetByAsync(long id)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector = s => s.Person_Id == id;
                return await base.GetModelByAsync(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Student GetBy(ApplicationForm applicationForm)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector = s => s.Application_Form_Id == applicationForm.Id;
                return base.GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Student> GetByAsync(ApplicationForm applicationForm)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector = s => s.Application_Form_Id == applicationForm.Id;
                return await base.GetModelByAsync(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StudentBiodataIDCardReport> GetBiodataReport()
        {
            try
            {
                List<StudentBiodataIDCardReport> reports =
                    (from a in repository.GetBy<VW_STUDENT_BIODATA>()
                        select new StudentBiodataIDCardReport
                        {
                            Person_Id = a.Person_Id.ToString(),
                            Person_Type_Id = a.Person_Type_Id.ToString(),
                            First_Name = a.First_Name,
                            Last_Name = a.Last_Name,
                            Other_Name = a.Other_Name,
                            Initial = a.Initial,
                            Title = a.Title,
                            Image_File_Url = a.Image_File_Url,
                            Signature_File_Url = a.Signature_File_Url,
                            Sex_Id = a.Sex_Id.ToString(),
                            Contact_Address = a.Contact_Address,
                            Email = a.Email,
                            Mobile_Phone = a.Mobile_Phone,
                            Date_Of_Birth = a.Date_Of_Birth.ToString(),
                            State_Id = a.State_Id,
                            Local_Government_Id = a.Local_Government_Id.ToString(),
                            Home_Town = a.Home_Town,
                            Home_Address = a.Home_Address,
                            Nationality_Id = a.Nationality_Id.ToString(),
                            Religion_Id = a.Religion_Id.ToString(),
                            Date_Entered = a.Date_Entered.ToString(),
                            Role_Id = a.Role_Id.ToString(),
                            Next_of_Kin_Name = a.Next_of_Kin_Name,
                            Next_of_Kin_Address = a.Next_of_Kin_Address,
                            Next_of_Kin_Mobile = a.Next_of_Kin_Mobile,
                            Application_Form_Id = a.Application_Form_Id.ToString(),
                            Sponsor_Name = a.Sponsor_Name,
                            Sponsor_Contact_Address = a.Sponsor_Contact_Address,
                            Sponsor_Mobile_Phone = a.Sponsor_Mobile_Phone,
                            Sponor_Email = a.Sponor_Email,
                            Applicant_Jamb_Registration_Number = a.Applicant_Jamb_Registration_Number,
                            Applicant_Jamb_Score = a.Applicant_Jamb_Score.ToString(),
                            Institution_Choice_Id = a.Institution_Choice_Id.ToString(),
                            Subject1 = a.Subject1.ToString(),
                            Subject2 = a.Subject2.ToString(),
                            Subject3 = a.Subject3.ToString(),
                            Subject4 = a.Subject4.ToString(),
                            Admission_List_Id = a.Admission_List_Id.ToString(),
                            Admission_List_Batch_Id = a.Admission_List_Batch_Id.ToString(),
                            Department_Option_Id = a.Department_Option_Id.ToString(),
                            Activated = a.Activated.ToString(),
                            Student_Type_Id = a.Student_Type_Id.ToString(),
                            Student_Category_Id = a.Student_Category_Id.ToString(),
                            Student_Status_Id = a.Student_Status_Id.ToString(),
                            Student_Number = a.Student_Number.ToString(),
                            Matric_Number = a.Matric_Number,
                            Title_Id = a.Title_Id.ToString(),
                            Marital_Status_Id = a.Marital_Status_Id.ToString(),
                            Blood_Group_Id = a.Blood_Group_Id.ToString(),
                            Genotype_Id = a.Genotype_Id.ToString(),
                            School_Contact_Address = a.School_Contact_Address,
                            Reason = a.Reason,
                            Reject_Category = a.Reject_Category,
                            Password_hash = a.Password_hash,
                            Department_Name = a.Department_Name,
                            Department_Code = a.Department_Code,
                            Faculty_Id = a.Faculty_Id.ToString(),
                            Nationality_Name = a.Nationality_Name,
                            State_Name = a.State_Name,
                            Local_Government_Name = a.Local_Government_Name,
                            Religion_Name = a.Religion_Name,
                            Marital_Status_Name = a.Marital_Status_Name,
                            Sex_Name = a.Sex_Name,
                            Next_Of_kin_Relationship = a.Next_Of_kin_Relationship.ToString(),
                            Sponsor_Relationship = a.Sponsor_Relationship.ToString(),
                            FULLNAME = a.FULLNAME,
                            Blood_Group = a.Blood_Group_Name,
                            YearOfGraduation = a.Year_Of_Graduation.ToString()
                        }).ToList();
                return reports;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public List<Student> GetStudentsBy(string matricNumber)
        {
            try
            {
                Expression<Func<STUDENT, bool>> selector = s => s.Matric_Number == matricNumber;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(Student student)
        {
            try
            {
                STUDENT entity = GetEntityBy(s => s.Person_Id == student.Id);

                if (entity != null)
                {
                    entity.Person_Id = student.Id;
                    entity.Student_Number = student.Number;
                    if (student.MatricNumber != null)
                    {
                        entity.Matric_Number = student.MatricNumber;
                    }

                    entity.School_Contact_Address = student.SchoolContactAddress;
                    entity.Activated = student.Activated;
                    entity.Reason = student.Reason;
                    entity.Reject_Category = student.RejectCategory;
                    if (student.Type != null && student.Type.Id > 0)
                    {
                        entity.Student_Type_Id = student.Type.Id;
                    }

                    if (student.Category != null && student.Category.Id > 0)
                    {
                        entity.Student_Category_Id = student.Category.Id;
                    }

                    if (student.Status != null && student.Status.Id > 0)
                    {
                        entity.Student_Status_Id = student.Status.Id;
                    }
                    if (student.Title != null && student.Title.Id > 0)
                    {
                        entity.Title_Id = student.Title.Id;
                    }
                    if (student.MaritalStatus != null && student.MaritalStatus.Id > 0)
                    {
                        entity.Marital_Status_Id = student.MaritalStatus.Id;
                    }
                    if (student.BloodGroup != null && student.BloodGroup.Id > 0)
                    {
                        entity.Blood_Group_Id = student.BloodGroup.Id;
                    }
                    if (student.Genotype != null && student.Genotype.Id > 0)
                    {
                        entity.Genotype_Id = student.Genotype.Id;
                    }
                    if (student.ApplicationForm != null && student.ApplicationForm.Id > 0)
                    {
                        entity.Application_Form_Id = student.ApplicationForm.Id;
                    }
                    if (student.IsEmailConfirmed != null)
                    {
                        entity.IsEmailConfirmed = student.IsEmailConfirmed;
                    }
                    if (student.Guid != null)
                    {
                        entity.Guid = student.Guid;
                    }
                    if (student.PasswordHash != null)
                    {
                        entity.Password_hash = student.PasswordHash;
                    }

                    int modifiedRecordCount = Save();


                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public bool ModifyStudentId(Student student,long PersonId)
        //{
        //    try
        //    {
        //        STUDENT entity = GetEntityBy(s => s.Person_Id == student.Id);

        //        if (entity != null)
        //        {
        //            entity.Person_Id = PersonId;
        //            int modifiedRecordCount = Save();


        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public long GetNextNumberFor()
        {
            try
            {
                long newSerialNumber = repository.GetMaxValueBy<STUDENT>(s => (int) s.Student_Number);
                newSerialNumber += 1;

                return newSerialNumber;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Student Add(Student student)
        {
            try
            {
                Person person = personLogic.Create(student);
                student.Id = person.Id;

                Student newStudent = base.Create(student);
                if (newStudent != null)
                {
                    newStudent.FullName = person.FullName;
                }

                return newStudent;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Student Create(Student model)
        {
            STUDENT studentEntity = GetEntityBy(s => s.Matric_Number == model.MatricNumber);
            if (studentEntity == null)
            {
                return base.Create(model);
            }
            return null;
        }

        public bool AssignMatricNumber(Person person, long studentNumber, string matricNumber)
        {
            try
            {
                STUDENT studentEntity = GetEntityBy(s => s.Person_Id == person.Id);
                studentEntity.Matric_Number = matricNumber;
                studentEntity.Student_Number = studentNumber;

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

        public bool AssignMatricNumber(ApplicationFormView applicant)
        {
            try
            {
                //Get admission list to be sure of student current department
                AdmissionList admissionList = new AdmissionList();
                AdmissionListLogic admissionLogic = new AdmissionListLogic();
                admissionList = admissionLogic.GetBy(applicant.FormId);
                if (admissionList == null)
                {
                    return false;
                }

                //if (admissionList.Programme.Id == 4)
                //{
                //    //IAS 
                //     return false;
                //}

                Faculty faculty = new Faculty();
                faculty = admissionList.Deprtment.Faculty;

                Department department = new Department();
                department = admissionList.Deprtment;

                Session session = new Session() { Id = applicant.SessionId };
                SessionLogic sessionLogic = new SessionLogic();
                session = sessionLogic.GetModelBy(a => a.Session_Id == session.Id);
                Programme programme = new Programme() { Id = admissionList.Programme.Id };
                

                Level newLevel = new Level() { Id = 1};
                if (admissionList.Form.Number.Contains("DE"))
                {
                    newLevel.Id = 2;
                }

                Student studentRecord = GetBy(applicant.PersonId);
                if (studentRecord != null)
                {
                    
                    Modify(studentRecord);
                    if (programme.Id == 1)
                    {
                        programme.Name = "REGULAR";
                    }
                    else
                    {
                            programme.Name = "IAS";
                    }
                    
                    abundanceNkEntities.STP_GENERATE_STUDENT_MATRIC_NUMBER(applicant.PersonId, applicant.FormId, programme.Id, session.Name.Substring(0,4), session.Id, programme.Name,studentRecord.MatricNumber);

                    StudentLevel studentLevel = new StudentLevel();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    studentLevel = studentLevelLogic.GetBy(applicant.PersonId);
                    if (studentLevel == null)
                    {
                        studentLevel = new StudentLevel();
                        studentLevel.Session = new Session() { Id = studentRecord.ApplicationForm.Setting.Session.Id};
                        studentLevel.Level = newLevel;
                        studentLevel.Student = studentRecord;
                        studentLevel.Department = department;
                        studentLevel.Programme = programme;
                        return studentLevelLogic.Create(studentLevel) != null ? true : false;
                    }
                    else
                    {
                        studentLevel.Session = new Session() { Id = studentRecord.ApplicationForm.Setting.Session.Id};
                        studentLevel.Level = newLevel;
                        studentLevel.Student = studentRecord;
                        studentLevel.Department = department;
                        studentLevel.Programme = programme;
                        return studentLevelLogic.Modify(studentLevel);
                    }
                    
                    
                }
                else
                {
                    
                         //assign matric no to applicant
                        Student student = new Student();
                        student.Id = applicant.PersonId;
                        student.ApplicationForm = new ApplicationForm() { Id = applicant.FormId };
                        student.Type = new StudentType() { Id = 1 };
                        student.Category = new StudentCategory() { Id = 1 };
                        student.Status = new StudentStatus() { Id = 1 };
                        student.Number = null;
                        student.MatricNumber = null;
                        student.PasswordHash = "1234567";
                        Student newStudent = base.Create(student);

                    
                        if (programme.Id == 1)
                        {
                            programme.Name = "REGULAR";
                        }
                        else
                        {
                              programme.Name = "IAS";
                        }

                        var result = abundanceNkEntities.STP_GENERATE_STUDENT_MATRIC_NUMBER(applicant.PersonId, applicant.FormId, programme.Id, session.Name.Substring(0, 4), session.Id ,programme.Name,student.MatricNumber);
                        if (result != null)
                        {
                            StudentLevel studentLevel = new StudentLevel();
                            studentLevel.Session = new Session() { Id = session.Id };
                            studentLevel.Level = newLevel;
                            studentLevel.Student = student;
                            studentLevel.Department = department;
                            studentLevel.Programme = programme;
                            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                            return studentLevelLogic.Create(studentLevel) != null ? true : false;
                        }
                      
                   
                    
                   

                }

                return false;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AssignPGMatricNumber(ApplicationFormView applicant)
        {
            try
            {
                //Get admission list to be sure of student current department
                AdmissionList admissionList = new AdmissionList();
                AdmissionListLogic admissionLogic = new AdmissionListLogic();
                admissionList = admissionLogic.GetBy(applicant.FormId);
                if (admissionList == null)
                {
                    return false;
                }


                Faculty faculty = new Faculty();
                faculty = admissionList.Deprtment.Faculty;

                Department department = new Department();
                department = admissionList.Deprtment;

                Session session = new Session() { Id = applicant.SessionId };
                Programme programme = new Programme() { Id = admissionList.Programme.Id };
                

                Level newLevel = new Level() { Id = 1};
                if (admissionList.Form.Number.Contains("DE"))
                {
                    newLevel.Id = 2;
                }

                Student studentRecord = GetBy(applicant.PersonId);
                if (studentRecord != null)
                {
                    
                    Modify(studentRecord);

                    StudentLevel studentLevel = new StudentLevel();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    studentLevel = studentLevelLogic.GetBy(applicant.PersonId);
                    if (studentLevel == null)
                    {
                        studentLevel = new StudentLevel();
                        //studentLevel.Session = new Session() { Id = 7 };
                        studentLevel.Session = session;
                        studentLevel.Level = newLevel;
                        studentLevel.Student = studentRecord;
                        studentLevel.Department = department;
                        studentLevel.Programme = programme;
                        return studentLevelLogic.Create(studentLevel) != null ? true : false;
                    }
                    else
                    {
                        //studentLevel.Session = new Session() { Id = 7};
                        studentLevel.Session = session;
                        studentLevel.Level = newLevel;
                        studentLevel.Student = studentRecord;
                        studentLevel.Department = department;
                        studentLevel.Programme = programme;
                        return studentLevelLogic.Modify(studentLevel);
                    }
                    
                    
                }
                else
                {
                         //assign matric no to applicant
                        Student student = new Student();
                        student.Id = applicant.PersonId;
                        student.ApplicationForm = new ApplicationForm() { Id = applicant.FormId };
                        student.Type = new StudentType() { Id = 1 };
                        student.Category = new StudentCategory() { Id = 1 };
                        student.Status = new StudentStatus() { Id = 1 };
                        student.Number = null;
                        student.MatricNumber = applicant.FormNumber;
                        student.PasswordHash = "1234567";
                        Student newStudent = base.Create(student);
                        
                        StudentLevel studentLevel = new StudentLevel();
                        studentLevel.Session = new Session() { Id = 7};
                        studentLevel.Level = newLevel;
                        studentLevel.Student = student;
                        studentLevel.Department = department;
                        studentLevel.Programme = programme;
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        return studentLevelLogic.Create(studentLevel) != null ? true : false;
                        
                      
                   
                    
                   

                }

                return false;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool NewAssignPGMatricNumber(ApplicationFormView applicant)
        {
            try
            {
                //Get admission list to be sure of student current department
                AdmissionList admissionList = new AdmissionList();
                AdmissionListLogic admissionLogic = new AdmissionListLogic();
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                SessionLogic sessionLogic = new SessionLogic();
                admissionList = admissionLogic.GetBy(applicant.FormId);
                if (admissionList == null)
                {
                    return false;
                }


                Faculty faculty = new Faculty();
                faculty = admissionList.Deprtment.Faculty;

                Department department = new Department();
                department = admissionList.Deprtment;

                //Session session = new Session() { Id = applicant.SessionId };
                Session session = sessionLogic.GetModelBy(d => d.Session_Id == applicant.SessionId);
                // Programme programme = new Programme() { Id = admissionList.Programme.Id };
                Programme programme = programmeLogic.GetModelsBy(d=>d.Programme_Id== admissionList.Programme.Id).FirstOrDefault();

                Level newLevel = new Level() { Id = 1 };
                if (admissionList.Form.Number.Contains("DE"))
                {
                    newLevel.Id = 2;
                }
                PersonMergerLogic personMergerLogic = new PersonMergerLogic();
                //Check if applicant has done any course in the institution, previously
                var hasStudentRecordPreviously = personMergerLogic.GetModelsBy(g => g.New_Person_Id == applicant.PersonId).FirstOrDefault();
                Student studentRecord = new Student();
                Student oldStudentRecord = new Student();
                if (hasStudentRecordPreviously?.PersonMergerId > 0)
                {
                     oldStudentRecord = GetBy(hasStudentRecordPreviously.OldPerson.Id);
                    studentRecord = GetBy(hasStudentRecordPreviously.NewPerson.Id);
                    if (oldStudentRecord?.Id > 0 && oldStudentRecord?.MatricNumber!=null && studentRecord==null)
                    {
                        
                        //Create a new record of student, but assign the old matric number to the student
                        Student student = new Student();
                        student.Id = applicant.PersonId;
                        student.ApplicationForm = new ApplicationForm() { Id = applicant.FormId };
                        student.Type = new StudentType() { Id = 1 };
                        student.Category = new StudentCategory() { Id = 1 };
                        student.Status = new StudentStatus() { Id = 1 };
                        student.Number = null;
                        student.MatricNumber = oldStudentRecord.MatricNumber;
                        student.PasswordHash = "1234567";
                        Student newStudent = base.Create(student);
                        //ModifyStudentId(studentRecord, applicant.PersonId);
                        studentRecord = GetBy(applicant.PersonId);
                    }
                }
                else
                {
                    studentRecord = GetBy(applicant.PersonId);
                }
                
                if (studentRecord != null)
                {

                    Modify(studentRecord);

                    StudentLevel studentLevel = new StudentLevel();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    studentLevel = studentLevelLogic.GetBy(applicant.PersonId);
                    if (studentLevel == null)
                    {
                        studentLevel = new StudentLevel();
                        //studentLevel.Session = new Session() { Id = 7 };
                        studentLevel.Session = session;
                        studentLevel.Level = newLevel;
                        studentLevel.Student = studentRecord;
                        studentLevel.Department = department;
                        studentLevel.Programme = programme;
                        return studentLevelLogic.Create(studentLevel) != null ? true : false;
                    }
                    else
                    {
                        //studentLevel.Session = new Session() { Id = 7};
                        studentLevel.Session = session;
                        studentLevel.Level = newLevel;
                        studentLevel.Student = studentRecord;
                        studentLevel.Department = department;
                        studentLevel.Programme = programme;
                        return studentLevelLogic.Modify(studentLevel);
                    }


                }
                else
                {
                    //assign matric no to applicant
                    Student student = new Student();
                    student.Id = applicant.PersonId;
                    student.ApplicationForm = new ApplicationForm() { Id = applicant.FormId };
                    student.Type = new StudentType() { Id = 1 };
                    student.Category = new StudentCategory() { Id = 1 };
                    student.Status = new StudentStatus() { Id = 1 };
                    student.Number = null;
                    student.MatricNumber = null;
                    student.PasswordHash = "1234567";
                    Student newStudent = base.Create(student);

                    var result = abundanceNkEntities.STP_GENERATE_POST_GRADUATE_MATRIC_NUMBER(applicant.PersonId, applicant.FormId, programme.Id, session.Name.Substring(0, 4), session.Id, programme.Name, student.MatricNumber);
                    if (result != null)
                    {
                        StudentLevel studentLevel = new StudentLevel();
                        studentLevel.Session = new Session() { Id = session.Id };
                        studentLevel.Level = newLevel;
                        studentLevel.Student = student;
                        studentLevel.Department = department;
                        studentLevel.Programme = programme;
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        return studentLevelLogic.Create(studentLevel) != null ? true : false;
                    }

                }

                return false;

            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool UpdateMatricNumber(StudentLevel studentLevel, Student student)
        {
            try
            {
                //StudentMatricNumberAssignment startMatricNo = studentMatricNumberAssignmentLogic.GetBy(studentLevel.Department.Faculty, studentLevel.Department, studentLevel.Programme, studentLevel.Level, studentLevel.Session);
                //if (startMatricNo != null)
                //{
                //    long studentNumber = 0;
                //    string matricNumber = "";

                //    if (startMatricNo.Used)
                //    {
                //        string[] matricNoArray = startMatricNo.MatricNoStartFrom.Split('/');
                //        studentNumber = GetNextStudentNumber(studentLevel.Department.Faculty, studentLevel.Department, studentLevel.Programme, studentLevel.Level, studentLevel.Session);
                //        matricNoArray[matricNoArray.Length - 1] = UtilityLogic.PaddNumber(studentNumber, 4);
                //        matricNumber = string.Join("/", matricNoArray);


                //    }
                //    else
                //    {
                //        matricNumber = startMatricNo.MatricNoStartFrom;
                //        studentNumber = startMatricNo.MatricSerialNoStartFrom;
                //        bool markedAsUsed = studentMatricNumberAssignmentLogic.MarkAsUsed(startMatricNo);
                //    }


                //    student.Status = new StudentStatus() { Id = 1 };
                //    student.Number = studentNumber;
                //    student.MatricNumber = matricNumber;
                //    return Modify(student);


                //}
                //else
                //{
                //    throw new Exception(studentLevel.Level.Name + " for " + studentLevel.Department.Name + " for the current academic session has not been set! Please contact your system administrator.");
                //}
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long GetNextStudentNumber(Faculty faculty, Department department, Programme programme, Level level,
            Session session)
        {
            try
            {
                long newStudentNumber = 0;
                List<ApplicationFormView> applicationForms =
                    (from a in
                        repository.GetBy<VW_ASSIGNED_MATRIC_NUMBER>(
                            a =>
                                a.Department_Id == department.Id && a.Programme_Id == programme.Id &&
                                a.Level_Id == level.Id && a.Session_Id == session.Id)
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


                //List<ApplicationFormView> applicationForms = (from s in repository.GetBy<STUDENT_LEVEL>(s => s.DEPARTMENT.Faculty_Id == faculty.Id && s.Department_Id == department.Id && s.Programme_Id == programme.Id && s.Level_Id == level.Id && s.Session_Id == session.Id)
                //           select new ApplicationFormView
                //           {
                //               FormId = (long) s.STUDENT.Application_Form_Id,
                //               StudentNumber = (long) s.STUDENT.Student_Number,
                //               MatricNumber = s.STUDENT.Matric_Number,
                //               PersonId = s.Person_Id,

                //           }).ToList();

                //if (applicationForms != null && applicationForms.Count > 0)
                //{
                //    long rawMaxStudentNumber = applicationForms.Max(s => s.StudentNumber);
                //    newStudentNumber = rawMaxStudentNumber + 1;
                //}

                return newStudentNumber;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StudentReport> GetStudentInformationBy(Department department, Programme programme, Session session,
            Level level)
        {
            try
            {
                var studentReportList = new List<StudentReport>();
                var studentLevelList = new List<StudentLevel>();
                var studentLevelLogic = new StudentLevelLogic();
                var personLogic = new PersonLogic();
                if (department != null && programme != null && session != null && level != null)
                {
                    studentLevelList =
                        studentLevelLogic.GetModelsBy(
                            p =>
                                p.Department_Id == department.Id && p.Programme_Id == programme.Id &&
                                p.Level_Id == level.Id && p.Session_Id == session.Id);
                    foreach (StudentLevel studentLevelitem in studentLevelList)
                    {
                        var person = new Person();
                        var studentReport = new StudentReport();
                        person = personLogic.GetModelBy(p => p.Person_Id == studentLevelitem.Student.Id);
                        studentReport.Fullname = person.LastName + " " + person.FirstName + " " + person.OtherName;
                        studentReport.RegistrationNumber = studentLevelitem.Student.MatricNumber;
                        studentReport.Address = person.HomeAddress;
                        studentReport.Othernames = person.FirstName + "  " + person.OtherName;
                        studentReport.Lastname = person.LastName;
                        if (person.Sex != null)
                        {
                            studentReport.Sex = person.Sex.Name;
                        }
                        else
                        {
                            studentReport.Sex = "";
                        }
                        if (person.State != null)
                        {
                            studentReport.State = person.State.Name;
                        }
                        else
                        {
                            studentReport.State = "";
                        }
                        studentReport.MobileNumber = person.MobilePhone;
                        if (person.LocalGovernment != null)
                        {
                            studentReport.LocalGovernment = person.LocalGovernment.Name;
                        }
                        else
                        {
                            studentReport.LocalGovernment = "";
                        }

                        studentReportList.Add(studentReport);
                    }
                    return studentReportList.OrderBy(a => a.RegistrationNumber).ToList();
                }
                return new List<StudentReport>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetRelationShip(int RelationShipId)
        {
            string relationshipName = "";
            try
            {
                var relationshipLogic = new RelationshipLogic();
                Relationship relationship = relationshipLogic.GetModelBy(a => a.Relationship_Id == RelationShipId);
                if (relationship != null)
                {
                    relationshipName = relationship.Name;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return relationshipName;
        }

        public List<StudentReport> GetStudentBiodata(Person person)
        {
            try
            {
                List<StudentReport> studentReport =
                    (from a in repository.GetBy<VW_STUDENT_BIODATA>(a => a.Person_Id == person.Id)
                        select new StudentReport
                        {
                            Fullname = a.Last_Name + " " + a.First_Name + " " + a.Other_Name,
                            Title = a.Title,
                            Sex = a.Sex_Name,
                            DateOfBirth = a.Date_Of_Birth.ToString(),
                            Nationality = a.Nationality_Name,
                            PlaceOfBirth = a.Home_Town,
                            PlaceOfOrigin = a.Home_Town,
                            LocalGovernment = a.Local_Government_Name,
                            State = a.State_Name,
                            MaritalStatus = a.Marital_Status_Name,
                            Religion = a.Religion_Name,
                            ContactAddress = a.Contact_Address,
                            HomeAddress = a.Home_Address,
                            MobileNumber = a.Mobile_Phone,
                            TelephoneNumber = a.Mobile_Phone,
                            NextOfKinName = a.Next_of_Kin_Name,
                            NextOfKinAddress = a.Next_of_Kin_Address,
                            NextOfKinPhone = a.Next_of_Kin_Mobile,
                            NextOfKinRelatioship = GetRelationShip(a.Next_Of_kin_Relationship),
                            SponsorName = a.Sponsor_Name,
                            SponsorAddress = a.Sponsor_Contact_Address,
                            SponsorPhone = a.Sponsor_Mobile_Phone,
                            SponsorRelationship = GetRelationShip(a.Sponsor_Relationship),
                            Lastname = a.Image_File_Url,
                            RegistrationNumber = a.Applicant_Jamb_Registration_Number,
                            Department = a.Department_Name,
                        }).ToList();
                return studentReport;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StudentReport> GetStudentCertificateOfEligibility(Person person)
        {
            try
            {
                List<StudentReport> studentReport =
                    (from a in repository.GetBy<VW_STUDENT_O_LEVEL_RESULT_DETAIL>(a => a.Person_Id == person.Id)
                        select new StudentReport
                        {
                            Fullname = a.Name,
                            Title = a.Title,
                            Sex = a.Sex_Id.ToString(),
                            DateOfBirth = a.Date_Of_Birth.ToString(),
                            Nationality = a.Nationality_Name,
                            PlaceOfBirth = a.Home_Town,
                            PlaceOfOrigin = a.Home_Town,
                            LocalGovernment = a.Local_Government_Name,
                            State = a.State_Name,
                            MaritalStatus = a.Marital_Status_Name,
                            Religion = a.Religion_Name,
                            ContactAddress = a.Contact_Address,
                            HomeAddress = a.Home_Address,
                            TelephoneNumber = a.Mobile_Phone,
                            NextOfKinName = a.Next_of_Kin_Name,
                            NextOfKinAddress = a.Next_of_Kin_Address,
                            NextOfKinPhone = a.Next_of_Kin_Mobile,
                            SponsorName = a.Sponsor_Name,
                            SponsorAddress = a.Sponsor_Contact_Address,
                            SponsorPhone = a.Sponsor_Mobile_Phone,
                            OLevelGrade = a.O_Level_Grade_Name,
                            OLevelSubject = a.O_Level_Subject_Name,
                            OLevelType = a.O_Level_Type_Name,
                            OLevelYear = a.Exam_Year.ToString(),
                            RegistrationNumber = a.Matric_Number,
                            Department = a.Department_Name
                        }).ToList();

                return studentReport;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StudentReport> GetStudentOLevelDetail(Person person)
        {
            try
            {
                var studentReportList = new List<StudentReport>();
                var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                List<OLevelResultDetail> oLevelResultDetails =
                    oLevelResultDetailLogic.GetModelsBy(a => a.APPLICANT_O_LEVEL_RESULT.Person_Id == person.Id);

                if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                {
                    foreach (OLevelResultDetail oLevelResultDetail in oLevelResultDetails)
                    {
                        var studentReport = new StudentReport();
                        studentReport.OLevelGrade = oLevelResultDetail.Grade.Name;
                        studentReport.OLevelSubject = oLevelResultDetail.Subject.Name;
                        studentReport.OLevelType = oLevelResultDetail.Header.Type.Name;
                        studentReport.OLevelYear = oLevelResultDetail.Header.ExamYear.ToString();
                        studentReportList.Add(studentReport);
                    }
                }


                return studentReportList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StudentReport> GetRegisteredStudentBy(Department department, Programme programme, Session session,
            Level level)
        {
            try
            {
                var studentReportList = new List<StudentReport>();
                var courseRegistrationList = new List<CourseRegistration>();

                var courseLogic = new CourseRegistrationLogic();
                var personLogic = new PersonLogic();
                if (department != null && programme != null && session != null && level != null)
                {
                    courseRegistrationList =
                        courseLogic.GetModelsBy(
                            c =>
                                c.Department_Id == department.Id && c.Level_Id == level.Id &&
                                c.Programme_Id == programme.Id && c.Session_Id == session.Id);
                    foreach (CourseRegistration registration in courseRegistrationList)
                    {
                        var studentReport = new StudentReport();
                        studentReport.Fullname = registration.Student.LastName + " " + registration.Student.FirstName +
                                                 " " + registration.Student.OtherName;
                        studentReport.RegistrationNumber = registration.Student.MatricNumber;
                        studentReport.Address = registration.Student.HomeAddress;
                        studentReport.Othernames = registration.Student.FirstName + "  " +
                                                   registration.Student.OtherName;
                        studentReport.Lastname = registration.Student.LastName;
                        if (registration.Student.Sex != null)
                        {
                            studentReport.Sex = registration.Student.Sex.Name;
                        }
                        else
                        {
                            studentReport.Sex = "";
                        }
                        if (registration.Student.State != null)
                        {
                            studentReport.State = registration.Student.State.Name;
                        }
                        else
                        {
                            studentReport.State = "";
                        }
                        studentReport.MobileNumber = registration.Student.MobilePhone;
                        if (registration.Student.LocalGovernment != null)
                        {
                            studentReport.LocalGovernment = registration.Student.LocalGovernment.Name;
                        }
                        else
                        {
                            studentReport.LocalGovernment = "";
                        }

                        studentReportList.Add(studentReport);
                        //studentReportList.OrderBy(p => p.RegistrationNumber);
                    }
                    return studentReportList.OrderBy(a => a.RegistrationNumber).ToList();
                }
                return new List<StudentReport>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private long GetNextStudentNumber(StudentLevel studentLevel)
        {
            try
            {
                var studentList = new List<StudentLevel>();
                var studentLogic = new StudentLevelLogic();
                studentList =
                    studentLogic.GetModelsBy(
                        s =>
                            s.Department_Id == studentLevel.Department.Id && s.Level_Id == studentLevel.Level.Id &&
                            s.Programme_Id == studentLevel.Programme.Id && s.Session_Id == studentLevel.Session.Id);
                var studentNumber = (long) studentList.Max(p => p.Student.Number);
                long newNumber = studentNumber + 1;
                return newNumber;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CheckMatricNumberDuplicate(string matricNumber)
        {
            try
            {
                var student = new Student();
                var studentLogic = new StudentLogic();
                var studentList = new List<StudentLevel>();
                var studentListSort = new List<StudentLevel>();
                var studentLevel = new StudentLevel();
                var studentLevelLogic = new StudentLevelLogic();
                studentList = studentLevelLogic.GetModelsBy(x => x.STUDENT.Matric_Number == matricNumber);
                foreach (StudentLevel studentItem in studentList)
                {
                    if (studentList.Count() > 1)
                    {
                        studentList.RemoveAt(0);
                        for (int i = 0; i < studentList.Count(); i++)
                        {
                            studentItem.Student.Number = null;
                            // UpdateMatricNumber(studentListSort[i]);
                            using (var scope = new TransactionScope())
                            {
                                var courseRegistrationLogic = new CourseRegistrationLogic();
                                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                                CourseRegistration courseRegistration =
                                    courseRegistrationLogic.GetBy(studentItem.Student, studentItem.Level,
                                        studentItem.Programme, studentItem.Department, studentItem.Session);
                                if (courseRegistration != null && courseRegistration.Id > 0)
                                {
                                    Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                                        cr => cr.Student_Course_Registration_Id == courseRegistration.Id;
                                    if (courseRegistrationDetailLogic.Delete(selector))
                                    {
                                        Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> deleteSelector =
                                            cr => cr.Student_Course_Registration_Id == courseRegistration.Id;
                                        courseRegistrationLogic.Delete(deleteSelector);
                                    }
                                    else
                                    {
                                        Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> deleteSelector =
                                            cr => cr.Student_Course_Registration_Id == courseRegistration.Id;
                                        courseRegistrationLogic.Delete(deleteSelector);
                                        scope.Complete();
                                    }
                                }

                                Expression<Func<STUDENT_LEVEL, bool>> deleteStudentLevelSelector =
                                    sl => sl.Person_Id == studentItem.Student.Id;
                                if (studentLevelLogic.Delete(deleteStudentLevelSelector))
                                {
                                    Expression<Func<STUDENT, bool>> deleteStudentSelector =
                                        sl => sl.Person_Id == studentItem.Student.Id;
                                    if (studentLogic.Delete(deleteStudentSelector))
                                    {
                                        var applicantLogic = new ApplicantLogic();
                                        ApplicationFormView applicant = applicantLogic.GetBy(studentItem.Student);
                                        if (applicant != null)
                                        {
                                            bool matricNoAssigned = studentLogic.AssignMatricNumber(applicant);
                                            if (matricNoAssigned)
                                            {
                                                scope.Complete();
                                            }
                                        }
                                        else
                                        {
                                            scope.Complete();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateMatricNumber(StudentLevel studentLevel)
        {
            try
            {
                StudentMatricNumberAssignment startMatricNo =
                    studentMatricNumberAssignmentLogic.GetBy(studentLevel.Department.Faculty, studentLevel.Department,
                        studentLevel.Programme, studentLevel.Level, studentLevel.Session);
                if (startMatricNo != null)
                {
                    long studentNumber = 0;
                    string matricNumber = "";

                    if (startMatricNo.Used)
                    {
                        string[] matricNoArray = startMatricNo.MatricNoStartFrom.Split('/');
                        studentNumber = GetNextStudentNumber(studentLevel);
                        matricNoArray[matricNoArray.Length - 1] = UtilityLogic.PaddNumber(studentNumber, 4);
                        matricNumber = string.Join("/", matricNoArray);
                    }
                    else
                    {
                        matricNumber = startMatricNo.MatricNoStartFrom;
                        studentNumber = startMatricNo.MatricSerialNoStartFrom;
                        bool markedAsUsed = studentMatricNumberAssignmentLogic.MarkAsUsed(startMatricNo);
                    }


                    studentLevel.Student.Number = studentNumber;
                    studentLevel.Student.MatricNumber = matricNumber;
                    return Modify(studentLevel.Student);
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteMatricNumberDuplicate(string matricNumber)
        {
            try
            {
                var student = new Student();
                var studentLogic = new StudentLogic();
                var studentList = new List<StudentLevel>();
                var studentListSort = new List<StudentLevel>();
                var studentLevel = new StudentLevel();
                var studentLevelLogic = new StudentLevelLogic();
                studentList = studentLevelLogic.GetModelsBy(x => x.STUDENT.Matric_Number == matricNumber);
                if (studentList != null && studentList.Count > 0)
                {
                    foreach (StudentLevel studentItem in studentList)
                    {
                        using (var scope = new TransactionScope())
                        {
                            var courseRegistrationLogic = new CourseRegistrationLogic();
                            var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                            CourseRegistration courseRegistration = courseRegistrationLogic.GetBy(studentItem.Student,
                                studentItem.Level, studentItem.Programme, studentItem.Department, studentItem.Session);
                            if (courseRegistration != null && courseRegistration.Id > 0)
                            {
                                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                                    cr => cr.Student_Course_Registration_Id == courseRegistration.Id;
                                if (courseRegistrationDetailLogic.Delete(selector))
                                {
                                    Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> deleteSelector =
                                        cr => cr.Student_Course_Registration_Id == courseRegistration.Id;
                                    courseRegistrationLogic.Delete(deleteSelector);
                                }
                                else
                                {
                                    Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> deleteSelector =
                                        cr => cr.Student_Course_Registration_Id == courseRegistration.Id;
                                    courseRegistrationLogic.Delete(deleteSelector);
                                    scope.Complete();
                                }
                            }

                            Expression<Func<STUDENT_LEVEL, bool>> deleteStudentLevelSelector =
                                sl => sl.Person_Id == studentItem.Student.Id;
                            if (studentLevelLogic.Delete(deleteStudentLevelSelector))
                            {
                                Expression<Func<STUDENT, bool>> deleteStudentSelector =
                                    sl => sl.Person_Id == studentItem.Student.Id;
                                if (studentLogic.Delete(deleteStudentSelector))
                                {
                                    Expression<Func<ONLINE_PAYMENT, bool>> deleteOnlinePaymentSelector =
                                        sl => sl.PAYMENT.Person_Id == studentItem.Student.Id;
                                    var onlinePyamentLogic = new OnlinePaymentLogic();
                                    if (onlinePyamentLogic.Delete(deleteOnlinePaymentSelector))
                                    {
                                        Expression<Func<PAYMENT, bool>> deletePaymentSelector =
                                            sl => sl.Person_Id == studentItem.Student.Id;
                                        var paymentLogic = new PaymentLogic();
                                        paymentLogic.Delete(deletePaymentSelector);
                                    }


                                    Expression<Func<PERSON, bool>> deletePersonSelector =
                                        sl => sl.Person_Id == studentItem.Student.Id;
                                    if (personLogic.Delete(deletePersonSelector))
                                    {
                                        scope.Complete();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteDuplicateMatricNumber(string matricNumber)
        {
            try
            {
                var studentLogic = new StudentLogic();
                var studentList = new List<StudentLevel>();
                var studentLevel = new StudentLevel();
                var studentLevelLogic = new StudentLevelLogic();
                studentList = studentLevelLogic.GetModelsBy(x => x.STUDENT.Matric_Number == matricNumber);
                legitPersonId = GetLegitimateStudent(studentList);
                if (studentList != null && studentList.Count > 0)
                {
                    foreach (StudentLevel studentItem in studentList)
                    {
                        using (var scope = new TransactionScope())
                        {
                            var courseRegistrationLogic = new CourseRegistrationLogic();
                            int currentSessionId = 0;
                            List<CourseRegistration> courseRegistrations =
                                courseRegistrationLogic.GetListBy(studentItem.Student, studentItem.Programme,
                                    studentItem.Department);
                            foreach (CourseRegistration courseregistration in courseRegistrations)
                            {
                                if (courseregistration.Session.Id == 1)
                                {
                                    currentSessionId = 1;
                                }
                            }
                            if ((courseRegistrations.Count == 0 || currentSessionId == 1) &&
                                studentItem.Student.Id != legitPersonId)
                            {
                                if (courseRegistrations.Count != 0)
                                {
                                    var courseRegistrationDetails = new List<CourseRegistrationDetail>();
                                    var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                                    CourseRegistration courseReg =
                                        courseRegistrationLogic.GetModelBy(
                                            p =>
                                                p.Person_Id == studentItem.Student.Id &&
                                                p.Session_Id == currentSessionId);
                                    courseRegistrationDetails =
                                        courseRegistrationDetailLogic.GetModelsBy(
                                            crd => crd.Student_Course_Registration_Id == courseReg.Id);
                                    if (courseRegistrationDetails.Count > 0)
                                    {
                                        foreach (
                                            CourseRegistrationDetail courseRegistrationDetail in
                                                courseRegistrationDetails)
                                        {
                                            Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>>
                                                deleteCourseRegistrationDetailSelector =
                                                    crd => crd.Student_Course_Registration_Id == courseReg.Id;
                                            courseRegistrationDetailLogic.Delete(deleteCourseRegistrationDetailSelector);
                                        }
                                    }

                                    Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> deleteCourseRegistrationSelector
                                        = cr => cr.Student_Course_Registration_Id == courseReg.Id;
                                    courseRegistrationLogic.Delete(deleteCourseRegistrationSelector);
                                }

                                //List<STUDENT_LEVEL> studentLevels = studentLevelLogic.GetEntitiesBy(sl => sl.Person_Id == studentItem.Student.Id);
                                //if (studentLevels.Count > 1)
                                //{
                                //    studentLevelLogic.Delete(studentLevels);
                                //}

                                Expression<Func<STUDENT_LEVEL, bool>> deleteStudentLevelSelector =
                                    sl => sl.Student_Level_Id == studentItem.Id;

                                if (studentLevelLogic.Delete(deleteStudentLevelSelector))
                                {
                                    CheckStudentSponsor(studentItem);
                                    CheckStudentFinanceInformation(studentItem);
                                    CheckStudentAcademicInformation(studentItem);
                                    CheckStudentResultDetails(studentItem);
                                    CheckStudentNDResult(studentItem);
                                    CheckStudentEmploymentImformation(studentItem);
                                    Expression<Func<STUDENT, bool>> deleteStudentSelector =
                                        s => s.Person_Id == studentItem.Student.Id;
                                    if (studentLogic.Delete(deleteStudentSelector))
                                    {
                                        Expression<Func<ONLINE_PAYMENT, bool>> deleteOnlinePaymentSelector =
                                            op => op.PAYMENT.Person_Id == studentItem.Student.Id;
                                        var paymentLogic = new PaymentLogic();
                                        var paymentEtransactLogic = new PaymentEtranzactLogic();
                                        PaymentEtranzact paymentEtranzact =
                                            paymentEtransactLogic.GetModelBy(
                                                pe => pe.ONLINE_PAYMENT.PAYMENT.Person_Id == studentItem.Student.Id);
                                        if (paymentEtranzact == null)
                                        {
                                            var onlinePyamentLogic = new OnlinePaymentLogic();
                                            if (onlinePyamentLogic.Delete(deleteOnlinePaymentSelector))
                                            {
                                                Expression<Func<PAYMENT, bool>> deletePaymentSelector =
                                                    p => p.Person_Id == studentItem.Student.Id;

                                                paymentLogic.Delete(deletePaymentSelector);
                                            }
                                        }
                                        else
                                        {
                                            var payments = new List<Payment>();
                                            payments =
                                                paymentLogic.GetModelsBy(p => p.Person_Id == studentItem.Student.Id);
                                            var person = new Person {Id = legitPersonId};
                                            foreach (Payment payment in payments)
                                            {
                                                payment.Person = person;
                                                paymentLogic.Modify(payment);
                                            }
                                        }

                                        var studentExtraYearLogic = new StudentExtraYearLogic();
                                        Expression<Func<STUDENT_EXTRA_YEAR_SESSION, bool>> extraYearSessionSelector =
                                            eys => eys.Person_Id == studentItem.Student.Id;
                                        StudentExtraYearSession studentExtraYearSession =
                                            studentExtraYearLogic.GetModelBy(extraYearSessionSelector);
                                        if (studentExtraYearSession != null)
                                        {
                                            studentExtraYearLogic.Delete(extraYearSessionSelector);
                                        }

                                        scope.Complete();
                                    }
                                }
                            }
                            else
                            {
                                legitPersonId = studentItem.Student.Id;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CheckStudentEmploymentImformation(StudentLevel studentItem)
        {
            try
            {
                var studentEmploymentInformationLogic = new StudentEmploymentInformationLogic();
                StudentEmploymentInformation studentEmploymentInformation =
                    studentEmploymentInformationLogic.GetModelBy(ss => ss.Person_Id == studentItem.Student.Id);
                if (studentEmploymentInformation != null)
                {
                    Expression<Func<STUDENT_EMPLOYMENT_INFORMATION, bool>> deleteStudentEmploymentInformation =
                        ss => ss.Person_Id == studentItem.Student.Id;
                    studentEmploymentInformationLogic.Delete(deleteStudentEmploymentInformation);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CheckStudentNDResult(StudentLevel studentItem)
        {
            try
            {
                var studentNDResultLogic = new StudentNdResultLogic();
                StudentNdResult studentNDResult =
                    studentNDResultLogic.GetModelBy(ss => ss.Person_Id == studentItem.Student.Id);
                if (studentNDResult != null)
                {
                    Expression<Func<STUDENT_ND_RESULT, bool>> deleteStudentNDResult =
                        ss => ss.Person_Id == studentItem.Student.Id;
                    studentNDResultLogic.Delete(deleteStudentNDResult);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private long GetLegitimateStudent(List<StudentLevel> studentList)
        {
            long personId = 0;
            try
            {
                foreach (StudentLevel studentItem in studentList)
                {
                    var courseRegistrationLogic = new CourseRegistrationLogic();

                    List<CourseRegistration> courseRegistrations = courseRegistrationLogic.GetListBy(
                        studentItem.Student, studentItem.Programme, studentItem.Department);
                    foreach (CourseRegistration courseregistration in courseRegistrations)
                    {
                        if (courseRegistrations.Count >= 1 && courseregistration.Session.Id != 1)
                        {
                            personId = courseregistration.Student.Id;
                        }
                        else if (courseRegistrations.Count == 1 && personId == 0)
                        {
                            personId = courseregistration.Student.Id;
                        }
                    }
                }
                if (personId == 0)
                {
                    personId = studentList.Where(s => s.Session.Id == 1).FirstOrDefault().Student.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return personId;
        }

        private static void CheckStudentSponsor(StudentLevel studentItem)
        {
            var studentSponsorLogic = new StudentSponsorLogic();
            StudentSponsor studentSponsor = studentSponsorLogic.GetModelBy(ss => ss.Person_Id == studentItem.Student.Id);
            if (studentSponsor != null)
            {
                Expression<Func<STUDENT_SPONSOR, bool>> deleteStudentSponsorSelector =
                    ss => ss.Person_Id == studentItem.Student.Id;
                studentSponsorLogic.Delete(deleteStudentSponsorSelector);
            }
        }

        private static void CheckStudentFinanceInformation(StudentLevel studentItem)
        {
            var studentFinanceInformationLogic = new StudentFinanceInformationLogic();
            StudentFinanceInformation studentFinanceInformation =
                studentFinanceInformationLogic.GetModelBy(ss => ss.Person_Id == studentItem.Student.Id);
            if (studentFinanceInformation != null)
            {
                Expression<Func<STUDENT_FINANCE_INFORMATION, bool>> deleteStudentFinanceInfoSelector =
                    sfi => sfi.Person_Id == studentItem.Student.Id;
                studentFinanceInformationLogic.Delete(deleteStudentFinanceInfoSelector);
            }
        }

        private static void CheckStudentAcademicInformation(StudentLevel studentItem)
        {
            var studentAcademicInformationLogic = new StudentAcademicInformationLogic();
            StudentAcademicInformation studentAcademicInformation =
                studentAcademicInformationLogic.GetModelBy(ss => ss.Person_Id == studentItem.Student.Id);
            if (studentAcademicInformation != null)
            {
                Expression<Func<STUDENT_ACADEMIC_INFORMATION, bool>> deleteStudentAcademicInfoSelector =
                    sai => sai.Person_Id == studentItem.Student.Id;
                studentAcademicInformationLogic.Delete(deleteStudentAcademicInfoSelector);
            }
        }

        private static void CheckStudentResultDetails(StudentLevel studentItem)
        {
            var studentResultDetailLogic = new StudentResultDetailLogic();
            StudentResultDetail studentResultDetail =
                studentResultDetailLogic.GetModelBy(srd => srd.Person_Id == studentItem.Student.Id);
            if (studentResultDetail != null)
            {
                Expression<Func<STUDENT_RESULT_DETAIL, bool>> deleteStudentResultDetailSelector =
                    srd => srd.Person_Id == studentItem.Student.Id;
                studentResultDetailLogic.Delete(deleteStudentResultDetailSelector);
            }
        }
        public List<StudentDetails> GetStudentDetialBy(Session session, Programme programme)
        {
            try
            {
                var studentDetailList = from a in repository.GetBy<VW_STDUENT_DETAILS>(a => a.Session_Id == session.Id && a.Programme_Id == programme.Id)
                                        select new StudentDetails
                                        {
                                            FullName = a.NAME,
                                            Sex = a.Sex_Name,
                                            ExamNumber = a.Application_Exam_Number,
                                            ApplicationForm = a.Application_Form_Number,
                                            Department = a.Department_Name,
                                            Program = a.Programme_Name,
                                            PhoneNumber = a.Mobile_Phone,
                                            MatricNumber = a.Matric_Number,
                                            Level = a.Level_Name,
                                            Session = a.Session_Name
                                        };
                return studentDetailList.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AdmissionListModel> GetStudentIdData(Session session)
        {
            try
            {
                var studentDetailList = from a in repository.GetBy<VW_STUDENT_ID_DATA>(a => a.Session_Id == session.Id)
                                        select new AdmissionListModel
                                        {
                                            FullName = a.FULL_NAME,
                                            PersonId = a.Person_Id,
                                            BloodGroup = a.BLOOD_GROUP,
                                            MatricNumber = a.Matric_Number,
                                            Department = a.DEPARTMENT,
                                            Programme = a.PROGRAMME,
                                            SessionName = a.SESSION,
                                            YearOfAdmission = a.Year_Of_Admission,
                                            YearOfGraduation = a.Year_Of_Graduation,
                                            ImageUrl = a.IMAGE_URL
                                        };
                return studentDetailList.OrderBy(a => a.Programme).ThenBy(a => a.Department).ThenBy(a => a.MatricNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AdmissionListModel> GetStudentIdData(DateTime DateFrom, DateTime DateTo)
        {
            try
            {

                
                var studentDetailList = from a in repository.GetBy<VW_STUDENT_ID_DATA>().ToList().Where(a => a.Date_Entered >= DateFrom && a.Date_Entered <= DateTo)
                                        select new AdmissionListModel
                                        {
                                            FullName = a.FULL_NAME,
                                            PersonId = a.Person_Id,
                                            BloodGroup = a.BLOOD_GROUP,
                                            MatricNumber = a.Matric_Number,
                                            Department = a.DEPARTMENT,
                                            Programme = a.PROGRAMME,
                                            SessionName = a.SESSION,
                                            YearOfAdmission = a.Year_Of_Admission,
                                            YearOfGraduation = a.Year_Of_Graduation,
                                            ImageUrl = a.IMAGE_URL
                                        };
                    return studentDetailList.OrderBy(a => a.Programme).ThenBy(a => a.Department).ThenBy(a => a.MatricNumber).ToList();
                
            
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AdmittedStudentDataView> GetAdmittedStudentDataBy(Department department, Programme programme, Session session,Person person)
        {
            try
            {
                

                var admittedStudentData = from a in repository.GetBy<VW_ADMITTED_STUDENT_DATA>(a => a.Session_Id == session.Id && a.Department_Id == department.Id &&
                    a.Programme_Id == programme.Id && a.Person_Id == person.Id )
                                        select new AdmittedStudentDataView
                                        {
                                            Name = a.NAME,
                                            PersonId = a.Person_Id,
                                            DepartmentId = a.Department_Id,
                                            DepartmentName = a.Department_Name,
                                            ProgrammeId = a.Programme_Id,
                                            ProgrammeName =  a.Programme_Name,
                                            SessionId = a.Session_Id,
                                            SessionName = a.Session_Name,
                                            LocalGovernmentName  = a.Local_Government_Name,
                                            StateName = a.State_Name,
                                            MobilePhone =  a.Mobile_Phone,
                                            Email = a.Email,
                                            SponsorName =  a.Sponsor_Name,
                                            SponsorContactAddress =  a.Sponsor_Contact_Address,
                                            SponsorMobilePhone =  a.Sponsor_Mobile_Phone,
                                            RelationshipName =  a.Relationship_Name,
                                            OLevelSubjectName =  a.O_Level_Subject_Name,
                                            OLevelGradeName = a.O_Level_Grade_Name,
                                            OLevelTypeName = a.O_Level_Type_Name,
                                            OLevelExamSittingName = a.O_Level_Exam_Sitting_Name,
                                            ImageFileUrl = appRoot + a.Image_File_Url,
                                            ApplicantJambRegistrationNumber = a.Applicant_Jamb_Registration_Number,
                                            ApplicantJambScore =  a.Applicant_Jamb_Score,
                                            OLevelExamSittingId = a.O_Level_Exam_Sitting_Id,
                                            ExamYear = a.Exam_Year,
                                            ExamNumber =  a.Exam_Number

                                            
                                        };
                return admittedStudentData.ToList();
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        public List<Result> GetTranscriptBy(Student student, Department department)
        {
            try
            {
                List<Result> results = new List<Result>();
                if (student == null || student.Id <= 0)
                {
                    throw new Exception("Student not set! Please select student and try again.");
                }
                else if (department.Id == 50)
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Person_Id == student.Id)
                               select new Result
                               {
                                   StudentId = sr.Person_Id,
                                   Sex = sr.Sex_Name,
                                   //Name = sr.na,
                                   MatricNumber = sr.Matric_Number,
                                   CourseId = sr.Course_Id,
                                   CourseCode=sr.Course_Code,
                                   CourseName = sr.Course_Name,
                                   CourseUnit = sr.Course_Unit,
                                   FacultyName = sr.Faculty_Name,
                                   TestScore = sr.Test_Score,
                                   ExamScore = sr.Exam_Score,
                                   Score = sr.Total_Score,
                                   Grade = sr.Grade,
                                   GradePoint = sr.Exam_Score,
                                   Email = sr.Email,
                                   Address = sr.Contact_Address,
                                   MobilePhone = sr.Mobile_Phone,
                                   PassportUrl = sr.Image_File_Url,
                                   GPCU = (decimal?)(sr.Grade_Point * sr.Course_Unit),
                                   SessionName = sr.Session_Name,
                                   Semestername = sr.Semester_Name,
                                   LevelName = sr.Level_Name,
                                   ProgrammeName = sr.Programme_Name,
                                   DepartmentName = sr.Department_Name,
                                   SessionSemesterId = sr.Session_Semester_Id,
                                   SequenceNumber = sr.Sequence_Number,
                                   SessionId = sr.Session_Id,
                                   SemesterId = sr.Semester_Id,
                                   LevelId = sr.Level_Id,
                                   ProgrammeId = sr.Programme_Id,
                                    DepartmentId= sr.Department_Id,
                                   //DateOfBirth = sr.,
                                   //Country = sr.Country

                               }).ToList();
                }
                else
                {
                    results = (from sr in repository.GetBy<VW_STUDENT_RESULT_2>(x => x.Person_Id == student.Id)
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
                                   GradePoint = sr.Exam_Score,
                                   Email = sr.Email,
                                   Address = sr.Contact_Address,
                                   MobilePhone = sr.Mobile_Phone,
                                   PassportUrl = sr.Image_File_Url,
                                   //GPCU = (decimal?)(sr.Total_Score * sr.Course_Unit),
                                   GPCU = (decimal?)(sr.Grade_Point * sr.Course_Unit),
                                   SessionName = sr.Session_Name,
                                   Semestername = sr.Semester_Name,
                                   LevelName = sr.Level_Name,
                                   ProgrammeName = sr.Programme_Name,
                                   DepartmentName = sr.Department_Name,
                                   SessionSemesterId = sr.Session_Semester_Id,
                                   SequenceNumber = sr.Sequence_Number,
                                   SessionId = sr.Session_Id,
                                   SemesterId = sr.Semester_Id,
                                   LevelId = sr.Level_Id,
                                   ProgrammeId = sr.Programme_Id,
                                   DepartmentId = sr.Department_Id,
                               }).ToList();
                }



                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetTodaysDateFormat()
        {
            string dateFormat = "";
            try
            {
                DateTime date = DateTime.Today;
                int monthId = date.Month;
                string monthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(monthId);

                //var s = (date.Day%10 == 1 && date.Day != 11)
                //    ? "st"
                //    : (date.Day%10 == 2 && date.Day != 12)
                //        ? "nd"
                //        : (date.Day % 10 == 3 && date.Day != 13)
                //            ? "rd"
                //            : "th";

                dateFormat = monthName + " " + date.Day + "," + " " + date.Year;
            }
            catch (Exception)
            {

                throw;
            }
            return dateFormat;
        }
        public string GetGraduationStatus(decimal? CGPA, string graduationDate)
        {
            string remark = null;
            //int graduationId = Convert.ToInt32(graduationDate);
            try
            {
                //if (CGPA != null && graduationId < 1989)
                //{

                //    if (CGPA >= (decimal)3.50 && CGPA <= (decimal)4.00)
                //    {
                //        remark = "FIRST CLASS";
                //    }
                //    else if (CGPA >= (decimal)3.00 && CGPA <= (decimal)3.49)
                //    {
                //        remark = "SECOND CLASS (UPPER DIVISION)";
                //    }
                //    else if (CGPA >= (decimal)2.50 && CGPA <= (decimal)2.99)
                //    {
                //        remark = "SECOND CLASS (LOWER DIVISION)";
                //    }
                //    else if (CGPA >= (decimal)2.20 && CGPA <= (decimal)2.49)
                //    {
                //        remark = "THIRD CLASS";
                //    }
                //    else if (CGPA >= (decimal)2.00 && CGPA <= (decimal)2.19)
                //    {
                //        remark = "PASS";
                //    }
                //    else if (CGPA < (decimal)2.0)
                //    {
                //        remark = "FAIL";
                //    }
                //}
                //else
                //{
                    if (CGPA >= (decimal)4.5 && CGPA <= (decimal)5.0)
                    {
                        remark = "FIRST CLASS";
                    }
                    else if (CGPA >= (decimal)3.50 && CGPA <= (decimal)4.49)
                    {
                        remark = "SECOND CLASS (UPPER DIVISION)";
                    }
                    else if (CGPA >= (decimal)2.40 && CGPA <= (decimal)3.49)
                    {
                        remark = "SECOND CLASS (LOWER DIVISION)";
                    }
                    else if (CGPA >= (decimal)1.5 && CGPA <= (decimal)2.39)
                    {
                        remark = "THIRD CLASS";
                    }
                    else if (CGPA >= (decimal)1.00 && CGPA <= (decimal)1.49)
                    {
                        remark = "PASS";
                    }
                    else if (CGPA < (decimal)1.0)
                    {
                        remark = "FAIL";
                    }
                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return remark;
        }
        public List<StudentCountSummaryDataView> GetReturningNewStudentCountBy(Session session)
        {
            List<StudentCountSummaryDataView> newStudentList = new List<StudentCountSummaryDataView>();
            try
            {
                //New Student Count
                var newStudent = from a in repository.GetBy<VW_NEW_STUDENTCOUNT>(a =>(a.Session_Id == session.Id && a.Programme_Id == 1) || (a.Session_Id == session.Id && a.Programme_Id == 4)).ToList()
                                          select new StudentCountSummaryDataView
                                          {
                                              
                                              DepartmentName=a.Department_Name,
                                              SessionName=a.Session_Name,
                                              ProgrammeId=a.Programme_Id,
                                              DepartmentId=a.Department_Id,
                                              ProgrammeName=a.Programme_Name,
                                              NewStudentCount=(int)a.NEW_Student_Count

                                          };
                newStudentList = newStudent.ToList();
                //Old Student Count
                var oldStudent = from a in repository.GetBy<VW_RETURNING_STUDENTCOUNT>(a => (a.Session_Id == session.Id && a.Programme_Id == 1) || (a.Session_Id == session.Id && a.Programme_Id == 4))
                                 select new StudentCountSummaryDataView
                                 {

                                     DepartmentName = a.Department_Name,
                                     SessionName = a.Session_Name,
                                     ProgrammeId = a.Programme_Id,
                                     DepartmentId = a.Department_Id,
                                     ReturningStudentCount = (int)a.Returning_Student_Count

                                 };

                var oldStudentList = oldStudent.ToList();
                if (newStudentList.Count > 0)
                {
                    for(int i=0; i < newStudentList.Count; i++)
                    {
                        var deptId = newStudentList[i].DepartmentId;
                        var programmeId = newStudentList[i].ProgrammeId;
                        var oldStudentCount= oldStudentList.Where(d => d.ProgrammeId == programmeId && d.DepartmentId == deptId).FirstOrDefault();
                        newStudentList[i].ReturningStudentCount = oldStudentCount != null ? oldStudentCount.ReturningStudentCount : 0;
                    }
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return newStudentList.OrderBy(c=>c.ProgrammeName).ThenBy(d=>d.DepartmentName).ToList();
        }
        public List<Result> GetAbuaStateIndigenesRecord(Session session, int departmentId)
        {
            List<Result> studentList = new List<Result>();
            FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
            try
            {
               
                List<string> sessionNameList = new List<string>();
                if(session?.Id>0 && !string.IsNullOrEmpty(session.Name))
                {
                    sessionNameList=SessionList(session);
                    var studentDetailList = from a in repository.GetBy<VW_ABIASTUDENTINDIGENESREPORT>().ToList().Where(a =>a.Department_Id == departmentId && a.CalculatedLevelId > 0
                                            && (a.Programme_Id == 1 || a.Programme_Id == 4))
                                            select new Result
                                            {
                                                Name = a.Name,
                                                StudentId = (long)a.Person_Id,
                                                MatricNumber = a.Matric_Number,
                                                DepartmentName = a.Department_Name,
                                                ProgrammeName = a.Programme_Name,
                                                SessionName = session.Name,
                                                SessionId = session.Id,
                                                DepartmentId = (int)a.Department_Id,
                                                ProgrammeId = a.Programme_Id,
                                                LevelId = a.CalculatedLevelId,
                                                LevelName = a.CalculatedLevelName,
                                                LGA=a.Local_Government_Name,
                                                DepartmentOptionId=a.Department_Option_Id!=null?(int)a.Department_Option_Id:0
                                            };
                    var distinctmatricNos=studentDetailList.GroupBy(a=>a.StudentId).ToList();
                    //Get total due payment for all programme for selected department
                    var feeDetails=feeDetailLogic.GetModelsBy(f =>f.Department_Id == departmentId && f.Session_Id == session.Id && f.Payment_Mode_Id == (int)PaymentModes.FullInstallment && f.Fee_Type_Id==(int)FeeTypes.SchoolFees);
                    foreach (var item in distinctmatricNos)
                    {
                        var record=studentDetailList.Where(a => a.StudentId == item.Key).LastOrDefault();
                        //Get total due payment
                           var  duePayment = feeDetails.Where(f => f.Programme.Id == record.ProgrammeId && f.Department.Id == record.DepartmentId
                          && f.Level.Id == record.LevelId).Sum(f => f.Fee.Amount);
                        
                        
                        //get the total amount paid by student
                        
                            var amountPaid=AmountPaidByStudentPerSession(session.Id, item.Key);
                        record.TotalFeesPaid= amountPaid > 0 ? string.Format("{0:0,0.00}", amountPaid) : "0";
                        record.TotalFeesDue = duePayment>0 ? string.Format("{0:0,0.00}", duePayment) : "0";
                        record.Balance = (duePayment-amountPaid )> 0 ? string.Format("{0:0,0.00}", (duePayment - amountPaid)) : "0";
                        studentList.Add(record);

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return studentList.OrderBy(a=>a.LevelName).ToList();
        }
        public List<Result> GetAbuaStateIndigenesRecordCount(Session session)
        {
            List<Result> studentList = new List<Result>();
            List<Result> DepartmentStudentList = new List<Result>();
            try
            {

                List<string> sessionNameList = new List<string>();
                if (session?.Id > 0 && !string.IsNullOrEmpty(session.Name))
                {
                    sessionNameList = SessionList(session);
                    var studentDetailList = from a in repository.GetBy<VW_ABIASTUDENTINDIGENESREPORT>().ToList().Where(a =>a.CalculatedLevelId>0
                                            && (a.Programme_Id == 1 || a.Programme_Id == 4))
                                            select new Result
                                            {
                                                Name = a.Name,
                                                StudentId = (long)a.Person_Id,
                                                MatricNumber = a.Matric_Number,
                                                DepartmentName = a.Department_Name,
                                                ProgrammeName = a.Programme_Name,
                                                SessionName = session.Name,
                                                SessionId = session.Id,
                                                DepartmentId =(int) a.Department_Id,
                                                ProgrammeId = a.Programme_Id,
                                                LevelId = (int)a.Level_Id,
                                                LevelName = a.Level_Name
                                            };
                    var distinctmatricNos = studentDetailList.GroupBy(a => a.StudentId).ToList();
                    foreach (var item in distinctmatricNos)
                    {
                        var record = studentDetailList.Where(a => a.StudentId == item.Key).LastOrDefault();
                        studentList.Add(record);

                    }
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    var allDepartment=departmentLogic.GetAll();
                    foreach(var dept in allDepartment)
                    {
                        var studentbyDepartment=studentList.Where(a => a.DepartmentId == dept.Id).ToList();
                        if (studentbyDepartment?.Count > 0)
                        {
                            Result deptResult = new Result()
                            {
                                DepartmentName = dept.Name,
                                Count = studentbyDepartment.Count,
                                SessionName = session.Name
                            };
                            DepartmentStudentList.Add(deptResult);
                        }
                    }
                   

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return DepartmentStudentList.OrderBy(a => a.DepartmentName).ToList();
        }
        public List<string> SessionList(Session session)
        {
            List<string> sessionNameList = new List<string>();
            if (session?.Id > 0 && !string.IsNullOrEmpty(session.Name))
            {
                var splitSessionname = session.Name.Split('/');
                int firstSessionName = Convert.ToInt32(splitSessionname[0]);
                int secondSessionName = Convert.ToInt32(splitSessionname[1]);
                sessionNameList.Add(session.Name);
                for (int i = 1; i < 5; i++)
                {
                  var sessionName= Convert.ToString((firstSessionName - i) + "/" +(secondSessionName - i));
                    sessionNameList.Add(sessionName);
                }
            }
            return sessionNameList;
        }
        public decimal AmountPaidByStudentPerSession(int sessionId, long studentId)
        {
            PaymentLogic paymentLogic = new PaymentLogic();
            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
            PaystackLogic paystackLogic = new PaystackLogic();
            string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
            string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
            string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
            string MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();
            PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL, MonnifyUser, MonnifySecrect, MonnifyCode);
            decimal amountPaid = 0M;
            var generatedPaymentInvoices=paymentLogic.GetModelsBy(f => f.Person_Id == studentId && f.Session_Id == sessionId && f.Fee_Type_Id==(int)FeeTypes.SchoolFees);
            
            for(int i=0; i<generatedPaymentInvoices.Count; i++)
            {
                long paymentId = generatedPaymentInvoices[i].Id;
                var paymentEtranzact=paymentEtranzactLogic.GetModelBy(f => f.Payment_Id == paymentId);
                var paystack=paystackLogic.GetModelBy(f => f.Payment_Id == paymentId && f.status.Contains("success") && f.domain.Contains("live"));
               var monnify= paymentMonnifyLogic.GetModelBy(f => f.Payment_Id == paymentId && f.Completed);
                if (paymentEtranzact != null)
                {
                    amountPaid += (decimal)paymentEtranzact.TransactionAmount;
                }
                else if (paystack != null)
                {
                    amountPaid += (decimal)((paystack.amount)/100);
                }
                else if (monnify != null)
                {
                    amountPaid += (decimal)monnify.AmountPaid;
                }
                
            }
            return amountPaid;
        }
        public List<Result> UndergraduateNominalReport(Session session, int departmentId)
        {
            List<Result> studentList = new List<Result>();
            try
            {

                List<string> sessionNameList = new List<string>();
                if (session?.Id > 0 && !string.IsNullOrEmpty(session.Name))
                {
                    sessionNameList = SessionList(session);
                    var studentDetailList = from a in repository.GetBy<VW_UNDERGRADUATE_NOMINAL_REPORT>().ToList().Where(a => a.Department_Id == departmentId && a.CalculatedLevelId > 0
                                            && (a.Programme_Id == 1 || a.Programme_Id == 4))
                                            select new Result
                                            {
                                                Name = a.Name,
                                                StudentId = (long)a.Person_Id,
                                                MatricNumber = a.Matric_Number,
                                                DepartmentName = a.Department_Name,
                                                ProgrammeName = a.Programme_Name,
                                                SessionName = session.Name,
                                                SessionId = session.Id,
                                                DepartmentId = (int)a.Department_Id,
                                                ProgrammeId = a.Programme_Id,
                                                LevelId = a.CalculatedLevelId,
                                                LevelName = a.CalculatedLevelName,
                                                LGA = a.Local_Government_Name,
                                                DepartmentOptionId = a.Department_Option_Id != null ? (int)a.Department_Option_Id : 0,
                                                ReportTitle="UNDERGRADUATE NOMINAL ROLL FOR ",
                                                State=a.State_Name
                                            };
                    var distinctmatricNos = studentDetailList.GroupBy(a => a.StudentId).ToList();
                    foreach (var item in distinctmatricNos)
                    {
                        var record = studentDetailList.Where(a => a.StudentId == item.Key).FirstOrDefault();
                        studentList.Add(record);

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return studentList.OrderBy(a => a.LevelName).ThenBy(a=>a.State).ToList();
        }
        public List<Result> PGNominalReport(Session session, int departmentId)
        {
            List<Result> studentList = new List<Result>();
            try
            {

                List<string> sessionNameList = new List<string>();
                if (session?.Id > 0 && !string.IsNullOrEmpty(session.Name))
                {
                    sessionNameList = SessionList(session);
                    var studentDetailList = from a in repository.GetBy<VW_PGSTUDENT_NOMINAL_REPORT>().ToList().Where(a => a.Department_Id == departmentId && a.GeneratedLevelId > 0)
                                            select new Result
                                            {
                                                Name = a.Name,
                                                StudentId = (long)a.Person_Id,
                                                MatricNumber = a.Matric_No,
                                                DepartmentName = a.Department_Name,
                                                ProgrammeName = a.Programme_Name,
                                                SessionName = session.Name,
                                                SessionId = session.Id,
                                                DepartmentId = (int)a.Department_Id,
                                                ProgrammeId = a.Programme_Id,
                                                LevelId = a.GeneratedLevelId,
                                                LevelName = a.GeneratedLevel,
                                                LGA = a.Local_Government_Name,
                                                ReportTitle = "POST GRADUATE NOMINAL ROLL FOR ",
                                                State = a.State_Name
                                            };
                    var distinctmatricNos = studentDetailList.GroupBy(a => a.StudentId).ToList();
                    foreach (var item in distinctmatricNos)
                    {
                        var record = studentDetailList.Where(a => a.StudentId == item.Key).FirstOrDefault();
                        studentList.Add(record);

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return studentList.OrderBy(a => a.LevelName).ThenBy(a => a.State).ToList();
        }

    }
}