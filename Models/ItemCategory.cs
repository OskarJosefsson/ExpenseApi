using System.Diagnostics.CodeAnalysis;

namespace ExpenseApi.Models
{
    public class ItemCategory
    {
        [SetsRequiredMembers]
        public ItemCategory(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public required string Name { get; set; }
    }
}