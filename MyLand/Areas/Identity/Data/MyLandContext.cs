using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyLand.Areas.Identity.Data;
using MyLand.Models;

namespace MyLand.Data
{
    public class MyLandContext : IdentityDbContext<MyLandUser>
    {
        public MyLandContext(DbContextOptions<MyLandContext> options)
            : base(options)
        {
        }
        public DbSet<MyLand.Models.Property> Property { get; set; }
        public DbSet<MyLandUser> User { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

            builder.Entity<Property>()
                .HasOne(s => s.User);
            
            builder.Entity<MyLandUser>()
                .HasMany<Property>();
                
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
