using ExpenseApi.Models;

namespace ExpenseApi.Services
{

    public interface IStoreService
    {
       Store ConvertToStore(string name);
    }

    public class StoreService : IStoreService
    {
        public Store ConvertToStore(string name)
        {
            throw new NotImplementedException();
        }
    }
}
