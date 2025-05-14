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


        public int Id { get; set; }
        public required string Name { get; set; }
        public required decimal Cost { get; set; }
        public int Amount { get; set; } = 1;
        public ItemCategory? ItemCategory { get; set; }
        public int? ItemCategoryId { get; set; }



    }
}
