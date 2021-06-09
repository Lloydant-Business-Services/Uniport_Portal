using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using ClosedXML.Excel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class StaffCourseAllocationController :BaseController
    {
        public enum SpeicalCaseCodes
        {
            Sick = 101,
            Absent = 201,
            Other = 301
        }

        private const string ID = "Id";
        private const string NAME = "Name";
        private string FileUploadURL;
        private StaffCourseAllocationViewModel viewModel;

        public ActionResult AllocatedCourses()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occured" + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AllocatedCourses(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if(viewModel != null)
                {
                    var courseAllocationLogic = new CourseAllocationLogic();
                    viewModel.CourseAllocationList =
                        courseAllocationLogic.GetModelsBy(
                            p =>
                                p.Level_Id == viewModel.CourseAllocation.Level.Id &&
                                p.Programme_Id == viewModel.CourseAllocation.Programme.Id &&
                                p.Session_Id == viewModel.CourseAllocation.Session.Id);
                }

                KeepCourseDropDownState(viewModel);
            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }

            return View("AllocatedCourses",viewModel);
        }

        public ActionResult AllocateCourse()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.User = viewModel.UserSelectList;
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.Department = new SelectList(new List<Department>(),ID,NAME);
                ViewBag.Course = new SelectList(new List<Course>(),ID,NAME);
                ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), ID, NAME);
            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult AllocateCourse(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if(viewModel != null)
                {
                    var courseAllocationLogic = new CourseAllocationLogic();
                    CourseAllocation courseAllocationList = new CourseAllocation();
                    if (viewModel.CourseAllocation.DepartmentOption?.Id > 0)
                    {
                        courseAllocationList=courseAllocationLogic.GetModelsBy(
                            p =>
                                p.Level_Id == viewModel.CourseAllocation.Level.Id &&
                                p.Programme_Id == viewModel.CourseAllocation.Programme.Id &&
                                p.Session_Id == viewModel.CourseAllocation.Session.Id &&
                                 p.Semester_Id == viewModel.CourseAllocation.Semester.Id &&
                                p.Course_Id == viewModel.CourseAllocation.Course.Id &&
                                p.Department_Id == viewModel.CourseAllocation.Department.Id && p.DepartmentOption_Id==viewModel.CourseAllocation.DepartmentOption.Id).FirstOrDefault();
                    }
                    else
                    {
                        courseAllocationList=courseAllocationLogic.GetModelsBy(
                            p =>
                                p.Level_Id == viewModel.CourseAllocation.Level.Id &&
                                p.Programme_Id == viewModel.CourseAllocation.Programme.Id &&
                                p.Session_Id == viewModel.CourseAllocation.Session.Id &&
                                 p.Semester_Id == viewModel.CourseAllocation.Semester.Id &&
                                p.Course_Id == viewModel.CourseAllocation.Course.Id &&
                                p.Department_Id == viewModel.CourseAllocation.Department.Id).FirstOrDefault();
                    }
                        

                    if(courseAllocationList == null && viewModel.CourseAllocation.Course != null &&
                        viewModel.CourseAllocation.Department != null && viewModel.CourseAllocation.Level != null &&
                        viewModel.CourseAllocation.Programme != null && viewModel.CourseAllocation.Session != null &&
                        viewModel.CourseAllocation.User != null)
                    {
                        courseAllocationLogic.Create(viewModel.CourseAllocation);
                        SetMessage("Course Allocated",Message.Category.Information);
                        KeepCourseDropDownState(viewModel);
                        return View(viewModel);
                    }
                    courseAllocationList.User.Id = viewModel.CourseAllocation.User.Id;
                    courseAllocationLogic.Modify(courseAllocationList);
                    SetMessage("Course allocation updated",Message.Category.Error);
                    KeepCourseDropDownState(viewModel);
                    return View("AllocateCourse");
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }
            return View();
        }

        public ActionResult AllocateCourseByDepartment()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.User = viewModel.UserSelectList;
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.Department = new SelectList(new List<Department>(),ID,NAME);
                ViewBag.Course = new SelectList(new List<Course>(),ID,NAME);
                var loggeduser = new UserLogic();
                var user = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                if (user != null && user.Id > 0)
                {
                   StaffLogic staffLogic = new StaffLogic();
                   Staff staff = staffLogic.GetBy(user.Id );
                    if (staff != null && staff.Id > 0)
                    {

                        viewModel.CourseAllocation = new CourseAllocation();
                        viewModel.CourseAllocation.Department = staff.Department;
                        if ((bool)staff.isManagement)
                        {
                            ViewBag.Department = Utility.PopulateDepartmentSelectListItemByFacultyId(staff.Department.Faculty.Id);
                        }
                        else
                        {
                            ViewBag.Department = Utility.PopulateDepartmentSelectListItem();
                        }


                    }

                }


            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult AllocateCourseByDepartment(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if(viewModel != null)
                {
                    var courseAllocationLogic = new CourseAllocationLogic();
                    List<CourseAllocation> courseAllocationList =
                        courseAllocationLogic.GetModelsBy(
                            p =>
                                p.Level_Id == viewModel.CourseAllocation.Level.Id &&
                                p.Programme_Id == viewModel.CourseAllocation.Programme.Id &&
                                p.Session_Id == viewModel.CourseAllocation.Session.Id &&
                                 p.Semester_Id == viewModel.CourseAllocation.Semester.Id && 
                                p.Department_Id == viewModel.CourseAllocation.Department.Id);

                    if(courseAllocationList.Count <= 0 && viewModel.CourseAllocation.Department != null &&
                        viewModel.CourseAllocation.Level != null && viewModel.CourseAllocation.Programme != null &&
                        viewModel.CourseAllocation.Session != null)
                    {
                        var courseLogic = new CourseLogic();
                        viewModel.Courses = courseLogic.GetBy(viewModel.CourseAllocation.Department,
                            viewModel.CourseAllocation.Level,viewModel.CourseAllocation.Semester,
                            viewModel.CourseAllocation.Programme);
                        //SetMessage("Course Allocated", Message.Category.Information);
                        if(viewModel.Courses != null && viewModel.Courses.Count > 0)
                        {
                            var courseAllocations = new List<CourseAllocation>();
                            foreach(Course course in viewModel.Courses)
                            {
                                var courseAllocation = new CourseAllocation();
                                courseAllocation.Programme = course.Programme;
                                courseAllocation.Department = course.Department;
                                courseAllocation.Level = course.Level;
                                courseAllocation.Semester = course.Semester;
                                courseAllocation.Course = course;
                                courseAllocation.Session = viewModel.CourseAllocation.Session;
                                courseAllocations.Add(courseAllocation);
                            }
                            viewModel.CourseAllocationList = courseAllocations;
                        }

                        KeepCourseDropDownState(viewModel);
                        return View(viewModel);
                    }
                    viewModel.CourseAllocationList = courseAllocationList;
                    KeepCourseDropDownState(viewModel);
                    return View(viewModel);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }
            return View("AllocateCourseByDepartment");
        }

        [HttpPost]
        public ActionResult SavedAllocateCourseByDepartment(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if(viewModel != null)
                {
                    var courseAllocationLogic = new CourseAllocationLogic();
                    using(var scope = new TransactionScope())
                    {
                        foreach(CourseAllocation courseAllocation in viewModel.CourseAllocationList)
                        {
                            courseAllocationLogic.Create(courseAllocation);
                        }
                        scope.Complete();
                        SetMessage("Course Allocated",Message.Category.Information);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }
            return RedirectToAction("AllocateCourseByDepartment");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Autocomplete(string term)
        {
            if(string.IsNullOrEmpty(term))
            {
                return null;
            }
            var userLogic = new UserLogic();
            List<User> users = userLogic.GetModelsBy(u => u.User_Name.Contains(term));
            return Json(new SelectList(users,ID,NAME),JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadResultSheet()
        {
            try
            {
                var viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(),ID,NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.Course = new SelectList(new List<Course>(),ID,NAME);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message,Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult DownloadResultSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if(viewModel != null)
                {
                    var courseAllocation = new CourseAllocation();
                    var courseAllocationLogic = new CourseAllocationLogic();
                    courseAllocation =
                        courseAllocationLogic.GetModelBy(
                            p =>
                                p.Course_Id == viewModel.Course.Id && p.Department_Id == viewModel.Department.Id &&
                                p.Level_Id == viewModel.Level.Id && p.Programme_Id == viewModel.Programme.Id &&
                                p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id &&
                                p.USER.User_Name == User.Identity.Name);
                    if(courseAllocation == null)
                    {
                        if(!User.IsInRole("Admin"))
                        {
                            SetMessage(
                                "You are not allocated to this course, with this Programme-Department combination",
                                Message.Category.Error);
                            KeepDropDownState(viewModel);
                            return RedirectToAction("DownloadResultSheet");
                        }
                    }
                    var gv = new GridView();

                    var ds = new DataTable();
                    var resultFormatList = new List<ResultFormat>();

                    var sessionLogic = new SessionLogic();
                    Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                    var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    var courseRegistrationDetailList = new List<CourseRegistrationDetail>();
                    var courseRegistrationList = new List<CourseRegistration>();
                    var courseRegistrationLogic = new CourseRegistrationLogic();
                    courseRegistrationList =
                        courseRegistrationLogic.GetModelsBy(
                            p =>
                                p.Session_Id == session.Id && p.Level_Id == viewModel.Level.Id &&
                                p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id);
                    if(viewModel.Course != null && viewModel.Semester != null && courseRegistrationList.Count > 0)
                    {
                        int count = 1;
                        foreach(CourseRegistration courseRegistration in courseRegistrationList)
                        {
                            courseRegistrationDetailList =
                                courseRegistrationDetailLogic.GetModelsBy(
                                    p =>
                                        p.Course_Id == viewModel.Course.Id && p.Semester_Id == viewModel.Semester.Id &&
                                        p.Student_Course_Registration_Id == courseRegistration.Id);
                            if(courseRegistrationDetailList.Count > 0)
                            {
                                foreach(
                                    CourseRegistrationDetail courseRegistrationDetailItem in
                                        courseRegistrationDetailList)
                                {
                                    var resultFormat = new ResultFormat();
                                    // resultFormat.SN = count;
                                    resultFormat.MatricNo =
                                        courseRegistrationDetailItem.CourseRegistration.Student.MatricNumber;
                                    resultFormatList.Add(resultFormat);
                                    count++;
                                }
                            }
                        }
                    }

                    if(resultFormatList.Count > 0)
                    {
                        List<ResultFormat> list = resultFormatList.OrderBy(p => p.MatricNo).ToList();
                        var sort = new List<ResultFormat>();
                        for(int i = 0;i < list.Count;i++)
                        {
                            list[i].SN = (i + 1);
                            sort.Add(list[i]);
                        }

                        gv.DataSource = sort; // resultFormatList.OrderBy(p => p.MATRICNO);
                        var courseLogic = new CourseLogic();
                        Course course = courseLogic.GetModelBy(p => p.Course_Id == viewModel.Course.Id);
                        gv.Caption = course.Name.ToUpper() + " " + course.Code + " " + " DEPARTMENT OF " + " " +
                                     course.Department.Name.ToUpper() + " " + course.Unit + " " + "Units";
                        gv.DataBind();

                        string filename = course.Code.Replace("/","").Replace("\\","") + course.Department.Code +
                                          ".xls";
                        //ExportToExcel(sort);
                        return new DownloadFileActionResult(gv,filename);
                    }
                    Response.Write("No data available for download");
                    Response.End();
                    return new JavaScriptResult();
                }
                SetMessage("Input is null",Message.Category.Error);

                KeepDropDownState(viewModel);

                return RedirectToAction("DownloadResultSheet");
            }
            catch(Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message,Message.Category.Error);
            }

            return RedirectToAction("DownloadResultSheet");
        }

        public ActionResult UploadResultSheet()
        {
            try
            {
                var viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(),ID,NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.Course = new SelectList(new List<Course>(),ID,NAME);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message,Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult UploadResultSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var courseAllocation = new CourseAllocation();
                var courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation =
                    courseAllocationLogic.GetModelBy(
                        p =>
                            p.Course_Id == viewModel.Course.Id && p.Department_Id == viewModel.Department.Id &&
                            p.Level_Id == viewModel.Level.Id && p.Programme_Id == viewModel.Programme.Id &&
                            p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id &&
                            p.USER.User_Name == User.Identity.Name);
                //if (courseAllocation == null)
                //{
                //    if (!User.IsInRole("Admin"))
                //    {
                //        SetMessage("You are not allocated to this course, with this Programme-Department combination", Message.Category.Error);
                //        RetainDropdownState(viewModel);
                //        return View(viewModel);
                //    }
                //}
                var resultFormatList = new List<ResultFormat>();
                foreach(string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving,hpf.FileName);
                    hpf.SaveAs(savedFileName);
                    DataSet studentSet = ReadExcel(savedFileName);

                    if(studentSet != null && studentSet.Tables[0].Rows.Count > 0)
                    {
                        for(int i = 1;i < studentSet.Tables[0].Rows.Count;i++)
                        {
                            var resultFormat = new ResultFormat();
                            resultFormat.SN = Convert.ToInt32(studentSet.Tables[0].Rows[i][0].ToString().Trim());
                            resultFormat.MatricNo = studentSet.Tables[0].Rows[i][1].ToString().Trim();
                            resultFormat.CA = Convert.ToDecimal(studentSet.Tables[0].Rows[i][4].ToString().Trim());
                            resultFormat.Exam = Convert.ToDecimal(studentSet.Tables[0].Rows[i][5].ToString().Trim());

                            if(resultFormat.MatricNo != "")
                            {
                                resultFormatList.Add(resultFormat);
                            }
                        }
                        resultFormatList.OrderBy(p => p.MatricNo);
                        viewModel.resultFormatList = resultFormatList;
                        TempData["staffCourseAllocationViewModel"] = viewModel;
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error occured! " + ex.Message,Message.Category.Error);
            }
            RetainDropdownState(viewModel);
            return View(viewModel);
        }

        public ActionResult SaveUploadedResultSheet()
        {
            var viewModel = (StaffCourseAllocationViewModel)TempData["staffCourseAllocationViewModel"];
            try
            {
                if(viewModel != null)
                {
                    int status = validateFields(viewModel.resultFormatList);
                    if(status > 0)
                    {
                        ResultFormat format = viewModel.resultFormatList.ElementAt((status - 1));
                        SetMessage("Validation Error for" + " " + format.MatricNo,Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return RedirectToAction("StaffResultSheet");
                    }

                    bool resultAdditionStatus = addStudentResult(viewModel);
                    SetMessage("Upload successful",Message.Category.Information);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error occured " + ex.Message,Message.Category.Error);
            }

            RetainDropdownState(viewModel);
            return RedirectToAction("StaffResultSheet");
        }

        public ActionResult DownloadAttendanceSheet()
        {
            try
            {
                var viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(),ID,NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.Course = new SelectList(new List<Course>(),ID,NAME);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message,Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult AttendanceReport(StaffCourseAllocationViewModel viewModel)
        {
            var courseAllocation = new CourseAllocation();
            var courseAllocationLogic = new CourseAllocationLogic();
            // courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Id == viewModel.Course.Id && p.Department_Id == viewModel.Department.Id && p.Level_Id == viewModel.Level.Id && p.Programme_Id == viewModel.Programme.Id && p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
            //if (courseAllocation == null)
            //{
            //if (!User.IsInRole("Admin"))
            //{
            //    SetMessage("You are not allocated to this course, with this Programme-Department combination", Message.Category.Error);
            //    KeepDropDownState(viewModel);
            //    return RedirectToAction("DownloadResultSheet");
            //}
            //}

            ViewBag.SessionId = viewModel.Session.Id.ToString();
            ViewBag.SemesterId = viewModel.Semester.Id.ToString();
            ViewBag.ProgrammeId = viewModel.Programme.Id.ToString();
            ViewBag.DepartmentId = viewModel.Department.Id.ToString();
            ViewBag.LevelId = viewModel.Level.Id.ToString();
            ViewBag.CourseId = viewModel.Course.Id.ToString();

            return View();
        }

        public ActionResult StaffResultSheet()
        {
            var viewModel = new StaffCourseAllocationViewModel();
            ViewBag.Session = viewModel.SessionSelectList;
            ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
            ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult StaffResultSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(viewModel);
                TempData["vModel"] = viewModel;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return View(viewModel);
        }

        public ActionResult ResultUploadSheet(string cid)
        {
            try
            {
                long courseId = Convert.ToInt64(Utility.Decrypt(cid));
                var courseAllocation = new CourseAllocation();
                var courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == courseId);
                if(courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination",
                        Message.Category.Error);
                    return RedirectToAction("StaffResultSheet");
                }
                var gv = new GridView();
                var ds = new DataTable();
                var resultFormatList = new List<ResultFormat>();
                var courseRegistrationLogic = new CourseRegistrationLogic();
                resultFormatList = courseRegistrationLogic.GetDownloadResultFormats(courseAllocation);
                if(resultFormatList.Count > 0)
                {
                    List<ResultFormat> list = resultFormatList.OrderBy(p => p.MatricNo).ToList();
                    var sort = new List<ResultFormat>();
                    for(int i = 0;i < list.Count;i++)
                    {
                        list[i].SN = (i + 1);
                        sort.Add(list[i]);
                    }

                    gv.DataSource = sort; // resultFormatList.OrderBy(p => p.MATRICNO);
                    var courseLogic = new CourseLogic();
                    Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                    // gv.Caption = course.Name + " "+ course.Code + " " + " DEPARTMENT OF " + " " + course.Department.Name.ToUpper() + " " + course.Unit + " " + "Units";
                    gv.HeaderStyle.BackColor = Color.DarkBlue;
                    gv.HeaderStyle.ForeColor = Color.White;
                    gv.DataBind();
                    if (course.DepartmentOption?.Id > 0)
                    {
                        course.Department.Code = course.Department.Code + "_" + course.DepartmentOption.Name;
                    }
                    string filename = course.Code.Replace("/","").Replace("\\","") + course.Department.Code + ".xls";
                    //ExportToExcel(sort);
                    return new DownloadFileActionResult(gv,filename);
                 
                }
                Response.Write("No data available for download");
                Response.End();
                return new JavaScriptResult();
            }
            catch(Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message,Message.Category.Error);
            }
            return RedirectToAction("StaffResultSheet");
        }

        [HttpPost]
        public ActionResult ResultUploadSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var courseAllocation = new CourseAllocation();
                var courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == viewModel.cid);
                var courseLogic = new CourseLogic();
                Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                if(courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination",
                        Message.Category.Error);
                    return RedirectToAction("StaffResultSheet");
                }
                var resultFormatList = new List<ResultFormat>();
                foreach(string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving,hpf.FileName);
                    FileUploadURL = savedFileName;
                    hpf.SaveAs(savedFileName);
                    DataSet studentSet = ReadExcel(savedFileName);

                    if(studentSet != null && studentSet.Tables[0].Rows.Count > 0)
                    {
                        for(int i = 0;i < studentSet.Tables[0].Rows.Count;i++)
                        {
                            var resultFormat = new ResultFormat();
                            resultFormat.SN = Convert.ToInt32(studentSet.Tables[0].Rows[i][0].ToString().Trim());
                            resultFormat.MatricNo = studentSet.Tables[0].Rows[i][1].ToString().Trim();
                            resultFormat.Fullname = studentSet.Tables[0].Rows[i][2].ToString().Trim();
                            resultFormat.Department = studentSet.Tables[0].Rows[i][3].ToString().Trim();
                            if(studentSet.Tables[0].Rows[i][4].ToString() != "")
                            {
                                resultFormat.CA = Convert.ToDecimal(studentSet.Tables[0].Rows[i][4].ToString().Trim());
                            }
                            else
                            {
                                resultFormat.CA = 0;
                            }
                            if(studentSet.Tables[0].Rows[i][4].ToString() != "")
                            {
                                resultFormat.Exam = Convert.ToDecimal(studentSet.Tables[0].Rows[i][5].ToString().Trim());
                            }
                            else
                            {
                                resultFormat.Exam = 0;
                            }

                            resultFormat.CourseCode = studentSet.Tables[0].Rows[i][6].ToString().Trim();
                            if(resultFormat.MatricNo != "")
                            {
                                resultFormatList.Add(resultFormat);
                            }
                        }
                        resultFormatList.OrderBy(p => p.MatricNo);
                        viewModel.resultFormatList = resultFormatList;
                        viewModel.Course = course;
                        var vModel = (StaffCourseAllocationViewModel)TempData["vModel"];
                        vModel.Course = course;
                        vModel.resultFormatList = resultFormatList;
                        vModel.CourseAllocation = courseAllocation;
                        TempData["staffCourseAllocationViewModel"] = vModel;
                    }
                }

            }
            catch(Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message,Message.Category.Error);
            }
            RetainDropdownState(viewModel);
            return View(viewModel);
        }

        public ActionResult DownloadZip(string downloadName)
        {
            TempData["downloadName"] = downloadName + ".zip";
            return View();
        }

        public ActionResult GetZip()
        {
            try
            {
                var downloadName = (string)TempData["downloadName"];
                TempData.Keep("downloadName");
                string path = "~/Content/temp/" + downloadName;
                Response.Redirect(path,false);
            }
            catch(Exception ex)
            {
                SetMessage("Error, " + ex.Message,Message.Category.Error);
            }

            return View("DownloadZip");
        }

        private bool addStudentResult(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                viewModel = (StaffCourseAllocationViewModel)TempData["staffCourseAllocationViewModel"];
                var sessionSemesterLogicc = new SessionSemesterLogic();
                var sessionSemester = new SessionSemester();
                var userLogic = new UserLogic();
                var studentLogic = new StudentLogic();
                var testResultType = new StudentResultType { Id = 1 };
                var examResultType = new StudentResultType { Id = 2 };
                User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                sessionSemester =
                    sessionSemesterLogicc.GetModelBy(
                        p => p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id);
                if(viewModel != null && viewModel.resultFormatList.Count > 0)
                {
                    var courseRegistration = new CourseRegistration();
                    var courseRegistrationLogic = new CourseRegistrationLogic();
                    var courseRegistrationDetail = new CourseRegistrationDetail();
                    var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    StudentResultDetail studentResultDetailTest;
                    StudentResultDetail studentResultDetailExam;
                    List<StudentResultDetail> studentResultDetailTestList;
                    List<StudentResultDetail> studentResultDetailExamList;
                    StudentResultLogic studentResultLogic;
                    StudentResult studentResultTest;
                    StudentResult studentResultExam;
                    var studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                    var StudentExamRawScoreSheetLogic = new StudentExamRawScoreSheetResultLogic();

                    var studentResultDetailLogic = new StudentResultDetailLogic();
                    UserLogic loggeduser = new UserLogic();
                    var logedInuser = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,new TimeSpan(0,15,0)))
                    {
                        foreach(ResultFormat resultFormat in viewModel.resultFormatList)
                        {
                            InitializeStudentResult(viewModel,"ADMIN",sessionSemester,testResultType,examResultType,
                                user,out studentResultDetailTest,out studentResultDetailExam,
                                out studentResultDetailTestList,out studentResultDetailExamList,out studentResultLogic,
                                out studentResultTest,out studentResultExam);
                            studentResultDetailTest.Course = viewModel.Course;

                            string operation = "UPLOAD RESULT";
                            string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (StaffCourseAllocationController)";
                            string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                            var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                            courseRegistrationDetailAudit.Action = action;
                            courseRegistrationDetailAudit.Operation = operation;
                            courseRegistrationDetailAudit.Client = client;
                            courseRegistrationDetailAudit.Time = DateTime.Now;
                            courseRegistrationDetailAudit.User = logedInuser;



                            courseRegistration =
                                courseRegistrationLogic.GetModelsBy(
                                    c =>
                                        c.STUDENT.Matric_Number == resultFormat.MatricNo.Trim() &&
                                        c.Session_Id == viewModel.Session.Id).LastOrDefault();
                            if(courseRegistration != null)
                            {
                                studentResultDetailTest.Student =
                                    studentLogic.GetModelsBy(
                                        p =>
                                            p.Matric_Number == resultFormat.MatricNo.Trim() &&
                                            p.Person_Id == courseRegistration.Student.Id).FirstOrDefault();
                                if(studentResultDetailTest.Student != null)
                                {
                                    //To ensure that there is no alteration of existing Test score
                                    var existingCourse=courseRegistrationDetailLogic.GetModelsBy(f => f.Course_Id == viewModel.Course.Id && f.Student_Course_Registration_Id == courseRegistration.Id).FirstOrDefault();
                                    //Enable alteration of existing test score(request from school)
                                    if(viewModel.Course?.Department?.Id==63 && (viewModel.Course?.Programme?.Id==4 || viewModel.Course?.Programme?.Id == 1) && viewModel.Session?.Id == 18 && viewModel.Course?.Level?.Id==4)
                                    {
                                        studentResultDetailTest.Score = resultFormat.CA;
                                    }
                                    else
                                    {
                                        studentResultDetailTest.Score = existingCourse?.TestScore != null && existingCourse?.TestScore != 0 ? existingCourse.TestScore : resultFormat.CA;
                                    }
                                   
                                    studentResultDetailTest.SpecialCaseMessage =
                                        resultFormat.ResultSpecialCaseMessages.TestSpecialCaseMessage;
                                    studentResultDetailTestList.Add(studentResultDetailTest);
                                    studentResultTest.Results = studentResultDetailTestList;
                                    studentResultLogic.Add(studentResultTest, courseRegistrationDetailAudit);

                                    studentResultDetailExam.Course = viewModel.Course;
                                    studentResultDetailExam.Student = studentResultDetailTest.Student;

                                    //To ensure that there is no alteration of existing exam score
                                    if (viewModel.Course?.Department?.Id == 63 && (viewModel.Course?.Programme?.Id == 4 || viewModel.Course?.Programme?.Id == 1) && viewModel.Session?.Id == 18)
                                    {
                                        studentResultDetailExam.Score = resultFormat.Exam;
                                        studentResultDetailExam.SpecialCaseMessage = 
                                            resultFormat.ResultSpecialCaseMessages.ExamSpecialCaseMessage;
                                    }
                                    else
                                    {
                                        studentResultDetailExam.Score = existingCourse?.ExamScore != null && existingCourse?.ExamScore != 0 ? existingCourse.ExamScore : resultFormat.Exam;
                                        studentResultDetailExam.SpecialCaseMessage = existingCourse?.SpecialCase != null ? existingCourse.SpecialCase :
                                            resultFormat.ResultSpecialCaseMessages.ExamSpecialCaseMessage;
                                    }
                                       
                                    studentResultDetailExamList.Add(studentResultDetailExam);
                                    studentResultExam.Results = studentResultDetailExamList;
                                    studentResultLogic.Add(studentResultExam, courseRegistrationDetailAudit);
                                }
                            }
                        }
                        scope.Complete();
                    }
                }

                return true;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private static void InitializeStudentResult(StaffCourseAllocationViewModel viewModel,string FileUploadURL,
            SessionSemester sessionSemester,StudentResultType testResultType,StudentResultType examResultType,
            User user,out StudentResultDetail studentResultDetailTest,out StudentResultDetail studentResultDetailExam,
            out List<StudentResultDetail> studentResultDetailTestList,
            out List<StudentResultDetail> studentResultDetailExamList,out StudentResultLogic studentResultLogic,
            out StudentResult studentResultTest,out StudentResult studentResultExam)
        {
            studentResultDetailTest = new StudentResultDetail();
            studentResultDetailExam = new StudentResultDetail();
            studentResultDetailTestList = new List<StudentResultDetail>();
            studentResultDetailExamList = new List<StudentResultDetail>();
            studentResultLogic = new StudentResultLogic();
            studentResultTest = new StudentResult();
            studentResultExam = new StudentResult();

            studentResultTest.MaximumObtainableScore = 30;
            studentResultTest.DateUploaded = DateTime.Now;
            studentResultTest.Department = viewModel.Course.Department;
            studentResultTest.Level = viewModel.CourseAllocation.Level;
            studentResultTest.Programme = viewModel.CourseAllocation.Programme;
            studentResultTest.SessionSemester = sessionSemester;
            studentResultTest.UploadedFileUrl = FileUploadURL;
            studentResultTest.Uploader = user;
            studentResultTest.Type = testResultType;

            studentResultExam.MaximumObtainableScore = 70;
            studentResultExam.DateUploaded = DateTime.Now;
            studentResultExam.Department = viewModel.Course.Department;
            studentResultExam.Level = viewModel.CourseAllocation.Level;
            studentResultExam.Programme = viewModel.CourseAllocation.Programme;
            studentResultExam.SessionSemester = sessionSemester;
            studentResultExam.UploadedFileUrl = FileUploadURL;
            studentResultExam.Uploader = user;
            studentResultExam.Type = examResultType;
        }

        private List<ResultHolder> RetrieveCourseRegistrationInformation(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if(viewModel != null && viewModel.resultFormatList != null)
                {
                    var results = new List<ResultHolder>();
                    foreach(ResultFormat resultFormat in viewModel.resultFormatList)
                    {
                        var students = new List<Model.Model.Student>();
                        var studentLogic = new StudentLogic();
                        students = studentLogic.GetModelsBy(p => p.Matric_Number == resultFormat.MatricNo);

                        var courseRegistrationLogic = new CourseRegistrationLogic();
                        if(students.Count == 1)
                        {
                            var courseRegistration = new CourseRegistration();
                            var result = new ResultHolder();
                            long studentId = students[0].Id;
                            courseRegistration =
                                courseRegistrationLogic.GetModelBy(
                                    p =>
                                        p.Person_Id == studentId && p.Level_Id == viewModel.Level.Id &&
                                        p.Session_Id == viewModel.Session.Id &&
                                        p.Department_Id == viewModel.Department.Id &&
                                        p.Programme_Id == viewModel.Programme.Id);

                            if(courseRegistration != null)
                            {
                                result.CourseRegistration = courseRegistration;
                                result.ResultFormat = resultFormat;
                                results.Add(result);
                            }
                        }
                    }
                    return results;
                }
                return new List<ResultHolder>();
            }
            catch(Exception)
            {
                throw;
            }
        }

        private int validateFields(List<ResultFormat> list)
        {
            try
            {
                int failedReason = 0;

                if(list != null && list.Count > 0)
                {
                    for(int i = 0;i < list.Count;i++)
                    {
                        bool testStatus;
                        bool examStatus;
                        bool totalStatus;
                        decimal? testScore = list[i].CA;
                        decimal? inputExamScore = list[i].Exam;
                        if(testScore == (decimal)SpeicalCaseCodes.Sick ||
                            inputExamScore == (decimal)SpeicalCaseCodes.Sick ||
                            testScore == (decimal)SpeicalCaseCodes.Absent ||
                            inputExamScore == (decimal)SpeicalCaseCodes.Absent)
                        {
                            AssignSpecialCaseRemarks(list,i,testScore,inputExamScore);
                        }
                        else
                        {
                            if(testScore >= 0 && testScore <= 30)
                            {
                                testStatus = true;
                            }
                            else
                            {
                                testStatus = false;
                                if(failedReason == 0)
                                {
                                    failedReason += (i + 1);
                                }
                            }

                            if((inputExamScore >= 0 && inputExamScore <= 70))
                            {
                                examStatus = true;
                            }
                            else
                            {
                                examStatus = false;
                                if(failedReason == 0)
                                {
                                    failedReason += (i + 1);
                                }
                            }
                        }
                    }
                }
                if(failedReason > 0)
                {
                    return failedReason;
                }
                return 0;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private static void AssignSpecialCaseRemarks(List<ResultFormat> list,int i,decimal? testScore,
            decimal? inputExamScore)
        {
            try
            {
                if(testScore == (decimal)SpeicalCaseCodes.Sick)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "SICK: TEST";
                    list[i].ResultSpecialCaseMessages.TestSpecialCaseMessage = "SICK: TEST";
                    list[i].CA = 0;
                }
                else if(inputExamScore == (decimal)SpeicalCaseCodes.Sick)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "SICK";
                    list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = "SICK";
                    list[i].Exam = 0;
                }
                else if(inputExamScore == (decimal)SpeicalCaseCodes.Absent)
                {
                    list[i].ResultSpecialCaseMessages.SpecialCaseMessage = "ABSENT";
                    list[i].ResultSpecialCaseMessages.ExamSpecialCaseMessage = "ABSENT";
                    list[i].Exam = 0;
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

                return Json(new SelectList(departments,ID,NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetDepartmentOptions(string id, string departmentId)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(departmentId))
                {
                    return null;
                }

                var programme = new Programme { Id = Convert.ToInt32(id) };
                var department = new Department { Id = Convert.ToInt32(departmentId) };
                var departmentOptionLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOptions = departmentOptionLogic.GetBy(department,programme);

                return Json(new SelectList(departmentOptions, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
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

                return Json(new SelectList(semesters,ID,NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetCourses(int[] ids)
        {
            try
            {
                if(ids.Count() == 0)
                {
                    return null;
                }
                var level = new Level { Id = Convert.ToInt32(ids[1]) };
                var department = new Department { Id = Convert.ToInt32(ids[0]) };
                var semester = new Semester { Id = Convert.ToInt32(ids[2]) };
                var programme = new Programme { Id = Convert.ToInt32(ids[3]) };
                var session = new Session { Id = Convert.ToInt32(ids[4]) };
                List<Course> courseList = Utility.GetOnlyRegisteredCoursesByLevelDepartmentAndSemester(level,department,
                    semester,programme,session);

                return Json(new SelectList(courseList,ID,NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetCoursesNew(int[] ids)
        {
            try
            {
                if (ids.Count() == 0)
                {
                    return null;
                }
                var level = new Level { Id = Convert.ToInt32(ids[1]) };
                var department = new Department { Id = Convert.ToInt32(ids[0]) };
                var semester = new Semester { Id = Convert.ToInt32(ids[2]) };
                var programme = new Programme { Id = Convert.ToInt32(ids[3]) };
                var session = new Session { Id = Convert.ToInt32(ids[4]) };

                var departmentOption = new DepartmentOption { Id = Convert.ToInt32(ids[5]) };
                
                
                List<Course> courseList = Utility.GetOnlyRegisteredCoursesByLevelDepartmentAndSemester(level, department,
                    semester, programme, session, departmentOption);

                return Json(new SelectList(courseList, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void KeepDropDownState(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.User = viewModel.UserSelectList;
                if(viewModel.Semester != null)
                {
                    var sessionSemesterList = new List<SessionSemester>();
                    var sessionSemesterLogic = new SessionSemesterLogic();
                    sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id);

                    var semesters = new List<Semester>();
                    foreach(SessionSemester sessionSemester in sessionSemesterList)
                    {
                        semesters.Add(sessionSemester.Semester);
                    }

                    ViewBag.Semester = new SelectList(semesters,ID,NAME,viewModel.Semester.Id);
                }
                if(viewModel.Department != null && viewModel.Department.Id > 0)
                {
                    var departmentLogic = new DepartmentLogic();
                    var departments = new List<Department>();
                    departments = departmentLogic.GetBy(viewModel.Programme);

                    ViewBag.Department = new SelectList(departments,ID,NAME,viewModel.Department.Id);
                }
                if(viewModel.Course != null && viewModel.Course.Id > 0)
                {
                    var courseList = new List<Course>();
                    courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Level,viewModel.Department,
                        viewModel.Semester,viewModel.Programme);

                    ViewBag.Course = new SelectList(courseList,ID,NAME,viewModel.Course.Id);
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void KeepCourseDropDownState(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.User = viewModel.UserSelectList;
                ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), ID, NAME);
                if (viewModel.CourseAllocation.Semester != null)
                {
                    var sessionSemesterList = new List<SessionSemester>();
                    var sessionSemesterLogic = new SessionSemesterLogic();
                    sessionSemesterList =
                        sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.CourseAllocation.Session.Id);

                    var semesters = new List<Semester>();
                    foreach(SessionSemester sessionSemester in sessionSemesterList)
                    {
                        semesters.Add(sessionSemester.Semester);
                    }

                    ViewBag.Semester = new SelectList(semesters,ID,NAME,viewModel.CourseAllocation.Semester.Id);
                }
                if(viewModel.CourseAllocation.Department != null)
                {
                    var departmentLogic = new DepartmentLogic();
                    var departments = new List<Department>();
                    departments = departmentLogic.GetBy(viewModel.CourseAllocation.Programme);

                    ViewBag.Department = new SelectList(departments,ID,NAME,viewModel.CourseAllocation.Department.Id);
                    if (viewModel.CourseAllocation.DepartmentOption != null)
                    {
                        var departmentOptionLogic = new DepartmentOptionLogic();
                        var departmentOptions = new List<DepartmentOption>();
                        departmentOptions = departmentOptionLogic.GetBy(viewModel.CourseAllocation.Department, viewModel.CourseAllocation.Programme);

                        ViewBag.DepartmentOption = new SelectList(departmentOptions, ID, NAME, viewModel.CourseAllocation.DepartmentOption.Id);
                        
                    }
                    else
                    {
                        ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), ID, NAME);
                    }
                }
               
                if (viewModel.CourseAllocation.Course != null)
                {
                    var courseList = new List<Course>();
                    courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.CourseAllocation.Level,
                        viewModel.CourseAllocation.Department,viewModel.CourseAllocation.Semester,
                        viewModel.CourseAllocation.Programme);

                    ViewBag.Course = new SelectList(courseList,ID,NAME,viewModel.CourseAllocation.Course.Id);
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private DataSet ReadExcel(string filepath)
        {
            DataSet Result = null;
            try
            {

                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" +
                                  "Extended Properties=Excel 8.0;";

                //string xConnStr = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filepath}; Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'";
                var connection = new OleDbConnection(xConnStr);

                connection.Open();
                DataTable sheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,null);
                foreach(DataRow dataRow in sheet.Rows)
                {
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
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public void RetainDropdownState(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var semesterLogic = new SemesterLogic();
                var departmentLogic = new DepartmentLogic();
                var sessionLogic = new SessionLogic();
                var programmeLogic = new ProgrammeLogic();
                var levelLogic = new LevelLogic();
                if(viewModel != null)
                {
                    if(viewModel.Session != null)
                    {
                        ViewBag.Session = new SelectList(sessionLogic.GetModelsBy(p => p.Activated == true),ID,NAME,
                            viewModel.Session.Id);
                    }
                    else
                    {
                        ViewBag.Session = viewModel.SessionSelectList;
                    }
                    if(viewModel.Semester != null)
                    {
                        ViewBag.Semester = new SelectList(semesterLogic.GetAll(),ID,NAME,viewModel.Semester.Id);
                    }
                    else
                    {
                        ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                    }
                    if(viewModel.Programme != null)
                    {
                        ViewBag.Programme = new SelectList(programmeLogic.GetModelsBy(p => p.Activated == true),ID,
                            NAME,viewModel.Programme.Id);
                    }
                    else
                    {
                        ViewBag.Programme = viewModel.ProgrammeSelectList;
                    }
                    if(viewModel.Department != null && viewModel.Programme != null)
                    {
                        ViewBag.Department = new SelectList(departmentLogic.GetBy(viewModel.Programme),ID,NAME,
                            viewModel.Department.Id);
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(new List<Department>(),ID,NAME);
                    }
                    if(viewModel.Level != null)
                    {
                        ViewBag.Level = new SelectList(levelLogic.GetAll(),ID,NAME,viewModel.Level.Id);
                    }
                    else
                    {
                        ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                    }
                    if(viewModel.Course != null && viewModel.Level != null && viewModel.Semester != null &&
                        viewModel.Department != null)
                    {
                        List<Course> courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Level,
                            viewModel.Department,viewModel.Semester,viewModel.Programme);
                        ViewBag.Course = new SelectList(courseList,ID,NAME,viewModel.Level.Id);
                    }
                    else
                    {
                        ViewBag.Course = new SelectList(new List<Course>(),ID,NAME);
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult StaffReportSheet()
        {
            StaffCourseAllocationViewModel viewModel = null;
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
            }
            catch(Exception)
            {
                throw;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult StaffReportSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.Session_Id == viewModel.Session.Id &&
                            p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(viewModel);
            }
            catch(Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult StaffDownloadReportSheet(string cid)
        {
            try
            {
                long Id = Convert.ToInt64(Utility.Decrypt(cid));
                viewModel = new StaffCourseAllocationViewModel();
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocation =
                    courseAllocationLogic.GetModelsBy(p => p.Course_Allocation_Id == Id).LastOrDefault();
                if(viewModel.CourseAllocation != null)
                {
                    var reportViewModel = new ReportViewModel();
                    reportViewModel.Department = viewModel.CourseAllocation.Department;
                    reportViewModel.Course = viewModel.CourseAllocation.Course;
                    reportViewModel.Faculty = viewModel.CourseAllocation.Department.Faculty;
                    reportViewModel.Level = viewModel.CourseAllocation.Level;
                    reportViewModel.Programme = viewModel.CourseAllocation.Programme;
                    reportViewModel.Semester = viewModel.CourseAllocation.Semester;
                    reportViewModel.Session = viewModel.CourseAllocation.Session;
                    TempData["ReportViewModel"] = reportViewModel;
                    return RedirectToAction("ResultSheet","Report",new { area = "admin" });
                }

                return RedirectToAction("StaffReportSheet","StaffCourseAllocation",new { area = "admin" });
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult DownloadBlankResultSheet()
        {
            try
            {
                var viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                return View(viewModel);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message,Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult DownloadBlankResultSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);
                KeepDropDownState(viewModel);
                TempData["vModel"] = viewModel;
            }
            catch(Exception)
            {
                throw;
            }
            return View(viewModel);
        }

        public ActionResult BlankResultUploadSheet(long cid)
        {
            try
            {
                var courseAllocation = new CourseAllocation();
                var courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == cid);
                if(courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination",
                        Message.Category.Error);
                    return RedirectToAction("StaffResultSheet");
                }
                var gv = new GridView();
                var ds = new DataTable();
                var resultFormatList = new List<ResultFormat>();
                var sampleFormat = new ResultFormat();
                sampleFormat.MatricNo = "N/XX/15/12345";
                resultFormatList.Add(sampleFormat);
                gv.DataSource = resultFormatList; // resultFormatList.OrderBy(p => p.MATRICNO);
                var courseLogic = new CourseLogic();
                Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                gv.Caption = course.Name + " " + course.Code + " " + " DEPARTMENT OF " + " " +
                             course.Department.Name.ToUpper() + " " + course.Unit + " " + "Units";
                gv.DataBind();

                string filename = course.Code.Replace("/","").Replace("\\","") + course.Department.Code + ".xls";
                return new DownloadFileActionResult(gv,filename);
            }
            catch(Exception ex)
            {
                SetMessage("Error occured! " + ex.Message,Message.Category.Error);
            }
            return RedirectToAction("StaffResultSheet");
        }

        [HttpPost]
        public ActionResult BlankResultUploadSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var courseAllocation = new CourseAllocation();
                var courseAllocationLogic = new CourseAllocationLogic();
                courseAllocation = courseAllocationLogic.GetModelBy(p => p.Course_Allocation_Id == viewModel.cid);
                var courseLogic = new CourseLogic();
                Course course = courseLogic.GetModelBy(p => p.Course_Id == courseAllocation.Course.Id);
                if(courseAllocation == null)
                {
                    SetMessage("You are not allocated to this course, with this programme-department combination",
                        Message.Category.Error);
                    return RedirectToAction("StaffResultSheet");
                }
                var resultFormatList = new List<ResultFormat>();
                foreach(string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving,hpf.FileName);
                    FileUploadURL = savedFileName;
                    hpf.SaveAs(savedFileName);
                    DataSet studentSet = ReadExcel(savedFileName);

                    if(studentSet != null && studentSet.Tables[0].Rows.Count > 0)
                    {
                        for(int i = 1;i < studentSet.Tables[0].Rows.Count;i++)
                        {
                            var resultFormat = new ResultFormat();
                            resultFormat.SN = Convert.ToInt32(studentSet.Tables[0].Rows[i][0].ToString().Trim());
                            resultFormat.MatricNo = studentSet.Tables[0].Rows[i][1].ToString().Trim();
                            resultFormat.CA = Convert.ToDecimal(studentSet.Tables[0].Rows[i][4].ToString().Trim());
                            resultFormat.Exam = Convert.ToDecimal(studentSet.Tables[0].Rows[i][5].ToString().Trim());
                            if(resultFormat.MatricNo != "")
                            {
                                resultFormatList.Add(resultFormat);
                            }
                        }
                        resultFormatList.OrderBy(p => p.MatricNo);
                        viewModel.resultFormatList = resultFormatList;
                        viewModel.Course = course;
                        var vModel = (StaffCourseAllocationViewModel)TempData["vModel"];
                        vModel.Course = course;
                        vModel.resultFormatList = resultFormatList;
                        vModel.CourseAllocation = courseAllocation;
                        TempData["staffCourseAllocationViewModel"] = vModel;
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message,Message.Category.Error);
            }
            RetainDropdownState(viewModel);
            return View(viewModel);
        }

        public ActionResult SaveUploadedBlankResultSheet()
        {
            var viewModel = (StaffCourseAllocationViewModel)TempData["staffCourseAllocationViewModel"];
            try
            {
                if(viewModel != null)
                {
                    int status = validateFields(viewModel.resultFormatList);

                    if(status > 0)
                    {
                        ResultFormat format = viewModel.resultFormatList.ElementAt((status - 1));
                        SetMessage("Validation Error for" + " " + format.MatricNo,Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return RedirectToAction("DownloadBlankResultSheet");
                    }

                    bool resultAdditionStatus = AddUnregisteredStudentResult(viewModel);

                    SetMessage("Upload successful",Message.Category.Information);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error occured " + ex.Message,Message.Category.Error);
            }

            RetainDropdownState(viewModel);
            return RedirectToAction("DownloadBlankResultSheet");
        }

        private bool AddUnregisteredStudentResult(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                viewModel = (StaffCourseAllocationViewModel)TempData["staffCourseAllocationViewModel"];
                var sessionSemesterLogicc = new SessionSemesterLogic();
                var sessionSemester = new SessionSemester();
                var userLogic = new UserLogic();
                var testResultType = new StudentResultType { Id = 1 };
                var examResultType = new StudentResultType { Id = 2 };
                User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                sessionSemester =
                    sessionSemesterLogicc.GetModelBy(
                        p => p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id);
                if(viewModel != null && viewModel.resultFormatList.Count > 0)
                {
                    var studentExamRawScoreSheet = new StudentExamRawScoreSheet();
                    var StudentExamRawScoreSheetLogic = new StudentExamRawScoreSheetNotRegisteredLogic();

                    var studentResultDetailLogic = new StudentResultDetailLogic();
                    using(var scope = new TransactionScope(TransactionScopeOption.Required,new TimeSpan(0,15,0)))
                    {
                        foreach(ResultFormat resultFormat in viewModel.resultFormatList)
                        {
                            studentExamRawScoreSheet =
                                StudentExamRawScoreSheetLogic.GetModelBy(
                                    p =>
                                        p.Student_Matric_Number == resultFormat.MatricNo &&
                                        p.Semester_Id == viewModel.Semester.Id && p.Session_Id == viewModel.Session.Id &&
                                        p.Course_Id == viewModel.Course.Id);
                            if(studentExamRawScoreSheet == null)
                            {
                                studentExamRawScoreSheet = CreateUnregisteredStudentExamRawScoreSheet(viewModel,user,
                                    studentExamRawScoreSheet,StudentExamRawScoreSheetLogic,resultFormat,"");
                            }
                            else
                            {
                                ModifyUnregisteredStudentExamRawScoreSheet(viewModel,user,studentExamRawScoreSheet,
                                    StudentExamRawScoreSheetLogic,resultFormat,"");
                            }
                        }
                        scope.Complete();
                    }
                }

                return true;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private static StudentExamRawScoreSheet CreateUnregisteredStudentExamRawScoreSheet(
            StaffCourseAllocationViewModel viewModel,User user,StudentExamRawScoreSheet studentExamRawScoreSheet,
            StudentExamRawScoreSheetNotRegisteredLogic studentExamRawScoreSheetNotRegisteredLogic,
            ResultFormat resultFormat,string fileURL)
        {
            studentExamRawScoreSheet = new StudentExamRawScoreSheet();
            studentExamRawScoreSheet.Course = viewModel.Course;

            studentExamRawScoreSheet.Semester = viewModel.Semester;
            studentExamRawScoreSheet.Session = viewModel.Session;
            studentExamRawScoreSheet.Special_Case = resultFormat.ResultSpecialCaseMessages.SpecialCaseMessage;
            studentExamRawScoreSheet.MatricNumber = resultFormat.MatricNo;
            studentExamRawScoreSheet.Uploader = user;
            studentExamRawScoreSheet.FileUploadURL = fileURL;
            studentExamRawScoreSheetNotRegisteredLogic.Create(studentExamRawScoreSheet);
            return studentExamRawScoreSheet;
        }

        private static void ModifyUnregisteredStudentExamRawScoreSheet(StaffCourseAllocationViewModel viewModel,
            User user,StudentExamRawScoreSheet studentExamRawScoreSheet,
            StudentExamRawScoreSheetNotRegisteredLogic studentExamRawScoreSheetNotRegisteredLogic,
            ResultFormat resultFormat,string fileURL)
        {
            studentExamRawScoreSheet.Course = viewModel.Course;

            studentExamRawScoreSheet.Semester = viewModel.Semester;
            studentExamRawScoreSheet.Session = viewModel.Session;
            studentExamRawScoreSheet.Special_Case = resultFormat.ResultSpecialCaseMessages.SpecialCaseMessage;
            studentExamRawScoreSheet.MatricNumber = resultFormat.MatricNo;
            studentExamRawScoreSheet.Uploader = user;
            studentExamRawScoreSheet.FileUploadURL = fileURL;
            bool isScoreSheetModified = studentExamRawScoreSheetNotRegisteredLogic.Modify(studentExamRawScoreSheet);
        }

        public ActionResult StaffBlankReportSheet()
        {
            StaffCourseAllocationViewModel viewModel = null;
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
            }
            catch(Exception)
            {
                throw;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult StaffBlankReportSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name);

                KeepDropDownState(viewModel);
            }
            catch(Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult AdminBlankReportSheet()
        {
            StaffCourseAllocationViewModel viewModel = null;
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(),ID,NAME);
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AdminBlankReportSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.Programme_Id == viewModel.Programme.Id &&
                            p.Department_Id == viewModel.Department.Id);

                KeepDropDownState(viewModel);
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult StaffDownloadBlankReportSheet(int Session_Id,int Semester_Id,int Programme_Id,
            int Department_Id,int Level_Id,int Course_Id)
        {
            try
            {
                return RedirectToAction("UnregisteredStudentResultSheet",
                    new
                    {
                        area = "admin",
                        controller = "Report",
                        levelId = Level_Id,
                        semesterId = Semester_Id,
                        progId = Programme_Id,
                        deptId = Department_Id,
                        sessionId = Session_Id,
                        courseId = Course_Id
                    });
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult ViewUploadedCourses()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),"Id","Name");
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ContentResult ViewUploadedCourses(string sessionId,string semesterId)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            List<UploadedCourseFormat> uploadedCourses = new List<UploadedCourseFormat>();
            try
            {

                var studentResultLogic = new StudentResultLogic();
                viewModel = new StaffCourseAllocationViewModel
                {
                    Session = new Session()
                    {
                        Id = Convert.ToInt32(sessionId)
                    },
                    Semester = new Semester()
                    {
                        Id = Convert.ToInt32(semesterId)
                    }
                };

                uploadedCourses = studentResultLogic.GetUploadedCourses(viewModel.Session, viewModel.Semester);
                TempData["UploadedCourses"] = uploadedCourses;
                TempData.Keep("UploadedCourses");

            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            RetainSessionSemesterDropDown(viewModel);
            var serializedList = serializer.Serialize(uploadedCourses);
            ContentResult uploadedCourseList = new ContentResult
            {
                Content = serializedList,
                ContentType = "application/json"
            };

            return uploadedCourseList;
        }

        private void RetainSessionSemesterDropDown(StaffCourseAllocationViewModel viewModel)
        {
            var sessionSemesterList = new List<SessionSemester>();
            var sessionSemesterLogic = new SessionSemesterLogic();
            sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id);

            var semesters = new List<Semester>();
            foreach(SessionSemester sessionSemester in sessionSemesterList)
            {
                semesters.Add(sessionSemester.Semester);
            }
            ViewBag.Session = viewModel.SessionSelectList;
            if(viewModel.Semester != null)
            {
                ViewBag.Semester = new SelectList(semesters,"Id","Name",viewModel.Semester.Id);
            }
            else
            {
                ViewBag.Semester = new SelectList(new List<Semester>(),"Id","Name");
            }
        }

        public ActionResult ViewScoreSheet(string index,string session)
        {
            try
            {
                long Id = Convert.ToInt64(index);
                long SessionId = Convert.ToInt64(session);
                viewModel = new StaffCourseAllocationViewModel();
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocation =
                    courseAllocationLogic.GetModelsBy(p => p.Course_Id == Id && p.Session_Id == SessionId).LastOrDefault();
                if (viewModel.CourseAllocation != null)
                {
                    var reportViewModel = new ReportViewModel();
                    reportViewModel.Department = viewModel.CourseAllocation.Department;
                    reportViewModel.Course = viewModel.CourseAllocation.Course;
                    reportViewModel.Faculty = viewModel.CourseAllocation.Department.Faculty;
                    reportViewModel.Level = viewModel.CourseAllocation.Level;
                    reportViewModel.Programme = viewModel.CourseAllocation.Programme;
                    reportViewModel.Semester = viewModel.CourseAllocation.Semester;
                    reportViewModel.Session = viewModel.CourseAllocation.Session;
                    TempData["ReportViewModel"] = reportViewModel;
                }

                return RedirectToAction("ResultSheet", "Report", new { area = "admin" });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult DepartmentReportSheet()
        {
            StaffCourseAllocationViewModel viewModel = null;
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
            }
            catch(Exception)
            {
                throw;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DepartmentReportSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Session_Id == viewModel.Session.Id && p.USER.User_Name == User.Identity.Name &&
                            p.Is_HOD == true);
                if(viewModel.CourseAllocations != null && viewModel.CourseAllocations.Count > 0)
                {
                    int HodDepartmentId = viewModel.CourseAllocations[0].HodDepartment.Id;
                    viewModel.CourseAllocations =
                        courseAllocationLogic.GetModelsBy(
                            p =>
                                p.Department_Id == HodDepartmentId && p.Level_Id == viewModel.Level.Id &&
                                p.Session_Id == viewModel.Session.Id);
                }
                KeepDropDownState(viewModel);
            }
            catch(Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult ViewAlternateScoreSheet()
        {
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),"Id","Name");
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewAlternateScoreSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var studentResultLogic = new StudentResultLogic();
                viewModel.UploadedCourses = studentResultLogic.GetUploadedAlternateCourses(viewModel.Session,
                    viewModel.Semester);
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }
            TempData["UploadedCourses"] = viewModel.UploadedCourses;
            TempData.Keep("UploadedCourses");
            RetainSessionSemesterDropDown(viewModel);

            return View(viewModel);
        }

        public ActionResult DownloadViewAlternateScoreSheet(int Session_Id,int Semester_Id,int Programme_Id,
            int Department_Id,int Level_Id,int Course_Id)
        {
            try
            {
                return RedirectToAction("UnregisteredStudentResultSheet",
                    new
                    {
                        area = "admin",
                        controller = "Report",
                        levelId = Level_Id,
                        semesterId = Semester_Id,
                        progId = Programme_Id,
                        deptId = Department_Id,
                        sessionId = Session_Id,
                        courseId = Course_Id
                    });
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult AdminDownloadBlankReportSheet()
        {
            StaffCourseAllocationViewModel viewModel = null;
            try
            {
                viewModel = new StaffCourseAllocationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(),ID,NAME);
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AdminDownloadBlankReportSheet(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                var courseAllocationLogic = new CourseAllocationLogic();
                viewModel.CourseAllocations =
                    courseAllocationLogic.GetModelsBy(
                        p =>
                            p.Level_Id == viewModel.Level.Id && p.COURSE.Semester_Id == viewModel.Semester.Id &&
                            p.Session_Id == viewModel.Session.Id && p.Programme_Id == viewModel.Programme.Id &&
                            p.Department_Id == viewModel.Department.Id);

                KeepDropDownState(viewModel);
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult AdminBlankResultUploadSheet(long cid,int deptId,int progId,int levelId,int sessionId)
        {
            try
            {
                var scoreSheetNotRegisteredLogic = new StudentExamRawScoreSheetNotRegisteredLogic();

                var gv = new GridView();
                var ds = new DataTable();
                List<ResultFormat> resultFormatList = scoreSheetNotRegisteredLogic.GetDownloadResultFormats(cid,deptId,
                    progId,levelId,sessionId);

                List<ResultFormat> list = resultFormatList.OrderBy(p => p.MatricNo).ToList();
                var sort = new List<ResultFormat>();
                for(int i = 0;i < list.Count;i++)
                {
                    list[i].SN = (i + 1);
                    sort.Add(list[i]);
                }

                gv.DataSource = sort; // resultFormatList.OrderBy(p => p.MATRICNO);
                var courseLogic = new CourseLogic();
                Course course = courseLogic.GetModelBy(p => p.Course_Id == cid);
                gv.Caption = course.Name + " " + course.Code + " " + " DEPARTMENT OF " + " " +
                             course.Department.Name.ToUpper() + " " + course.Unit + " " + "Units";
                gv.DataBind();

                string filename = course.Code.Replace("/","").Replace("\\","") + course.Department.Code + ".xls";
                return new DownloadFileActionResult(gv,filename);
            }
            catch(Exception ex)
            {
                SetMessage("Error occured! " + ex.Message,Message.Category.Error);
            }
            return RedirectToAction("AdminDownloadBlankReportSheet");
        }

        public ActionResult CourseEvaulationReport()
        {
            var viewModel = new StaffCourseAllocationViewModel();
            try
            {
                PopulateDropDown(viewModel);
            }
            catch (Exception ex)
            {
                    
                throw;
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CourseEvaulationReport(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                CourseEvaluationAnswerLogic courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                SessionSemester sessionSemester = sessionSemesterLogic.GetModelBy(a => a.Semester_Id == viewModel.CourseAllocation.Semester.Id && a.Session_Id == viewModel.CourseAllocation.Session.Id);
                viewModel.CourseEvaluationReports = courseEvaluationAnswerLogic.GetCourseEvaluationReport(viewModel.CourseAllocation.Programme, viewModel.CourseAllocation.Department, viewModel.CourseAllocation.Level, sessionSemester);
                PopulateDropDown(viewModel);
            }
            catch (Exception ex)
            {
                
                throw;
            }
            return View(viewModel);
        }

        public void PopulateDropDown(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                 DepartmentLogic departmentLogic = new DepartmentLogic();

                 if (viewModel.CourseAllocation != null)
                 {
                     if (viewModel.CourseAllocation.Session != null && viewModel.CourseAllocation.Session.Id > 0)
                     {
                         ViewBag.Session = new SelectList(viewModel.SessionSelectList, Utility.VALUE, Utility.TEXT, viewModel.CourseAllocation.Session.Id);
                     }
                     else
                     {
                         ViewBag.Session = viewModel.SessionSelectList;
                     }
                     if (viewModel.CourseAllocation.Semester != null && viewModel.CourseAllocation.Semester.Id > 0)
                     {
                         SemesterLogic semesterLogic = new SemesterLogic();
                         List<Semester> semster = semesterLogic.GetAll();
                         ViewBag.Semester = new SelectList(semster, ID, NAME, viewModel.CourseAllocation.Semester.Id);
                     }
                     if (viewModel.CourseAllocation.Programme != null && viewModel.CourseAllocation.Programme.Id > 0)
                     {
                         ViewBag.Programme = new SelectList(viewModel.ProgrammeSelectList, Utility.VALUE, Utility.TEXT, viewModel.CourseAllocation.Programme.Id);
                     }
                     else
                     {
                         ViewBag.Programme = viewModel.ProgrammeSelectList;
                     }
                     if (viewModel.CourseAllocation.Department != null && viewModel.CourseAllocation.Programme != null && viewModel.CourseAllocation.Department.Id > 0 && viewModel.CourseAllocation.Programme.Id > 0)
                     {
                         List<Department> department = departmentLogic.GetBy(viewModel.CourseAllocation.Programme);
                         ViewBag.Department = new SelectList(department, Utility.ID, Utility.NAME, viewModel.CourseAllocation.Department.Id);
                     }
                     else
                     {
                         ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                     }
                     if (viewModel.CourseAllocation.Level != null && viewModel.CourseAllocation.Level.Id > 1)
                     {
                         ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME, viewModel.CourseAllocation.Level.Id);
                     }
                     else
                     {
                         ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                     }
                 }
                 else
                 {
                     ViewBag.Session = viewModel.SessionSelectList;
                     ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                     ViewBag.Programme = viewModel.ProgrammeSelectList;
                     ViewBag.User = viewModel.UserSelectList;
                     ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                     ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                     ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                 }
              
                
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public JsonResult GetCourseEvaulationReport(string courseId)
        {
            List<CourseEvaluationReport> courseEvaluationReports;
          
            try
            {
                List<CourseEvaluationReport> reportData = new List<CourseEvaluationReport>();
                Course course = new Course(){Id = Convert.ToInt32(courseId)};
                CourseEvaluationAnswerLogic courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();

                viewModel.CourseEvaluationAnswers = courseEvaluationAnswerLogic.GetCourseEvaluationReport(course);
                int distinctStudents = viewModel.CourseEvaluationAnswers.Select(s => s.Student.Id).Distinct().Count();
                
                for (int i = 0; i < viewModel.CourseEvaluationAnswers.Count; i++)
                {
                    
                        CourseEvaluationReport courseEvaluationReport = new CourseEvaluationReport();
                        courseEvaluationReport.Score = viewModel.CourseEvaluationAnswers[i].Score / distinctStudents;
                        courseEvaluationReport.NumberOfStudent = distinctStudents;
                        courseEvaluationReport.Question = viewModel.CourseEvaluationAnswers[i].CourseEvaluationQuestion.Question;
                        courseEvaluationReport.QuestionId = viewModel.CourseEvaluationAnswers[i].CourseEvaluationQuestion.Id;
                        courseEvaluationReport.Section = viewModel.CourseEvaluationAnswers[i].CourseEvaluationQuestion.Section;
                        reportData.Add(courseEvaluationReport);
                }

                courseEvaluationReports = reportData.OrderBy(s => s.Section).ThenBy(s => s.QuestionId).ToList();
            }
            catch (Exception ex)
            {
                
                throw;
            }
            return Json(courseEvaluationReports, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadIndigeneZip(string downloadName)
        {
            TempData["downloadIndigeneName"] = downloadName + ".zip";
            return View();
        }

        public ActionResult GetIndigeneZip()
        {
            try
            {
                var downloadName = (string)TempData["downloadIndigeneName"];
                TempData.Keep("downloadIndigeneName");
                string path = "~/Content/tempIndigene/" + downloadName;
                Response.Redirect(path, false);
            }
            catch (Exception ex)
            {
                SetMessage("Error, " + ex.Message, Message.Category.Error);
            }

            return View("DownloadIndigeneZip");
        }
        public ActionResult DownloadUnderGraduateNominalZip(string downloadName)
        {
            TempData["downloadUndergraduateNominal"] = downloadName + ".zip";
            return View();
        }
        public ActionResult DownloadUnderGraduateNominal()
        {
            try
            {
                var downloadName = (string)TempData["downloadUndergraduateNominal"];
                TempData.Keep("downloadUndergraduateNominal");
                string path = "~/Content/tempNominal/" + downloadName;
                Response.Redirect(path, false);
            }
            catch (Exception ex)
            {
                SetMessage("Error, " + ex.Message, Message.Category.Error);
            }

            return View("DownloadUnderGraduateNominalZip");
        }
        public ActionResult DownloadPGNominalZip(string downloadName)
        {
            TempData["downloadUndergraduateNominal"] = downloadName + ".zip";
            return View();
        }
        public ActionResult DownloadPGNominal()
        {
            try
            {
                var downloadName = (string)TempData["downloadUndergraduateNominal"];
                TempData.Keep("downloadUndergraduateNominal");
                string path = "~/Content/tempNominal/" + downloadName;
                Response.Redirect(path, false);
            }
            catch (Exception ex)
            {
                SetMessage("Error, " + ex.Message, Message.Category.Error);
            }

            return View("DownloadPGNominalZip");
        }
        public void ExportToExcel(List<ResultFormat> source)
        {
            //var products = GetProducts();
            //GridView1.DataSource = products;
            GridView gridView = new GridView();
            gridView.DataSource = source;
            gridView.DataBind();
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Students");
            var totalCols = gridView.Rows[0].Cells.Count;
            var totalRows = gridView.Rows.Count;
            var headerRow = gridView.HeaderRow;
            for (var i = 1; i <= totalCols; i++)
            {
                workSheet.Cells[1, i].Value = headerRow.Cells[i - 1].Text;
            }
            for (var j = 1; j <= totalRows; j++)
            {
                for (var i = 1; i <= totalCols; i++)
                {
                    var results = source.ElementAt(j - 1);
                    workSheet.Cells[j + 1, i].Value = results.GetType().GetProperty(headerRow.Cells[i - 1].Text).GetValue(results, null);
                }
            }
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=products.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
    }
}