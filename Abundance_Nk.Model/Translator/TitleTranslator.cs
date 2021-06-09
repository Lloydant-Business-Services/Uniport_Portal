using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class TitleTranslator : TranslatorBase<Title, TITLE>
    {
        public override Title TranslateToModel(TITLE entity)
        {
            try
            {
                Title model = null;
                if (entity != null)
                {
                    model = new Title();
                    model.Id = entity.Title_Id;
                    model.Name = entity.Title_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override TITLE TranslateToEntity(Title model)
        {
            try
            {
                TITLE entity = null;
                if (model != null)
                {
                    entity = new TITLE();
                    entity.Title_Id = model.Id;
                    entity.Title_Name = model.Name;
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