using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ScratchCardLogic : BusinessBaseLogic<ScratchCard, SCRATCH_CARD>
    {
        public ScratchCardLogic()
        {
            base.translator = new ScratchCardTranslator();
        }

        public ScratchCard GetBy(string pin)
        {
            try
            {
                Expression<Func<SCRATCH_CARD, bool>> selector = s => s.Pin == pin;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ScratchCard> GetBy(long PersonId ,int BatchId)
        {
            try
            {
                Expression<Func<SCRATCH_CARD, bool>> selector = s => s.Person_Id == PersonId && s.Scratch_Card_Batch_Id == BatchId;
                return GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool ValidatePin(string pin, FeeType fee_type)
        {
            try
            {
                Expression<Func<SCRATCH_CARD, bool>> selector =
                    s => s.Pin == pin && s.SCRATCH_CARD_BATCH.SCRATCH_CARD_TYPE.Fee_Type_Id == fee_type.Id;
                List<ScratchCard> scratchCardPayments = GetModelsBy(selector);
                if (scratchCardPayments != null && scratchCardPayments.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsPinUsed(string pin, long personId)
        {
            try
            {
                Expression<Func<SCRATCH_CARD, bool>> selector = s => s.Pin == pin && s.Person_Id != null;
                List<ScratchCard> scratchCardPayments = GetModelsBy(selector);
                if (scratchCardPayments != null && scratchCardPayments.Count > 0)
                {
                    Expression<Func<SCRATCH_CARD, bool>> expression = s => s.Pin == pin && s.Person_Id == personId;
                    scratchCardPayments = GetModelsBy(expression);
                    if (scratchCardPayments != null && scratchCardPayments.Count > 0)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsPinUsed(string pin, String jambNumber)
        {
            try
            {

                Expression<Func<SCRATCH_CARD, bool>> selector = s => s.Pin == pin && s.Person_Id != null;
                List<ScratchCard> scratchCardPayments = GetModelsBy(selector);
                if (scratchCardPayments != null && scratchCardPayments.Count > 0)
                {
                    //Get PersonId tied to pin
                    ApplicantJambDetailLogic applicantJambDetailLogic = new ApplicantJambDetailLogic();
                    var details = applicantJambDetailLogic.GetAllBy(jambNumber);
                    foreach (ApplicantJambDetail detail in details)
                    {
                        if (detail != null && detail.Person != null)
                        {
                            long personId = detail.Person.Id;
                            if (personId > 0)
                            {
                                Expression<Func<SCRATCH_CARD, bool>> expression = s => s.Pin == pin && s.Person_Id == personId;
                                scratchCardPayments = GetModelsBy(expression);
                                if (scratchCardPayments != null && scratchCardPayments.Count > 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;

                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdatePin(string pin, Person person)
        {
            try
            {
                Expression<Func<SCRATCH_CARD, bool>> selector = p => p.Pin == pin;
                SCRATCH_CARD scratchCardEntity = GetEntityBy(selector);

                if (scratchCardEntity == null || scratchCardEntity.Scratch_Card_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                if (scratchCardEntity.First_Used_Date == null)
                {
                    scratchCardEntity.First_Used_Date = DateTime.Now;
                }
                if (scratchCardEntity.Person_Id == null)
                {
                    scratchCardEntity.Person_Id = person.Id;
                }

                if (scratchCardEntity.Usage_Count == 0)
                {
                    scratchCardEntity.Usage_Count = scratchCardEntity.Usage_Count++;

                }


                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(string pin, Person model)
        {
            try
            {
                Expression<Func<SCRATCH_CARD, bool>> selector = p => p.Pin == pin;
                SCRATCH_CARD cardEntity = GetEntityBy(selector);

                if (cardEntity == null || cardEntity.Scratch_Card_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.Id > 0)
                {
                    cardEntity.Person_Id = model.Id;
                }


                int modifiedRecordCount = Save();
                if (modifiedRecordCount > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool DidUserBuyCard(long personId, ScratchCardBatch scratchCardBatch)
        {
            try
            {
                Expression<Func<SCRATCH_CARD, bool>> expression = s => s.Person_Id == personId && s.Scratch_Card_Batch_Id == scratchCardBatch.Id;
                var scratchCardPayments = GetModelsBy(expression);
                if (scratchCardPayments != null && scratchCardPayments.Count > 0)
                {
                    return true;
                }
                return false;
                
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
