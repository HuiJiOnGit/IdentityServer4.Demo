using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using IdentityServer.Empty;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Empty.BackgroundServices;
using IdentityServer.Empty.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer.Empty.Data;

#region log

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Debug)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();
Log.Information("Starting host...");

#endregion log

#region ConfigureServices

/*
 * SameSite策略问题,chrome80后会出现
 * 1.解决方案可见 https://www.cnblogs.com/i3yuan/p/14033016.html#autoid-20-0-0
 * 1.1 将http升级为https
 */

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("IdentityServer4");
var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

//Identity详细配置可见 https://www.cnblogs.com/i3yuan/p/14327822.html
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password = new PasswordOptions
    {
        RequireDigit = true, //要求有数字介于0-9 之间,  默认true
        RequiredLength = 5, //要求密码最小长度，   默认是 6 个字符
        RequireLowercase = true, //要求小写字母,  默认true
        RequireNonAlphanumeric = true, //要求特殊字符,  默认true
        RequiredUniqueChars = 1, //要求需要密码中的非重复字符数,  默认1
        RequireUppercase = true //要求大写字母 ，默认true
    };
    options.Lockout = new LockoutOptions
    {
        AllowedForNewUsers = false, // 新用户锁定账户, 默认true
        DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(5), //锁定时长，默认是 5 分钟
        MaxFailedAccessAttempts = 3 //登录错误最大尝试次数，默认 5 次
    };
    //这里需要注意的是 options.SignIn.RequireConfirmedAccount 设置项，缺省设置为true，
    //这种情况下，新注册的用户需要进行确认才能完成注册，如果没有安装邮件系统，这个步骤无法完成，所以这里改为false。
    options.SignIn = new SignInOptions
    {
        RequireConfirmedAccount = false,
        RequireConfirmedEmail = false,
        RequireConfirmedPhoneNumber = false,
    };
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

#region IdentityServer4

builder.Services.AddIdentityServer(options =>
{
    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
    options.EmitStaticAudienceClaim = true;
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
})
//.AddInMemoryIdentityResources(Config.IdentityResources)
//.AddInMemoryApiScopes(Config.ApiScopes)
//.AddInMemoryClients(Config.Clients)
//.AddTestUsers(TestUsers.Users)
//.AddDeveloperSigningCredential();
.AddConfigurationStore(options =>
{
    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
        sql => sql.MigrationsAssembly(migrationsAssembly));
})
.AddOperationalStore(options =>
{
    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
        sql => sql.MigrationsAssembly(migrationsAssembly));
    // 自动清理 token ，可选
    options.EnableTokenCleanup = true;
    // 自动清理 token ，可选
    options.TokenCleanupInterval = 30;
})
.AddAspNetIdentity<ApplicationUser>()
.AddDeveloperSigningCredential();

#endregion IdentityServer4

//初始化数据,初始化之后可以注释
//builder.Services.AddHostedService<InitializeDatabaseBackgroundServices>();

var app = builder.Build();
#endregion ConfigureServices

#region Configure

app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseIdentityServer();
app.UseAuthentication();;
app.UseAuthorization();//必须要用授权中间件,因为授权页面需要这个
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.Run();

#endregion Configure

