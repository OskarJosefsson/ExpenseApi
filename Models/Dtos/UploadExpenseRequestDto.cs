using System.ComponentModel.DataAnnotations;

namespace ExpenseApi.Models.Dtos
{
    public class UploadExpenseRequestDto
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
