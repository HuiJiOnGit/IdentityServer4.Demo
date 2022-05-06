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
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();
Log.Information("Starting host...");

#endregion log

#region ConfigureServices

/*
 * ����������
 * 1.�û���¼���޷���ת�ؿͻ���.��������ο� https://www.cnblogs.com/i3yuan/p/14033016.html#autoid-20-0-0
 * 1.1 ����Ϊhttps, ���߽�cookies�� SameSite ���Ը�Ϊ Lax
 */

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddControllersWithViews();

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("IdentityServer4");
var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

//Identity��ϸ���ÿ��Բο���� https://www.cnblogs.com/i3yuan/p/14327822.html
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password = new PasswordOptions
    {
        RequireDigit = true, //Ҫ�������ֽ���0-9 ֮��,  Ĭ��true
        RequiredLength = 5, //Ҫ��������С���ȣ�   Ĭ���� 6 ���ַ�
        RequireLowercase = true, //Ҫ��Сд��ĸ,  Ĭ��true
        RequireNonAlphanumeric = true, //Ҫ�������ַ�,  Ĭ��true
        RequiredUniqueChars = 1, //Ҫ����Ҫ�����еķ��ظ��ַ���,  Ĭ��1
        RequireUppercase = true //Ҫ���д��ĸ ��Ĭ��true
    };
    options.Lockout = new LockoutOptions
    {
        AllowedForNewUsers = false, // ���û������˻�, Ĭ��true
        DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(5), //����ʱ����Ĭ���� 5 ����
        MaxFailedAccessAttempts = 3 //��¼��������Դ�����Ĭ�� 5 ��
    };
    options.SignIn.RequireConfirmedAccount = true;
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
    // �Զ����� token ����ѡ
    options.EnableTokenCleanup = true;
    // �Զ����� token ����ѡ
    options.TokenCleanupInterval = 30;
})
.AddAspNetIdentity<ApplicationUser>()
.AddDeveloperSigningCredential();

#endregion IdentityServer4

//��̨����config��̬��Ԥ�����ֶε����ݿ�
builder.Services.AddHostedService<InitializeDatabaseBackgroundServices>();

var app = builder.Build();
#endregion ConfigureServices

#region Configure

app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseIdentityServer();
app.UseAuthentication();;
app.UseAuthorization();//�ͻ���������Ȩҳ(Consent)����Ҫ�������
app.MapDefaultControllerRoute();
app.Run();

#endregion Configure

