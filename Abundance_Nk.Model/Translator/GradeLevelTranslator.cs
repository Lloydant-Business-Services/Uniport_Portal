using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class GradeLevelTranslator : TranslatorBase<GradeLevel, GRADE_LEVEL>
    {
        public override GradeLevel TranslateToModel(GRADE_LEVEL entity)
        {
            try
            {
                GradeLevel model = null;
                if (entity != null)
                {
                    model = new GradeLevel();
                    model.Id = entity.Grade_Level_Id;
                    model.Name = entity.Grade_Level_Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override GRADE_LEVEL TranslateToEntity(GradeLevel model)
        {
            try
            {
                GRADE_LEVEL entity = null;
                if (model != null)
                {
                    entity = new GRADE_LEVEL();
                    entity.Grade_Level_Id = model.Id;
                    entity.Grade_Level_Name = model.Name;
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
