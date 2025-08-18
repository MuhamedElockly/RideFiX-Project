using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities.CoreEntites.EmergencyEntities;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Service.CoreServices.ChatServices;
using Service.Exception_Implementation;
using Service.Exception_Implementation.AlreadyFound;
using Service.Exception_Implementation.ArgumantNullException;
using Service.Exception_Implementation.BadRequestExceptions;
using Service.Exception_Implementation.NotFoundExceptions;
using Service.Specification_Implementation;
using Service.Specification_Implementation.CarOwnerSpecifications;
using Service.Specification_Implementation.RequestSpecifications;
using ServiceAbstraction;
using ServiceAbstraction.CoreServicesAbstractions;
using SharedData.DTOs.ChatSessionDTOs;
using SharedData.DTOs.RequestsDTOs;
using SharedData.DTOs.TechnicianDTOs;
using SharedData.Enums;

namespace Service.CoreServices.EmergencyReqServices
{
    public class RequstServices : IRequestServices
    {
        //private readonly IServiceManager serviceManager;

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ITechnicianService techService;
        private readonly IChatSessionService chatSessionService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICarOwnerService carOwnerService;
        public RequstServices(IUnitOfWork _unitOfWork,
            IMapper _mapper,
            ITechnicianService technicianService,
            IChatSessionService chatSessionService,
            IHttpContextAccessor httpContextAccessor,
            ICarOwnerService carOwnerService)
        {

            unitOfWork = _unitOfWork;
            mapper = _mapper;
            techService = technicianService;
            this.chatSessionService = chatSessionService;
            this.httpContextAccessor = httpContextAccessor;
            this.carOwnerService = carOwnerService;
            //this.serviceManager = serviceManager;
        }

        #region Cancel methods

        #region Cancel for Car Owner
        public async Task CancelAll(int CarOwnerID)
        {
            if (CarOwnerID <= 0)
            {
                throw new ArgumentNullException(nameof(CarOwnerID), "Car Owner ID cannot be null or negative.");
            }
            var requestBrief = await carOwnerService.IsRequested(CarOwnerID);
            int requestId = requestBrief.Id;
            var Request = await unitOfWork.GetRepository<EmergencyRequest, int>().GetByIdAsync(requestId);
            if (Request == null)
            {
                throw new RequestNotFoundException();
            }
            if (Request.IsCompleted)
            {
                throw new RequestAlreadyCompletedException();
            }

            //Request.IsCompleted = true;

            var spec = new CancelledRquestSpecification(CarOwnerID, requestId);
            var emergencyRequests = await unitOfWork.EmergencyRequestRepository.GetAllAsync(spec);
            if (emergencyRequests == null || !emergencyRequests.Any())
            {
                throw new RequestNotFoundException();
            }
            var spec2 = new CancelledReverseRquestSpecification(requestId);
            var emergencyRequestsTechnicians = await unitOfWork.GetRepository<TechReverseRequest, int>().GetAllAsync(spec2);
            if (emergencyRequestsTechnicians != null && emergencyRequestsTechnicians.Any())
            {
                foreach (var emergencyRequestTechnician in emergencyRequestsTechnicians)
                {
                    if (emergencyRequestTechnician.CallState == RequestState.Waiting || emergencyRequestTechnician.CallState == RequestState.Answered)
                    {
                        emergencyRequestTechnician.CallState = RequestState.Cancelled;
                    }
                    else
                    {
                        throw new RequestAlreadyAccepted();
                    }
                }
            }

            foreach (var emergencyRequest in emergencyRequests)
            {
                if (emergencyRequest.CallStatus == RequestState.Waiting)
                {
                    emergencyRequest.CallStatus = RequestState.Cancelled;
                    await unitOfWork.EmergencyRequestRepository.UpdateAsync(emergencyRequest);
                }
                if (emergencyRequest.CallStatus == RequestState.Answered && emergencyRequest.EmergencyRequests.IsCompleted == false)
                {
                    emergencyRequest.CallStatus = RequestState.Cancelled;
                    emergencyRequest.EmergencyRequests.EndTimeStamp = DateTime.UtcNow;
                    await unitOfWork.EmergencyRequestRepository.UpdateAsync(emergencyRequest);
                }

            }

            var chatsession = await chatSessionService.GetChatSessionsByCarOwnerId(CarOwnerID);
            if (chatsession != null)
            {
                //throw new ChatSessionNotFoundException();
                chatsession.IsClosed = true;
                await chatSessionService.update(chatsession);
            }
            await unitOfWork.SaveChangesAsync();

        }
        #endregion

