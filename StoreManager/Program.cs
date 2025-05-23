using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StoreManager.ApiServices.DummyRest;
using StoreManager.Domain.IRepo;
using StoreManager.Domain.IService;
using StoreManager.Features.EmailHelper;
using StoreManager.Features.ExcellHelper;
using StoreManager.Features.ImageHelper;
using StoreManager.Infrastructure.Data;
using StoreManager.Infrastructure.Data.Repo;
using StoreManager.Infrastructure.Service;
using StoreManager.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("default");

// Configure Serilog
builder.Services.AddLogging();
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/StoreManagerLogs.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

// Replace the default logging with Serilog
builder.Services.AddSingleton(Log.Logger);
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString)
    );
builder.Services.AddIdentity<AppUser, IdentityRole>(
    options =>
    {

    }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IDummyRestApiService, DummyRestApiService>();

builder.Services.AddScoped<IEmailHelper, EmailHelper>();
builder.Services.AddScoped<IExcellHelper, ExcellHelper>();
builder.Services.AddScoped<IImageHelper, ImageHelper>();
builder.Services.AddHttpClient();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseExceptionHandler("/Error");
    //app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();

    context.Database.Migrate();

    var userMgr = services.GetRequiredService<UserManager<AppUser>>();
    var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();

    await IdentitySeedData.Initialize(context, userMgr, roleMgr);
}

app.Run();
