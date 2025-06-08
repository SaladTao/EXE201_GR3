using exe201.Models;
using exe201.Repository;
using exe201.Repository.Order;
using exe201.Repository.OrderDetail;
using exe201.Repository.Product;
using exe201.Service;
using exe201.Service.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Add DbContexts
builder.Services.AddDbContext<EcommerceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")));

// Add session services and cache BEFORE build
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrEmpty(openAiApiKey))
{
    throw new Exception("OPENAI_API_KEY is missing!");
}

Console.WriteLine($"OPENAI API Key: {openAiApiKey}"); // Kiểm tra xem API key có được lấy đúng không.


// Add HttpContextAccessor if needed
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

var app = builder.Build();
var connString = builder.Configuration.GetConnectionString("PostgreSqlConnection");
if (string.IsNullOrEmpty(connString))
{
    throw new Exception("Connection string 'PostgreSqlConnection' is null or empty!");
}
Console.WriteLine($"Connection string: {connString}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add Session middleware BEFORE Authorization
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
