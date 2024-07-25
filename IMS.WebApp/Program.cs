using IMS.Plugins.EFCoreSqlServer;
using IMS.Plugins.InMemory;
using IMS.UseCases.Activities;
using IMS.UseCases.Inventories;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Products;
using IMS.UseCases.Reporting;
using IMS.WebApp.Data;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// configure authorizations
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("Department", "Administration"));
    options.AddPolicy("Inventory", policy => policy.RequireClaim("Department", "InventoryManagement"));
    options.AddPolicy("Sales", policy => policy.RequireClaim("Department", "Sales"));
    options.AddPolicy("Purchasers", policy => policy.RequireClaim("Department", "Purchasing"));
    options.AddPolicy("Productions", policy => policy.RequireClaim("Department", "ProductionManagement"));
});

var constr = builder.Configuration.GetConnectionString("InventoryManagement");

//Configure EF Core for Identity
builder.Services.AddDbContext<AccountDbContext>(options =>
{
    options.UseSqlServer(constr);
});

//Configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
}).AddEntityFrameworkStores<AccountDbContext>();

builder.Services.AddDbContextFactory<IMSDbContext>(options =>
{
    options.UseSqlServer(constr);
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

#region "Repositories"

if (builder.Environment.IsEnvironment("Testing"))
{
    StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

    builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();
    builder.Services.AddSingleton<IProductRepository, ProductRepository>();
    builder.Services.AddSingleton<IInventoryTransactionRepository, InventoryTransactionRepository>();
    builder.Services.AddSingleton<IProductTransactionRepository, ProductTransactionRepository>();
}
else
{
    builder.Services.AddTransient<IInventoryRepository, InventoryEFCoreRepository>();
    builder.Services.AddTransient<IProductRepository, ProductEFCoreRepository>();
    builder.Services.AddTransient<IInventoryTransactionRepository, InventoryTransactionEFCoreRepository>();
    builder.Services.AddTransient<IProductTransactionRepository, ProductTransactionEFCoreRepository>();
}

#endregion "Repositories"

#region "UseCases"

// Activities
builder.Services.AddTransient<IPurchaseInventoryUseCase, PurchaseInventoryUseCase>();
builder.Services.AddTransient<IProduceProductUseCase, ProduceProductUseCase>();
builder.Services.AddTransient<ISellProductUseCase, SellProductUseCase>();

// Inventories
builder.Services.AddTransient<IViewInventoriesByNameUseCase, ViewInventoriesByNameUseCase>();
builder.Services.AddTransient<IAddInventoryUseCase, AddInventoryUseCase>();
builder.Services.AddTransient<IEditInventoryUseCase, EditInventoryUseCase>();
builder.Services.AddTransient<IViewInventoryByIdUseCase, ViewInventoryByIdUseCase>();

// Products
builder.Services.AddTransient<IViewProductsByNameUseCase, ViewProductsByNameUseCase>();
builder.Services.AddTransient<IAddProductUseCase, AddProductUseCase>();
builder.Services.AddTransient<IEditProductUseCase, EditProductUseCase>();
builder.Services.AddTransient<IViewProductByIdUseCase, ViewProductByIdUseCase>();

// Inventory Transactions
builder.Services.AddTransient<ISearchInventoryTransactionsUseCase, SearchInventoryTransactionsUseCase>();

// Product Transactions
builder.Services.AddTransient<ISearchProductTransactionsUseCase, SearchProductTransactionsUseCase>();

#endregion "UseCases"

var app = builder.Build();

if (app.Configuration.GetValue<bool>("UpdateDatabase"))
{
    // Migrate latest database changes during startup
    using (var scope = app.Services.CreateScope())
    {
        ILogger logger = app.Services.GetService<ILogger<Program>>();

        logger.LogInformation("Start migrations IMSDbContext");

        var dbContext = scope.ServiceProvider.GetRequiredService<IMSDbContext>();

        // Here is the migration executed
        dbContext.Database.Migrate();

        logger.LogInformation("End migrations IMSDbContext");
    }

    // Migrate latest database changes during startup
    using (var scope = app.Services.CreateScope())
    {
        ILogger logger = app.Services.GetService<ILogger<Program>>();

        logger.LogInformation("Start migrations AccountDbContext");

        var dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();

        // Here is the migration executed
        dbContext.Database.Migrate();

        logger.LogInformation("End migrations AccountDbContext");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();