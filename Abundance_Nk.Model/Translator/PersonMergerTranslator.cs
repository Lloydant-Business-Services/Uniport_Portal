using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class PersonMergerTranslator : TranslatorBase<PersonMerger, PERSON_MERGER>
    {
        private readonly PersonTranslator personTranslator;

        public PersonMergerTranslator()
        {
            personTranslator = new PersonTranslator();
        }

        public override PersonMerger TranslateToModel(PERSON_MERGER entity)
        {
            try
            {
                PersonMerger model = null;
                if (entity != null)
                {
                    model = new PersonMerger();
                    model.PersonMergerId = entity.Person_Merger_Id;

                    model.OldPerson = personTranslator.Translate(entity.PERSON1);
                    model.NewPerson = personTranslator.Translate(entity.PERSON);

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PERSON_MERGER TranslateToEntity(PersonMerger model)
        {
            try
            {
                PERSON_MERGER entity = null;
                if (model != null)
                {
                    entity = new PERSON_MERGER();
                    entity.Person_Merger_Id = model.PersonMergerId;
                    if(model.OldPerson?.Id > 0)
                    {
                        entity.Old_Person_Id = model.OldPerson.Id;
                    }
                    if (model.NewPerson?.Id > 0)
                    {
                        entity.New_Person_Id = model.NewPerson.Id;
                    }



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
