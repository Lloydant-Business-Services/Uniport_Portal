using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
   [RoleBasedAttribute]
    public class UserController :BaseController
    {
        private readonly UserLogic userLogic;
        private Person person;
        private PersonLogic personLogic;
        private PersonType personType;
        private PersonTypeLogic personTypeLogic;
        private User user;
        private UserViewModel viewModel;

        public UserController()
        {
            viewModel = new UserViewModel();
            user = new User();
            person = new Person();
            personType = new PersonType();
            personTypeLogic = new PersonTypeLogic();
            userLogic = new UserLogic();
            personLogic = new PersonLogic();
        }

        public ActionResult Index()
        {
            try
            {
                var userList = new List<User>();
                userList =
                    userLogic.GetModelsBy(p => p.Role_Id == 8 || p.Role_Id == 10 || p.Role_Id == 12 || p.Role_Id == 14 ||p.Role_Id==17 || p.Role_Id==16 || p.Role_Id==18)
                        .OrderBy(a => a.Username)
                        .ToList();
                viewModel.Users = userList;
            }
            catch(Exception e)
            {
                SetMessage("Error Occurred " + e.Message,Message.Category.Error);
            }
            return View(viewModel);
        }

        public ActionResult CreateUser()
        {
            ViewBag.SexList = viewModel.SexSelectList;
            ViewBag.RoleList = viewModel.RoleSelectList;
            ViewBag.SecurityQuestionList = viewModel.SecurityQuestionSelectList;

            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(UserViewModel viewModel)
        {
            try
            {
                if(viewModel != null)
                {
                    var user = new User();
                    var userLogic = new UserLogic();
                    user = userLogic.GetModelBy(p => p.User_Name == viewModel.User.Username);
                    if(user != null)
                    {
                        SetMessage("Error: Staff with this Name already exist",Message.Category.Error);
                        return RedirectToAction("CreateUser");
                    }

                    var role = new Role { Id = 10 };
                    viewModel.User.Role = role;
                    viewModel.User.LastLoginDate = DateTime.Now;
                    viewModel.User.Activated =true;
                    userLogic.Create(viewModel.User);
                    SetMessage("User Created Succesfully",Message.Category.Information);
                    return RedirectToAction("Index","User");
                }
                SetMessage("Input is null",Message.Category.Error);
                return RedirectToAction("CreateUser");
            }
            catch(Exception e)
            {
                SetMessage("Error Occured " + e.Message,Message.Category.Error);
            }
            return View("CreateUser",viewModel);
        }

        public ActionResult ViewUserDetails(int? id)
        {
            try
            {
                viewModel = null;
                if(id != null)
                {
                    user = userLogic.GetModelBy(p => p.User_Id == id);
                    viewModel = new UserViewModel();
                    viewModel.User = user;
                    return View(viewModel);
                }
                SetMessage("Error Occured: Select a User",Message.Category.Error);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                SetMessage("Error Occured " + ex.Message,Message.Category.Error);
            }
            return View();
        }

        public ActionResult EditUser(int? id)
        {
            viewModel = null;
            try
            {
                if(id != null)
                {
                    TempData["userId"] = id;
                    user = userLogic.GetModelBy(p => p.User_Id == id);
                    viewModel = new UserViewModel();
                    viewModel.User = user;
                }
                else
                {
                    SetMessage("Select a User",Message.Category.Error);
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occured " + ex.Message,Message.Category.Error);
            }

            ViewBag.RoleList = viewModel.RoleSelectList;
            ViewBag.SecurityQuestionList = viewModel.SecurityQuestionSelectList;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditUser(UserViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    //Role role = new Role() { Id = 8 };
                    //viewModel.User.Role = role;
                    
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase hpf = Request.Files[file];
                        if (hpf.ContentLength == 0)
                        {
                            continue;
                        }
                        FileInfo fileInfo = new FileInfo(hpf.FileName);
                        string pathForSaving = Server.MapPath("~/Content/User/SignatureImage");
                        string fileName = (int)TempData["userId"] + "__" + fileInfo.Name;

                        if (CreateFolderIfNeeded(pathForSaving))
                        {
                            DeleteFileIfExist(pathForSaving, fileName);
                            string savedFileName = Path.Combine(pathForSaving, fileName);
                            hpf.SaveAs(savedFileName);
                            viewModel.User.SignatureUrl = "/Content/User/SignatureImage/" + fileName;
                        }
                    }
                    viewModel.User.Id = (int)TempData["userId"];
                    userLogic.Update(viewModel.User);
                    SetMessage("User Edited Successfully", Message.Category.Information);
                }
                else
                {
                    SetMessage("Input is null", Message.Category.Warning);
                    return RedirectToAction("EditUser");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }

        public ActionResult EditUserRole()
        {
            try
            {
                viewModel = new UserViewModel();
                ViewBag.Role = viewModel.RoleSelectList;
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditUserRole(UserViewModel viewModel)
        {
            try
            {
                if(viewModel.User.Role != null)
                {
                    var userLogic = new UserLogic();
                    viewModel.Users = userLogic.GetModelsBy(u => u.Role_Id == viewModel.User.Role.Id);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            ViewBag.Role = viewModel.RoleSelectList;
            return View(viewModel);
        }

        public ActionResult EditRole(string id)
        {
            try
            {
                int userId = Convert.ToInt32(id);
                var userLogic = new UserLogic();

                viewModel = new UserViewModel();
                viewModel.User = userLogic.GetModelBy(u => u.User_Id == userId);

                ViewBag.Role = new SelectList(viewModel.RoleSelectList,"Value","Text",viewModel.User.Role.Id);
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditRole(UserViewModel viewModel)
        {
            try
            {
                if(viewModel.User != null)
                {
                    var userLogic = new UserLogic();
                    bool isUserModified = userLogic.Modify(viewModel.User);

                    if(isUserModified)
                    {
                        SetMessage("Operation Successful!",Message.Category.Information);
                        return RedirectToAction("EditRoleByUserName");
                    }
                    SetMessage("No item was Modified",Message.Category.Information);
                    return RedirectToAction("EditRoleByUserName");
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return RedirectToAction("EditRoleByUserName");
        }

        public ActionResult EditRoleByUserName()
        {
            try
            {
                viewModel = new UserViewModel();
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditRoleByUserName(UserViewModel viewModel)
        {
            try
            {
                if(viewModel.User.Username != null)
                {
                    var userLogic = new UserLogic();
                    User user = userLogic.GetModelBy(u => u.User_Name == viewModel.User.Username);
                    viewModel.User = user;
                    if(user == null)
                    {
                        SetMessage("User does not exist!",Message.Category.Error);
                        return View(viewModel);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            ViewBag.Role = viewModel.RoleSelectList;
            return View(viewModel);
        }

        public ActionResult EditFacultyUser()
        {
            try
            {
                viewModel = new UserViewModel();
                ViewBag.Faculty = viewModel.FacultySelectList;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditFacultyUser(UserViewModel viewModel)
        {
            try
            {
                if (viewModel != null && viewModel.Faculty != null && viewModel.User.Username != null)
                {
                    UserLogic userLogic = new UserLogic();
                    var getUser = userLogic.GetModelBy(s => s.User_Name == viewModel.User.Username);
                    if(getUser != null && getUser.Role.Id == (int)Roles.FacultyOfficer)
                    {
                        FacultyOfficerLogic facultyOfficerLogic = new FacultyOfficerLogic();
                        var checkIfAssigned = facultyOfficerLogic.GetModelBy(s => s.User_Id == getUser.Id);
                        if (checkIfAssigned == null)
                        {

                            FacultyOfficer facultyOfficer = new FacultyOfficer();
                            facultyOfficer.Description = "New Faculty";
                            facultyOfficer.Faculty = viewModel.Faculty;
                            facultyOfficer.Officer = getUser;
                            facultyOfficerLogic.Create(facultyOfficer);
                            SetMessage("User tied to this faculty successfully ", Message.Category.Information);
                        }
                        if(checkIfAssigned != null && checkIfAssigned.Faculty.Id == viewModel.Faculty.Id)
                        {
                            SetMessage("User is already tied to this faculty ", Message.Category.Warning);
                        }
                        if (checkIfAssigned != null && checkIfAssigned.Faculty.Id != viewModel.Faculty.Id)
                        {
                            checkIfAssigned.Faculty.Id = viewModel.Faculty.Id;
                            facultyOfficerLogic.Modify(checkIfAssigned);
                            SetMessage("User faculty modified successfully", Message.Category.Information);
                        }
                    }
                    else
                    {
                        SetMessage("Error! User does not exit or user is not a faculty officer, please check ", Message.Category.Information);
                    }
                }

            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            ViewBag.Faculty = viewModel.FacultySelectList;
            return View();
        }

        public ActionResult ChangeCourseStaffRole()
        {
            try
            {
                viewModel = new UserViewModel();
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        
        [HttpPost]
        public ActionResult ChangeCourseStaffRole(UserViewModel viewModel)
        {
            try
            {
                if(viewModel.User.Username != null)
                {
                    var userLogic = new UserLogic();
                    var courseAllocationLogic = new CourseAllocationLogic();
                    var courseAllocation = new CourseAllocation();

                    User user = userLogic.GetModelBy(u => u.User_Name == viewModel.User.Username);
                    if(user == null)
                    {
                        SetMessage("User Does Not Exist",Message.Category.Error);
                        return View(viewModel);
                    }

                    courseAllocation = courseAllocationLogic.GetModelsBy(ca => ca.User_Id == user.Id).FirstOrDefault();
                    if(courseAllocation == null)
                    {
                        SetMessage("Staff has not been allocated any course",Message.Category.Error);
                        return View(viewModel);
                    }
                    viewModel.CourseAllocation = courseAllocation;
                    viewModel.User = user;
                    RetainDropDownState(viewModel);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult SaveCourseStaffRole(UserViewModel viewModel)
        {
            try
            {
                var userLogic = new UserLogic();
                var courseAllocationLogic = new CourseAllocationLogic();
                var user = new User();
                var courseAllocations = new List<CourseAllocation>();

                user = userLogic.GetModelBy(u => u.User_Id == viewModel.User.Id);
                courseAllocations = courseAllocationLogic.GetModelsBy(ca => ca.User_Id == user.Id);
                if(courseAllocations.FirstOrDefault().Department.Id != viewModel.CourseAllocation.Department.Id ||
                    courseAllocations.FirstOrDefault().Programme.Id != viewModel.CourseAllocation.Programme.Id ||
                    courseAllocations.FirstOrDefault().Level.Id != viewModel.CourseAllocation.Level.Id ||
                    courseAllocations.FirstOrDefault().Session.Id != viewModel.CourseAllocation.Session.Id ||
                    courseAllocations.FirstOrDefault().Semester.Id != viewModel.CourseAllocation.Semester.Id)
                {
                    SetMessage(
                        "The User has not been allocated any course in this Programme, Department, Level, Session and Semester",
                        Message.Category.Error);
                    RetainDropDownState(viewModel);
                    return View("ChangeCourseStaffRole");
                }
                using(var scope = new TransactionScope())
                {
                    user.Role = viewModel.User.Role;
                    userLogic.Modify(user);

                    for(int i = 0;i < courseAllocations.Count;i++)
                    {
                        if(user.Role.Id != 12)
                        {
                            courseAllocations[i].HodDepartment = null;
                            courseAllocations[i].IsHOD = null;
                            courseAllocationLogic.Modify(courseAllocations[i]);
                        }
                        else
                        {
                            courseAllocations[i].HodDepartment = viewModel.CourseAllocation.Department;
                            courseAllocations[i].IsHOD = true;
                            courseAllocationLogic.Modify(courseAllocations[i]);
                        }
                    }

                    scope.Complete();
                    SetMessage("Operation Successful!",Message.Category.Information);
                }
            }
            catch(Exception)
            {
                throw;
            }

            return RedirectToAction("ChangeCourseStaffRole");
        }

        public void RetainDropDownState(UserViewModel viewModel)
        {
            try
            {
                if(viewModel.CourseAllocation != null)
                {
                    if(viewModel.CourseAllocation.Programme != null)
                    {
                        ViewBag.Programme = new SelectList(viewModel.ProgrammeSelectList,"Value","Text",
                            viewModel.CourseAllocation.Programme.Id);
                    }
                    else
                    {
                        ViewBag.Programme = viewModel.ProgrammeSelectList;
                    }

                    if(viewModel.CourseAllocation.Department != null && viewModel.CourseAllocation.Programme != null)
                    {
                        var departmentLogic = new DepartmentLogic();
                        ViewBag.Department = new SelectList(
                            departmentLogic.GetBy(viewModel.CourseAllocation.Programme),"Id","Name",
                            viewModel.CourseAllocation.Department.Id);
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(new List<Department>(),"Id","Name");
                    }

                    if(viewModel.CourseAllocation.Semester != null && viewModel.CourseAllocation.Session != null)
                    {
                        var sessionSemesterLogic = new SessionSemesterLogic();
                        List<SessionSemester> sessionSemesterList =
                            sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.CourseAllocation.Session.Id);

                        var semesters = new List<Semester>();
                        foreach(SessionSemester sessionSemester in sessionSemesterList)
                        {
                            semesters.Add(sessionSemester.Semester);
                        }

                        ViewBag.Semester = new SelectList(semesters,"Id","Name",viewModel.CourseAllocation.Session.Id);
                    }
                    else
                    {
                        ViewBag.Semester = new SelectList(new List<Semester>(),"Id","Name");
                    }

                    if(viewModel.CourseAllocation.Session != null)
                    {
                        ViewBag.Session = new SelectList(viewModel.SessionSelectList,"Value","Text",
                            viewModel.CourseAllocation.Session.Id);
                    }
                    else
                    {
                        ViewBag.Session = viewModel.SessionSelectList;
                    }

                    if(viewModel.CourseAllocation.Level != null)
                    {
                        ViewBag.Level = new SelectList(viewModel.LevelSelectList,"Value","Text",
                            viewModel.CourseAllocation.Level.Id);
                    }
                    else
                    {
                        ViewBag.Level = viewModel.LevelSelectList;
                    }
                }
                if(viewModel.User != null)
                {
                    if(viewModel.User.Role != null)
                    {
                        ViewBag.Role = new SelectList(viewModel.RoleSelectList,"Value","Text",viewModel.User.Role.Id);
                    }
                    else
                    {
                        ViewBag.Role = viewModel.RoleSelectList;
                    }
                }
                if(viewModel.Staff != null)
                {
                    if (viewModel.Staff.Department != null)
                    {
                        var departmentLogic = new DepartmentLogic();
                        ViewBag.Department = new SelectList(
                            departmentLogic.GetBy(new Programme() { Id = 1 }), "Id", "Name",
                            viewModel.Staff.Department.Id);
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");
                    }
                }
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

                return Json(new SelectList(departments,"Id","Name"),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetSemester(string id)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var session = new Session { Id = Convert.ToInt32(id) };
                var semesterLogic = new SemesterLogic();
                var sessionSemesterList = new List<SessionSemester>();
                var sessionSemesterLogic = new SessionSemesterLogic();
                sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id);

                var semesters = new List<Semester>();
                foreach(SessionSemester sessionSemester in sessionSemesterList)
                {
                    semesters.Add(sessionSemester.Semester);
                }

                return Json(new SelectList(semesters,"Id","Name"),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void DeleteFileIfExist(string folderPath, string fileName)
        {
            try
            {
                string wildCard = fileName + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath, wildCard, SearchOption.TopDirectoryOnly);

                if (files != null && files.Count() > 0)
                {
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ActionResult AssignHOD()
        {
            try
            {
                viewModel = new UserViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }


        [HttpPost]
        public ActionResult AssignHOD(UserViewModel viewModel)
        {
            try
            {
                if (viewModel.User.Username != null)
                {
                    var userLogic = new UserLogic();
                    var staffLogic = new StaffLogic();

                    User user = userLogic.GetModelBy(u => u.User_Name == viewModel.User.Username);
                    if (user == null)
                    {
                        SetMessage("User Does Not Exist", Message.Category.Error);
                        return View(viewModel);
                    }

                    viewModel.User = user;
                    Staff staff = staffLogic.GetModelBy(s => s.User_Id == user.Id);
                    if (staff != null)
                    {
                        viewModel.Staff = staff;
                    }
                    else
                    {
                        viewModel.Staff = new Staff();
                        viewModel.Staff.Department = new Department() { Id = 1};
                        viewModel.Staff.isHead = true;
                        viewModel.Staff.isManagement = false;
                        viewModel.Staff.User = user;
                    }
                   
                    
                    RetainDropDownState(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult SaveAssignHOD(UserViewModel viewModel)
        {
            try
            {
                var userLogic = new UserLogic();
                var staffLogic = new StaffLogic();
                var user = new User();

                user = userLogic.GetModelBy(u => u.User_Id == viewModel.User.Id);
                Staff staff = staffLogic.GetModelBy(s => s.User_Id == user.Id);
               
                using (var scope = new TransactionScope())
                {
                    user.Role = viewModel.User.Role;
                    userLogic.Modify(user);
                    if (staff != null)
                    {
                        staff.Department = viewModel.Staff.Department;
                        staff.isHead = viewModel.Staff.isHead;
                        staff.isManagement = viewModel.Staff.isManagement;
                        staffLogic.Modify(staff);
                    }
                    else
                    {
                        staff = new Staff();
                        staff.User = user;
                        staff.Type = new StaffType() { Id = 1 };
                        staff.Department = viewModel.Staff.Department;
                        staff.isHead = viewModel.Staff.isHead;
                        staff.isManagement = viewModel.Staff.isManagement;
                        staffLogic.Create(staff);
                    }

                    scope.Complete();
                    SetMessage("Operation Successful!", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            return RedirectToAction("AssignHOD");
        }

    }
}