using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class CardPaymentTranslator : TranslatorBase<CardPayment, CARD_PAYMENT>
    {
        private readonly PaymentTranslator paymentTranslator;
        private readonly ScratchCardTranslator scratchCardTranslator;

        public CardPaymentTranslator()
        {
            paymentTranslator = new PaymentTranslator();
            scratchCardTranslator = new ScratchCardTranslator();
        }

        public override CardPayment TranslateToModel(CARD_PAYMENT entity)
        {
            try
            {
                CardPayment model = null;
                if (entity != null)
                {
                    model = new CardPayment();
                    model.Payment = paymentTranslator.Translate(entity.PAYMENT);
                    model.Card = scratchCardTranslator.Translate(entity.SCRATCH_CARD);
                    model.TransactionDate = entity.Transaction_Date;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override CARD_PAYMENT TranslateToEntity(CardPayment model)
        {
            try
            {
                CARD_PAYMENT entity = null;
                if (model != null)
                {
                    entity = new CARD_PAYMENT();
                    entity.Payment_Id = model.Payment.Id;
                    entity.Scratch_Card_Id = model.Card.Id;
                    model.TransactionDate = entity.Transaction_Date;
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