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
using exe201.Service.AI;
using Microsoft.AspNetCore.DataProtection.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<EcommerceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var huggingFaceApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrEmpty(huggingFaceApiKey))
{
    throw new Exception("OPENAI_API_KEY is missing! Please set the API key in your environment variables.");
}

Console.WriteLine($"OPENAI_API_KEY Key: {huggingFaceApiKey}");

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddScoped<CohereService>();

//builder.Services.AddHttpClient("Cohere", client =>
//{
//    client.BaseAddress = new Uri("https://api.cohere.ai/v1/generate");
//    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Environment.GetEnvironmentVariable("OPENAI_API_KEY")}");
//});


builder.Services.AddHttpClient("Cohere", client =>
{
    client.BaseAddress = new Uri("https://api.cohere.ai/v1/");
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

//builder.Services.AddDataProtection()
//    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "keys"))) 
//    .SetApplicationName("EcommerceApp"); 


// Cấu hình DataProtection với FileSystemXmlRepository và ILoggerFactory
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "keys")))
    .SetApplicationName("EcommerceApp")
    .AddKeyManagementOptions(options =>
    {
        // Thêm ILoggerFactory vào FileSystemXmlRepository
        var loggerFactory = builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        options.XmlRepository = new FileSystemXmlRepository(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "keys")), loggerFactory);
    });


var app = builder.Build();




var connString = builder.Configuration.GetConnectionString("PostgreSqlConnection");
if (string.IsNullOrEmpty(connString))
{
    throw new Exception("Connection string 'PostgreSqlConnection' is null or empty!");
}
Console.WriteLine($"Connection string: {connString}");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();

    endpoints.MapGet("/", context =>
    {
        context.Response.Redirect("/Home/Index");
        return Task.CompletedTask;
    });
});
app.MapRazorPages();

app.Run();
