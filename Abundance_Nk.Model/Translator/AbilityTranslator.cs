using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class AbilityTranslator : TranslatorBase<Ability, ABILITY>
    {
        public override Ability TranslateToModel(ABILITY entity)
        {
            try
            {
                Ability model = null;
                if (entity != null)
                {
                    model = new Ability();
                    model.Id = entity.Ability_Id;
                    model.Name = entity.Ability_Name;
                    model.Description = entity.Ability_Description;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override ABILITY TranslateToEntity(Ability model)
        {
            try
            {
                ABILITY entity = null;
                if (model != null)
                {
                    entity = new ABILITY();
                    entity.Ability_Id = model.Id;
                    entity.Ability_Name = model.Name;
                    entity.Ability_Description = model.Description;
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