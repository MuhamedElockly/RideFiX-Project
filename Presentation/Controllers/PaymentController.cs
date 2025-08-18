using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.credit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using SharedData.DTOs.ChatDTOs;
using SharedData.DTOs.Payment;
using SharedData.Wrapper;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public PaymentController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("PaymentInt/{coins}")]
        public async Task<IActionResult> ProcessPayment(int coins)
        {
             var result = await _serviceManager.paymentService.CreateOrUpdatePaymentIntentRideCoinsAsync(coins);
            return Ok(result);
        }

        //[HttpPost("InitializePayment")]
        //public async Task<IActionResult> InitializePayment(int coins)
        //{
        //    var Id = await _serviceManager.paymentService.CreateChargeEntityAsync(coins);
        //    return Ok(Id);
        //}

        [HttpPost("CreateTransaction/{coinChargeId}")]
        public async Task<IActionResult> CreateTransaction(int coinChargeId)
        {
            if(coinChargeId <= 0)
            {
                return BadRequest( $"{ coinChargeId} is not a valid coin charge ID.");

            }
            await _serviceManager.paymentService.CreateTransaction(coinChargeId);
            return Ok(ApiResponse<CoinTopUp>.SuccessResponse(null, "transaction created successfully"));
        }
        [HttpGet("GetPersonalCoins")]
        public async Task<IActionResult> GetPersonalCoins()
        {
            var coins = await _serviceManager.paymentService.GetPersonalCoins();
            if (coins < 0)
            {
                return NotFound(ApiResponse<int>.FailResponse("No coins found"));
            }
            return Ok(ApiResponse<int>.SuccessResponse(coins, "Coins retrieved successfully"));
        }
    }
}
