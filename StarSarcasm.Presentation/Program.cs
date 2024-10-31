using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Interfaces.IFileUploadService;
using StarSarcasm.Application.Interfaces.ISMSService;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Domain.Entities.Email;
using StarSarcasm.Infrastructure.BackgroundJobs;
using StarSarcasm.Infrastructure.Data;
using StarSarcasm.Infrastructure.Hubs;
using StarSarcasm.Infrastructure.Services;
using StarSarcasm.Infrastructure.Services.FileUploadService;
using StarSarcasm.Infrastructure.Services.SMSServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "StarSarcasm",

    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {

        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter Token"

    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer",
                },Name="Bearer",
                In=ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        ValidIssuer = builder.Configuration["JWT:issuer"],
        ValidAudience = builder.Configuration["JWT:audience"],
        ClockSkew = TimeSpan.Zero
    };

});

// Hangfire Service
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<MessageScheduler>(); 
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<UserCleanUpService>();
builder.Services.AddScoped<UserCleanUpScheduler>();
builder.Services.AddScoped<AwardDrawService>();
builder.Services.AddScoped<DrawScheduler>();

//Firebase Services 
builder.Services.AddScoped<FirebaseNotificationService>();

// Identity Services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders()
    .AddSignInManager<SignInManager<ApplicationUser>>();

// Email Configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// inject services 
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
    o.MaximumReceiveMessageSize = 1024 * 1024;
});
builder.Services.AddTransient<IOTPService, OTPService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserDrawService, UserDrawService>();
builder.Services.AddScoped<IAwardDrawService,AwardDrawService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<IAppMessageService, AppMessageService>();
builder.Services.AddScoped<IPostservice, PostService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


builder.Services.AddDbContext<Context>(options =>
            options.UseSqlServer(builder.Configuration
            .GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard("/dashborad");
using (var scope = app.Services.CreateScope())
{
    var messageScheduler = scope.ServiceProvider.GetRequiredService<MessageScheduler>();
    var userCleanUpScheduler = scope.ServiceProvider.GetRequiredService<UserCleanUpScheduler>();
	var drawScheduler = scope.ServiceProvider.GetRequiredService<DrawScheduler>();

	messageScheduler.ScheduleMessagesForUnsubscribedUsers();
    messageScheduler.ScheduleMessagesForSubscribedUsers();
   // messageScheduler.ScheduleMessagesForSubscribedUsersTest();
    userCleanUpScheduler.UserCleanUp();
	drawScheduler.ScheduleDrawEnd();
}
app.UseCors();
app.MapHub<ChatHub>("/ChatHub");
app.MapControllers();

app.Run();
