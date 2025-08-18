using AutoMapper;
using Domain.Contracts;
using Domain.Entities.CoreEntites.EmergencyEntities;
using Hangfire;
using Service.Exception_Implementation.BadRequestExceptions;
using Service.Exception_Implementation.NotFoundExceptions;
using Service.Specification_Implementation.CarOwnerSpecifications;
using Service.Specification_Implementation.RequestSpecifications;
using Service.Specification_Implementation.TechnicianSpecifications;
using ServiceAbstraction.CoreServicesAbstractions;
using SharedData.DTOs.TechnicianEmergencyRequestDTOs;
using SharedData.Enums;
using SharedData.QueryModel;

namespace Service.CoreServices.EmergencyReqServices
{
    public class TechnicianRequestEmergency : ITechnicianRequestEmergency
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        // private readonly IHubContext<ChatHub> hubContext;
        private IChatSessionService chatSessionService;


        public TechnicianRequestEmergency(IUnitOfWork _unitOfWork, IMapper _mapper, IChatSessionService _chatSessionService)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            chatSessionService = _chatSessionService;
        }
        public async Task<bool> ApplyRequestFromHomePage(TechnicianApplyEmergencyRequestDTO emergencyRequestDTO)
        {
            var techSpec = new TechnicianWithAppUserSpec(emergencyRequestDTO.UserId, emergencyRequestDTO.Pin);
            var technician = await unitOfWork.GetRepository<Technician, int>().GetByIdAsync(techSpec);
            if (technician == null) return false;
            var repo = unitOfWork.GetRepository<EmergencyRequest, int>();
            EmergencyRequest requestToUpdate = await repo.GetByIdAsync(emergencyRequestDTO.RequestId);
            //TechReverseRequest isTechnicanExists = requestToUpdate.TechReverseRequests.Where(r => r.TechnicianId == emergencyRequestDTO.UserId).First();
            var rechRequestRepo = unitOfWork.GetRepository<TechReverseRequest, int>();
            TechReverseRequest isTechnicanExists = await rechRequestRepo.GetByIdAsync(new TechReverseRequestSpec(emergencyRequestDTO.RequestId, emergencyRequestDTO.UserId));

            if (requestToUpdate != null && isTechnicanExists == null && technician.ApplicationUser.Coins >= 50 )
            {
                requestToUpdate.TechReverseRequests.Add(new TechReverseRequest
                {
                    EmergencyRequestId = emergencyRequestDTO.RequestId,
                    TechnicianId = emergencyRequestDTO.UserId,
                    CallState = RequestState.Waiting,
                    TimeStamp = DateTime.UtcNow
                });
                int affectedRows = await unitOfWork.SaveChangesAsync();
                if (affectedRows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public async Task<List<EmergencyRequestDetailsDTO>> GetAllAcceptedRequestsAsync(int tecId)
        {

            var requestTechRepo = unitOfWork.GetRepository<EmergencyRequestTechnicians, int>();
            var joinEntries = await requestTechRepo.GetAllAsync(new EmergencyRequestTechnicanSpecefication(new RequestQueryData() { TechnicainId = tecId, CallState = RequestState.Answered, IsCompleted = false }));
            if (joinEntries == null || !joinEntries.Any())
                throw new TRequestNotFoundException("you don't have accpetence requests");
            return mapper.Map<List<EmergencyRequestDetailsDTO>>(joinEntries);
        }

        public async Task<List<EmergencyRequestDetailsDTO>> GetAllActiveRequestsAsync(int tecId)
        {

            var requests = unitOfWork.GetRepository<EmergencyRequest, int>();
            var spec = new ActiveRequestsForTechnicianSpec(tecId);
            var activeRequests = await requests.GetAllAsync(spec);
            if (activeRequests == null || !activeRequests.Any())
                throw new TRequestNotFoundException("there is no active requests to apply");

            return mapper.Map<List<EmergencyRequestDetailsDTO>>(activeRequests);



        }

        public async Task<List<EmergencyRequestDetailsDTO>> GetAllCompletedRequestsAsync(int technicianId)
        {
            var tech = await unitOfWork.GetRepository<Technician, int>().GetByIdAsync(technicianId);
            if (tech == null) throw new TechnicianBadRequestException("there is no tech with this id");
            var spec = new TechnicianCompletedEmergencyRequestSpec(technicianId, true);
            var requests = unitOfWork.GetRepository<EmergencyRequest, int>();
            var completedRequests = await requests.GetAllAsync(spec);
            if (completedRequests == null || !completedRequests.Any()) throw new CompletedRequestNotFoundException("there is no completed Requests for now");
            var result = mapper.Map<List<EmergencyRequestDetailsDTO>>(completedRequests);
            return result;

        }

        public async Task<List<EmergencyRequestDetailsDTO>> GetAllRequestsAssignedToTechnicianAsync(int technicianId)
        {
            var tech = await unitOfWork.GetRepository<Technician, int>().GetByIdAsync(technicianId);
            if (tech == null) throw new TechnicianBadRequestException("there is no tech with this id");


            var spec = new EmergencyRequestTechniciansAssignedToTechSpec(technicianId, RequestState.Waiting);

            var repo = unitOfWork.GetRepository<EmergencyRequestTechnicians, int>();

            var assignedEntries = await repo.GetAllAsync(spec);

            var result = mapper.Map<List<EmergencyRequestDetailsDTO>>(assignedEntries);

            return result;
        }

        public async Task<EmergencyRequestDetailsDTO> GetRequestDetailsByIdAsync(int requestId, int technicianId)
        {
            var tech = await unitOfWork.GetRepository<Technician, int>().GetByIdAsync(technicianId);
            if (tech == null) throw new TechnicianBadRequestException("there is no tech with this id");

            var req = await unitOfWork.GetRepository<EmergencyRequest, int>().GetByIdAsync(requestId);
            if (req == null) throw new EmergencyRequestbadRequestException("there is no request with this id");

            var spec = new EmergencyRequestTechnicianWithRequestSpec(requestId, technicianId);

            var joinEntry = await unitOfWork
                .GetRepository<EmergencyRequestTechnicians, int>()
                .GetByIdAsync(spec);


            if (joinEntry == null)
            {
                Console.WriteLine("join entry null");
                throw new EmergencyRequestbadRequestException("invalid tech or request");
            }


            return mapper.Map<EmergencyRequestDetailsDTO>(joinEntry);
        }
        public async Task<List<TechReverseRequestDTO>> GetTechAllAppliedRequestsAsync(int techId)
        {
            var tech = await unitOfWork.GetRepository<Technician, int>().GetByIdAsync(techId);
            if (tech == null) throw new TechnicianBadRequestException("there is no tech with this id");
            var repo = unitOfWork.GetRepository<TechReverseRequest, int>();
            var spec = new TechReverseRequestSpec(techId);
            var techRequests = await repo.GetAllAsync(spec);
            if (techRequests == null || !techRequests.Any())
                throw new TRequestNotFoundException("you didn't apply yet");
            else
                return mapper.Map<List<TechReverseRequestDTO>>(techRequests);
        }



        public async Task<bool> UpdateRequestFromCarOwnerAsync(TechnicianUpdateEmergencyRequestDTO dto)
        {
            // 1) Verify technician & PIN
            var technician = await LoadTechnicianWithPinAsync(dto.TechnicianId, dto.Pin);
            if (technician is null)
                throw new TechnicianBadRequestException("Invalid technician credentials: TechnicianId or PIN is incorrect.");

            if (technician.ApplicationUser.Coins < 50)
            {
                throw new NoSufeciantAmountOfMoneyBadRequestException();
            }

                // 2) Load request + links 
                var request = await LoadRequestWithLinksAsync(dto.RequestId);
            if (request is null)
                throw new TRequestNotFoundException($"Emergency request with Id={dto.RequestId} was not found.");

            // 3) Must be assigned to this technician
            var link = GetLinkForTechnician(request, dto.TechnicianId);
            if (link is null)
                throw new TechnicianBadRequestException("This request is not assigned to the specified technician.");


            switch (dto.RequestState)
            {
                case RequestState.Answered:
                    {
                        //  Technician limit (max 2 active accepted)
                        if (await TechnicianHasReachedActiveAcceptedLimitAsync(dto.TechnicianId))
                            throw new TechnicianBadRequestException("Technician already has 2 active accepted requests.");

                        //  Someone else already accepted?
                        if (AnotherTechnicianAlreadyAccepted(request))
                            throw new TechnicianBadRequestException("Another technician has already accepted this request.");

                        //  Accept + assign
                        AcceptOnLink(request, link, dto.TechnicianId);

                        //  Find or create chat session (reopen if closed)
                        var chat = await chatSessionService.GetOrCreateSessionAsync(request.CarOwnerId, dto.TechnicianId);

                        // boardcast using SignalR notification 


                        // Reject others
                        RejectOtherTechnicians(request, dto.TechnicianId);
                        break;
                    }

                case RequestState.Rejected:
                    {
                        RejectOnLink(link);
                        break;
                    }

                default:
                    throw new TechnicianBadRequestException($"Unsupported RequestState '{dto.RequestState}'.");
            }
            var spec = new CarOwnerWithAppUserSpecification(request.CarOwnerId);
            var carowner =await  unitOfWork.GetRepository<CarOwner, int>().GetByIdAsync(spec);
            technician.ApplicationUser.Coins -= 50;
            carowner.ApplicationUser.Coins -= 50;
            await unitOfWork.SaveChangesAsync();
            BackgroundJob.Schedule(() => DisableCancelAsync(dto.RequestId), TimeSpan.FromMinutes(5));

            return true;
        }
        public async Task DisableCancelAsync(int requestId)
        {
            var emergancyRequest = await unitOfWork.GetRepository<EmergencyRequest, int>().GetByIdAsync(requestId);
            emergancyRequest.IsCanCancelByTechnician = false;
            await unitOfWork.SaveChangesAsync();
        }


        #region helper methods to apply clean code for UpdateRequestFromCarOwnerAsync()

        private async Task<Technician?> LoadTechnicianWithPinAsync(int technicianId, int pin)
        {
            var techSpec = new TechnicianWithAppUserSpec(technicianId, pin);
            return await unitOfWork.GetRepository<Technician, int>().GetByIdAsync(techSpec);
        }

        private async Task<EmergencyRequest?> LoadRequestWithLinksAsync(int requestId)
        {
            var spec = new EmergencyRequestWithTechnicianLinkSpec(requestId);
            return await unitOfWork.GetRepository<EmergencyRequest, int>().GetByIdAsync(spec);
        }

        private EmergencyRequestTechnicians? GetLinkForTechnician(EmergencyRequest request, int technicianId)
        {
            return request.EmergencyRequestTechnicians
                          .FirstOrDefault(e => e.TechnicianId == technicianId);
        }

        private async Task<bool> TechnicianHasReachedActiveAcceptedLimitAsync(int technicianId)
        {

            var spec = new TechnicianActiveAnsweredRequestsSpec(technicianId);
            var active = await unitOfWork.GetRepository<EmergencyRequest, int>().GetAllAsync(spec);

            return active.Count(r => !r.IsCompleted) >= 1;
        }

        private bool AnotherTechnicianAlreadyAccepted(EmergencyRequest request)
        {
            return request.EmergencyRequestTechnicians
                          .Any(l => l.CallStatus == RequestState.Answered);
        }

        private void AcceptOnLink(EmergencyRequest request, EmergencyRequestTechnicians link, int technicianId)
        {
            link.CallStatus = RequestState.Answered;


            request.TechnicianId = technicianId;


        }

        private void RejectOnLink(EmergencyRequestTechnicians link)
        {
            link.CallStatus = RequestState.Rejected;
        }

        private void RejectOtherTechnicians(EmergencyRequest request, int acceptedTechnicianId)
        {
            foreach (var l in request.EmergencyRequestTechnicians)
            {
                if (l.TechnicianId != acceptedTechnicianId)
                    l.CallStatus = RequestState.Rejected;
            }
        }
        #endregion






    }
}
