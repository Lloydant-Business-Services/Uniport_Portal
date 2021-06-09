using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class AdmissionCriteriaForOLevelTypeTranslator :
        TranslatorBase<AdmissionCriteriaForOLevelType, ADMISSION_CRITERIA_FOR_O_LEVEL_TYPE>
    {
        private readonly AdmissionCriteriaTranslator admissionCriteriaTranslator;
        private readonly OLevelTypeTranslator oLevelTypeTranslator;

        public AdmissionCriteriaForOLevelTypeTranslator()
        {
            oLevelTypeTranslator = new OLevelTypeTranslator();
            admissionCriteriaTranslator = new AdmissionCriteriaTranslator();
        }

        public override AdmissionCriteriaForOLevelType TranslateToModel(ADMISSION_CRITERIA_FOR_O_LEVEL_TYPE entity)
        {
            try
            {
                AdmissionCriteriaForOLevelType model = null;
                if (entity != null)
                {
                    model = new AdmissionCriteriaForOLevelType();
                    model.Id = entity.Admission_Criteria_For_O_Level_Type_Id;
                    model.MainCriteria = admissionCriteriaTranslator.Translate(entity.ADMISSION_CRITERIA);
                    model.OLevelType = oLevelTypeTranslator.Translate(entity.O_LEVEL_TYPE);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ADMISSION_CRITERIA_FOR_O_LEVEL_TYPE TranslateToEntity(AdmissionCriteriaForOLevelType model)
        {
            try
            {
                ADMISSION_CRITERIA_FOR_O_LEVEL_TYPE entity = null;
                if (model != null)
                {
                    entity = new ADMISSION_CRITERIA_FOR_O_LEVEL_TYPE();
                    entity.Admission_Criteria_For_O_Level_Type_Id = model.Id;
                    entity.Admission_Criteria_Id = model.MainCriteria.Id;
                    entity.O_Level_Type_Id = model.OLevelType.Id;
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