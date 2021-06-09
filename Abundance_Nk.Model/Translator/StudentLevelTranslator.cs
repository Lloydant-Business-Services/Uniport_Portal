using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentLevelTranslator : TranslatorBase<StudentLevel, STUDENT_LEVEL>
    {
        private readonly DepartmentOptionTranslator departmentOptionTranslator;
        private readonly DepartmentTranslator departmentTranslator;
        private readonly LevelTranslator levelTranslator;
        private readonly ProgrammeTranslator programmeTranslator;
        private readonly SessionTranslator sessionTranslator;
        private readonly StudentTranslator studentTranslator;
        //private SessionSemesterTranslator sessionSemesterTranslator;

        public StudentLevelTranslator()
        {
            levelTranslator = new LevelTranslator();
            studentTranslator = new StudentTranslator();
            sessionTranslator = new SessionTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
            programmeTranslator = new ProgrammeTranslator();
            departmentTranslator = new DepartmentTranslator();
        }

        public override StudentLevel TranslateToModel(STUDENT_LEVEL entity)
        {
            try
            {
                StudentLevel model = null;
                if (entity != null)
                {
                    model = new StudentLevel();
                    model.Id = entity.Student_Level_Id;
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_LEVEL TranslateToEntity(StudentLevel model)
        {
            try
            {
                STUDENT_LEVEL entity = null;
                if (model != null)
                {
                    entity = new STUDENT_LEVEL();
                    entity.Student_Level_Id = model.Id;
                    entity.Person_Id = model.Student.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Department_Id = model.Department.Id;

                    if (model.DepartmentOption != null && model.DepartmentOption.Id > 0)
                    {
                        entity.Department_Option_Id = model.DepartmentOption.Id;
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