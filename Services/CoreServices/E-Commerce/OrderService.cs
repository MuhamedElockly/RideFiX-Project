using AutoMapper;
using Domain.Contracts;
using Domain.Entities.e_Commerce;
using Service.Exception_Implementation.ArgumantNullException;
using Service.Exception_Implementation.NotFoundExceptions;
using Service.Specification_Implementation.E_CommerceSpecifications;
using ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction;
using SharedData.DTOs.E_CommerceDTOs;
using SharedData.Enums;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.CoreServices.E_Commerce
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IShoppingCartService shoppingCartService;
        private readonly IMapper mapper;

        public OrderService(IUnitOfWork unitOfWork,
                            IShoppingCartService shoppingCartService,
                            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.shoppingCartService = shoppingCartService;
            this.mapper = mapper;
        }

        public async Task<OrderDto> CreateOrderAsync(string location , string userId)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ShoppingCartArgumentException("Location must be provided.");
            }
            if (string.IsNullOrWhiteSpace(userId))
                throw new ShoppingCartArgumentException("UserId must be provided.");

            var cartItems = await shoppingCartService.GetCartItemsAsync();
            if (cartItems == null || !cartItems.Any())
            {
                throw new ShoppingCartArgumentException("Cart is empty.");
            }

            var order = new Domain.Entities.e_Commerce.Order
            {
                location = location,
                orderState = OrderStatus.Pending,
                totalPrice = 0 ,
                UserId=userId
            };

            foreach (var item in cartItems)
            {
                var product = await unitOfWork.GetRepository<Domain.Entities.e_Commerce.Product, int>()
                    .GetByIdAsync(item.ProductId);

                if (product == null)
                    throw new ItemNotFoundException($"Product with ID {item.ProductId} not found.");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"Not enough stock for product {product.Name}. Available: {product.StockQuantity}");

                product.StockQuantity -= item.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    totalPrice = product.Price * item.Quantity
                };

                order.orderItems.Add(orderItem);
            }

            order.totalPrice = order.orderItems.Sum(i => i.totalPrice);

            await unitOfWork.GetRepository<Domain.Entities.e_Commerce.Order, int>().AddAsync(order);
            await unitOfWork.SaveChangesAsync();

            await shoppingCartService.ClearCartAsync();

            var orderDto = mapper.Map<OrderDto>(order);
            return orderDto;
        }

   public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
{
            var spec = new OrdersByUserSpecification(userId);
    var orders = await unitOfWork.GetRepository<Domain.Entities.e_Commerce.Order, int>().GetAllAsync(spec);

    if (orders == null || !orders.Any())
        return new List<OrderDto>();

    return mapper.Map<List<OrderDto>>(orders);
}


        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ShoppingCartArgumentException("Order ID must be greater than zero.");

            var spec = new OrdersWithItemsSpecification(orderId);
            var order = await unitOfWork.GetRepository<Domain.Entities.e_Commerce.Order, int>().GetByIdAsync(spec);

            if (order == null)
                throw new ItemNotFoundException($"Order with ID {orderId} not found.");

            return mapper.Map<OrderDto>(order);
        }
    }
}
