using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class AdmissionListTranslator : TranslatorBase<AdmissionList, ADMISSION_LIST>
    {
        private readonly AdmissionListBatchTranslator admissionListBatchTranslator;
        private readonly ApplicationFormTranslator applicationFormTranslator;
        private readonly DepartmentOptionTranslator departmentOptionTranslator;
        private readonly DepartmentTranslator departmentTranslator;
        private readonly ProgrammeTranslator programmeTranslator;

        public AdmissionListTranslator()
        {
            applicationFormTranslator = new ApplicationFormTranslator();
            admissionListBatchTranslator = new AdmissionListBatchTranslator();
            departmentTranslator = new DepartmentTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
            programmeTranslator = new ProgrammeTranslator();
        }

        public override AdmissionList TranslateToModel(ADMISSION_LIST entity)
        {
            try
            {
                AdmissionList model = null;
                if (entity != null)
                {
                    model = new AdmissionList();
                    model.Id = entity.Admission_List_Id;
                    model.Form = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    model.Batch = admissionListBatchTranslator.Translate(entity.ADMISSION_LIST_BATCH);
                    model.Deprtment = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Activated = entity.Activated ?? true;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ADMISSION_LIST TranslateToEntity(AdmissionList model)
        {
            try
            {
                ADMISSION_LIST entity = null;
                if (model != null)
                {
                    entity = new ADMISSION_LIST();
                    entity.Admission_List_Id = model.Id;
                    entity.Application_Form_Id = model.Form.Id;
                    entity.Admission_List_Batch_Id = model.Batch.Id;
                    entity.Department_Id = model.Deprtment.Id;
                    entity.Programme_Id = model.Programme.Id;
                    if (model.DepartmentOption != null)
                    {
                        entity.Department_Option_Id = model.DepartmentOption.Id;
                    }
                    entity.Activated = model.Activated;
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