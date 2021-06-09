using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class SponsorTranslator : TranslatorBase<Sponsor, APPLICANT_SPONSOR>
    {
        private readonly ApplicationFormTranslator applicationFormTranslator;
        private readonly PersonTranslator personTranslator;
        private readonly RelationshipTranslator relationshipTranslator;
        private AbilityTranslator abilityTranslator;

        public SponsorTranslator()
        {
            personTranslator = new PersonTranslator();
            relationshipTranslator = new RelationshipTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            abilityTranslator = new AbilityTranslator();
        }

        public override Sponsor TranslateToModel(APPLICANT_SPONSOR sponsorEntity)
        {
            try
            {
                Sponsor sponsor = null;
                if (sponsorEntity != null)
                {
                    sponsor = new Sponsor();
                    sponsor.Person = personTranslator.Translate(sponsorEntity.PERSON);
                    sponsor.Relationship = relationshipTranslator.TranslateToModel(sponsorEntity.RELATIONSHIP);
                    sponsor.Name = sponsorEntity.Sponsor_Name;
                    //sponsor.Ability = abilityTranslator.Translate(sponsorEntity.ABILITY);
                    //sponsor.ExtraCurricullarActivities = sponsorEntity.Extra_Curricular_Activities;
                    sponsor.ContactAddress = sponsorEntity.Sponsor_Contact_Address;
                    sponsor.MobilePhone = sponsorEntity.Sponsor_Mobile_Phone;
                    //sponsor.OtherAbility = sponsorEntity.Other_Ability;
                    sponsor.ApplicationForm = applicationFormTranslator.Translate(sponsorEntity.APPLICATION_FORM);
                }

                return sponsor;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_SPONSOR TranslateToEntity(Sponsor sponsor)
        {
            try
            {
                APPLICANT_SPONSOR sponsorEntity = null;
                if (sponsor != null)
                {
                    sponsorEntity = new APPLICANT_SPONSOR();
                    sponsorEntity.Person_Id = sponsor.Person.Id;
                    sponsorEntity.Relationship_Id = sponsor.Relationship.Id;
                    sponsorEntity.Sponsor_Name = sponsor.Name;
                    //sponsorEntity.Ability_Id = sponsor.Ability.Id;
                    //sponsorEntity.Extra_Curricular_Activities = sponsor.ExtraCurricullarActivities;
                    sponsorEntity.Sponsor_Contact_Address = sponsor.ContactAddress;
                    sponsorEntity.Sponsor_Mobile_Phone = sponsor.MobilePhone;
                    //sponsorEntity.Other_Ability = sponsor.OtherAbility;
                    sponsorEntity.Application_Form_Id = sponsor.ApplicationForm.Id;
                }

                return sponsorEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}