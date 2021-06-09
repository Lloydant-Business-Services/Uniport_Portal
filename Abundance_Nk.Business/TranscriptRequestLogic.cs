using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class TranscriptRequestLogic : BusinessBaseLogic<TranscriptRequest, TRANSCRIPT_REQUEST>
    {
        public TranscriptRequestLogic()
        {
            translator = new TranscriptRequestTranslator();
        }

        public TranscriptRequest GetBy(long Id)
        {
            TranscriptRequest request = null;
            try
            {
                request = GetModelBy(a => a.Student_id == Id && a.Transcript_Status_Id < 5);
            }
            catch (Exception)
            {
                throw;
            }
            return request;
        }
        public TranscriptRequest GetBy(Payment payment)
        {
            TranscriptRequest request = null;
            try
            {
                request = GetModelBy(a => a.Transcript_Status_Id < 5 && a.Payment_Id == payment.Id);
            }
            catch (Exception)
            {
                throw;
            }
            return request;
        }
        public List<TranscriptRequest> GetBy(Student student)
        {
            List<TranscriptRequest> request = null;
            try
            {
                request = GetModelsBy(a => a.Student_id == student.Id);
            }
            catch (Exception)
            {
                throw;
            }
            return request;
        }

        public bool Modify(TranscriptRequest model)
        {
            try
            {
                Expression<Func<TRANSCRIPT_REQUEST, bool>> selector = af => af.Transcript_Request_Id == model.Id;
                TRANSCRIPT_REQUEST entity = GetEntityBy(selector);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.payment != null && model.payment.Id > 0)
                {
                    entity.Payment_Id = model.payment.Id;
                }

                entity.Destination_Address = model.DestinationAddress;
                entity.Destination_State_Id = model.DestinationState.Id;
                entity.Destination_Country_Id = model.DestinationCountry.Id;
                entity.Transcript_clearance_Status_Id = model.transcriptClearanceStatus.TranscriptClearanceStatusId;
                entity.Transcript_Status_Id = model.transcriptStatus.TranscriptStatusId;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TranscriptRequest> GetProcessedTranscripts()
        {
            List<TranscriptRequest> transcriptRequests = new List<TranscriptRequest>();
            try
            {
                List<TranscriptRequestModel> transcriptRequestInterswitch = (from sr in repository.GetBy<VW_TRANSCRIPT_REQUEST_BY_INTERSWITCH>(s => s.Transcript_Status_Id == 4)
                                                                             select new TranscriptRequestModel
                                                                   {
                                                                       TranscriptRequestId = sr.Transcript_Request_Id,
                                                                       PaymentId = sr.Payment_Id,
                                                                       Studentid = sr.Payment_Id,
                                                                       Name = sr.NAME,
                                                                       RegNo = sr.RegNo,
                                                                       Email = sr.Email,
                                                                       DepartmentName = sr.Department_Name,
                                                                       DepartmentId = sr.Department_Id,
                                                                       ProgrammeId = sr.Programme_Id,
                                                                       LevelId = sr.Level_Id,
                                                                       FacultyId = sr.Faculty_Id,
                                                                       DateRequested = sr.Date_Requested,
                                                                       DestinationAddress = sr.Destination_Address,
                                                                       DestinationStateId = sr.Destination_State_Id,
                                                                       DestinationCountryId = sr.Destination_Country_Id,
                                                                       TranscriptClearanceStatusId = sr.Transcript_clearance_Status_Id,
                                                                       TranscriptStatusId = sr.Transcript_Status_Id,
                                                                       TranscriptStatusName = sr.Transcript_Status_Name,
                                                                       DeliveryService = sr.Delivery_Service,
                                                                       DeliveryZone = sr.Delivery_Zone
                                                                   }).ToList();

                List<TranscriptRequestModel> transcriptRequestEtranzact = (from sr in repository.GetBy<VW_TRANSCRIPT_REQUEST_BY_ETRANZACT>(s => s.Transcript_Status_Id == 4)
                                                                           select new TranscriptRequestModel
                                                                           {
                                                                               TranscriptRequestId = sr.Transcript_Request_Id,
                                                                               PaymentId = sr.Payment_Id,
                                                                               Studentid = sr.Payment_Id,
                                                                               Name = sr.Name,
                                                                               RegNo = sr.RegNo,
                                                                               Email = sr.Email,
                                                                               DepartmentName = sr.Department_Name,
                                                                               DepartmentId = sr.Department_Id,
                                                                               ProgrammeId = sr.Programme_Id,
                                                                               LevelId = sr.Level_Id,
                                                                               FacultyId = sr.Faculty_Id,
                                                                               DateRequested = sr.Date_Requested,
                                                                               DestinationAddress = sr.Destination_Address,
                                                                               DestinationStateId = sr.Destination_State_Id,
                                                                               DestinationCountryId = sr.Destination_Country_Id,
                                                                               TranscriptClearanceStatusId = sr.Transcript_clearance_Status_Id,
                                                                               TranscriptStatusId = sr.Transcript_Status_Id,
                                                                               TranscriptStatusName = sr.Transcript_Status_Name,
                                                                               DeliveryService = sr.Delivery_Service,
                                                                               DeliveryZone = sr.Delivery_Zone
                                                                           }).ToList();

                foreach (var model in transcriptRequestInterswitch)
                {
                    TranscriptRequest request = new TranscriptRequest();

                    request.Name = model.Name;
                    request.student = new Student()
                    {
                        MatricNumber = model.RegNo
                    };
                    request.transcriptClearanceStatus = new TranscriptClearanceStatus()
                    {
                        TranscriptClearanceStatusName = model.TranscriptStatusName,
                    };
                    request.transcriptStatus = new TranscriptStatus()
                    {
                        TranscriptStatusId = model.TranscriptStatusId
                    };
                    request.DeliveryService = model.DeliveryService;
                     request.DateRequested = model.DateRequested;
                    request.DestinationAddress = model.DestinationAddress;
                    request.Id = model.TranscriptRequestId;

                    transcriptRequests.Add(request);
                }

                foreach (var model in transcriptRequestEtranzact)
                {
                    TranscriptRequest request = new TranscriptRequest();
                    request.Name = model.Name;
                    request.student = new Student()
                    {
                        MatricNumber =  model.RegNo
                    };
                    request.transcriptClearanceStatus = new TranscriptClearanceStatus()
                    {
                        TranscriptClearanceStatusName = model.TranscriptStatusName,
                    };
                    request.transcriptStatus = new TranscriptStatus()
                    {
                        TranscriptStatusId = model.TranscriptStatusId
                    };
                    request.DeliveryService = model.DeliveryService;
                    request.DateRequested = model.DateRequested;
                    request.DestinationAddress = model.DestinationAddress;
                    request.Id = model.TranscriptRequestId;

                    transcriptRequests.Add(request);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return transcriptRequests;
        }

        public List<TranscriptRequest> GetDispatchedTranscripts()
        {
            List<TranscriptRequest> transcriptRequests = new List<TranscriptRequest>();
            try
            {
                
            List<TranscriptRequestModel> transcriptRequestInterswitch = (from sr in repository.GetBy<VW_TRANSCRIPT_REQUEST_BY_INTERSWITCH>(s => s.Transcript_Status_Id == 5)
                                                                             select new TranscriptRequestModel
                                                                   {
                                                                       TranscriptRequestId = sr.Transcript_Request_Id,
                                                                       PaymentId = sr.Payment_Id,
                                                                       Studentid = sr.Payment_Id,
                                                                       Name = sr.NAME,
                                                                       RegNo = sr.RegNo,
                                                                       Email = sr.Email,
                                                                       DepartmentName = sr.Department_Name,
                                                                       DepartmentId = sr.Department_Id,
                                                                       ProgrammeId = sr.Programme_Id,
                                                                       LevelId = sr.Level_Id,
                                                                       FacultyId = sr.Faculty_Id,
                                                                       DateRequested = sr.Date_Requested,
                                                                       DestinationAddress = sr.Destination_Address,
                                                                       DestinationStateId = sr.Destination_State_Id,
                                                                       DestinationCountryId = sr.Destination_Country_Id,
                                                                       TranscriptClearanceStatusId = sr.Transcript_clearance_Status_Id,
                                                                       TranscriptStatusId = sr.Transcript_Status_Id,
                                                                       TranscriptStatusName = sr.Transcript_Status_Name,
                                                                       DeliveryService = sr.Delivery_Service,
                                                                       DeliveryZone = sr.Delivery_Zone
                                                                   }).ToList();

                List<TranscriptRequestModel> transcriptRequestEtranzact = (from sr in repository.GetBy<VW_TRANSCRIPT_REQUEST_BY_ETRANZACT>(s => s.Transcript_Status_Id == 5)
                                                                           select new TranscriptRequestModel
                                                                           {
                                                                               TranscriptRequestId = sr.Transcript_Request_Id,
                                                                               PaymentId = sr.Payment_Id,
                                                                               Studentid = sr.Payment_Id,
                                                                               Name = sr.Name,
                                                                               RegNo = sr.RegNo,
                                                                               Email = sr.Email,
                                                                               DepartmentName = sr.Department_Name,
                                                                               DepartmentId = sr.Department_Id,
                                                                               ProgrammeId = sr.Programme_Id,
                                                                               LevelId = sr.Level_Id,
                                                                               FacultyId = sr.Faculty_Id,
                                                                               DateRequested = sr.Date_Requested,
                                                                               DestinationAddress = sr.Destination_Address,
                                                                               DestinationStateId = sr.Destination_State_Id,
                                                                               DestinationCountryId = sr.Destination_Country_Id,
                                                                               TranscriptClearanceStatusId = sr.Transcript_clearance_Status_Id,
                                                                               TranscriptStatusId = sr.Transcript_Status_Id,
                                                                               TranscriptStatusName = sr.Transcript_Status_Name,
                                                                               DeliveryService = sr.Delivery_Service,
                                                                               DeliveryZone = sr.Delivery_Zone
                                                                           }).ToList();

                foreach (var model in transcriptRequestInterswitch)
                {
                    TranscriptRequest request = new TranscriptRequest();

                    request.Name = model.Name;
                    request.student = new Student()
                    {
                        MatricNumber = model.RegNo
                    };
                    request.transcriptClearanceStatus = new TranscriptClearanceStatus()
                    {
                        TranscriptClearanceStatusName = model.TranscriptStatusName,
                    };
                    request.transcriptStatus = new TranscriptStatus()
                    {
                        TranscriptStatusId = model.TranscriptStatusId
                    };
                    request.DateRequested = model.DateRequested;
                    request.DestinationAddress = model.DestinationAddress;
                    request.Id = model.TranscriptRequestId;

                    transcriptRequests.Add(request);
                }

                foreach (var model in transcriptRequestEtranzact)
                {
                    TranscriptRequest request = new TranscriptRequest();
                    request.Name = model.Name;
                    request.student = new Student()
                    {
                        MatricNumber =  model.RegNo
                    };
                    request.transcriptClearanceStatus = new TranscriptClearanceStatus()
                    {
                        TranscriptClearanceStatusName = model.TranscriptStatusName,
                    };
                    request.transcriptStatus = new TranscriptStatus()
                    {
                        TranscriptStatusId = model.TranscriptStatusId
                    };
                    request.DateRequested = model.DateRequested;
                    request.DestinationAddress = model.DestinationAddress;
                    request.Id = model.TranscriptRequestId;

                    transcriptRequests.Add(request);
                }
            }
            catch (Exception ex)
            {
                    
                throw;
            }
            return transcriptRequests;
        }
    }
   
}