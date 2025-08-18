using SharedData.DTOs.E_CommerceDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string location , string userId );
        Task<List<OrderDto>> GetUserOrdersAsync(string userId);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
    }
}
