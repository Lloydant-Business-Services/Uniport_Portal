using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class CourseRegistrationTranslator : TranslatorBase<CourseRegistration, STUDENT_COURSE_REGISTRATION>
    {
        private readonly DepartmentTranslator departmentTranslator;
        private readonly DepartmentOptionTranslator departmentOptionTranslator;
        private readonly LevelTranslator levelTranslator;
        private readonly ProgrammeTranslator programmeTranslator;
        private readonly SessionTranslator sessionTranslator;
        private readonly StaffTranslator staffTranslator;
        private readonly StudentTranslator studentTranslator;

        public CourseRegistrationTranslator()
        {
            levelTranslator = new LevelTranslator();
            studentTranslator = new StudentTranslator();
            departmentTranslator = new DepartmentTranslator();
            programmeTranslator = new ProgrammeTranslator();
            sessionTranslator = new SessionTranslator();
            staffTranslator = new StaffTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
        }

        public override CourseRegistration TranslateToModel(STUDENT_COURSE_REGISTRATION entity)
        {
            try
            {
                CourseRegistration model = null;
                if (entity != null)
                {
                    model = new CourseRegistration();
                    model.Id = entity.Student_Course_Registration_Id;
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Approved = entity.Approved;
                   // model.Approver = staffTranslator.Translate(entity.STAFF);
                    model.DateApproved = entity.Date_Approved;
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_COURSE_REGISTRATION TranslateToEntity(CourseRegistration model)
        {
            try
            {
                STUDENT_COURSE_REGISTRATION entity = null;
                if (model != null)
                {
                    entity = new STUDENT_COURSE_REGISTRATION();
                    entity.Student_Course_Registration_Id = model.Id;
                    entity.Person_Id = model.Student.Id;
                    entity.Level_Id = model.Level.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Approved = model.Approved;
                    entity.Date_Approved = model.DateApproved;

                    if (model.Approver != null && model.Approver.Id > 0)
                    {
                        entity.Approver_Id = model.Approver.Id;
                    }
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