        #region Cancel for Technician
        public async Task CancelForTechnician(int requestId)
        {
            var reqRepo = unitOfWork.GetRepository<EmergencyRequest, int>();
            var emergencyRequest = await reqRepo.GetByIdAsync(requestId);
            var user = httpContextAccessor.HttpContext;
            if (user == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            int entityId;
            bool isValid = int.TryParse(user.User.Claims.FirstOrDefault(s => s.Type == "Id")?.Value, out entityId);
            if (emergencyRequest == null)
            {
                throw new RequestNotFoundException();
            }
            if (emergencyRequest.IsCanCancelByTechnician)
            {
                var techRepo = unitOfWork.EmergencyRequestRepository;
                var spec = new techsForCancelSpecification(requestId, entityId);
                var emergencyRequestTechnicians = await techRepo.GetAllAsync(spec);
                foreach (var item in emergencyRequestTechnicians)
                {
                    item.CallStatus = RequestState.Cancelled;
                }
                await unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("This request cannot be cancelled by the technician.");
            }
        }
        #endregion

        #endregion

        public async Task CompleteRequest(int requestId)
        {
            var emergencyRequest = await unitOfWork.GetRepository<EmergencyRequest, int>().GetByIdAsync(requestId);
            if (emergencyRequest == null)
            {
                throw new RequestNotFoundException();
            }
            if (emergencyRequest.IsCompleted)
            {
                throw new RequestAlreadyCompletedException();
            }

            emergencyRequest.IsCompleted = true;
            emergencyRequest.EndTimeStamp = DateTime.UtcNow;
            emergencyRequest.CompeletRequestDate = DateOnly.FromDateTime(DateTime.UtcNow);

            unitOfWork.GetRepository<EmergencyRequest, int>().Update(emergencyRequest);
            if (emergencyRequest.TechnicianId.HasValue)
            {
                await chatSessionService.CompleteChatSession(emergencyRequest.TechnicianId.Value, emergencyRequest.CarOwnerId);
                await unitOfWork.SaveChangesAsync();
            }

        }

        public async Task CreateRealRequest(RealRequestDTO request)
        {
            bool isPresent = await IsPresent(request);
            if (!isPresent)
            {
                var carOwnerRepo = unitOfWork.GetRepository<CarOwner, int>();
                var specification = new CarOwnerUserPinSpecification(request);
                var owner = await carOwnerRepo.GetByIdAsync(specification);
                if (owner == null)
                {
                    throw new CarOwnerNotFoundException();
                }
                if (request.pin == owner.ApplicationUser.PIN)
                {
                    var emergancyRequest = new EmergencyRequest
                    {
                        CarOwnerId = request.CarOwnerId,
                        Description = request.Description,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude,
                        IsCompleted = false,
                        TimeStamp = DateTime.UtcNow,
                        EndTimeStamp = null,
                        categoryId = request.categoryId
                    };
                    await unitOfWork.GetRepository<EmergencyRequest, int>().AddAsync(emergancyRequest);
                    await unitOfWork.SaveChangesAsync();


                    if (request.TechnicianIDs != null && request.TechnicianIDs.Any())
                    {
                        foreach (var technicianId in request.TechnicianIDs)
                        {
                            var emergencyRequestTechnicians = new EmergencyRequestTechnicians
                            {
                                EmergencyRequestId = emergancyRequest.Id,
                                TechnicianId = technicianId,
                                CallStatus = RequestState.Waiting
                            };
                            await unitOfWork.EmergencyRequestRepository.AddAsync(emergencyRequestTechnicians);
                        }
                    }

                    await unitOfWork.SaveChangesAsync();
                    BackgroundJob.Schedule(() => AutoCancelAsync(emergancyRequest.Id), TimeSpan.FromHours(4));

                }
                else
                {
                    throw new PinCodeBadRequestException();
                }
            }
            else
            {
                throw new RequestAlreadyFoundException();
            }

        }
        public async Task AutoCancelAsync(int reqId)
        {
            var repo = unitOfWork.EmergencyRequestRepository;
            var spec = new AutoCancelSpecification(reqId);
            var techs = await repo.GetAllAsync(spec);
            foreach (var item in techs)
            {
                item.CallStatus = RequestState.Cancelled;
            }
            await unitOfWork.SaveChangesAsync();
        }


        public async Task<PreRequestDTO> CreateRequestAsync(CreatePreRequestDTO request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }

            var spec = new CarOwnerSpecification(request);
            var user = await unitOfWork.GetRepository<CarOwner, int>().GetByIdAsync(spec);

            if (user == null)
            {
                throw new CarOwnerNotFoundException();
            }
            if( user.ApplicationUser.Coins < 50 )
            {
                throw new NoSufeciantAmountOfMoneyBadRequestException();
            }
            var filteredTechnicians = await techService.GetTechniciansByFilterAsync(request);
            if (filteredTechnicians == null || !filteredTechnicians.Any())
            {
                return new PreRequestDTO { };
            }

            return new PreRequestDTO { Technicians = filteredTechnicians };

        }

