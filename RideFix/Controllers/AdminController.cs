using Domain.Entities.CoreEntites.EmergencyEntities;
using Microsoft.AspNetCore.Mvc;
using Presistence.Migrations;
using ServiceAbstraction;
using SharedData.DTOs.ActivityDTOs;
using SharedData.DTOs.Admin.TechnicianCategory;
using SharedData.DTOs.Admin.Users;
using SharedData.DTOs.ReportDtos;
using SharedData.Wrapper;
using System;
using System.Collections.Generic;

namespace RideFix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IServiceManager serviceManager;
        public AdminController(IServiceManager _serviceManager)
        {
            serviceManager = _serviceManager;
        }

        //  Users 

        [HttpGet("carOwners")]
        [EndpointSummary("Get all car owner users")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<List<ReadUsersDTO>>))]
        public async Task<IActionResult> GetAllCarOwnerUsers()
        {
            var users = await serviceManager.adminService.GetAllCarOwnersAsync();
            return Ok(ApiResponse<List<ReadUsersDTO>>.SuccessResponse(users, "Successful request"));
        }

        [HttpGet("technicians")]
        [EndpointSummary("Get all technician users")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<List<ReadUsersDTO>>))]
        public async Task<IActionResult> GetAllTechnicianUsers()
        {
            var users = await serviceManager.adminService.GetAllTechniciansAsync();
            return Ok(ApiResponse<List<ReadUsersDTO>>.SuccessResponse(users, "Successful request"));
        }

        [HttpPost("carOwner/{id}")]
        [EndpointSummary("ban car onwer by ID")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(404, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> BanCarOwnerUser(int id)
        {
            await serviceManager.adminService.BanCarOwnerAsync(id);


            return Ok(ApiResponse<string>.SuccessResponse(null, "User soft deleted successfully"));
        }

        [HttpPost("tecnician/{id}")]
        [EndpointSummary("ban tecnician by ID")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(404, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> BanTechnicianUser(int id)
        {
            await serviceManager.adminService.BanTechnianAsync(id);


            return Ok(ApiResponse<string>.SuccessResponse(null, "User soft deleted successfully"));
        }

        [HttpPost("carOwner/{id}/active")]
        [EndpointSummary("active a previously baned user by ID")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(404, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> ActiveCarOwnerUser(int id)
        {
            await serviceManager.adminService.ActivateCarOwonerAsync(id);


            return Ok(ApiResponse<string>.SuccessResponse(null, "User restored successfully"));
        }



        [HttpPost("technician/{id}/active")]
        [EndpointSummary("active a previously baned user by ID")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(404, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> ActiveTechnicianUser(int id)
        {
            await serviceManager.adminService.ActivateTechnianAsync(id);


            return Ok(ApiResponse<string>.SuccessResponse(null, "User restored successfully"));
        }

        // Categories 

        [HttpGet("categories")]
        [EndpointSummary("Get all technician categories")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<List<ReadTCategoryDTO>>))]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await serviceManager.adminService.GetAllCategoriesAsync();
            return Ok(ApiResponse<List<ReadTCategoryDTO>>.SuccessResponse(categories, "Successful request"));
        }

        [HttpPost("categories")]
        [EndpointSummary("Create a new technician category")]
        [ProducesResponseType(201, Type = typeof(ApiResponse<ReadTCategoryDTO>))]
        public async Task<IActionResult> CreateCategory(CreateTCategoryDTO dto)
        {
            await serviceManager.adminService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetAllCategories), ApiResponse<string>.SuccessResponse("", "Category created successfully"));
        }

        [HttpPut("categories/{id}")]
        [EndpointSummary("Update an existing technician category")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
        [ProducesResponseType(404, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UpdateCategory(int id, UpdateTCategoryDTO dto)
        {
            await serviceManager.adminService.UpdateCategoryAsync(id, dto);


            return Ok(ApiResponse<bool>.SuccessResponse(true, "Category updated successfully"));
        }

        [HttpDelete("categories/{id}")]
        [EndpointSummary("Soft delete a technician category by ID")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType(404, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> SoftDeleteCategory(int id)
        {
            await serviceManager.adminService.DeleteCategoryAsync(id);


            return Ok(ApiResponse<string>.SuccessResponse(null, "Category soft deleted successfully"));
        }

        [HttpGet("users-count")]
        public async Task<IActionResult> GetUsersCount()
        {
            var counts = await serviceManager.adminService.GetUsersCountAsync();
            return Ok(counts);
        }
        [HttpGet("requests-count")]
        public async Task<IActionResult> GetRequestsCount()
        {
            var counts = await serviceManager.adminService.GetrequestsCountAsync();
            return Ok(counts);
        }

        [HttpGet("activities")]
        [EndpointSummary("Get all system activities categorized by type")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<CategorizedActivityReportDTO>))]
        public async Task<IActionResult> GetCategorizedActivities([FromQuery] int hoursBack = 12)
        {
            try
            {
                if (hoursBack <= 0 || hoursBack > 168) 
                {
                    return BadRequest(ApiResponse<CategorizedActivityReportDTO>.FailResponse("Hours back must be between 1 and 168 (7 days)"));
                }

                var result = await serviceManager.activityReportService.GetCategorizedActivitiesAsync(hoursBack);
                
                return Ok(ApiResponse<CategorizedActivityReportDTO>.SuccessResponse(result, "تم جلب النشاطات المصنفة بنجاح"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CategorizedActivityReportDTO>.FailResponse("حدث خطأ أثناء جلب النشاطات المصنفة"));
            }
        }
        [HttpGet("dashboard-statistics")]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            var stats = await serviceManager.adminService.GetDashboardStatisticsAsync();
            return Ok(ApiResponse<object>.SuccessResponse(stats,"successfull request"));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var results=await serviceManager.adminService.GetReportsAsync();
          
            return Ok(ApiResponse<object>.SuccessResponse(results,"success response"));
        }

    }
}
