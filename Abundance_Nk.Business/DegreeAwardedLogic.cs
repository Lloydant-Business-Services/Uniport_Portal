using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class DegreeAwardedLogic:BusinessBaseLogic<DegreeAwarded,DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT>
    {
        public DegreeAwardedLogic()
        {
            translator = new DegreeAwardedTranslator();
        }

        public DegreeAwarded GetBy(Department department, Programme programme)
        {
            try
            {
                return GetModelBy(a => a.Programme_Id == programme.Id && a.Department_Id == department.Id);
            }
            catch (Exception ex)
            { 
                throw ex;
            }
        }
    }
}
