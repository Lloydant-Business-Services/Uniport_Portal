using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
     public class GstScanLogic : BusinessBaseLogic<GstScan,GST_SCAN_RESULT>
    {
         public GstScanLogic()
         {
             translator = new GstScanTranslator();
         }

        public GstScan GetBy(GstScan gstScan)
        {
            try
            {
               return GetModelBy(a => a.EXAMNO == gstScan.MatricNumber && a.COURSE_CODE == gstScan.CourseCode && a.COURSE_TITLE == gstScan.CourseTitle && a.DEPARTMENT == gstScan.DepartmentName && a.SESSION_NAME == gstScan.SemesterName && a.Session_Id==gstScan.SessionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
         public GstScan Modify(GstScan gstScan)
         {
             try
             {
                 if (gstScan.Id <= 0)
                 {
                     return null;
                 }
                 Expression<Func<GST_SCAN_RESULT, bool>> selector = p => p.ID == gstScan.Id;
                 GST_SCAN_RESULT scanEntity = GetEntityBy(selector);
                 if (scanEntity != null)
                 {
                     if (gstScan.Name != null)
                     {
                         scanEntity.FULLNAME = gstScan.Name;
                     }
                     if (gstScan.MatricNumber != null)
                     {
                         scanEntity.EXAMNO = gstScan.MatricNumber;
                     }
                     if (gstScan.RawScore > 0)
                     {
                         scanEntity.RAW_SCORE = gstScan.RawScore;
                     }
                     if (gstScan.Ca > 0)
                     {
                         scanEntity.CA = gstScan.Ca;
                     }
                     if (gstScan.Ca > 0 && gstScan.RawScore > 0)
                     {
                         scanEntity.TOTAL = gstScan.Ca + gstScan.RawScore;
                     }
                     else
                     {
                         scanEntity.TOTAL = gstScan.RawScore;
                     }
                     int modified = Save();
                     if (modified > 0)
                     {
                         return gstScan;
                     }

                 }

                 return null;

             }
             catch (Exception ex)
             {

                 throw;
             }
         }
    }
}
