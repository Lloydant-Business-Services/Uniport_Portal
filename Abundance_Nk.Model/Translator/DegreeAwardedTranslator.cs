using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class DegreeAwardedTranslator:TranslatorBase<DegreeAwarded,DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT>
    {
        private DepartmentTranslator departmentTranslator;
        private ProgrammeTranslator programmeTranslator;

        public DegreeAwardedTranslator()
        {
            departmentTranslator = new DepartmentTranslator();
            programmeTranslator = new ProgrammeTranslator();
        }
        public override DegreeAwarded TranslateToModel(DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT entity)
        {
            try
            {
                DegreeAwarded model = null;
                if (entity != null)
                {
                    model = new DegreeAwarded();
                    model.Id = entity.Id;
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Degree = entity.Degree_Name;
                    model.Duration = entity.Duration;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT TranslateToEntity(DegreeAwarded model)
        {
           try
            {
                DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT entity = null;
                if (model != null)
                {
                    entity = new DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT();
                    entity.Id = model.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Degree_Name = model.Degree;
                    entity.Duration = model.Duration;
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
