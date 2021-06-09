using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class CourseAllocationTranslator : TranslatorBase<CourseAllocation, COURSE_ALLOCATION>
    {
        private readonly CourseTranslator courseTranslator;
        private readonly DepartmentTranslator departmentTranslator;
        private readonly LevelTranslator levelTranslator;
        private readonly ProgrammeTranslator programmeTranslator;
        private readonly SemesterTranslator semesterTranslator;
        private readonly SessionTranslator sessionTranslator;
        private readonly UserTranslator userTranslator;
        private readonly DepartmentOptionTranslator departmentOptionTranslator;

        public CourseAllocationTranslator()
        {
            courseTranslator = new CourseTranslator();
            userTranslator = new UserTranslator();
            sessionTranslator = new SessionTranslator();
            levelTranslator = new LevelTranslator();
            departmentTranslator = new DepartmentTranslator();
            programmeTranslator = new ProgrammeTranslator();
            semesterTranslator = new SemesterTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
        }

        public override CourseAllocation TranslateToModel(COURSE_ALLOCATION entity)
        {
            try
            {
                CourseAllocation model = null;
                if (entity != null)
                {
                    model = new CourseAllocation();
                    model.Id = entity.Course_Allocation_Id;
                    model.Course = courseTranslator.TranslateToModel(entity.COURSE);
                    model.Department = departmentTranslator.TranslateToModel(entity.DEPARTMENT);
                    model.Level = levelTranslator.TranslateToModel(entity.LEVEL);
                    model.Programme = programmeTranslator.TranslateToModel(entity.PROGRAMME);
                    model.Semester = semesterTranslator.TranslateToModel(entity.SEMESTER);
                    model.Session = sessionTranslator.TranslateToModel(entity.SESSION);
                    model.User = userTranslator.TranslateToModel(entity.USER);
                    model.IsDean = entity.Is_Dean;
                    model.IsHOD = entity.Is_HOD;
                    model.HodDepartment = departmentTranslator.Translate(entity.DEPARTMENT1);
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override COURSE_ALLOCATION TranslateToEntity(CourseAllocation model)
        {
            try
            {
                COURSE_ALLOCATION entity = null;
                if (model != null)
                {
                    entity = new COURSE_ALLOCATION();
                    entity.Course_Allocation_Id = model.Id;
                    entity.Course_Id = model.Course.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Semester_Id = model.Semester.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.User_Id = model.User.Id;
                    if (model.HodDepartment != null)
                    {
                        entity.HOD_Department_Id = model.HodDepartment.Id;
                    }
                    entity.Is_Dean = model.IsDean;
                    entity.Is_HOD = model.IsHOD;
                    if (model.DepartmentOption?.Id > 0)
                    {
                        entity.DepartmentOption_Id = model.DepartmentOption.Id;
                    }
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}