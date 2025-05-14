using ExpenseApi.Models;
using ExpenseApi.Repositories;

namespace ExpenseApi.Services
{

    public interface IStoreService
    {
        Task<IEnumerable<Store>> GetStoresAsync();
        Task<Store> GetStoreAsync(int id);
        Task<Store> CreateStoreAsync(Store store);
        Task<bool> UpdateStoreAsync(int id, Store store);
        Task<bool> DeleteStoreAsync(int id);

        Store ConvertToStore(string name);
    }

    public class StoreService : IStoreService
    {
        public Store ConvertToStore(string name)
        {
            throw new NotImplementedException();
        }

        private readonly IStoreRepo _storeRepository;

        public StoreService(IStoreRepo storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<IEnumerable<Store>> GetStoresAsync()
        {

            return await _storeRepository.GetAllAsync();
        }

        public async Task<Store> GetStoreAsync(int id)
        {

            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null)
            {

            }
            return store;
        }

        public async Task<Store> CreateStoreAsync(Store store)
        {

            if (string.IsNullOrEmpty(store.Name))
            {
                throw new ArgumentException("Store name cannot be empty.");
            }

            return await _storeRepository.CreateAsync(store);
        }

        public async Task<bool> UpdateStoreAsync(int id, Store store)
        {
            if (string.IsNullOrEmpty(store.Name))
            {
                throw new ArgumentException("Store name cannot be empty.");
            }

            return await _storeRepository.UpdateAsync(store);
        }

        public async Task<bool> DeleteStoreAsync(int id)
        {

            return await _storeRepository.DeleteAsync(id);
        }
    }
}
