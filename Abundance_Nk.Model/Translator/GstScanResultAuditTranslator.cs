using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
     public class GstScanResultAuditTranslator : TranslatorBase<GstScanResultAudit,GST_SCAN_RESULT_AUDIT>
    {
          
        private readonly UserTranslator userTranslator;
 
        public GstScanResultAuditTranslator()
        {
            
            userTranslator = new UserTranslator();
            
        }
        public override GstScanResultAudit TranslateToModel(GST_SCAN_RESULT_AUDIT entity)
        {
            try
            {
                GstScanResultAudit model = null;
                if (entity != null)
                {
                    model = new GstScanResultAudit();
                    model.Id = entity.Gst_Scan_Result_Audit_Id;
                    model.CourseCode = entity.Course_Code;
                    model.CourseTitle = entity.Course_Title;
                    model.ExamNo = entity.Exam_No;
                    model.FullName = entity.FullName;
                    model.DepartmentName = entity.Department;
                    model.RawScore = entity.Raw_Score;
                    model.CA = entity.CA;
                    model.Total = entity.Total;
                    model.SemesterName = entity.Session_Name;
                    model.SemesterId = entity.Semester_Id ?? 1;
                    model.SessionId = entity.Session_Id ?? 7;
                    model.User = userTranslator.Translate(entity.USER);
                    model.Action = entity.Action;
                    model.Client = entity.Client;
                    model.DateUploaded = entity.Date_Uploaded;
                    model.Operation = entity.Operation;
                }
                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public override GST_SCAN_RESULT_AUDIT TranslateToEntity(GstScanResultAudit model)
        {
            try
            {
                GST_SCAN_RESULT_AUDIT entity = null;
                if (model != null)
                {
                    entity = new GST_SCAN_RESULT_AUDIT();
                    entity.Gst_Scan_Result_Id = model.GstScan.Id;
                    entity.Course_Code = model.CourseCode;
                    entity.Course_Title = model.CourseTitle;
                    entity.Exam_No = model.ExamNo;
                    entity.FullName = model.FullName;
                    entity.Department = model.DepartmentName;
                    entity.Raw_Score = model.RawScore;
                    entity.CA = model.CA;
                    entity.Total = model.Total;
                    entity.Session_Name = model.SemesterName;
                    entity.Session_Id = model.SessionId;
                    entity.Semester_Id = model.SemesterId;
                    entity.Action = model.Action;
                    entity.Client = model.Client;
                    entity.Date_Uploaded = model.DateUploaded;
                    entity.User_Id = model.User.Id;
                    entity.Operation = model.Operation;
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
