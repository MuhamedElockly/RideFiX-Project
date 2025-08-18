using AutoMapper;
using Domain.Entities.CoreEntites.CarMaintenance_Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Service.Exception_Implementation.ArgumantNullException;
using Service.Exception_Implementation.NotFoundExceptions;
using ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction;
using SharedData.DTOs.E_CommerceDTOs;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.CoreServices.E_Commerce
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IDatabase database;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IProductsService productsService;
        public ShoppingCartService(
            IConnectionMultiplexer redis,
            IHttpContextAccessor httpContextAccessor,
            IProductsService productsService,
            IMapper mapper)
        {
            database = redis.GetDatabase();
            this.httpContextAccessor = httpContextAccessor;
            this.productsService = productsService;
            this.mapper = mapper;
        }
        //public async Task AddToCartAsync(int productId, int quantity)
        //{
        //    if (productId <= 0 || quantity <= 0)
        //    {
        //        throw new ShoppingCartArgumentException("Product ID and quantity must be greater than zero.");
        //    }
        //    string userId = GetUserId();
        //    string cartKey = $"cart:{userId}";

        //    var Cart = await GetCartItemsAsync();
        //    var existingItem = Cart.FirstOrDefault(item => item.ProductId == productId);
        //    var product = await productsService.GetProductByIdAsync(productId);
        //    if (product == null)
        //    {
        //        throw new InvalidOperationException($"Product with ID {productId} not found.");
        //    }

        //    if ( product.Quantity >= quantity) {
        //        if (existingItem != null)
        //        {

        //            existingItem.Quantity += quantity;
        //        }
        //        else
        //        {

        //            product.Quantity = quantity;
        //            Cart.Add(product);
        //        }



        //    }

        //    var serializedCart = JsonSerializer.Serialize(Cart);
        //    await database.StringSetAsync(cartKey, serializedCart);
        //}
        public async Task AddToCartAsync(int productId, int quantity)
        {
            if (productId <= 0 || quantity <= 0)
            {
                throw new ShoppingCartArgumentException("Product ID and quantity must be greater than zero.");
            }

            string userId = GetUserId();
            string cartKey = $"cart:{userId}";

            var cart = await GetCartItemsAsync();
            var existingItem = cart.FirstOrDefault(item => item.ProductId == productId);

            var product = await productsService.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {productId} not found.");
            }

            if (existingItem != null)
            {
            
                int newQuantity = existingItem.Quantity + quantity;

                if (newQuantity > product.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock for product {product.ProductName}. Available: {product.Quantity}"
                    );
                }

                existingItem.Quantity = newQuantity;
            }
            else
            {
                if (quantity > product.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock for product {product.ProductName}. Available: {product.Quantity}"
                    );
                }

                product.Quantity = quantity; 
                cart.Add(product);
            }

            var serializedCart = JsonSerializer.Serialize(cart);
            await database.StringSetAsync(cartKey, serializedCart);
        }


        public Task ClearCartAsync()
        {
            string userId = GetUserId();
            string cartKey = $"cart:{userId}";
            return database.KeyDeleteAsync(cartKey);
        }

        public async Task<List<CartItemDto>> GetCartItemsAsync()
        {
            string userId = GetUserId();
            string cartKey = $"cart:{userId}";
            var value = await database.StringGetAsync(cartKey);
            if (value.IsNullOrEmpty)
            {
                return new List<CartItemDto>();
            }
            var cartItems = JsonSerializer.Deserialize<List<CartItemDto>>(value);
            return cartItems;
        }

        public async Task RemoveItemAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ShoppingCartArgumentException("Product ID must be greater than zero.");
            }
            string userId = GetUserId();
            string cartKey = $"cart:{userId}";
            var cartItems = await GetCartItemsAsync();
            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                var serializedCart = JsonSerializer.Serialize(cartItems);
                await database.StringSetAsync(cartKey, serializedCart);
            }
            else
            {
                throw new ItemNotFoundException($"Item with Product ID {productId} not found in the cart.");
            }
        }

        public async Task UpdateItemQuantityAsync(int productId, int newQuantity)
        {
            if (newQuantity <= 0 || productId <= 0)
            {
                throw new ShoppingCartArgumentException("Product ID and quantity must be greater than zero.");
            }

            string userId = GetUserId();
            string cartKey = $"cart:{userId}";
            var cartItems = await GetCartItemsAsync();

            var itemToUpdate = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (itemToUpdate != null)
            {
                cartItems.Remove(itemToUpdate);
                itemToUpdate.Quantity = newQuantity;
                cartItems.Add(itemToUpdate);
                var serializedCart = JsonSerializer.Serialize(cartItems);
                await database.StringSetAsync(cartKey, serializedCart);
            }
            else
            {
                throw new ItemNotFoundException($"Item with Product ID {productId} not found in the cart.");
            }

        }
        public string GetUserId ()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("User ID claim not found.");
            }
            return userIdClaim.Value;
        }
    }
}
