using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class TranscriptIncidentLogTranslator :
        TranslatorBase<TranscriptIncidentLog, TRANSCRIPT_INCIDENT_LOG>
    {
        private readonly UserTranslator userTranslator;
        private readonly TranscriptRequestTranslator transcriptRequestTranslator;
        private readonly DepartmentTranslator departmentTranslator;


        public TranscriptIncidentLogTranslator()
        {
            transcriptRequestTranslator = new TranscriptRequestTranslator();
            userTranslator = new UserTranslator();
            departmentTranslator = new DepartmentTranslator();
        }

        public override TranscriptIncidentLog TranslateToModel(TRANSCRIPT_INCIDENT_LOG entity)
        {
            try
            {
                TranscriptIncidentLog model = null;
                if (entity != null)
                {
                    model = new TranscriptIncidentLog();
                    model.Id = entity.Id;
                    model.Phone_No = entity.Phone_No;
                    model.Status = entity.Status;
                    model.TicketId = entity.Ticket_Id;
                    //model.TranscriptRequest = transcriptRequestTranslator.Translate(entity.TRANSCRIPT_REQUEST);
                    //model.LoggedUser = userTranslator.Translate(entity.USER);
                    //model.ClosedUser = userTranslator.Translate(entity.USER1);
                    model.Date_Closed = entity.Date_Closed;
                    model.Date_Opened = entity.Date_Opened;
                    model.Description = entity.Description;
                    model.Email = entity.Email;
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override TRANSCRIPT_INCIDENT_LOG TranslateToEntity(TranscriptIncidentLog model)
        {
            try
            {
                TRANSCRIPT_INCIDENT_LOG entity = null;
                if (model != null)
                {
                    entity = new TRANSCRIPT_INCIDENT_LOG();
                    entity.Id = model.Id;
                    entity.LoggedBy = model.LoggedUser.Id;
                    entity.Phone_No = model.Phone_No;
                    entity.Status = model.Status;
                    entity.Ticket_Id = model.TicketId;
                    entity.Transcript_Request_Id = model.TranscriptRequest.Id;
                    if (model.ClosedUser != null)
                    {
                        entity.ClosedBy = model.ClosedUser.Id;
                    }
                    
                    entity.Date_Closed = model.Date_Closed;
                    entity.Date_Opened = model.Date_Opened;
                    entity.Description = model.Description;
                    entity.Email = model.Email;
                    entity.Department_Id = model.Department.Id;

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
