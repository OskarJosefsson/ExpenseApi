namespace ExpenseApi.Models
{
    public class ReceiptUsers
    {
        public Guid User { get; set; }
        public Guid Receipt { get; set; }
        public int Percentage { get; set; }
    }
}
