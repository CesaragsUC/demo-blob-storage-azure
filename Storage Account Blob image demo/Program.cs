using Microsoft.Extensions.DependencyInjection;
using Storage_Account_Blob_image_demo.Models;
using Storage_Account_Blob_image_demo.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddOptions();

//option 1 to read appsettings.Development.json
var AppName = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json")
    .Build()
    .GetSection("AzureConnections")["AzureConnectionsString"];

//option 2 to read appsettings.Development.json
// lê o arquivo appsettings.Development.json
var settings = builder.Configuration.GetSection("AzureConnections").Get<AzureConfiguration>();

//Register Services
builder.Services.AddSingleton<AzureBlobService>(x=> new AzureBlobService(settings.AzureConnectionsString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
