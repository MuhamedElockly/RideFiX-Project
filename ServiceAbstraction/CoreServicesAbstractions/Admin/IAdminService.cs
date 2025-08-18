using SharedData.DTOs.Admin.TechnicianCategory;
using SharedData.DTOs.Admin.Users;
using SharedData.DTOs.ReportDtos;
using SharedData.Enums;

namespace ServiceAbstraction.CoreServicesAbstractions.Admin
{
    public interface IAdminService
    {
        // Users
        
        Task<List<ReadUsersDTO>> GetAllTechniciansAsync();
        Task<List<ReadUsersDTO>> GetAllCarOwnersAsync();
        Task BanTechnianAsync(int userId);
        Task BanCarOwnerAsync(int userId);
        Task ActivateCarOwonerAsync(int userId);
        Task ActivateTechnianAsync(int userId);
        Task<Object> GetUsersCountAsync();

        // requests
        Task<Object> GetrequestsCountAsync();


        // Categories
        Task<List<ReadTCategoryDTO>> GetAllCategoriesAsync();
        Task CreateCategoryAsync(CreateTCategoryDTO dto);
        Task UpdateCategoryAsync(int id, UpdateTCategoryDTO dto);
        Task DeleteCategoryAsync(int id);
        Task<object> GetDashboardStatisticsAsync();
        // report
        Task<object> GetReportsAsync();
        Task UpdateReportStateAsync(UpdateReportDTO reportDTO);

    }
}
