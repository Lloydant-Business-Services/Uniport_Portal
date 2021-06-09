using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class DesignationTranslator : TranslatorBase<Designation, DESIGNATION>
    {
        public override Designation TranslateToModel(DESIGNATION entity)
        {
            try
            {
                Designation model = null;
                if (entity != null)
                {
                    model = new Designation();
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

        public override DESIGNATION TranslateToEntity(Designation model)
        {
            try
            {
                DESIGNATION entity = null;
                if (model != null)
                {
                    entity = new DESIGNATION();
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
