
using AutoMapper;
using Domain.Contracts;
using Domain.Entities.CoreEntites.CarMaintenance_Entities;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Exception_Implementation.ArgumantNullException;
using Service.Specification_Implementation.MaintenanceSpecification;
using Service.Specification_Implementation.MaintananceDTOs;
using ServiceAbstraction.CoreServicesAbstractions.CarMservices;
using SharedData.DTOs.Car;
using SharedData.DTOs.CarMaintananceDTOs;
using SharedData.DTOs.MTypesDtos;
using SharedData.Enums;

namespace Service.CoreServices.CarMservices
{
    public class CarMaintananceService : ICarMaintananceService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICarServices carServices;
        private readonly IMaintenanceTypesService maintenanceTypesService;
        private readonly IEmailService emailService;

        public CarMaintananceService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ICarServices carServices,
            IMaintenanceTypesService maintenanceTypesService,
            IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.carServices = carServices;
            this.maintenanceTypesService = maintenanceTypesService;
            this.emailService = emailService;
        }

        #region Add New Maintainence Record

        public async Task AddMaintananceRecord(CarMaintananceAllDTO carMaintananceAllDTO)
        {
            try
            {
                var idClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(s => s.Type == "Id")?.Value;
                var emailClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(s => s.Type == "Email")?.Value;
                var nameClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(s => s.Type == "Name")?.Value;

                #region Validate of Input
                if (idClaim == null)
                {
                    throw new CarMainTainanceNullException();
                }
                if (!int.TryParse(idClaim, out var carOwnerId))
                {
                    throw new Exception("Invalid CarOwner Id in Claims.");
                }
                if (carOwnerId <= 0)
                {
                    throw new CarMainTainanceNullException();
                }
                #endregion

                #region Helper Objects

                var car = await carServices.GetCarDetailsAsync();
                var maintenanceType = await maintenanceTypesService.GetMTypebyIdAsync(carMaintananceAllDTO.MaintenanceTypeId);
                if (maintenanceType == null)
                {
                    throw new CarMainTainanceNullException();
                }

                #endregion

                var NextMDate = DueDateCalculateAsync(maintenanceType, carMaintananceAllDTO.PerformedAt, car);

                #region Initialize CarMaintainance Data

                var originCarMaintenanceRecord = mapper.Map<CarMaintenanceRecord>(carMaintananceAllDTO);
                originCarMaintenanceRecord.NextMaintenanceDue = NextMDate;
                originCarMaintenanceRecord.CarId = car.Id;

                #endregion

                ScheduleMaintainanceReminder(emailClaim!,
                    maintenanceType.Name,
                    nameClaim!,
                    NextMDate);
                await carServices.SetCarStats(carMaintananceAllDTO.PerformedAt, carMaintananceAllDTO.Cost, car.Id);
                await unitOfWork.GetRepository<CarMaintenanceRecord, int>().AddAsync(originCarMaintenanceRecord);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BadHttpRequestException("Server Error or Invalid input Data");
            }
        }

        private DateTime DueDateCalculateAsync(MaintenanceTypeDetailsDto maintenanceType, DateTime Mdate, CarDetailsDto car)
        {
            if (maintenanceType?.RepeatEveryKM != null)
            {
                var TimeInMonths = (int)(maintenanceType?.RepeatEveryKM / car?.AvgKmPerMonth)!;  // الشهور المطلوبة
                DateTime nextMaintenanceDate = Mdate.AddMonths(TimeInMonths);
                return nextMaintenanceDate;
            }
            else if (maintenanceType?.RepeatEveryDays != null)
            {
                DateTime nextMaintenanceDate = Mdate.AddDays((int)maintenanceType?.RepeatEveryDays!);
                return nextMaintenanceDate;
            }
            else
            {
                throw new ArgumentException("Error in calculating");
            }
        }

        private void ScheduleMaintainanceReminder(string toEmail, string maintananceType, string ownername, DateTime maintananceDate)
        {
            if (string.IsNullOrEmpty(toEmail) ||
                string.IsNullOrEmpty(maintananceType) ||
                string.IsNullOrEmpty(ownername) ||
                maintananceDate == default)
            {
                throw new ArgumentException("Email, maintenance type, owner name, and maintenance date cannot be null");
            }

            DateTime currentDate = DateTime.Today;
            if (currentDate >= maintananceDate)
            {
                return;
            }
            int daysDifference = (maintananceDate - currentDate).Days;

            BackgroundJob.Schedule(() =>
                        emailService.SendEmail(toEmail, maintananceType, ownername, DateOnly.FromDateTime(maintananceDate)),
                        TimeSpan.FromDays(daysDifference));
        }

        #endregion

        #region Get Maintainance Summary

        public async Task<List<MaintenanceSummaryDTO>> GetMaintenanceSummary()
        {
            var idClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(s => s.Type == "Id")?.Value;
            if (idClaim == null)
            {
                throw new CarMainTainanceNullException();
            }
            var Flag = int.TryParse(idClaim, out var roleId);
            if (Flag)
            {
                var CarId = await carServices.GetCarIdByOwnerId(roleId);
                var summaryList = await GetMListAsync(CarId);
                if (!summaryList.Any())
                {
                    throw new ArgumentException(message: "There is no M types");
                }
                return summaryList;
            }
            throw new ArgumentException(message: "Bad Request, error in Role Id");
        }

        private async Task<List<MaintenanceSummaryDTO>> GetMListAsync(int CarId)
        {
            var MRepo = unitOfWork.GetRepository<MaintenanceTypes, int>();
            var spec = new MaintenanceSummarySpecification(CarId);
            var maintenanceDataQ = await MRepo.GetAllQueryable(spec).ToListAsync();

            var maintenanceList = maintenanceDataQ.Select(mt => new
            {
                mt.Name,
                LastRecord = mt.CarMaintenanceRecords.FirstOrDefault()
            });

            //var maintenanceList = await maintenanceDataQ
            //    .Select(mt => new
            //    {
            //        mt.Name,
            //        LastRecord = mt.CarMaintenanceRecords
            //            .Where(r => r.CarId == CarId)
            //            .OrderByDescending(r => r.PerformedAt)
            //            .Select(r => new
            //            {
            //                r.Id,
            //                r.PerformedAt,
            //                r.CarKMsAtTime,
            //                r.NextMaintenanceDue
            //            })
            //            .FirstOrDefault()
            //    })
            //    .ToListAsync();


            var SummaryDtoList = new List<MaintenanceSummaryDTO>();

            foreach (var item in maintenanceList)
            {
                if (item.LastRecord != null)
                {
                    SummaryDtoList.Add(new MaintenanceSummaryDTO()
                    {
                        MaintenanceTypeName = item.Name,
                        LastMaintenanceDate = item.LastRecord.PerformedAt,
                        NextExpectedMaintenance = item.LastRecord.NextMaintenanceDue,
                        Status = item.LastRecord.NextMaintenanceDue <= DateTime.UtcNow ? MaintenanceEnum.Needed
                                                                        : MaintenanceEnum.NoNeeded
                    });
                }
                else
                {
                    SummaryDtoList.Add(new MaintenanceSummaryDTO()
                    {
                        MaintenanceTypeName = item.Name,
                        LastMaintenanceDate = null,
                        NextExpectedMaintenance = null,
                        Status = MaintenanceEnum.NoInfo
                    });
                }
            }
            return SummaryDtoList;
        }

        #endregion

        #region M History by Id

        public async Task<List<MaintananceHistory>> GetAllMaintananceHistoryByID(int maintananceId)
        {
            var idClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(s => s.Type == "Id")?.Value;
            if (idClaim == null || !int.TryParse(idClaim, out var carOwnerId) || carOwnerId <= 0)
            {
                throw new CarMainTainanceNullException();
            }
            int CarId = await carServices.GetCarIdByOwnerId(carOwnerId);

            if (maintananceId <= 0)
            {
                throw new MaintananceArgumentException();
            }
            var spec = new GetMaintananceByID(CarId, maintananceId);
            var carMaintenanceRecords = await unitOfWork.GetRepository<CarMaintenanceRecord, int>().GetAllWithSpecAsync(spec);
            if (carMaintenanceRecords == null || !carMaintenanceRecords.Any())
            {
                throw new MaintananceArgumentException("There is no maintanance matches");
            }
            var maintananceHistory = mapper.Map<List<MaintananceHistory>>(carMaintenanceRecords);
            return maintananceHistory;
        }


        #endregion
    }
}



