using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.BLL.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "ClientScheme";
    options.DefaultSignInScheme = "ClientScheme";
    options.DefaultChallengeScheme = "ClientScheme";
})
.AddCookie("ClientScheme", options =>
{
    options.LoginPath = "/Users/Login";
    //options.AccessDeniedPath = "/Users/AccessDenied";
    options.Cookie.Name = "ClientAuthCookie";
})
.AddCookie("AdminScheme", options =>
{
    options.LoginPath = "/Admin/AdminLogin";
    options.LogoutPath = "/Admin/AdminLogout";
    //options.AccessDeniedPath = "/Admin/AccessDenied";
    options.Cookie.Name = "AdminAuthCookie";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Admin}/{action=Index}/{id?}"
);

app.Run();
