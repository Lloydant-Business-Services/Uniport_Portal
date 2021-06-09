using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class UnitTranslator : TranslatorBase<Unit, UNIT>
    {
        public override Unit TranslateToModel(UNIT entity)
        {
            try
            {
                Unit model = null;
                if (entity != null)
                {
                    model = new Unit();
                    model.Active = entity.Active;
                    model.Id = entity.Id;
                    model.Name = entity.Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override UNIT TranslateToEntity(Unit model)
        {
            try
            {
                UNIT entity = null;
                if (model != null)
                {
                    entity = new UNIT();
                    entity.Id = model.Id;
                    entity.Active = model.Active;
                    entity.Name = model.Name;
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
