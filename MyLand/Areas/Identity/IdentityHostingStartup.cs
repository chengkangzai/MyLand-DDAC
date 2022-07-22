using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLand.Areas.Identity.Data;
using MyLand.Data;

[assembly: HostingStartup(typeof(MyLand.Areas.Identity.IdentityHostingStartup))]
namespace MyLand.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<MyLandContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("MyLandContextConnection")).AddXRayInterceptor());

                services.AddDefaultIdentity<MyLandUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<MyLandContext>();
            });
        }
    }
}