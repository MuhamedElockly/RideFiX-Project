using SharedData.DTOs.E_CommerceDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string location);
        Task<List<OrderDto>> GetUserOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(int orderId);
    }
}