        public async Task<EmergencyTechnicianID> EmergencyTechnicianID(int requestId)
        {
            if (requestId <= 0)
            {
                throw new EmergencyNullException();
            }
            var emergencyTechnician = await unitOfWork.GetRepository<EmergencyRequest, int>().GetByIdAsync(requestId);
            if (emergencyTechnician == null)
            {
                throw new RequestNotFoundException();
            }
            if (emergencyTechnician.Technician == null)
            {
                throw new TechnicianNotFoundException();
            }
            var mappedTechnician = mapper.Map<EmergencyTechnicianID>(emergencyTechnician);
            return mappedTechnician;
        }

        public async Task<int> GetCurrentRequestId()
        {

            var user = httpContextAccessor.HttpContext;
            if (user == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            int entityId;
            bool isValid = int.TryParse(user.User.Claims.FirstOrDefault(s => s.Type == "Id")?.Value, out entityId);
            var spec = new GetCurrentRequestSpecification(entityId);
            var emergencyRequest = await unitOfWork.GetRepository<EmergencyRequest, int>().GetByIdAsync(spec);
            if (emergencyRequest == null)
            {
                throw new RequestNotFoundException();
            }
            return emergencyRequest.Id;

        }

        public async Task<bool> IsPresent(RealRequestDTO request)
        {
            var spec = new WaitingRequestSpecification(request.CarOwnerId);
            var emergencyRequest = await unitOfWork.GetRepository<EmergencyRequest, int>().GetAllAsync(spec);
            if (emergencyRequest == null || !emergencyRequest.Any())
            {
                return false;
            }
            return true;


        }

        public async Task<List<RequestBreifDTO>> RequestBreifDTOs(int carOwnerID)
        {
            var spec = new RequestBreifSpecification(carOwnerID);
            var emergencyRequests = await unitOfWork.GetRepository<EmergencyRequest, int>().GetAllAsync(spec);
            if (emergencyRequests == null || !emergencyRequests.Any())
            {
                throw new RequestNotFoundException();
            }
            var mappedRequests = mapper.Map<List<RequestBreifDTO>>(emergencyRequests);
            return mappedRequests;
        }

        public async Task<RequestDetailsDTO> RequestDetailsDTOs(int requestId)
        {
            var spec = new RequestDetailsSpecification(requestId);
            var emergencyRequest = await unitOfWork.GetRepository<EmergencyRequest, int>().GetByIdAsync(spec);
            if (emergencyRequest == null)
            {
                throw new RequestNotFoundException();
            }
            string city = await techService.GetCity(emergencyRequest.Latitude, emergencyRequest.Longitude);
            if (emergencyRequest.Technician == null)
            {
                throw new RequestDetailsException();
            }
            var mappedRequest = new RequestDetailsDTO()
            {
                City = city,
                Description = emergencyRequest.Description,
                TechnicianName = emergencyRequest.Technician.ApplicationUser.Name,
                CategoryName = emergencyRequest.category.Name,
                RequestDate = emergencyRequest.CompeletRequestDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                Rate = emergencyRequest.Review?.Rate,
                Comment = emergencyRequest.Review?.Comment,
            };
            return mappedRequest;
        }
    }
}
