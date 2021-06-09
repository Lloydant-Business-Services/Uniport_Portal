using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ProgrammeTranslator : TranslatorBase<Programme, PROGRAMME>
    {
        public override Programme TranslateToModel(PROGRAMME entity)
        {
            try
            {
                Programme programme = null;
                if (entity != null)
                {
                    programme = new Programme();
                    programme.Id = entity.Programme_Id;
                    programme.Name = entity.Programme_Name;
                    programme.Description = entity.Programme_Description;
                    programme.ShortName = entity.Programme_Short_Name;
                    programme.Activated = entity.Activated;
                }

                return programme;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PROGRAMME TranslateToEntity(Programme programme)
        {
            try
            {
                PROGRAMME entity = null;
                if (programme != null)
                {
                    entity = new PROGRAMME();
                    entity.Programme_Id = programme.Id;
                    entity.Programme_Name = programme.Name;
                    entity.Programme_Description = programme.Description;
                    entity.Programme_Short_Name = programme.ShortName;
                    entity.Activated = programme.Activated;
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