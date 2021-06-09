using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseUnitLogic : BusinessBaseLogic<CourseUnit, COURSE_UNIT>
    {
        public CourseUnitLogic()
        {
            translator = new CourseUnitTranslator();
        }

        public CourseUnit GetBy(Department department, Level level, Semester semester, Programme programme)
        {
            try
            {
                Expression<Func<COURSE_UNIT, bool>> selector =
                    cu =>
                        cu.Department_Id == department.Id && cu.Level_Id == level.Id && cu.Semester_Id == semester.Id &&
                        cu.Programme_Id == programme.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}