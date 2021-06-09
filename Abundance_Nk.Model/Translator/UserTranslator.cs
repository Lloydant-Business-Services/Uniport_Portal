using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class UserTranslator : TranslatorBase<User, USER>
    {
        private readonly RoleTranslator roleTranslator;
        private readonly SecurityQuestionTranslator securityQuestionTranslator;

        public UserTranslator()
        {
            roleTranslator = new RoleTranslator();
            securityQuestionTranslator = new SecurityQuestionTranslator();
        }

        public override User TranslateToModel(USER entity)
        {
            try
            {
                User model = null;
                if (entity != null)
                {
                    model = new User();
                    model.Id = entity.User_Id;
                    model.Username = entity.User_Name;
                    model.Password = entity.Password;
                    model.Email = entity.Email;
                    model.SecurityQuestion = securityQuestionTranslator.Translate(entity.SECURITY_QUESTION);
                    model.SecurityAnswer = entity.Security_Answer;
                    model.Role = roleTranslator.Translate(entity.ROLE);
                    model.LastLoginDate = entity.LastLoginDate;
                    model.LastPasswordChangeDate = entity.LastPasswordChangeDate;
                    model.SignatureUrl = entity.Signature_Url;
                    model.ProfileImageUrl = entity.Profile_Image_Url;
					model.Activated = entity.Activated ?? true ;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override USER TranslateToEntity(User model)
        {
            try
            {
                USER entity = null;
                if (model != null)
                {
                    entity = new USER();
                    entity.User_Id = model.Id;
                    entity.User_Name = model.Username;
                    entity.Password = model.Password;
                    entity.Email = model.Email;
                    entity.Security_Question_Id = model.SecurityQuestion.Id;
                    entity.Security_Answer = model.SecurityAnswer;
                    entity.Role_Id = model.Role.Id;
                    entity.LastLoginDate = model.LastLoginDate;
                    entity.LastPasswordChangeDate = model.LastPasswordChangeDate;
                    entity.Signature_Url = model.SignatureUrl;
                    entity.Profile_Image_Url = model.ProfileImageUrl;
					entity.Activated = model.Activated;
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