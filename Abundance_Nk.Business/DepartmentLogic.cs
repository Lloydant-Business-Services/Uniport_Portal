using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Data;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class DepartmentLogic : BusinessBaseLogic<Department, DEPARTMENT>
    {
        public DepartmentLogic()
        {
            translator = new DepartmentTranslator();
        }

        public List<Department> GetBy(Programme programme)
        {
            try
            {
                repository = new Repository();
                List<Department> departments = (from d in repository.GetBy<VW_PROGRAMME_DEPARTMENT>()
                    where d.Programme_Id == programme.Id 
                    select new Department
                    {
                        Id = d.Department_Id,
                        Name = d.Department_Name
                    }
                    ).OrderBy(a => a.Name).ToList();

                return departments;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Department> GetDepartmentForApplicationBy(Programme programme)
        {
            try
            {
                repository = new Repository();
                List<Department> departments = new List<Department>();
                if (programme.Id == 1)
                {
                                     departments = (from d in repository.GetBy<VW_PROGRAMME_DEPARTMENT>()
                                                    where d.Programme_Id == programme.Id && d.Active_PUTME_Application
                                                    select new Department
                                                    {
                                                        Id = d.Department_Id,
                                                        Name = d.Department_Name
                                                    }
                    ).OrderBy(a => a.Name).ToList();
                }
                else
                {
                                     departments = (from d in repository.GetBy<VW_PROGRAMME_DEPARTMENT>()
                                                    where d.Programme_Id == programme.Id
                                                    select new Department
                                                    {
                                                        Id = d.Department_Id,
                                                        Name = d.Department_Name
                                                    }
                    ).OrderBy(a => a.Name).ToList();
                }

                return departments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Department> GetBy(Faculty faculty,Programme programme)
        {
            try
            {
                repository = new Repository();
                List<Department> departments = (from d in repository.GetBy<VW_PROGRAMME_DEPARTMENT>()
                    where d.Faculty_Id == faculty.Id && d.Programme_Id == programme.Id
                    select new Department
                    {
                        Id = d.Department_Id,
                        Name = d.Department_Name
                    }
                    ).OrderBy(a => a.Name).ToList();

                return departments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Department> GetBy(Faculty faculty)
        {
            try
            {
                List<Department> departments = new List<Department>();
                departments = GetModelsBy(a => a.Faculty_Id == faculty.Id).ToList();
                return departments;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool Modify(Department department)
        {
            try
            {
                Expression<Func<DEPARTMENT, bool>> selector = f => f.Department_Id == department.Id;
                DEPARTMENT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Department_Name = department.Name;
                entity.Department_Code = department.Code;
                entity.Faculty_Id = department.Faculty.Id;

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
    }
}