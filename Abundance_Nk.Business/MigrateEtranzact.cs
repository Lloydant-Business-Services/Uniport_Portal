using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Business
{
    
    public class MigrateEtranzact
    {
        private Payment _payment;
        private StudentPayment _studentPayment;
        private Decimal _Amount;
        private Level _level;
        private PaymentEtranzactLogic paymentEtranzactLogic;
        private StudentLogic studentLogic;
        private PaymentLogic paymentLogic;
        private StudentPaymentLogic studentPaymentLogic;
        private OnlinePaymentLogic onlinePaymentLogic;
        private ExcelData _excelData;
        private PaymentMode _paymentMode;
        private Student _student;
        private FeeType _feeType;
        private PaymentTerminal _paymentTerminal;
        public MigrateEtranzact(ExcelData excelData)
        {
            _feeType = new FeeType(){Id = (int)FeeTypes.SchoolFees};
            _paymentTerminal = new PaymentTerminal(){Id =5,TerminalId = "0110000127"};
            _excelData = excelData;
            studentLogic = new StudentLogic();
            paymentLogic = new PaymentLogic();
            onlinePaymentLogic = new OnlinePaymentLogic();
            studentPaymentLogic = new StudentPaymentLogic();
            paymentEtranzactLogic = new PaymentEtranzactLogic();
            ProcessData(_excelData);
        }
        private void  ProcessData(ExcelData _excelData)
        {
            try
            {
                if (_excelData != null && _excelData.MatricNumber != null)
                {
                    if (studentLogic.DoesStudentExist(_excelData.MatricNumber))
                    {
                        
                        _student = studentLogic.GetBy(_excelData.MatricNumber);
                        _paymentMode = GetPaymentMode(_excelData.Semester);
                        _payment = GenerateInvoice(_student, _paymentMode);
                        _Amount = GetAmount(_excelData);
                        _level = GetLevel(_excelData);
                        if (_Amount != null && _Amount > 0 && _level != null && _level.Id > 0)
                        {
                            _studentPayment = CreateStudentPaymentLog(_student, _payment, _Amount, _level);
                             if (_student != null && _paymentMode != null && _payment != null)
                            {
                                 paymentEtranzactLogic.RetrievePinsWithoutInvoice(_excelData.ConfirmationNumber,_payment.InvoiceNumber,_feeType,_paymentTerminal);
                            
                            }
                        }
                        
                       

                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
           
        }

        private Payment GenerateInvoice(Student student,PaymentMode paymentMode)
        {
            Payment payment = null;
            try
            {
                if (student != null && student.Id > 0)
                {
                    payment = new Payment();
                    payment.PaymentMode = paymentMode;
                    payment.PaymentType = new PaymentType(){Id = 2};
                    payment.Person = new Person() {Id = student.Id};
                    payment.PersonType = new PersonType(){Id = 3};
                    payment.FeeType = _feeType;
                    payment.DatePaid = DateTime.Now;
                    payment.Session = new Session(){Id = 1};
                    payment = paymentLogic.Create(payment);

                    OnlinePayment onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = new PaymentChannel(){Id = 1};
                    onlinePayment.Payment = payment;
                    onlinePayment.TransactionDate = DateTime.Now;
                    onlinePaymentLogic.Create(onlinePayment);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return payment;
        }

        private PaymentMode GetPaymentMode(string Semester)
        {
            PaymentMode paymentMode = null;
            try
            {
                switch (Semester)
                {
                    case  "FIRST SEMESTER":
                        return paymentMode = new PaymentMode() {Id = 2};
                        break;
                    case  "SECOND SEMESTER":
                        return paymentMode = new PaymentMode() {Id = 3};
                        break;
                    case  "BOTH":
                        return paymentMode = new PaymentMode() {Id = 1};
                        break;
                    default:
                        return paymentMode = new PaymentMode() {Id = 2};
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private StudentPayment CreateStudentPaymentLog(Student student, Payment payment,Decimal Amount, Level level)
        {
            try
            {
                
                var studentPayment = new StudentPayment();
                studentPayment.Id = payment.Id;
                studentPayment.Level = level;
                studentPayment.Session = new Session(){Id = 1};
                studentPayment.Student = student;
                studentPayment.Amount = Amount;
                studentPayment.Status = false;
                studentPayment = studentPaymentLogic.Create(studentPayment);
                return studentPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Decimal GetAmount(ExcelData _excelData)
        {
           return Convert.ToDecimal(_excelData.Amount) - 350;
        }

        private Level GetLevel(ExcelData _excelData)
        {
           Level level = null;
            try
            {
                switch (_excelData.Level)
                {
                    case  "100 LEVEL":
                        return level = new Level() {Id = 1};
                        break;
                    case  "200 LEVEL":
                        return level = new Level() {Id = 2};
                        break;
                    case  "300 LEVEL":
                        return level = new Level() {Id = 3};
                        break;
                    case  "400 LEVEL":
                        return level = new Level() {Id = 4};
                        break;
                    case  "500 LEVEL":
                        return level = new Level() {Id = 5};
                        break;
                    case  "600 LEVEL":
                        return level = new Level() {Id = 6};
                        break;
                    default:
                        return level = new Level() {Id = 1};
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }

    public class ExcelData
    {
        public string MatricNumber { get; set; }
        public decimal Amount { get; set; }
        public string FullName { get; set; }
        public string ConfirmationNumber { get; set; }
        public string PaymentType { get; set; }
        public string Programme { get; set; }
        public string Level { get; set; }
        public string Semester { get; set; }
    }

    public class MigrateExcelData
    {
        
        private List<ExcelData> excelDatas;
        Abundance_NkEntities _entities = new Abundance_NkEntities();

        public List<ExcelData> GetExcelDatas()
        {
            excelDatas = new List<ExcelData>();
           
            var data = _entities.MIGRATION_ETRANZACT;
            foreach (MIGRATION_ETRANZACT migrationEtranzact in data)
            {
                ExcelData excelData = new ExcelData();
                excelData.FullName = migrationEtranzact.FULLNAME;
                excelData.MatricNumber = migrationEtranzact.Matric_No_;
                excelData.ConfirmationNumber = migrationEtranzact.CONFIRMATION_CODE;
                excelData.Amount = Convert.ToDecimal(migrationEtranzact.AMOUNT);
                excelData.Semester = migrationEtranzact.SEMESTER;
                excelData.PaymentType = migrationEtranzact.PAYMENT_DETAILS;
                excelData.Programme = migrationEtranzact.PROGRAMME;
                excelData.Level = migrationEtranzact.LEVEL;
                excelDatas.Add(excelData);
            }
            return excelDatas;
        }
    }

}
