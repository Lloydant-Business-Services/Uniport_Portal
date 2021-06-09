using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class TranscriptRequestTranslator : TranslatorBase<TranscriptRequest, TRANSCRIPT_REQUEST>
    {
        private readonly CountryTranslator countryTranslator;
        private readonly PaymentTranslator paymentTranslator;
        private readonly StateTranslator stateTranslator;
        private readonly StudentTranslator studentTranslator;
        private readonly TranscriptClearanceStatusTranslator transcriptClearanceStatusTranslator;
        private readonly TranscriptStatusTranslator transcriptStatusTranslator;
        private readonly DeliveryServiceZoneTranslator deliveryServiceZoneTranslator;

        public TranscriptRequestTranslator()
        {
            studentTranslator = new StudentTranslator();
            paymentTranslator = new PaymentTranslator();
            countryTranslator = new CountryTranslator();
            stateTranslator = new StateTranslator();
            transcriptClearanceStatusTranslator = new TranscriptClearanceStatusTranslator();
            transcriptStatusTranslator = new TranscriptStatusTranslator();
            deliveryServiceZoneTranslator = new DeliveryServiceZoneTranslator();
        }

        public override TranscriptRequest TranslateToModel(TRANSCRIPT_REQUEST entity)
        {
            try
            {
                TranscriptRequest model = null;
                if (entity != null)
                {
                    model = new TranscriptRequest();
                    model.Id = entity.Transcript_Request_Id;
                    if (entity.PAYMENT != null)
                    {
                        model.payment = paymentTranslator.Translate(entity.PAYMENT);
                    }
                    model.student = studentTranslator.Translate(entity.STUDENT);
                    model.DateRequested = entity.Date_Requested;
                    model.DestinationAddress = entity.Destination_Address;
                    model.DestinationCountry = countryTranslator.Translate(entity.COUNTRY);
                    model.DestinationState = stateTranslator.Translate(entity.STATE);
                    model.transcriptClearanceStatus =
                        transcriptClearanceStatusTranslator.Translate(entity.TRANSCRIPT_CLEARANCE_STATUS);
                    model.transcriptStatus = transcriptStatusTranslator.Translate(entity.TRANSCRIPT_STATUS);
                    model.DeliveryServiceZone = deliveryServiceZoneTranslator.Translate(entity.DELIVERY_SERVICE_ZONE);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override TRANSCRIPT_REQUEST TranslateToEntity(TranscriptRequest model)
        {
            try
            {
                TRANSCRIPT_REQUEST entity = null;
                if (model != null)
                {
                    entity = new TRANSCRIPT_REQUEST();
                    entity.Transcript_Request_Id = model.Id;
                    if (model.payment != null)
                    {
                        entity.Payment_Id = model.payment.Id;
                    }

                    entity.Student_id = model.student.Id;
                    entity.Date_Requested = model.DateRequested;
                    entity.Destination_Address = model.DestinationAddress;
                    entity.Destination_Country_Id = model.DestinationCountry.Id;
                    entity.Destination_State_Id = model.DestinationState.Id;

                    entity.Transcript_clearance_Status_Id = model.transcriptClearanceStatus.TranscriptClearanceStatusId;
                    entity.Transcript_Status_Id = model.transcriptStatus.TranscriptStatusId;
                    if (model.DeliveryServiceZone != null && model.DeliveryServiceZone.Id > 0)
                    {
                        entity.Delivery_Service_Zone_Id = model.DeliveryServiceZone.Id;
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