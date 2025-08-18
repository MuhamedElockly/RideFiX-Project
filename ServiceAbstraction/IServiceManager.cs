using Service.CoreServices.CarMservices;
using ServiceAbstraction.CoreServicesAbstractions;
using ServiceAbstraction.CoreServicesAbstractions.Account;
using ServiceAbstraction.CoreServicesAbstractions.Admin;
using ServiceAbstraction.CoreServicesAbstractions.CarMservices;
using ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction;
using ServiceAbstraction.CoreServicesAbstractions.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IServiceManager
    {
        IChatService ChatService { get; }
        ITechnicianService technicianService { get; }
        IRequestServices requestServices { get; }
        ITechnicianRequestEmergency technicianRequestEmergency { get; }
        ICategoryService categoryService { get; }
        IReviewService reviewService { get; }
        ICarOwnerService carOwnerService { get; }
        IUserProfileService userProfileService { get; }
        IUserConnectionIdService userConnectionIdService { get; }
        IChatSessionService chatSessionService { get; }
        IMessegeService messegeService { get; }
        ICarServices carServices { get; }
        ICarMaintananceService carMaintananceService { get; }
        IEmailService emailService { get; }
        IMaintenanceTypesService maintenanceTypesService { get; }

        IReverserRequestService reverserRequestService { get; }

        IProductCategoryService productCategoryService { get; }
        IProductsService productsService { get; }
        IShoppingCartService shoppingCartService { get; }
        IOrderService orderService { get; }

        IReportsServices reportsServices { get; }

        IAdminService adminService { get; }
        IActivityReportService activityReportService { get; }
        IRateService rateService { get; }


        public IPaymentService paymentService { get; }
    }
}
