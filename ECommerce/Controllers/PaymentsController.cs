using ECommerce.DTOs.Basket;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("{basketId}")]
        [Authorize]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentntent(string basketId)
        => Ok(await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId));

        [HttpPost("webHook")]
        public async Task<IActionResult> WebHook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var signatureHeader = Request.Headers["Stripe-Signature"];

                if (string.IsNullOrEmpty(signatureHeader))
                {
                    return BadRequest("Missing signature");
                }


                await _paymentService.UpdateOrderPaymentStatusAsync(json, signatureHeader);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
