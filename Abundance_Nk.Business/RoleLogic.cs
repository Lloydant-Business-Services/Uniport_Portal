using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class RoleLogic : BusinessBaseLogic<Role, ROLE>
    {
       
        public RoleLogic()
        {
            base.translator = new RoleTranslator();
        }

        public override List<Role> GetAll()
        {
            try
            {
                return base.GetAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Role> GetAll(Person user)
        {
            try
            {
                var roles = new List<Role>();
                if (user != null)
                {
                    if (user.Role.Id != 2)
                    {
                        Expression<Func<ROLE, bool>> selector = r => r.Role_Id != 2 && r.Role_Id != user.Role.Id;
                        roles = base.GetModelsBy(selector);
                    }
                    else
                    {
                        roles = base.GetAll();
                    }
                }

                return roles;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Role Get(Person user)
        {
            try
            {
                Role role = null;
                if (user != null)
                {
                    Expression<Func<ROLE, bool>> selector = r => r.Role_Id == user.Role.Id;
                    role = base.GetModelBy(selector);
                }

                return role;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(Role role)
        {
            try
            {
                Expression<Func<ROLE, bool>> selector = r => r.Role_Id == role.Id;
                ROLE roleEntity = GetEntityBy(selector);
                roleEntity.Role_Name = role.Name;
                roleEntity.Role_Description = role.Description;

                int rowsAffected = repository.Save();
                if (rowsAffected > 0)
                {
                    return true;
                }
                throw new Exception(NoItemModified);
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException(ArgumentNullException);
            }
                //catch (UpdateException)
                //{
                //    throw new UpdateException(UpdateException);
                //}
            catch (Exception)
            {
                throw;
            }
        }

        public bool Remove(Role role)
        {
            try
            {
                Func<ROLE, bool> selector = r => r.Role_Id == role.Id;
                bool suceeded = base.Delete(selector);

                base.repository.Save();
                return suceeded;
            }
            catch (Exception)
            {
                throw;
            }
        }

      }
}