using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Qless.Repositories.Data;
using Qless.Repositories.Interfaces;
using Qless.Repositories.Repositories;
using Qless.Services.Interfaces;
using Qless.Services.Services;
using Qless.WebAPI.Middleware;

namespace Qless.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

            var corsPolicyName = "AnyOrigin";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: corsPolicyName,
                    policy =>
                    {
                        policy.WithOrigins("*")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            // Add services to the container.
            builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
            builder.Services.AddScoped<ICardService, CardService>();
            builder.Services.AddScoped<IDiscountService, DiscountService>();
            builder.Services.AddScoped<ICardRepository, CardRepository>();
            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(corsPolicyName);

            app.UseAuthorization();

            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}