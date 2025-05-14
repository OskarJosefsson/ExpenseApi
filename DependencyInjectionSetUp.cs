// DependencyInjectionSetup.cs
using Microsoft.Extensions.DependencyInjection;
using ExpenseApi.Services;
using ExpenseApi.Repositories;
using ExpenseApi.Data;

namespace ExpenseApi
{
    public static class DependencyInjectionSetup
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IReceiptService, ReceiptService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IChatGippityService, ChatGippityService>();

            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IStoreRepo, StoreRepo>();
            services.AddScoped<IReceiptRepo, ReceiptRepo>();
            services.AddScoped<IReceiptItemRepo, ReceiptItemRepo>();
            services.AddScoped<IItemCategoryRepo, ItemCategoryRepo>();

            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
            services.AddScoped<DatabaseInit>();
        }
    }
}
