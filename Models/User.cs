namespace ExpenseApi.Models
{
    public class User
    {
        public Guid UserId { get; set; } = Guid.NewGuid();

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? AvatarUrl { get; set; }

        public string Provider { get; set; } = string.Empty;

        public string ProviderUserId { get; set; } = string.Empty;

        public string Type { get; set; } = "Standard"; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }
    }

}
