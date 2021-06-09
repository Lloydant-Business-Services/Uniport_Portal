using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class GstScanTranslator : TranslatorBase<GstScan, GST_SCAN_RESULT>
    {
         
        public override GstScan TranslateToModel(GST_SCAN_RESULT entity)
        {
            try
            {
                GstScan model = null;
                if (entity != null)
                {
                    model = new GstScan();
                    model.Id = entity.ID;
                    model.CourseCode = entity.COURSE_CODE;
                    model.CourseTitle = entity.COURSE_TITLE;
                    model.MatricNumber = entity.EXAMNO;
                    model.Name = entity.FULLNAME;
                    model.DepartmentName = entity.DEPARTMENT;
                    model.RawScore = entity.RAW_SCORE;
                    model.Ca = entity.CA;
                    model.Total = entity.TOTAL;
                    model.SemesterName = entity.SESSION_NAME;
                    model.SemesterId = entity.Semester_Id ?? 1;
                    model.SessionId = entity.Session_Id ?? 7;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override GST_SCAN_RESULT TranslateToEntity(GstScan model)
        {
            try
            {
                GST_SCAN_RESULT entity = null;
                if (model != null)
                {
                    entity = new GST_SCAN_RESULT();
                    entity.COURSE_CODE = model.CourseCode;
                    entity.COURSE_TITLE = model.CourseTitle;
                    entity.EXAMNO = model.MatricNumber;
                    entity.FULLNAME = model.Name;
                    entity.DEPARTMENT = model.DepartmentName;
                    entity.RAW_SCORE = model.RawScore;
                    entity.CA = model.Ca;
                    entity.TOTAL = model.Total;
                    entity.SESSION_NAME = model.SemesterName;
                    entity.Session_Id = model.SessionId;
                    entity.Semester_Id = model.SemesterId;
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
