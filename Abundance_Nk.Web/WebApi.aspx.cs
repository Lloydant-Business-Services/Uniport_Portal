using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Windows.Forms;
using System.Xml;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System.Threading.Tasks;

namespace Abundance_Nk.Web
{
    public partial class WebApi : Page
    {
        StudentLevelLogic studentLogic;
        AdmissionListLogic admissionListLogic;
        AppliedCourseLogic appliedCourseLogic;
        StudentPaymentLogic studentPaymentLogic;
        FeeDetailLogic feeDetailLogic;


        protected void Page_Load(object sender, EventArgs e)
        {
             studentLogic = new StudentLevelLogic();
             admissionListLogic = new AdmissionListLogic();
             appliedCourseLogic = new AppliedCourseLogic();
             studentPaymentLogic = new StudentPaymentLogic();
             feeDetailLogic = new FeeDetailLogic();

            if (Request.QueryString["payee_id"] != null && Request.QueryString["payment_type"] != null)
            {
                string payeeid = Request.QueryString["payee_id"];
                string payment_type = Request.QueryString["payment_type"];
                RegisterAsyncTask(new PageAsyncTask(() => BuidXmlInMemoryOptimized(payeeid, payment_type)));
               // BuidXml(payeeid, payment_type).Wait();
            }
        }

        //private void BuidXml(string InvoiceNumber, string paymentpurpose)
        //{
        //    string url = "";
        //    try
        //    {
        //        string filename = InvoiceNumber.Replace("-", "").Replace("/", "").Replace(":", "").Replace(" ", "") +
        //                          DateTime.Now.ToString()
        //                              .Replace("-", "")
        //                              .Replace("/", "")
        //                              .Replace(":", "")
        //                              .Replace(" ", "");
        //        url = "~/PayeeId/" + filename + ".xml";
        //        if (!Directory.Exists(Server.MapPath("~/PayeeId")))
        //        {
        //            Directory.CreateDirectory(Server.MapPath("~/PayeeId"));
        //        }

        //        var payment = new Payment();
        //        var paymentLogic = new PaymentLogic();
        //        string paymentStatus = "FEE HAS NOT BEEN PAID";

        //        payment = paymentLogic.GetBy(InvoiceNumber);
        //        if (payment == null)
        //        {
        //            CreateErrorTree(url);
        //            return;
        //        }

        //        if (paymentLogic.PaymentAlreadyMade(payment))
        //        {
        //            paymentStatus = "FEE HAS BEEN PAID";
        //        }

        //        var paymentEtranzactType = new PaymentEtranzactType();
        //        var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
        //        paymentEtranzactType =
        //            paymentEtranzactTypeLogic.GetModelsBy(
        //                m =>
        //                    m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id &&
        //                    m.Session_Id == payment.Session.Id && m.Payment_Mode_Id == payment.PaymentMode.Id)
        //                .FirstOrDefault();
        //        if (paymentEtranzactType == null)
        //        {
        //            CreateErrorTree(url);
        //            return;
        //        }

        //        StudentLevelLogic studentLogic = new StudentLevelLogic();
        //        AdmissionListLogic admissionListLogic = new AdmissionListLogic();
        //        AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
        //        StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();

        //        var studentLevel = studentLogic.GetBy(payment.Person.Id);
        //        var paymentLog = studentPaymentLogic.GetBy(payment);
        //        var admission = admissionListLogic.GetBy(payment.Person);
        //        var appliedCourse = appliedCourseLogic.GetBy(payment.Person);

        //        if (studentLevel == null)
        //        {
        //            Department department;
        //            Programme programme;
        //            if (admission != null || appliedCourse != null)
        //            {
        //                if (admission != null && admission.Deprtment != null)
        //                {
        //                    department = admission.Deprtment;
        //                }
        //                else
        //                {
        //                    department = appliedCourse.Department;
        //                }
        //                if (admission != null && admission.Programme != null)
        //                {

        //                    programme = admission.Programme;
        //                }
        //                else
        //                {
        //                    programme = appliedCourse.Programme;
        //                }

        //                studentLevel = new StudentLevel()
        //                {
        //                    Level = new Level() { Id = 1, Name = "100 Level" },
        //                    Student = new Student() { Id = payment.Person.Id, MatricNumber = "N/A" },
        //                    Department = department,
        //                    Programme = programme,
        //                    Session = payment.Session
        //                };
        //            }

