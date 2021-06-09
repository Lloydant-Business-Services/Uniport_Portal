using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class TranscriptIncidentLogLogic : BusinessBaseLogic<TranscriptIncidentLog, TRANSCRIPT_INCIDENT_LOG>
    {
        public TranscriptIncidentLogLogic()
        {
            translator = new TranscriptIncidentLogTranslator();
        }
        public bool ModifyList(List<TranscriptIncidentLog> model)
        {
            try
            {
                if (model.Count > 0)
                {
                    for(int i = 0; i < model.Count; i++)
                    {
                        var Id = model[i].Id;
                        Expression<Func<TRANSCRIPT_INCIDENT_LOG, bool>> selector =c =>c.Id == Id;
                        var entity = GetEntityBy(selector);
                        if (entity != null)
                        {
                            entity.Status = model[i].Status;
                            entity.ClosedBy = model[i].ClosedUser.Id;
                            entity.Date_Closed = model[i].Date_Closed;
                            Save();

                        }
                    }
                    return true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return false;
        }
        public bool TicketIdExist(string ticket)
        {
            Expression<Func<TRANSCRIPT_INCIDENT_LOG, bool>> selector =
                    c =>
                        c.Ticket_Id == ticket;
            var exist=GetModelsBy(selector);
            if (exist != null)
            {
                return true;
            }
            return false;
        }
        public string ValidTicket()
        {
            var ticket = "";
            var valid = false;
            try
            {
                
                do
                {
                    ticket = UniqueCode();
                    valid = TicketIdExist(ticket);
                    if (!valid)
                        break;
                    return ticket;
                } while (valid);

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return ticket;
        }
        public string UniqueCode()
        {
            try
            {
                string str = "0123456789ABCDEFWXRYZMNOPQVLK";

                string code = "";

                code = Shuffle(str).Substring(0, 12).ToUpper();

                return code.ToUpper();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string Shuffle(string str)
        {
            char[] array = str.ToCharArray();
            Random rng = new Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }
    }
}
