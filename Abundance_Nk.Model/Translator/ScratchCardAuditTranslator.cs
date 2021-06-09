using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{

    public class ScratchCardAuditTranslator : TranslatorBase<ScratchCardAudit, SCRATCH_CARD_AUDIT>
    {
        private readonly PersonTranslator personTranslator;
        private readonly UserTranslator userTranslator;
        private readonly ScratchCardTranslator scratchCardTranslator;

        public ScratchCardAuditTranslator()
        {
            userTranslator = new UserTranslator();
            personTranslator = new PersonTranslator();
            scratchCardTranslator = new ScratchCardTranslator();
        }


        public override SCRATCH_CARD_AUDIT TranslateToEntity(ScratchCardAudit model)
        {
            try
            {
                SCRATCH_CARD_AUDIT entity = null;
                if (model != null)
                {
                    entity = new SCRATCH_CARD_AUDIT();
                    entity.Person_Id = model.NewPerson.Id;
                    entity.Old_Person_Id = model.OldPerson.Id;
                    entity.Pin = model.Pin;
                    entity.Scratch_Card_Id = model.ScratchCard.Id;
                    entity.Action = model.Action;
                    entity.Operation = model.Operation;
                    entity.Client = model.Client;
                    entity.Serial_Number = model.SerialNumber;
                    entity.User_Id = model.User.Id;
                    entity.Time = model.Time;

                }
                return entity;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public override ScratchCardAudit TranslateToModel(SCRATCH_CARD_AUDIT entity)
        {
            try
            {
                ScratchCardAudit model = null;
                if (entity != null)
                {
                    model = new ScratchCardAudit();
                    model.Id = entity.Scratch_Card_Id;
                    model.NewPerson = personTranslator.Translate(entity.PERSON);
                    model.OldPerson = personTranslator.Translate(entity.PERSON1);
                    model.Pin = entity.Pin;
                    model.ScratchCard = scratchCardTranslator.Translate(entity.SCRATCH_CARD);
                    model.Action = entity.Action;
                    model.Operation = entity.Operation;
                    model.Client = entity.Client;
                    model.SerialNumber = entity.Serial_Number;
                    model.User = userTranslator.Translate(entity.USER);
                    model.Time = entity.Time;
                }
                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
