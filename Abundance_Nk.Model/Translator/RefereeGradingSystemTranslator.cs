using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class RefereeGradingSystemTranslator : TranslatorBase<RefereeGradingSystem, REFEREE_GRADING_SYSTEM>
    {
        public override RefereeGradingSystem TranslateToModel(REFEREE_GRADING_SYSTEM entity)
        {
            try
            {
                RefereeGradingSystem model = null;
                if (entity != null)
                {
                    model = new RefereeGradingSystem();
                    model.Id = entity.Id;
                    model.Score = entity.Score;
                    model.Active = entity.Active;
                    model.Type = entity.Type;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override REFEREE_GRADING_SYSTEM TranslateToEntity(RefereeGradingSystem model)
        {
            try
            {
                REFEREE_GRADING_SYSTEM entity = null;
                if (model != null)
                {
                    entity = new REFEREE_GRADING_SYSTEM();
                    entity.Id = model.Id;
                    entity.Score = model.Score;
                    entity.Active = model.Active;
                    entity.Type = model.Type;
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
