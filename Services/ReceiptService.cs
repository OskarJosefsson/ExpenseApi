using ExpenseApi.Models;
using ExpenseApi.Models.Dtos;
using ExpenseApi.Repositories;

namespace ExpenseApi.Services
{
    public interface IReceiptService
    {
        Receipt ConvertToReceipt(ChatGptReceipt receipt);
        Task<Receipt?> AddReceipt(User user, Receipt receipt);
    }
    public class ReceiptService : IReceiptService
    {
        private readonly IStoreService _storeService;
        private readonly ICategoryService _categoryService;
        private readonly IUserRepo _userRepo;
        private readonly IReceiptRepo _receiptRepo;
        public ReceiptService(IStoreService storeService, ICategoryService categoryService, IUserRepo userRepo, IReceiptRepo receiptRepo)
        {
            _storeService = storeService;
            _categoryService = categoryService;
            _userRepo = userRepo;
            _receiptRepo = receiptRepo;
        }

        public async Task<Receipt?> AddReceipt(User user, Receipt receipt)
        {
           
            var existingUser = await _userRepo.GetByIdAsync(user.UserId);
            if (existingUser == null)
                return null;

            receipt.User = existingUser;

            await _receiptRepo.CreateAsync(receipt);

            return receipt;
        }

        public Receipt ConvertToReceipt(ChatGptReceipt input)
        {

            Receipt receipt = new Receipt();

            if(input.Items != null)
            receipt.Items = ConvertToReceiptItem(input.Items);

            if(!string.IsNullOrEmpty(input.ShopName))
            receipt.Store = _storeService.ConvertToStore(input.ShopName);

            if(!string.IsNullOrEmpty(input.ReceiptCategory))
            receipt.Category = _categoryService.ConvertToCategory(input.ReceiptCategory);

            receipt.TotalCost = input.TotalCost;


            return receipt;

        }

        public List<ReceiptItem> ConvertToReceiptItem(List<Item> input)
        {
            List<ReceiptItem> list = new List<ReceiptItem>();

            foreach (var item in input)
            {
              var newItem = new ReceiptItem(item.Name, item.Cost);

                if (!string.IsNullOrEmpty(item.Category))
                {
                    newItem.ItemCategory = new ItemCategory(item.Category); 
                }

                list.Add(newItem);
            }

            return list;
        }
    }
}
