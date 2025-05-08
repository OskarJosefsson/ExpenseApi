using ExpenseApi.Models;

namespace ExpenseApi.Services
{
    public interface ICategoryService
    {
        Category ConvertToCategory(string name);

        ItemCategory ConvertToItemCategory(string name);
    }

    public class CategoryService : ICategoryService
    {
        public Category ConvertToCategory(string name)
        {
            throw new NotImplementedException();
        }

        public ItemCategory ConvertToItemCategory(string name)
        {
            throw new NotImplementedException();
        }
    }
}
