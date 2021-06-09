using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class UserLogic : BusinessBaseLogic<User, USER>
    {
        public UserLogic()
        {
            translator = new UserTranslator();
        }
         public User GetBy(string Username)
        {
            try
            {
                Expression<Func<USER, bool>> selector = p => p.User_Name == Username;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool isPasswordDueForChange(string Username, string Password)
        {
            try
            {
                Expression<Func<USER, bool>> selector = p => p.User_Name == Username && p.Password == Password;
                User UserDetails = GetModelBy(selector);
                if (UserDetails != null && UserDetails.Password != null)
                {
                    var Days = (DateTime.Now -(DateTime)UserDetails.LastPasswordChangeDate).TotalDays;
                    if (Days >= 30)
                    {
                        return true;
                    }
                    
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool isPasswordDueForChange(string Username)
        {
            try
            {
                Expression<Func<USER, bool>> selector = p => p.User_Name == Username;
                User UserDetails = GetModelBy(selector);
                if (UserDetails != null && UserDetails.Password != null)
                {
                    //var Days = (DateTime.Now - (DateTime)UserDetails.LastLoginDate).TotalDays;
                    //if (Days >= 30)
                    //{
                    //    return true;
                    //}
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool ValidateUser(string Username, string Password)
        {
            try
            {
                Expression<Func<USER, bool>> selector = p => p.User_Name == Username && p.Password == Password && p.Activated == true;
                User UserDetails = GetModelBy(selector);
                if (UserDetails != null && UserDetails.Password != null)
                {
                    UpdateLastLogin(UserDetails);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateLastLogin(User user)
        {
            try
            {
                Expression<Func<USER, bool>> selector = p => p.User_Name == user.Username && p.Password == user.Password;
                USER userEntity = GetEntityBy(selector);
                if (userEntity == null || userEntity.User_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                userEntity.User_Name = user.Username;
                userEntity.Password = user.Password;
                userEntity.Email = user.Email;
                userEntity.Role_Id = user.Role.Id;
                userEntity.Security_Question_Id = user.SecurityQuestion.Id;
                userEntity.Security_Answer = user.SecurityAnswer;
                userEntity.LastLoginDate = DateTime.Now;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ChangeUserPassword(User user)
        {
            try
            {
                Expression<Func<USER, bool>> selector = p => p.User_Name == user.Username;
                USER userEntity = GetEntityBy(selector);
                if (userEntity == null || userEntity.User_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                userEntity.User_Name = user.Username;
                userEntity.Password = user.Password;
                userEntity.Email = user.Email;
                userEntity.Role_Id = user.Role.Id;
                userEntity.Security_Question_Id = user.SecurityQuestion.Id;
                userEntity.Security_Answer = user.SecurityAnswer;
                userEntity.LastLoginDate = user.LastLoginDate;
                userEntity.LastPasswordChangeDate = DateTime.Now;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User Update(User model)
        {
            try
            {
                Expression<Func<USER, bool>> selector = a => a.User_Id == model.Id;
                USER entity = GetEntityBy(selector);

                entity.User_Name = model.Username;
                entity.Password = model.Password;
                entity.Email = model.Email;
                entity.Activated = model.Activated;
                if (model.Role != null)
                {
                    entity.Role_Id = model.Role.Id;
                }
                if (model.SignatureUrl != null)
                {
                    entity.Signature_Url = model.SignatureUrl;
                }
                entity.Security_Answer = model.SecurityAnswer;
                entity.Security_Question_Id = model.SecurityQuestion.Id;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(User model)
        {
            try
            {
                Expression<Func<USER, bool>> selector = u => u.User_Id == model.Id;
                USER entity = GetEntityBy(selector);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.Password != null)
                {
                     entity.Password = model.Password;
                }
               
                if (model.Role != null)
                {
                    entity.Role_Id = model.Role.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}