        //        }

        //        if (paymentLog == null)
        //        {
        //            paymentLog = new StudentPayment();
        //            paymentLog.Amount = feeDetailLogic.GetFeeByDepartmentLevel(studentLevel.Department, studentLevel.Level, studentLevel.Programme, payment.FeeType, payment.Session, payment.PaymentMode);
        //        }

        //        if (payment != null && studentLevel != null && paymentLog != null && paymentEtranzactType != null)
        //        {
        //            string fullname = payment.Person.FullName;
        //            string faculty = studentLevel.Department.Faculty.Name.ToUpper();
        //            string dept = studentLevel.Department.Name.ToUpper();
        //            string level = studentLevel.Level.Name.ToUpper();
        //            string studenttypeid = studentLevel.Department.Name.ToUpper();
        //            string modeofentry = "N/A";
        //            string sessionid = payment.Session.Name.ToUpper();
        //            string Amount = paymentLog.Amount.ToString() ?? "0";
        //            string paymentstatus = "-";
        //            string PaymentType = paymentEtranzactType.Name.ToUpper();
        //            string phoneNo = payment.Person.MobilePhone;
        //            string email = payment.Person.Email;
        //            string MatricNo = studentLevel.Student.MatricNumber.ToUpper();
        //            string levelid = studentLevel.Level.Id.ToString();
        //            string PaymentCategory = paymentEtranzactType.Name.ToUpper();
        //            string semester = "N/A";
        //            string semesterid = "N/A";
        //            string shortFallAmount = "N/A";

        //            CreateTree(fullname, faculty, dept, level, studenttypeid, modeofentry, sessionid, InvoiceNumber, Amount, paymentstatus, semester, PaymentType, MatricNo, email, phoneNo, PaymentCategory, url);
        //            return;

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        CreateErrorTree(url);
        //        return;
        //        throw ex;
        //    }
        //}

