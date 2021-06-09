using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseAllocationLogic : BusinessBaseLogic<CourseAllocation, COURSE_ALLOCATION>
    {
        public CourseAllocationLogic()
        {
            translator = new CourseAllocationTranslator();
        }

        public bool Modify(CourseAllocation courseAllocation)
        {
            try
            {
                Expression<Func<COURSE_ALLOCATION, bool>> selector = l => l.Course_Allocation_Id == courseAllocation.Id;
                COURSE_ALLOCATION entityCourseAllocation = GetEntityBy(selector);
                if (entityCourseAllocation == null)
                {
                    throw new Exception("Not Found");
                }
                entityCourseAllocation.User_Id = courseAllocation.User.Id;
                int modofiedCount = Save();
                if (modofiedCount > 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
    }
}