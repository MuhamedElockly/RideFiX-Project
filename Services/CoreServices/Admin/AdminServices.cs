using AutoMapper;
using Domain.Contracts;
using Domain.Contracts.SpecificationContracts;
using Domain.Entities;
using Domain.Entities.CoreEntites.EmergencyEntities;
using Domain.Entities.Reporting;
using Service.Exception_Implementation.BadRequestExceptions;
using Service.Exception_Implementation.NotFoundExceptions;
using Service.Specification_Implementation.CarOwnerSpecifications;
using Service.Specification_Implementation.ChatSessionsSpecifications;
using Service.Specification_Implementation.RequestSpecifications;
using Service.Specification_Implementation.ReviewSpecification;
using Service.Specification_Implementation.TechnicianSpecifications;
using ServiceAbstraction.CoreServicesAbstractions.Admin;
using Services.Specification_Implementation.Emergency;
using SharedData.DTOs.Admin.TechnicianCategory;
using SharedData.DTOs.Admin.Users;
using SharedData.DTOs.MessegeDTOs;
using SharedData.DTOs.ReportDtos;
using SharedData.DTOs.TechnicianDTOs;
using SharedData.Enums;


namespace Service.CoreServices.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AdminService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }





        #region Private Helpers

        private async Task<List<ReadUsersDTO>> GetUsersAsync<TEntity, TKey>(ISpecification<TEntity, TKey> spec, string notFoundMessage)
            where TEntity : BaseEntity<TKey>
        {
            var repo = unitOfWork.GetRepository<TEntity, TKey>();
            var users = await repo.GetAllAsync(spec);

            if (users == null || !users.Any())
                throw new UsersNotFoundException(notFoundMessage);

            return mapper.Map<List<ReadUsersDTO>>(users);
        }

        private async Task SetUserActivationAsync<TEntity>(ISpecification<TEntity, int> spec, int userId,

            string notFoundMessage,
            Action<TEntity> setActivation)
            where TEntity : BaseEntity<int>
        {
            if (userId <= 0) throw new UsersNotFoundException(notFoundMessage);

            var repo = unitOfWork.GetRepository<TEntity, int>();
            var user = await repo.GetByIdAsync(spec);
            if (user == null) throw new UsersNotFoundException(notFoundMessage);

            setActivation(user);
            repo.Update(user);
            await unitOfWork.SaveChangesAsync();

        }




        private async Task<TCategory> GetCategoryByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<TCategory, int>();
            var category = await repo.GetByIdAsync(id);
            if (category == null)
                throw new CategoriesNotFoundException("there is no category with this id");

            return category;
        }

        #endregion

        #region Users
        public async Task<List<ReadTechnicianReviewDTO>> GetTechnicianReviewAsync(int technicianId)
        {
            var spec = new ReviewSpecification(technicianId);
            var repo = unitOfWork.GetRepository<Domain.Entities.CoreEntites.EmergencyEntities.Review, int>();
            var result = await repo.GetAllAsync(spec);

            if (result == null || !result.Any())
                throw new ReviewsNotFoundException("there is no reviews for this technician");
            return mapper.Map<List<ReadTechnicianReviewDTO>>(result);

        }
        public async Task<object> GetUsersCountAsync()
        {
            var techniciansCount = await unitOfWork
                .GetRepository<Technician, int>()
                .CountAsync(new TechnicianWithUserAndReviewsSpec());

            var carOwnersCount = await unitOfWork
                .GetRepository<CarOwner, int>()
                .CountAsync(new CarOwnerWithUserAndReviewsSpec());

            return new
            {
                TechniciansCount = techniciansCount,
                CarOwnersCount = carOwnersCount
            };
        }

        public Task<List<ReadUsersDTO>> GetAllCarOwnersAsync()
            => GetUsersAsync(new CarOwnerWithUserAndReviewsSpec(), "there is no car owners");

        public Task<List<ReadUsersDTO>> GetAllTechniciansAsync()
            => GetUsersAsync(new TechnicianWithUserAndReviewsSpec(), "there is no technicians");

        public Task BanTechnianAsync(int userId)
            => SetUserActivationAsync<Technician>(new TechnicianWithUserAndReviewsSpec(userId), userId, "there is no technician with this id", t => t.ApplicationUser.IsActivated = false);

        public Task BanCarOwnerAsync(int userId)
            => SetUserActivationAsync<CarOwner>(new CarOwnerWithUserAndReviewsSpec(userId), userId, "there is no car owner with this id", c => c.ApplicationUser.IsActivated = false);

        public Task ActivateTechnianAsync(int userId)
            => SetUserActivationAsync<Technician>(new TechnicianWithUserAndReviewsSpec(userId), userId, "there is no technician with this id", t => t.ApplicationUser.IsActivated = true);

        public Task ActivateCarOwonerAsync(int userId)
            => SetUserActivationAsync<CarOwner>(new CarOwnerWithUserAndReviewsSpec(userId), userId, "there is no car owner with this id", c => c.ApplicationUser.IsActivated = true);

        #endregion

        #region Categories

        public async Task<List<ReadTCategoryDTO>> GetAllCategoriesAsync()
        {
            var spec = new CategoriesByNameSpec();
            var repo = unitOfWork.GetRepository<TCategory, int>();
            var categories = await repo.GetAllAsync(spec);

            if (categories == null || !categories.Any())
                throw new CategoriesNotFoundException("there is no categories");

            return mapper.Map<List<ReadTCategoryDTO>>(categories);
        }

        public async Task CreateCategoryAsync(CreateTCategoryDTO dto)
        {
            var entity = mapper.Map<TCategory>(dto);
            var repo = unitOfWork.GetRepository<TCategory, int>();
            await repo.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();


        }

        public async Task UpdateCategoryAsync(int id, UpdateTCategoryDTO dto)
        {
            var category = await GetCategoryByIdAsync(id);
            category.Name = dto.Name;

            unitOfWork.GetRepository<TCategory, int>().Update(category);
            await unitOfWork.SaveChangesAsync();

        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await GetCategoryByIdAsync(id);
            category.IsDeleted = true;

            unitOfWork.GetRepository<TCategory, int>().Update(category);
            await unitOfWork.SaveChangesAsync();

        }





        #endregion

        #region requests
        public async Task<object> GetrequestsCountAsync()
        {
            var requestRepo = unitOfWork.GetRepository<EmergencyRequest, int>();

            var allRequestsCount = await requestRepo.CountAsync(new EmergencyRequestTotalSpecification());

            var waitingRequestsCount = await requestRepo.CountAsync(new EmergencyRequestTotalSpecification(RequestState.Waiting));

            return new
            {
                AllRequestsCount = allRequestsCount,
                WaitingRequestsCount = waitingRequestsCount
            };
        }
        #endregion

        #region Dashbord Statistics
        public async Task<object> GetDashboardStatisticsAsync()
        {
            var techRepo = unitOfWork.GetRepository<Technician, int>();
            var carOwnerRepo = unitOfWork.GetRepository<CarOwner, int>();

            var techniciansCount = await techRepo.CountAsync(new TechniciansSpecification());
            var carOwnersCount = await carOwnerRepo.CountAsync(new CarOwnerSpecification());
            var totalUsers = techniciansCount + carOwnersCount;

            // --- Dates ---
            var firstDayThisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);
            var firstDayNextMonth = firstDayThisMonth.AddMonths(1);

            // --- New users this month ---
            int newUsersThisMonth = await GetNewUsersForThisMonth(techRepo, carOwnerRepo, firstDayThisMonth);

            // --- New users last month (between firstDayLastMonth and firstDayThisMonth) ---
            int newUsersLastMonth = await GetNewUsersForLastMonth(techRepo, carOwnerRepo, firstDayThisMonth, firstDayLastMonth);
            // % growth difference
            double diffNewUsersPercent = CountNewUsersFromPastMonthInPrecent(newUsersThisMonth, newUsersLastMonth);


            // --- Avg Tech Rate ---
            var Technicians = await techRepo.GetAllWithSpecAsync(new TechnicianWithUserAndReviewsSpec());
            var avgTechRate = Technicians.Any(t => t.reviews.Any())
                ? Math.Round(Technicians.Where(t => t.reviews.Any()).Average(t => t.reviews.Average(r => r.Rate)), 2)
                : 0;

            double GetUserPercent(int count) =>
                totalUsers == 0 ? 0 : Math.Round((count / (double)totalUsers) * 100, 2);

            // --- Requests ---
            var requestRepo = unitOfWork.GetRepository<EmergencyRequest, int>();
            var totalRequests = await requestRepo.CountAsync(new EmergencyRequestTotalSpecification());
            var completedCount = await requestRepo.CountAsync(new EmergencyRequestTotalSpecification(RequestState.Completed));
            var waitingCount = await requestRepo.CountAsync(new EmergencyRequestTotalSpecification(RequestState.Waiting));
            var activeCount = await requestRepo.CountAsync(new EmergencyRequestTotalSpecification(RequestState.Answered));
            var canceledCount = await requestRepo.CountAsync(new EmergencyRequestTotalSpecification(RequestState.Cancelled));

            double GetRequestPercent(int count) =>
                totalRequests == 0 ? 0 : Math.Round((count / (double)totalRequests) * 100, 2);

            return new
            {
                Users = new
                {
                    Technicians = new { Count = techniciansCount, Percent = GetUserPercent(techniciansCount) },
                    CarOwners = new { Count = carOwnersCount, Percent = GetUserPercent(carOwnersCount) },
                    Growth = new
                    {
                        ThisMonth = newUsersThisMonth,
                        LastMonth = newUsersLastMonth,
                        Difference = diffNewUsersPercent
                    },
                    Rates = avgTechRate
                },
                Requests = new
                {
                    Completed = new { Count = completedCount, Percent = GetRequestPercent(completedCount) },
                    Waiting = new { Count = waitingCount, Percent = GetRequestPercent(waitingCount) },
                    Active = new { Count = activeCount, Percent = GetRequestPercent(activeCount) },
                    Canceled = new { Count = canceledCount, Percent = GetRequestPercent(canceledCount) }
                }
            };
        }

        private static async Task<int> GetNewUsersForLastMonth(IGenericRepository<Technician, int> techRepo, IGenericRepository<CarOwner, int> carOwnerRepo, DateTime firstDayThisMonth, DateTime firstDayLastMonth)
        {
            var newTechCountLastMonth = await techRepo.CountAsync(new TechnicianWithAppUserSpec(firstDayLastMonth, firstDayThisMonth));
            var newCarOwnerCountLastMonth = await carOwnerRepo.CountAsync(new CarOwnerSpecification(firstDayLastMonth, firstDayThisMonth));
            var newUsersLastMonth = newTechCountLastMonth + newCarOwnerCountLastMonth;
            return newUsersLastMonth;
        }

        private static async Task<int> GetNewUsersForThisMonth(IGenericRepository<Technician, int> techRepo, IGenericRepository<CarOwner, int> carOwnerRepo, DateTime firstDayThisMonth)
        {
            var newTechCountThisMonth = await techRepo.CountAsync(new TechnicianWithAppUserSpec(firstDayThisMonth));
            var newCarOwnerCountThisMonth = await carOwnerRepo.CountAsync(new CarOwnerSpecification(firstDayThisMonth));
            var newUsersThisMonth = newTechCountThisMonth + newCarOwnerCountThisMonth;
            return newUsersThisMonth;
        }

        private static double CountNewUsersFromPastMonthInPrecent(int newUsersThisMonth, int newUsersLastMonth)
        {
            double diffNewUsersPercent;
            if (newUsersLastMonth == 0)
            {
                diffNewUsersPercent = newUsersThisMonth > 0 ? 100 : 0; // avoid divide by zero
            }
            else
            {
                diffNewUsersPercent = Math.Round(((double)(newUsersThisMonth - newUsersLastMonth) / newUsersLastMonth) * 100, 2);
            }

            return diffNewUsersPercent;
        }
        #endregion

        #region reports
        public async Task<object> GetReportsAsync()
        {
            var reportRepo = unitOfWork.GetRepository<Report, int>();
            var techRepo = unitOfWork.GetRepository<Technician, int>();
            var carOwnerRepo = unitOfWork.GetRepository<CarOwner, int>();
            var chatRepo = unitOfWork.GetRepository<ChatSession, int>();

            var reports = await reportRepo.GetAllAsync();
            if (reports == null || !reports.Any())
                throw new ReportNotFoundException("There are no reports in DB");

            var technicians = await techRepo.GetAllAsync(new TechniciansSpecification());
            var carOwners = await carOwnerRepo.GetAllAsync(new CarOwnerSpecification());
            var dtos = new List<ReadReportDTO>();



            foreach (var report in reports)
            {

                // Find ReportingUser
                var reportingTech = technicians.FirstOrDefault(t => t.ApplicationUserId == report.ReportingUserId);
                var reportingCarOwner = carOwners.FirstOrDefault(c => c.ApplicationUserId == report.ReportingUserId);

                // Find ReportedUser
                var reportedTech = technicians.FirstOrDefault(t => t.ApplicationUserId == report.ReportedUserId);
                var reportedCarOwner = carOwners.FirstOrDefault(c => c.ApplicationUserId == report.ReportedUserId);

                // IDs for chat lookup
                var techId = reportingTech?.Id ?? reportedTech?.Id ?? 0;
                var carOwnerId = reportingCarOwner?.Id ?? reportedCarOwner?.Id ?? 0;

                // Get chats between this pair
                var chats = await chatRepo.GetAllAsync(new ChatSessionBetweenParticipantsSpec(carOwnerId, techId));
                var chatList = chats.ToList();




                dtos.Add(new ReadReportDTO
                {
                    Description = report.Description,
                    CreatedAt = report.CreatedAt,
                    ReportState = report.reportState,
                    ReportingUserId = report.ReportingUserId,
                    ReportingUserRole = reportingTech != null ? "Technician" : "CarOwner",
                    ReportingEntityId = reportingTech?.Id ?? reportingCarOwner?.Id ?? 0,
                    ReportedUserId = report.ReportedUserId,
                    ReportedUserRole = reportedTech != null ? "Technician" : "CarOwner",
                    ReportedEntityId = reportedTech?.Id ?? reportedCarOwner?.Id ?? 0,
                    ReportId = report.Id,
                    TechnicianName = (reportingTech ?? reportedTech)?.ApplicationUser.Name,
                    CarOwnerName = (reportingCarOwner ?? reportedCarOwner)?.ApplicationUser.Name,



                    Messages = chatList.SelectMany(c => c.massages).Select(m => new ReadMessageDTO
                    {
                        MessageId = m.Id,
                        Text = m.Text,
                        SentAt = m.SentAt,
                        IsSeen = m.IsSeen,
                        SenderId = m.ApplicationId,
                        SenderName = m.ApplicationUser?.Name

                    }).ToList()
                });
            }



            return new { Reports = dtos };
        }

        public async Task UpdateReportStateAsync(UpdateReportDTO reportDTO)
        {
            var report = await unitOfWork.GetRepository<Report, int>().GetByIdAsync(reportDTO.ReportId);
            if (report == null) throw new ReportNotFoundException("there is no report with this id");
            switch (reportDTO.ReportState)
            {
                case ReportState.Approved:
                    {
                        report.reportState = ReportState.Approved;

                    }


                    break;
                case ReportState.Rejected:
                    {
                        report.reportState = ReportState.Rejected;

                    }
                    break;
                default:

                    throw new TechnicianBadRequestException($"Unsupported RequestState '{reportDTO.ReportState}'.");



            }
            await unitOfWork.SaveChangesAsync();
        }



        #endregion


    }
}