        private async Task BuidXml(string InvoiceNumber, string paymentpurpose)
        {
            string url = "";
            try
            {
                string filename = InvoiceNumber.Replace("-", "").Replace("/", "").Replace(":", "").Replace(" ", "") +
                                  DateTime.Now.ToString()
                                      .Replace("-", "")
                                      .Replace("/", "")
                                      .Replace(":", "")
                                      .Replace(" ", "");
                url = "~/PayeeId/" + filename + ".xml";
                if (!Directory.Exists(Server.MapPath("~/PayeeId")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/PayeeId"));
                }

                var payment = new Payment();
                var paymentLogic = new PaymentLogic();
                string paymentStatus = "FEE HAS NOT BEEN PAID";

                payment = await paymentLogic.GetByAsync(InvoiceNumber);
                if (payment == null)
                {
                    CreateErrorTree(url);
                    return;
                }

                if (await paymentLogic.PaymentAlreadyMadeAsync(payment))
                {
                    paymentStatus = "FEE HAS BEEN PAID";
                }
                StudentLevelLogic studentLogic = new StudentLevelLogic();
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();

                var studentLevel = await studentLogic.GetByAsync(payment.Person.Id);
                var appliedCourse = await appliedCourseLogic.GetByAsync(payment.Person);
                var paymentLog = await studentPaymentLogic.GetByAsync(payment);
                var admission = await admissionListLogic.GetByAsync(payment.Person);
                

                if (studentLevel == null)
                {
                    Department department;
                    Programme programme;
                    if (admission != null || appliedCourse != null)
                    {
                        if (admission != null && admission.Deprtment != null)
                        {
                            department = admission.Deprtment;
                        }
                        else
                        {
                            department = appliedCourse.Department;
                        }
                        if (admission != null && admission.Programme != null)
                        {

                            programme = admission.Programme;
                        }
                        else
                        {
                            programme = appliedCourse.Programme;
                        }

                        studentLevel = new StudentLevel()
                        {
                            Level = new Level() { Id = 1, Name = "100 Level" },
                            Student = new Student() { Id = payment.Person.Id, MatricNumber = "N/A" },
                            Department = department,
                            Programme = programme,
                            Session = payment.Session
                        };
                    }

                }
                var paymentEtranzactType = new PaymentEtranzactType();
                var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                paymentEtranzactType = await paymentEtranzactTypeLogic.GetModelsByFODAsync(
                        m =>
                            m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id &&
                            m.Session_Id == payment.Session.Id && m.Payment_Mode_Id == payment.PaymentMode.Id && m.Programme_Id == studentLevel.Programme.Id);
                if (paymentEtranzactType == null)
                {
                    CreateErrorTree(url);
                    return;
                }

                if (paymentLog == null)
                {
                    paymentLog = new StudentPayment();
                    paymentLog.Amount = await feeDetailLogic.GetFeeByDepartmentLevelAsync(studentLevel.Department, studentLevel.Level, studentLevel.Programme, payment.FeeType, payment.Session, payment.PaymentMode);
                }

                if (payment != null && studentLevel != null && paymentLog != null && paymentEtranzactType != null)
                {
                    string fullname = payment.Person.FullName;
                    string faculty = studentLevel.Department.Faculty.Name.ToUpper();
                    string dept = studentLevel.Department.Name.ToUpper();
                    string level = studentLevel.Level.Name.ToUpper();
                    string studenttypeid = studentLevel.Department.Name.ToUpper();
                    string modeofentry = "N/A";
                    string sessionid = payment.Session.Name.ToUpper();
                    string Amount = paymentLog.Amount.ToString() ?? "0";
                    string paymentstatus = "-";
                    string PaymentType = paymentEtranzactType.Name.ToUpper();
                    string phoneNo = payment.Person.MobilePhone;
                    string email = payment.Person.Email;
                    string MatricNo = studentLevel.Student.MatricNumber.ToUpper();
                    string levelid = studentLevel.Level.Id.ToString();
                    string PaymentCategory = paymentEtranzactType.Name.ToUpper();
                    string semester = "N/A";
                    string semesterid = "N/A";
                    string shortFallAmount = "N/A";

                    CreateTree(fullname, faculty, dept, level, studenttypeid, modeofentry, sessionid, InvoiceNumber, Amount, paymentstatus, semester, PaymentType, MatricNo, email, phoneNo, PaymentCategory, url);
                    return;

                }


            }
            catch (Exception ex)
            {
                CreateErrorTree(url);
                return;
                throw ex;
            }
        }

        private async Task BuidXmlInMemoryOptimized(string InvoiceNumber, string paymentpurpose)
        {
            
            try
            {
                
                var payment = new Payment();
                var paymentLogic = new PaymentLogic();
                var paymentEtranzactLogic = new PaymentEtranzactLogic();
                string paymentStatus = "FEE HAS NOT BEEN PAID";

                payment = await paymentLogic.GetByAsync(InvoiceNumber);
                if (payment == null)
                {
                    CreateErrorTree();
                    return;
                }

                if (await paymentEtranzactLogic.PaymentAlreadyMadeAsync(payment))
                {
                    paymentStatus = "FEE HAS BEEN PAID";
                }

                

                StudentLevelLogic studentLogic = new StudentLevelLogic();
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();

                var studentLevel = await studentLogic.GetByAsync(payment.Person.Id);
                var paymentLog = await studentPaymentLogic.GetByAsync(payment);
                var admission = await admissionListLogic.GetByAsync(payment.Person);
                var appliedCourse = await appliedCourseLogic.GetByAsync(payment.Person);

                if (studentLevel == null)
                {
                    Department department;
                    Programme programme;
                    if (admission != null || appliedCourse != null)
                    {
                        if (admission != null && admission.Deprtment != null)
                        {
                            department = admission.Deprtment;
                        }
                        else
                        {
                            department = appliedCourse.Department;
                        }
                        if (admission != null && admission.Programme != null)
                        {

                            programme = admission.Programme;
                        }
                        else
                        {
                            programme = appliedCourse.Programme;
                        }

                        studentLevel = new StudentLevel()
                        {
                            Level = new Level() { Id = 1, Name = "100 Level" },
                            Student = new Student() { Id = payment.Person.Id, MatricNumber = "N/A" },
                            Department = department,
                            Programme = programme,
                            Session = payment.Session
                        };
                    }

                }

                var paymentEtranzactType = new PaymentEtranzactType();
                var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                if(payment!=null && payment.FeeType!=null && payment.FeeType.Id == 12)
                {
                    paymentEtranzactType = await paymentEtranzactTypeLogic.GetModelsByFODAsync(
                        m =>
                            m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id &&
                            m.Session_Id == payment.Session.Id && m.Payment_Mode_Id == payment.PaymentMode.Id);
                }
                else
                {
                    paymentEtranzactType = await paymentEtranzactTypeLogic.GetModelsByFODAsync(
                        m =>m.Payment_Etranzact_Type_Name.Contains(paymentpurpose)  && m.Fee_Type_Id == payment.FeeType.Id &&
                            m.Session_Id == payment.Session.Id && m.Payment_Mode_Id == payment.PaymentMode.Id && m.Programme_Id == studentLevel.Programme.Id
                            );
                }
                
                if (paymentEtranzactType == null)
                {
                    CreateErrorTree();
                    return;
                }

                if (paymentLog == null)
                {
                    paymentLog = new StudentPayment();
                    paymentLog.Amount = await feeDetailLogic.GetFeeByDepartmentLevelAsync(studentLevel.Department, studentLevel.Level, studentLevel.Programme, payment.FeeType, payment.Session, payment.PaymentMode);
                }

                if (payment != null && studentLevel != null && paymentLog != null && paymentEtranzactType != null)
                {
                    string fullname = payment.Person.FullName;
                    string faculty = studentLevel.Department.Faculty.Name.ToUpper();
                    string dept = studentLevel.Department.Name.ToUpper();
                    string level = studentLevel.Level.Name.ToUpper();
                    string studenttypeid = studentLevel.Programme.Name.ToUpper();
                    string modeofentry = "N/A";
                    string sessionid = payment.Session.Name.ToUpper();
                    string Amount = paymentLog.Amount.ToString() ?? "0";
                    string paymentstatus = paymentStatus;
                    string PaymentType = paymentEtranzactType.Name.ToUpper();
                    string phoneNo = payment.Person.MobilePhone;
                    string email = payment.Person.Email;
                    string MatricNo = studentLevel.Student.MatricNumber.ToUpper();
                    string levelid = studentLevel.Level.Id.ToString();
                    string PaymentCategory = paymentEtranzactType.Name.ToUpper();
                    string semester = "N/A";
                    string semesterid = "N/A";
                    string shortFallAmount = "N/A";

                    CreateTree(fullname, faculty, dept, level, studenttypeid, modeofentry, sessionid, InvoiceNumber, Amount, paymentstatus, semester, PaymentType, MatricNo, email, phoneNo, PaymentCategory);
                    return;

                }


            }
            catch (Exception ex)
            {
                CreateErrorTree();
                return;
                throw ex;
            }
        }


        private async Task BuidXmlInMemory(string InvoiceNumber, string paymentpurpose)
        {
            //string url = "";
            try
            {
               
                var payment = new Payment();
                var paymentLogic = new PaymentLogic();
                string paymentStatus = "FEE HAS NOT BEEN PAID";

                payment = await paymentLogic.GetByAsync(InvoiceNumber);
                if (payment == null)
                {
                    CreateErrorTree();
                    return;
                }

                if (await paymentLogic.PaymentAlreadyMadeAsync(payment))
                {
                    paymentStatus = "FEE HAS BEEN PAID";
                }

                var paymentEtranzactType = new PaymentEtranzactType();
                var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                paymentEtranzactType = await paymentEtranzactTypeLogic.GetModelsByFODAsync(m =>
                            m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id &&
                            m.Session_Id == payment.Session.Id && m.Payment_Mode_Id == payment.PaymentMode.Id)
                         ;
                if (paymentEtranzactType == null)
                {
                    CreateErrorTree();
                    return;
                }

                var person = new Person();
                var personLogic = new PersonLogic();
                person = await personLogic.GetModelByAsync(p => p.Person_Id == payment.Person.Id);
                if (person != null)
                {
                    string fullname = person.FullName;
                    string faculty = "";
                    string dept = "";
                    string level = "";
                    string studenttypeid = "";
                    string modeofentry = "";
                    string sessionid = "";
                    string Amount = "";
                    string paymentstatus = "";
                    string PaymentType = "";
                    string phoneNo = person.MobilePhone;
                    string email = person.Email;
                    string MatricNo = "";
                    string levelid = "";
                    string PaymentCategory = "";
                    string semester = "";
                    string semesterid = "";
                    string shortFallAmount = "";

                    var appliedCourse = new AppliedCourse();
                    var appliedCourseLogic = new AppliedCourseLogic();
                    appliedCourse = await appliedCourseLogic.GetModelByAsync(m => m.Person_Id == payment.Person.Id);
                    if (appliedCourse != null)
                    {
                        paymentEtranzactType = await
                   paymentEtranzactTypeLogic.GetModelsByFODAsync(
                       m =>
                           m.Payment_Etranzact_Type_Name == paymentpurpose && m.Fee_Type_Id == payment.FeeType.Id &&
                           m.Session_Id == payment.Session.Id && m.Payment_Mode_Id == payment.PaymentMode.Id && m.Programme_Id == appliedCourse.Programme.Id)
                       ;
                    }
                    var admissionList = new AdmissionList();
                    var admissionListLogic = new AdmissionListLogic();
                    admissionList = await admissionListLogic.GetByAsync(person);
                    var student = new Student();
                    var studentLogic = new StudentLogic();
                    student = await studentLogic.GetByAsync(payment.Person.Id);
                    var studentLevel = new StudentLevel();
                    var levelLogic = new StudentLevelLogic();
                    if (student != null)
                    {
                        studentLevel = await levelLogic.GetByAsync(student.Id);
                        if (studentLevel != null)
                        {
                            paymentEtranzactType = await
                             paymentEtranzactTypeLogic.GetModelsByFODAsync(m =>
                                     m.Payment_Etranzact_Type_Name.Contains(paymentpurpose) && m.Fee_Type_Id == payment.FeeType.Id &&
                                     m.Session_Id == payment.Session.Id && m.Payment_Mode_Id == payment.PaymentMode.Id && m.Programme_Id == studentLevel.Programme.Id);

                        }
                    }

                    Decimal PaymentAmount = 0;
                    if ((student != null && studentLevel != null && payment.FeeType.Id == 3) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 17) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 2) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 4) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 5) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 19) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 20) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 21) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 22) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 23) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 24) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 25) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 26) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 27) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 13) ||
                        (student != null && studentLevel != null && payment.FeeType.Id == 14))
                    {
                        var studentPaymentLogic = new StudentPaymentLogic();
                        StudentPayment studentPayment = await studentPaymentLogic.GetModelByAsync(a => a.Payment_Id == payment.Id);
                        if (studentPayment != null)
                        {
                            faculty = studentLevel.Department.Faculty.Name;
                            dept = studentLevel.Department.Name;
                            level = studentLevel.Level.Name;
                            studenttypeid = studentLevel.Programme.Name;
                            modeofentry = "N/A";
                            sessionid = payment.Session.Name;
                            paymentstatus = paymentStatus;
                            PaymentType = paymentEtranzactType.Name;
                            phoneNo = person.MobilePhone;
                            email = person.Email;
                            MatricNo = studentLevel.Student.MatricNumber;
                            levelid = studentLevel.Level.Id.ToString();
                            PaymentCategory = paymentEtranzactType.Name;
                            semester = "N/A";
                            Amount = studentPayment.Amount.ToString();
                        }
                        else
                        {
                            CreateErrorTree();
                            return;
                        }
                    }
                    else if ((admissionList != null && payment.FeeType.Id == 3) || (admissionList != null && payment.FeeType.Id == 2) || (admissionList != null && payment.FeeType.Id == 4) || (admissionList != null && payment.FeeType.Id == 5))
                    {
                        PaymentAmount = await paymentLogic.SetFeeDetailsAsync(payment, admissionList.Programme.Id, 1,
                                payment.PaymentMode.Id, admissionList.Deprtment.Id, payment.Session.Id);

                        faculty = admissionList.Deprtment.Faculty.Name;
                        dept = admissionList.Deprtment.Name;
                        studenttypeid = admissionList.Programme.Name;
                        level = "N/A";
                        modeofentry = "N/A";
                        sessionid = payment.Session.Name; ;
                        //payment.FeeDetails = await paymentLogic.SetFeeDetailsAsync(payment.FeeType);
                        Amount = PaymentAmount.ToString();
                        paymentstatus = paymentStatus;
                        PaymentType = paymentEtranzactType.Name;
                        phoneNo = person.MobilePhone;
                        email = person.Email;
                        MatricNo = InvoiceNumber;
                        levelid = "N/A";
                        PaymentCategory = paymentEtranzactType.Name;
                        semester = "N/A";
                        Amount = PaymentAmount.ToString();
                    }
                    else if (appliedCourse != null && appliedCourse.Programme.Id == paymentEtranzactType.programme.Id && (payment.FeeType.Id == 1 || payment.FeeType.Id == 4 ||
                             payment.FeeType.Id == 5 || payment.FeeType.Id == 6 || payment.FeeType.Id == 17 ||
                             payment.FeeType.Id == 18))
                    {
                        PaymentAmount = await paymentLogic.SetFeeDetailsAsync(payment, appliedCourse.Programme.Id, 1,
                                payment.PaymentMode.Id, appliedCourse.Department.Id, payment.Session.Id);

                        faculty = appliedCourse.Department.Faculty.Name;
                        dept = appliedCourse.Department.Name;
                        studenttypeid = appliedCourse.Programme.Name;
                        level = "N/A";
                        modeofentry = "N/A";
                        sessionid = payment.Session.Name;
                       // payment.FeeDetails = await paymentLogic.SetFeeDetailsAsync(payment.FeeType);
                        Amount = PaymentAmount.ToString();
                        paymentstatus = paymentStatus;
                        PaymentType = paymentEtranzactType.Name;
                        phoneNo = person.MobilePhone;
                        email = person.Email;
                        MatricNo = InvoiceNumber;
                        levelid = "N/A";
                        PaymentCategory = paymentEtranzactType.Name;
                        semester = "N/A";
                    }

                    else
                    {
                        var shortFallLogic = new ShortFallLogic();
                        var shortFall = new ShortFall();
                        if (payment.FeeType.Id == 12)
                        {
                            paymentEtranzactType = await paymentEtranzactTypeLogic.GetModelsByFODAsync(m => m.Payment_Etranzact_Type_Name == paymentpurpose);
                            shortFall = shortFallLogic.GetModelBy(p => p.Payment_Id == payment.Id);
                            if (shortFall != null)
                            {
                                shortFallAmount = shortFall.Amount.ToString();
                                faculty = studentLevel.Department.Faculty.Name;
                                dept = studentLevel.Department.Name;
                                level = studentLevel.Level.Name;
                                studenttypeid = studentLevel.Programme.Name;
                                modeofentry = "N/A";
                                sessionid = payment.Session.Name;
                                Amount = shortFallAmount;
                                paymentstatus = paymentStatus;
                                PaymentType = paymentEtranzactType.Name;
                                phoneNo = person.MobilePhone;
                                email = person.Email;
                                MatricNo = studentLevel.Student.MatricNumber;
                                levelid = studentLevel.Level.Id.ToString();
                                PaymentCategory = paymentEtranzactType.Name;
                                semester = "N/A";
                            }
                            else
                            {
                                CreateErrorTree();
                            }
                        }
                        else
                        {
                            paymentEtranzactType = await paymentEtranzactTypeLogic.GetModelsByFODAsync(
                                    m =>
                                        m.Payment_Etranzact_Type_Name == paymentpurpose &&
                                        m.Fee_Type_Id == payment.FeeType.Id);

                            if (paymentEtranzactType == null)
                            {
                                CreateErrorTree();
                                return;
                            }

                            faculty = studentLevel.Department.Faculty.Name;
                            dept = studentLevel.Department.Name;
                            level = studentLevel.Level.Name;
                            studenttypeid = studentLevel.Programme.Name;
                            modeofentry = "N/A";
                            sessionid = payment.Session.Name;
                            Amount = payment.FeeDetails.Sum(a => a.Fee.Amount).ToString();
                            paymentstatus = paymentStatus;
                            PaymentType = paymentEtranzactType.Name;
                            phoneNo = person.MobilePhone;
                            email = person.Email;
                            MatricNo = studentLevel.Student.MatricNumber;
                            levelid = studentLevel.Level.Id.ToString();
                            PaymentCategory = paymentEtranzactType.Name;
                            semester = "N/A";
                        }
                    }


                    CreateTree(fullname, faculty, dept, level, studenttypeid, modeofentry, sessionid, InvoiceNumber, Amount, paymentstatus, semester, PaymentType, MatricNo, email, phoneNo, PaymentCategory);

                    return;
                }
            }
            catch (Exception ex)
            {
                //CreateErrorTree(url);
                //return;
                throw ex;
            }
        }

        private string GetAndUpdateAmount(StudentLevel studentLevel, Payment payment)
        {
            string amount = "";
            try
            {
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                FeeDetail feeDetail =
                    feeDetailLogic.GetModelsBy(
                        f =>
                            f.Department_Id == studentLevel.Department.Id && f.Fee_Type_Id == payment.FeeType.Id &&
                            f.Level_Id == studentLevel.Level.Id
                            && f.Payment_Mode_Id == payment.PaymentMode.Id &&
                            f.Programme_Id == studentLevel.Programme.Id && f.Session_Id == payment.Session.Id).LastOrDefault();
                if (feeDetail != null)
                {
                    StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                    StudentPayment studentPayment = studentPaymentLogic.GetModelBy(p => p.Payment_Id == payment.Id);

                    if (studentPayment != null)
                    {
                        studentPayment.Amount = feeDetail.Fee.Amount;

                        studentPaymentLogic.Modify(studentPayment);
                    }

                    amount = feeDetail.Fee.Amount.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return amount;
        }

        //private void CreateTree(string payeename, string Faculty, string Department, string Level, string ProgrammeType,
        //    string StudyType, string Session, string PayeeID, string Amount, string FeeStatus, string Semester,
        //    string PaymentType, string MatricNumber, string Email, string PhoneNumber, string category, string url)
        //{
        //    try
        //    {
        //        var fs = new FileStream(Server.MapPath(url), FileMode.Create);
        //        fs.Close();


        //        var writer = new XmlTextWriter(Server.MapPath(url), Encoding.UTF8);
        //        writer.WriteStartDocument(true);
        //        writer.Formatting = Formatting.Indented;
        //        writer.Indentation = 1;

        //        writer.WriteStartElement("FeeRequest");
        //        createNode(payeename, Faculty, Department, Level, ProgrammeType, StudyType, Session, PayeeID, Amount,
        //            FeeStatus, Semester, PaymentType, MatricNumber, Email, PhoneNumber, category, writer);
        //        writer.WriteEndElement();
        //        writer.WriteEndDocument();
        //        writer.Close();

        //        WriteXmlToPage(Server.MapPath(url));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private void CreateTree(string payeename, string Faculty, string Department, string Level, string ProgrammeType,
           string StudyType, string Session, string PayeeID, string Amount, string FeeStatus, string Semester,
           string PaymentType, string MatricNumber, string Email, string PhoneNumber, string category)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(memoryStream, System.Text.Encoding.UTF8))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.Indentation = 4;

                    xmlWriter.WriteStartDocument(true);
                    xmlWriter.WriteStartElement("FeeRequest");

                    createNode(payeename,
                        Faculty,
                        Department,
                        Level,
                        ProgrammeType,
                        StudyType,
                        Session,
                        PayeeID,
                        Amount,
                        FeeStatus,
                        Semester,
                        PaymentType,
                        MatricNumber,
                        Email,
                        PhoneNumber,
                        category,
                        xmlWriter);

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();

                    WriteXmlToPage(memoryStream);
                }
            }
        }

        private void CreateTree(string payeename, string Faculty, string Department, string Level, string ProgrammeType,
           string StudyType, string Session, string PayeeID, string Amount, string FeeStatus, string Semester,
           string PaymentType, string MatricNumber, string Email, string PhoneNumber, string category, string url)
        {
            try
            {
                var fs = new FileStream(Server.MapPath(url), FileMode.Create);
                fs.Close();


                var writer = new XmlTextWriter(Server.MapPath(url), Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 1;

                writer.WriteStartElement("FeeRequest");
                createNode(payeename, Faculty, Department, Level, ProgrammeType, StudyType, Session, PayeeID, Amount,
                    FeeStatus, Semester, PaymentType, MatricNumber, Email, PhoneNumber, category, writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

                WriteXmlToPage(Server.MapPath(url));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void createNode(string payeeName, string Faculty, string Department, string Level, string ProgrammeType,
            string StudyType, string Session, string PayeeID, string Amount, string FeeStatus, string Semester,
            string PaymentType, string MatricNumber, string Email, string PhoneNumber, string category,
            XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("PayeeName");
                writer.WriteString(payeeName);
                writer.WriteEndElement();

                writer.WriteStartElement("Faculty");
                writer.WriteString(Faculty);
                writer.WriteEndElement();

                writer.WriteStartElement("Department");
                writer.WriteString(Department);
                writer.WriteEndElement();

                writer.WriteStartElement("Level");
                writer.WriteString(Level);
                writer.WriteEndElement();

                writer.WriteStartElement("ProgrammeType");
                writer.WriteString(ProgrammeType);
                writer.WriteEndElement();

                writer.WriteStartElement("StudyType");
                writer.WriteString(StudyType);
                writer.WriteEndElement();

                writer.WriteStartElement("Session");
                writer.WriteString(Session);
                writer.WriteEndElement();

                writer.WriteStartElement("PayeeID");
                writer.WriteString(PayeeID);
                writer.WriteEndElement();

                writer.WriteStartElement("Amount");
                writer.WriteString(Amount);
                writer.WriteEndElement();

                writer.WriteStartElement("FeeStatus");
                writer.WriteString(FeeStatus);
                writer.WriteEndElement();

                writer.WriteStartElement("Semester");
                writer.WriteString(Semester);
                writer.WriteEndElement();

                writer.WriteStartElement("PaymentType");
                writer.WriteString(PaymentType);
                writer.WriteEndElement();


                writer.WriteStartElement("PaymentCategory");
                writer.WriteString(category);
                writer.WriteEndElement();

                writer.WriteStartElement("MatricNumber");
                writer.WriteString(MatricNumber);
                writer.WriteEndElement();

                writer.WriteStartElement("Email");
                writer.WriteString(Email);
                writer.WriteEndElement();

                writer.WriteStartElement("PhoneNumber");
                writer.WriteString(PhoneNumber);
                writer.WriteEndElement();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void CreateErrorTree()
        {
            try
            {
              
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(memoryStream, System.Text.Encoding.UTF8))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlWriter.Indentation = 1;

                        xmlWriter.WriteStartDocument(true);
                        xmlWriter.WriteStartElement("FeeRequest");
                        xmlWriter.WriteStartElement("Error");
                        xmlWriter.WriteString("-1");
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndDocument();
                        xmlWriter.Flush();

                        WriteXmlToPage(memoryStream);
                        //xmlWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateErrorTree(string url)
        {
            try
            {
                var fs = new FileStream(Server.MapPath(url), FileMode.Create);
                fs.Close();

                var writer = new XmlTextWriter(Server.MapPath(url), Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 1;

                writer.WriteStartElement("FeeRequest");
                writer.WriteStartElement("Error");
                writer.WriteString("-1");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

                WriteXmlToPage(Server.MapPath(url));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void WriteXmlToPage(string url)
        {
            Response.Clear();
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            WebRequest req = WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
            var sr = new StreamReader(resp.GetResponseStream());
            var t = new RichTextBox();
            t.Text = sr.ReadToEnd().Trim();
            sr.Close();

            for (int i = 0; i <= t.Lines.GetUpperBound(0); i++)
            {
                Response.Write(t.Lines[i]);
            }

            File.Delete(url);
            Response.End();
        }

        private void WriteXmlToPage(MemoryStream memoryStream)
        {
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                Response.Clear();
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                System.Windows.Forms.RichTextBox t = new System.Windows.Forms.RichTextBox();
                t.Text = streamReader.ReadToEnd().Trim();
                streamReader.Close();

                for (int i = 0; i <= t.Lines.GetUpperBound(0); i++)
                {
                    Response.Write(t.Lines[i].ToString());
                }

                Response.End();
            }
        }

        private Level SetLevel(Programme programme)
        {
            try
            {
                Level level;
                switch (programme.Id)
                {
                    case 1:
                    {
                        return level = new Level {Id = 1};
                    }
                    case 2:
                    {
                        return level = new Level {Id = 2};
                    }
                    case 3:
                    {
                        return level = new Level {Id = 3};
                    }
                    case 4:
                    {
                        return level = new Level {Id = 4};
                    }
                    case 5:
                    {
                        return level = new Level {Id = 5};
                    }
                    case 6:
                    {
                        return level = new Level {Id = 6};
                    }
                }
                return level = new Level();
            }
            catch (Exception)
            {
                throw;
            }
        }
   
    }
}