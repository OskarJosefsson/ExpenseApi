namespace ExpenseApi.Models
{
    public class Receipt
    {
        public Guid Id { get; set; }
        public decimal TotalCost { get; set; }
        public Store? Store { get; set; }
        public List<ReceiptItem>? Items { get; set; }
        public Category? Category { get; set; }
        public User User { get; set; }
    }
}
