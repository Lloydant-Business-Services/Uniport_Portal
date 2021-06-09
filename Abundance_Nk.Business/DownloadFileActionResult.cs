using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Business
{
    public class DownloadFileActionResult : ActionResult
    {
        public DownloadFileActionResult(GridView gv, string pFileName)
        {
            ExcelGridView = gv;
            fileName = pFileName;
        }

        public GridView ExcelGridView { get; set; }
        public string fileName { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpContext curContext = HttpContext.Current;
            curContext.Response.Clear();
            curContext.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            curContext.Response.Charset = "";
            curContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            curContext.Response.ContentType = "application/vnd.ms-excel";
            //curContext.Response.ContentType = "application/pdf";
            //curContext.Response.ContentType = "application/ms-excel";
            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);
            ExcelGridView.RenderControl(htw);
            byte[] byteArray = Encoding.ASCII.GetBytes(sw.ToString());
            var s = new MemoryStream(byteArray);
            var sr = new StreamReader(s, Encoding.ASCII);
            curContext.Response.Write(sr.ReadToEnd());
            curContext.Response.End();


        }
    }
}