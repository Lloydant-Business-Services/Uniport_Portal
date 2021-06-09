using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class UserAuditTranslator: TranslatorBase<UserAudit, USER_AUDIT>
    {
        private readonly RoleTranslator roleTranslator;
        private readonly SecurityQuestionTranslator securityQuestionTranslator;

        public UserAuditTranslator()
        {
            roleTranslator = new RoleTranslator();
            securityQuestionTranslator = new SecurityQuestionTranslator();
        }

        public override UserAudit TranslateToModel(USER_AUDIT entity)
        {
            try
            {
                UserAudit model = null;
                if (entity != null)
                {
                    model = new UserAudit();
                    model.Id = entity.Id;
                    model.User_Id = entity.User_Id;
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
                    model.User = new User();
                    model.User.Id = entity.Action_User_Id;
                    model.Operatiion = entity.Operation;
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.DateUploaded = entity.Time;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override USER_AUDIT TranslateToEntity(UserAudit model)
        {
            try
            {
                USER_AUDIT entity = null;
                if (model != null)
                {
                    entity = new USER_AUDIT();
                    entity.Id = model.Id;
                    entity.User_Id = model.User_Id;
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
                    entity.Action_User_Id = model.User.Id;
                    entity.Operation = model.Operatiion;
                    entity.Action = model.Action;
                    entity.Client = model.Client;
                    entity.Time = model.DateUploaded;
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
