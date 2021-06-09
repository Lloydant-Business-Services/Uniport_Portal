using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class EducationalQualificationTranslator :
        TranslatorBase<EducationalQualification, EDUCATIONAL_QUALIFICATION>
    {
        public override EducationalQualification TranslateToModel(EDUCATIONAL_QUALIFICATION entity)
        {
            try
            {
                EducationalQualification model = null;
                if (entity != null)
                {
                    model = new EducationalQualification();
                    model.Id = entity.Educational_Qualification_Id;
                    model.ShortName = entity.Educational_Qualification_Abbreviation;
                    model.Name = entity.Educational_Qualification_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override EDUCATIONAL_QUALIFICATION TranslateToEntity(EducationalQualification model)
        {
            try
            {
                EDUCATIONAL_QUALIFICATION entity = null;
                if (model != null)
                {
                    entity = new EDUCATIONAL_QUALIFICATION();
                    entity.Educational_Qualification_Id = model.Id;
                    entity.Educational_Qualification_Abbreviation = model.ShortName;
                    entity.Educational_Qualification_Name = model.Name;
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