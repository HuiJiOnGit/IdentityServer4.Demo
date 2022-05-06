using System.Security.Claims;
using IdentityModel;
using IdentityServer.Empty.Data;
using IdentityServer.Empty.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace IdentityServer.Empty.BackgroundServices
{
    public class InitializeDatabaseBackgroundServices : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InitializeDatabaseBackgroundServices> _logger;

        public InitializeDatabaseBackgroundServices(IServiceProvider serviceProvider, ILogger<InitializeDatabaseBackgroundServices> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"{nameof(InitializeDatabaseBackgroundServices)} star");

            using var scope = _serviceProvider.CreateScope();
            using var _configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            using var appDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            try
            {
                if (!_configurationDbContext.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        _configurationDbContext.Clients.Add(client.ToEntity());
                    }
                }
                if (!_configurationDbContext.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        _configurationDbContext.IdentityResources.Add(resource.ToEntity());
                    }
                }

                if (!_configurationDbContext.ApiScopes.Any())
                {
                    foreach (var resource in Config.ApiScopes)
                    {
                        _configurationDbContext.ApiScopes.Add(resource.ToEntity());
                    }
                }
                await _configurationDbContext.SaveChangesAsync();

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var alice = userMgr.FindByNameAsync("alice").Result;
                if (alice == null)
                {
                    alice = new ApplicationUser
                    {
                        UserName = "alice",
                        Email = "AliceSmith@email.com",
                        EmailConfirmed = true,
                    };
                    var result = await userMgr.CreateAsync(alice, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = await userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        });
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("alice created");
                }
                else
                {
                    Log.Debug("alice already exists");
                }

                var bob = await userMgr.FindByNameAsync("bob");
                if (bob == null)
                {
                    bob = new ApplicationUser
                    {
                        UserName = "bob",
                        Email = "BobSmith@email.com",
                        EmailConfirmed = true
                    };
                    var result = await userMgr.CreateAsync(bob, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = await userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim("location", "somewhere")
                        });
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("bob created");
                }
                else
                {
                    Log.Debug("bob already exists");
                }

                _logger.LogDebug($"{nameof(InitializeDatabaseBackgroundServices)} Execute Success");
            }
            catch
            {
                _logger.LogDebug($"{nameof(InitializeDatabaseBackgroundServices)} Execute Error");
            }
        }
    }
}