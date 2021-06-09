using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class GstScanResultAuditLogic : BusinessBaseLogic<GstScanResultAudit,GST_SCAN_RESULT_AUDIT>
    {
        public GstScanResultAuditLogic()
        {
            translator = new GstScanResultAuditTranslator();
        }

        public bool Modify(GstScanResultAudit gstScanResultAudit)
        {
            try
            {
                if (gstScanResultAudit.Id <= 0)
                {
                    return false;
                }
                Expression<Func<GST_SCAN_RESULT_AUDIT, bool>> selector = p => p.Gst_Scan_Result_Audit_Id == gstScanResultAudit.Id;
                GST_SCAN_RESULT_AUDIT scanResultEntity = GetEntityBy(selector);
                if (scanResultEntity != null)
                {
                    if (gstScanResultAudit.ExamNo != null)
                    {
                        scanResultEntity.Exam_No = gstScanResultAudit.ExamNo;
                    }
                    if (gstScanResultAudit.RawScore > 0)
                    {
                        scanResultEntity.Raw_Score = gstScanResultAudit.RawScore;
                    }
                    if (gstScanResultAudit.CA > 0)
                    {
                        scanResultEntity.CA = gstScanResultAudit.CA;
                    }
                    if (gstScanResultAudit.CA > 0 && gstScanResultAudit.RawScore > 0)
                    {
                        scanResultEntity.Total = gstScanResultAudit.CA + gstScanResultAudit.RawScore;
                    }
                    else
                    {
                        scanResultEntity.Total = gstScanResultAudit.RawScore; 
                    }

                    scanResultEntity.Operation = gstScanResultAudit.Operation;
                    scanResultEntity.Action = gstScanResultAudit.Action;
                    scanResultEntity.Date_Uploaded = DateTime.Now;
                    int modified = Save();
                    if (modified > 0)
                    {
                        return true;
                    }

                }

                return false;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
