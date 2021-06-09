using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentSponsorTranslator : TranslatorBase<StudentSponsor, STUDENT_SPONSOR>
    {
        private readonly RelationshipTranslator relationshipTranslator;
        private readonly StudentTranslator studentTranslator;

        public StudentSponsorTranslator()
        {
            studentTranslator = new StudentTranslator();
            relationshipTranslator = new RelationshipTranslator();
        }

        public override StudentSponsor TranslateToModel(STUDENT_SPONSOR entity)
        {
            try
            {
                StudentSponsor model = null;
                if (entity != null)
                {
                    model = new StudentSponsor();
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Relationship = relationshipTranslator.Translate(entity.RELATIONSHIP);
                    model.Name = entity.Sponsor_Name;
                    model.ContactAddress = entity.Sponsor_Contact_Address;
                    model.MobilePhone = entity.Sponsor_Mobile_Phone;
                    model.Email = entity.Email;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_SPONSOR TranslateToEntity(StudentSponsor model)
        {
            try
            {
                STUDENT_SPONSOR entity = null;
                if (model != null)
                {
                    entity = new STUDENT_SPONSOR();
                    entity.Person_Id = model.Student.Id;
                    entity.Relationship_Id = model.Relationship.Id;
                    entity.Sponsor_Name = model.Name;
                    entity.Sponsor_Contact_Address = model.ContactAddress;
                    entity.Sponsor_Mobile_Phone = model.MobilePhone;
                    entity.Email = model.Email;
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