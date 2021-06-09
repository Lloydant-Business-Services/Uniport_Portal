using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ProgrammeDepartmentTranslator : TranslatorBase<ProgrammeDepartment, PROGRAMME_DEPARTMENT>
    {
        private readonly DepartmentTranslator departmentTranslator;
        private readonly ProgrammeTranslator programmeTranslator;

        public ProgrammeDepartmentTranslator()
        {
            programmeTranslator = new ProgrammeTranslator();
            departmentTranslator = new DepartmentTranslator();
        }

        public override ProgrammeDepartment TranslateToModel(PROGRAMME_DEPARTMENT entity)
        {
            try
            {
                ProgrammeDepartment model = null;
                if (entity != null)
                {
                    model = new ProgrammeDepartment();
                    //model.Id = entity.Department_Id;
                    model.Id = entity.Programme_Department_Id;
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Activate = entity.Activate;
                    model.ActivePUTMEApplication = entity.Active_PUTME_Application;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PROGRAMME_DEPARTMENT TranslateToEntity(ProgrammeDepartment model)
        {
            try
            {
                PROGRAMME_DEPARTMENT entity = null;
                if (model != null)
                {
                    entity = new PROGRAMME_DEPARTMENT();
                    // entity.Department_Id = model.Id;
                    entity.Programme_Department_Id = model.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Activate = model.Activate;
                    entity.Active_PUTME_Application = model.ActivePUTMEApplication;
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