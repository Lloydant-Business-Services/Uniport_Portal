using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class ResultTranscriptReporter : System.Web.UI.Page
    {
        string studentId;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    lblMessage.Text = "";
                    if (Request.QueryString["Id"] != null)
                    {
                        studentId = Request.QueryString["Id"];
                        BuildStudentTranscript(studentId);
                    }
                }


            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
        private void BuildStudentTranscript(string sId)
        {
            try
            {
                string addreessUrl = "";
                long stdId = Convert.ToInt64(sId);
                StudentLevel studentLevel = new StudentLevel();
                StudentLogic studentLogic = new StudentLogic();
                List<Model.Model.Result> rawresults = new List<Model.Model.Result>();
                List<Model.Model.Result> results = new List<Model.Model.Result>();
                studentLevel =GetStudentCurrentLevel(stdId);
                if (studentLevel != null)
                {
                    var student = studentLogic.GetModelsBy(x => x.Person_Id == stdId).FirstOrDefault();
                    rawresults = studentLogic.GetTranscriptBy(student, studentLevel.Department);
                    if (rawresults?.Count > 0)
                    {
                        var groupedByCourse = rawresults.GroupBy(c => c.CourseId).ToList();
                        var orderedResult = rawresults.OrderBy(f => f.LevelId).ToList();
                        foreach (var item in groupedByCourse)
                        {
                    
                            var individualResult = orderedResult.Where(g => g.CourseId == item.Key && g.Score >= 40).ToList();
                            if (individualResult?.Count > 1)
                            {
                                var firstResult = individualResult.FirstOrDefault();
                                results.Add(firstResult);
                            }
                            else
                            {
                                //Get all the results of the course Id
                                var allResult = orderedResult.Where(g => g.CourseId == item.Key).ToList();
                                results.AddRange(allResult);
                            }
                        }
                    }
                    if (results.Count>0 && results != null)
                    {
                        GenerateTranscriptForOtherDepartments(results, student, stdId);
                        string bind_ds = "dsMasterSheet";
                        string reportPath = Server.MapPath("~/Reports/ResultTranscript2.rdlc");
                        ReportViewer1.Reset();
                        ReportViewer1.LocalReport.ReportPath = reportPath;
                        string replacRegNumber = results[0].MatricNumber.Replace('/', '_');
                        ReportViewer1.LocalReport.DisplayName = results[1].Name + " " + "Transcript Result";
                        ReportViewer1.LocalReport.EnableExternalImages = true;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        //
                        //addreessUrl = results[0].WaterMark;
                        ReportDataSource rdc1 = new ReportDataSource(bind_ds, results);
                        ReportViewer1.LocalReport.DataSources.Add(rdc1);
                        string imageFilePath = "~/Content/ExcelUploads/" + replacRegNumber + ".Jpeg";
                        string imagePath = new Uri(Server.MapPath(imageFilePath)).AbsoluteUri;
                        //string addressPath = new Uri(Server.MapPath(addreessUrl)).AbsoluteUri;
                        string reportMark = Server.MapPath("/Content/Images/absu.bmp");
                        string addressPath = new Uri(reportMark).AbsoluteUri;

                        ReportParameter parameter = new ReportParameter("ImagePath", imagePath);
                        ReportParameter parameter1 = new ReportParameter("AddressPath", addressPath);
                        ReportViewer1.LocalReport.SetParameters(new[] { parameter, parameter1 });
                        ReportViewer1.LocalReport.Refresh();
                        //ReportDataSource rdc2 = new ReportDataSource("dsPaymentclearanceDue", duePaymentList);
                        //ReportDataSource rdc3 = new ReportDataSource("dsPaymentclearancePaid", madePaymentList);
                        //ReportDataSource rdc1 = new ReportDataSource("dsMasterSheet", results);
                        //ReportViewer1.LocalReport.DataSources.Add(rdc1);
                        //ReportViewer1.LocalReport.DataSources.Add(rdc2);
                        //ReportViewer1.LocalReport.DataSources.Add(rdc3);
                        //ReportViewer1.LocalReport.Refresh();
                        //ReportViewer1.DataBind();
                        //var groupedResults = results.OrderBy(x=>x.SessionId).GroupBy(x => x.SessionId);
                        //if(groupedResults.Count()>0 && groupedResults != null)
                        //{
                        //    foreach(var groupedResult in groupedResults)
                        //    {
                        //        var min=results.Where(x => x.SessionId == groupedResult.Key);
                        //    }
                        //}

                    }
                }
                
                
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
        public StudentLevel GetStudentCurrentLevel(long sId)
        {
            try
            {
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                var studentLevel=studentLevelLogic.GetModelsBy(x => x.Person_Id == sId).FirstOrDefault();
                return studentLevel;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        private void GenerateTranscriptForOtherDepartments(List<Model.Model.Result> results, Student student, long sId)
        {
            try
            {
                StudentAcademicInformationLogic studentAcademicInformationLogic = new StudentAcademicInformationLogic();
                StudentAcademicInformation studentAcademicInformation = new StudentAcademicInformation();
                studentAcademicInformation=studentAcademicInformationLogic.GetModelsBy(x => x.STUDENT.Person_Id == sId).FirstOrDefault();
                //if (studentAcademicInformation != null)
                //{
                    List<int> totalUnits = new List<int>();
                    List<decimal> totalGradePoint = new List<decimal>();
                    StudentLogic resultLogic = new StudentLogic();
                    string imageUrl = "";
                    string remark = "";
                    string graduationDate = "";
                    string admissionDate = "";
                    int distinctSession = 0;
                var numberOfCompletedSessions = 0;

                    if (!sId.ToString().Contains('_'))
                    {
                        imageUrl = GenerateQrCode(results[0].MatricNumber);
                    }
                if (studentAcademicInformation != null)
                {
                    string yearofEntry = studentAcademicInformation != null ? studentAcademicInformation.YearOfAdmission.ToString() : null;
                    string yearofGraduation = studentAcademicInformation != null ? studentAcademicInformation.YearOfGraduation.ToString() : null;
                    string[] yearofEntryText = yearofEntry.Split('/');
                    string[] yearofGraduationText = yearofGraduation.Split('/');
                    graduationDate = yearofGraduationText.LastOrDefault();
                    admissionDate = yearofEntryText.FirstOrDefault();
                    numberOfCompletedSessions = Convert.ToInt32(graduationDate) - Convert.ToInt32(admissionDate);
                }
                    

                    
                    string addreessUrl = GenerateWaterMark(results[0].Address);

                    var firstOrDefault = results.FirstOrDefault();
                    DegreeAwarded degreeAward = new DegreeAwarded();
                    foreach (Model.Model.Result result in results)
                    {
                        if (firstOrDefault != null)
                        {
                            var id = firstOrDefault.DepartmentId;
                            var programmeId = firstOrDefault.ProgrammeId;

                            distinctSession = results.Select(s => s.SessionId).ToList().Distinct().Count();

                            DegreeAwardedLogic degreeAwardsLogic = new DegreeAwardedLogic();
                            degreeAward = degreeAwardsLogic.GetModelBy(ad => ad.Department_Id == id && ad.Programme_Id == programmeId);
                        }

                        if (degreeAward != null && degreeAward.Id > 0)
                        {
                            remark = degreeAward.Degree;
                        }
                        else
                        {
                            throw new Exception("No Degree Award Was Set For The Department kindly Set Degree And Try Again.");
                        }

                        if (degreeAward.Programme.Id == 11)
                        {
                            result.Semestername = "LONG VACATION";
                        }

                        if (string.IsNullOrWhiteSpace(result.Grade))
                        {
                            result.GradePoints = " ";
                        }
                        else
                        {
                            string converttodecimal = Math.Round((Decimal)result.GPCU, 2).ToString("0.0");
                            result.GradePoints = converttodecimal;
                        }

                        result.GraduationDate = graduationDate!=""? graduationDate:"";
                        result.AdmissionDate = admissionDate!=""?admissionDate:"";
                        result.QrCode = imageUrl;
                        result.Address = studentAcademicInformation!=null?studentAcademicInformation.YearOfAdmission.ToString():null;
                        result.WaterMark = addreessUrl;
                        totalUnits.Add(result.CourseUnit);
                        if (result.GPCU == null)
                        {
                            result.GPCU = 0;
                        }
                        totalGradePoint.Add((decimal)result.GPCU);

                    }
                    for (int i = 0; i < totalUnits.Count; i++)
                    {
                        if (string.IsNullOrEmpty(results[i].cGPA))
                        {
                            results[i].TotalSemesterCourseUnit = totalUnits.Sum();
                            results[i].TotalGradePoint = totalGradePoint.Sum();
                            string cGPA = Math.Round((decimal)(results[i].TotalGradePoint / results[i].TotalSemesterCourseUnit), 2).ToString("0.00");
                            results[i].CGPA = Convert.ToDecimal(cGPA);
                            //results[i].CGPA = Math.Round(Convert.ToDecimal(results[i].cGPA), 2);
                            results[i].cGPA = cGPA;
                            results[i].Remark = remark;
                            results[i].DegreeClassification = resultLogic.GetGraduationStatus(results[i].CGPA, results[i].GraduationDate);
                            results[i].Date = resultLogic.GetTodaysDateFormat();
                            results[i].DateOfBirth = studentAcademicInformation!=null?studentAcademicInformation.Student.DateOfBirth:null;
                            //results[i].WesVerificationNumber = student.WesVerificationNumber;
                            results[i].NumberOfNonCompletedSession = numberOfCompletedSessions==0? 0 :CheckIfStudiesWhereComplete(numberOfCompletedSessions, distinctSession);
                        }
                        else
                        {
                            results[i].Remark = remark;
                            results[i].CGPA = Convert.ToDecimal(results[i].cGPA);
                            results[i].DegreeClassification = resultLogic.GetGraduationStatus(results[i].CGPA, results[i].GraduationDate);
                            results[i].Date = resultLogic.GetTodaysDateFormat();
                            results[i].DateOfBirth = studentAcademicInformation!=null?studentAcademicInformation.Student.DateOfBirth :null;
                            results[i].TotalSemesterCourseUnit = totalUnits.Sum();
                            results[i].TotalGradePoint = totalGradePoint.Sum();
                            //results[i].WesVerificationNumber = !string.IsNullOrEmpty(student.WesVerificationNumber) ? student.WesVerificationNumber : " ";
                        }

                    }
                //}

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public string GenerateQrCode(string regNo, string alt = "QR code", int height = 100, int width = 100, int margin = 0)
        {

            string replacRegNumber = regNo.Replace('/', '_');
            string folderPath = "~/Content/ExcelUploads/";
            string imagePath = "~/Content/ExcelUploads/" + replacRegNumber + ".Jpeg";
            string verifyUrl = "localhost:6390/Student/Result/VerifyTranscript/" + replacRegNumber;


            string wildCard = replacRegNumber + "*.*";
            IEnumerable<string> files = Directory.EnumerateFiles(Server.MapPath("~/Content/ExcelUploads/"), wildCard, SearchOption.TopDirectoryOnly);

            if (files != null && files.Count() > 0)
            {
                return imagePath;
            }
            // If the directory doesn't exist then create it.
            if (!Directory.Exists(Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(folderPath);
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var result = barcodeWriter.Write(verifyUrl);

            string barcodePath = Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }


            }

            return imagePath;
        }
        public string GenerateWaterMark(string address)
        {
            string returnImagePath = "~/Content/Junk/defaultImage1.bmp";
            string imagePath = Server.MapPath("~/Content/Junk/defaultImage1.bmp");
            string defultImagePath = Server.MapPath("~/Content/Junk/defaultImage.bmp");

            Bitmap bmp = new Bitmap(defultImagePath);
            RectangleF rectf = new RectangleF(0, 0, bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(address, new Font("Tahoma", 16), Brushes.Gainsboro, rectf, format);
            g.Flush();
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(imagePath, FileMode.Create))
                {
                    bmp.Save(memory, ImageFormat.Bmp);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }


            }
            return returnImagePath;
        }
        private int CheckIfStudiesWhereComplete(int startAndEndSessionDifference, int numberofSessions)
        {
            int numberNotCompleted;
            try
            {
                numberNotCompleted = startAndEndSessionDifference - numberofSessions;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return numberNotCompleted;
        }
    }
}