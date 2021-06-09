using Abundance_Nk.Web.Models.Intefaces;
using System;

namespace Abundance_Nk.Web.Models.Result
{
    public class ReportProcessor
    {
        private readonly ReportBase report;
        private IReport view;

        public ReportProcessor(IReport _view,ReportBase _report)
        {
            view = _view;
            report = _report;
        }

        public void DisplayResult()
        {
            try
            {
                report.GetData();

                if(report.NoResultFound())
                {
                    return;
                }

                report.SetPath();
                report.SetProperties();
                report.SetParameter();
                report.DisplayHelper();
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}