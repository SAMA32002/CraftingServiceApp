using System.Text.Json.Serialization;
using CraftingServiceApp.AdminAPI.Extentions;
using CraftingServiceApp.AdminAPI.Helpers;
using CraftingServiceApp.AdminAPI.Interfaces;
using CraftingServiceApp.AdminAPI.Services;
using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerService();
builder.Services.AddAutoMapper(typeof(MappingProfile));


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
builder.Services.AddTransient<EmailService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<CraftingServiceApp.BLL.Interfaces.IPostService, CraftingServiceApp.BLL.Interfaces.PostService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ISliderItemService, SliderItemService>();
builder.Services.AddScoped<IFileService, FileService>();



builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerMiddlewares();
}

app.UseRouting();
app.UseCors("AllowAll"); 
app.UseAuthorization();
app.MapControllers();
app.Run();


