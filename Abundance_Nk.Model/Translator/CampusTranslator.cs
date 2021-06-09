using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class CampusTranslator : TranslatorBase<Campus, CAMPUS>
    {
        public override Campus TranslateToModel(CAMPUS entity)
        {
            try
            {
                Campus model = null;
                if (entity != null)
                {
                    model = new Campus();
                    model.Active = entity.Active;
                    model.Description = entity.Description;
                    model.Id = entity.Campus_Id;
                    model.Name = entity.Name;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override CAMPUS TranslateToEntity(Campus model)
        {
            try
            {
                CAMPUS entity = null;
                if (model != null)
                {
                    entity = new CAMPUS();
                    entity.Active = model.Active;
                    entity.Campus_Id = model.Id;
                    entity.Description = model.Description;
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
