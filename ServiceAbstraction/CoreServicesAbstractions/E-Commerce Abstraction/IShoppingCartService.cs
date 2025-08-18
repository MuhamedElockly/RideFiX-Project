using SharedData.DTOs.E_CommerceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction
{
    public interface IShoppingCartService
    {
        public Task AddToCartAsync(int productId, int quantity);
        public Task<List<CartItemDto>> GetCartItemsAsync();
        public Task ClearCartAsync();
        public Task RemoveItemAsync(int productId);
        public Task UpdateItemQuantityAsync(int productId, int newQuantity);
        public string GetUserId();
    }
}
