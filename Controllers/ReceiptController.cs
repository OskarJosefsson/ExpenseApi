using ExpenseApi.Models;
using ExpenseApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateReceipt([FromBody] Receipt receipt)
        {
            if (receipt == null)
            {
                return BadRequest("Receipt cannot be null");
            }

            User user = new User();

            var createdReceipt = await _receiptService.AddReceipt(user, receipt);

            if (createdReceipt == null)
            {
                return StatusCode(500, "An error occurred while adding the receipt.");
            }

            return CreatedAtAction(nameof(CreateReceipt), new { id = createdReceipt.Id }, createdReceipt);
        }


    }
}
