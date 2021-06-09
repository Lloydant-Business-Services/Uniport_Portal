using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class NationalityTranslator : TranslatorBase<Nationality, NATIONALITY>
    {
        public override Nationality TranslateToModel(NATIONALITY nationalityEntity)
        {
            try
            {
                Nationality nationality = null;
                if (nationalityEntity != null)
                {
                    nationality = new Nationality();
                    nationality.Id = nationalityEntity.Nationality_Id;
                    nationality.Name = nationalityEntity.Nationality_Name;
                }

                return nationality;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override NATIONALITY TranslateToEntity(Nationality nationality)
        {
            try
            {
                NATIONALITY nationalityEntity = null;
                if (nationality != null)
                {
                    nationalityEntity = new NATIONALITY();
                    nationalityEntity.Nationality_Id = nationality.Id;
                    nationalityEntity.Nationality_Name = nationality.Name;
                }

                return nationalityEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}