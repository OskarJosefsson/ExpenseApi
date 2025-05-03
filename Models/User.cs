namespace ExpenseApi.Models
{
    public class User
    {
        public required Guid UserId { get; set; }
        public string? Name { get; set; }

    }
}
