using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using ExcelLibrary.SpreadSheet;

namespace Abundance_Nk.Business
{
    public class ExportToExcel : ActionResult
    {
        public ExportToExcel(List<ResultFormat> dataToExport, string filename)
        {
            try
            {
                workbook = new Workbook();
                var worksheet = new Worksheet("Sheet 1");
                worksheet.Cells[0, 0] = new Cell("SN");
                worksheet.Cells[0, 1] = new Cell("MATRIC NO");
                worksheet.Cells[0, 2] = new Cell("FULLNAME");
                worksheet.Cells[0, 3] = new Cell("DEPARTMENT");
                worksheet.Cells[0, 4] = new Cell("CA SCORE");
                worksheet.Cells[0, 5] = new Cell("EXAM SCORE");
                worksheet.Cells[0, 6] = new Cell("COURSE CODE");
                int i = 1;
                foreach (ResultFormat resultFormat in dataToExport)
                {
                    worksheet.Cells[i, 0] = new Cell(resultFormat.SN);
                    worksheet.Cells[i, 1] = new Cell(resultFormat.MatricNo);
                    worksheet.Cells[i, 2] = new Cell(resultFormat.Fullname);
                    worksheet.Cells[i, 3] = new Cell(resultFormat.Department);
                    worksheet.Cells[i, 4] = new Cell(resultFormat.CA);
                    worksheet.Cells[i, 5] = new Cell(resultFormat.Exam);
                    worksheet.Cells[i, 6] = new Cell(resultFormat.CourseCode);
                    i++;
                }
                workbook.Worksheets.Add(worksheet);
                workbook.Save(filename);
                savePath = filename;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Workbook workbook { get; set; }
        public string savePath { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpContext curContext = HttpContext.Current;
            curContext.Response.Clear();
            curContext.Response.Charset = "";
            curContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            curContext.Response.Redirect(savePath);
        }
    }
}