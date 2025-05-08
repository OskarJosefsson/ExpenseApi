using System.Diagnostics.CodeAnalysis;

namespace ExpenseApi.Models
{
    public class ReceiptItem
    {
        [SetsRequiredMembers]
        public ReceiptItem(string name, decimal cost)
        {
            Name = name;
            Cost = cost;
        }

        public required string Name { get; set; }
        public required decimal Cost { get; set; }
        public ItemCategory? ItemCategory { get; set; }
    }
}