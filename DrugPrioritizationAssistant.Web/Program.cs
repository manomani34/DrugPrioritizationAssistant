using DrugPrioritizationAssistant.Application.Features.Drugs.Commands;
using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Application.Services;
using DrugPrioritizationAssistant.Infrastructure.Identity;
using DrugPrioritizationAssistant.Infrastructure.Persistence;
using DrugPrioritizationAssistant.Infrastructure.Repositories;
using DrugPrioritizationAssistant.Infrastructure.Services;
using DrugPrioritizationAssistant.Web.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// =========================================================
// MVC
// =========================================================

builder.Services.AddControllersWithViews();


// =========================================================
// Database
// =========================================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));


// =========================================================
// Identity
// =========================================================

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
    {
        // Password
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        // User
        options.User.RequireUniqueEmail = true;

        // Lockout
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// =========================================================
// Identity Cookie
// =========================================================

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath =
        "/Identity/Account/Login";

    options.AccessDeniedPath =
        "/Identity/Account/AccessDenied";

    options.ExpireTimeSpan =
        TimeSpan.FromHours(8);

    options.SlidingExpiration = true;
});


// =========================================================
// Authorization
// =========================================================

builder.Services.AddAuthorization();

builder.Services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();

builder.Services.AddScoped<
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();


// =========================================================
// Repositories
// =========================================================

builder.Services.AddScoped<
    IDrugRepository,
    DrugRepository>();

builder.Services.AddScoped<
    IDrugNeedAssessmentRepository,
    DrugNeedAssessmentRepository>();

builder.Services.AddScoped<
    IProductionFeasibilityRepository,
    ProductionFeasibilityRepository>();

builder.Services.AddScoped<
    IDrugScoreRepository,
    DrugScoreRepository>();

builder.Services.AddScoped<
    IScoringSettingsRepository,
    ScoringSettingsRepository>();


// =========================================================
// Application Services
// =========================================================

builder.Services.AddScoped<
    IOpportunityScoreCalculator,
    OpportunityScoreCalculator>();

builder.Services.AddScoped<
    IRankingService,
    RankingService>();

builder.Services.AddScoped<
    IDrugPriorityExplanationService,
    DrugPriorityExplanationService>();

builder.Services.AddScoped<
    IScoringSettingsService,
    ScoringSettingsService>();


// =========================================================
// Infrastructure Services
// =========================================================

builder.Services.AddScoped<
    IPermissionService,
    PermissionService>();


// =========================================================
// MediatR
// =========================================================

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(CreateDrugCommand).Assembly));


// =========================================================
// Build
// =========================================================

var app = builder.Build();


// =========================================================
// Middleware
// =========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// =========================================================
// Authentication & Authorization
// =========================================================

app.UseAuthentication();

app.UseAuthorization();


// =========================================================
// Area Route
// =========================================================

app.MapControllerRoute(
    name: "areas",
    pattern:
        "{area:exists}/{controller=Home}/{action=Index}/{id?}");


// =========================================================
// Default Route
// =========================================================

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}");


// =========================================================
// Identity & Authorization Seed
// =========================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<AppDbContext>();

    var userManager =
        services.GetRequiredService<
            UserManager<ApplicationUser>>();

    var roleManager =
        services.GetRequiredService<
            RoleManager<IdentityRole<int>>>();


    await IdentitySeeder.SeedAsync(
        userManager,
        roleManager);


    await AuthorizationSeeder.SeedAsync(
        context,
        roleManager);
}


// =========================================================
// Run
// =========================================================

app.Run();