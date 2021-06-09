using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class UploadReturningStudentsController :Controller
    {
        private const string ID = "Id";
        private const string NAME = "Name";

        public ActionResult ReturningStudents()
        {
            var viewModel = new UploadReturningStudentViewModel();
            try
            {
                if (TempData["UploadedStudent"] != null)
                {
                    viewModel.UploadedStudents = (List<UploadedStudentModel>) TempData["UploadedStudent"];
                }

                populateDropdowns(viewModel);
                return View(viewModel);
            }
            catch(Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ReturningStudents(UploadReturningStudentViewModel viewModel)
        {
            try
            {
                var programme = new Programme();
                var programmeLogic = new ProgrammeLogic();
                var departmentLogic = new DepartmentLogic();
                var department = new Department();
                var level = new Level();
                var levelLogic = new LevelLogic();
                var session = new Session();
                var sessionLogic = new SessionLogic();

                programme = programmeLogic.GetModelBy(p => p.Programme_Id == viewModel.Programme.Id);
                department = departmentLogic.GetModelBy(p => p.Department_Id == viewModel.Department.Id);
                level = levelLogic.GetModelBy(p => p.Level_Id == viewModel.Level.Id);
                session = sessionLogic.GetModelBy(p => p.Session_Id == viewModel.Session.Id);
                var returningStudentList = new List<ReturningStudents>();
                foreach(string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving,hpf.FileName);
                    hpf.SaveAs(savedFileName);
                    IExcelServiceManager excelServiceManager = new ExcelServiceManager();
                    //DataSet studentList = ReadExcel(savedFileName);
                    DataSet studentList  = excelServiceManager.UploadExcel(savedFileName);
                    if(studentList != null && studentList.Tables[0].Rows.Count > 0)
                    {
                        for(int i = 0;i < studentList.Tables[0].Rows.Count;i++)
                        {
                            var returningStudent = new ReturningStudents();
                            returningStudent.MatricNumber = studentList.Tables[0].Rows[i][0].ToString();
                            returningStudent.Firstname = studentList.Tables[0].Rows[i][1].ToString();
                            returningStudent.Surname = studentList.Tables[0].Rows[i][2].ToString();
                            returningStudent.Othername = studentList.Tables[0].Rows[i][3].ToString();
                            returningStudent.Gender = studentList.Tables[0].Rows[i][4].ToString();
                            returningStudent.State = studentList.Tables[0].Rows[i][5].ToString();
                            returningStudent.LocalGovernmentArea = studentList.Tables[0].Rows[i][6].ToString();
                            returningStudentList.Add(returningStudent);
                        }
                    }
                }

                viewModel.ReturningStudentList = returningStudentList;
                keepDropdownState(viewModel);
                TempData["UploadReturningStudentViewModel"] = viewModel;
                return View(viewModel);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult SaveUpload()
        {
            try
            {
                var studentLogic = new StudentLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                LevelLogic levelLogic = new LevelLogic();
                SessionLogic sessionLogic = new SessionLogic();

                var viewModel = (UploadReturningStudentViewModel)TempData["UploadReturningStudentViewModel"];

                List<UploadedStudentModel> uploadedStudents = new List<UploadedStudentModel>();
                Department department = departmentLogic.GetModelBy(d => d.Department_Id == viewModel.Department.Id);
                Programme programme = programmeLogic.GetModelBy(p => p.Programme_Id == viewModel.Programme.Id);
                Level level = levelLogic.GetModelBy(l => l.Level_Id == viewModel.Level.Id);
                Model.Model.Session session = sessionLogic.GetModelBy(s => s.Session_Id == viewModel.Session.Id);

                if(viewModel.ReturningStudentList != null && viewModel.ReturningStudentList.Count() > 0)
                {
                    for(int i = 0;i < viewModel.ReturningStudentList.Count();i++)
                    {
                        if(viewModel.ReturningStudentList[i].Surname.Trim() != "" ||
                            viewModel.ReturningStudentList[i].Firstname.Trim() != "")
                        {
                            using(var scope = new TransactionScope())
                            {
                                var person = new Person();
                                var student = new Model.Model.Student();
                                var studentLevel = new StudentLevel();
                                person.LastName = viewModel.ReturningStudentList[i].Surname;
                                person.FirstName = viewModel.ReturningStudentList[i].Firstname;
                                person.OtherName = viewModel.ReturningStudentList[i].Othername;
                                student.MatricNumber = viewModel.ReturningStudentList[i].MatricNumber;
                                string gender = viewModel.ReturningStudentList[i].Gender;
                                string state = viewModel.ReturningStudentList[i].State;
                                string localGovernmentArea = viewModel.ReturningStudentList[i].LocalGovernmentArea;
                                person = CreatePerson(person,gender,state,localGovernmentArea);
                                student = CreateStudent(student,person,viewModel);
                                studentLevel = CreateStudentLevel(student,viewModel);

                                UploadedStudentModel uploadedStudent = new UploadedStudentModel();

                                uploadedStudent.Name = person.LastName + " " + person.FirstName + " " + person.OtherName;
                                uploadedStudent.MatricNumber = student.MatricNumber;
                                uploadedStudent.Department = department.Name;
                                uploadedStudent.Level = level.Name;
                                uploadedStudent.Programme = programme.Name;
                                uploadedStudent.Session = session.Name;

                                uploadedStudents.Add(uploadedStudent);
                                
                                scope.Complete();
                            }
                        }
                        //  student = studentLogic.GetModelBy(p => p.Matric_Number == viewModel.ReturningStudentList[i].MatricNumber.Trim());
                    }

                    TempData["UploadedStudent"] = uploadedStudents;
                    TempData["Action"] = "Upload Successful";
                }
                else
                {
                    TempData["Action"] = "Data Empty";
                }
                return RedirectToAction("ReturningStudents");
            }
            catch(Exception)
            {
                throw;
            }
        }

        private StudentLevel CreateStudentLevel(Model.Model.Student student,UploadReturningStudentViewModel viewModel)
        {
            try
            {
                var studentLevel = new StudentLevel();
                var studentLevelLogic = new StudentLevelLogic();
                studentLevel.Student = student;
                if(viewModel != null)
                {
                    studentLevel.Level = viewModel.Level;
                    studentLevel.Programme = viewModel.Programme;
                    studentLevel.Department = viewModel.Department;
                    studentLevel.Session = viewModel.Session;
                }

                if (student != null && student.Id > 0 && studentLevelLogic.GetModelsBy(s => s.Person_Id == student.Id).LastOrDefault() == null)
                {
                    studentLevel = studentLevelLogic.Create(studentLevel);
                }
                
                return studentLevel;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private Model.Model.Student CreateStudent(Model.Model.Student student,Person person, UploadReturningStudentViewModel viewModel)
        {
            try
            {
                StudentType studentType = null;
                var studentCategory = new StudentCategory { Id = 2 };
                var studentStatus = new StudentStatus { Id = 1 };
                var maritalStatus = new MaritalStatus { Id = 1 };
                var personLogic = new PersonLogic();
                person = personLogic.GetModelBy(p => p.Person_Id == person.Id);
                Title title = null;
                var studentLogic = new StudentLogic();
                if(viewModel.Programme != null)
                {
                    if(viewModel.Programme.Id == 3 || viewModel.Programme.Id == 4)
                    {
                        studentType = new StudentType { Id = 2 };
                        student.Type = studentType;
                    }
                    else
                    {
                        studentType = new StudentType { Id = 1 };
                        student.Type = studentType;
                    }
                }
                student.Number = 100;
                if(person != null)
                {
                    student.Id = person.Id;
                    if(person.Sex != null)
                    {
                        if(person.Sex.Id == 1)
                        {
                            title = new Title { Id = 1 };
                            student.Title = title;
                        }
                        else
                        {
                            title = new Title { Id = 2 };
                            student.Title = title;
                        }
                    }
                }
                student.MaritalStatus = maritalStatus;
                student.SchoolContactAddress = "Abia State University";

                student.Category = studentCategory;
                student.Status = studentStatus;

                long? maxStudentNumber = 0;
                long? nextStudentNumber = 0;

                string matricNumber = student.MatricNumber;

                if (string.IsNullOrEmpty(student.MatricNumber))
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    SessionLogic sessionLogic = new SessionLogic();
                    StudentAssignedMatricNumberLogic assignedMatricNumberLogic = new StudentAssignedMatricNumberLogic();

                    Session session = sessionLogic.GetModelBy(s => s.Session_Id == viewModel.Session.Id);

                    maxStudentNumber = studentLogic.GetEntitiesBy(s => s.Matric_Number != null).Max(s => s.Student_Number);
                    nextStudentNumber = maxStudentNumber == 0 ? 100758 : maxStudentNumber + 1;

                    student.Number = nextStudentNumber;
                    student.MatricNumber = session.Name.Split('/').FirstOrDefault() + "/" + nextStudentNumber + "/REGULAR";

                    Model.Model.Student existingStudent = studentLogic.GetModelsBy(s => s.Matric_Number == student.MatricNumber).LastOrDefault();

                    while (existingStudent != null)
                    {
                        student.MatricNumber = session.Name.Split('/').FirstOrDefault() + "/" + (nextStudentNumber + 1) + "/REGULAR";
                        student.Number = nextStudentNumber + 1;

                        existingStudent = studentLogic.GetModelsBy(s => s.Matric_Number == student.MatricNumber).LastOrDefault();
                    }

                    matricNumber = student.MatricNumber;

                    StudentAssignedMatricNumber assignedMatricNumber = new StudentAssignedMatricNumber();
                    assignedMatricNumber.Person = person;
                    assignedMatricNumber.Programme = viewModel.Programme;
                    assignedMatricNumber.Session = session;
                    assignedMatricNumber.StudentMatricNumber = student.MatricNumber;
                    assignedMatricNumber.StudentNumber = student.Number ?? 0;

                    if (assignedMatricNumberLogic.GetModelBy(s => s.Person_Id == person.Id) == null)
                    {
                        assignedMatricNumberLogic.Create(assignedMatricNumber);
                    }
                }

                student = studentLogic.Create(student);

                student = student ?? new Model.Model.Student() {MatricNumber = matricNumber};

                return student;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private Person CreatePerson(Person person,string gender,string stateValue,string localGovernmentArea)
        {
            try
            {
                Sex sex = null;
                State state = null;
                LocalGovernment localGovernment = null;
                var nationality = new Nationality { Id = 1 };
                var personType = new PersonType { Id = 3 };
                var role = new Role { Id = 5 };
                var localGovernmentLogic = new LocalGovernmentLogic();
                var personLogic = new PersonLogic();
                var stateLogic = new StateLogic();
                if(gender.Trim() == "M")
                {
                    sex = new Sex { Id = 1 };
                    person.Sex = sex;
                }
                else if(gender.Trim() == "F")
                {
                    sex = new Sex { Id = 2 };
                    person.Sex = sex;
                }
                else
                {
                    sex = new Sex { Id = 1 };
                    person.Sex = sex;
                }
                state = stateLogic.GetModelBy(p => p.State_Name == stateValue.Trim());
                if(state != null)
                {
                    person.State = state;
                }
                else
                {
                    state = new State { Id = "AB" };
                    person.State = state;
                }
                localGovernment =
                    localGovernmentLogic.GetModelBy(p => p.Local_Government_Name == localGovernmentArea.Trim());
                if(localGovernment != null)
                {
                    person.LocalGovernment = localGovernment;
                }
                else
                {
                    localGovernment = new LocalGovernment { Id = 1 };
                    person.LocalGovernment = localGovernment;
                }
                person.Nationality = nationality;
                person.DateEntered = DateTime.Now;
                person.Role = role;
                person.Type = personType;
                person = personLogic.Create(person);
                return person;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public JsonResult GetDepartments(string id)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var programme = new Programme { Id = Convert.ToInt32(id) };
                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments,ID,NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static DataSet ReadExcel(string filepath)
        {
            DataSet Result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" +
                                  "Extended Properties=Excel 8.0;";
                //Extended Properties = "Excel 12.0;HDR=YES";
                var connection = new OleDbConnection(xConnStr);

                connection.Open();
                DataTable sheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,null);
                DataRow dataRow = sheet.Rows[0];
                string sheetName = dataRow[2].ToString().Replace("'","");
                var command = new OleDbCommand("Select * FROM [" + sheetName + "]",connection);
                // Create DbDataReader to Data Worksheet

                var MyData = new OleDbDataAdapter();
                MyData.SelectCommand = command;
                var ds = new DataSet();
                ds.Clear();
                MyData.Fill(ds);
                connection.Close();

                Result = ds;
                //}
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        private void populateDropdowns(UploadReturningStudentViewModel viewModel)
        {
            try
            {
                ViewBag.DepartmentId = new SelectList(new List<Department>(),ID,NAME);
                ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
                ViewBag.LevelId = viewModel.LevelSelectListItem;
                ViewBag.SessionId = viewModel.SessionSelectListItem;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void keepDropdownState(UploadReturningStudentViewModel viewModel)
        {
            try
            {
                var programme = new Programme();
                var programmeLogic = new ProgrammeLogic();
                var departmentLogic = new DepartmentLogic();
                var department = new Department();
                var level = new Level();
                var levelLogic = new LevelLogic();
                var session = new Session();
                var sessionLogic = new SessionLogic();
                List<Department> departmentList = departmentLogic.GetBy(viewModel.Programme);
                ViewBag.DepartmentId = new SelectList(departmentList,ID,NAME,viewModel.Department.Id);
                ViewBag.ProgrammeId = new SelectList(programmeLogic.GetAll(),ID,NAME,viewModel.Programme.Id);
                ViewBag.LevelId = new SelectList(levelLogic.GetAll(),ID,NAME,viewModel.Level.Id);
                ViewBag.SessionId = new SelectList(sessionLogic.GetAll(),ID,NAME,viewModel.Session.Id);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}