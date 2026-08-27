using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SATH.Aitisis.App.Common.Users;
using SATH.Aitisis.App.Services.Applications;
using SATH.Aitisis.App.Services.Applications.StatusTransitions;
using SATH.Aitisis.App.Services.ApplicationTypes;
using SATH.Aitisis.App.Services.DashboardStatistics;
using SATH.Aitisis.App.Services.Employees;
using SATH.Aitisis.App.Services.Notifications;
using SATH.Aitisis.Infrastructure.Identity;
using SATH.Aitisis.Infrastructure.Persistence;
using SATH.Aitisis.Infrastructure.Repositories.Applications;
using SATH.Aitisis.Infrastructure.Repositories.ApplicationTypes;
using SATH.Aitisis.Infrastructure.Repositories.DashboardStatistics;
using SATH.Aitisis.Infrastructure.Repositories.Notifications;
using SATH.Aitisis.Infrastructure.Services.Employees;
using SATH.Aitisis.Infrastructure.Storage;
using SATH.Aitisis.App.Services.ApprovalCertificates;
using SATH.Aitisis.Infrastructure.Services.ApprovalCertificates;
using SATH.Aitisis.App.Services.Payments;
using SATH.Aitisis.Infrastructure.Repositories.Payments;
using SATH.Aitisis.Infrastructure.Payments.Stripe;


var builder = WebApplication.CreateBuilder(args);

//to knoww that when I'm saying SathAitisisDbContext I need to use MySQL 
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<SathAitisisDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));



// it's important to say that I am using Identity but with my special ApplicationUser class and IdentityRole class
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<SathAitisisDbContext>()   // I tell Identity to use my special DbContext
    .AddDefaultTokenProviders();

// Identity -> ApplicationUser / IdentityRole -> SathAitisisDbContext -> MySQL




// the IEmployeeManagementService interface is implemented by the EmployeeManagementService class which is registered as a scoped service in the dependency injection container
// This means that a new instance of EmployeeManagementService will be created for each HTTP request, and it will be shared across the components that require it within that request.
builder.Services.AddScoped<IEmployeeManagementService, EmployeeManagementService>();


// the IApplicationTypeService interface is implemented by the ApplicationTypeService class that is registered
// as a scoped service in the dependency injection container
builder.Services.AddScoped<IApplicationTypeService, ApplicationTypeService>();

// the IApplicationTypeRepository interface is implemented by the ApplicationTypeRepository class that is registered
// as a scoped service in the dependency injection container
// that way the ApplicationTypeService can use the ApplicationTypeRepository to access the database for the CRUD operations on ApplicationType entities
builder.Services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();


builder.Services.AddScoped<IApplicationService, ApplicationService>();


builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.AddScoped<IUserLookupService, UserLookupService>();

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


builder.Services.AddScoped<IDashboardStatisticsService, DashboardStatisticsService>();
builder.Services.AddScoped<IDashboardStatisticsRepository, DashboardStatisticsRepository>();

// to handle the transition of application statuses independently of the application service class
builder.Services.AddScoped<IApplicationStatusTransitionPolicy, ApplicationStatusTransitionPolicy>();

// for the pdfs 
builder.Services.AddScoped<IApprovalCertificateService, ApprovalCertificateService>();
builder.Services.AddScoped<IApprovalCertificatePdfGenerator, QuestPdfApprovalCertificateGenerator>();


// for the payments
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentGateway, StripePaymentGatewayAdapter>();
builder.Services.Configure<StripePaymentOptions>(builder.Configuration.GetSection("Stripe"));



// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();



// Seeds the predefined application roles on startup (the predifined role is actually the Admin)
using (var scope = app.Services.CreateScope())
{
    // from SATH.Aitisis.Infrastructure.Identity.IdentitySeeder
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await IdentitySeeder.SeedRolesAsync(roleManager);
    await IdentitySeeder.SeedAdminAsync(userManager);
}



app.UseRouting();


app.UseAuthentication();   // who is that user?
app.UseAuthorization();    // what can that user do?





app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// because Microsoft says that Identity default UI uses Razor Pages
app.MapRazorPages();


app.Run();
