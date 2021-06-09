using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class RefereeGradingCategoryTranslator : TranslatorBase<RefereeGradingCategory, REFEREE_GRADING_CATEGORY>
    {
        public override RefereeGradingCategory TranslateToModel(REFEREE_GRADING_CATEGORY entity)
        {
            try
            {
                RefereeGradingCategory model = null;
                if (entity != null)
                {
                    model = new RefereeGradingCategory();
                    model.Id = entity.Id;
                    model.GradeCategory = entity.Grade_Category;
                    model.Active = entity.Active;
                    
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override REFEREE_GRADING_CATEGORY TranslateToEntity(RefereeGradingCategory model)
        {
            try
            {
                REFEREE_GRADING_CATEGORY entity = null;
                if (model != null)
                {
                    entity = new REFEREE_GRADING_CATEGORY();
                    entity.Id = model.Id;
                    entity.Grade_Category = model.GradeCategory;
                    entity.Active = model.Active;
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
