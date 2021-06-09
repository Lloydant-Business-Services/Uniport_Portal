using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseLogic : BusinessBaseLogic<Course, COURSE>
    {
        public CourseLogic()
        {
            translator = new CourseTranslator();
        }

        public List<Course> GetOnlyRegisteredCourses(Department department, Level level, Semester semester,
            Programme programme, Session session)
        {
            try
            {
                List<Course> courses =
                    (from a in
                        repository.GetBy<VW_REGISTERED_COURSES>(
                            a =>
                                a.Department_Id == department.Id && a.Programme_Id == programme.Id &&
                                a.Level_Id == level.Id && a.Semester_Id == semester.Id && a.Session_Id == session.Id)
                        select new Course
                        {
                            Id = a.Course_Id,
                            Name = a.Course_Code + '-' + a.Course_Name
                        }).GroupBy(x => new {x.Id, x.Name}).Select(x => x.FirstOrDefault()).ToList();
                return courses;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Course> GetOnlyRegisteredCourses(Department department, Level level, Semester semester,
    Programme programme, Session session, DepartmentOption departmentOption)
        {
            try
            {
                List<Course> courses = new List<Course>();
                if (departmentOption?.Id > 0)
                {
                    courses = (from a in
                          repository.GetBy<VW_REGISTERED_COURSES>(
                              a =>
                                  a.Department_Id == department.Id && a.Programme_Id == programme.Id &&
                                  a.Level_Id == level.Id && a.Semester_Id == semester.Id && a.Session_Id == session.Id && a.Department_Option_Id==departmentOption.Id)
                               select new Course
                               {
                                   Id = a.Course_Id,
                                   Name = a.Course_Code + '-' + a.Course_Name
                               }).GroupBy(x => new { x.Id, x.Name }).Select(x => x.FirstOrDefault()).ToList();
                }
                else
                {
                    courses=(from a in
                        repository.GetBy<VW_REGISTERED_COURSES>(
                            a =>
                                a.Department_Id == department.Id && a.Programme_Id == programme.Id &&
                                a.Level_Id == level.Id && a.Semester_Id == semester.Id && a.Session_Id == session.Id)
                     select new Course
                     {
                         Id = a.Course_Id,
                         Name = a.Course_Code + '-' + a.Course_Name
                     }).GroupBy(x => new { x.Id, x.Name }).Select(x => x.FirstOrDefault()).ToList();
                }
                return courses;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetBy(Department department, Level level, Semester semester, Programme programme)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector =
                    c =>
                        c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id &&
                        c.Programme_Id == programme.Id && c.Activated;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetBy(Department department, Level level, Semester semester, Programme programme, DepartmentOption departmentOption)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector;
                if (departmentOption?.Id > 0)
                {
                    selector =
                    c =>
                        c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id &&
                        c.Programme_Id == programme.Id && c.Department_Option_Id==departmentOption.Id && c.Activated;
                }
                else
                {
                    selector =
                    c =>
                        c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id &&
                        c.Programme_Id == programme.Id && c.Activated;
                }
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetBy(Department department, DepartmentOption departmentOption, Level level,
            Semester semester, Programme programme)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector =
                    c =>
                        c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id &&
                        c.Department_Option_Id == departmentOption.Id && c.Programme_Id == programme.Id;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetBy(Department department, Level level, Semester semester, Programme programme,
            bool status)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector =
                    c =>
                        c.Department_Id == department.Id && c.Level_Id == level.Id && c.Semester_Id == semester.Id &&
                        c.Programme_Id == programme.Id && c.Activated == status;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetBy(Department department, DepartmentOption departmentOption, Level level,
            Semester semester, Programme programme, bool status)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector =
                    c =>
                        c.Department_Id == department.Id && c.Department_Option_Id == departmentOption.Id &&
                        c.Level_Id == level.Id && c.Semester_Id == semester.Id && c.Programme_Id == programme.Id &&
                        c.Activated == status;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetExtraYearBy(Department department, Level level, Semester semester, bool status)
        {
            try
            {
                if (level.Id <= 2)
                {
                    Expression<Func<COURSE, bool>> selector =
                        c =>
                            c.Department_Id == department.Id && c.Level_Id <= level.Id && c.Semester_Id == semester.Id &&
                            c.Activated == status;
                    return base.GetModelsBy(selector);
                }
                else
                {
                    Expression<Func<COURSE, bool>> selector =
                        c =>
                            c.Department_Id == department.Id && c.Level_Id > 2 && c.Semester_Id == semester.Id &&
                            c.Activated == status;
                    return base.GetModelsBy(selector);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Course> GetExtraYearBy(Department department, DepartmentOption departmentOption, Level level,
            Semester semester, bool status)
        {
            try
            {
                if (level.Id <= 2)
                {
                    Expression<Func<COURSE, bool>> selector =
                        c =>
                            c.Department_Id == department.Id && c.Department_Option_Id == departmentOption.Id &&
                            c.Level_Id <= level.Id && c.Semester_Id == semester.Id && c.Activated == status;
                    return base.GetModelsBy(selector);
                }
                else
                {
                    Expression<Func<COURSE, bool>> selector =
                        c =>
                            c.Department_Id == department.Id && c.Department_Option_Id == departmentOption.Id &&
                            c.Level_Id > 2 && c.Semester_Id == semester.Id && c.Activated == status;
                    return base.GetModelsBy(selector);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public Course GetBy(long id)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector = c => c.Course_Id == id;
                return base.GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(List<Course> courses)
        {
            try
            {
                foreach (Course course in courses)
                {
                    int modified = 0;
                    Expression<Func<COURSE, bool>> selector = c => c.Course_Id == course.Id;
                    COURSE courseEntity = GetEntityBy(selector);
                    if (courseEntity == null && course.Code != null && course.Name != null)
                    {
                        var newCourse = new Course();
                        newCourse.Name = course.Name;
                        newCourse.Code = course.Code;
                        newCourse.Department = course.Department;
                        if (course.DepartmentOption.Id > 0)
                        {
                            newCourse.DepartmentOption = course.DepartmentOption;
                        }
                        newCourse.IsRegistered = false;
                        newCourse.Level = course.Level;
                        newCourse.Semester = course.Semester;
                        newCourse.Type = course.Type;
                        newCourse.Programme = course.Programme;
                        newCourse.Unit = course.Unit;
                        newCourse.Activated = true;
                        Create(newCourse);
                    }
                    else
                    {
                        if (course.Code == null && course.Unit <= 0 && course.Name == null)
                        {
                            Delete(selector);
                        }
                        else
                        {
                            courseEntity.Course_Code = course.Code;
                            courseEntity.Course_Unit = course.Unit;
                            courseEntity.Course_Name = course.Name;
                            courseEntity.Course_Type_Id = course.Type.Id;
                            modified = Save();
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool Modify(Course course)
        {
            try
            {
                int modified = 0;
                Expression<Func<COURSE, bool>> selector = c => c.Course_Id == course.Id;
                COURSE courseEntity = GetEntityBy(selector);
                if (courseEntity == null)
                {
                    var newCourse = new Course();
                    newCourse.Name = course.Name;
                    newCourse.Code = course.Code;
                    newCourse.Department = course.Department;
                    newCourse.IsRegistered = false;
                    newCourse.Level = course.Level;
                    newCourse.Semester = course.Semester;
                    newCourse.Programme = course.Programme;
                    newCourse.Type = new CourseType {Id = 1};
                    newCourse.Unit = course.Unit;
                    Create(newCourse);
                    return true;
                }
                courseEntity.Course_Code = course.Code;
                courseEntity.Course_Unit = course.Unit;
                courseEntity.Course_Name = course.Name;
                modified = Save();
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        public List<Course> GetBy(Department department, Level level, Programme programme, DepartmentOption departmentOption)
        {
            try
            {
                Expression<Func<COURSE, bool>> selector;
                if (departmentOption?.Id > 0)
                {
                    selector =
                    c =>
                        c.Department_Id == department.Id && c.Level_Id == level.Id && 
                        c.Programme_Id == programme.Id && c.Department_Option_Id == departmentOption.Id;
                }
                else
                {
                    selector =
                    c =>
                        c.Department_Id == department.Id && c.Level_Id == level.Id &&
                        c.Programme_Id == programme.Id;
                }
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ActivateDeactivateCourse(Course course)
        {
            try
            {
                int modified = 0;
                Expression<Func<COURSE, bool>> selector = c => c.Course_Id == course.Id;
                COURSE courseEntity = GetEntityBy(selector);
                courseEntity.Activated = (bool)course.Activated;
                
                modified = Save();
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }


    }
}