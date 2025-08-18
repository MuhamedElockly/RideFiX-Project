using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Service.CoreServices.CarMservices;

using ServiceAbstraction;
using ServiceAbstraction.CoreServicesAbstractions;
using ServiceAbstraction.CoreServicesAbstractions.Account;
using ServiceAbstraction.CoreServicesAbstractions.Admin;
using ServiceAbstraction.CoreServicesAbstractions.CarMservices;
using ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction;
using ServiceAbstraction.CoreServicesAbstractions.Reports;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        #region services abstraction
        public IRequestServices requestServices { get; }
        public ITechnicianService technicianService { get; }

        public ITechnicianRequestEmergency technicianRequestEmergency { get; }
        public ICategoryService categoryService { get; }
        public IReviewService reviewService { get; }
        public ICarOwnerService carOwnerService { get; }

        public IUserProfileService userProfileService { get; }

        public IChatService ChatService { get; }

        public IMessegeService messegeService { get; }
        public IUserConnectionIdService userConnectionIdService { get; }
        public IChatSessionService chatSessionService { get; }

        public ICarServices carServices { get; }

        public ICarMaintananceService carMaintananceService { get; }
        public IEmailService emailService { get; }

        public IMaintenanceTypesService maintenanceTypesService { get; }
        public IReverserRequestService reverserRequestService { get; }

        public IProductCategoryService productCategoryService { get; }
        public IProductsService productsService { get; }
        public IShoppingCartService shoppingCartService { get; } 

        public IAdminService adminService { get; }
        public IActivityReportService activityReportService { get; }


        public IReportsServices reportsServices { get; }
        #endregion

        public ServiceManager(
                    IRequestServices requestServices,
                    ITechnicianService technicianService,
                    ITechnicianRequestEmergency _tech,
                    ICategoryService categoryService,
                    IReviewService reviewService,
                    ICarOwnerService carOwnerService,
                    IUserProfileService _userProfile,
                    IChatService chatservice,
                    IMessegeService messegeService,
                    IUserConnectionIdService userConnectionIdService,
                    IChatSessionService chatSessionService,
                    ICarServices carServices,
                    ICarMaintananceService carMaintananceService,
                    IMaintenanceTypesService maintenanceTypesService,
                    IEmailService emailService,

                    IProductCategoryService productCategoryService,
                    IProductsService productsService,
                    IShoppingCartService shoppingCartService,

                    IReverserRequestService reverserRequestService,
                    IReportsServices reportsServices,


          //          IReportsServices reportsServices,

                    IAdminService adminService,
                    IActivityReportService activityReportService)

        {
            this.requestServices = requestServices;
            this.technicianService = technicianService;
            this.technicianRequestEmergency = _tech;
            this.categoryService = categoryService;
            this.carOwnerService = carOwnerService;
            this.reviewService = reviewService;
            this.userProfileService = _userProfile;
            this.ChatService = chatservice;
            this.messegeService = messegeService;
            this.userConnectionIdService = userConnectionIdService;
            this.chatSessionService = chatSessionService;
            this.carServices = carServices;
            this.carMaintananceService = carMaintananceService;
            this.maintenanceTypesService = maintenanceTypesService;
            this.emailService = emailService;
            this.maintenanceTypesService = maintenanceTypesService;
            this.reverserRequestService = reverserRequestService;
            this.productCategoryService = productCategoryService;
            this.productsService = productsService;
            this.shoppingCartService = shoppingCartService;
            this.reportsServices = reportsServices;
            this.adminService = adminService;
            this.activityReportService = activityReportService;
        }
    }
}
