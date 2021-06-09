using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class SexTranslator : TranslatorBase<Sex, SEX>
    {
        public override Sex TranslateToModel(SEX sexEntity)
        {
            try
            {
                Sex sex = null;
                if (sexEntity != null)
                {
                    sex = new Sex();
                    sex.Id = sexEntity.Sex_Id;
                    sex.Name = sexEntity.Sex_Name;
                }

                return sex;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SEX TranslateToEntity(Sex sex)
        {
            try
            {
                SEX entity = null;
                if (sex != null)
                {
                    entity = new SEX();
                    entity.Sex_Id = sex.Id;
                    entity.Sex_Name = sex.Name;
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