using Microsoft.Reporting.WebForms;

namespace Abundance_Nk.Web.Models.Intefaces
{
    public interface IReport
    {
        string Message { set; get; }
        ReportViewer Viewer { set; get; }
        int ReportType { get; }
    }
}