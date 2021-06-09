using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class SponsorLogic : BusinessBaseLogic<Sponsor, APPLICANT_SPONSOR>
    {
        public SponsorLogic()
        {
            translator = new SponsorTranslator();
        }

        public bool Modify(Sponsor sponsor)
        {
            try
            {
                APPLICANT_SPONSOR entity = GetEntityBy(sponsor.Person);

                if (entity == null)
                {
                    return false;
                }

                entity.Relationship_Id = sponsor.Relationship.Id;
                entity.Sponsor_Contact_Address = sponsor.ContactAddress;
                entity.Sponsor_Mobile_Phone = sponsor.MobilePhone;
                entity.Sponsor_Name = sponsor.Name;

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

        private APPLICANT_SPONSOR GetEntityBy(Person person)
        {
            try
            {
                Expression<Func<APPLICANT_SPONSOR, bool>> selector = s => s.Person_Id == person.Id;
                APPLICANT_SPONSOR entity = GetEntityBy(selector);

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}