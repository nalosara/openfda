using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddHttpClient(); // Register HttpClient
builder.Services.AddScoped<OpenFdaService>();
builder.Services.AddScoped<DrugService>();
builder.Services.AddScoped<AdverseEventService>();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddSession();
builder.Services.AddSingleton<ITempDataDictionaryFactory, TempDataDictionaryFactory>();

builder.Services.AddAuthorization();


// Register DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is null or empty.");
}
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Fetch and store drugs
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var drugService = services.GetRequiredService<DrugService>();
        await drugService.FetchAndStoreDrugsAsync();
        Console.WriteLine("Drugs fetched and stored successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while fetching drugs: {ex.Message}");
    }
}

// Fetch and store adverse events
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var adverseEventService = services.GetRequiredService<AdverseEventService>();
        await adverseEventService.FetchAndStoreAdverseEventsAsync(totalLimit: 500, batchSize: 500);
        Console.WriteLine("Adverse events fetched and stored successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while fetching adverse events: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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