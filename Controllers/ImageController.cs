using ExpenseApi.Models;
using ExpenseApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ExpenseApi.Models.Dtos.ChatGptReceipt;

    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages");
        private readonly IChatGippityService _chatGippityService;
        private readonly IImageService _imageService;
        private readonly IReceiptService _receiptService;

    public ImageController(IChatGippityService chatGippityService, IImageService imageService, IReceiptService receiptService)
    {
        if (!Directory.Exists(_imagePath))
        {
            Directory.CreateDirectory(_imagePath);
        }

        _chatGippityService = chatGippityService;
        _imageService = imageService;
        _receiptService = receiptService;
    }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [Authorize]
        public async Task<IActionResult> CreateExpenseFromImage(IFormFile file, User user)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

        var imageUrl = await _imageService.UploadImageAsync(file);

        var words = await _imageService.ExtractTextFromImageAsync(imageUrl.ToString());

        var response = await _chatGippityService.AnalyzeReceiptImage(words);

        var receipt = _receiptService.ConvertToReceipt(response);

        receipt = await _receiptService.AddReceipt(user, receipt);

        return Ok(new { Receipt = receipt });

        }
    }

