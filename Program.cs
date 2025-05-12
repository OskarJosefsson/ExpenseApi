using ExpenseApi.Services;
using ExpenseApi.Data; 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ExpenseApi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ExpenseApi.Identity;
using ExpenseApi.Models;
using Microsoft.Extensions.Options;

namespace ExpenseApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            builder.Services.AddSingleton(resolver =>
            resolver.GetRequiredService<IOptions<JwtSettings>>().Value);

            builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("Google"));
            builder.Services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<GoogleSettings>>().Value);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(IdentityData.AdminUserPolicyName, p => 
                p.RequireClaim(IdentityData.AdminUserClaimName, "true"));

                options.AddPolicy(IdentityData.MemberUserPolicyName, p => 
                p.RequireClaim(IdentityData.MemberUserClaimName, "true"));

            });

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IExpenseRepo, ExpenseRepo>();
            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
            builder.Services.AddScoped<IReceiptRepo, ReceiptRepo>();
            builder.Services.AddScoped<IChatGippityService, ChatGippityService>();
            builder.Services.AddScoped<IReceiptService, ReceiptService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IStoreService, StoreService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<DatabaseInit>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });


            var app = builder.Build();

            // Initialize database
            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInit>();
                await initializer.InitializeDatabaseAsync();
            }


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}
