using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class CourseUnitTranslator : TranslatorBase<CourseUnit, COURSE_UNIT>
    {
        private readonly DepartmentTranslator departmentTranslator;
        private readonly LevelTranslator levelTranslator;
        private readonly ProgrammeTranslator programmeTranslator;
        private readonly SemesterTranslator semesterTranslator;

        public CourseUnitTranslator()
        {
            levelTranslator = new LevelTranslator();
            departmentTranslator = new DepartmentTranslator();
            semesterTranslator = new SemesterTranslator();
            programmeTranslator = new ProgrammeTranslator();
        }

        public override CourseUnit TranslateToModel(COURSE_UNIT entity)
        {
            try
            {
                CourseUnit model = null;
                if (entity != null)
                {
                    model = new CourseUnit();
                    model.Id = entity.Course_Unit_Id;
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Semester = semesterTranslator.Translate(entity.SEMESTER);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.MinimumUnit = entity.Minimum_Unit;
                    model.MaximumUnit = entity.Maximum_Unit;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override COURSE_UNIT TranslateToEntity(CourseUnit model)
        {
            try
            {
                COURSE_UNIT entity = null;
                if (model != null)
                {
                    entity = new COURSE_UNIT();
                    entity.Course_Unit_Id = model.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Semester_Id = model.Semester.Id;
                    entity.Minimum_Unit = model.MinimumUnit;
                    entity.Maximum_Unit = model.MaximumUnit;
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