using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class HostelLogic : BusinessBaseLogic<Hostel,HOSTEL>
    {
        public HostelLogic()
        {
            translator = new HostelTranslator();
        }

        public List<HostelAllocationReport> GetHostelReportBy(Session session, Hostel hostel)
        {
            try
            {
                HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                List<HostelAllocationReport> hostelAllocationReports = new List<HostelAllocationReport>();
                StudentLogic studentLogic = new StudentLogic();
                SessionLogic sessionLogic = new SessionLogic();
                List<HostelAllocationReport> activatedBedSpaces = (from p in repository.GetBy<VW_ACTIVATED_BED_SPACES>(p => p.Hostel_Id == hostel.Id)
                                                    select new HostelAllocationReport
                    {
                        HostelId = p.Hostel_Id,
                        HostelName = p.Hostel_Name,
                        SeriesId =  p.Series_Id,
                        SeriesName = p.Series_Name,
                        RoomId = p.Room_Id,
                        RoomNumber = p.Room_Number,
                        Reserved = p.Reserved,
                        CornerId =  p.Corner_Id,
                        CornerName =  p.Corner_Name

                    }).ToList();

               
                var sessionId = session.Id;
                session = sessionLogic.GetModelBy(s => s.Session_Id == sessionId);
                List<HostelAllocation> hostelAllocations = hostelAllocationLogic.GetModelsBy(h => h.Hostel_Id == hostel.Id && h.Session_Id == sessionId);
                for (int i = 0; i < activatedBedSpaces.Count; i++)
                {
                    var seriesId = activatedBedSpaces[i].SeriesId;
                    var roomId = activatedBedSpaces[i].RoomId;
                    var cornerId = activatedBedSpaces[i].CornerId;

                    var isAllocatedRoom = hostelAllocations.LastOrDefault(s => s.Series.Id == seriesId && s.Room.Id == roomId && s.Corner.Id == cornerId);
                   
                    HostelAllocationReport hostelAllocationReport = new HostelAllocationReport();
                    if (isAllocatedRoom == null)
                    {
                      
                        hostelAllocationReport.HostelName = activatedBedSpaces[i].HostelName;
                        hostelAllocationReport.MatricNumber = "-";
                        hostelAllocationReport.FullName = "-";
                        hostelAllocationReport.CornerName = activatedBedSpaces[i].CornerName;
                        hostelAllocationReport.SessionName = session.Name;
                        hostelAllocationReport.Reserved = false;
                        hostelAllocationReport.SeriesName = activatedBedSpaces[i].SeriesName;
                        hostelAllocationReport.RoomId = activatedBedSpaces[i].RoomId;
                        hostelAllocationReport.RoomNumber = activatedBedSpaces[i].RoomNumber;
                        hostelAllocationReport.HostelId = activatedBedSpaces[i].HostelId;
                    }
                    else
                    {
                        Student student = studentLogic.GetBy(isAllocatedRoom.Person.Id);

                        if (student == null || student.FullName==null || student.MatricNumber==null)
                        {

                        }


                        hostelAllocationReport.HostelName = activatedBedSpaces[i].HostelName;
                        hostelAllocationReport.MatricNumber = student.MatricNumber;
                        hostelAllocationReport.FullName = student.FullName;
                        hostelAllocationReport.CornerName = activatedBedSpaces[i].CornerName;
                        hostelAllocationReport.SessionName = session.Name;
                        hostelAllocationReport.Reserved = true;
                        hostelAllocationReport.SeriesName = activatedBedSpaces[i].SeriesName;
                        hostelAllocationReport.RoomId = activatedBedSpaces[i].RoomId;
                        hostelAllocationReport.RoomNumber = activatedBedSpaces[i].RoomNumber;
                        hostelAllocationReport.HostelId = activatedBedSpaces[i].HostelId;
                    }
                    hostelAllocationReports.Add(hostelAllocationReport);
                }
                return hostelAllocationReports.OrderBy(s => s.RoomId).ToList();
            }

            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
