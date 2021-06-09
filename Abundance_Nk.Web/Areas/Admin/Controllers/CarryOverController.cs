﻿using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class CarryOverController :BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private CourseRegistrationDetailLogic courseRegistrationDetailLogic;
        private CourseRegistrationLogic courseRegistrationLogic;
        private DepartmentLogic departmentLogic;
        private ProgrammeLogic programmeLogic;
        private StudentLogic studentLogic;

        public CarryOverController()
        {
            courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
            courseRegistrationLogic = new CourseRegistrationLogic();
            studentLogic = new StudentLogic();
            programmeLogic = new ProgrammeLogic();
            departmentLogic = new DepartmentLogic();
        }

        public ActionResult Index()
        {
            var viewModel = new CarryOverViewModel();

            try
            {
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(),"Id","Name");
                ViewBag.SessionId = viewModel.SessionSelectListItem;
                ViewBag.LevelId = viewModel.LevelSelectListItem;
            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewCarryOverStudents(CarryOverViewModel viewModel)
        {
            try
            {
                ViewBag.deptId = viewModel.Department.Id.ToString();
                ViewBag.progId = viewModel.Programme.Id.ToString();
                ViewBag.sessionId = viewModel.Session.Id.ToString();
                ViewBag.semesterId = viewModel.Semester.Id.ToString();
                ViewBag.levelId = viewModel.Level.Id.ToString();
                return View();
            }
            catch(Exception ex)
            {
                SetMessage("Error" + ex.Message,Message.Category.Error);
                ;
            }

            return View();
        }

        public ActionResult CarryOverStudentsByCourse()
        {
            var viewModel = new CarryOverViewModel();
            try
            {
                ViewBag.Session = viewModel.SessionSelectListItem;
                ViewBag.Semester = new SelectList(new List<Semester>(),ID,NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectListItem;
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.Department = new SelectList(new List<Department>(),ID,NAME);
                ViewBag.Course = new SelectList(new List<Course>(),ID,NAME);
            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CarryOverStudentsByCourseReport(CarryOverViewModel viewModel)
        {
            try
            {
                ViewBag.SessionId = viewModel.Session.Id.ToString();
                ViewBag.SemesterId = viewModel.Semester.Id.ToString();
                ViewBag.ProgrammeId = viewModel.Programme.Id.ToString();
                ViewBag.DepartmentId = viewModel.Department.Id.ToString();
                ViewBag.LevelId = viewModel.Level.Id.ToString();
                ViewBag.CourseId = viewModel.Course.Id.ToString();
            }
            catch(Exception ex)
            {
                SetMessage("Error" + ex.Message,Message.Category.Error);
                ;
            }

            return View();
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

                return Json(new SelectList(semesters,ID,NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw;
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
                var semester = new Semester { Id = Convert.ToInt32(ids[1]) };
                var programme = new Programme { Id = Convert.ToInt32(ids[2]) };
                List<Course> courseList = Utility.GetCoursesByLevelDepartmentAndSemester(level,department,semester,
                    programme);

                return Json(new SelectList(courseList,ID,NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}