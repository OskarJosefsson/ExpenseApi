using System.Text.Json.Serialization;

namespace ExpenseApi.Models.Dtos
{
    public class ChatGptReceipt
    {
        public List<Item>? Items { get; set; }
        public decimal TotalCost { get; set; }
        public string? ShopName { get; set; }
        public string? ReceiptCategory { get; set; }
    }

    public class Item
    {
        public required string Name { get; set; }
        public decimal Cost { get; set; }
        public string? Category { get; set; }
    }
}
