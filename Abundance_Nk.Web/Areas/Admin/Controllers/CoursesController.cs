using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class CoursesController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private CourseViewModel viewmodel;
        // GET: Admin/Courses
        public ActionResult Index()
        {
            viewmodel = new CourseViewModel();
            ViewBag.Departments = viewmodel.DepartmentSelectListItem;
            ViewBag.levels = viewmodel.levelSelectListItem;
            ViewBag.Programmes = viewmodel.ProgrammeSelectListItem;
            ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(CourseViewModel vModel)
        {
            try
            {
                var firstSemester = new Semester { Id = 1 };
                var secondSemester = new Semester { Id = 2 };
                var courseLogic = new CourseLogic();

                if (vModel.course.DepartmentOption != null && vModel.course.DepartmentOption.Id > 0)
                {
                    vModel.firstSemesterCourses = courseLogic.GetBy(vModel.course.Department,
                        vModel.course.DepartmentOption, vModel.course.Level, firstSemester, vModel.course.Programme);
                    vModel.secondSemesterCourses = courseLogic.GetBy(vModel.course.Department,
                        vModel.course.DepartmentOption, vModel.course.Level, secondSemester, vModel.course.Programme);
                }
                else
                {
                    vModel.firstSemesterCourses = courseLogic.GetBy(vModel.course.Department, vModel.course.Level,
                        firstSemester, vModel.course.Programme);
                    vModel.secondSemesterCourses = courseLogic.GetBy(vModel.course.Department, vModel.course.Level,
                        secondSemester, vModel.course.Programme);
                }

                RetainDropdown(vModel);
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(vModel);
        }

        public void RetainDropdown(CourseViewModel vModel)
        {
            try
            {
                ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);

                if (vModel.course.Department != null && vModel.course.Department.Id > 0)
                {
                    vModel.DepartmentOpionSelectListItem =
                        Utility.PopulateDepartmentOptionSelectListItem(vModel.course.Department, vModel.course.Programme);
                    ViewBag.Departments = new SelectList(vModel.DepartmentSelectListItem, Utility.VALUE, Utility.TEXT,
                        vModel.course.Department.Id);
                    if (vModel.course.DepartmentOption != null && vModel.course.DepartmentOption.Id > 0)
                    {
                        ViewBag.DepartmentOptionId = new SelectList(vModel.DepartmentOpionSelectListItem, Utility.VALUE,
                            Utility.TEXT, vModel.course.DepartmentOption.Id);
                    }
                }
                else
                {
                    ViewBag.Departments = viewmodel.DepartmentSelectListItem;
                    ViewBag.DepartmentOptionId = new SelectList(vModel.DepartmentOpionSelectListItem, VALUE, TEXT);
                }
                if (vModel.course.Level != null && vModel.course.Level.Id > 0)
                {
                    ViewBag.levels = new SelectList(vModel.levelSelectListItem, Utility.VALUE, Utility.TEXT,
                        vModel.course.Level.Id);
                }
                else
                {
                    ViewBag.levels = viewmodel.levelSelectListItem;
                }
                if (vModel.course.Programme != null && vModel.course.Programme.Id > 0)
                {
                    ViewBag.Programmes = new SelectList(vModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT,
                        vModel.course.Programme.Id);
                }
                else
                {
                    ViewBag.Programmes = vModel.ProgrammeSelectListItem;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult Edit()
        {
            viewmodel = new CourseViewModel();
            ViewBag.Departments = viewmodel.DepartmentSelectListItem;
            ViewBag.CourseTypeId = viewmodel.CourseTypeSelectListItem;
            ViewBag.levels = viewmodel.levelSelectListItem;
            ViewBag.Programmes = viewmodel.ProgrammeSelectListItem;
            ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(CourseViewModel vModel)
        {
            try
            {
                var firstSemester = new Semester { Id = 1 };
                var secondSemester = new Semester { Id = 2 };
                var courseLogic = new CourseLogic();

                if (vModel.course.DepartmentOption != null && vModel.course.DepartmentOption.Id > 0)
                {
                    vModel.firstSemesterCourses = courseLogic.GetBy(vModel.course.Department,
                        vModel.course.DepartmentOption, vModel.course.Level, firstSemester, vModel.course.Programme);
                    vModel.secondSemesterCourses = courseLogic.GetBy(vModel.course.Department,
                        vModel.course.DepartmentOption, vModel.course.Level, secondSemester, vModel.course.Programme);
                }
                else
                {
                    vModel.firstSemesterCourses = courseLogic.GetBy(vModel.course.Department, vModel.course.Level,
                        firstSemester, vModel.course.Programme);
                    vModel.secondSemesterCourses = courseLogic.GetBy(vModel.course.Department, vModel.course.Level,
                        secondSemester, vModel.course.Programme);
                }

                var firstSemesterBlanks = new Course();
                firstSemesterBlanks.Unit = 0;
                firstSemesterBlanks.Id = -1;
                firstSemesterBlanks.Semester = firstSemester;
                firstSemesterBlanks.Department = vModel.course.Department;
                firstSemesterBlanks.DepartmentOption = vModel.course.DepartmentOption;
                firstSemesterBlanks.Level = vModel.course.Level;
                firstSemesterBlanks.Programme = vModel.course.Programme;
                firstSemesterBlanks.Type = new CourseType();

                var secondSemesterBlanks = new Course();
                secondSemesterBlanks.Unit = 0;
                secondSemesterBlanks.Id = -1;
                secondSemesterBlanks.Semester = secondSemester;
                secondSemesterBlanks.Department = vModel.course.Department;
                secondSemesterBlanks.DepartmentOption = vModel.course.DepartmentOption;
                secondSemesterBlanks.Level = vModel.course.Level;
                secondSemesterBlanks.Programme = vModel.course.Programme;
                secondSemesterBlanks.Type = new CourseType();

                int blankCount = 5;
                if (vModel.firstSemesterCourses.Count < 1 && vModel.secondSemesterCourses.Count < 1)
                {
                    blankCount = 15;
                }
                for (int i = 0; i < blankCount; i++)
                {
                    vModel.firstSemesterCourses.Add(firstSemesterBlanks);
                    vModel.secondSemesterCourses.Add(secondSemesterBlanks);
                }
                if (vModel.firstSemesterCourses != null)
                {
                    for (int i = 0; i < vModel.firstSemesterCourses.Count; i++)
                    {
                        if (vModel.firstSemesterCourses[i].Type != null && vModel.firstSemesterCourses[i].Type.Id > 0)
                        {
                            ViewData["CourseTypeIdViewData" + i] = new SelectList(vModel.CourseTypeSelectListItem, VALUE,
                                TEXT, vModel.firstSemesterCourses[i].Type.Id);
                        }
                        else
                        {
                            ViewData["CourseTypeIdViewData" + i] = new SelectList(vModel.CourseTypeSelectListItem, VALUE,
                                TEXT, 0);
                        }
                    }
                }
                if (vModel.secondSemesterCourses != null)
                {
                    for (int i = 0; i < vModel.secondSemesterCourses.Count; i++)
                    {
                        if (vModel.secondSemesterCourses[i].Type != null && vModel.secondSemesterCourses[i].Type.Id > 0)
                        {
                            ViewData["SecondSemesterCourseTypeIdViewData" + i] =
                                new SelectList(vModel.CourseTypeSelectListItem, VALUE, TEXT,
                                    vModel.secondSemesterCourses[i].Type.Id);
                        }
                        else
                        {
                            ViewData["SecondSemesterCourseTypeIdViewData" + i] =
                                new SelectList(vModel.CourseTypeSelectListItem, VALUE, TEXT, 0);
                        }
                    }
                }
                RetainDropdown(vModel);
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Information);
            }
            return View(vModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveCourseChanges(CourseViewModel vModel)
        {
            bool status1 = false;
            bool status2 = false;
            try
            {
                if (vModel.firstSemesterCourses.Count > 0)
                {
                    var courseLogic = new CourseLogic();
                    var semester = new Semester { Id = 1 };
                    status1 = courseLogic.Modify(vModel.firstSemesterCourses);
                }
                if (vModel.secondSemesterCourses.Count > 0)
                {
                    var courseLogic = new CourseLogic();
                    var semester = new Semester { Id = 2 };
                    status2 = courseLogic.Modify(vModel.secondSemesterCourses);
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }
            if (status1 == true && status2 == true)
            {
                SetMessage("Courses were updated successfully", Message.Category.Information);
            }
            else
            {
                SetMessage("Some course were not updated because they have been registered", Message.Category.Information);

            }
            return RedirectToAction("Edit");
        }

        public JsonResult GetDepartmentOptionByDepartment(string id, string programmeid)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var department = new Department { Id = Convert.ToInt32(id) };
                var programme = new Programme { Id = Convert.ToInt32(programmeid) };
                var departmentLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOptions = departmentLogic.GetBy(department, programme);

                return Json(new SelectList(departmentOptions, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult ViewStudentCourses()
        {
            var viewModel = new CourseViewModel();
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Level = viewModel.levelSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewStudentCourses(CourseViewModel viewModel)
        {
            var studentLogic = new StudentLogic();
            var studentLevelLogic = new StudentLevelLogic();
            var courseRegistrationLogic = new CourseRegistrationLogic();
            var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
            var courseLogic = new CourseLogic();

            var student = new Model.Model.Student();
            var studentLevel = new StudentLevel();
            var courseRegistration = new CourseRegistration();
            var courseRegistrationDetail = new CourseRegistrationDetail();

            var students = new List<Model.Model.Student>();
            var courses = new List<Course>();
            var courseRegistrations = new List<CourseRegistration>();
            var courseIds = new List<long>();
            var courseRegDetails = new List<CourseRegistrationDetail>();

            try
            {
                if (viewModel.Student.MatricNumber != null && viewModel.Session.Id > 0 && viewModel.level.Id > 0)
                {
                    students = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
                    if (students.Count != 1)
                    {
                        SetMessage("Duplicate Matric Number or No Student Record", Message.Category.Error);
                        RetainStudentCourseDropDown(viewModel);
                        return View(viewModel);
                    }

                    student = students.FirstOrDefault();
                    studentLevel =
                        studentLevelLogic.GetModelBy(
                            sl =>
                                sl.Person_Id == student.Id && sl.Level_Id == viewModel.level.Id &&
                                sl.Session_Id == viewModel.Session.Id);
                    courses =
                        courseLogic.GetModelsBy(
                            c =>
                                c.Activated && c.Department_Id == studentLevel.Department.Id &&
                                c.Level_Id == viewModel.level.Id);
                    courseRegistrations =
                        courseRegistrationLogic.GetModelsBy(
                            cr =>
                                cr.Department_Id == studentLevel.Department.Id && cr.Level_Id == viewModel.level.Id &&
                                cr.Person_Id == student.Id && cr.Programme_Id == studentLevel.Programme.Id &&
                                cr.Session_Id == viewModel.Session.Id);

                    if (courseRegistrations.Count == 1)
                    {
                        courseRegistration = courseRegistrations.FirstOrDefault();
                        courseRegDetails =
                            courseRegistrationDetailLogic.GetModelsBy(
                                crd => crd.Student_Course_Registration_Id == courseRegistration.Id);
                        courseIds = courseRegDetails.Select(crd => crd.Course.Id).Distinct().ToList();

                        for (int i = 0; i < courses.Count; i++)
                        {
                            if (courseIds.Contains(courses[i].Id))
                            {
                                courses[i].IsRegistered = true;
                                long thisCourseId = courses[i].Id;
                                courseRegistrationDetail =
                                    courseRegDetails.Where(crd => crd.Course.Id == thisCourseId).SingleOrDefault();
                                if (courseRegistrationDetail.Mode.Id == 2)
                                {
                                    courses[i].isCarryOverCourse = true;
                                }
                            }
                        }

                        viewModel.Courses = courses;
                        viewModel.Student = student;
                        viewModel.CourseRegistration = courseRegistration;
                        RetainStudentCourseDropDown(viewModel);
                        return View(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainStudentCourseDropDown(viewModel);
            return View(viewModel);
        }

        public ActionResult SaveStudentCourses(CourseViewModel viewModel)
        {
            try
            {
                if (viewModel.Courses.Count > 0)
                {
                    var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    var firstAttemptCourseMode = new CourseMode { Id = 1 };
                    var carryOverCourseMode = new CourseMode { Id = 2 };
                    var courseRegistrationDetail = new CourseRegistrationDetail();
                    var courseRegistrationLogic = new CourseRegistrationLogic();
                    var courseLogic = new CourseLogic();

                    CourseRegistration courseRegistration =
                        courseRegistrationLogic.GetModelBy(
                            cr => cr.Student_Course_Registration_Id == viewModel.CourseRegistration.Id);

                    for (int i = 0; i < viewModel.Courses.Count; i++)
                    {
                        if (viewModel.Courses[i].IsRegistered)
                        {
                            long thisCourseId = viewModel.Courses[i].Id;
                            Course thisCourse = courseLogic.GetModelBy(c => c.Course_Id == thisCourseId);
                            courseRegistrationDetail =
                                courseRegistrationDetailLogic.GetModelBy(
                                    crd =>
                                        crd.Student_Course_Registration_Id == courseRegistration.Id &&
                                        crd.Course_Id == thisCourseId);
                            if (courseRegistrationDetail != null)
                            {
                                if (viewModel.Courses[i].isCarryOverCourse)
                                {
                                    courseRegistrationDetail.Mode = carryOverCourseMode;
                                    courseRegistrationDetailLogic.Modify(courseRegistrationDetail);
                                }
                                else
                                {
                                    courseRegistrationDetail.Mode = firstAttemptCourseMode;
                                    courseRegistrationDetailLogic.Modify(courseRegistrationDetail);
                                }
                            }
                            else
                            {
                                courseRegistrationDetail = new CourseRegistrationDetail();
                                courseRegistrationDetail.CourseRegistration = courseRegistration;
                                courseRegistrationDetail.Course = thisCourse;
                                if (viewModel.Courses[i].isCarryOverCourse)
                                {
                                    courseRegistrationDetail.Mode = carryOverCourseMode;
                                }
                                else
                                {
                                    courseRegistrationDetail.Mode = firstAttemptCourseMode;
                                }
                                courseRegistrationDetail.CourseUnit = thisCourse.Unit;
                                courseRegistrationDetail.Semester = thisCourse.Semester;

                                courseRegistrationDetailLogic.Create(courseRegistrationDetail);
                            }
                        }
                        else
                        {
                            long thisCourseId = viewModel.Courses[i].Id;
                            courseRegistrationDetail =
                                courseRegistrationDetailLogic.GetModelBy(
                                    crd =>
                                        crd.Student_Course_Registration_Id == courseRegistration.Id &&
                                        crd.Course_Id == thisCourseId);
                            if (courseRegistrationDetail != null)
                            {
                                courseRegistrationDetailLogic.Delete(
                                    crd => crd.Student_Course_Registration_Detail_Id == courseRegistrationDetail.Id);
                            }
                        }
                    }

                    SetMessage("Operation Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewStudentCourses");
        }

        private void RetainStudentCourseDropDown(CourseViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.Session != null && viewModel.Session.Id > 0)
                    {
                        ViewBag.Session = new SelectList(Utility.PopulateAllSessionSelectListItem(), "Value", "Text",
                            viewModel.Session.Id);
                    }
                    else
                    {
                        ViewBag.Session = new SelectList(Utility.PopulateAllSessionSelectListItem(), "Value", "Text");
                    }

                    if (viewModel.level != null && viewModel.level.Id > 0)
                    {
                        ViewBag.Level = new SelectList(Utility.PopulateLevelSelectListItem(), "Value", "Text",
                            viewModel.level.Id);
                    }
                    else
                    {
                        ViewBag.Level = new SelectList(Utility.PopulateLevelSelectListItem(), "Value", "Text");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult ViewCourses()
        {
            viewmodel = new CourseViewModel();
            ViewBag.Departments = viewmodel.DepartmentSelectListItem;
            ViewBag.levels = viewmodel.levelSelectListItem;
            ViewBag.Programmes = viewmodel.ProgrammeSelectListItem;
            ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);

            return View();
        }
        [HttpPost]
        public ActionResult ViewCourses(CourseViewModel viewModel)
        {
            RetainDropdown(viewModel);
            if (viewModel.course.Programme?.Id > 0 && viewModel.course.Department?.Id > 0 && viewModel.course.Level?.Id > 0)
            {
                CourseLogic courseLogic = new CourseLogic();
                viewModel.Courses = courseLogic.GetBy(viewModel.course.Department, viewModel.course.Level, viewModel.course.Programme, viewModel.course.DepartmentOption);
            }
            else
            {
                SetMessage("Please, select the requires Fields to continue", Message.Category.Information);
                return View(viewModel);
            }
            return View(viewModel);
        }
        public JsonResult DeactivateCourse(string id)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    var courseId = Convert.ToInt32(id);
                    CourseLogic courseLogic = new CourseLogic();
                    var course = courseLogic.GetModelBy(f => f.Course_Id == courseId);
                    if (course?.Id > 0)
                    {
                        course.Activated = false;
                        courseLogic.ActivateDeactivateCourse(course);
                        result.IsError = false;
                        result.Message = "Operation Successful";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }



                }


            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ActivateCourse(string id)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    var courseId = Convert.ToInt32(id);
                    CourseLogic courseLogic = new CourseLogic();
                    var course = courseLogic.GetModelBy(f => f.Course_Id == courseId);
                    if (course?.Id > 0)
                    {
                        course.Activated = true;
                        courseLogic.ActivateDeactivateCourse(course);
                        result.IsError = false;
                        result.Message = "Operation Successful";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }



                }


            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[AllowAnonymous]
        public ActionResult UploadCoursesByExcelSheet()
        {
            try
            {
                var viewModel = new CourseViewModel();
                ViewBag.ProgrammeSL = viewModel.ProgrammeSelectListItem;
                ViewBag.LevelSL = viewModel.levelSelectListItem;
                ViewBag.CourseTypeSL = viewModel.CourseTypeSelectListItem;

                return View();
            }
            catch (Exception ex) { throw ex; }
        }

        [HttpPost]
        //[AllowAnonymous]
        public ActionResult UploadCoursesByExcelSheet(CourseViewModel viewModel)
        {
            try
            {
                List<CourseUploadFormat> resultFormatList = new List<CourseUploadFormat>();

                if (Request.Files != null && Request.Files.Count > 0)
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase hpf = Request.Files[file];
                        string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                        string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                        hpf.SaveAs(savedFileName);
                        DataSet courseSet = ReadExcel(savedFileName);

                        if (courseSet != null && courseSet.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < courseSet.Tables[0].Rows.Count; i++)
                            {
                                var tblRow = courseSet.Tables[0].Rows[i];

                                var resultFormat = new CourseUploadFormat();
                                resultFormat.CourseCode = tblRow[0].ToString().Trim();
                                resultFormat.CourseName = tblRow[1].ToString().Trim();
                                resultFormat.CourseUnit = Convert.ToInt32(tblRow[2].ToString().Trim());
                                resultFormat.SemesterId = Convert.ToInt32(tblRow[3].ToString().Trim());

                                if (!(string.IsNullOrEmpty(resultFormat.CourseName)
                                    && string.IsNullOrEmpty(resultFormat.CourseCode))
                                    && resultFormat.CourseUnit > 0
                                    && resultFormat.SemesterId > 0)
                                {
                                    resultFormatList.Add(resultFormat);
                                }
                            }
                            resultFormatList.OrderBy(p => p.CourseName);
                            var courseLogic = new CourseLogic();

                            for (int i = 0; i < resultFormatList.Count; i++)
                            {
                                var course = new Course();
                                course.Department = new Department() { Id = viewModel.Department.Id };
                                course.Semester = new Semester() { Id = resultFormatList[i].SemesterId };
                                course.Type = new CourseType() { Id = viewModel.CourseType.Id };
                                course.Level = new Level() { Id = viewModel.level.Id };
                                course.Programme = new Programme() { Id = viewModel.programme.Id };
                                course.Code = resultFormatList[i].CourseCode?.ToUpper();
                                course.Name = resultFormatList[i].CourseName?.ToUpper();
                                course.Unit = resultFormatList[i].CourseUnit;
                                course.Activated = true;

                                var courseExists = courseLogic.GetModelsBy(c => c.Department_Id == course.Department.Id
                                    && c.Semester_Id == course.Semester.Id
                                    && c.Course_Type_Id == course.Type.Id
                                    && c.Course_Code == course.Code
                                    && c.Course_Name == course.Name
                                    && c.Course_Unit == course.Unit
                                    && c.Level_Id == course.Level.Id
                                    && c.Programme_Id == course.Programme.Id).FirstOrDefault();

                                if (courseExists == null)
                                {
                                    courseLogic.Create(course);
                                }
                            }

                            SetMessage("Course(s) upload complete", Message.Category.Information);
                        }
                    }
                }

                return RedirectToAction("UploadCoursesByExcelSheet");
            }
            catch (Exception ex) { throw ex; }
        }

        private DataSet ReadExcel(string filepath)
        {
            DataSet Result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" +
                                  "Extended Properties=Excel 8.0;";
                var connection = new OleDbConnection(xConnStr);

                connection.Open();
                DataTable sheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow dataRow in sheet.Rows)
                {
                    string sheetName = dataRow[2].ToString().Replace("'", "");
                    var command = new OleDbCommand("Select * FROM [" + sheetName + "]", connection);
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
            catch (Exception ex) { throw ex; }

            return Result;
        }
    }
}