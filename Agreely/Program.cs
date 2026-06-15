using Agreely.BackgroundServices;
using Agreely.Repositories.Interfaces;
using Agreely.Repositories.Repositories;
using Agreely.Services;
using Agreely.Services.Interfaces;
using Agreely.Services.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddScoped<Agreely.Data.DatabaseHelper>();

builder.Services.AddScoped<IGroupRepository, GroupRepository>(
    provider => new GroupRepository(connectionString));

builder.Services.AddScoped<IGroupMembershipRepository, GroupMembershipRepository>(
    provider => new GroupMembershipRepository(connectionString));

builder.Services.AddScoped<IGroupService, GroupService>();

builder.Services.AddScoped<ICommitmentRepository, CommitmentRepository>(
    provider => new CommitmentRepository(connectionString));

builder.Services.AddScoped<IVoteRepository, VoteRepository>(
    provider => new VoteRepository(connectionString));

builder.Services.AddScoped<ICommitmentService, CommitmentService>();

builder.Services.AddScoped<IVoteService, VoteService>();

builder.Services.AddScoped<IUserRepository, UserRepository>(
    provider => new UserRepository(connectionString));

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>(
    provider => new ActivityLogRepository(connectionString));

builder.Services.AddScoped<IActivityLogService, ActivityLogService>();

builder.Services.AddScoped<HealthStatusEvaluator>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>(
    provider => new NotificationRepository(connectionString));

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddHostedService<CommitmentHealthBackgroundService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

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

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
