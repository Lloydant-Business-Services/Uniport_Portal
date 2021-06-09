using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using System.Transactions;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class GstScanController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private GstViewModel viewmodel;
        // GET: Admin/GstScan
        public ActionResult Index()
        {
            try
            {
                viewmodel = new GstViewModel();
                viewmodel.Programme = new Programme();
                viewmodel.Department = new Department();
                viewmodel.GstScanAnswer = new GstScanAnswer();
                ViewBag.Departments = viewmodel.DepartmentSelectListItem;
                ViewBag.Programmes = viewmodel.ProgrammeSelectListItem;
                ViewBag.Answers = viewmodel.AnswerSelectListItem;
            }
            catch (Exception ex)
            {
                 SetMessage("Error:" + ex.Message,Message.Category.Information);
            }
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Index(GstViewModel viewmodel,HttpPostedFileBase file)
        {
            try
            {
                viewmodel.GstScan = new GstScan();
                viewmodel.GstScanList = new List<GstScan>();
                if (file != null && viewmodel.Department != null)
                {
                    System.IO.StreamReader r = new StreamReader(file.InputStream);
                    viewmodel.ScannedFile = r.ReadToEnd();
                    string root = Server.MapPath("~/Content/AnswerTextFile");
                    string newFileName = "Scan" + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + ".txt";
                    viewmodel.ScannedFilePath = Path.Combine(root, newFileName);
                    file.SaveAs(viewmodel.ScannedFilePath);
                    ViewBag.Departments = viewmodel.DepartmentSelectListItem;
                    ViewBag.Programmes = viewmodel.ProgrammeSelectListItem;
                    ViewBag.Answers = viewmodel.AnswerSelectListItem;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult Scan(GstViewModel viewmodel, string course_code)
        {
             try
             {
             //   GstScanLogic gstScanLogic = new GstScanLogic();
             //   GstScanAnswerLogic scanAnswerLogic = new GstScanAnswerLogic();
             //   if (viewmodel.ScannedFilePath != "")
             //   {
             //       using (TransactionScope scope = new TransactionScope())
             //       {
             //           string Answer = "";
             //           List<GstScanAnswer> answers =
             //               scanAnswerLogic.GetModelsBy(iL => iL.Description == viewmodel.GstScanAnswer.description).ToList();
             //           if (answers != null)
             //           {
             //               for (int z = 0; z < answers.Count; z++)
             //               {
             //                   Answer = Answer + answers[z].answer;
             //               }

             //               int counter = 0;
             //           string line;
             //           System.IO.StreamReader file = new System.IO.StreamReader(viewmodel.ScannedFilePath);
             //           while ((line = file.ReadLine()) != null)
             //           {
             //               if (line != "")
             //               {
             //                   GstScan gstScan = new GstScan();
             //                   gstScan.MatricNumber = line.Substring(0, 13).Replace("\t", "");
             //                   gstScan.CourseCode = line.Substring(13, 6).Replace("\t", "");
             //                   gstScan.CourseCode = course_code;
             //                   gstScan.Name =
             //                       line.Substring(32, 26)
             //                           .Replace("\t", "")
             //                           .Replace("Q", "")
             //                           .Replace("Z", " ")
             //                           .Replace("?", "");
             //                   gstScan.DepartmentName =
             //                       db.DEPARTMENT.ToList()
             //                           .Where(d => d.Department_Id == Convert.ToInt32(viewmodel.Department.Id))
             //                           .SingleOrDefault()
             //                           .Department_Name.ToUpper();
             //                   gstScan.Department = new Department(){Id = viewmodel.Department.Id};
             //                   gstScan.Programme = new Programme(){Id = viewmodel.Programme.Id};
                               
             //                   gstScan.ProgrammeName =
             //                       db.PROGRAMME.ToList()
             //                           .Where(d => d.Programme_Id == Convert.ToInt32(viewmodel.Programme.Id))
             //                           .SingleOrDefault()
             //                           .Programme_Name.ToUpper();
             //                   gstScan.Shaded = line.Substring(58).Replace("\t", "");
             //                   gstScan.Score = GetCandidateScore(Answer, gstScan.Shaded);
             //                   gstScanLogic.Add(gstScan);
             //                   counter++;

             //               }

             //           }
             //           }

                        

             //           scope.Complete();
             //       }
             //   }
             }
             catch (Exception)
             {
                 
                 throw;
             }


             return RedirectToAction("Index");
        }
        private static int GetCandidateScore(string Answer, string Shaded)
        {
            try
            {
                char[] Answers = Answer.ToArray();

                char[] result = Shaded.ToArray();
                int score = 0;
                for (int i = 0; i <= 49; i++)
                {
                    if (Shaded[i] == Answers[i] || Answers[i] == 'Z')
                    {
                        score++;
                    }
                }
                return score;
            }
            catch (Exception)
            {
                    
                throw;
            }
        }
        public ActionResult UploadAnswer()
        {
            try
            {
                viewmodel = new GstViewModel();
                viewmodel.GstScanAnswer = new GstScanAnswer();
            }
            catch (Exception)
            {
                
                throw;
            }
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult UploadAnswer(string description,HttpPostedFileBase file)
        {
            try
            {
               if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/ExcelUploads"), fileName);
                    file.SaveAs(path);
               
                    string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + "Extended Properties=Excel 8.0;";
                    OleDbConnection connection = new OleDbConnection(xConnStr);
                    OleDbCommand command = new OleDbCommand("Select * FROM [Sheet1$]", connection);
                    connection.Open();

                    // Create DbDataReader to Data Worksheet
                    OleDbDataAdapter MyData = new OleDbDataAdapter();
                    MyData.SelectCommand = command;
                    DataSet ds = new DataSet();
                    ds.Clear();
                    MyData.Fill(ds);
                    connection.Close();

                    //GstScanAnswerLogic gstScanAnswerLogic = new GstScanAnswerLogic();
                    //DataTable table = ds.Tables[0];
                    //int count =  table.Rows.Count;
                    //int counter = 0;
                    //for (int i = 0; i < count; i++)
                    //{
                    //    GstScanAnswer gstScanAnswer = new GstScanAnswer();
                    //    counter = counter + 1;
                    //    gstScanAnswer.Id = counter;
                    //    gstScanAnswer.answer = table.Rows[i]["Answer"].ToString();
                    //    gstScanAnswer.description = description;
                    //    gstScanAnswerLogic.Create(gstScanAnswer);

                    //}

                    db.SaveChanges();      
                    SetMessage("Upload Successful",Message.Category.Information);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return View(viewmodel);
        }
    
    }
}