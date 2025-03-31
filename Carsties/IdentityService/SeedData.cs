using System.Security.Claims;
using Duende.IdentityModel;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityService;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (userMgr.Users.Any()) return;

            var aldin = userMgr.FindByNameAsync("aldin").Result;
            if (aldin == null)
            {
                aldin = new ApplicationUser
                {
                    UserName = "aldin",
                    Email = "aldinphilo@gmail.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(aldin, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(aldin, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Aldin Philo"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("aldin created");
            }
            else
            {
                Log.Debug("aldin already exists");
            }

            var philo = userMgr.FindByNameAsync("philo").Result;
            if (philo == null)
            {
                philo = new ApplicationUser
                {
                    UserName = "philo",
                    Email = "philoaldin@gmail.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(philo, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(philo, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Philo Aldin"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("philo created");
            }
            else
            {
                Log.Debug("philo already exists");
            }
        }
    }
}
