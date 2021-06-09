using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentAssignedMatricNumberTranslator : TranslatorBase<StudentAssignedMatricNumber, STUDENT_ASSIGNED_MATRIC_NUMBER>
    {
        private PersonTranslator personTranslator;
        private ProgrammeTranslator programmeTranslator;
        private SessionTranslator sessionTranslator;

        public StudentAssignedMatricNumberTranslator()
        {
            personTranslator = new PersonTranslator();
            programmeTranslator = new ProgrammeTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override StudentAssignedMatricNumber TranslateToModel(STUDENT_ASSIGNED_MATRIC_NUMBER entity)
        {
            try
            {
                StudentAssignedMatricNumber model = null;
                if (entity != null)
                {
                    model = new StudentAssignedMatricNumber();
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.StudentMatricNumber = entity.Student_Matric_Number;
                    model.StudentNumber = entity.Student_Number;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_ASSIGNED_MATRIC_NUMBER TranslateToEntity(StudentAssignedMatricNumber model)
        {
            try
            {
                STUDENT_ASSIGNED_MATRIC_NUMBER entity = null;
                if (model != null)
                {
                    entity = new STUDENT_ASSIGNED_MATRIC_NUMBER();
                    entity.Student_Number = model.StudentNumber;
                    entity.Session_Id = model.Session.Id;
                    entity.Student_Matric_Number = model.StudentMatricNumber;
                    entity.Person_Id = model.Person.Id;
                    entity.Programme_Id = model.Programme.Id;
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
